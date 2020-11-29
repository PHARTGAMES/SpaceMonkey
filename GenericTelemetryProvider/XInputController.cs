using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using SharpDX.XInput;
using System.Numerics;
using XInputDotNetPure;

namespace GenericTelemetryProvider
{
    class XInputController
    {
        public Vector2 leftThumb, rightThumb = new Vector2(0, 0);
        public float leftTrigger, rightTrigger;
        PlayerIndex playerIndex = PlayerIndex.One;
        public XInputController()
        {

            for(int i = 0; i < 4; ++i)
            {
                GamePadState gamePadState = GamePad.GetState((PlayerIndex)i);

                if(gamePadState.IsConnected == true)
                {
                    playerIndex = (PlayerIndex)i;
                    break;
                }


            }

        }

        public void Update()
        {
            GamePadState gamePadState = GamePad.GetState(playerIndex);

//            leftThumb.X = (float)gamePadState.ThumbSticks.Left.X / ((float)short.MaxValue * 3.0f);
//            leftThumb.Y = (float)gamePadState.ThumbSticks.Left.Y / ((float)short.MaxValue * 3.0f);
//            rightThumb.X = (float)gamePadState.ThumbSticks.Right.X / ((float)short.MaxValue * 3.0f);
 //           rightThumb.Y = (float)gamePadState.ThumbSticks.Right.Y / ((float)short.MaxValue * 3.0f);

            leftThumb.X = gamePadState.ThumbSticks.Left.X;
            leftThumb.Y = gamePadState.ThumbSticks.Left.Y;
            rightThumb.X = gamePadState.ThumbSticks.Right.X;
            rightThumb.Y = gamePadState.ThumbSticks.Right.Y;


            rightTrigger = gamePadState.Triggers.Right;
            leftTrigger = gamePadState.Triggers.Left;
        }
    }
}
