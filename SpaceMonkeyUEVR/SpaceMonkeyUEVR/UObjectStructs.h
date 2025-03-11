#pragma once
#include "uevr/API.hpp"

using namespace uevr;

//vector
template<typename T>
struct TVector {
    T X, Y, Z;

    TVector() : X(T{}), Y(T{}), Z(T{}) { }
    TVector(T x, T y, T z) : X(x), Y(y), Z(z) { }

    template<typename U>
    TVector(const TVector<U>& Other)
        : X(static_cast<T>(Other.X))
        , Y(static_cast<T>(Other.Y))
        , Z(static_cast<T>(Other.Z))
    { }
};

template<typename T>
struct TVector4 {
    T X, Y, Z, W;


    TVector4() : X(T{}), Y(T{}), Z(T{}), W(T{}) { }
    TVector4(T x, T y, T z, T w) : X(x), Y(y), Z(z), W(w) { }



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
using FVector4Double = TVector4<double>;
using FVector4Float = TVector4<float>;


enum class ESpawnActorCollisionHandlingMethod
{
    Undefined,
    AlwaysSpawn,
    AdjustIfPossibleButAlwaysSpawn,
    AdjustIfPossibleButDontSpawnIfColliding,
    DontSpawnIfColliding
};

enum class ESpawnActorScaleMethod
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


// A constexpr function to compute the maximum of two values
constexpr std::size_t constexpr_max(std::size_t a, std::size_t b) {
    return a > b ? a : b;
}

// Primary template for TAlignOfTransform (left undefined)
template<typename T>
struct TAlignOfTransform;

// Specialization for float
template<>
struct TAlignOfTransform<float> {
    // Ensure a minimum alignment of 16 bytes
    static constexpr std::size_t Value = constexpr_max(16, alignof(TVector4<float>));
};

// Specialization for double
template<>
struct TAlignOfTransform<double> {
    // Ensure a minimum alignment of 16 bytes
    static constexpr std::size_t Value = constexpr_max(16, alignof(TVector4<double>));
};

template<typename T>
struct alignas(TAlignOfTransform<T>::Value) TTransform {
    TVector4<T> rotation;
    TVector4<T> translation;
    TVector4<T> scale3d;
};

