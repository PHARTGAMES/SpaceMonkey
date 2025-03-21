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
#include "WWMath.h"

static UE4Motion* s_motionInstance = NULL;


typedef HRESULT(__stdcall* Present)(IDXGISwapChain* pSwapChain, UINT SyncInterval, UINT Flags);
Present oPresent = nullptr;
void* pPresentFunc = nullptr;

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

//	Log::Print("TickMotion");
//	Log::Print("Pos: %f, %f, %f ", Inputs->Pos.X, Inputs->Pos.Y, Inputs->Pos.Z);
//	Log::Print("Rot: %f, %f, %f ", Inputs->Rot.Pitch, Inputs->Rot.Yaw, Inputs->Rot.Roll);
//	Log::Print("DT: %f ", Inputs->DT);

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
	if (s_motionInstance != NULL)
	{
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

		//Log::Print("stack: %x", stack);


		stack->SetOutput<UE4::FVector>("Pos", pos);
		stack->SetOutput<UE4::FRotator>("Rot", rot);
		stack->SetOutput<UE4::FVector>("Constants", constants);

		//Log::Print("GetHeadTracking set outputs");
	}
}


BPFUNCTION(SetPlayerCameraManager)
{
	struct InputParams
	{
		UE4::APlayerCameraManager *PCM;
	};
	auto Inputs = stack->GetInputParams<InputParams>();

	Log::Print("SetPlayerCameraManager");
	Log::Print("Player Camera Manager %x", Inputs->PCM);
}

HRESULT __stdcall hkPresent(IDXGISwapChain* pSwapChain, UINT SyncInterval, UINT Flags)
{
	///////////////////////////PRESENT///////////////////////////////
		//https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/dxgi-present
		// Call the original Present method
	HRESULT result = oPresent(pSwapChain, SyncInterval, Flags);
	///////////////////////////PRESENT///////////////////////////////

	if (s_motionInstance != nullptr)
	{
		s_motionInstance->OnPresent();
	}

	return result;
}

int HookPresent()
{

	if (pPresentFunc != nullptr)
		return 1;

	DXGI_SWAP_CHAIN_DESC sd;
	ZeroMemory(&sd, sizeof(sd));
	sd.BufferCount = 2;
	sd.BufferDesc.Width = 0;
	sd.BufferDesc.Height = 0;
	sd.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
	sd.BufferDesc.RefreshRate.Numerator = 60;
	sd.BufferDesc.RefreshRate.Denominator = 1;
	sd.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
	sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	sd.OutputWindow = GetActiveWindow(); // Find Game HWND
	sd.SampleDesc.Count = 1;
	sd.SampleDesc.Quality = 0;
	sd.Windowed = TRUE;
	sd.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;


	IDXGISwapChain* g_pSwapChain;
	ID3D11Device* g_pd3dDevice;
	ID3D11DeviceContext* g_pd3dDeviceContext;


	const D3D_FEATURE_LEVEL featureLevelArray[2] = { D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL_10_0, };
	if (D3D11CreateDeviceAndSwapChain(0, D3D_DRIVER_TYPE_HARDWARE, 0, 0, featureLevelArray, 2, D3D11_SDK_VERSION, &sd, &g_pSwapChain, &g_pd3dDevice, nullptr, &g_pd3dDeviceContext) != S_OK)
	{
		return 1;
	}

	Log::Print("After D3D11CreateDeviceAndSwapChain\n");


	// Get the address of the Present method
	IDXGISwapChain* pSwapChain = g_pSwapChain; // Get the IDXGISwapChain instance
	//pPresentFunc = *(void**)(*(uintptr_t**)pSwapChain)[8];

	pPresentFunc = (Present)((DWORD_PTR*)((DWORD_PTR*)pSwapChain)[0])[8];

	Log::Print("After pPresentFunc = (Present)\n");

	// Create the hook
	if (MH_CreateHook(pPresentFunc, &hkPresent, reinterpret_cast<LPVOID*>(&oPresent)) != MH_OK)
	{
		return 1;
	}

	Log::Print("After MH_CreateHook\n");


	// Enable the hook
	if (MH_EnableHook(pPresentFunc) != MH_OK)
	{
		return 1;
	}

	Log::Print("After MH_EnableHook\n");


	g_pSwapChain->Release();

	Log::Print("After g_pSwapChain->Release();\n");

	g_pd3dDevice->Release();

	Log::Print("After g_pd3dDevice->Release();\n");

	g_pd3dDeviceContext->Release();

	Log::Print("After g_pd3dDeviceContext->Release();\n");

	return 0;

}

// Only Called Once, if you need to hook shit, declare some global non changing values
void UE4Motion::InitializeMod()
{
	s_motionInstance = this;

	UE4::InitSDK();
	SetupHooks();

	REGISTER_FUNCTION(TickMotion);

	REGISTER_FUNCTION(GetHeadTracking);
	REGISTER_FUNCTION(SetPlayerCameraManager);

	if (GameProfile::SelectedGameProfile.MotionOnPresent == 1)
	{
		MinHook::Init(); //Uncomment if you plan to do hooks

		//UseMenuButton = true; // Allows Mod Loader To Show Button

		if (HookPresent() != 0)
		{
			Log::Error("HookPresent() failed\n");
		}
	}

	if (m_motionIPC == NULL)
	{
		m_motionIPC = new WWSharedMemory("OM_FRAME", "OM_FRAME_MUTEX", WWSharedMemType::WWSharedMem_Write, (void*)&m_frameData, sizeof(m_frameData));
	}
}



void UE4Motion::OnPresent()
{
	if (GameProfile::SelectedGameProfile.MotionOnPresent == 0)
		return;

	if (m_motionIPC == nullptr)
		return;


	float timeNow = SystemTime::GetInSeconds();

	m_presentDT = timeNow - m_presentTime;

	//Log::Print("Present DT: %f", m_presentDT);

	m_presentTime = timeNow;

	m_frameData.m_time = m_presentTime;

	m_motionIPC->Write();

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
	m_presentTime = 0;
	m_presentDT = 1.0f / 60.0f;
}

void UE4Motion::_TickMotion(UE4::FVector a_pos, UE4::FRotator a_rot, float a_dt)
{
	//Log::Print("_TickMotion\n");

	if (m_motionIPC != NULL)
	{
		m_systemTime += a_dt;
		m_frameData.m_time = m_systemTime;// SystemTime::GetInSeconds();
		m_frameData.m_posX = a_pos.Y * 0.01f; //convert to meters
		m_frameData.m_posY = a_pos.Z * 0.01f; //convert to meters
		m_frameData.m_posZ = a_pos.X * 0.01f; //convert to meters

		MatrixRotationEulerID pyr[3] = { MatrixRotationEulerID::MREID_Roll, MatrixRotationEulerID::MREID_Pitch, MatrixRotationEulerID::MREID_Yaw };
		float eulerAngles[3] = { a_rot.Roll * deg2rad, -a_rot.Pitch * deg2rad, a_rot.Yaw * deg2rad };

		float rotM34[3][4];
		EulerXYZToM34(eulerAngles, m_frameData.m_posX, m_frameData.m_posY, m_frameData.m_posZ, rotM34, pyr);

		XMFLOAT3 rht = XMFLOAT3(rotM34[0][0], rotM34[1][0], rotM34[2][0]);
		XMFLOAT3 up = XMFLOAT3(rotM34[0][1], rotM34[1][1], rotM34[2][1]);
		XMFLOAT3 fwd = XMFLOAT3(rotM34[0][2], rotM34[1][2], rotM34[2][2]);

//		Log::Print("Motion-----------------------------------------------\nMotion eul: %f, %f, %f\nMotion rht: %f, %f, %f\nMotion up: %f, %f, %f\nMotion fwd: %f, %f, %f\nMotion pos: %f, %f, %f\n", a_rot.Pitch, a_rot.Yaw, a_rot.Roll, rht.x, rht.y, rht.z, up.x, up.y, up.z, fwd.x, fwd.y, fwd.z, rotM34[0][3], rotM34[1][3], rotM34[2][3]);

		m_frameData.m_fwdX = fwd.x;
		m_frameData.m_fwdY = fwd.y;
		m_frameData.m_fwdZ = fwd.z;

		m_frameData.m_upX = up.x;
		m_frameData.m_upY = up.y;
		m_frameData.m_upZ = up.z;


		//Log::Print("_TickMotion internal\n");

		if(GameProfile::SelectedGameProfile.MotionOnPresent == 0)
			m_motionIPC->Write();

		//Log::Print("Position: X:%f, Y:%f, Z:%f, \n", a_pos.X, a_pos.Y, a_pos.Z);
		//Log::Print("Rotation: P:%f, Y:%f, R:%f, \n", a_rot.Pitch, a_rot.Yaw, a_rot.Roll);
	}
		
}


void UE4Motion::_GetHeadTracking(UE4::FVector& a_pos, UE4::FRotator& a_rot, float &a_hFov, float &a_worldScale)
{
	//Log::Print("_GetHeadTracking\n");
	a_pos.X = a_pos.Y = a_pos.Z = 0.0f;
	a_rot.Pitch = a_rot.Yaw = a_rot.Roll = 0.0f;
	a_hFov = 120.0f;
	a_worldScale = 1.0f;

	//Log::Print("_GetHeadTracking before tick\n");

	WWCaptureTrackData trackingData;
	if (WWTickHeadTracking(trackingData))
	{
		//Log::Print("_GetHeadTracking tick\n");

		float ry, rp, rr, px, py, pz = 0;
		M34ToYPRXYZ(ry, rp, rr, px, py, pz, trackingData.m_headVehicle);

		a_pos.X = pz;
		a_pos.Y = px;
		a_pos.Z = py;

		a_rot.Pitch = -rp * rad2deg;
		a_rot.Yaw = -ry * rad2deg;
		a_rot.Roll = -pz * rad2deg;

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
	Log::Print("UE4Motion::OnDestroy\n");

	WWCleanupHeadTracking();

	if (m_motionIPC != NULL)
	{
		m_motionIPC->Destroy();
		delete m_motionIPC;
		m_motionIPC = NULL;
	}
}

