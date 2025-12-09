#include "pch.h"
#include "SpaceMonkeyTelemetryAPI.h"
#include "systemtime.h"
#include "XInput.h"
#include "XInputFFBConfig.h"
#include "XInputFFBHost.h"
#include "XInputFFBConfigUI.h"
#include "SMMatrixReader.h"
#include "Debug.h"
#include "IVSDK.cpp"

SpaceMonkeyTelemetryAPI* m_telemetryAPI = nullptr;
CMCustomUDPData* m_frameData = nullptr;
double m_lastSampleTime = 0.0;
double m_sampleRate = 1.0 / 60.0;
double m_realTime = 0.0;
double m_gtaTime = 0.0;
double m_fixedTime = 0.0;

static XInputFFBHost *s_xInputFFBHost = nullptr;
static XInputFFBConfigUI* s_xInputFFBConfigUI = nullptr;

static SMMatrixReader* s_matrixReader = nullptr;

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





void PrepareFrame()
{

	if (m_telemetryAPI && s_xInputFFBHost)
	{
		CMatrix* telemetryMatrix = nullptr;
		CPed* playerPed = FindPlayerPed();
		CVehicle* vehicle = playerPed != nullptr && playerPed->m_pVehicle && playerPed->m_pVehicle->IsDriver(playerPed) ? playerPed->m_pVehicle : nullptr;


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

//		playerPed->GetVelocity();

		float worldScale = 1.0f;

		s_matrixReader->SetSourceAddress(telemetryMatrix);

		m_frameData->steering_input = s_xInputFFBHost->GetXInputAxisValueForEffectType(XInputFFBEffectType::Steering);

		if (telemetryMatrix != nullptr)
		{

			//m_frameData->position_x = telemetryMatrix->pos.x * worldScale;
			//m_frameData->position_y = telemetryMatrix->pos.z * worldScale;
			//m_frameData->position_z = telemetryMatrix->pos.y * worldScale;


			//m_frameData->position_x = 0.0f;
			//m_frameData->position_y = 0.0f;
			//m_frameData->position_z = sin(activeTime * 3.14f) * 10.0f;

			//m_frameData->world_dir_fwd_x = 0;
			//m_frameData->world_dir_fwd_y = 0;
			//m_frameData->world_dir_fwd_z = 1;

			//m_frameData->world_dir_rht_x = 1;
			//m_frameData->world_dir_rht_y = 0;
			//m_frameData->world_dir_rht_z = 0;


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

		}

	}

}

float s_fixedTimer = 0.0f;
float s_lastGTATime = 0.0f;
float s_lastRealTime = 0.0f;

void SMMatrixReaderCallback(SMMatrixReader::Matrix m, double tSeconds)
{

	s_fixedTimer += 1.0f / 60.0f;
	float nowGTATime = CTimer::m_snTimeInMilliseconds / 1000.0f;

//	float timerValue = (float)tSeconds;
//	float timerValue = nowGTATime;
	float timerValue = s_fixedTimer;
	

	float timeDelta = timerValue - m_frameData->total_time;
	Debug::Log("TimeDelta: %f\n", timeDelta);
	float gtaTimeDelta = nowGTATime - s_lastGTATime;
	Debug::Log("GTATimeDelta: %f\n", gtaTimeDelta);
	float realTimeDelta = (float)tSeconds - s_lastRealTime;
	s_lastRealTime = (float)tSeconds;
	Debug::Log("RealTimeDelta: %f\n", realTimeDelta);

	//if (timeDelta > ((1.0f / 60.0f)*1.5f))
	//{
	//	Debug::Log("--SPIKE--\n");
	//}
	Debug::Log("GTATime: %f\n", nowGTATime);
	Debug::Log("RealTime: %f\n", tSeconds);
	Debug::Log("FixedTime: %f\n", s_fixedTimer);

	s_lastGTATime = nowGTATime;


	m_frameData->total_time = timerValue;

	float worldScale = 1.0f;

	//m[0] m[1] m[2] m[3] right
	//m[4] m[5] m[6] m[7] at
	//m[8] m[9] m[10] m[11] up
	//m[12] m[13] m[14] m[15] pos

	float xPosDelta = (m[12] * worldScale) - m_frameData->position_x;
	Debug::Log("XPosDelta: %f\n", xPosDelta);
	Debug::Log("XVel: %f\n", xPosDelta / timeDelta);

	m_frameData->position_x = m[12] * worldScale;
	m_frameData->position_y = m[14] * worldScale;
	m_frameData->position_z = m[13] * worldScale;

	m_frameData->world_dir_fwd_x = m[4];
	m_frameData->world_dir_fwd_y = m[6];
	m_frameData->world_dir_fwd_z = m[5];

	m_frameData->world_dir_rht_x = m[0];
	m_frameData->world_dir_rht_y = m[2];
	m_frameData->world_dir_rht_z = m[1];

	m_telemetryAPI->SendFrame();

}

// every frame while in-game
void SpaceMonkeyLoop()
//void SpaceMonkeyLoop(CVehicle *procVeh)
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
		s_xInputFFBHost->EnableFocusMonitor(true);

		s_matrixReader = new SMMatrixReader();
		s_matrixReader->SetPollIntervalMs(1);
		s_matrixReader->SetStableDurationMs(5);
		s_matrixReader->Start(&SMMatrixReaderCallback);

	}
	else
	{
		s_xInputFFBHost->Update(CTimer::ms_fTimeStep, (uint32_t)VehicleIndexToFlag(m_frameData->vehicle_type));
	}

	UpdateInput();

	PrepareFrame();
//	SendFrame();
}





void SendFrame()
{

	if (m_telemetryAPI && s_xInputFFBHost)
	{
		unsigned int gameTimeMS = 0;
		Scripting::GET_GAME_TIMER(&gameTimeMS);
//		m_gtaTime = (float)gameTimeMS / 1000.0f;
		m_gtaTime = CTimer::m_snTimeInMilliseconds / 1000.0f;
//		m_gtaTime += CTimer::ms_fTimeStep; //out of sync with camera event
		m_realTime = SystemTime::GetInSeconds(); //use SystemTime for camera event.

		float activeTime = m_gtaTime;
//		float activeTime = m_realTime;

		if (m_lastSampleTime == 0.0f)
		{
			m_lastSampleTime = activeTime;
			return;
		}


		CMatrix* telemetryMatrix = nullptr;
		CPed* playerPed = FindPlayerPed();
		CVehicle* vehicle = playerPed != nullptr && playerPed->m_pVehicle && playerPed->m_pVehicle->IsDriver(playerPed) ? playerPed->m_pVehicle : nullptr;


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

		float worldScale = 1.0f;
		bool positionChanged = false;
		if (telemetryMatrix != nullptr)
		{
			CVector framePos = CVector(m_frameData->position_x, m_frameData->position_y, m_frameData->position_z);
			CVector newPos = CVector(telemetryMatrix->pos.x * worldScale, telemetryMatrix->pos.z * worldScale, telemetryMatrix->pos.y * worldScale);

			float diff = (newPos - framePos).Magnitude();
			positionChanged = diff != 0.0f;
		}


		double timeDelta = activeTime - m_lastSampleTime;

//		if(timeDelta >= m_sampleRate)
//		if(m_lastSampleTime != activeTime && positionChanged)		
//		if(m_lastSampleTime != activeTime || positionChanged)
//		if(positionChanged)
		if(m_lastSampleTime != activeTime)		
		{
			m_fixedTime += m_sampleRate;
//			activeTime = m_fixedTime;
			m_lastSampleTime = activeTime;


			m_frameData->steering_input = s_xInputFFBHost->GetXInputAxisValueForEffectType(XInputFFBEffectType::Steering);

			if (telemetryMatrix != nullptr)
			{

//				m_frameData->total_time = m_gtaTime;
				m_frameData->total_time = activeTime;
//				m_frameData->total_time = m_fixedTime;
				//m_frameData->position_x = telemetryMatrix->pos.x * worldScale;
				//m_frameData->position_y = telemetryMatrix->pos.z * worldScale;
				//m_frameData->position_z = telemetryMatrix->pos.y * worldScale;


				//m_frameData->position_x = 0.0f;
				//m_frameData->position_y = 0.0f;
				//m_frameData->position_z = sin(activeTime * 3.14f) * 10.0f;

				//m_frameData->world_dir_fwd_x = 0;
				//m_frameData->world_dir_fwd_y = 0;
				//m_frameData->world_dir_fwd_z = 1;

				//m_frameData->world_dir_rht_x = 1;
				//m_frameData->world_dir_rht_y = 0;
				//m_frameData->world_dir_rht_z = 0;



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
		//else
		//{
		//}

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

	plugin::processScriptsEvent::Add(SpaceMonkeyLoop);
//	plugin::drawingEvent::Add(SpaceMonkeyLoop);
//	plugin::processAutomobileEvent::Add(SpaceMonkeyLoop);
//	plugin::processCameraEvent::Add(SpaceMonkeyLoop);

}

void plugin::gameShutdownEvent()
{
	if (s_matrixReader != nullptr)
	{
		s_matrixReader->Stop();
		delete s_matrixReader;
		s_matrixReader = nullptr;
	}

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