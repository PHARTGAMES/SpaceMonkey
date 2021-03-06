using UnityEngine;
using PhysType;
using MonsterGamesAPI;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Text;

namespace MonsterGamesTelemetry
{
    class TelemetryExporter : MonoBehaviour
    {
        MonsterGamesData data = new MonsterGamesData();
        float startTime;

        private UdpClient udpClient;

        public void Start()
        {
            startTime = Time.time;

            udpClient = new UdpClient();
            udpClient.Connect("127.0.0.1", 13371);
        }

        public void Update()
        {

            LCarPacket carPacket = GetCarPacket(0);

            if (carPacket != null)
            {
                data.paused = MGIPauseController.GetGlobal().IsPaused();

                Vector3 position = carPacket.rigidbody_f.pos.Convert();
                Quaternion rotation = carPacket.rigidbody_f.q.Convert();

                Matrix4x4 ltw = new Matrix4x4();

                ltw.SetTRS(position, rotation, Vector3.one);

                data.m11 = ltw.m00;
                data.m12 = ltw.m10;
                data.m13 = ltw.m20;
                data.m14 = ltw.m30;

                data.m21 = ltw.m01;
                data.m22 = ltw.m11;
                data.m23 = ltw.m21;
                data.m24 = ltw.m31;

                data.m31 = ltw.m02;
                data.m32 = ltw.m12;
                data.m33 = ltw.m22;
                data.m34 = ltw.m32;

                data.m41 = ltw.m03;
                data.m42 = ltw.m13;
                data.m43 = ltw.m23;
                data.m44 = ltw.m33;

                data.dt = Time.deltaTime;

                data.gears = 6;
                data.gear = carPacket.gear;

                data.engineRPM = carPacket.rpm;

                string output = JsonConvert.SerializeObject(data, Formatting.Indented);
                byte[] bytes = Encoding.UTF8.GetBytes(output);

                udpClient.Send(bytes, bytes.Length);
            }


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

            GUI.TextArea(new Rect(0, 0, 300, 50), "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!! SPACEMONKEY INJECTED !!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n");
        }

        protected static LCarPacket GetCarPacket(int carIdx)
        {
            LCarPacket result = null;

            MGIGameMaster mgigameMaster = MGIGameMaster.TryGlobal();
            if (mgigameMaster != null && mgigameMaster.HasPhysicsStarted() && mgigameMaster.IsFocusCar(carIdx))
            {
                int focusCar = MGIGameMaster.GetGlobal().GetFocusCar(carIdx);
                result = MGIProviderManager.GetGlobal().GetPhysXProvider().GetCarPacket(focusCar);
            }

            return result;
        }

    }
}