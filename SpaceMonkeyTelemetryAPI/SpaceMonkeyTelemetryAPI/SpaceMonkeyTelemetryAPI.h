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

class CMCustomUDPData;

class SMT_API SpaceMonkeyTelemetryAPI
{
public:

	virtual CMCustomUDPData* InitSendSharedMemory(const std::string &a_packetFormatPath) = 0;
	virtual CMCustomUDPData* InitRecieveSharedMemory(const std::string &a_packetFormatPath) = 0;
	virtual void SendFrame() = 0;
	virtual void RecieveFrame() = 0;
	virtual void Deinit() = 0;
	static std::string GetInstallDir();
	virtual uint8_t* GetPacket() = 0;
	virtual void SetPacket(uint8_t*) = 0;


};

#ifdef __cplusplus
extern "C" {
#endif

SMT_API SpaceMonkeyTelemetryAPI* SpaceMonkeyTelemetryAPI_Create();

#ifdef __cplusplus
}
#endif
