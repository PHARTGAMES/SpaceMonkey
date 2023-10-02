#pragma once
#include "Mod/Mod.h"
#include "IWibbleWobbleCapture.h"
#include "WWSharedMemory.h"


#pragma pack( push, 0 )
volatile struct __declspec(dllexport) OpenMotionFrameData
{
	float m_time = 0;
	float m_posX = 0; //position x
	float m_posY = 0; //position y
	float m_posZ = 0; //position z
	float m_fwdX = 0; //forward direction X, identity 0 
	float m_fwdY = 0; //forward direction Y, identity 0
	float m_fwdZ = 1; //forward direction Z, identity 1
	float m_upX = 0; //up direction X, identity 0; left handed; right = up cross fwd
	float m_upY = 1; //up direction Y, identity 1
	float m_upZ = 0; //up direction Z, identity 0
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
		m_systemTime = 0.0f;

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

	void _TickMotion(UE4::FVector a_pos, UE4::FRotator a_rot, float a_dt);

	void _GetHeadTracking(UE4::FVector& a_pos, UE4::FRotator& a_rot, float& a_hFov, float& a_worldScale);


	void _Cleanup();

	void OnDestroy();


private:
	// If you have a BP Mod Actor, This is a straight refrence to it
	UE4::AActor* ModActor = NULL;
	WWSharedMemory *m_motionIPC = NULL;

	OpenMotionFrameData m_frameData;

	float m_systemTime;
};

