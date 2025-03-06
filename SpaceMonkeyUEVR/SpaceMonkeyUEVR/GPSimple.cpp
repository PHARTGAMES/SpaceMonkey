#include "GPSimple.h"
#include "UEVRUtils.h"
#include "UObjectStructs.h"
#include "UEVRFunctions.h"
#include "SpaceMonkeyTelemetryAPI.h"

GPSimple::GPSimple(UEVRGameConfig* a_game_config) : UEVRGamePlugin(a_game_config)
{
	m_game_config_gp_simple = static_cast<GPSimpleConfig*>(a_game_config);

	m_systemTime = 0.0;
	m_resolved_object = nullptr;

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

		if (m_resolved_object == nullptr)
		{
			m_resolved_object = get_child_object_for_path(pawn, m_game_config_gp_simple->m_object_path);
		}

		if (m_resolved_object != nullptr)
		{
			FVectorDouble location;
			FVectorDouble forward;
			FVectorDouble up;
			FVectorDouble right;
			get_actor_transform_vectors(m_resolved_object, &location, &forward, &up, &right, m_game_config_gp_simple->m_use_doubles);

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
	}
	else
	{
		m_systemTime = 0.0;
		m_resolved_object = nullptr;
	}

}



API::UObject* GPSimple::get_child_object_for_path(API::UObject* a_object, std::vector<std::string>& a_object_path)
{
	if (a_object_path.size() == 0)
	{
		API::get()->log_info("get_child_object_for_path called with empty a_object_path");
		
		return a_object;
	}

	API::UObject* curr_object = a_object;

	try
	{

		for (int o = 0; o < a_object_path.size(); ++o)
		{
			std::string curr_path = a_object_path[o];

			//properties
			if (curr_path.compare("Properties") == 0)
			{
				//get next path
				curr_path = a_object_path[++o];

				//try get as an array
				const auto& object_array = curr_object->get_property<API::TArray<API::UObject*>>(string_to_wstring(curr_path));

				//is an array
				if (&object_array != nullptr && object_array.count != 0)
				{
					for (int c = 0; c < object_array.count; ++c)
					{
						API::UObject* test_object = object_array.data[c];
						if (test_object != nullptr)
						{
							std::wstring obj_name = test_object->get_fname()->to_string();

							//found object
							if (obj_name.find(string_to_wstring(curr_path)) != std::wstring::npos)
							{
								curr_object = test_object;

								API::get()->log_info("get_child_object_for_path matched property %s to %s", curr_path.c_str(), curr_object->get_full_name().c_str());
								//go back to start of the o loop
								break;
							}
						}
					}
				}
				else //single object
				{
					API::UObject* object = curr_object->get_property<API::UObject*>(string_to_wstring(curr_path));
					if (object != nullptr)
					{
						curr_object = a_object;
					}
				}
			}
			else //components
			if (curr_path.compare("Components") == 0)
			{
				//get next path
				curr_path = a_object_path[++o];

				GetActorComponentsParams<double> get_actor_component_function_params;
				get_actor_component_function_params.c = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.ActorComponent");
				call_function_on_uobject<GetActorComponentsParams<double>>(curr_object, &get_actor_component_function_params);

				if (get_actor_component_function_params.return_value.empty())
				{
					API::get()->log_error("get_child_object_for_path Failed to find any ActorComponents");
				}

				for (auto component : get_actor_component_function_params.return_value)
				{
					std::wstring obj_name = component->get_fname()->to_string();

					//found object
					if (obj_name.find(string_to_wstring(curr_path)) != std::wstring::npos)
					{
						curr_object = component;

						API::get()->log_info("get_child_object_for_path matched component %s to %s", curr_path.c_str(), curr_object->get_full_name().c_str());
						//go back to start of the o loop
						break;
					}
				}
			}


		}

	}
	catch (std::exception* e)
	{

	}

	API::get()->log_info("get_child_object_for_path resolved %s", curr_object->get_full_name().c_str());

	return curr_object;
}