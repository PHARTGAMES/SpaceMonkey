﻿using UnityEngine;
using System.Net.Sockets;
using Falcon.Game2;
using Falcon.UniversalAircraft;
using Falcon.World;

namespace TCATelemetry
{
    class TelemetryExporter : MonoBehaviour
    {
        TCAData data = new TCAData();
        float startTime;
        uint packetCounter = 0;

        private UdpClient udpClient;
        Vector3 lastVelocity = Vector3.zero;
        Vector3 lastRotation = Vector3.zero;
        Vector3 lastRotVel = Vector3.zero;
        Vector3 lastPosition = Vector3.zero;
        

        public void Start()
        {
            startTime = Time.time;

            udpClient = new UdpClient();
            udpClient.ExclusiveAddressUse = false;
            udpClient.Connect("127.0.0.1", 13371);
        }

        public void FixedUpdate()
        { 
            //            Debug.unityLogger.logEnabled = true;

            float deltaTime = Time.fixedDeltaTime;

            UniAircraft playerAircraft = FlightGame.Instance.PlayerAircraft;

            if (playerAircraft == null)
                return;

            Transform planeTransform = playerAircraft.transform;


            Vector3 position = Vector3.zero;// planeTransform.position;
            Quaternion rotation = planeTransform.rotation;

            //float moveMag = (position - lastPosition).magnitude;

            //if(moveMag >= FloatingOrigin.Instance.Threshold)
            //{
            //    lastPosition = -(lastPosition.normalized) * FloatingOrigin.Instance.Threshold;
            //}

            data.packetId = packetCounter;

            if (packetCounter == uint.MaxValue-1)
                packetCounter = 0;
            else
                packetCounter++;


            Vector3 velocity = playerAircraft.Rigidbody.velocity;// (position - lastPosition) / deltaTime;
            lastPosition = position;

            velocity = planeTransform.InverseTransformDirection(velocity);
            //velocity = playerAircraft.FlightStats.LocalVelocity;

            Vector3 acceleration = ((velocity - lastVelocity) / deltaTime) * 0.10197162129779283f;
            lastVelocity = velocity;

            data.posX = position.x;
            data.posY = position.y;
            data.posZ = position.z;

            Vector3 pyr = rotation.eulerAngles * ((float)Mathf.PI / 180.0f);
            data.pitch = pyr.x;
            data.yaw = pyr.y;
            data.roll = pyr.z;

            data.velX = velocity.x;
            data.velY = velocity.y;
            data.velZ = velocity.z;

            data.accelX = acceleration.x;
            data.accelY = acceleration.y;
            data.accelZ = acceleration.z;

            data.pitchVel = CalculateAngularChange(lastRotation.x, pyr.x) / deltaTime;
            data.yawVel = CalculateAngularChange(lastRotation.y, pyr.y) / deltaTime;
            data.rollVel = CalculateAngularChange(lastRotation.z, pyr.z) / deltaTime;

            lastRotation = pyr;

            data.pitchAccel = (data.pitchVel - lastRotVel.x) / deltaTime;
            data.yawAccel = (data.yawVel - lastRotVel.y) / deltaTime;
            data.rollAccel = (data.rollVel - lastRotVel.z) / deltaTime;

            lastRotVel = new Vector3(data.pitchVel, data.yawVel, data.rollVel);

            data.paused = false;

            data.dt = deltaTime;

            data.engineRPM = (700.0f + (playerAircraft.FlightControls.Throttle * 5000.0f));


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