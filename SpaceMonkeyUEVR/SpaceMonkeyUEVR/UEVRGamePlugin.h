#pragma once
#include "uevr/API.hpp"

using namespace uevr;

class UEVRGameConfig;

class UEVRGamePlugin
{

public:

    UEVRGamePlugin(UEVRGameConfig* a_game_config) {}

    virtual void on_pre_engine_tick(API::UGameEngine * engine, float delta) = 0;

    virtual void on_post_engine_tick(API::UGameEngine* engine, float delta) = 0;

};

