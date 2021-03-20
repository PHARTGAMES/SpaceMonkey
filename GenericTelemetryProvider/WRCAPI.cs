using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace WRCAPI
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct WRCWheelData
    {
        public float pos1X; 
        public float pos1Y;
        public float pos1Z; //higher than pos2, Z is UP!
        public float f1;
        public float f2;
        public float f3;
        public float f4;
        public float f5;
        public float f6;
        public float f7;
        public float f8;
        public float f9;
        public float f10;
        public float f11;
        public float f12;
        public float pos2X;
        public float pos2Y;
        public float pos2Z;
        public float f13;
        public float f14;
        public float f15;
        public float f16;
        public float f17;
        public float f18;
        public float f19;
        public float fBigValue; //3310 etc???
        public float orientA; // could be a quat but there's 5 values here....
        public float orientB;
        public float orientC;
        public float orientD;
        public float orientE; // last one is something else?
        public float f25;
        public float f26;
        public float f27;
        public float f28;
        public float f29;
        public float wheelSpeed;
        public float f31;
        public float f32;
        public float f33;


    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class WRCWheelGroupData
    {
        public WRCWheelData wheelFL;
        public WRCWheelData wheelFR;
        public WRCWheelData wheelRL;
        public WRCWheelData wheelRR;
    }
}
