using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericTelemetryProvider
{
    class InputModule
    {
        public static InputModule Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputModule();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }
        static InputModule instance;

        public XInputController controller;


        public InputModule()
        {
            controller = new XInputController();
        }

        public void Update()
        {
            controller.Update();
        }

    }
}
