// dllmain.cpp : Defines the entry point for the DLL application.
#include "UE4Motion.h"
//Mod* CoreMod;

static Mod* CoreMod = NULL;
void CreateMod()
{
    CoreMod = new UE4Motion();
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        CreateMod();
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        if (CoreMod != NULL && lpReserved != NULL)
        {
            ((UE4Motion*)CoreMod)->OnDestroy();
        }
        break;
    }
    return TRUE;
}

