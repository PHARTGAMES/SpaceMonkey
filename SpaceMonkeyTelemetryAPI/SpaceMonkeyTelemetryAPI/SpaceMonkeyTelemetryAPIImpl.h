#pragma once
#include "SpaceMonkeyTelemetryAPI.h"
#include "SharedMemory.h"
#include <string>
#include "CMCustomUDPData.h"

class SMT_API SpaceMonkeyTelemetryAPIImpl : public SpaceMonkeyTelemetryAPI
{
protected:
	std::string m_installDir;
	SharedMemory *m_sharedMem;
	CMCustomUDPData m_frameData;
	uint8_t* m_packet = nullptr;
	int m_packetSize = 0;
	bool m_ownsPacket = true;


public:
	SpaceMonkeyTelemetryAPIImpl() {}
	CMCustomUDPData* InitSendSharedMemory(const std::string& a_packetFormatPath) override;
	CMCustomUDPData* InitRecieveSharedMemory(const std::string& a_packetFormatPath) override;
	void SendFrame() override;
	void RecieveFrame() override;
	void Deinit() override;
	uint8_t* GetPacket() override;
	void SetPacket(uint8_t*) override;




};

