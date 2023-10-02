using System;
using System.Runtime.InteropServices;

namespace OpenMotion
{
    [StructLayout(LayoutKind.Sequential, Pack = 0, CharSet = CharSet.Ansi)]
    public class OpenMotionAPI
    {
		public float m_time = 0;
		public float m_posX = 0; //position x
		public float m_posY = 0; //position y
		public float m_posZ = 0; //position z
		public float m_fwdX = 0; //forward direction X, identity 0 
		public float m_fwdY = 0; //forward direction Y, identity 0
		public float m_fwdZ = 1; //forward direction Z, identity 1
		public float m_upX = 0; //up direction X, identity 0; left handed; right = up cross fwd
		public float m_upY = 1; //up direction Y, identity 1
		public float m_upZ = 0; //up direction Z, identity 0
	}



}
