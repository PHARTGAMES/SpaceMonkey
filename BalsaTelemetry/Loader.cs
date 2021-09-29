using BalsaCore;
using UnityEngine;

namespace BalsaTelemetry
{
    [BalsaAddon]
    public class Loader
    {

        private static bool firstLoad = true;

        [BalsaAddonInit]
        public static void BalsaInit()
        {
            Loader.Load = new GameObject();
            Loader.Load.AddComponent<TelemetryExporter>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }

        //Main menu load
        public static void MenuLoad()
        {
            if (firstLoad)
            {
                firstLoad = false;
                //Main menu code
            }
        }

        //Game exit
        [BalsaAddonFinalize]
        public static void BalsaFinalize()
        {
            Load.DestroyGameObject();
        }

        private static GameObject Load;
    }
}
