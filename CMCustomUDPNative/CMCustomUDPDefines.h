#pragma once

#ifdef CMCUSTOMUDPNATIVE_EXPORTS
#define CMCUSTOMUDPNATIVE_API __declspec(dllexport)
#else
#define CMCUSTOMUDPNATIVE_API __declspec(dllimport)
#endif