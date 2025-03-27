#pragma once

#include <RedLib.hpp>

inline const RED4ext::Sdk* gSdk;
inline RED4ext::PluginHandle gPluginHandle;
using namespace Red;

struct SpaceMonkeySystem : public Red::IGameSystem
{
public:



    // Override OnRegisterUpdates from IUpdatableSystem.
    // This method will be called by the update registrar, allowing our system to register its update callback.
    virtual void OnRegisterUpdates(UpdateRegistrar* aRegistrar) override;

    void Tick(FrameInfo& aInfo, JobQueue& aQueue) noexcept;

private:

    RTTI_IMPL_TYPEINFO(SpaceMonkeySystem);
    RTTI_IMPL_ALLOCATOR();


};
