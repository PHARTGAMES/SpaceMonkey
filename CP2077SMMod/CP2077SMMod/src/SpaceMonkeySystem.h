#pragma once

#include <RedLib.hpp>
#include "SpaceMonkeyTelemetryAPI.h"

inline const RED4ext::Sdk* gSdk;
inline RED4ext::PluginHandle gPluginHandle;
using namespace Red;

struct SpaceMonkeySystem : public Red::IGameSystem
{
public:

    virtual void OnRegisterUpdates(UpdateRegistrar* aRegistrar) override;
    virtual void OnInitialize(const JobHandle& aJob) override; 
    virtual void OnUninitialize() override; 

    void Tick(FrameInfo& aInfo, JobQueue& aQueue) noexcept;

    SpaceMonkeyTelemetryAPI* m_telemetryAPI = nullptr;
    SpaceMonkeyTelemetryFrameData m_frameData{};
    double m_systemTime = 0.0;

private:

    RTTI_IMPL_TYPEINFO(SpaceMonkeySystem);
    RTTI_IMPL_ALLOCATOR();


};

RTTI_DEFINE_CLASS(SpaceMonkeySystem);
