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
