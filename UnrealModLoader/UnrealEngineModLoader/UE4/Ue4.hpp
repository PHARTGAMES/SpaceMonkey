#pragma once
#include <Windows.h>
#include <Psapi.h>
#include "Utilities/EngineDefFinder.h"
#include "Basic.hpp"
#include "Utilities//Pattern.h"
#include "CoreUObject_classes.hpp"
namespace UE4
{
	static void InitSDK()
	{
		DWORD64   Names_offset = NULL;
		if (!FName::IsUsingNamePool())
		{
			Names_offset = (*(DWORD64*)(GameProfile::SelectedGameProfile.GName));
			FName::GNames = (DWORD*)Names_offset;
		}
		else
		{
			Names_offset = (DWORD64)(GameProfile::SelectedGameProfile.GName);
			FName::GNames = (DWORD*)Names_offset;
		}

		DWORD64   GObjObjects_offset = NULL;
		GObjObjects_offset = (DWORD64)(GameProfile::SelectedGameProfile.GObject);
		UObject::GObjects = (FUObjectArray*)GObjObjects_offset;


		Log::Info("GObjects->ObjFirstGCIndex: %d", UObject::GObjects->ObjFirstGCIndex);
		Log::Info("GObjects->ObjLastNonGCIndex: %d", UObject::GObjects->ObjLastNonGCIndex);
		Log::Info("GObjects->MaxObjectsNotConsideredByGC: %d", UObject::GObjects->MaxObjectsNotConsideredByGC);
		Log::Info("GObjects->OpenForDisregardForGC: %d", UObject::GObjects->OpenForDisregardForGC);
		Log::Info("GObjects->ObjObjects: 0x%p", UObject::GObjects->ObjObjects);


		DWORD64   GWorldObjects = NULL;
		GWorldObjects = (DWORD64)(GameProfile::SelectedGameProfile.GWorld);
		UWorld::GWorld = (UWorld**)GWorldObjects;


		#ifdef UNREALENGINEMODLOADER_EXPORTS //Stops dumb errors from the ExampleMod shit
		if (GameProfile::SelectedGameProfile.IsUObjectMissing)
		{
			Log::Warn("UObject Not Defined. Scanning for def.");
			UE4::UObject* CoreUobjectObject = nullptr;
			UE4::UObject* UEObject = nullptr;
			int baseIndex = 1;

			uint16_t storedIndexOffs = GameProfile::SelectedGameProfile.defs.UObject.Index;

			if (GameProfile::SelectedGameProfile.IsUsingFChunkedFixedUObjectArray)
			{
				Log::Info("UObject Array Length: %d", UE4::UObject::GObjects->GetAsChunckArray().Num());


				int arrayCount = UE4::UObject::GObjects->GetAsChunckArray().Num();

				Log::Info("UObject Array Length: %d", UE4::UObject::GObjects->GetAsChunckArray().Num());


				for (baseIndex = 1; baseIndex < arrayCount; baseIndex++)
				{
					UE4::UObject* obj = UE4::UObject::GObjects->GetAsChunckArray().GetByIndex(baseIndex).Object;

					Log::Info("InitSDK:LoopObjects: Index: %d, Object 0x%p", baseIndex, obj);
				}


				for (baseIndex = 1; baseIndex < arrayCount; baseIndex++)
				{
					CoreUobjectObject = UE4::UObject::GObjects->GetAsChunckArray().GetByIndex(baseIndex).Object;
					UEObject = UE4::UObject::GObjects->GetAsChunckArray().GetByIndex(baseIndex+1).Object;

					if (CoreUobjectObject != nullptr && UEObject != nullptr)
					{
						Log::Info("Found CoreUobjectObject and UEObject at baseIndex: %d", baseIndex);

						GameProfile::SelectedGameProfile.defs.UObject.Index = storedIndexOffs;
						if (ClassDefFinder::FindUObjectIndexDefs(CoreUobjectObject, UEObject, baseIndex))
						{
							break;
						}
					}
				}

				//CoreUobjectObject = UE4::UObject::GObjects->GetAsChunckArray().GetByIndex(1).Object;
				//UEObject = UE4::UObject::GObjects->GetAsChunckArray().GetByIndex(2).Object;

			}
			else
			{
				Log::Info("UObject Array Length: %d", UE4::UObject::GObjects->GetAsTUArray().Num());

				CoreUobjectObject = UE4::UObject::GObjects->GetAsTUArray().GetByIndex(1).Object;
				UEObject = UE4::UObject::GObjects->GetAsTUArray().GetByIndex(2).Object;
			}
			ClassDefFinder::FindUObjectDefs(CoreUobjectObject, UEObject, baseIndex);
			GameProfile::SelectedGameProfile.IsUObjectMissing = false;
		}



		if (GameProfile::SelectedGameProfile.IsUFieldMissing)
		{
			Log::Warn("UField Not Defined. Scanning for def.");
			ClassDefFinder::FindUFieldDefs();
			GameProfile::SelectedGameProfile.IsUFieldMissing = false;
		}
		if (GameProfile::SelectedGameProfile.IsUStructMissing)
		{
			Log::Warn("UStruct Not Defined. Scanning for def.");
			ClassDefFinder::FindUStructDefs();
			GameProfile::SelectedGameProfile.IsUStructMissing = false;
		}
		if (GameProfile::SelectedGameProfile.IsUFunctionMissing)
		{
			Log::Warn("UFunction Not Defined. Scanning for def.");
			ClassDefFinder::FindUFunctionDefs();
			GameProfile::SelectedGameProfile.IsUFunctionMissing = false;
		}

		if (GameProfile::SelectedGameProfile.IsPropertyMissing)
		{
			ClassDefFinder::FindUEProperty();
		}
		Log::Info("All Engine Classes Found");
		#endif

	}
}