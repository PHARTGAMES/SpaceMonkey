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

