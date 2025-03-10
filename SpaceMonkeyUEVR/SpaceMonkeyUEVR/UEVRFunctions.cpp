#include "UEVRFunctions.h"
#include "UObjectStructs.h"

void get_actor_transform(uevr::API::UObject* uobject, FVectorDouble* location, FRotatorDouble* rotation, bool doubles)
{
	if (doubles)
	{
		GetLocationFunctionParams<double> get_location_params_double;
		call_function_on_uobject<GetLocationFunctionParams<double>>(uobject, &get_location_params_double);

		GetRotationFunctionParams<double> get_rotation_params_double;
		call_function_on_uobject<GetRotationFunctionParams<double>>(uobject, &get_rotation_params_double);

		location->X = get_location_params_double.return_value.X;
		location->Y = get_location_params_double.return_value.Y;
		location->Z = get_location_params_double.return_value.Z;

		rotation->Pitch = get_rotation_params_double.return_value.Pitch;
		rotation->Yaw = get_rotation_params_double.return_value.Yaw;
		rotation->Roll = get_rotation_params_double.return_value.Roll;
	}
	else
	{
		GetLocationFunctionParams<float> get_location_params_float;
		call_function_on_uobject<GetLocationFunctionParams<float>>(uobject, &get_location_params_float);

		GetRotationFunctionParams<float> get_rotation_params_float;
		call_function_on_uobject<GetRotationFunctionParams<float>>(uobject, &get_rotation_params_float);


		location->X = (double)get_location_params_float.return_value.X;
		location->Y = (double)get_location_params_float.return_value.Y;
		location->Z = (double)get_location_params_float.return_value.Z;

		rotation->Pitch = (double)get_rotation_params_float.return_value.Pitch;
		rotation->Yaw = (double)get_rotation_params_float.return_value.Yaw;
		rotation->Roll = (double)get_rotation_params_float.return_value.Roll;
	}

}

void get_actor_transform_vectors(uevr::API::UObject* uobject, FVectorDouble* location, FVectorDouble* forward, FVectorDouble* up, FVectorDouble* right, bool doubles)
{
	if (doubles)
	{
		GetLocationFunctionParams<double> get_location_params_double;
		call_function_on_uobject<GetLocationFunctionParams<double>>(uobject, &get_location_params_double);

		GetActorForwardFunctionParams<double> get_forward_params_double;
		call_function_on_uobject<GetActorForwardFunctionParams<double>>(uobject, &get_forward_params_double);

		GetActorUpFunctionParams<double> get_up_params_double;
		call_function_on_uobject<GetActorUpFunctionParams<double>>(uobject, &get_up_params_double);

		GetActorRightFunctionParams<double> get_right_params_double;
		call_function_on_uobject<GetActorRightFunctionParams<double>>(uobject, &get_right_params_double);


		location->X = get_location_params_double.return_value.X;
		location->Y = get_location_params_double.return_value.Y;
		location->Z = get_location_params_double.return_value.Z;

		forward->X = get_forward_params_double.return_value.X;
		forward->Y = get_forward_params_double.return_value.Y;
		forward->Z = get_forward_params_double.return_value.Z;

		up->X = get_up_params_double.return_value.X;
		up->Y = get_up_params_double.return_value.Y;
		up->Z = get_up_params_double.return_value.Z;

		right->X = get_right_params_double.return_value.X;
		right->Y = get_right_params_double.return_value.Y;
		right->Z = get_right_params_double.return_value.Z;



	}
	else
	{
		GetLocationFunctionParams<float> get_location_params_float;
		call_function_on_uobject<GetLocationFunctionParams<float>>(uobject, &get_location_params_float);

		GetActorForwardFunctionParams<float> get_forward_params_float;
		call_function_on_uobject<GetActorForwardFunctionParams<float>>(uobject, &get_forward_params_float);

		GetActorUpFunctionParams<float> get_up_params_float;
		call_function_on_uobject<GetActorUpFunctionParams<float>>(uobject, &get_up_params_float);

		GetActorRightFunctionParams<float> get_right_params_float;
		call_function_on_uobject<GetActorRightFunctionParams<float>>(uobject, &get_right_params_float);

		location->X = get_location_params_float.return_value.X;
		location->Y = get_location_params_float.return_value.Y;
		location->Z = get_location_params_float.return_value.Z;

		forward->X = get_forward_params_float.return_value.X;
		forward->Y = get_forward_params_float.return_value.Y;
		forward->Z = get_forward_params_float.return_value.Z;

		up->X = get_up_params_float.return_value.X;
		up->Y = get_up_params_float.return_value.Y;
		up->Z = get_up_params_float.return_value.Z;

		right->X = get_right_params_float.return_value.X;
		right->Y = get_right_params_float.return_value.Y;
		right->Z = get_right_params_float.return_value.Z;
	}

}



//bool get_uobject_is_valid(uevr::API::UObject* uobject)
//{
//	if (uobject == nullptr)
//		return false;
//
//	struct {
//		bool return_value{};
//	} get_uobject_is_valid_lowlevel_function_params;
//
//
//	uobject->call_function(L"IsValidLowLevel", &get_uobject_is_valid_lowlevel_function_params);
//
//	struct {
//		bool return_value{};
//	} get_uobject_is_pending_kill_function_params;
//
//
//	uobject->call_function(L"IsPendingKill", &get_uobject_is_pending_kill_function_params);
//
//
//	return get_uobject_is_valid_lowlevel_function_params.return_value && !get_uobject_is_pending_kill_function_params.return_value;
//}