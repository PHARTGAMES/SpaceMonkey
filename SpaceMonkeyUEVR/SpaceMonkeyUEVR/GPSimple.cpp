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
					m_resolved_object = create_transform_offset_object(m_resolved_object, pawn, m_game_config_gp_simple->m_use_doubles, m_game_config_gp_simple->m_spawn_actor_version, m_game_config_gp_simple->m_transform_offset);
				}
			}

			//resolved something, extract telemetry
			if (m_resolved_object != nullptr)
			{
				FVectorDouble location;
				FVectorDouble forward;
				FVectorDouble up;
				FVectorDouble right;
				get_actor_transform_vectors(m_resolved_object, &location, &forward, &up, &right, m_game_config_gp_simple->m_use_doubles, m_game_config_gp_simple->m_transform_offset);

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

template <typename T, int Version>
API::UObject* create_transform_offset_object_impl(API::UObject* a_parent, API::UObject* a_pawn, const TransformOffset &a_transformOffset)
{
	return a_parent;
}

//// Templated helper function that does the spawning
//template <typename T, int Version>
//API::UObject* create_transform_offset_object_impl(API::UObject* a_parent, API::UObject* a_pawn, const TransformOffset &a_transformOffset)
//{
//	auto ugameplay_statics_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.GameplayStatics");
//	API::UObject* statics_default_object = ugameplay_statics_class->get_class_default_object();
//
//	if (statics_default_object == nullptr)
//	{
//		API::get()->log_info("create_transform_offset_object failed to get default object for Engine.GameplayStatics");
//		return a_parent;
//	}
//		
//	// Set up a transform of type T
//	TTransform<T> trans;
//	trans.rotation = TVector4<T>(0, 0, 0, 1);
//	trans.translation = TVector4<T>(0, 0, 0, 0);
//	trans.scale3d = TVector4<T>(1, 1, 1, 1);
//
//	API::UClass* actor_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.StaticMeshActor");
//
//	// Fill begin parameters
//	BeginDeferredActorSpawnFromClassParams<T, Version> begin_params;
//	begin_params.pawn = a_pawn;
//	begin_params.actor_class = actor_class;
//	begin_params.spawn_transform = trans;
//	begin_params.collision_handling_override = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
//	begin_params.owner = a_parent;
//	if constexpr (Version == 1)
//	{
//		begin_params.tranform_scale_method = ESpawnActorScaleMethod::OverrideRootScale;
//	}
//
//	statics_default_object->call_function(L"BeginDeferredActorSpawnFromClass", &begin_params);
//
//	if (begin_params.return_value != nullptr)
//	{
//		API::get()->log_info("create_transform_offset_object spawned actor: %s", wstring_to_string(begin_params.return_value->get_full_name()).c_str());
//	}
//
//	API::UObject* spawned_actor = begin_params.return_value;
//
//	if (spawned_actor == nullptr)
//	{
//		return a_parent;
//	}
//
//	// Finish spawning if the actor was successfully spawned
//	FinishSpawningActorParams<T, Version> finish_params;
//	finish_params.actor = spawned_actor;
//	finish_params.spawn_transform = trans;
//	if constexpr (Version == 1)
//	{
//		finish_params.transform_scale_method = ESpawnActorScaleMethod::OverrideRootScale;
//	}
//
//	statics_default_object->call_function(L"FinishSpawningActor", &finish_params);
//
//	spawned_actor = finish_params.return_value;
//
//	if (spawned_actor == nullptr)
//	{
//		return a_parent;
//	}
//
//
//	API::get()->log_info("create_transform_offset_object finished spawning actor: %s", wstring_to_string(finish_params.return_value->get_full_name()).c_str());
//
//	//fixme: 
//	//get parent root component with GetRootComponent
//	ActorGetRootComponentFunctionParams<T> parent_actor_get_root_component_params;
//	call_function_on_uobject<ActorGetRootComponentFunctionParams<T>>(a_parent, &parent_actor_get_root_component_params);
//
//	API::UObject* parent_root_component = parent_actor_get_root_component_params.return_value;
//
//	if (parent_root_component == nullptr)
//	{
//		API::get()->log_info("create_transform_offset_object failed to get root component of parent actor");
//		return a_parent;
//	}
//
//	API::get()->log_info("create_transform_offset_object got root component of parent actor");
//
//
//
//	////create default subobject for spawned actor (DefaultSceneRoot)
//	//ObjectCreateDefaultSubobjectParams<T> object_create_default_subobject_params;
//	//object_create_default_subobject_params.default_subobject_name = API::FName(std::wstring(L"DefaultSceneRoot"));
//	//API::UClass* scene_component_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.SceneComponent");
//	//object_create_default_subobject_params.return_type = object_create_default_subobject_params.class_to_create_by_default = scene_component_class;
//	//object_create_default_subobject_params.required = true;
//	//object_create_default_subobject_params.transient = false;
//
//	//call_function_on_uobject<ObjectCreateDefaultSubobjectParams<T>>(spawned_actor, &object_create_default_subobject_params);
//
//	//API::UObject* spawned_actor_default_object = object_create_default_subobject_params.return_value;
//
//	//if (spawned_actor_default_object == nullptr)
//	//{
//	//	API::get()->log_info("create_transform_offset_object failed to create DefaultSceneRoot for spawned actor");
//	//	return a_parent;
//	//}
//	//API::get()->log_info("create_transform_offset_object created DefaultSceneRoot for spawned actor");
//
//
//	//get spawned actor root component with GetRootComponent
//	ActorGetRootComponentFunctionParams<T> spawned_actor_get_root_component_params;
//	call_function_on_uobject<ActorGetRootComponentFunctionParams<T>>(spawned_actor, &spawned_actor_get_root_component_params);
//
//	API::UObject* spawned_root_component = spawned_actor_get_root_component_params.return_value;
//
//	if (spawned_root_component == nullptr)
//	{
//		API::get()->log_info("create_transform_offset_object failed to get root component of spawned actor");
//		return a_parent;
//	}
//
//	API::get()->log_info("create_transform_offset_object got root component of spawned actor");
//
//		
//	////call AttachToComponent on the spawned actor's root component to attach it to the parent root component
//	//ComponentAttachToComponentFunctionParams<T> component_attach_to_component_params;
//	//component_attach_to_component_params.parent = parent_root_component;
//	//component_attach_to_component_params.attachmentRules.LocationRule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.attachmentRules.RotationRule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.attachmentRules.ScaleRule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.attachmentRules.bWeldSimulatedBodies = false;
//	//component_attach_to_component_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//
//	//call_function_on_uobject<ComponentAttachToComponentFunctionParams<T>>(spawned_root_component, &component_attach_to_component_params);
//
//	//if (!component_attach_to_component_params.return_value)
//	//{
//	//	API::get()->log_info("create_transform_offset_object AttachToComponent failed");
//	//	return a_parent;
//	//}
//
//
//
//
//	////call AttachToComponent on the spawned actor's root component to attach it to the parent root component
//	//K2ComponentAttachToComponentFunctionParams<T> component_attach_to_component_params;
//	//component_attach_to_component_params.parent = parent_root_component;
//	//component_attach_to_component_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//	//component_attach_to_component_params.location_rule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.rotation_rule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.scale_rule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.weld_simulated_bodies = false;
//
//	//call_function_on_uobject<K2ComponentAttachToComponentFunctionParams<T>>(spawned_root_component, &component_attach_to_component_params);
//
//	//call_function_on_uobject<K2ComponentAttachToComponentFunctionParams<T>>(spawned_actor, &component_attach_to_component_params);
//
//
//
//
//
//
//
//
//
//
//	struct
//	{
//
//	}component_register_component_params;
//
//	spawned_root_component->call_function(L"RegisterComponent", &component_register_component_params);
//
//
//	//fixme: call SetMobility here instead
////must set spawned component mobility to movable otherwise attachment fails.
//	//unsigned char* mobilityData = (unsigned char*)spawned_root_component->get_property_data(L"Mobility");
//	//mobilityData[0] = 2; //Movable
//
//	struct
//	{
//		EComponentMobility mobility;
//
//	}component_set_mobility_params;
//
//	component_set_mobility_params.mobility = EComponentMobility::Movable;
//
//	spawned_root_component->call_function(L"SetMobility", &component_set_mobility_params);
//
//
//
//
//	struct
//	{
//		ECollisionEnabled new_type;
//
//	}component_set_collision_enabled_params;
//
//	component_set_collision_enabled_params.new_type = ECollisionEnabled::NoCollision;
//
//	spawned_root_component->call_function(L"SetCollisionEnabled", &component_set_collision_enabled_params);
//
//
//
//	struct
//	{
//		bool new_absolute_location;
//		bool new_absolute_rotation;
//		bool new_absolute_scale;
//
//	}component_set_absolute_params;
//
//	component_set_absolute_params.new_absolute_location = false;
//	component_set_absolute_params.new_absolute_rotation = false;
//	component_set_absolute_params.new_absolute_scale = false;
//
//	spawned_root_component->call_function(L"SetAbsolute", &component_set_absolute_params);
//
//
//	//FIXME: call SetRelativeLocationAndRotation on the new actor root component using the location/rotator version; support two versions, one with ETeleportType as last parameter.
//	ComponentSetRelativeLocationAndRotationFunctionParams<T> component_set_relative_location_and_rotation_params;
//	component_set_relative_location_and_rotation_params.new_location = TVector<T>((T)a_transformOffset.m_locationX, (T)a_transformOffset.m_locationY, (T)a_transformOffset.m_locationZ);
//	component_set_relative_location_and_rotation_params.new_rotation = TRotator<T>((T)a_transformOffset.m_rotationPitch, (T)a_transformOffset.m_rotationYaw, (T)a_transformOffset.m_rotationRoll);
//	component_set_relative_location_and_rotation_params.sweep = false;
//	component_set_relative_location_and_rotation_params.outSweepHitResult = nullptr;
////	component_set_relative_location_and_rotation_params.teleport = ETeleportType::ResetPhysics;
//	component_set_relative_location_and_rotation_params.teleport = ETeleportType::TeleportPhysics;
//
//	call_function_on_uobject<ComponentSetRelativeLocationAndRotationFunctionParams<T>>(spawned_root_component, &component_set_relative_location_and_rotation_params);
//
//
//
//	struct
//	{
//		TVector<T> return_value{};
//	}component_get_relative_location_params;
//
//	spawned_root_component->call_function(L"GetRelativeLocation", &component_get_relative_location_params);
//
//	//call AttachToComponent on the spawned actor's root component to attach it to the parent root component
//	struct
//	{
//		API::UObject* parent;
//		API::FName socket_name;
//		EAttachmentRule location_rule;
//		EAttachmentRule rotation_rule;
//		EAttachmentRule scale_rule;
//		bool weld_simulated_bodies;
//		bool return_value{};
//
//	}component_attach_to_component_params;
//
//	component_attach_to_component_params.parent = parent_root_component;
//	component_attach_to_component_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//	component_attach_to_component_params.location_rule = EAttachmentRule::KeepRelative;
//	component_attach_to_component_params.rotation_rule = EAttachmentRule::KeepRelative;
//	component_attach_to_component_params.scale_rule = EAttachmentRule::KeepRelative;
//	component_attach_to_component_params.weld_simulated_bodies = false;
//
//
//
//	spawned_root_component->call_function(L"K2_AttachToComponent", &component_attach_to_component_params);
//	//spawned_actor->call_function(L"K2_AttachToComponent", &component_attach_to_component_params);
//
//
//
//	call_function_on_uobject<ComponentSetRelativeLocationAndRotationFunctionParams<T>>(spawned_root_component, &component_set_relative_location_and_rotation_params);
//
//
//
//
//	spawned_root_component->call_function(L"GetRelativeLocation", &component_get_relative_location_params);
//
//
//
//	////call AttachToActor on the spawned actor to attach it to the parent
//	//K2AttachToActorFunctionParams<T> actor_attach_to_actor_params;
//	//actor_attach_to_actor_params.parent = a_parent;
//	//actor_attach_to_actor_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//	//actor_attach_to_actor_params.location_rule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.rotation_rule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.scale_rule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.weld_simulated_bodies = false;
//
//	//call_function_on_uobject<K2AttachToActorFunctionParams<T>>(spawned_actor, &actor_attach_to_actor_params);
//
//
//	////call AttachToActor on the spawned actor to attach it to the parent
//	//AttachToActorFunctionParams<T> actor_attach_to_actor_params;
//	//actor_attach_to_actor_params.parent = a_parent;
//	//actor_attach_to_actor_params.attachmentRules.LocationRule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.attachmentRules.RotationRule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.attachmentRules.ScaleRule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.attachmentRules.bWeldSimulatedBodies = false;
//	//actor_attach_to_actor_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//
//	//call_function_on_uobject<AttachToActorFunctionParams<T>>(spawned_actor, &actor_attach_to_actor_params);
//
//
//	//if (!actor_attach_to_actor_params.return_value)
//	//{
//	//	API::get()->log_info("create_transform_offset_object AttachToActor failed");
//	//	return a_parent;
//	//}
//
//
//	if (!component_attach_to_component_params.return_value)
//	{
//		API::get()->log_info("create_transform_offset_object AttachToComponent failed");
//		return a_parent;
//	}
//
//
//	API::get()->log_info("create_transform_offset_object AttachToComponent succeeded");
//
//	////FIXME: call SetRelativeLocationAndRotation on the new actor root component using the location/rotator version; support two versions, one with ETeleportType as last parameter.
//	//ComponentSetRelativeLocationAndRotationFunctionParams<T> component_set_relative_location_and_rotation_params;
//	//component_set_relative_location_and_rotation_params.new_location = TVector<T>((T)a_transformOffset.m_locationX, (T)a_transformOffset.m_locationY, (T)a_transformOffset.m_locationZ);
//	//component_set_relative_location_and_rotation_params.new_rotation = TRotator<T>((T)a_transformOffset.m_rotationPitch, (T)a_transformOffset.m_rotationYaw, (T)a_transformOffset.m_rotationRoll);
//	//component_set_relative_location_and_rotation_params.sweep = false;
//	//component_set_relative_location_and_rotation_params.outSweepHitResult = nullptr;
//	//component_set_relative_location_and_rotation_params.teleport = ETeleportType::ResetPhysics;
//
//	//call_function_on_uobject<ComponentSetRelativeLocationAndRotationFunctionParams<T>>(spawned_root_component, &component_set_relative_location_and_rotation_params);
//
//
//
//	struct
//	{
//		TVector<T> new_location;
//		bool sweep;
//		FHitResult* sweep_hit_result;
//		//ETeleportType teleport;
//	}component_set_relative_location_params;
//
//
//	component_set_relative_location_params.new_location = TVector<T>((T)a_transformOffset.m_locationX, (T)a_transformOffset.m_locationY, (T)a_transformOffset.m_locationZ);
//	component_set_relative_location_params.sweep = false;
//	component_set_relative_location_params.sweep_hit_result = nullptr;
//	//component_set_relative_location_params.teleport = ETeleportType::ResetPhysics;
//
//	spawned_root_component->call_function(L"SetRelativeLocation", &component_set_relative_location_params);
////	spawned_actor->call_function(L"SetActorRelativeLocation", &component_set_relative_location_params);
//
//
//	spawned_root_component->call_function(L"GetRelativeLocation", &component_get_relative_location_params);
//
//	struct
//	{
//		TRotator<T> return_value{};
//	}component_get_relative_rotation_params;
//
//
//	spawned_root_component->call_function(L"GetRelativeRotation", &component_get_relative_rotation_params);
//
//
//
//	return spawned_actor;
//}




//
//
//// Templated helper function that does the spawning
//template <typename T, int Version>
//API::UObject* create_transform_offset_object_impl(API::UObject* a_parent, API::UObject* a_pawn, const TransformOffset& a_transformOffset)
//{
//	//auto ugameplay_statics_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.GameplayStatics");
//	//API::UObject* statics_default_object = ugameplay_statics_class->get_class_default_object();
//
//	//if (statics_default_object == nullptr)
//	//{
//	//	API::get()->log_info("create_transform_offset_object failed to get default object for Engine.GameplayStatics");
//	//	return a_parent;
//	//}
//
//	//// Set up a transform of type T
//	//TTransform<T> trans;
//	//trans.rotation = TVector4<T>(0, 0, 0, 1);
//	//trans.translation = TVector4<T>(0, 0, 0, 0);
//	//trans.scale3d = TVector4<T>(1, 1, 1, 1);
//
//	//API::UClass* actor_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.StaticMeshActor");
//
//	//// Fill begin parameters
//	//BeginDeferredActorSpawnFromClassParams<T, Version> begin_params;
//	//begin_params.pawn = a_pawn;
//	//begin_params.actor_class = actor_class;
//	//begin_params.spawn_transform = trans;
//	//begin_params.collision_handling_override = ESpawnActorCollisionHandlingMethod::AlwaysSpawn;
//	//begin_params.owner = a_parent;
//	//if constexpr (Version == 1)
//	//{
//	//	begin_params.tranform_scale_method = ESpawnActorScaleMethod::OverrideRootScale;
//	//}
//
//	//statics_default_object->call_function(L"BeginDeferredActorSpawnFromClass", &begin_params);
//
//	//if (begin_params.return_value != nullptr)
//	//{
//	//	API::get()->log_info("create_transform_offset_object spawned actor: %s", wstring_to_string(begin_params.return_value->get_full_name()).c_str());
//	//}
//
//	//API::UObject* spawned_actor = begin_params.return_value;
//
//	//if (spawned_actor == nullptr)
//	//{
//	//	return a_parent;
//	//}
//
//	//// Finish spawning if the actor was successfully spawned
//	//FinishSpawningActorParams<T, Version> finish_params;
//	//finish_params.actor = spawned_actor;
//	//finish_params.spawn_transform = trans;
//	//if constexpr (Version == 1)
//	//{
//	//	finish_params.transform_scale_method = ESpawnActorScaleMethod::OverrideRootScale;
//	//}
//
//	//statics_default_object->call_function(L"FinishSpawningActor", &finish_params);
//
//	//spawned_actor = finish_params.return_value;
//
//	//if (spawned_actor == nullptr)
//	//{
//	//	return a_parent;
//	//}
//	//	API::get()->log_info("create_transform_offset_object finished spawning actor: %s", wstring_to_string(finish_params.return_value->get_full_name()).c_str());
//
//	API::UClass* actor_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.Actor");
//	API::UObject* spawned_actor = API::get()->spawn_object(actor_class, a_parent);
//
//
//	API::UClass* scene_component_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.SceneComponent");
//	API::UObject* spawned_root_component = API::get()->spawn_object(scene_component_class, spawned_actor);
//
//	struct
//	{
//		EComponentMobility mobility;
//
//	}component_set_mobility_params;
//
//	component_set_mobility_params.mobility = EComponentMobility::Movable;
//
//	spawned_root_component->call_function(L"SetMobility", &component_set_mobility_params);
//
//
//	struct
//	{
//		API::UObject* new_root_component;
//		bool return_result;
//	}component_set_root_component_params;
//
//	component_set_root_component_params.new_root_component = spawned_root_component;
//
//	spawned_actor->call_function(L"SetRootComponent", &component_set_root_component_params);
//	
//
//
//	struct
//	{
//
//	}component_register_component_params;
//
//	spawned_root_component->call_function(L"RegisterComponent", &component_register_component_params);
//
//
//
//
//
//	//FIXME: call SetRelativeLocationAndRotation on the new actor root component using the location/rotator version; support two versions, one with ETeleportType as last parameter.
//	ComponentSetRelativeLocationAndRotationFunctionParams<T> component_set_relative_location_and_rotation_params;
//	component_set_relative_location_and_rotation_params.new_location = TVector<T>((T)a_transformOffset.m_locationX, (T)a_transformOffset.m_locationY, (T)a_transformOffset.m_locationZ);
//	component_set_relative_location_and_rotation_params.new_rotation = TRotator<T>((T)a_transformOffset.m_rotationPitch, (T)a_transformOffset.m_rotationYaw, (T)a_transformOffset.m_rotationRoll);
//	component_set_relative_location_and_rotation_params.sweep = false;
//	component_set_relative_location_and_rotation_params.outSweepHitResult = nullptr;
//	//	component_set_relative_location_and_rotation_params.teleport = ETeleportType::ResetPhysics;
//	component_set_relative_location_and_rotation_params.teleport = ETeleportType::TeleportPhysics;
//
//	call_function_on_uobject<ComponentSetRelativeLocationAndRotationFunctionParams<T>>(spawned_root_component, &component_set_relative_location_and_rotation_params);
//
//
//
//	struct
//	{
//		TVector<T> return_value{};
//	}component_get_relative_location_params;
//
//	spawned_root_component->call_function(L"GetRelativeLocation", &component_get_relative_location_params);
//
//
//	//fixme: 
//	//get parent root component with GetRootComponent
//	ActorGetRootComponentFunctionParams<T> parent_actor_get_root_component_params;
//	call_function_on_uobject<ActorGetRootComponentFunctionParams<T>>(a_parent, &parent_actor_get_root_component_params);
//
//	API::UObject* parent_root_component = parent_actor_get_root_component_params.return_value;
//
//	if (parent_root_component == nullptr)
//	{
//		API::get()->log_info("create_transform_offset_object failed to get root component of parent actor");
//		return a_parent;
//	}
//
//	API::get()->log_info("create_transform_offset_object got root component of parent actor");
//
//
//
//	////create default subobject for spawned actor (DefaultSceneRoot)
//	//ObjectCreateDefaultSubobjectParams<T> object_create_default_subobject_params;
//	//object_create_default_subobject_params.default_subobject_name = API::FName(std::wstring(L"DefaultSceneRoot"));
//	//API::UClass* scene_component_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.SceneComponent");
//	//object_create_default_subobject_params.return_type = object_create_default_subobject_params.class_to_create_by_default = scene_component_class;
//	//object_create_default_subobject_params.required = true;
//	//object_create_default_subobject_params.transient = false;
//
//	//call_function_on_uobject<ObjectCreateDefaultSubobjectParams<T>>(spawned_actor, &object_create_default_subobject_params);
//
//	//API::UObject* spawned_actor_default_object = object_create_default_subobject_params.return_value;
//
//	//if (spawned_actor_default_object == nullptr)
//	//{
//	//	API::get()->log_info("create_transform_offset_object failed to create DefaultSceneRoot for spawned actor");
//	//	return a_parent;
//	//}
//	//API::get()->log_info("create_transform_offset_object created DefaultSceneRoot for spawned actor");
//
//
//	//get spawned actor root component with GetRootComponent
//	ActorGetRootComponentFunctionParams<T> spawned_actor_get_root_component_params;
//	call_function_on_uobject<ActorGetRootComponentFunctionParams<T>>(spawned_actor, &spawned_actor_get_root_component_params);
//
////	API::UObject* spawned_root_component = spawned_actor_get_root_component_params.return_value;
//
//	//if (spawned_root_component == nullptr)
//	//{
//	//	API::get()->log_info("create_transform_offset_object failed to get root component of spawned actor");
//	//	return a_parent;
//	//}
//
//	//API::get()->log_info("create_transform_offset_object got root component of spawned actor");
//
//
//	////call AttachToComponent on the spawned actor's root component to attach it to the parent root component
//	//ComponentAttachToComponentFunctionParams<T> component_attach_to_component_params;
//	//component_attach_to_component_params.parent = parent_root_component;
//	//component_attach_to_component_params.attachmentRules.LocationRule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.attachmentRules.RotationRule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.attachmentRules.ScaleRule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.attachmentRules.bWeldSimulatedBodies = false;
//	//component_attach_to_component_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//
//	//call_function_on_uobject<ComponentAttachToComponentFunctionParams<T>>(spawned_root_component, &component_attach_to_component_params);
//
//	//if (!component_attach_to_component_params.return_value)
//	//{
//	//	API::get()->log_info("create_transform_offset_object AttachToComponent failed");
//	//	return a_parent;
//	//}
//
//
//
//
//	////call AttachToComponent on the spawned actor's root component to attach it to the parent root component
//	//K2ComponentAttachToComponentFunctionParams<T> component_attach_to_component_params;
//	//component_attach_to_component_params.parent = parent_root_component;
//	//component_attach_to_component_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//	//component_attach_to_component_params.location_rule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.rotation_rule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.scale_rule = EAttachmentRule::KeepRelative;
//	//component_attach_to_component_params.weld_simulated_bodies = false;
//
//	//call_function_on_uobject<K2ComponentAttachToComponentFunctionParams<T>>(spawned_root_component, &component_attach_to_component_params);
//
//	//call_function_on_uobject<K2ComponentAttachToComponentFunctionParams<T>>(spawned_actor, &component_attach_to_component_params);
//
//
//
//
//
//
//
//
//
//
//	//struct
//	//{
//
//	//}component_register_component_params;
//
//	//spawned_root_component->call_function(L"RegisterComponent", &component_register_component_params);
//
//
//	//fixme: call SetMobility here instead
////must set spawned component mobility to movable otherwise attachment fails.
//	//unsigned char* mobilityData = (unsigned char*)spawned_root_component->get_property_data(L"Mobility");
//	//mobilityData[0] = 2; //Movable
//
//	//struct
//	//{
//	//	EComponentMobility mobility;
//
//	//}component_set_mobility_params;
//
//	//component_set_mobility_params.mobility = EComponentMobility::Movable;
//
//	//spawned_root_component->call_function(L"SetMobility", &component_set_mobility_params);
//
//
//
//
//	//struct
//	//{
//	//	ECollisionEnabled new_type;
//
//	//}component_set_collision_enabled_params;
//
//	//component_set_collision_enabled_params.new_type = ECollisionEnabled::NoCollision;
//
//	//spawned_root_component->call_function(L"SetCollisionEnabled", &component_set_collision_enabled_params);
//
//
//
//	//struct
//	//{
//	//	bool new_absolute_location;
//	//	bool new_absolute_rotation;
//	//	bool new_absolute_scale;
//
//	//}component_set_absolute_params;
//
//	//component_set_absolute_params.new_absolute_location = false;
//	//component_set_absolute_params.new_absolute_rotation = false;
//	//component_set_absolute_params.new_absolute_scale = false;
//
//	//spawned_root_component->call_function(L"SetAbsolute", &component_set_absolute_params);
//
//
//	////FIXME: call SetRelativeLocationAndRotation on the new actor root component using the location/rotator version; support two versions, one with ETeleportType as last parameter.
//	//ComponentSetRelativeLocationAndRotationFunctionParams<T> component_set_relative_location_and_rotation_params;
//	//component_set_relative_location_and_rotation_params.new_location = TVector<T>((T)a_transformOffset.m_locationX, (T)a_transformOffset.m_locationY, (T)a_transformOffset.m_locationZ);
//	//component_set_relative_location_and_rotation_params.new_rotation = TRotator<T>((T)a_transformOffset.m_rotationPitch, (T)a_transformOffset.m_rotationYaw, (T)a_transformOffset.m_rotationRoll);
//	//component_set_relative_location_and_rotation_params.sweep = false;
//	//component_set_relative_location_and_rotation_params.outSweepHitResult = nullptr;
//	////	component_set_relative_location_and_rotation_params.teleport = ETeleportType::ResetPhysics;
//	//component_set_relative_location_and_rotation_params.teleport = ETeleportType::TeleportPhysics;
//
//	//call_function_on_uobject<ComponentSetRelativeLocationAndRotationFunctionParams<T>>(spawned_root_component, &component_set_relative_location_and_rotation_params);
//
//
//
//	//struct
//	//{
//	//	TVector<T> return_value{};
//	//}component_get_relative_location_params;
//
//	//spawned_root_component->call_function(L"GetRelativeLocation", &component_get_relative_location_params);
//
//	//call AttachToComponent on the spawned actor's root component to attach it to the parent root component
//	struct
//	{
//		API::UObject* parent;
//		API::FName socket_name;
//		EAttachmentRule location_rule;
//		EAttachmentRule rotation_rule;
//		EAttachmentRule scale_rule;
//		bool weld_simulated_bodies;
//		bool return_value{};
//
//	}component_attach_to_component_params;
//
//	component_attach_to_component_params.parent = parent_root_component;
//	component_attach_to_component_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//	component_attach_to_component_params.location_rule = EAttachmentRule::KeepRelative;
//	component_attach_to_component_params.rotation_rule = EAttachmentRule::KeepRelative;
//	component_attach_to_component_params.scale_rule = EAttachmentRule::KeepRelative;
//	component_attach_to_component_params.weld_simulated_bodies = false;
//
//	spawned_root_component->call_function(L"K2_AttachToComponent", &component_attach_to_component_params);
//	//spawned_actor->call_function(L"K2_AttachToComponent", &component_attach_to_component_params);
//
//
//
//	call_function_on_uobject<ComponentSetRelativeLocationAndRotationFunctionParams<T>>(spawned_root_component, &component_set_relative_location_and_rotation_params);
//
//
//
//
//	spawned_root_component->call_function(L"GetRelativeLocation", &component_get_relative_location_params);
//
//
//
//	////call AttachToActor on the spawned actor to attach it to the parent
//	//K2AttachToActorFunctionParams<T> actor_attach_to_actor_params;
//	//actor_attach_to_actor_params.parent = a_parent;
//	//actor_attach_to_actor_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//	//actor_attach_to_actor_params.location_rule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.rotation_rule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.scale_rule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.weld_simulated_bodies = false;
//
//	//call_function_on_uobject<K2AttachToActorFunctionParams<T>>(spawned_actor, &actor_attach_to_actor_params);
//
//
//	////call AttachToActor on the spawned actor to attach it to the parent
//	//AttachToActorFunctionParams<T> actor_attach_to_actor_params;
//	//actor_attach_to_actor_params.parent = a_parent;
//	//actor_attach_to_actor_params.attachmentRules.LocationRule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.attachmentRules.RotationRule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.attachmentRules.ScaleRule = EAttachmentRule::KeepRelative;
//	//actor_attach_to_actor_params.attachmentRules.bWeldSimulatedBodies = false;
//	//actor_attach_to_actor_params.socket_name = API::FName(L"None", API::FName::EFindName::Find);
//
//	//call_function_on_uobject<AttachToActorFunctionParams<T>>(spawned_actor, &actor_attach_to_actor_params);
//
//
//	//if (!actor_attach_to_actor_params.return_value)
//	//{
//	//	API::get()->log_info("create_transform_offset_object AttachToActor failed");
//	//	return a_parent;
//	//}
//
//
//	//if (!component_attach_to_component_params.return_value)
//	//{
//	//	API::get()->log_info("create_transform_offset_object AttachToComponent failed");
//	//	return a_parent;
//	//}
//
//
//	API::get()->log_info("create_transform_offset_object AttachToComponent succeeded");
//
//	////FIXME: call SetRelativeLocationAndRotation on the new actor root component using the location/rotator version; support two versions, one with ETeleportType as last parameter.
//	//ComponentSetRelativeLocationAndRotationFunctionParams<T> component_set_relative_location_and_rotation_params;
//	//component_set_relative_location_and_rotation_params.new_location = TVector<T>((T)a_transformOffset.m_locationX, (T)a_transformOffset.m_locationY, (T)a_transformOffset.m_locationZ);
//	//component_set_relative_location_and_rotation_params.new_rotation = TRotator<T>((T)a_transformOffset.m_rotationPitch, (T)a_transformOffset.m_rotationYaw, (T)a_transformOffset.m_rotationRoll);
//	//component_set_relative_location_and_rotation_params.sweep = false;
//	//component_set_relative_location_and_rotation_params.outSweepHitResult = nullptr;
//	//component_set_relative_location_and_rotation_params.teleport = ETeleportType::ResetPhysics;
//
//	//call_function_on_uobject<ComponentSetRelativeLocationAndRotationFunctionParams<T>>(spawned_root_component, &component_set_relative_location_and_rotation_params);
//
//
//
//	struct
//	{
//		TVector<T> new_location;
//		bool sweep;
//		FHitResult* sweep_hit_result;
//		//ETeleportType teleport;
//	}component_set_relative_location_params;
//
//
//	component_set_relative_location_params.new_location = TVector<T>((T)a_transformOffset.m_locationX, (T)a_transformOffset.m_locationY, (T)a_transformOffset.m_locationZ);
//	component_set_relative_location_params.sweep = false;
//	component_set_relative_location_params.sweep_hit_result = nullptr;
//	//component_set_relative_location_params.teleport = ETeleportType::ResetPhysics;
//
//	spawned_root_component->call_function(L"SetRelativeLocation", &component_set_relative_location_params);
//	//	spawned_actor->call_function(L"SetActorRelativeLocation", &component_set_relative_location_params);
//
//
//	spawned_root_component->call_function(L"GetRelativeLocation", &component_get_relative_location_params);
//
//	struct
//	{
//		TRotator<T> return_value{};
//	}component_get_relative_rotation_params;
//
//
//	spawned_root_component->call_function(L"GetRelativeRotation", &component_get_relative_rotation_params);
//
//
//
//	return spawned_actor;
//}
//
//
//






// Main dispatch function – selects the proper template instantiation based on a_use_double and a_spawn_actor_version
API::UObject* GPSimple::create_transform_offset_object(API::UObject* a_parent, API::UObject* a_pawn, bool a_use_double, int a_spawn_actor_version , const TransformOffset& a_transformOffset)
{
	if (a_use_double)
	{
		switch (a_spawn_actor_version)
		{
		case 0:
			return create_transform_offset_object_impl<double, 0>(a_parent, a_pawn, a_transformOffset);
		case 1:
			return create_transform_offset_object_impl<double, 1>(a_parent, a_pawn, a_transformOffset);
		default:
			break;
		}
	}
	else
	{
		switch (a_spawn_actor_version)
		{
		case 0:
			return create_transform_offset_object_impl<float, 0>(a_parent, a_pawn, a_transformOffset);
		case 1:
			return create_transform_offset_object_impl<float, 1>(a_parent, a_pawn, a_transformOffset);
		default:
			break;
		}
	}
	return a_parent;
}


template<typename T>
void get_actor_transform_vectors_impl(uevr::API::UObject* uobject, TVector<T>* location, TVector<T>* forward, TVector<T>* up, TVector<T>* right, const TransformOffset& transform_offset)
{

	struct
	{
		TVector<T> return_value;
	} get_location_params;
	uobject->call_function(L"K2_GetActorLocation", &get_location_params);

	struct
	{
		TRotator<T> return_value;
	} get_rotation_params;
	uobject->call_function(L"K2_GetActorRotation", &get_rotation_params);

	struct
	{
		TVector<T> return_value;
	} get_scale_params;
	uobject->call_function(L"GetActorScale3D", &get_scale_params);


	TVector4<T> actor_location = TVector4<T>((T)get_location_params.return_value.X, (T)get_location_params.return_value.Y, (T)get_location_params.return_value.Z, (T)1.0);
	TVector4<T> actor_rotation_quat = ConvertRotatorToQuaternion<T>(get_rotation_params.return_value);
	TVector4<T> actor_scale = TVector4<T>((T)get_scale_params.return_value.X, (T)get_scale_params.return_value.Y, (T)get_scale_params.return_value.Z, (T)1.0);

	TVector4<T> offset_location = TVector4<T>((T)transform_offset.m_locationX, (T)transform_offset.m_locationY, (T)transform_offset.m_locationZ, (T)1.0);
	TVector4<T> offset_rotation_quat = ConvertRotatorToQuaternion<T>(TRotator<T>((T)transform_offset.m_rotationPitch, (T)transform_offset.m_rotationYaw, (T)transform_offset.m_rotationRoll));


	TVector4<T> scaled_location = VectorMultiply<T>(actor_scale, offset_location);
	TVector<T> rotated_location = VectorQuaternionRotateVector<T>(actor_rotation_quat, TVector<T>(scaled_location.X, scaled_location.Y, scaled_location.Z));
	TVector4<T> final_location = VectorAdd<T>(TVector4<T>((T)rotated_location.X, (T)rotated_location.Y, (T)rotated_location.Z, (T)1.0), actor_location);

	TVector4<T> final_rotation = VectorQuaternionMultiply(actor_rotation_quat, offset_rotation_quat);

	*location = TVector<T>((T)final_location.X, (T)final_location.Y, (T)final_location.Z);
	*forward = VectorQuaternionRotateVector(final_rotation, TVector<T>(1.0, 0.0, 0.0));
	*up = VectorQuaternionRotateVector(final_rotation, TVector<T>(0.0, 0.0, 1.0));
	*right = VectorQuaternionRotateVector(final_rotation, TVector<T>(0.0, 1.0, 0.0));
}


void GPSimple::get_actor_transform_vectors(uevr::API::UObject* uobject, FVectorDouble* location, FVectorDouble* forward, FVectorDouble* up, FVectorDouble* right, bool doubles, const TransformOffset& transform_offset)
{
	if (doubles)
	{
		get_actor_transform_vectors_impl<double>(uobject, location, forward, up, right, transform_offset);
	}
	else
	{
		FVectorFloat locationf;
		FVectorFloat forwardf;
		FVectorFloat upf;
		FVectorFloat rightf;

		get_actor_transform_vectors_impl<float>(uobject, &locationf, &forwardf, &upf, &rightf, transform_offset);

		*location = FVectorDouble(locationf);
		*forward = FVectorDouble(forwardf);
		*up = FVectorDouble(upf);
		*right = FVectorDouble(rightf);
	}
}


