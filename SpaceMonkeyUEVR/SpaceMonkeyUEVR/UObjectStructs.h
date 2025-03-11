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


template <typename T, int Version>
struct BeginDeferredActorSpawnFromClassParams;

template <typename T>
struct BeginDeferredActorSpawnFromClassParams<T, 0> {
    API::UObject* pawn;
    API::UClass* actor_class;
    TTransform<T> spawn_transform;
    ESpawnActorCollisionHandlingMethod collision_handling_override;
    API::UObject* owner;
    // Version 0: no transform scale method
    API::UObject* return_value{};
};

template <typename T>
struct BeginDeferredActorSpawnFromClassParams<T, 1> {
    API::UObject* pawn;
    API::UClass* actor_class;
    TTransform<T> spawn_transform;
    ESpawnActorCollisionHandlingMethod collision_handling_override;
    API::UObject* owner;
    ESpawnActorScaleMethod tranform_scale_method;
    API::UObject* return_value{};
};

// Templated structs for FinishSpawningActor parameters

template <typename T, int Version>
struct FinishSpawningActorParams;

template <typename T>
struct FinishSpawningActorParams<T, 0> {
    API::UObject* actor;
    TTransform<T> spawn_transform;
    // Version 0: no transform scale method
    API::UObject* return_value{};
};

template <typename T>
struct FinishSpawningActorParams<T, 1> {
    API::UObject* actor;
    TTransform<T> spawn_transform;
    ESpawnActorScaleMethod transform_scale_method;
    API::UObject* return_value{};
};

enum class EAttachmentRule : unsigned char
{
    /** Keeps current relative transform as the relative transform to the new parent. */
    KeepRelative,

    /** Automatically calculates the relative transform such that the attached component maintains the same world transform. */
    KeepWorld,

    /** Snaps transform to the attach point */
    SnapToTarget
};

enum EComponentMobility : int
{
    /**
 * Static objects cannot be moved or changed in game.
 * - Allows baked lighting
 * - Fastest rendering
 */
    Static,

    /**
     * A stationary light will only have its shadowing and bounced lighting from static geometry baked by Lightmass, all other lighting will be dynamic.
     * - It can change color and intensity in game.
     * - Can't move
     * - Allows partial baked lighting
     * - Dynamic shadows
     */
     Stationary,

     /**
      * Movable objects can be moved and changed in game.
      * - Totally dynamic
      * - Can cast dynamic shadows
      * - Slowest rendering
      */
      Movable
};

enum ECollisionEnabled : int
{
    /** Will not create any representation in the physics engine. Cannot be used for spatial queries (raycasts, sweeps, overlaps) or simulation (rigid body, constraints). Best performance possible (especially for moving objects) */
    NoCollision,
    /** Only used for spatial queries (raycasts, sweeps, and overlaps). Cannot be used for simulation (rigid body, constraints). Useful for character movement and things that do not need physical simulation. Performance gains by keeping data out of simulation tree. */
    QueryOnly,
    /** Only used only for physics simulation (rigid body, constraints). Cannot be used for spatial queries (raycasts, sweeps, overlaps). Useful for jiggly bits on characters that do not need per bone detection. Performance gains by keeping data out of query tree */
    PhysicsOnly,
    /** Can be used for both spatial queries (raycasts, sweeps, overlaps) and simulation (rigid body, constraints). */
    QueryAndPhysics,
    /** Only used for probing the physics simulation (rigid body, constraints). Cannot be used for spatial queries (raycasts,
    sweeps, overlaps). Useful for when you want to detect potential physics interactions and pass contact data to hit callbacks
    or contact modification, but don't want to physically react to these contacts. */
    ProbeOnly,
    /** Can be used for both spatial queries (raycasts, sweeps, overlaps) and probing the physics simulation (rigid body,
    constraints). Will not allow for actual physics interaction, but will generate contact data, trigger hit callbacks, and
    contacts will appear in contact modification. */
    QueryAndProbe
};

struct FAttachmentTransformRules
{

    static FAttachmentTransformRules KeepRelativeTransform;
    static FAttachmentTransformRules KeepWorldTransform;
    static FAttachmentTransformRules SnapToTargetNotIncludingScale;
    static FAttachmentTransformRules SnapToTargetIncludingScale;

    //FAttachmentTransformRules()
    //    : LocationRule(EAttachmentRule::KeepRelative),
    //    RotationRule(EAttachmentRule::KeepRelative),
    //    ScaleRule(EAttachmentRule::KeepRelative),
    //    bWeldSimulatedBodies(false)
    //{}

    FAttachmentTransformRules(EAttachmentRule InRule, bool bInWeldSimulatedBodies)
        : LocationRule(InRule)
        , RotationRule(InRule)
        , ScaleRule(InRule)
        , bWeldSimulatedBodies(bInWeldSimulatedBodies)
    {}

    FAttachmentTransformRules(EAttachmentRule InLocationRule, EAttachmentRule InRotationRule, EAttachmentRule InScaleRule, bool bInWeldSimulatedBodies)
        : LocationRule(InLocationRule)
        , RotationRule(InRotationRule)
        , ScaleRule(InScaleRule)
        , bWeldSimulatedBodies(bInWeldSimulatedBodies)
    {}

    EAttachmentRule LocationRule;

    /** The rule to apply to rotation when attaching */
    EAttachmentRule RotationRule;

    /** The rule to apply to scale when attaching */
    EAttachmentRule ScaleRule;

    /** Whether to weld simulated bodies together when attaching */
    bool bWeldSimulatedBodies;
};


enum class ETeleportType : unsigned char
{
    /** Do not teleport physics body. This means velocity will reflect the movement between initial and final position, and collisions along the way will occur */
    None,

    /** Teleport physics body so that velocity remains the same and no collision occurs */
    TeleportPhysics,

    /** Teleport physics body and reset physics state completely */
    ResetPhysics
};

struct FHitResult
{
    unsigned char padding[264];
};