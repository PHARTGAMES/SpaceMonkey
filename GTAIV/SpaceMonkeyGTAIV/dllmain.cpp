#include "pch.h"
#include "SpaceMonkeyTelemetryAPI.h"
#include "systemtime.h"
#include "IVSDK.cpp"

SpaceMonkeyTelemetryAPI* m_telemetryAPI = nullptr;
SpaceMonkeyTelemetryFrameData m_frameData{};
double m_systemTime = 0.0;
double m_lastSampleTime = 0.0;
double m_sampleRate = 1.0 / 60.0;

// every frame while in-game
void SpaceMonkeyLoop()
{

	if (m_telemetryAPI)
	{
//		m_systemTime += CTimer::ms_fTimeStep; //out of sync with camera event
		m_systemTime = SystemTime::GetInSeconds(); //use SystemTime for camera event.
		double timeNow = m_systemTime;

		if (m_lastSampleTime == 0.0f)
		{
			m_lastSampleTime = timeNow;
			return;
		}

		double timeDelta = timeNow - m_lastSampleTime;

		if(timeDelta >= m_sampleRate)
		{
			m_lastSampleTime = timeNow;

			CMatrix* telemetryMatrix = nullptr;
			CVehicle* vehicle = FindPlayerVehicle();

			if (vehicle != nullptr)
			{
				telemetryMatrix = vehicle->m_pMatrix;
			}
			else
			{
				CPed* ped = FindPlayerPed();
				if (ped != nullptr)
				{
					telemetryMatrix = ped->m_pMatrix;
				}
			}

			if (telemetryMatrix != nullptr)
			{
				memset(&m_frameData, 0, sizeof(SpaceMonkeyTelemetryFrameData));

				m_frameData.m_time = timeNow;
				float worldScale = 1.0f;
				m_frameData.m_posX = telemetryMatrix->pos.x * worldScale;
				m_frameData.m_posY = telemetryMatrix->pos.z * worldScale;
				m_frameData.m_posZ = telemetryMatrix->pos.y * worldScale;

				m_frameData.m_fwdX = telemetryMatrix->up.x;
				m_frameData.m_fwdY = telemetryMatrix->up.z;
				m_frameData.m_fwdZ = telemetryMatrix->up.y;

				m_frameData.m_upX = telemetryMatrix->at.x;
				m_frameData.m_upY = telemetryMatrix->at.z;
				m_frameData.m_upZ = telemetryMatrix->at.y;

				//---------------debug--------------				 				 
				//m_frameData.m_posX = 0.0;
				//m_frameData.m_posY = 0.0;
				//m_frameData.m_posZ = sin(frameDataTime*2.0) * 5.0f;
				
				//m_frameData.m_fwdX = 0.0f;
				//m_frameData.m_fwdY = 0.0f;
				//m_frameData.m_fwdZ = 1.0f;

				//m_frameData.m_upX = 1.0f;
				//m_frameData.m_upY = 0.0f;
				//m_frameData.m_upZ = 0.0f;
				//------------debug------------------

				m_telemetryAPI->SendFrame(&m_frameData);
			}

		}


	}

	//// spawn an admiral if L is pressed
	//if (Scripting::IS_GAME_KEYBOARD_KEY_JUST_PRESSED(KEY_L))
	//{
	//	int index;
	//	auto mdl = CModelInfo::GetModelInfo(rage::atStringHash("admiral"), &index);
	//	CStreaming::ScriptRequestModel(rage::atStringHash("admiral"));
	//	CStreaming::LoadAllRequestedModels(0);
	//	CMatrix mat = *FindPlayerPed()->m_pMatrix;
	//	mat.pos.x += 2;
	//	CVehicle* veh = VehicleFactory->CreateVehicle(index, RANDOM_VEHICLE, &mat, 1);
	//	CWorld::Add(veh, 0);
	//}
}

// ran after the sdk initializes, add all your hooks/events/etc here
void plugin::gameStartupEvent()
{

	if (m_telemetryAPI != nullptr)
	{
		m_telemetryAPI->Deinit();
		delete m_telemetryAPI;
		m_telemetryAPI = nullptr;
	}
	m_telemetryAPI = SpaceMonkeyTelemetryAPI_Create();

	if (m_telemetryAPI != nullptr)
	{
		m_telemetryAPI->InitSendSharedMemory();
	}
	m_systemTime = 0.0;
	m_lastSampleTime = 0.0;

//	plugin::processScriptsEvent::Add(SpaceMonkeyLoop);
//	plugin::drawingEvent::Add(SpaceMonkeyLoop);
	plugin::processCameraEvent::Add(SpaceMonkeyLoop);

}

void plugin::gameShutdownEvent()
{
    if (m_telemetryAPI != nullptr)
    {
        m_telemetryAPI->Deinit();
        delete m_telemetryAPI;
        m_telemetryAPI = nullptr;
    }
}