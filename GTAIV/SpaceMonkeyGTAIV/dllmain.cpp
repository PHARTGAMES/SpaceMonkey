#include "pch.h"
#include "SpaceMonkeyTelemetryAPI.h"
#include "systemtime.h"
#include "XInput.h"
#include "XInputFFBConfig.h"
#include "XInputFFBHost.h"
#include "XInputFFBConfigUI.h"
#include "IVSDK.cpp"

SpaceMonkeyTelemetryAPI* m_telemetryAPI = nullptr;
CMCustomUDPData* m_frameData = nullptr;
double m_lastSampleTime = 0.0;
double m_sampleRate = 1.0 / 30.0;
double m_realTime = 0.0;
double m_gtaTime = 0.0;
double m_fixedTime = 0.0;

static XInputFFBHost *s_xInputFFBHost = nullptr;
static XInputFFBConfigUI* s_xInputFFBConfigUI = nullptr;

void __stdcall OnUIChange(const char* msg) {
	printf("UI changed: %s\n", msg);
}

void UpdateInput()
{
	if (Scripting::IS_GAME_KEYBOARD_KEY_PRESSED(KEY_RIGHT_SHIFT))
	{
		if (Scripting::IS_GAME_KEYBOARD_KEY_JUST_PRESSED(KEY_M))
		{
			if (s_xInputFFBConfigUI == nullptr)
			{
				s_xInputFFBConfigUI = new XInputFFBConfigUI();
			}
			else
			{
				return;
			}

			s_xInputFFBConfigUI->Start(&OnUIChange, s_xInputFFBHost);

		}
	}
}

// every frame while in-game
void SpaceMonkeyLoop()
{
	
	if (s_xInputFFBHost == nullptr)
	{
		s_xInputFFBHost = new XInputFFBHost();
		s_xInputFFBHost->Initialize();
		s_xInputFFBHost->LoadConfig();
		s_xInputFFBHost->CreateXInputFFBDevices();
		s_xInputFFBHost->ResolveHostWindow();
		s_xInputFFBHost->SetInputFocus(XInputFFBHost::InputFocus::Host);
		s_xInputFFBHost->EnumerateSourceDevices();
	}
	else
	{
		
		s_xInputFFBHost->Update(CTimer::ms_fTimeStep, (uint32_t)VehicleIndexToFlag(m_frameData->vehicle_type));
	}

	UpdateInput();

	if (m_telemetryAPI && s_xInputFFBHost)
	{
		unsigned int gameTimeMS = 0;
		Scripting::GET_GAME_TIMER(&gameTimeMS);
		m_gtaTime = (float)gameTimeMS / 1000.0f;
//		m_gtaTime = CTimer::m_snTimeInMilliseconds / 1000.0f;
//		m_gtaTime += CTimer::ms_fTimeStep; //out of sync with camera event
		m_realTime = SystemTime::GetInSeconds(); //use SystemTime for camera event.

		if (m_lastSampleTime == 0.0f)
		{
			m_lastSampleTime = m_realTime;
			return;
		}

		double timeDelta = m_realTime - m_lastSampleTime;

		if(timeDelta >= m_sampleRate)
		{
			m_fixedTime += m_sampleRate;
			m_lastSampleTime = m_realTime;

			CMatrix* telemetryMatrix = nullptr;
			CPed* playerPed = FindPlayerPed();
			CVehicle* vehicle = playerPed != nullptr && playerPed->m_pVehicle && playerPed->m_pVehicle->IsDriver(playerPed) ? playerPed->m_pVehicle : nullptr;

			const XINPUT_STATE& inputState = s_xInputFFBHost->GetXInputState(0);

			m_frameData->steering_input = (inputState.Gamepad.sThumbLX / 32767.0f);

			//if (playerPed != nullptr && vehicle != nullptr && vehicle->IsDriver(playerPed))
			//{
			//	telemetryMatrix = vehicle->m_pMatrix;
			//}
			//else
			{
				if (playerPed != nullptr)
				{
					telemetryMatrix = playerPed->m_pMatrix;
				}
			}

			if (telemetryMatrix != nullptr)
			{

//				m_frameData->total_time = m_gtaTime;
				m_frameData->total_time = m_realTime;
//				m_frameData->total_time = m_fixedTime;
				float worldScale = 1.0f;
				m_frameData->position_x = telemetryMatrix->pos.x * worldScale;
				m_frameData->position_y = telemetryMatrix->pos.z * worldScale;
				m_frameData->position_z = telemetryMatrix->pos.y * worldScale;

				m_frameData->world_dir_fwd_x = telemetryMatrix->at.x;
				m_frameData->world_dir_fwd_y = telemetryMatrix->at.z;
				m_frameData->world_dir_fwd_z = telemetryMatrix->at.y;

				m_frameData->world_dir_rht_x = telemetryMatrix->right.x;
				m_frameData->world_dir_rht_y = telemetryMatrix->right.z;
				m_frameData->world_dir_rht_z = telemetryMatrix->right.y;

				if (vehicle != nullptr && vehicle->IsDriver(playerPed))
				{
					m_frameData->vehicle_type = (float)CMCustomUDPData::VehicleType::Car;

					switch (vehicle->m_nWheelCount)
					{
					case 4:
					{
						m_frameData->vehicle_type = (float)CMCustomUDPData::VehicleType::Car;
						m_frameData->suspension_position_fl = vehicle->m_pWheels[0].m_suspensionPos;
						m_frameData->suspension_position_bl = vehicle->m_pWheels[1].m_suspensionPos;
						m_frameData->suspension_position_fr = vehicle->m_pWheels[2].m_suspensionPos;
						m_frameData->suspension_position_br = vehicle->m_pWheels[3].m_suspensionPos;

						break;
					}
					case 2:
					{
						m_frameData->vehicle_type = (float)CMCustomUDPData::VehicleType::Bike;
						m_frameData->suspension_position_fl = vehicle->m_pWheels[0].m_suspensionPos;
						m_frameData->suspension_position_bl = vehicle->m_pWheels[1].m_suspensionPos;

						break;
					}
					}

					////m_frameData->suspension_velocity_fl = vehicle->m_pWheels[0]._f108;
					//m_frameData->suspension_velocity_fr = vehicle->m_pWheels[0]._f10c;
					//m_frameData->suspension_velocity_bl = vehicle->m_pWheels[0]._f110;
					//m_frameData->suspension_velocity_br = vehicle->m_pWheels[0]._f114;

					//m_frameData->suspension_acceleration_fl = vehicle->m_pWheels[0]._f118;
					//m_frameData->suspension_acceleration_fr = vehicle->m_pWheels[0]._f120;
					//m_frameData->suspension_acceleration_bl = vehicle->m_pWheels[0]._f124;
					//m_frameData->suspension_acceleration_br = vehicle->m_pWheels[0].m_fRotationZ;


				}
				else
				{
					m_frameData->vehicle_type = (float)CMCustomUDPData::VehicleType::Pedestrian;

				}



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

				m_telemetryAPI->SendFrame();
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

	if (s_xInputFFBHost != nullptr)
	{
		s_xInputFFBHost->Deinit();
		delete s_xInputFFBHost;
		s_xInputFFBHost = nullptr;
	}

	if (m_telemetryAPI != nullptr)
	{
		m_telemetryAPI->Deinit();
		delete m_telemetryAPI;
		m_telemetryAPI = nullptr;
	}
	m_telemetryAPI = SpaceMonkeyTelemetryAPI_Create();

	if (m_telemetryAPI != nullptr)
	{
		m_frameData = m_telemetryAPI->InitSendSharedMemory("PacketFormats\\SpaceMonkeyTelemetryDefault.xml");
	}
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

	if (s_xInputFFBHost != nullptr)
	{
		s_xInputFFBHost->Deinit();
		delete s_xInputFFBHost;
		s_xInputFFBHost = nullptr;
	}

	if (s_xInputFFBConfigUI != nullptr)
	{
		delete s_xInputFFBConfigUI;
		s_xInputFFBConfigUI = nullptr;
	}



}