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




BPFUNCTION(TickMotion)
{


	struct InputParams
	{
		UE4::FVector Pos;
		UE4::FRotator Rot;
	};
	auto Inputs = stack->GetInputParams<InputParams>();

	if (s_motionInstance != NULL)
	{
		s_motionInstance->_TickMotion(Inputs->Pos, Inputs->Rot);
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



void UE4Motion::_Cleanup()
{

}

void UE4Motion::_TickMotion(UE4::FVector a_pos, UE4::FRotator a_rot)
{
	if (m_ipc != NULL)
	{

		m_frameData.m_time = SystemTime::GetInSeconds();
		m_frameData.m_posX = a_pos.X * 0.01f; //convert to meters
		m_frameData.m_posY = a_pos.Y * 0.01f; //convert to meters
		m_frameData.m_posZ = a_pos.Z * 0.01f; //convert to meters

		float deg2rad = (3.14159265359f / 180.0f);
		m_frameData.m_rotP = a_rot.Pitch * deg2rad;
		m_frameData.m_rotY = a_rot.Yaw * deg2rad;
		m_frameData.m_rotR = a_rot.Roll * deg2rad;

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

			//Log::Print("Freetrack Position: X:%f, Y:%f, Z:%f, \n", a_pos.X, a_pos.Y, a_pos.Z);
			//Log::Print("Freetrack Rotation: P:%f, Y:%f, R:%f, \n", a_rot.Pitch, a_rot.Yaw, a_rot.Roll);

		}
		else
		{
			Debug::Log("NULL ftData\n");
		}
	}
}


void UE4Motion::OnDestroy()
{
	Debug::Log("UE4Motion::OnDestroy\n");

	if (m_wwFreetrack.IsInitialized())
		m_wwFreetrack.Destroy();

	if (m_ipc != NULL)
	{
		m_ipc->Destroy();
		delete m_ipc;
		m_ipc = NULL;
	}
}

