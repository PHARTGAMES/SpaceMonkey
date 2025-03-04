#include "GPSimple.h"
#include "UEVRUtils.h"
#include "UObjectStructs.h"
#include "UEVRFunctions.h"
#include "SpaceMonkeyTelemetryAPI.h"

GPSimple::GPSimple(UEVRGameConfig* a_game_config) : UEVRGamePlugin(a_game_config)
{
	m_game_config_gp_simple = static_cast<GPSimpleConfig*>(a_game_config);

	m_selected_pawn = nullptr;

	m_systemTime = 0.0;

	API::get()->log_info("Created GPSimple Instance");

}


void GPSimple::on_pre_engine_tick(API::UGameEngine* engine, float delta)
{

}





void GPSimple::on_post_engine_tick(API::UGameEngine* engine, float delta)
{

	if(m_selected_pawn == nullptr)
	{ 
		const auto pawn = API::get()->get_local_pawn(0);

		//valid pawn
		if (pawn != nullptr)
		{
			//find m_pawn_display_name_substring within pawn name
			if (pawn->get_full_name().find(string_to_wstring(m_game_config_gp_simple->m_pawn_display_name_substring)) != std::wstring::npos)
			{
				m_selected_pawn = pawn;
			}
			else
			{
				m_selected_pawn = nullptr;
			}
		}
		else
		{
			m_selected_pawn = nullptr;
		}

	}


	//valid pawn
	if (m_selected_pawn != nullptr)
	{

		//FRotatorDouble rotation;
		//FVectorDouble location;
		//get_actor_transform(m_selected_pawn, &location, &rotation, m_game_config_gp_simple->m_use_doubles);


		FVectorDouble location;
		FVectorDouble forward;
		FVectorDouble up;
		FVectorDouble right;
		get_actor_transform_vectors(m_selected_pawn, &location, &forward, &up, &right, m_game_config_gp_simple->m_use_doubles);

		memset(&m_frameData, 0, sizeof(SpaceMonkeyTelemetryFrameData));

		m_systemTime += delta;
		m_frameData.m_time = m_systemTime;
		
		m_frameData.m_posX = location.X;
		m_frameData.m_posY = location.Y;
		m_frameData.m_posZ = location.Z;

		m_frameData.m_fwdX = forward.X;
		m_frameData.m_fwdY = forward.Y;
		m_frameData.m_fwdZ = forward.Z;

		m_frameData.m_upX = up.X;
		m_frameData.m_upY = up.Y;
		m_frameData.m_upZ = up.Z;

		m_telemetryAPI->SendFrame(&m_frameData);
	}


}