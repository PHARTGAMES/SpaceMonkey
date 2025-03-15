#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include "UEVRGamePlugin.h"
#include "UEVRGameConfig.h"
#include "nlohmann/json.hpp"
#include "JsonHelper.h"
#include "uevr/API.hpp"

using json = nlohmann::json;



class TransformOffset
{
public:

	double m_locationX;
	double m_locationY;
	double m_locationZ;
	double m_rotationPitch;
	double m_rotationYaw;
	double m_rotationRoll;
};


inline void to_json(json& a_json, const TransformOffset& a_config)
{
	a_json = json{ {"m_locationX", a_config.m_locationX}, };
}


inline void from_json(const json& a_json, TransformOffset& a_config)
{
	JsonGetOptional(a_json, "m_locationX", a_config.m_locationX, 0.0);
	JsonGetOptional(a_json, "m_locationY", a_config.m_locationY, 0.0);
	JsonGetOptional(a_json, "m_locationZ", a_config.m_locationZ, 0.0);
	JsonGetOptional(a_json, "m_rotationPitch", a_config.m_rotationPitch, 0.0);
	JsonGetOptional(a_json, "m_rotationYaw", a_config.m_rotationYaw, 0.0);
	JsonGetOptional(a_json, "m_rotationRoll", a_config.m_rotationRoll, 0.0);
}



class GPSimpleConfig : public UEVRGameConfig 
{
public:
	std::string m_pawn_display_name_substring;
	std::vector<std::string> m_object_path;
	TransformOffset m_transform_offset;
};

class GPSimple : public UEVRGamePlugin
{
protected:
	GPSimpleConfig* m_game_config_gp_simple;
		
	double m_system_time;
	API::UObject* m_resolved_object;
	int m_pawn_index = 0;
	std::wstring m_selected_pawn_name;
	API::UClass* m_scene_component_class;
	API::UClass* m_actor_class;

	double m_last_present_time;


public:
	GPSimple(UEVRGameConfig* a_game_config);

    virtual void on_pre_engine_tick(API::UGameEngine* engine, float delta) override;

    virtual void on_post_engine_tick(API::UGameEngine* engine, float delta) override;

	virtual void on_present() override;

	bool is_correct_pawn(API::UObject* object);

	API::UObject* get_child_object_for_path(API::UObject* a_actor, std::vector<std::string>& a_object_path);

	void get_scenecomponent_transform_vectors(uevr::API::UObject* uobject, FVectorDouble* location, FVectorDouble* forward, FVectorDouble* up, FVectorDouble* right, bool doubles, const TransformOffset& transform_offset);

	void tick(float delta);

	void reset_system_time();

};


inline void to_json(json& a_json, const GPSimpleConfig& a_config)
{
	to_json(a_json, static_cast<const UEVRGameConfig&>(a_config));

	a_json = json{ {"m_pawn_display_name_substring", a_config.m_pawn_display_name_substring}, {"m_transform_offset", a_config.m_transform_offset} };
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
	JsonGetOptional(a_json, "m_transform_offset", a_config.m_transform_offset);
}





