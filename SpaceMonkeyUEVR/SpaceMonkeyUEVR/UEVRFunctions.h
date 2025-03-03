#pragma once

#include "uevr/API.hpp"
#include "UObjectStructs.h"

// Templated function parameter structs for location and rotation
template<typename T>
struct GetLocationFunctionParams {
    TVector<T> return_value;
};

template<typename T>
struct GetRotationFunctionParams {
    TRotator<T> return_value;
};

// Traits class to associate each function parameter struct with its function name
template<typename FuncParams>
struct FunctionNameTraits;

template<typename T>
struct FunctionNameTraits<GetLocationFunctionParams<T>> {
    static constexpr const wchar_t* name = L"K2_GetActorLocation";
};

template<typename T>
struct FunctionNameTraits<GetRotationFunctionParams<T>> {
    static constexpr const wchar_t* name = L"K2_GetActorRotation";
};

// A templated helper function to wrap call_function
// (Assumes your uobject type has a member function call_function that accepts a wchar_t* and a void pointer)
template<typename FuncParams>
void call_function_on_uobject(uevr::API::UObject* object, FuncParams* params) 
{
    object->call_function(FunctionNameTraits<FuncParams>::name, params);
}

void get_actor_transform(uevr::API::UObject* uobject, FVectorDouble* location, FRotatorDouble* rotation, bool doubles);
