#include <CmdMessenger.h>
#include <XInput.h>

CmdMessenger cmdMessenger = CmdMessenger(Serial1);

enum
{
  kInputCommand, // input command
};

void attachCommandCallbacks()
{
  cmdMessenger.attach(kInputCommand, OnCommand);
}

void OnCommand()
{
  int16_t inputControl = cmdMessenger.readInt16Arg(); //read the input id, these map to the XInputControl enum

  switch(inputControl)
  {
    case BUTTON_LOGO:
    case BUTTON_A:
    case BUTTON_B:
    case BUTTON_X:
    case BUTTON_Y:
    case BUTTON_LB:
    case BUTTON_RB:
    case BUTTON_BACK:
    case BUTTON_START:
    case BUTTON_L3:
    case BUTTON_R3:
    {
      bool inputValue = cmdMessenger.readBoolArg(); //read the value for the input
      XInput.setButton(inputControl, inputValue);

//      digitalWrite(13, inputValue?HIGH:LOW);

      break;
    }

    case DPAD_UP:
    case DPAD_DOWN:
    case DPAD_LEFT:
    case DPAD_RIGHT:
    {
      bool inputValue = cmdMessenger.readBoolArg(); //read the value for the input
      XInput.setDpad(inputControl, inputValue);
      break;
    }

    case TRIGGER_LEFT:
    case TRIGGER_RIGHT:
    {
      int32_t inputValue = cmdMessenger.readInt32Arg(); //read the value for the input
      XInput.setTrigger(inputControl, inputValue);
      break;
    }

    case JOY_LEFT:
    case JOY_RIGHT:
    {
      int32_t inputXAxis = cmdMessenger.readInt32Arg(); //read the value for the X axis
      int32_t inputYAxis = cmdMessenger.readInt32Arg(); //read the value for the Y axis
      XInput.setJoystick(inputControl, inputXAxis, inputYAxis);
      break;
    }

    default:
      break;
  }

}

void setup() 
{
	XInput.begin();

  // Listen on serial connection for messages from the PC
  // 115200 is the max speed on Arduino Uno, Mega, with AT8u2 USB
  // Use 57600 for the Arduino Duemilanove and others with FTDI Serial
  Serial1.begin(57600); 

  // Adds newline to every command
  //cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

//  pinMode(13, OUTPUT);

}

void loop() 
{
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();

	// XInput.press(BUTTON_A);
	// delay(1000);

	// XInput.release(BUTTON_A);
	// delay(1000);
}
