#pragma once

#ifdef SPACEMONKEYUEVR_EXPORTS
#define SMUEVR_API __declspec(dllexport)
#else
#define SMUEVR_API __declspec(dllimport)
#endif