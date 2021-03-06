# WRC Telemetry

This is a patch for WRC7, WRC8, and WRC9 that enables telemetry through shared memory.

Included you will find:
* `WrcInjectionPayload.dll` -- a replacement DLL for `PhysXCooking64_s.dll`
* `DirtRally2.exe` -- pretends WRC is DiRT Rally 2.0 for use with dashboards and motion systems

## Installation

Unzip `wrc-telemetry.zip` in WRC's installation folder then run `Install.bat`.

## Manual installation

1. Unzip `wrc-telemetry.zip` in WRC's installation folder.
2. Rename `PhysXCooking64_s.dll` to `PhysXCooking64_s_org.dll`.
3. Rename `WrcInjectionPayload.dll` to `PhysXCooking64_s.dll`.

NOTE: The file `PhysXCooking64_s_org.dll` must exist. It must be the original
      `PhysXCooking64_s.dll` from the latest version of the game.

## Usage

1. Install using the instructions above.
2. (optional) Start `DirtRally2.exe` to pretend that WRC is DiRT Rally 2.0. This is necesarry unless
              your dashboard/buttkicker/motion system includes support for this patch.
3. Start your dashboard/buttkicker/motion system.
4. Start WRC.
5. Enjoy :)

### Usage with SimTools

SimTools does not include native support for this telemetry protocol (as of Jan 2021.) Instead use
the included DiRT Rally 2.0 emulator:

	DirtRally2.exe /port 4123 /protocol simtools

SimTools doesn't start unless the game is "Patched". Follow the prompts issued by SimTools and select
the folder `Documents\My Games\DiRT Rally 2.0` when prompted (even if you don't have DiRT Rally 2.0
installed.)

### For use with SimHub

SimHub does not include native support for this telemetry protocol (as of Jan 2021.) Instead use
the included DiRT Rally 2.0 emulator:

	DirtRally2.exe /protocol extradata3

SimHub will complain that "DiRT Rally 2.0 telemetry is not configured". Ignore these warnings.

## Will it break in the future?

Yes.

When Epic Games Launcher decides to update the game it will overwrite `PhysXCooking64_s.dll` with an
unpatched copy. Run `Install.bat` to fix the issue.

The patch is resilient to smaller changes in WRC. However, if major changes are made to WRC this
patch will stop working permanently.

## Where is the source code?

I've shown this is possible. Now do it yourself.

## Is this legal?

Yes. This package includes no copyrighted code or other assets. IANAL.

## Telemetry protocol

```c++

#include <inttypes.h>

constexpr const wchar_t *SHARED_MEMORY_NAME =
	L"Local\\WRC-8wSotWzFKAhBlbW10ZJBKaWMdWszbBXg";

#pragma pack(push, 1)

struct WrcTelemetry
{
	uint32_t sequence_number; // Odd while game is updating shared memory
	uint32_t version; // Version of this struct

	// Version 1:
	int32_t gear; // Neutral = 1, First = 2, ...
	float velocity[3]; // Left, up, forward [m/s]
	float acceleration[3]; // Left, up, forward [m/s^2]
	int32_t engine_idle_rpm; // [rpm/10]
	int32_t engine_max_rpm; // [rpm/10]
	int32_t engine_rpm; // [rpm/10]
	float suspension_travel[4]; // It can move this much (LF,LR,RR,RF) [m]
	float suspension_position[4]; // From zero to `suspension_travel` [m]
	float unknown[4]; // (LF,LR,RR,RF) [?]
};

#pragma pack(pop)

```

## Known issues

* Lap time not available (compatibility value is guesstimated)
* Sector time/diff not available
* In game RPM meter does not match reported RPM value (game UI bug?)

## Changelog

Version 1.1:
* Semantic versioning
* Adds UDP packet rate limiting; use `/rate` flag to adjust
* Gear was off by one for DiRT Rally 2.0 compatibility
* RPM was off by factor 10 for DiRT Rally 2.0 compatibility
