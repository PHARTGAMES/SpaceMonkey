#pragma once
#include "uevr/API.hpp"

using namespace uevr;

//vector
template<typename T>
struct TVector {
    T X, Y, Z;

    TVector() : X(T{}), Y(T{}), Z(T{}) { }
    TVector(T x, T y, T z) : X(x), Y(y), Z(z) { }
};

//rotator
template<typename T>
struct TRotator {
    T Pitch, Yaw, Roll;

    TRotator() : Pitch(T{}), Yaw(T{}), Roll(T{}) { }
    TRotator(T pitch, T yaw, T roll) : Pitch(pitch), Yaw(yaw), Roll(roll) { }
};


//quaternion
template<typename T>
struct TQuaternion {
    T X, Y, Z, W;

    TQuaternion() : X(T{}), Y(T{}), Z(T{}), W(T{}) { }
    TQuaternion(T x, T y, T z, T w) : X(x), Y(y), Z(z), W(w) { }
};

// Type aliases
using FVectorFloat = TVector<float>;
using FRotatorFloat = TRotator<float>;
using FVectorDouble = TVector<double>;
using FRotatorDouble = TRotator<double>;



enum ESpawnActorCollisionHandlingMethod
{
    Undefined,
    AlwaysSpawn,
    AdjustIfPossibleButAlwaysSpawn,
    AdjustIfPossibleButDontSpawnIfColliding,
    DontSpawnIfColliding
};

enum ESpawnActorScaleMethod
{
    OverrideRootScale,
    MultiplyWithRoot,
    SelectDefaultAtRuntime
};


struct FActorSpawnParameters 
{
    API::FName name;
    API::UObject* actor_template;
    API::UObject* owner;
    API::UObject* instigator;
    API::UObject* override_level;
    API::UObject* override_parent_component;
    ESpawnActorCollisionHandlingMethod spawn_collision_handling_override;
    ESpawnActorScaleMethod tranform_scale_method;
};

template<typename T>
struct TTransform {
    TVector<T> location;
    TQuaternion<T> rotation;

};


