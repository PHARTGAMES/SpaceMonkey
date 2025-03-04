#pragma once
#include <string>

#ifdef SPACEMONKEYTELEMETRYAPI_EXPORTS
#define SMT_API __declspec(dllexport)
#else
#define SMT_API __declspec(dllimport)
#endif

#define SPACEMONKEY_TELEMETRY_VERSION 1
#define SPACEMONKEY_TELEMETRY_FILENAME "SMT_FRAME"
#define SPACEMONKEY_TELEMETRY_MUTEX "SMT_FRAME_MUTEX"


#pragma pack( push, 0 )
volatile struct SMT_API SpaceMonkeyTelemetryFrameData
{
	int m_version = 1; //version number
	double m_time = 0; //absolute time, used to calculate frame time delta
	double m_posX = 0; //world position x meters
	double m_posY = 0; //world position y meters
	double m_posZ = 0; //world position z meters
	double m_fwdX = 0; //world forward direction X, identity 0 
	double m_fwdY = 0; //world forward direction Y, identity 0
	double m_fwdZ = 1; //world forward direction Z, identity 1
	double m_upX = 0; //world up direction X, identity 0; left handed; right = up cross fwd
	double m_upY = 1; //world up direction Y, identity 1
	double m_upZ = 0; //world up direction Z, identity 0
	float m_idleRPM = 0; //engine idle rpm
	float m_maxRPM = 0; //engine max rpm
	float m_rpm = 0; //engine rpm
	float m_gear = 0; //gear
	float m_throttleInput = 0; //throttle 0 to 1
	float m_brakeInput = 0; //brake 0 to 1
	float m_steeringInput = 0; //steering input -1 to 1
	float m_clutchInput = 0; //clutch input 0 to 1

};
#pragma pack(pop)


class SMT_API SpaceMonkeyTelemetryAPI
{
public:

	virtual void InitSendSharedMemory() = 0;
	virtual void InitRecieveSharedMemory() = 0;
	virtual void SendFrame(SpaceMonkeyTelemetryFrameData* a_frame) = 0;
	virtual void RecieveFrame(SpaceMonkeyTelemetryFrameData* a_frame) = 0;
	virtual void Deinit() = 0;

};

#ifdef __cplusplus
extern "C" {
#endif

SMT_API SpaceMonkeyTelemetryAPI* SpaceMonkeyTelemetryAPI_Create();

#ifdef __cplusplus
}
#endif
