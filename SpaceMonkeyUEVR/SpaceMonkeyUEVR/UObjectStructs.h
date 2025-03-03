#pragma once


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

// Type aliases
using FVectorFloat = TVector<float>;
using FRotatorFloat = TRotator<float>;
using FVectorDouble = TVector<double>;
using FRotatorDouble = TRotator<double>;



