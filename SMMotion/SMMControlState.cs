using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMMotion
{

    public class SMMControlState
    {
        public float[] actuatorPositions;

        public float Actuator0
        {
            get
            {
                if(actuatorPositions.Length < 1)
                    return 0;

                return actuatorPositions[0];
            }
        }

        public float Actuator1
        {
            get
            {
                if (actuatorPositions.Length < 2)
                    return 0;

                return actuatorPositions[1];
            }
        }

        public float Actuator2
        {
            get
            {
                if (actuatorPositions.Length < 3)
                    return 0;

                return actuatorPositions[2];
            }
        }

        public float Actuator3
        {
            get
            {
                if (actuatorPositions.Length < 4)
                    return 0;

                return actuatorPositions[3];
            }
        }






    }
}
