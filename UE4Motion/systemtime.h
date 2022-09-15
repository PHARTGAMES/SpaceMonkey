//===================== Copyright (c) Valve Corporation. All Rights Reserved. ======================
//
// Functions for working with the system clock.
//
//==================================================================================================
#pragma once

#include <stdint.h>

namespace SystemTime
{
	// Automatically invoked, but can be called to specify a common base ticks for synchronization between processes.
	__declspec(dllexport) void Init( uint64_t nBaseTicks = 0 );

	// Returns the base ticks (for synchronizing with another process).
	__declspec(dllexport) uint64_t GetBaseTicks();

	// Returns current system time in ticks.
	__declspec(dllexport) uint64_t GetInTicks();

	// Returns current system time in seconds.
	__declspec(dllexport) double GetInSeconds();

	// Converts ticks to seconds.
	__declspec(dllexport) double GetInSeconds( uint64_t nTicks );
}

