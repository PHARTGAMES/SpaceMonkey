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

			if (m_world == nullptr)
			{
				resolve_world(engine);
			}

			//resolve child object
			if (m_resolved_object == nullptr)
			{
				m_resolved_object = get_child_object_for_path(pawn, m_game_config_gp_simple->m_object_path);

				//if (m_resolved_object != nullptr)
				//{
				//	m_resolved_object = create_transform_offset_object(m_resolved_object);
				//}
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


API::UObject* GPSimple::create_transform_offset_object(API::UObject* a_parent)
{


	//ActorGetWorldFunctionParams<double> actor_get_world_function_params;
	//call_function_on_uobject<ActorGetWorldFunctionParams<double>>(a_parent, &actor_get_world_function_params);

	//const API::UObject* world = actor_get_world_function_params.return_value;


	if (m_world != nullptr)
	{
		//FActorSpawnParameters spawn_params;
		//memset(&spawn_params, 0, sizeof(FActorSpawnParameters));
		//spawn_params.name = API::FName(std::wstring(L"SpaceMonkeyTransform"));
		//spawn_params.owner = a_parent;
		//spawn_params.spawn_collision_handling_override = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
		//spawn_params.tranform_scale_method = ESpawnActorScaleMethod::OverrideRootScale;

		////fixme:
		////if(m_game_config_gp_simple->m_use_doubles)

		//WorldSpawnActorFunctionParams<double> world_spawn_actor_function_params(spawn_params);
		//memset(&world_spawn_actor_function_params, 0, sizeof(WorldSpawnActorFunctionParams<double>));

		//FVectorDouble location = FVectorDouble(0, 0, 0);
		//FRotatorDouble rotation = FRotatorDouble(0, 0, 0);

		//world_spawn_actor_function_params.actor_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.Actor");
		//world_spawn_actor_function_params.location = &location;
		//world_spawn_actor_function_params.rotation = &rotation;

		//call_function_on_uobject<WorldSpawnActorFunctionParams<double>>((API::UObject*)m_world, &world_spawn_actor_function_params);

		//API::UObject* spawned_actor = world_spawn_actor_function_params.return_value;

		//if (spawned_actor != nullptr)
		//{

		//}

		const auto ugameplay_statics_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.GameplayStatics");

		if (ugameplay_statics_class != nullptr)
		{
			struct FBeginDeferredActorSpawnFromClassParams {
				API::UWorld* world;
				API::UClass* actor_class;
				TTransform<double>& spawn_transform;
				ESpawnActorCollisionHandlingMethod collision_handling_override;
				API::UObject* owner;
				ESpawnActorScaleMethod tranform_scale_method;
				API::UObject* return_value{};
				// Constructor initializing all members.
				FBeginDeferredActorSpawnFromClassParams(
					API::UWorld* InWorld,
					API::UClass* InActorClass,
					TTransform<double>& InSpawnTransform,
					ESpawnActorCollisionHandlingMethod InCollisionHandlingOverride,
					API::UObject* InOwner,
					ESpawnActorScaleMethod InTranformScaleMethod,
					API::UObject* InReturnValue = nullptr
				)
					: world(InWorld)
					, actor_class(InActorClass)
					, spawn_transform(InSpawnTransform)
					, collision_handling_override(InCollisionHandlingOverride)
					, owner(InOwner)
					, tranform_scale_method(InTranformScaleMethod)
					, return_value(InReturnValue)
				{}
			};

			TTransform<double> trans;
			memset(&trans, 0, sizeof(TTransform<double>));

			FBeginDeferredActorSpawnFromClassParams begin_deferred_actor_spawn_from_class_params(m_world,
				API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.Actor"),
				trans,
				ESpawnActorCollisionHandlingMethod::AlwaysSpawn, 
				a_parent,
				ESpawnActorScaleMethod::OverrideRootScale
				);


			//begin_deferred_actor_spawn_from_class_params.world = m_world;
			//begin_deferred_actor_spawn_from_class_params.actor_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.Actor");
			//begin_deferred_actor_spawn_from_class_params.spawn_transform = trans;
			//begin_deferred_actor_spawn_from_class_params.collision_handling_override = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
			//begin_deferred_actor_spawn_from_class_params.owner = a_parent;
			//begin_deferred_actor_spawn_from_class_params.tranform_scale_method = ESpawnActorScaleMethod::OverrideRootScale;

			ugameplay_statics_class->call_function(L"BeginDeferredActorSpawnFromClass", &begin_deferred_actor_spawn_from_class_params);

			if (begin_deferred_actor_spawn_from_class_params.return_value != nullptr)
			{
				API::get()->log_info("create_transform_offset_object spawned actor");
			}


		}


	}

	return a_parent;
}
