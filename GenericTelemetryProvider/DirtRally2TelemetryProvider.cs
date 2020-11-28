using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Threading;
using System.Diagnostics;
using Sojaner.MemoryScanner;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Net;
using System.Net.Sockets;


namespace GenericTelemetryProvider
{
    public class DirtRally2TelemetryProvider : GenericProviderBase
    {

        private Thread t;
        private int portNum = 20777;
        private IPEndPoint _senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        GenericProviderData filteredData;
        GenericProviderData rawData;
        char[] versionString = new char[] { 'D', 'I', 'R', 'T', 'R', 'L', 'Y', '2' };


        public override void Run()
        {
            base.Run();

            t = new Thread(ThreadUpdate);
            t.Start();

        }

        void ThreadUpdate()
        {
            bool isStopped = false;


            Stopwatch sw = new Stopwatch();
            sw.Start();


            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Any, portNum));


            filteredData = new GenericProviderData(versionString);
            rawData = new GenericProviderData(versionString);

            filteredData.version = versionString;
            rawData.version = versionString;

            float dt = 0.0f;

            while (!isStopped)
            {
                try
                {

                    // get data from game, 
                    if (socket.Available == 0)
                    {
                        if (sw.ElapsedMilliseconds > 500)
                        {
                            Thread.Sleep(1000);
                        }
                        continue;
                    }


                    Byte[] received = socket.Receive(ref _senderIP);

                    var alloc = GCHandle.Alloc(received, GCHandleType.Pinned);
                    DirtRally2UDPTelemetry telemetryData = (DirtRally2UDPTelemetry)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(DirtRally2UDPTelemetry));


                    dt = (float)sw.ElapsedMilliseconds / 1000.0f;
                    sw.Restart();

                    ProcessTelemetry(telemetryData);

                    Thread.Sleep(1000 / 100);
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            socket.Close();

        }

        void ProcessTelemetry(DirtRally2UDPTelemetry telemetryData)
        {

            Vector3 rht = -(new Vector3(telemetryData.left_dir_x, telemetryData.left_dir_y, telemetryData.left_dir_z));
            Vector3 fwd = new Vector3(telemetryData.forward_dir_x, telemetryData.forward_dir_y, telemetryData.forward_dir_z);
            Vector3 up = Vector3.Cross( fwd, rht);

            up = Vector3.Normalize(up);

            float pitch = (float)Math.Asin(-fwd.Y);
            float yaw = (float)Math.Atan2(fwd.X, fwd.Z);

            float roll = 0.0f;
            Vector3 rhtPlane = rht;
            rhtPlane.Y = 0;
            rhtPlane = Vector3.Normalize(rhtPlane);
            if (rhtPlane.Length() <= float.Epsilon)
            {
                roll = -(float)(Math.Sign(rht.Y) * Math.PI * 0.5f);
                //                        Debug.WriteLine( "---Roll = " + roll + " " + Math.Sign( rht.Y ) );
            }
            else
            {
                roll = -(float)Math.Asin(Vector3.Dot(up, rhtPlane));
                //                        Debug.WriteLine( "Roll = " + roll + " " + Math.Sign(rht.Y) );
            }
            //                  Debug.WriteLine( "" );

            rawData.pitch = pitch * (180.0f / (float)Math.PI);
            rawData.yaw = yaw * (180.0f / (float)Math.PI);
            rawData.roll = roll * (180.0f / (float)Math.PI);

            rawData.position_x = telemetryData.position_x;
            rawData.position_y = telemetryData.position_y;
            rawData.position_z = telemetryData.position_z;

            rawData.local_velocity_x = telemetryData.velocity_x;
            rawData.local_velocity_y = telemetryData.velocity_y;
            rawData.local_velocity_z = telemetryData.velocity_z;

            rawData.gforce_lateral = telemetryData.gforce_lateral;
            rawData.gforce_longitudinal = telemetryData.gforce_longitudinal;

            rawData.gforce_vertical = 0;

            rawData.engine_rpm = telemetryData.engine_rate;

            FilterModule.Instance.Filter(rawData, ref filteredData);


            byte[] writeBuffer = filteredData.ToByteArray();

            mutex.WaitOne();

            using (MemoryMappedViewStream stream = filteredMMF.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(writeBuffer);
            }

            writeBuffer = rawData.ToByteArray();
            using (MemoryMappedViewStream stream = rawMMF.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(writeBuffer);
            }

            mutex.ReleaseMutex();

        }

        /*

        bool ProcessTransform(Matrix4x4 transform, float dt)
        {

            Vector3 rht = new Vector3(transform.M11, transform.M12, transform.M13);
            Vector3 up = new Vector3(transform.M21, transform.M22, transform.M23);
            Vector3 fwd = new Vector3(transform.M31, transform.M32, transform.M33);

            //            rht = Vector3.Normalize(rht);
            //            up = Vector3.Normalize(up);
            //            fwd = Vector3.Normalize(fwd);

            float rhtMag = rht.Length();
            float upMag = up.Length();
            float fwdMag = fwd.Length();

            //transform.M11 = rht.X;
            //transform.M12 = rht.Y;
            //transform.M13 = rht.Z;

            //transform.M21 = up.X;
            //transform.M22 = up.Y;
            //transform.M23 = up.Z;

            //transform.M31 = fwd.X;
            //transform.M32 = fwd.Y;
            //transform.M33 = fwd.Z;

            //reading garbage
            if (rhtMag < 0.9f || upMag < 0.9f || fwdMag < 0.9f)
            {
                return false;
            }

            if (!lastFrameValid)
            {
                lastTransform = transform;
                lastFrameValid = true;
                lastVelocity = Vector3.Zero;
                lastWorldVelMag = 0.0f;
                return true;
            }

            dt = dtFilter.Filter(dt);

            if (dt <= 0)
                dt = 0.015f;

            //                     Vector3 currPos = transform.Translation;

            rawData.position_x = transform.M41;
            rawData.position_y = transform.M42;
            rawData.position_z = transform.M43;

            //filter position
            FilterModule.Instance.Filter(rawData, ref filteredData, posKeyMask, true);




            //            Vector3 currPos = Vector3.Lerp(lastTransform.Translation, transform.Translation, Math.Min(1.0f, dt * 4.0f));

            //assign
            Vector3 worldPosition = new Vector3(filteredData.position_x, filteredData.position_y, filteredData.position_z);

            Vector3 worldVelocity = (worldPosition - lastTransform.Translation) / dt;

            transform.Translation = worldPosition;
            lastTransform = transform;

            Matrix4x4 rotation = new Matrix4x4();
            rotation = transform;
            rotation.M41 = 0.0f;
            rotation.M42 = 0.0f;
            rotation.M43 = 0.0f;
            rotation.M44 = 1.0f;

            Matrix4x4 rotInv = new Matrix4x4();
            Matrix4x4.Invert(rotation, out rotInv);

            //transform world velocity to local space
            Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

            rawData.local_velocity_x = localVelocity.X;
            rawData.local_velocity_y = localVelocity.Y;
            rawData.local_velocity_z = localVelocity.Z;

            //filter local velocity
            FilterModule.Instance.Filter(rawData, ref filteredData, velKeyMask, false);

            //assign filtered local velocity
            localVelocity = new Vector3(filteredData.local_velocity_x, filteredData.local_velocity_y, filteredData.local_velocity_z);

            //calculate local acceleration
            Vector3 localAcceleration = ((localVelocity - lastVelocity) / dt) * 0.10197162129779283f; //convert to g accel
            lastVelocity = localVelocity;

            //calculate pitch yaw roll
            float pitch = (float)Math.Asin(-fwd.Y);
            float yaw = (float)Math.Atan2(fwd.X, fwd.Z);

            float roll = 0.0f;
            Vector3 rhtPlane = rht;
            rhtPlane.Y = 0;
            rhtPlane = Vector3.Normalize(rhtPlane);
            if (rhtPlane.Length() <= float.Epsilon)
            {
                roll = -(float)(Math.Sign(rht.Y) * Math.PI * 0.5f);
                //                        Debug.WriteLine( "---Roll = " + roll + " " + Math.Sign( rht.Y ) );
            }
            else
            {
                roll = -(float)Math.Asin(Vector3.Dot(up, rhtPlane));
                //                        Debug.WriteLine( "Roll = " + roll + " " + Math.Sign(rht.Y) );
            }
            //                  Debug.WriteLine( "" );

            rawData.pitch = pitch * (180.0f / (float)Math.PI);
            rawData.yaw = yaw * (180.0f / (float)Math.PI);
            rawData.roll = roll * (180.0f / (float)Math.PI);

            rawData.gforce_lateral = localAcceleration.X;
            rawData.gforce_vertical = localAcceleration.Y;
            rawData.gforce_longitudinal = localAcceleration.Z;

            //finally filter everything else
            FilterModule.Instance.Filter(rawData, ref filteredData, int.MaxValue & ~(posKeyMask | velKeyMask), false);

            //            string debugString = "";
            //            debugString += "yaw: " + telemetryToSend.yaw + "\n";

            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n" + dt + "\n" + rhtMag + "\n" + upMag + "\n" + fwdMag);

            byte[] writeBuffer = filteredData.ToByteArray();

            mutex.WaitOne();

            using (MemoryMappedViewStream stream = filteredMMF.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(writeBuffer);
            }

            writeBuffer = rawData.ToByteArray();
            using (MemoryMappedViewStream stream = rawMMF.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(writeBuffer);
            }


            // //debug
            //using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            //{
            //    BinaryReader reader = new BinaryReader(stream);
            //    byte[] readBuffer = reader.ReadBytes((int)stream.Length);

            //    var alloc = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
            //    GenericProviderData readTelemetry = (GenericProviderData)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(GenericProviderData));
            //}
            
            mutex.ReleaseMutex();

            return true;
        }
        */

    }



}
