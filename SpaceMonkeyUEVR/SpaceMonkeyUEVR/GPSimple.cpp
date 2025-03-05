#include "GPSimple.h"
#include "UEVRUtils.h"
#include "UObjectStructs.h"
#include "UEVRFunctions.h"
#include "SpaceMonkeyTelemetryAPI.h"

GPSimple::GPSimple(UEVRGameConfig* a_game_config) : UEVRGamePlugin(a_game_config)
{
	m_game_config_gp_simple = static_cast<GPSimpleConfig*>(a_game_config);

	m_systemTime = 0.0;

	API::get()->log_info("Created GPSimple Instance");

}


void GPSimple::on_pre_engine_tick(API::UGameEngine* engine, float delta)
{

}


bool GPSimple::is_correct_pawn(API::UObject* object)
{
	if (object == nullptr)
		return false;

	//find m_pawn_display_name_substring within pawn name
	if (object->get_full_name().find(string_to_wstring(m_game_config_gp_simple->m_pawn_display_name_substring)) == std::wstring::npos)
		return false;

	return true;
}



void GPSimple::on_post_engine_tick(API::UGameEngine* engine, float delta)
{
	const auto pawn = API::get()->get_local_pawn(0);


	//valid pawn
	if (is_correct_pawn(pawn))
	{
		FVectorDouble location;
		FVectorDouble forward;
		FVectorDouble up;
		FVectorDouble right;
		get_actor_transform_vectors(pawn, &location, &forward, &up, &right, m_game_config_gp_simple->m_use_doubles);

		memset(&m_frameData, 0, sizeof(SpaceMonkeyTelemetryFrameData));

		m_systemTime += delta;
		m_frameData.m_time = m_systemTime;
		
		double toMeters = 0.01;

		m_frameData.m_posX = location.Y * toMeters;
		m_frameData.m_posY = location.Z * toMeters;
		m_frameData.m_posZ = location.X * toMeters;

		m_frameData.m_fwdX = forward.Y;
		m_frameData.m_fwdY = forward.Z;
		m_frameData.m_fwdZ = forward.X;

		m_frameData.m_upX = up.Y;
		m_frameData.m_upY = up.Z;
		m_frameData.m_upZ = up.X;

		m_telemetryAPI->SendFrame(&m_frameData);
	}
	else
	{
		m_systemTime = 0.0;
	}

}