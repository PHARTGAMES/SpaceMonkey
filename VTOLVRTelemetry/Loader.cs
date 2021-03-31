using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VTOLVRTelemetry
{
    public class Loader
    {
        public static void Init()
        {
            Loader.Load = new GameObject();
            Loader.Load.AddComponent<TelemetryExporter>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }
        public static void Unload()
        {
            _Unload();
        }
        private static void _Unload()
        {
            GameObject.Destroy(Loader.Load);
        }
        private static GameObject Load;
    }
}
