using System;
using System.Collections;
using UnityEngine;
using System.Net.Sockets;

namespace VTOLVRTelemetry
{
    public class TelemetryExporter : MonoBehaviour
    {
        VTOLVRData data = new VTOLVRData();
        float startTime;

        public UdpClient udpClient;

        public string receiverIp = "127.0.0.1";

        public int receiverPort = 5640;

        uint packetCounter = 0;

        Vector3 lastRotation = Vector3.zero;
        Vector3 lastVelocity = Vector3.zero;
        Vector3 lastRotVel = Vector3.zero;
        
        // This method is run once, when the Mod Loader is done initialising this game object
        void Start()
        {
            startTime = Time.time;

            udpClient = new UdpClient();
            udpClient.ExclusiveAddressUse = false;
            udpClient.Connect("127.0.0.1", 13371);
        }

        private void FixedUpdate()
        {
            SendTelemetry(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
//            SendTelemetry(Time.deltaTime);
        }

        void SendTelemetry(float deltaTime)
        { 

            //Debug.unityLogger.logEnabled = true;

            Actor playerActor = FlightSceneManager.instance.playerActor;

            if (playerActor == null)
                return;

            GameObject playersVehicleGameObject = playerActor.gameObject;

            Transform planeTransform = playersVehicleGameObject.transform;

            data.packetId = packetCounter;

            if (packetCounter == uint.MaxValue - 1)
                packetCounter = 0;
            else
                packetCounter++;

            Vector3 position = planeTransform.position;
            Quaternion rotation = planeTransform.rotation;

            data.posX = position.x;
            data.posY = position.y;
            data.posZ = position.z;

            Vector3 velocity = playerActor.velocity;
            velocity = planeTransform.InverseTransformDirection(velocity);
            lastVelocity = velocity;

            data.velX = velocity.x;
            data.velY = velocity.y;
            data.velZ = velocity.z;

            Vector3 acceleration = playerActor.flightInfo.acceleration * 0.10197162129779283f; //convert to g accel

            data.accelX = acceleration.x;
            data.accelY = acceleration.y;
            data.accelZ = acceleration.z;

            Vector3 pyr = rotation.eulerAngles * ((float)Mathf.PI / 180.0f);
            data.pitch = pyr.x;
            data.yaw = pyr.y;
            data.roll = pyr.z;

            data.pitchVel = CalculateAngularChange(lastRotation.x, pyr.x) / deltaTime;
            data.yawVel = CalculateAngularChange(lastRotation.y, pyr.y) / deltaTime;
            data.rollVel = CalculateAngularChange(lastRotation.z, pyr.z) / deltaTime;

            lastRotation = pyr;

            data.pitchAccel = (data.pitchVel - lastRotVel.x) / deltaTime;
            data.yawAccel = (data.yawVel - lastRotVel.y) / deltaTime;
            data.rollAccel = (data.rollVel - lastRotVel.z) / deltaTime;

            lastRotVel = new Vector3(data.pitchVel, data.yawVel, data.rollVel);

            ModuleEngine[] engines = playersVehicleGameObject.GetComponentsInChildren<ModuleEngine>();
            float rpm = 0;
            foreach (ModuleEngine moduleEngine in playersVehicleGameObject.GetComponentsInChildren<ModuleEngine>())
            {
                rpm += moduleEngine.displayedRPM;
            }
            rpm /= (float)engines.Length;

            data.paused = false;

            byte[] bytes = data.ToByteArray();

            udpClient.SendAsync(bytes, bytes.Length);
        }

        public static float CalculateAngularChange(float sourceA, float targetA)
        {
            sourceA *= (180.0f / (float)Mathf.PI);
            targetA *= (180.0f / (float)Mathf.PI);

            float a = targetA - sourceA;
            float sign = Mathf.Sign(a);

            a = ((Mathf.Abs(a) + 180) % 360 - 180) * sign;

            return a * ((float)Mathf.PI / 180.0f);
        }

        private void OnDestroy()
        {
            if (udpClient != null)
                udpClient.Close();
        }

        public void OnGUI()
        {
            if (Time.time - startTime > 10)
                return;

            GUI.TextArea(new Rect(0, 0, 300, 50), "!!!!!!!!!!! SPACEMONKEY INJECTED !!!!!!!!!!!");
        }

    }
}