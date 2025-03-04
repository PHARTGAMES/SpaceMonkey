#include "pch.h"
#include "SpaceMonkeyTelemetryAPIImpl.h"



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

#ifdef __cplusplus
extern "C" {
#endif

	// Factory function to create an instance of the API.
	SMT_API SpaceMonkeyTelemetryAPI* SpaceMonkeyTelemetryAPI_Create() 
	{
		return new SpaceMonkeyTelemetryAPIImpl();
	}

	// Function to destroy an instance of the API.
	SMT_API void SpaceMonkeyTelemetryAPI_Destroy(SpaceMonkeyTelemetryAPI* instance) 
	{
		delete instance;
	}

	// Wrapper function for InitSendSharedMemory.
	SMT_API void SpaceMonkeyTelemetryAPI_InitSendSharedMemory(SpaceMonkeyTelemetryAPI* instance) 
	{
		if (instance) 
		{
			instance->InitSendSharedMemory();
		}
	}

	// Wrapper function for InitRecieveSharedMemory.
	SMT_API void SpaceMonkeyTelemetryAPI_InitRecieveSharedMemory(SpaceMonkeyTelemetryAPI* instance) 
	{
		if (instance) 
		{
			instance->InitRecieveSharedMemory();
		}
	}

	// Wrapper function for SendFrame.
	SMT_API void SpaceMonkeyTelemetryAPI_SendFrame(SpaceMonkeyTelemetryAPI* instance, SpaceMonkeyTelemetryFrameData* a_frame) 
	{
		if (instance && a_frame) 
		{
			instance->SendFrame(a_frame);
		}
	}

	// Wrapper function for RecieveFrame.
	SMT_API void SpaceMonkeyTelemetryAPI_RecieveFrame(SpaceMonkeyTelemetryAPI* instance, SpaceMonkeyTelemetryFrameData* a_frame) 
	{
		if (instance && a_frame) 
		{
			instance->RecieveFrame(a_frame);
		}
	}

	// Wrapper function for Deinit.
	SMT_API void SpaceMonkeyTelemetryAPI_Deinit(SpaceMonkeyTelemetryAPI* instance) 
	{
		if (instance) 
		{
			instance->Deinit();
		}
	}

#ifdef __cplusplus
}
#endif