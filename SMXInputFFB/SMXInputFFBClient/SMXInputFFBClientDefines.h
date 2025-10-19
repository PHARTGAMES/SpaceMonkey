#pragma once

#ifdef SMXINPUTFFBCLIENT_EXPORTS
#define SMXINPUTFFBCLIENT_API __declspec(dllexport)
#else
#define SMXINPUTFFBCLIENT_API __declspec(dllimport)
#endif