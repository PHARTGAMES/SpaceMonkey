#pragma once
#include "SpaceMonkeyUEVRDefines.h"
#include "UEVRUtils.h"
#include "UEVRGamePlugin.h"
#include "UEVRGameConfigTypes.h"
using namespace uevr;


class SMUEVR_API SpaceMonkeyUEVR : public uevr::Plugin
{
protected:
    UEVRGamePlugin* m_game_plugin = nullptr;

public:
    SpaceMonkeyUEVR() = default;

    void on_dllmain() override {}

    void on_initialize() override;

    void on_pre_engine_tick(API::UGameEngine* engine, float delta);

    void on_post_engine_tick(API::UGameEngine* engine, float delta);

    void on_pre_slate_draw_window(UEVR_FSlateRHIRendererHandle renderer, UEVR_FViewportInfoHandle viewport_info);

    void on_post_slate_draw_window(UEVR_FSlateRHIRendererHandle renderer, UEVR_FViewportInfoHandle viewport_info);

    void load_game_plugin();

    static std::string get_module_path();
};

std::unique_ptr<SpaceMonkeyUEVR> g_plugin{ new SpaceMonkeyUEVR() };



