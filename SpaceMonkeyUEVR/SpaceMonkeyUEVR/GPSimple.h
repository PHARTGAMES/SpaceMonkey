#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include "UEVRGamePlugin.h"
#include "UEVRGameConfig.h"
#include "nlohmann/json.hpp"
#include "JsonHelper.h"
#include "uevr/API.hpp"

using json = nlohmann::json;

class GPSimpleConfig : public UEVRGameConfig 
{
public:
	std::string m_pawn_display_name_substring;


};

class GPSimple : public UEVRGamePlugin
{
protected:
	GPSimpleConfig* m_game_config_gp_simple;

	API::UObject* m_selected_pawn = nullptr;

public:
	GPSimple(UEVRGameConfig* a_game_config);

    virtual void on_pre_engine_tick(API::UGameEngine* engine, float delta) override;

    virtual void on_post_engine_tick(API::UGameEngine* engine, float delta) override;




};


inline void to_json(json& a_json, const GPSimpleConfig& a_config)
{
	to_json(a_json, static_cast<const UEVRGameConfig&>(a_config));

	a_json = json{ {"m_pawn_display_name_substring", a_config.m_pawn_display_name_substring} };
}

inline void to_json(json& a_json, const GPSimpleConfig* a_config)
{
	to_json(a_json, *a_config);
}

inline void from_json(const json& a_json, GPSimpleConfig& a_config)
{
	from_json(a_json, static_cast<UEVRGameConfig&>(a_config));

	JsonGetOptional(a_json, "m_pawn_display_name_substring", a_config.m_pawn_display_name_substring);
}
