#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include<string>
#include "nlohmann/json.hpp"
#include "JsonHelper.h"
using json = nlohmann::json;

class UEVRGameConfig
{
public:

	std::string m_plugin_type;
	bool m_use_doubles;
	int m_spawn_actor_version;
};


inline void to_json(json& a_json, const UEVRGameConfig& a_config)
{
	a_json = json{ {"m_plugin_type", a_config.m_plugin_type}, {"m_use_doubles", a_config.m_use_doubles}, {"m_spawn_actor_version", a_config.m_spawn_actor_version} };
}


inline void from_json(const json& a_json, UEVRGameConfig& a_config)
{
	JsonGetOptional(a_json, "m_plugin_type", a_config.m_plugin_type);
	JsonGetOptional(a_json, "m_use_doubles", a_config.m_use_doubles, false);
	JsonGetOptional(a_json, "m_spawn_actor_version", a_config.m_spawn_actor_version, 0);
}



