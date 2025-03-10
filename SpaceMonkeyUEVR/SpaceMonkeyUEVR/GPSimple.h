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
	std::vector<std::string> m_object_path;
};

class GPSimple : public UEVRGamePlugin
{
protected:
	GPSimpleConfig* m_game_config_gp_simple;
		
	double m_systemTime;
	API::UObject* m_resolved_object;
	int m_pawn_index = 0;
	std::wstring m_selected_pawn_name;
	API::UWorld* m_world;

public:
	GPSimple(UEVRGameConfig* a_game_config);

    virtual void on_pre_engine_tick(API::UGameEngine* engine, float delta) override;

    virtual void on_post_engine_tick(API::UGameEngine* engine, float delta) override;

	bool is_correct_pawn(API::UObject* object);

	API::UObject* get_child_object_for_path(API::UObject* a_object, std::vector<std::string>& a_object_path);

	API::UObject* create_transform_offset_object(API::UObject* a_parent, API::UObject* a_pawn, bool a_use_double, int a_spawn_actor_version);

	void resolve_world(API::UGameEngine* engine);

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
	JsonGetOptional(a_json, "m_object_path", a_config.m_object_path);
}





