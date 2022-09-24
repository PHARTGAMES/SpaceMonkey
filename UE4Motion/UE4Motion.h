#pragma once
#include "Mod/Mod.h"
#include "WWSharedMemory.h"
#include "WWFreetrack.h"


#pragma pack( push, 0 )
volatile struct __declspec(dllexport) OpenMotionFrameData
{
	float m_time = 0;
	float m_posX = 0; //position x
	float m_posY = 0; //position y
	float m_posZ = 0; //position z
	float m_rotP = 0; //pitch (radians)
	float m_rotY = 0; //yaw (radians)
	float m_rotR = 0; //roll (radians)
};
#pragma pack(pop)


#pragma pack( push, 0 )
volatile struct __declspec(dllexport) WWFreeTrackFrameData
{
	float m_readtime = 0;
	float m_yaw = 0;   /* positive yaw to the left */
	float m_pitch = 0; /* positive pitch up */
	float m_roll = 0;  /* positive roll to the left */
	float m_x = 0;
	float m_y = 0;
	float m_z = 0;
};
#pragma pack(pop)


class UE4Motion : public Mod
{

public:

	//Basic Mod Info
	UE4Motion()
	{
		ModName = "UE4Motion"; // Mod Name -- If Using BP ModActor, Should Be The Same Name As Your Pak
		ModVersion = "1.0.0"; // Mod Version
		ModDescription = "Open Motion"; // Mod Description
		ModAuthors = "PEZZALUCIFER"; // Mod Author
		ModLoaderVersion = "2.2.0";

		// Dont Touch The Internal Stuff
		ModRef = this;
		CompleteModCreation();

	}

	//Called When Internal Mod Setup is finished
	virtual void InitializeMod() override;

	//InitGameState Call
	virtual void InitGameState() override;

	//Beginplay Hook of Every Actor
	virtual void BeginPlay(UE4::AActor* Actor) override;

	//PostBeginPlay of EVERY Blueprint ModActor
	virtual void PostBeginPlay(std::wstring ModActorName, UE4::AActor* Actor) override;

	//DX11 hook for when an image will be presented to the screen
	virtual void DX11Present(ID3D11Device* pDevice, ID3D11DeviceContext* pContext, ID3D11RenderTargetView* pRenderTargetView) override;

	virtual void OnModMenuButtonPressed() override;

	//Call ImGui Here (CALLED EVERY FRAME ON DX HOOK)
	virtual void DrawImGui() override;

	void _TickMotion(UE4::FVector a_pos, UE4::FRotator a_rot);

	void _GetHeadTracking(UE4::FVector& a_pos, UE4::FRotator& a_rot);

	void _GetHeadTrackingConstants(float& a_vfov, float& a_worldScale, float& a_aspectRatio);

	void _Cleanup();

	void OnDestroy();

private:
	// If you have a BP Mod Actor, This is a straight refrence to it
	UE4::AActor* ModActor = NULL;
	WWSharedMemory *m_ipc = NULL;
//	WWFreetrack m_wwFreetrack;
	WWSharedMemory* m_wwFreeTrack = NULL;

	OpenMotionFrameData m_frameData;

	WWFreeTrackFrameData m_freeTrackFrame;
};

