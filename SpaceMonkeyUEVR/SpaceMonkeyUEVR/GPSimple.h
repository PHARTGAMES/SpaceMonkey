#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include "UEVRGamePlugin.h"
#include "UEVRGameConfig.h"
#include "nlohmann/json.hpp"
#include "JsonHelper.h"
using json = nlohmann::json;

class GPSimpleConfig : public UEVRGameConfig 
{
public:
	std::string m_object_path;
	std::string m_object_class;


};

class GPSimple : public UEVRGamePlugin
{
public:
	GPSimple(UEVRGameConfig* a_game_config);

    virtual void on_pre_engine_tick(API::UGameEngine* engine, float delta) override;

    virtual void on_post_engine_tick(API::UGameEngine* engine, float delta) override;

	GPSimpleConfig* m_game_config_gp_simple;

};


inline void to_json(json& a_json, const GPSimpleConfig& a_config)
{
	to_json(a_json, static_cast<const UEVRGameConfig&>(a_config));

	a_json = json{ {"m_object_path", a_config.m_object_path}, {"m_object_class", a_config.m_object_class} };
}

inline void to_json(json& a_json, const GPSimpleConfig* a_config)
{
	to_json(a_json, *a_config);
}

inline void from_json(const json& a_json, GPSimpleConfig& a_config)
{
	from_json(a_json, static_cast<UEVRGameConfig&>(a_config));

	JsonGetOptional(a_json, "m_object_path", a_config.m_object_path);
	JsonGetOptional(a_json, "m_object_class", a_config.m_object_class);
}
