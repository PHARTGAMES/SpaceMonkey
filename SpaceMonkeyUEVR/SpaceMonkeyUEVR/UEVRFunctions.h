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


template<typename T>
struct ActorGetRootComponentFunctionParams {
    API::UObject* return_value{};
};

template<typename T>
struct FunctionNameTraits<ActorGetRootComponentFunctionParams<T>> {
    static constexpr const wchar_t* name = L"K2_GetRootComponent";
};


template<typename T>
struct ComponentAttachToComponentFunctionParams {
    API::UObject* parent;
    FAttachmentTransformRules attachmentRules = FAttachmentTransformRules::KeepRelativeTransform;
    API::FName socket_name;
    bool return_value{};
};


template<typename T>
struct FunctionNameTraits<ComponentAttachToComponentFunctionParams<T>> {
    static constexpr const wchar_t* name = L"AttachToComponent";
};




template<typename T>
struct K2ComponentAttachToComponentFunctionParams {
    API::UObject* parent;
    API::FName socket_name;
    EAttachmentRule location_rule;
    EAttachmentRule rotation_rule;
    EAttachmentRule scale_rule;
    bool weld_simulated_bodies;
    bool return_value{};
};


template<typename T>
struct FunctionNameTraits<K2ComponentAttachToComponentFunctionParams<T>> {
    static constexpr const wchar_t* name = L"K2_AttachToComponent";
};



template<typename T>
struct K2AttachToActorFunctionParams {
    API::UObject* parent;
    API::FName socket_name;
    EAttachmentRule location_rule;
    EAttachmentRule rotation_rule;
    EAttachmentRule scale_rule;
    bool weld_simulated_bodies;
    bool return_value;
};


template<typename T>
struct FunctionNameTraits<K2AttachToActorFunctionParams<T>> {
    static constexpr const wchar_t* name = L"K2_AttachToActor";
};

template<typename T>
struct AttachToActorFunctionParams {
    API::UObject* parent;
    FAttachmentTransformRules attachmentRules = FAttachmentTransformRules::KeepRelativeTransform;
    API::FName socket_name;
    bool return_value;
};


template<typename T>
struct FunctionNameTraits<AttachToActorFunctionParams<T>> {
    static constexpr const wchar_t* name = L"AttachToActor";
};




template<typename T>
struct ComponentSetRelativeLocationAndRotationFunctionParams {
    TVector<T> new_location;
    TRotator<T> new_rotation;
    bool sweep;
//    FHitResult outSweepHitResult; //dont care, always null
    FHitResult* outSweepHitResult; //dont care, always null
    ETeleportType teleport;
};


template<typename T>
struct FunctionNameTraits<ComponentSetRelativeLocationAndRotationFunctionParams<T>> {
    static constexpr const wchar_t* name = L"SetRelativeLocationAndRotation";
};



template<typename T>
struct ObjectCreateDefaultSubobjectParams {
    API::FName default_subobject_name;
    API::UClass* return_type;
    API::UClass* class_to_create_by_default;
    bool required;
    bool transient;
    API::UObject* return_value{};
};

template<typename T>
struct FunctionNameTraits<ObjectCreateDefaultSubobjectParams<T>> {
    static constexpr const wchar_t* name = L"CreateDefaultSubobject";
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


template<typename T>
inline TVector4<T> ConvertRotatorToQuaternion(const TRotator<T>& rot)
{
    // Define PI using the generic type T
    constexpr T PI = static_cast<T>(3.14159265358979323846);
    const T DEG_TO_RAD = PI / static_cast<T>(180);
    const T RADS_DIVIDED_BY_2 = DEG_TO_RAD / static_cast<T>(2);

    // Remove any full rotations (winding)
    T pitchNoWinding = std::fmod(rot.Pitch, static_cast<T>(360));
    T yawNoWinding = std::fmod(rot.Yaw, static_cast<T>(360));
    T rollNoWinding = std::fmod(rot.Roll, static_cast<T>(360));

    // Calculate sin and cos for half-angles
    T SP = std::sin(pitchNoWinding * RADS_DIVIDED_BY_2);
    T CP = std::cos(pitchNoWinding * RADS_DIVIDED_BY_2);
    T SY = std::sin(yawNoWinding * RADS_DIVIDED_BY_2);
    T CY = std::cos(yawNoWinding * RADS_DIVIDED_BY_2);
    T SR = std::sin(rollNoWinding * RADS_DIVIDED_BY_2);
    T CR = std::cos(rollNoWinding * RADS_DIVIDED_BY_2);

    // Convert Euler angles (pitch, yaw, roll) to a quaternion
    T x = CR * SP * SY - SR * CP * CY;
    T y = -CR * SP * CY - SR * CP * SY;
    T z = CR * CP * SY - SR * SP * CY;
    T w = CR * CP * CY + SR * SP * SY;

    return TVector4<T>(x, y, z, w);
}

template<typename T>
inline TVector<T> CrossProduct(const TVector<T>& A, const TVector<T>& B) 
{
    return TVector<T>(
        A.Y * B.Z - A.Z * B.Y,
        A.Z * B.X - A.X * B.Z,
        A.X * B.Y - A.Y * B.X
        );
}

// Generic element-wise multiplication for 4D vectors.
template<typename T>
inline TVector4<T> VectorMultiply(const TVector4<T>& Vec1, const TVector4<T>& Vec2) 
{
    return TVector4<T>(
        Vec1.X * Vec2.X,
        Vec1.Y * Vec2.Y,
        Vec1.Z * Vec2.Z,
        Vec1.W * Vec2.W
        );
}


template<typename T>
TVector4<T> VectorAdd(const TVector4<T>& Vec1, const TVector4<T>& Vec2) {
    return TVector4<T>(
        Vec1.X + Vec2.X,
        Vec1.Y + Vec2.Y,
        Vec1.Z + Vec2.Z,
        Vec1.W + Vec2.W
        );
}


// Generic quaternion–vector rotation.
// Rotates a 3D vector V by a quaternion Q using the formula:
//   T = 2 * (Q.xyz cross V)
//   V' = V + Q.W * T + (Q.xyz cross T)
template<typename T>
inline TVector<T> VectorQuaternionRotateVector(const TVector4<T>& Q, const TVector<T>& V) 
{
    // Form a 3D vector from the quaternion's x,y,z.
    TVector<T> QVec(Q.X, Q.Y, Q.Z);

    // Compute T = 2 * (Q cross V)
    TVector<T> Tvec = CrossProduct(QVec, V);
    Tvec.X *= 2;
    Tvec.Y *= 2;
    Tvec.Z *= 2;

    // Compute V' = V + Q.W * T + (Q cross T)
    TVector<T> VTemp(V.X + Q.W * Tvec.X,
        V.Y + Q.W * Tvec.Y,
        V.Z + Q.W * Tvec.Z);
    TVector<T> RotVec = CrossProduct(QVec, Tvec);

    return TVector<T>(VTemp.X + RotVec.X,
        VTemp.Y + RotVec.Y,
        VTemp.Z + RotVec.Z);
}


template<typename T>
inline TVector4<T> VectorMultiplyAdd(const TVector4<T>& A, const TVector4<T>& B, const TVector4<T>& C)
{
    return VectorAdd(VectorMultiply(A, B), C);
}


template<typename T>
inline TVector4<T> VectorReplicate(const TVector4<T>& Vec, int index) {
    T value = T{};
    switch (index) {
    case 0: value = Vec.X; break;
    case 1: value = Vec.Y; break;
    case 2: value = Vec.Z; break;
    case 3: value = Vec.W; break;
    default: break;
    }
    return TVector4<T>(value, value, value, value);
}

template<typename T>
inline TVector4<T> VectorSwizzle(const TVector4<T>& Vec, int i0, int i1, int i2, int i3) {
    auto getComponent = [&](int index) -> T {
        switch (index) {
        case 0: return Vec.X;
        case 1: return Vec.Y;
        case 2: return Vec.Z;
        case 3: return Vec.W;
        default: return T{};
        }
    };
    return TVector4<T>(
        getComponent(i0),
        getComponent(i1),
        getComponent(i2),
        getComponent(i3)
        );
}

template<typename T>
struct GlobalVectorConstants {
    static const TVector4<T> QMULTI_SIGN_MASK0;
    static const TVector4<T> QMULTI_SIGN_MASK1;
    static const TVector4<T> QMULTI_SIGN_MASK2;
};

template<typename T>
const TVector4<T> GlobalVectorConstants<T>::QMULTI_SIGN_MASK0 = TVector4<T>(+1, -1, +1, -1);

template<typename T>
const TVector4<T> GlobalVectorConstants<T>::QMULTI_SIGN_MASK1 = TVector4<T>(+1, +1, -1, -1);

template<typename T>
const TVector4<T> GlobalVectorConstants<T>::QMULTI_SIGN_MASK2 = TVector4<T>(-1, +1, +1, -1);


// Generic quaternion multiplication using a fused multiply-add approach.
// Quaternions are assumed to be stored as TVector4<T> with order (X, Y, Z, W).
template<typename T>
inline TVector4<T> VectorQuaternionMultiply(const TVector4<T>& Quat1, const TVector4<T>& Quat2)
{
    // Start with: Result = (Quat1.W replicated) * Quat2.
    TVector4<T> Result = VectorMultiply(VectorReplicate(Quat1, 3), Quat2);

    // Add: (Quat1.X replicated) * Swizzle(Quat2, W, Z, Y, X) multiplied by QMULTI_SIGN_MASK0.
    Result = VectorMultiplyAdd(
        VectorMultiply(VectorReplicate(Quat1, 0), VectorSwizzle(Quat2, 3, 2, 1, 0)),
        GlobalVectorConstants<T>::QMULTI_SIGN_MASK0,
        Result
    );

    // Add: (Quat1.Y replicated) * Swizzle(Quat2, Z, W, X, Y) multiplied by QMULTI_SIGN_MASK1.
    Result = VectorMultiplyAdd(
        VectorMultiply(VectorReplicate(Quat1, 1), VectorSwizzle(Quat2, 2, 3, 0, 1)),
        GlobalVectorConstants<T>::QMULTI_SIGN_MASK1,
        Result
    );

    // Add: (Quat1.Z replicated) * Swizzle(Quat2, Y, X, W, Z) multiplied by QMULTI_SIGN_MASK2.
    Result = VectorMultiplyAdd(
        VectorMultiply(VectorReplicate(Quat1, 2), VectorSwizzle(Quat2, 1, 0, 3, 2)),
        GlobalVectorConstants<T>::QMULTI_SIGN_MASK2,
        Result
    );

    return Result;
}

