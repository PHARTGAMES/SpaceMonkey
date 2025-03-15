#pragma once
#include "uevr/API.hpp"
#include "UObjectStructs.h"
#include "SpaceMonkeyTelemetryAPI.h"
#include <windows.h>

using namespace uevr;

class UEVRGameConfig;

class UEVRGamePlugin
{
protected:

    SpaceMonkeyTelemetryAPI* m_telemetryAPI;
    SpaceMonkeyTelemetryFrameData m_frameData;
public:

    UEVRGamePlugin(UEVRGameConfig* a_game_config);
    ~UEVRGamePlugin();

    virtual void on_pre_engine_tick(API::UGameEngine * engine, float delta) = 0;

    virtual void on_post_engine_tick(API::UGameEngine* engine, float delta) = 0;

    virtual void on_present() = 0;
};

