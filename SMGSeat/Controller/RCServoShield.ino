
#define VERSION "00.00.01"


#include <CmdMessenger.h>
#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>

// number of motors in the sim
const int MOTOR_COUNT = 6;

// millis till the motors should stop if no data is received
const unsigned int UPDATE_TIMEOUT_IN_MS = 30000;

// repeat time of the main loop
const unsigned int LOOP_IN_MS = 10;

// called this way, it uses the default address 0x40
Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver();
// you can also call it with a different address you want
//Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver(0x41);
// you can also call it with a different address and I2C interface
//Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver(0x40, Wire);


// Depending on your servo make, the pulse width min and max may vary, you 
// want these to be as small/large as possible without hitting the hard stop
// for max range. You'll have to tweak them as necessary to match the servos you
// have!
#define SERVOMIN  70 // This is the 'minimum' pulse length count (out of 4096)
#define SERVOMAX  520 // This is the 'maximum' pulse length count (out of 4096)
#define USMIN  500 // This is the rounded 'minimum' microsecond length based on the minimum pulse of 150
#define USMAX  2500 // This is the rounded 'maximum' microsecond length based on the maximum pulse of 600
#define SERVO_FREQ 50 // Analog servos run at ~50 Hz updates


bool isStopped = true;

volatile int position[6] = {0, 0, 0, 0, 0, 0};

unsigned long lastLoopTimeMS = 0;
unsigned long lastPositionUpdateTimeMS = 0;
volatile int32_t lastPositionPacketCount = 0;

// **************************************************************************
// Serial Line Manager API Setup
// **************************************************************************
// Commands
enum
{
  kAcknowledge      = 0,
  kError            = 1,
  kMessage          = 2,
  kSetPosition      = 3,
  kSetStandby       = 6,
  kSetStop          = 7,
  kArduinoReady     = 10,
};

CmdMessenger cmdMessenger = CmdMessenger(Serial);

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kSetPosition, OnSetPosition);
  cmdMessenger.attach(kSetStandby, OnStandby);
  cmdMessenger.attach(kSetStop, OnStop);
  cmdMessenger.attach(kArduinoReady, OnArduinoReady);  
}

// ***********************************************************
// *                    Callbacks                            *
// ***********************************************************

void OnArduinoReady()
{
  cmdMessenger.sendCmd(kArduinoReady,"Arduino ready");
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError, "Command without attached callback");
}

void OnSetPosition()
{
  lastPositionUpdateTimeMS = millis();

  int32_t PositionPacketCount = cmdMessenger.readInt32Arg();
  if(PositionPacketCount != lastPositionPacketCount+1) {
    cmdMessenger.sendCmd(kMessage, "PositionPacketCount");
  }
  lastPositionPacketCount = PositionPacketCount;
  
  position[0] = cmdMessenger.readInt16Arg();
  position[1] = cmdMessenger.readInt16Arg();
  position[2] = cmdMessenger.readInt16Arg();
  position[3] = cmdMessenger.readInt16Arg();
  position[4] = cmdMessenger.readInt16Arg();
  position[5] = cmdMessenger.readInt16Arg();

}


// stop all motors
void OnStop()
{
  if(!isStopped) {
    for (int i = 0; i < MOTOR_COUNT; i++) {
      position[i] = 0;
      driveMotors();
    }
    isStopped = true;
    //delay(1000);
  }
  cmdMessenger.sendCmd(kSetStop,"Arduino stopped");
}


void OnStart() {
//    if(isStopped) {
      isStopped = false;
//    }

    for (int i = 0; i < MOTOR_COUNT; i++) {
      position[i] = 0;
    }

    driveMotors();
    
    delay(500);
}

void OnStandby()
{
  for (int i = 0; i < MOTOR_COUNT; i++) {
    position[i] = 0;
  }
}


// **************************************************************************
// Arduino methods
// **************************************************************************

void setup()
{
  Serial.begin(115200);
    
  while (!Serial) {
    Serial.println("Serial line error, please check the serial conections and settings");
    delay(200); // will pause Arduino until serial console opens
  }      

  // Attach my application's user-defined callback methods
  Serial.println("Attaching command callbacks");
  attachCommandCallbacks();
  
  cmdMessenger.printLfCr();
  
  Serial.print("Starting RCServoShield ");
  Serial.println(VERSION);
  Serial.println();
  Serial.println("Servos configured");  

//  OnStart();

  pwm.begin();
  /*
   * In theory the internal oscillator (clock) is 25MHz but it really isn't
   * that precise. You can 'calibrate' this by tweaking this number until
   * you get the PWM update frequency you're expecting!
   * The int.osc. for the PCA9685 chip is a range between about 23-27MHz and
   * is used for calculating things like writeMicroseconds()
   * Analog servos run at ~50 Hz updates, It is importaint to use an
   * oscilloscope in setting the int.osc frequency for the I2C PCA9685 chip.
   * 1) Attach the oscilloscope to one of the PWM signal pins and ground on
   *    the I2C PCA9685 chip you are setting the value for.
   * 2) Adjust setOscillatorFrequency() until the PWM update frequency is the
   *    expected value (50Hz for most ESCs)
   * Setting the value here is specific to each individual I2C PCA9685 chip and
   * affects the calculations for the PWM update frequency. 
   * Failure to correctly set the int.osc value will cause unexpected PWM results
   */
  pwm.setOscillatorFrequency(27000000);
  pwm.setPWMFreq(SERVO_FREQ);  // Analog servos run at ~50 Hz updates
  
  Serial.println("Setup finished");
  Serial.println("Starting main loop");    
}


void update() {
  
  unsigned long now = millis();
  unsigned int loopTime = now - lastLoopTimeMS;

  if(now - lastPositionUpdateTimeMS >  UPDATE_TIMEOUT_IN_MS) 
  {
    OnStandby();
  }

  if(now - lastPositionUpdateTimeMS >  UPDATE_TIMEOUT_IN_MS + 1000) 
  {
    OnStop();
  } 
  else 
  {
    if(isStopped)
      OnStart();
  }

  if (loopTime < LOOP_IN_MS) {
    delay(LOOP_IN_MS - loopTime);       // wait for a constant loop timing
  }

  // we do need a constant loop time for PID calculations, so set the start
  lastLoopTimeMS = millis();

}

void driveMotors() 
{
  
  for(int t = 0; t < 6; t++)
  {
    for(int i = 0; i < 6; i++)
    {
      int index = (t * 6) + i;
//      pwm.writeMicroseconds(index, map(position[t], 0, 180, USMIN, USMAX));
      pwm.setPWM(index, 0, map(position[t], 0, 1000, SERVOMIN, SERVOMAX));
    }
  }
  

}
int testPos = SERVOMIN;//USMIN;
int moveDir = 1;
void loop()
{
  
  while(1) {
    cmdMessenger.feedinSerialData();
    if(!isStopped) {
      driveMotors();
    } else {
      delay(1000);
    }
    update();
  }
  
/*
  while(1)
  {

    testPos += 1 * moveDir;
    
    if(testPos > SERVOMAX)
    {
      moveDir = -1;
      testPos = SERVOMAX;
    }
    else
    if(testPos < SERVOMIN)
    {
      moveDir = 1;
      testPos = SERVOMIN;
    }
    pwm.setPWM(0, 0, testPos);
  
    delay(LOOP_IN_MS);
  }
  */
}
