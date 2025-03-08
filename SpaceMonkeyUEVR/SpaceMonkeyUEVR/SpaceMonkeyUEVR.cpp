#define _CRT_SECURE_NO_WARNINGS
#include "pch.h"
#include "uevr/Plugin.hpp"
#include "SpaceMonkeyUEVR.h"
#include "GPSimple.h"
#include "windows.h"
#include <string>

void SpaceMonkeyUEVR::load_game_plugin()
{
	if (m_game_plugin != nullptr)
		delete m_game_plugin;

	UEVRGameConfig* game_config = jsonHelperLoadFile<UEVRGameConfig>(get_module_path().c_str(), "sm_game_config.json");

	if (game_config != nullptr)
	{
		if (game_config->m_plugin_type.compare("GPSimple") == 0)
		{
			m_game_plugin = new GPSimple(game_config);
		}
	}

}


void SpaceMonkeyUEVR::on_initialize() 
{
    // Logs to the appdata UnrealVRMod log.txt file
    //API::get()->log_error("%s %s", "Hello", "error");
    //API::get()->log_warn("%s %s", "Hello", "warning");
    //API::get()->log_info("%s %s", "Hello", "info");

    load_game_plugin();
}


void SpaceMonkeyUEVR::on_pre_engine_tick(API::UGameEngine* engine, float delta) 
{
    //PLUGIN_LOG_ONCE("Pre Engine Tick: %f", delta);

    if(m_game_plugin != nullptr)
    {
        m_game_plugin->on_pre_engine_tick(engine, delta);
    }
}

void SpaceMonkeyUEVR::on_post_engine_tick(API::UGameEngine* engine, float delta) 
{
    //PLUGIN_LOG_ONCE("Post Engine Tick: %f", delta);

    if (m_game_plugin != nullptr)
    {
        m_game_plugin->on_post_engine_tick(engine, delta);
    }
}

void SpaceMonkeyUEVR::on_pre_slate_draw_window(UEVR_FSlateRHIRendererHandle renderer, UEVR_FViewportInfoHandle viewport_info) 
{
    //PLUGIN_LOG_ONCE("Pre Slate Draw Window");
}

void SpaceMonkeyUEVR::on_post_slate_draw_window(UEVR_FSlateRHIRendererHandle renderer, UEVR_FViewportInfoHandle viewport_info) 
{
    //PLUGIN_LOG_ONCE("Post Slate Draw Window");
}




std::string SpaceMonkeyUEVR::get_module_path()
{
    char path[MAX_PATH] = { 0 };
    HMODULE hModule = NULL;

    // Get the module handle for this function (inside the DLL)
    if (GetModuleHandleExA(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
        reinterpret_cast<LPCSTR>(&SpaceMonkeyUEVR::get_module_path), &hModule)) {
        if (GetModuleFileNameA(hModule, path, MAX_PATH)) {
            // Use filesystem to strip off the filename and keep only the directory
            return std::filesystem::path(path).parent_path().string();
        }
    }

    // On any failure, return empty string
    return "";
}

