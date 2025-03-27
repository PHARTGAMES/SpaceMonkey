#include <RED4ext/RED4ext.hpp>
#include "SpaceMonkeySystem.h"
using namespace Red;

void SpaceMonkeySystem::OnRegisterUpdates(Red::UpdateRegistrar* aRegistrar)
{
    aRegistrar->RegisterUpdate(RED4ext::UpdateTickGroup::PostBuckets, this, "SpaceMonkeySystemUpdate",
                               [this](Red::FrameInfo& aInfo, Red::JobQueue& aQueue) { Tick(aInfo, aQueue); });
}

void SpaceMonkeySystem::Tick(Red::FrameInfo& aInfo, Red::JobQueue& aQueue) noexcept
{
    gSdk->logger->Trace(gPluginHandle, "Running_OnUpdate");

    RED4ext::ScriptGameInstance gameInstance;
    RED4ext::Handle<RED4ext::IScriptable> playerHandle;
    RED4ext::ExecuteGlobalFunction("GetPlayer;GameInstance", &playerHandle, gameInstance);

    if (playerHandle)
    {
        gSdk->logger->Trace(gPluginHandle, "Got player handle");

        auto rtti = RED4ext::CRTTISystem::Get();
        auto playerPuppetCls = rtti->GetClass("PlayerPuppet");
        //        auto inCrouch = playerPuppetCls->GetProperty("inCrouch");
        //        auto value = inCrouch->GetValue<bool>(handle.instance);

        //        auto getHudManagerFunc = playerPuppetCls->GetFunction("GetHudManager");
        auto getMountedVehicleFunc = playerPuppetCls->GetFunction("GetMountedVehicle");

        RED4ext::Handle<RED4ext::IScriptable> mountedVehicle;
        RED4ext::ExecuteFunction(playerHandle, getMountedVehicleFunc, &mountedVehicle);

        if (mountedVehicle)
        {
            auto vehicleObjectCls = rtti->GetClass("VehicleObject");
            gSdk->logger->Trace(gPluginHandle, "Player vehicle mounted");
        }
        else
        {
            gSdk->logger->Trace(gPluginHandle, "Player vehicle not-mounted");
        }
    }
}
