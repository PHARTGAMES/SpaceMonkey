using System;
using System.Runtime.InteropServices;

namespace OpenMotion
{
    [StructLayout(LayoutKind.Sequential, Pack = 0, CharSet = CharSet.Ansi)]
    public class OpenMotionAPI
    {
		public float m_time = 0; //absolute time, used to calculate frame time delta
		public float m_posX = 0; //world position x meters
		public float m_posY = 0; //world position y meters
		public float m_posZ = 0; //world position z meters
		public float m_fwdX = 0; //world forward direction X, identity 0 
		public float m_fwdY = 0; //world forward direction Y, identity 0
		public float m_fwdZ = 1; //world forward direction Z, identity 1
		public float m_upX = 0; //world up direction X, identity 0; left handed; right = up cross fwd
		public float m_upY = 1; //world up direction Y, identity 1
		public float m_upZ = 0; //world up direction Z, identity 0
        public float m_idleRPM = 0; //engine idle rpm
        public float m_maxRPM = 0; //engine max rpm
        public float m_rpm = 0; //engine rpm
        public float m_gear = 0; //gear
        public float m_throttleInput = 0; //throttle 0 to 1
        public float m_brakeInput = 0; //brake 0 to 1
        public float m_steeringInput = 0; //steering input -1 to 1
        public float m_clutchInput = 0; //clutch input 0 to 1
    }



}
