#include <RED4ext/RED4ext.hpp>
#include "SpaceMonkeySystem.h"
#include "SpaceMonkeyTelemetryAPI.h"
#include <RED4ext/Scripting/Natives/Vector4.hpp>
using namespace Red;

void SpaceMonkeySystem::OnRegisterUpdates(Red::UpdateRegistrar* aRegistrar)
{
    aRegistrar->RegisterUpdate(RED4ext::UpdateTickGroup::PreRenderUpdate, this, "SpaceMonkeySystemUpdate",
                               {this, &SpaceMonkeySystem::Tick});
}

void SpaceMonkeySystem::Tick(Red::FrameInfo& aInfo, Red::JobQueue& aQueue) noexcept
{
//    gSdk->logger->Trace(gPluginHandle, "SpaceMonkeySystem::Tick");

    RED4ext::ScriptGameInstance gameInstance;
    RED4ext::Handle<RED4ext::IScriptable> playerHandle;
    RED4ext::ExecuteGlobalFunction("GetPlayer;GameInstance", &playerHandle, gameInstance);

    if (playerHandle)
    {
        gSdk->logger->Trace(gPluginHandle, "Got player handle");

        auto rtti = RED4ext::CRTTISystem::Get();
        auto playerPuppetCls = rtti->GetClass("PlayerPuppet");
        auto getMountedVehicleFunc = playerPuppetCls->GetFunction("GetMountedVehicle");

        RED4ext::Handle<RED4ext::IScriptable> mountedVehicle;
        RED4ext::ExecuteFunction(playerHandle, getMountedVehicleFunc, &mountedVehicle);

        if (mountedVehicle)
        {
            //auto vehicleObjectCls = rtti->GetClass("VehicleObject");
            auto entityCls = rtti->GetClass("entEntity");
            auto getWorldForwardFunc = entityCls->GetFunction("GetWorldForward");
            auto getWorldUpFunc = entityCls->GetFunction("GetWorldUp");
            auto getWorldPositionFunc = entityCls->GetFunction("GetWorldPosition");

            RED4ext::Vector4 worldForward;
            RED4ext::ExecuteFunction(mountedVehicle, getWorldForwardFunc, &worldForward);

            RED4ext::Vector4 worldUp;
            RED4ext::ExecuteFunction(mountedVehicle, getWorldUpFunc, &worldUp);

            RED4ext::Vector4 worldPosition;
            RED4ext::ExecuteFunction(mountedVehicle, getWorldPositionFunc, &worldPosition);

            memset(&m_frameData, 0, sizeof(SpaceMonkeyTelemetryFrameData));
            
            m_systemTime += aInfo.deltaTime;
            m_frameData.m_time = m_systemTime;

            m_frameData.m_posX = worldPosition.Y;
            m_frameData.m_posY = worldPosition.Z;
            m_frameData.m_posZ = worldPosition.X;

            m_frameData.m_fwdX = worldForward.Y;
            m_frameData.m_fwdY = worldForward.Z;
            m_frameData.m_fwdZ = worldForward.X;

            m_frameData.m_upX = worldUp.Y;
            m_frameData.m_upY = worldUp.Z;
            m_frameData.m_upZ = worldUp.X;

            m_telemetryAPI->SendFrame(&m_frameData);


//            gSdk->logger->Trace(gPluginHandle, "Player vehicle mounted");

        }
        else
        {
//            gSdk->logger->Trace(gPluginHandle, "Player vehicle not-mounted");
        }
    }
}



void SpaceMonkeySystem::OnInitialize(const JobHandle& aJob)
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
}

void SpaceMonkeySystem::OnUninitialize()
{
    if (m_telemetryAPI != nullptr)
    {
        m_telemetryAPI->Deinit();
        delete m_telemetryAPI;
        m_telemetryAPI = nullptr;
    }
}
