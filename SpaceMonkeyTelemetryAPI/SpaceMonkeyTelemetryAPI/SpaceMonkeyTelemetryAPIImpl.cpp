#include "pch.h"
#include "SpaceMonkeyTelemetryAPIImpl.h"


SpaceMonkeyTelemetryAPI* SpaceMonkeyTelemetryAPI_Create()
{
	return new SpaceMonkeyTelemetryAPIImpl();
}

void SpaceMonkeyTelemetryAPIImpl::InitSendSharedMemory()
{
	if (m_sharedMem == NULL)
	{
		m_sharedMem = new SharedMemory(SPACEMONKEY_TELEMETRY_FILENAME, SPACEMONKEY_TELEMETRY_MUTEX, SharedMemType::SharedMem_Write, (void*)&m_frameData, sizeof(m_frameData));
	}
}

void SpaceMonkeyTelemetryAPIImpl::InitRecieveSharedMemory()
{
	if (m_sharedMem == NULL)
	{
		m_sharedMem = new SharedMemory(SPACEMONKEY_TELEMETRY_FILENAME, SPACEMONKEY_TELEMETRY_MUTEX, SharedMemType::SharedMem_Read, (void*)&m_frameData, sizeof(m_frameData));
	}
}

void SpaceMonkeyTelemetryAPIImpl::SendFrame(SpaceMonkeyTelemetryFrameData* a_frame)
{
	if (m_sharedMem == nullptr)
		return;

	memcpy(&m_frameData, a_frame, sizeof(SpaceMonkeyTelemetryFrameData));
	m_frameData.m_version = SPACEMONKEY_TELEMETRY_VERSION;

	m_sharedMem->Write();
}

void SpaceMonkeyTelemetryAPIImpl::RecieveFrame(SpaceMonkeyTelemetryFrameData* a_frame)
{
	if (m_sharedMem == nullptr)
		return;

	m_sharedMem->Read();

	memcpy(a_frame, &m_frameData, sizeof(SpaceMonkeyTelemetryFrameData));
}

void SpaceMonkeyTelemetryAPIImpl::Deinit()
{
	if (m_sharedMem != nullptr)
	{
		m_sharedMem->Destroy();
		delete m_sharedMem;
		m_sharedMem = nullptr;
	}
}

