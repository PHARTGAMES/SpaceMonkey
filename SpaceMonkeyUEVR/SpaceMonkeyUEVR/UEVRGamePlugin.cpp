#include "UEVRGamePlugin.h"
#include "SpaceMonkeyTelemetryAPI.h"


UEVRGamePlugin::UEVRGamePlugin(UEVRGameConfig* a_game_config)
{
	m_telemetryAPI = SpaceMonkeyTelemetryAPI_Create();
	if (m_telemetryAPI != nullptr)
	{
		m_telemetryAPI->InitSendSharedMemory();
	}
}

UEVRGamePlugin::~UEVRGamePlugin()
{
	if (m_telemetryAPI != nullptr)
	{
		m_telemetryAPI->Deinit();
		delete m_telemetryAPI;
		m_telemetryAPI = nullptr;
	}

}