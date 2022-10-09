#include "UE4Motion.h"
#include "Utilities/MinHook.h"
#include <stdio.h>
#include "Logger.h"
#include "systemtime.h"
#include <cstdarg>
#include <stdio.h>
#include <wtypes.h>
#include <string>
#include <sstream>
#include <fstream>
#include <vector>

static UE4Motion* s_motionInstance = NULL;




//BPFUNCTION(WriteToFile)
//{
//	std::cout << "WriteToFile" << std::endl;
//	struct InputParams
//	{
//		UE4::FString NameTest;
//	};
//	auto Inputs = stack->GetInputParams<InputParams>();
//	stack->SetOutput<UE4::FString>("OutPutString", L"KboyGang");
//	stack->SetOutput<bool>("ReturnValue", true);
//}
// 




BPFUNCTION(TickMotion)
{


	struct InputParams
	{
		UE4::FVector Pos;
		UE4::FRotator Rot;
		float DT;
	};
	auto Inputs = stack->GetInputParams<InputParams>();

	if (s_motionInstance != NULL)
	{
		s_motionInstance->_TickMotion(Inputs->Pos, Inputs->Rot, Inputs->DT);
	}
}

BPFUNCTION(Cleanup)
{
	if (s_motionInstance != NULL)
	{
		s_motionInstance->_Cleanup();
	}
}


BPFUNCTION(GetHeadTracking)
{
	//Log::Print("Called GetHeadTracking");
	
	UE4::FVector pos;
	UE4::FRotator rot;
	float hFov;
	float worldScale;
	UE4::FVector constants;


	s_motionInstance->_GetHeadTracking(pos, rot, hFov, worldScale);

	constants.X = hFov;
	constants.Y = worldScale;

	//Log::Print("Pos: %f, %f, %f ", pos.X, pos.Y, pos.Z);
	//Log::Print("Rot: %f, %f, %f ", rot.Pitch, rot.Yaw, rot.Roll);

	stack->SetOutput<UE4::FVector>("Pos", pos);
	stack->SetOutput<UE4::FRotator>("Rot", rot);
	stack->SetOutput<UE4::FVector>("Constants", constants);
}

// Only Called Once, if you need to hook shit, declare some global non changing values
void UE4Motion::InitializeMod()
{
	s_motionInstance = this;

	UE4::InitSDK();
	SetupHooks();

	REGISTER_FUNCTION(TickMotion);

	REGISTER_FUNCTION(GetHeadTracking);

	//MinHook::Init(); //Uncomment if you plan to do hooks

	//UseMenuButton = true; // Allows Mod Loader To Show Button

	if (m_motionIPC == NULL)
	{
		m_motionIPC = new WWSharedMemory("OM_FRAME", "OM_FRAME_MUTEX", WWSharedMemType::WWSharedMem_Write, (void*)&m_frameData, sizeof(m_frameData));
	}


}

void UE4Motion::InitGameState()
{
}

void UE4Motion::BeginPlay(UE4::AActor* Actor)
{
}

void UE4Motion::PostBeginPlay(std::wstring ModActorName, UE4::AActor* Actor)
{

	//Log("PostBeginPlay: ActorName: %s\n", ModActorName.c_str());

	// Filters Out All Mod Actors Not Related To Your Mod
	std::wstring TmpModName(ModName.begin(), ModName.end());
	if (ModActorName == TmpModName)
	{
		//Sets ModActor Ref
		ModActor = Actor;
	}
}

void UE4Motion::DX11Present(ID3D11Device* pDevice, ID3D11DeviceContext* pContext, ID3D11RenderTargetView* pRenderTargetView)
{
}

void UE4Motion::OnModMenuButtonPressed()
{
}

void UE4Motion::DrawImGui()
{
}



void UE4Motion::_Cleanup()
{
	m_systemTime = 0.0f;
}

void UE4Motion::_TickMotion(UE4::FVector a_pos, UE4::FRotator a_rot, float a_dt)
{
	if (m_motionIPC != NULL)
	{
		m_systemTime += a_dt;
		m_frameData.m_time = m_systemTime;// SystemTime::GetInSeconds();
		m_frameData.m_posX = a_pos.Y * 0.01f; //convert to meters
		m_frameData.m_posY = a_pos.Z * 0.01f; //convert to meters
		m_frameData.m_posZ = a_pos.X * 0.01f; //convert to meters

		m_frameData.m_rotP = -a_rot.Pitch * deg2rad;
		m_frameData.m_rotY = a_rot.Yaw * deg2rad;
		m_frameData.m_rotR = -a_rot.Roll * deg2rad;

		m_motionIPC->Write();

		//Log::Print("Position: X:%f, Y:%f, Z:%f, \n", a_pos.X, a_pos.Y, a_pos.Z);
		//Log::Print("Rotation: P:%f, Y:%f, R:%f, \n", a_rot.Pitch, a_rot.Yaw, a_rot.Roll);
	}
		
}


void UE4Motion::_GetHeadTracking(UE4::FVector& a_pos, UE4::FRotator& a_rot, float &a_hFov, float &a_worldScale)
{
	a_pos.X = a_pos.Y = a_pos.Z = 0.0f;
	a_rot.Pitch = a_rot.Yaw = a_rot.Roll = 0.0f;
	a_hFov = 120.0f;
	a_worldScale = 1.0f;

	WWCaptureHeadTrackData trackingData;
	if (WWTickHeadTracking(trackingData))
	{
		a_pos.X = trackingData.m_z;
		a_pos.Y = trackingData.m_x;
		a_pos.Z = trackingData.m_y;

		a_rot.Pitch = -trackingData.m_pitch * rad2deg;
		a_rot.Yaw = -trackingData.m_yaw * rad2deg;
		a_rot.Roll = -trackingData.m_roll * rad2deg;

		a_hFov = trackingData.m_hFov;
		a_worldScale = trackingData.m_worldScale;

	//	Log::Print("_GetHeadTracking:hFov :%f \n", a_hFov);
	//	Log::Print("_GetHeadTracking:worldScale :%f \n", a_worldScale);
	}

	//Log::Print("Freetrack Position: X:%f, Y:%f, Z:%f, \n", a_pos.X, a_pos.Y, a_pos.Z);
	//Log::Print("Freetrack Rotation: P:%f, Y:%f, R:%f, \n", a_rot.Pitch, a_rot.Yaw, a_rot.Roll);
}


void UE4Motion::OnDestroy()
{
	Debug::Log("UE4Motion::OnDestroy\n");

	WWCleanupHeadTracking();

	if (m_motionIPC != NULL)
	{
		m_motionIPC->Destroy();
		delete m_motionIPC;
		m_motionIPC = NULL;
	}
}

