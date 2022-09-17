#include "UE4Motion.h"
#include "Utilities/MinHook.h"
#include <stdio.h>
#include "Logger.h"
#include "systemtime.h"

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


BPFUNCTION(SetMotionActor)
{
//	Log("Called SetMotionActor\n");

	//Log::Print("Called SetMotionActor\n");

//	std::cout << "WriteToFile" << std::endl;
	struct InputParams
	{
		UE4::AActor *MotionActor;
		UE4::FString OutputString;
	};
	auto Inputs = stack->GetInputParams<InputParams>();
	stack->SetOutput<bool>("ReturnValue", true);


	if (Inputs->MotionActor != NULL)
	{
		stack->SetOutput<UE4::FString>("OutputString", L"SetMotionActor Success");
		stack->SetOutput<bool>("ReturnValue", true);

		s_motionInstance->_SetMotionActor(Inputs->MotionActor);
	}
	else
	{
		s_motionInstance->_SetMotionActor(NULL);
		stack->SetOutput<UE4::FString>("OutputString", L"SetMotionActor Failed");
		stack->SetOutput<bool>("ReturnValue", false);
	}
 
//	stack->SetOutput<UE4::FString>("OutputString", L"KboyGang");
}


BPFUNCTION(TickMotion)
{
	if (s_motionInstance != NULL)
	{
		s_motionInstance->_TickMotion();
	}
}


BPFUNCTION(GetHeadTracking)
{
	//Log::Print("Called GetHeadTracking");
	
	UE4::FVector pos;
	UE4::FRotator rot;

	s_motionInstance->_GetHeadTracking(pos, rot);

	//Log::Print("Pos: %f, %f, %f ", pos.X, pos.Y, pos.Z);
	//Log::Print("Rot: %f, %f, %f ", rot.Pitch, rot.Yaw, rot.Roll);

	stack->SetOutput<UE4::FVector>("Pos", pos);
	stack->SetOutput<UE4::FRotator>("Rot", rot);
}


// Only Called Once, if you need to hook shit, declare some global non changing values
void UE4Motion::InitializeMod()
{
	s_motionInstance = this;

	UE4::InitSDK();
	SetupHooks();

	REGISTER_FUNCTION(SetMotionActor);

	REGISTER_FUNCTION(TickMotion);

	REGISTER_FUNCTION(GetHeadTracking);

	//MinHook::Init(); //Uncomment if you plan to do hooks

	//UseMenuButton = true; // Allows Mod Loader To Show Button

	if (m_ipc == NULL)
	{
		m_ipc = new WWSharedMemory("OM_FRAME", "OM_FRAME_MUTEX", WWSharedMemType::WWSharedMem_Write, (void*)&m_frameData, sizeof(m_frameData));
	}

	if (m_wwFreetrack.Create())
	{
		Log::Print("Freetrack Initialized");
	}
	else
	{
		Log::Print("Freetrack Failed to initialize");
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

void UE4Motion::_SetMotionActor(UE4::AActor* a_motionActor)
{
	m_motionActor = a_motionActor;
}

void UE4Motion::_TickMotion()
{
	if (m_motionActor != NULL && m_ipc != NULL)
	{
		UE4::FVector position = m_motionActor->GetActorLocation();
		UE4::FRotator rotation = m_motionActor->GetActorRotation();

		m_frameData.m_time = SystemTime::GetInSeconds();
		m_frameData.m_posX = position.X * 0.01f; //convert to meters
		m_frameData.m_posY = position.Y * 0.01f; //convert to meters
		m_frameData.m_posZ = position.Z * 0.01f; //convert to meters

		float deg2rad = (3.14159265359f / 180.0f);
		m_frameData.m_rotP = rotation.Pitch * deg2rad;
		m_frameData.m_rotY = rotation.Yaw * deg2rad;
		m_frameData.m_rotR = rotation.Roll * deg2rad;

		m_ipc->Write();

		//Log::Print("Position: X:%f, Y:%f, Z:%f, \n", position.X, position.Y, position.Z);
		//Log::Print("Rotation: P:%f, Y:%f, R:%f, \n", rotation.Pitch, rotation.Yaw, rotation.Roll);
	}
		
}

void UE4Motion::_GetHeadTracking(UE4::FVector& a_pos, UE4::FRotator& a_rot)
{
	if (m_wwFreetrack.IsInitialized())
	{
		WWFreetrack::FTData* ftData = m_wwFreetrack.GetFTData();

		if (ftData != NULL)
		{
			a_pos.X = ftData->X;
			a_pos.Y = ftData->Y;
			a_pos.Z = ftData->Z;

			float rad2deg = (180.0f / 3.14159265359f);
			a_rot.Pitch = ftData->Pitch * rad2deg;
			a_rot.Yaw = ftData->Yaw * rad2deg;
			a_rot.Roll = ftData->Roll * rad2deg;
		}
	}
}

