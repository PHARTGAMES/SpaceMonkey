#include <RED4ext/RED4ext.hpp>
#include <RED4ext/SystemUpdate.hpp>
#include <RED4ext/Scripting/Natives/GameTime.hpp>
#include <RED4ext/Scripting/Natives/ScriptGameInstance.hpp>

#include "SpaceMonkeySystem.h"



bool BaseInit_OnEnter(RED4ext::CGameApplication* aApp)
{


    return true;
}

bool BaseInit_OnUpdate(RED4ext::CGameApplication* aApp)
{
    return true;
}

bool BaseInit_OnExit(RED4ext::CGameApplication* aApp)
{
    return true;
}



bool Running_OnEnter(RED4ext::CGameApplication* aApp)
{
    auto system = Red::GetGameSystem<SpaceMonkeySystem>();

    if (system)
    {
        gSdk->logger->Trace(gPluginHandle, "Running_OnEnter");
    }

    return true;
}

bool Running_OnUpdate(RED4ext::CGameApplication* aApp)
{
//    gSdk->logger->Trace(gPluginHandle, "Running_OnUpdate");
//
//    RED4ext::ScriptGameInstance gameInstance;
//    RED4ext::Handle<RED4ext::IScriptable> playerHandle;
//    RED4ext::ExecuteGlobalFunction("GetPlayer;GameInstance", &playerHandle, gameInstance);
//
//    if (playerHandle)
//    {
//
//        gSdk->logger->Trace(gPluginHandle, "Got player handle");
//
//        auto rtti = RED4ext::CRTTISystem::Get();
//        auto playerPuppetCls = rtti->GetClass("PlayerPuppet");
////        auto inCrouch = playerPuppetCls->GetProperty("inCrouch");
////        auto value = inCrouch->GetValue<bool>(handle.instance);
//
//
////        auto getHudManagerFunc = playerPuppetCls->GetFunction("GetHudManager");
//        auto getMountedVehicleFunc = playerPuppetCls->GetFunction("GetMountedVehicle");
//
//        RED4ext::Handle<RED4ext::IScriptable> mountedVehicle;
//        RED4ext::ExecuteFunction(playerHandle, getMountedVehicleFunc, &mountedVehicle);
//
//        if (mountedVehicle)
//        {
//            auto vehicleObjectCls = rtti->GetClass("VehicleObject");
//            gSdk->logger->Trace(gPluginHandle, "Player vehicle mounted");
//        }
//        else
//        {
//            gSdk->logger->Trace(gPluginHandle, "Player vehicle not-mounted");
//        }
//    }

    return false;
}

bool Running_OnExit(RED4ext::CGameApplication* aApp)
{
    return true;
}

bool Shutdown_OnEnter(RED4ext::CGameApplication* aApp)
{
    return true;
}

bool Shutdown_OnUpdate(RED4ext::CGameApplication* aApp)
{
    return true;
}

bool Shutdown_OnExit(RED4ext::CGameApplication* aApp)
{
    return true;
}

RED4ext::UpdateRegistrar updateRegistrar;



//SpaceMonkeyUpdatableSystem updateSystem;


RED4EXT_C_EXPORT bool RED4EXT_CALL Main(RED4ext::PluginHandle aHandle, RED4ext::EMainReason aReason,
                                        const RED4ext::Sdk* aSdk)
{
    switch (aReason)
    {
    case RED4ext::EMainReason::Load:
    {

        gSdk = aSdk;
        gPluginHandle = aHandle;
        /*
         * Here you can register your custom functions, initalize variable, create hooks and so on.
         *
         * Be sure to store the plugin handle and the interface because you cannot get it again later. The plugin handle
         * is what identify your plugin through the extender.
         *
         * Returning "true" in this function loads the plugin, returning "false" will unload it and "Main" will be
         * called with "Unload" reason.
         */
        

        RED4ext::GameState initState;
        initState.OnEnter = &BaseInit_OnEnter;
        initState.OnUpdate = &BaseInit_OnUpdate;
        initState.OnExit = &BaseInit_OnExit;

        aSdk->gameStates->Add(aHandle, RED4ext::EGameStateType::BaseInitialization, &initState);


        RED4ext::GameState runningState;
        runningState.OnEnter = &Running_OnEnter;
        runningState.OnUpdate = &Running_OnUpdate;
        runningState.OnExit = &Running_OnExit;

        aSdk->gameStates->Add(aHandle, RED4ext::EGameStateType::Running, &runningState);

        RED4ext::GameState shutdownState;
        shutdownState.OnEnter = Shutdown_OnEnter;
        shutdownState.OnUpdate = &Shutdown_OnUpdate;
        shutdownState.OnExit = Shutdown_OnExit;

        aSdk->gameStates->Add(aHandle, RED4ext::EGameStateType::Shutdown, &shutdownState);


        break;
    }
    case RED4ext::EMainReason::Unload:
    {
        /*
         * Here you can free resources you allocated during initalization or during the time your plugin was executed.
         */
        break;
    }
    }

    /*
     * For more information about this function see https://docs.red4ext.com/mod-developers/creating-a-plugin#main.
     */

    return true;
}

RED4EXT_C_EXPORT void RED4EXT_CALL Query(RED4ext::PluginInfo* aInfo)
{
    /*
     * This function supply the necessary information about your plugin, like name, version, support runtime and SDK. DO
     * NOT do anything here yet!
     *
     * You MUST have this function!
     *
     * Make sure to fill all of the fields here in order to load your plugin correctly.
     *
     * Runtime version is the game's version, it is best to let it set to "RED4EXT_RUNTIME_LATEST" if you want to target
     * the latest game's version that the SDK defined, if the runtime version specified here and the game's version do
     * not match, your plugin will not be loaded. If you want to use RED4ext only as a loader and you do not care about
     * game's version use "RED4EXT_RUNTIME_INDEPENDENT".
     *
     * For more information about this function see https://docs.red4ext.com/mod-developers/creating-a-plugin#query.
     */

    aInfo->name = L"CP2077SMMod";
    aInfo->author = L"PEZZALUCIFER";
    aInfo->version = RED4EXT_SEMVER(1, 0, 0);
    aInfo->runtime = RED4EXT_RUNTIME_LATEST;
    aInfo->sdk = RED4EXT_SDK_LATEST;
}

RED4EXT_C_EXPORT uint32_t RED4EXT_CALL Supports()
{
    /*
     * This functions returns only what API version is support by your plugins.
     * You MUST have this function!
     *
     * For more information about this function see https://docs.red4ext.com/mod-developers/creating-a-plugin#supports.
     */
    return RED4EXT_API_VERSION_LATEST;
}
