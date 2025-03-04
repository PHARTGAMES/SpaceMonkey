#pragma once
#include "SpaceMonkeyTelemetryAPI.h"
#include "SharedMemory.h"

class SMT_API SpaceMonkeyTelemetryAPIImpl : public SpaceMonkeyTelemetryAPI
{
protected:
	SharedMemory *m_sharedMem;
	SpaceMonkeyTelemetryFrameData m_frameData;

public:
	SpaceMonkeyTelemetryAPIImpl() {}
	void InitSendSharedMemory() override;
	void InitRecieveSharedMemory() override;
	void SendFrame(SpaceMonkeyTelemetryFrameData* a_frame) override;
	void RecieveFrame(SpaceMonkeyTelemetryFrameData* a_frame) override;
	void Deinit() override;

};

