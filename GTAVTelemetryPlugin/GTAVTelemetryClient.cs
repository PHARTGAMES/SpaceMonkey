using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;
using GTAVAPI;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Diagnostics;


namespace GTAVTelemetryPlugin
{
    public class GTAVTelemetryClient : Script
    {
        GTAVData gtaData = new GTAVData();
        protected Mutex mmfMutex;
        protected MemoryMappedFile mmf;
        Stopwatch sw = new Stopwatch();


        public GTAVTelemetryClient()
        {
            Tick += OnTick;
            Aborted += OnAborted;
            Interval = 0;// 10;

            mmfMutex = new Mutex(false, "GTADataMMFMutex");
            mmf = MemoryMappedFile.CreateNew("GTADataMMF", 10000);
            sw.Start();
        }


        void OnTick(object sender, EventArgs e)
        {
            Ped player = Game.Player.Character;

            Matrix ltw = Matrix.Identity;

            Vector3 vel = Vector3.Zero;
            // Player in vehicle
            if (player.IsInVehicle())
            {
                Vehicle vehicle = player.CurrentVehicle;

                ltw = vehicle.Matrix;

                vel = vehicle.Velocity;
                gtaData.engineRPM = vehicle.CurrentRPM;
                gtaData.gear = vehicle.CurrentGear;
                gtaData.gears = vehicle.Gears;
            }
            else //walkies
            {
                ltw = player.Matrix;
                //vel = player.Velocity;
                vel = Vector3.Zero;

                gtaData.engineRPM = 0;
                gtaData.gear = 0;
                gtaData.gears = 0;
            }

            gtaData.inVehicle = player.IsInVehicle();
            gtaData.paused = Game.IsPaused;
            gtaData.m11 = ltw.M11;
            gtaData.m12 = ltw.M12;
            gtaData.m13 = ltw.M13;
            gtaData.m14 = ltw.M14;
            gtaData.m21 = ltw.M21;
            gtaData.m22 = ltw.M22;
            gtaData.m23 = ltw.M23;
            gtaData.m24 = ltw.M24;
            gtaData.m31 = ltw.M31;
            gtaData.m32 = ltw.M32;
            gtaData.m33 = ltw.M33;
            gtaData.m34 = ltw.M34;
            gtaData.m41 = ltw.M41;
            gtaData.m42 = ltw.M42;
            gtaData.m43 = ltw.M43;
            gtaData.m44 = ltw.M44;

            gtaData.velX = vel.X;
            gtaData.velY = vel.Y;
            gtaData.velZ = vel.Z;

            gtaData.dt = (float)sw.ElapsedMilliseconds / 1000.0f;
            sw.Restart();

            //write to mmf
            byte[] bytes = gtaData.ToByteArray();
            mmfMutex.WaitOne();
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytes);
            }
            mmfMutex.ReleaseMutex();
        }

        void OnAborted(object sender, EventArgs e)
        {

            if (mmf != null)
                mmf.Dispose();

            mmf = null;
        }

    }
}
