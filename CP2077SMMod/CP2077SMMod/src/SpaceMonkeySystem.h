#pragma once

#include <RED4ext/RED4ext.hpp>
#include <RedLib.hpp>
#include "SpaceMonkeyTelemetryAPI.h"

// RED4ext.SDK removed the unversioned API aliases (Sdk, PluginHandle, ...) in 2026; re-expose the v1 API.
namespace RED4ext { using namespace v1; }

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
