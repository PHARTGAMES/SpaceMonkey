#include "GPSimple.h"
#include "UEVRUtils.h"
#include "UObjectStructs.h"
#include "UEVRFunctions.h"
#include "SpaceMonkeyTelemetryAPI.h"
#include "systemtime.h"


GPSimple::GPSimple(UEVRGameConfig* a_game_config) : UEVRGamePlugin(a_game_config)
{
	m_game_config_gp_simple = static_cast<GPSimpleConfig*>(a_game_config);

	reset_system_time();
	m_resolved_object = nullptr;
	m_selected_pawn_name = L"";

	m_scene_component_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.SceneComponent");
	m_actor_class = API::get()->find_uobject<API::UClass>(L"Class /Script/Engine.Actor");

	API::get()->log_info("Created GPSimple Instance");

}


void GPSimple::on_pre_engine_tick(API::UGameEngine* engine, float delta)
{

}


bool GPSimple::is_correct_pawn(API::UObject* object)
{
	if (object == nullptr)
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

	//support empty substring; always use the pawn that passes the above checks
	if (m_game_config_gp_simple->m_pawn_display_name_substring.empty())
		return true;

	//find m_pawn_display_name_substring within pawn name
	if (object->get_full_name().find(string_to_wstring(m_game_config_gp_simple->m_pawn_display_name_substring)) == std::wstring::npos)
		return false;


	return true;
}



void GPSimple::on_post_engine_tick(API::UGameEngine* engine, float delta)
{
	if (m_game_config_gp_simple->m_tick_on_present)
	{
		return;
	}

	tick(delta);

}

void GPSimple::on_present()
{
	if (!m_game_config_gp_simple->m_tick_on_present)
	{
		return;
	}

	double system_time_now = SystemTime::GetInSeconds();
	double delta = system_time_now - m_last_present_time;
	m_last_present_time = system_time_now;

	tick(delta);
}

void GPSimple::reset_system_time()
{
	m_system_time = 0.0f;
	m_last_present_time = SystemTime::GetInSeconds();

}

void GPSimple::tick(float delta)
{
	try
	{

		auto pawn = API::get()->get_local_pawn(m_pawn_index);
		if (pawn == nullptr) //null pawn, past the end of the local pawns
		{
			m_pawn_index = 0;
			pawn = API::get()->get_local_pawn(m_pawn_index);
		}

		//valid pawn
		if (pawn != nullptr && is_correct_pawn(pawn))
		{

			std::wstring pawn_name = pawn->get_full_name();

			//pawn changed!
			if (m_selected_pawn_name.compare(pawn_name) != 0)
			{
				API::get()->log_info("tick: pawn changed from %s to %s", wstring_to_string(m_selected_pawn_name).c_str(), wstring_to_string(pawn_name).c_str());

				m_selected_pawn_name = pawn_name;
				m_resolved_object = nullptr;
				reset_system_time();
			}

			//resolve child object
			if (m_resolved_object == nullptr)
			{
				m_resolved_object = get_child_object_for_path(pawn, m_game_config_gp_simple->m_object_path);
			}

			//resolved something, extract telemetry
			if (m_resolved_object != nullptr)
			{
				FVectorDouble location;
				FVectorDouble forward;
				FVectorDouble up;
				FVectorDouble right;
				get_scenecomponent_transform_vectors(m_resolved_object, &location, &forward, &up, &right, m_game_config_gp_simple->m_use_doubles, m_game_config_gp_simple->m_transform_offset);

				memset(&m_frameData, 0, sizeof(SpaceMonkeyTelemetryFrameData));

				m_system_time += delta;
				m_frameData.m_time = m_system_time;

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
			reset_system_time();
			m_resolved_object = nullptr;
			m_selected_pawn_name = L"";
			m_pawn_index++; //try next local pawn
		}

	}
	catch (const std::exception& e)
	{
		API::get()->log_error("tick: exception: %s", e.what());
		m_pawn_index = 0;
		m_resolved_object = nullptr;
		reset_system_time();
	}
}



//this returns a scenecomponent
API::UObject* GPSimple::get_child_object_for_path(API::UObject* a_actor, std::vector<std::string>& a_object_path)
{
	API::UObject* actor_root_component = get_actor_root_component(a_actor);

	if (a_object_path.size() == 0)
	{
		API::get()->log_info("get_child_object_for_path called with empty a_object_path, resolving scene component: %s", wstring_to_string(actor_root_component->get_full_name()).c_str());
		
		return actor_root_component;
	}

	API::UObject* curr_object = a_actor;

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

		curr_object = actor_root_component;
	}

	//found something, make sure if it's an actor we return it's root component
	if (curr_object != nullptr)
	{
		if (curr_object->is_a(m_scene_component_class))
		{
			API::get()->log_info("get_child_object_for_path resolved scene component %s", wstring_to_string(curr_object->get_full_name()).c_str());
		} 
		else
		if (curr_object->is_a(m_actor_class)) //is an actor, return root component
		{
			API::UObject* curr_object_root_component = get_actor_root_component(curr_object);

			//successfully got curr_object's root component
			if (curr_object_root_component != nullptr)
			{
				curr_object = curr_object_root_component;
				API::get()->log_info("get_child_object_for_path resolved scene component %s", wstring_to_string(curr_object->get_full_name()).c_str());
			}
			else //failed to get root component, use source actor's root
			{
				curr_object = actor_root_component;
				API::get()->log_info("get_child_object_for_path failed to resolve scene component for actor %s", wstring_to_string(curr_object->get_full_name()).c_str());
			}
		}
	}

	return curr_object;
}


template<typename T>
void get_scenecomponent_transform_vectors_impl(uevr::API::UObject* uobject, TVector<T>* location, TVector<T>* forward, TVector<T>* up, TVector<T>* right, const TransformOffset& transform_offset)
{
//	API::get()->log_info("get_scenecomponent_transform_vectors_impl");

	struct
	{
		TVector<T> return_value;
	} get_location_params;
	uobject->call_function(L"K2_GetComponentLocation", &get_location_params);

//	API::get()->log_info("K2_GetComponentLocation %f,%f,%f", get_location_params.return_value.X, get_location_params.return_value.Y, get_location_params.return_value.Z);

	struct
	{
		TRotator<T> return_value;
	} get_rotation_params;
	uobject->call_function(L"K2_GetComponentRotation", &get_rotation_params);

//	API::get()->log_info("K2_GetComponentRotation %f,%f,%f", get_rotation_params.return_value.Pitch, get_rotation_params.return_value.Yaw, get_rotation_params.return_value.Roll);


	struct
	{
		TVector<T> return_value;
	} get_scale_params;
	uobject->call_function(L"K2_GetComponentScale", &get_scale_params);

//	API::get()->log_info("K2_GetComponentScale %f,%f,%f", get_scale_params.return_value.X, get_scale_params.return_value.Y, get_scale_params.return_value.Z);


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


void GPSimple::get_scenecomponent_transform_vectors(uevr::API::UObject* uobject, FVectorDouble* location, FVectorDouble* forward, FVectorDouble* up, FVectorDouble* right, bool doubles, const TransformOffset& transform_offset)
{
	if (doubles)
	{
		get_scenecomponent_transform_vectors_impl<double>(uobject, location, forward, up, right, transform_offset);
	}
	else
	{
		FVectorFloat locationf;
		FVectorFloat forwardf;
		FVectorFloat upf;
		FVectorFloat rightf;

		get_scenecomponent_transform_vectors_impl<float>(uobject, &locationf, &forwardf, &upf, &rightf, transform_offset);

		*location = FVectorDouble(locationf);
		*forward = FVectorDouble(forwardf);
		*up = FVectorDouble(upf);
		*right = FVectorDouble(rightf);
	}
}


