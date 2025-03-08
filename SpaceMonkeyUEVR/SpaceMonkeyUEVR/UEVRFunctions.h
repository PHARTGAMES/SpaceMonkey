#pragma once

#include "uevr/API.hpp"
#include "UObjectStructs.h"
using namespace uevr;

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



template<typename T>
struct GetActorForwardFunctionParams {
    TVector<T> return_value;
};

template<typename T>
struct FunctionNameTraits<GetActorForwardFunctionParams<T>> {
    static constexpr const wchar_t* name = L"GetActorForwardVector";
};

template<typename T>
struct GetActorUpFunctionParams {
    TVector<T> return_value;
};

template<typename T>
struct FunctionNameTraits<GetActorUpFunctionParams<T>> {
    static constexpr const wchar_t* name = L"GetActorUpVector";
};

template<typename T>
struct GetActorRightFunctionParams {
    TVector<T> return_value;
};

template<typename T>
struct FunctionNameTraits<GetActorRightFunctionParams<T>> {
    static constexpr const wchar_t* name = L"GetActorRightVector";
};

template<typename T>
struct GetActorComponentsParams {
    API::UClass* c;
    API::TArray<API::UObject*> return_value{};
};


template<typename T>
struct FunctionNameTraits<GetActorComponentsParams<T>> {
    static constexpr const wchar_t* name = L"K2_GetComponentsByClass";
};





template<typename T>
struct PawnIsLocallyControlledFunctionParams {
    bool return_value;
};

template<typename T>
struct FunctionNameTraits<PawnIsLocallyControlledFunctionParams<T>> {
    static constexpr const wchar_t* name = L"IsLocallyControlled";
};



template<typename T>
struct PawnIsPlayerControlledFunctionParams {
    bool return_value;
};

template<typename T>
struct FunctionNameTraits<PawnIsPlayerControlledFunctionParams<T>> {
    static constexpr const wchar_t* name = L"IsPlayerControlled";
};



template<typename T>
struct ActorGetWorldFunctionParams {
    API::UWorld* return_value; //world
};

template<typename T>
struct FunctionNameTraits<ActorGetWorldFunctionParams<T>> {
    static constexpr const wchar_t* name = L"GetWorld";
};


template<typename T>
struct WorldSpawnActorFunctionParams {
    API::UClass* actor_class;
    TVector<T> *location;
    TRotator<T> *rotation;
    FActorSpawnParameters &spawn_parameters;

    API::UObject* return_value; //aactor

    WorldSpawnActorFunctionParams(FActorSpawnParameters& InSpawnParameters)
        : spawn_parameters(InSpawnParameters), actor_class(nullptr), location(nullptr), rotation(nullptr), return_value(nullptr)
    {}
};

template<typename T>
struct FunctionNameTraits<WorldSpawnActorFunctionParams<T>> {
    static constexpr const wchar_t* name = L"SpawnActor";
};












// A templated helper function to wrap call_function
// (Assumes your uobject type has a member function call_function that accepts a wchar_t* and a void pointer)
template<typename FuncParams>
void call_function_on_uobject(API::UObject* object, FuncParams* params) 
{
    object->call_function(FunctionNameTraits<FuncParams>::name, params);
}








void get_actor_transform(uevr::API::UObject* uobject, FVectorDouble* location, FRotatorDouble* rotation, bool doubles);

void get_actor_transform_vectors(uevr::API::UObject* uobject, FVectorDouble* location, FVectorDouble* forward, FVectorDouble* up, FVectorDouble* right, bool doubles);

//bool get_uobject_is_valid(uevr::API::UObject* uobject);