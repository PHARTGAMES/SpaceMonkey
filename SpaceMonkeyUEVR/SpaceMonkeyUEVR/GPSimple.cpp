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
	m_selected_pawn_name = L"";
	m_world = nullptr;
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

	//is locally controlled?
	PawnIsLocallyControlledFunctionParams<double> pawn_is_locally_controlled_params;
	call_function_on_uobject<PawnIsLocallyControlledFunctionParams<double>>(object, &pawn_is_locally_controlled_params);

	//is player controlled?
	PawnIsPlayerControlledFunctionParams<double> pawn_is_player_controlled_params;
	call_function_on_uobject<PawnIsPlayerControlledFunctionParams<double>>(object, &pawn_is_player_controlled_params);

	//local and player controlled?
	if (pawn_is_locally_controlled_params.return_value != true || pawn_is_player_controlled_params.return_value != true)
		return false;

	return true;
}


void GPSimple::resolve_world(API::UGameEngine* engine)
{

	// Check if we can find the GameInstance and call is_a() on it.
	const auto game_instance = engine->get_property<API::UObject*>(L"GameInstance");

	if (game_instance != nullptr) {
		const auto game_instance_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.GameInstance");

		if (game_instance->is_a(game_instance_class)) {
			const auto& local_players = game_instance->get_property<API::TArray<API::UObject*>>(L"LocalPlayers");

			if (local_players.count > 0 && local_players.data != nullptr) {
				const auto local_player = local_players.data[0];

				const auto viewport_client = local_player->get_property<API::UObject*>(L"ViewportClient");

				if (viewport_client != nullptr) 
				{
					m_world = viewport_client->get_property<API::UWorld*>(L"World");

					API::get()->log_info("found world");
				}

			}
			else {
				API::get()->log_error("Failed to find LocalPlayers");
			}

			API::get()->log_info("GameInstance is a UGameInstance");
		}
		else {
			API::get()->log_error("GameInstance is not a UGameInstance");
		}
	}
	else {
		API::get()->log_error("Failed to find GameInstance");
	}
}


void GPSimple::on_post_engine_tick(API::UGameEngine* engine, float delta)
{

	try
	{

		const auto pawn = API::get()->get_local_pawn(m_pawn_index);
		if (pawn == nullptr) //null pawn, past the end of the local pawns
		{
			m_pawn_index = 0;
		}

		//valid pawn
		if (pawn != nullptr && is_correct_pawn(pawn))
		{

			std::wstring pawn_name = pawn->get_full_name();

			//pawn changed!
			if (m_selected_pawn_name.compare(pawn_name) != 0)
			{
				API::get()->log_info("on_post_engine_tick pawn changed from %s to %s", wstring_to_string(m_selected_pawn_name).c_str(), wstring_to_string(pawn_name).c_str());

				m_selected_pawn_name = pawn_name;
				m_resolved_object = nullptr;
				m_systemTime = 0.0;
				m_world = nullptr;
			}

			//if (m_world == nullptr)
			//{
			//	resolve_world(engine);
			//}

			//resolve child object
			if (m_resolved_object == nullptr)
			{
				m_resolved_object = get_child_object_for_path(pawn, m_game_config_gp_simple->m_object_path);

				if (m_resolved_object != nullptr)
				{
					m_resolved_object = create_transform_offset_object(m_resolved_object, pawn, m_game_config_gp_simple->m_use_doubles, m_game_config_gp_simple->m_spawn_actor_version);
				}
			}

			//resolved something, extract telemetry
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
			m_selected_pawn_name = L"";
			m_pawn_index++; //try next local pawn
			m_world = nullptr;
		}

	}
	catch (const std::exception& e)
	{
		API::get()->log_error("on_post_engine_tick exception: %s", e.what());
		m_pawn_index = 0;
		m_resolved_object = nullptr;
		m_systemTime = 0.0;
		m_world = nullptr;
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

				//
				//fixme: need some way to determine if it's an array before trying to call get_property; for now only single uobjects supported.
				//
				////try get as an array
				//const auto& object_array = curr_object->get_property<API::TArray<API::UObject*>>(string_to_wstring(curr_path));

				////is an array
				//if (&object_array != nullptr && object_array.count != 0 && object_array.data != nullptr)
				//{
				//	for (int c = 0; c < object_array.count; ++c)
				//	{
				//		API::UObject* test_object = object_array.data[c];
				//		if (test_object != nullptr)
				//		{
				//			std::wstring obj_name = test_object->get_fname()->to_string();

				//			//found object
				//			if (obj_name.find(string_to_wstring(curr_path)) != std::wstring::npos)
				//			{
				//				curr_object = test_object;

				//				API::get()->log_info("get_child_object_for_path matched property %s to %s", curr_path.c_str(), wstring_to_string(curr_object->get_full_name()).c_str());
				//				//go back to start of the o loop
				//				break;
				//			}
				//		}
				//	}
				//}
				//else //single object
				{
					API::UObject* object = curr_object->get_property<API::UObject*>(string_to_wstring(curr_path));
					if (object != nullptr)
					{
						curr_object = object;
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

						API::get()->log_info("get_child_object_for_path matched component %s to %s", curr_path.c_str(), wstring_to_string(curr_object->get_full_name()).c_str());
						//go back to start of the o loop
						break;
					}
				}
			}
		}

	}
	catch (const std::exception& e)
	{
		//banana in the tail piep
		API::get()->log_error("get_child_object_for_path exception: %s", e.what());
	}

	//found something, log it
	if (curr_object != nullptr)
	{
		API::get()->log_info("get_child_object_for_path resolved %s", wstring_to_string(curr_object->get_full_name()).c_str());
	}

	return curr_object;
}



// Templated helper function that does the spawning
template <typename T, int Version>
API::UObject* create_transform_offset_object_impl(API::UObject* a_parent, API::UObject* a_pawn)
{
	auto ugameplay_statics_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.GameplayStatics");
	API::UObject* statics_default_object = ugameplay_statics_class->get_class_default_object();

	if (statics_default_object != nullptr)
	{
		// Set up a transform of type T
		TTransform<T> trans;
		trans.rotation = TVector4<T>(0, 0, 0, 1);
		trans.translation = TVector4<T>(0, 0, 0, 0);
		trans.scale3d = TVector4<T>(1, 1, 1, 1);

		API::UClass* actor_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.Actor");

		// Fill begin parameters
		BeginDeferredActorSpawnFromClassParams<T, Version> begin_params;
		begin_params.pawn = a_pawn;
		begin_params.actor_class = actor_class;
		begin_params.spawn_transform = trans;
		begin_params.collision_handling_override = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
		begin_params.owner = a_parent;
		if constexpr (Version == 1)
		{
			begin_params.tranform_scale_method = ESpawnActorScaleMethod::OverrideRootScale;
		}

		statics_default_object->call_function(L"BeginDeferredActorSpawnFromClass", &begin_params);

		if (begin_params.return_value != nullptr)
		{
			API::get()->log_info("create_transform_offset_object spawned actor: %s",
				wstring_to_string(begin_params.return_value->get_full_name()).c_str());
		}

		API::UObject* spawned_actor = begin_params.return_value;

		// Finish spawning if the actor was successfully spawned
		if (spawned_actor != nullptr)
		{
			FinishSpawningActorParams<T, Version> finish_params;
			finish_params.actor = spawned_actor;
			finish_params.spawn_transform = trans;
			if constexpr (Version == 1)
			{
				finish_params.transform_scale_method = ESpawnActorScaleMethod::OverrideRootScale;
			}

			statics_default_object->call_function(L"FinishSpawningActor", &finish_params);

			if (finish_params.return_value != nullptr)
			{
				API::get()->log_info("create_transform_offset_object finished spawning actor: %s",
					wstring_to_string(finish_params.return_value->get_full_name()).c_str());
				return finish_params.return_value;
			}
		}
	}
	return a_parent;
}

// Main dispatch function – selects the proper template instantiation based on a_use_double and a_spawn_actor_version
API::UObject* GPSimple::create_transform_offset_object(API::UObject* a_parent, API::UObject* a_pawn, bool a_use_double, int a_spawn_actor_version)
{
	if (a_use_double)
	{
		switch (a_spawn_actor_version)
		{
		case 0:
			return create_transform_offset_object_impl<double, 0>(a_parent, a_pawn);
		case 1:
			return create_transform_offset_object_impl<double, 1>(a_parent, a_pawn);
		default:
			break;
		}
	}
	else
	{
		switch (a_spawn_actor_version)
		{
		case 0:
			return create_transform_offset_object_impl<float, 0>(a_parent, a_pawn);
		case 1:
			return create_transform_offset_object_impl<float, 1>(a_parent, a_pawn);
		default:
			break;
		}
	}
	return a_parent;
}
