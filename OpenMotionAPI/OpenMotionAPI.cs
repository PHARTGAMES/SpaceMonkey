using System;
using System.Runtime.InteropServices;

namespace OpenMotion
{
    [StructLayout(LayoutKind.Sequential, Pack = 0, CharSet = CharSet.Ansi)]
    public class OpenMotionAPI
    {
        public float m_time;
        public float m_posX;
        public float m_posY;
        public float m_posZ;
        public float m_rotPitch;
        public float m_rotYaw;
        public float m_rotRoll;

    }



}
