#include <RED4ext/RED4ext.hpp>
#include <RED4ext/SystemUpdate.hpp>
#include <RED4ext/Scripting/Natives/GameTime.hpp>
#include <RED4ext/Scripting/Natives/ScriptGameInstance.hpp>

#include "SpaceMonkeySystem.h"


RED4EXT_C_EXPORT bool RED4EXT_CALL Main(RED4ext::PluginHandle aHandle, RED4ext::EMainReason aReason,
                                        const RED4ext::Sdk* aSdk)
{
    switch (aReason)
    {
    case RED4ext::EMainReason::Load:
    {

        gSdk = aSdk;
        gPluginHandle = aHandle;

        TypeInfoRegistrar::RegisterDiscovered();

        /*
         * Here you can register your custom functions, initalize variable, create hooks and so on.
         *
         * Be sure to store the plugin handle and the interface because you cannot get it again later. The plugin handle
         * is what identify your plugin through the extender.
         *
         * Returning "true" in this function loads the plugin, returning "false" will unload it and "Main" will be
         * called with "Unload" reason.
         */
        
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
