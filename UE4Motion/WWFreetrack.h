#pragma once

#include <vector>
#include <thread>
#include <chrono>

#if defined( _WINDOWS )
#include <windows.h>
#endif

class __declspec(dllexport) WWFreetrack
{
public:

	const char* FREETRACK_HEAP = "FT_SharedMem";
	const char* FREETRACK_MUTEX = "FT_Mutext";

	/* only 6 headpose floats and the data id are filled -sh */
	typedef struct FTData__ {
		uint32_t DataID;
		int32_t CamWidth;
		int32_t CamHeight;
		/* virtual pose */
		float  Yaw;   /* positive yaw to the left */
		float  Pitch; /* positive pitch up */
		float  Roll;  /* positive roll to the left */
		float  X;
		float  Y;
		float  Z;
		/* raw pose with no smoothing, sensitivity, response curve etc. */
		float  RawYaw;
		float  RawPitch;
		float  RawRoll;
		float  RawX;
		float  RawY;
		float  RawZ;
		/* raw points, sorted by Y, origin top left corner */
		float  X1;
		float  Y1;
		float  X2;
		float  Y2;
		float  X3;
		float  Y3;
		float  X4;
		float  Y4;
	} volatile FTData;

	typedef struct FTHeap__ {
		FTData data;
		int32_t GameID;
		union
		{
			unsigned char table[8];
			int32_t table_ints[2];
		};
		int32_t GameID2;
	} volatile FTHeap;

	HANDLE hFTMemMap;
	FTHeap* ipc_heap;
	HANDLE ipc_mutex;

	FTData* m_ftData;
	std::thread* m_ftThread = NULL;
	bool m_initialized = false;

	bool IsInitialized()
	{
		return m_initialized;
	}

	//FreeTrack implementation from OpenTrack (https://github.com/opentrack/opentrack/tree/unstable/freetrackclient)
	bool Create()
	{
		if (ipc_heap != NULL)
			return TRUE;

		hFTMemMap = CreateFileMappingA(INVALID_HANDLE_VALUE,
			NULL,
			PAGE_READWRITE,
			0,
			sizeof(FTHeap),
			(LPCSTR)FREETRACK_HEAP);

		if (hFTMemMap == NULL)
			return (ipc_heap = NULL), FALSE;

		ipc_heap = (FTHeap*)MapViewOfFile(hFTMemMap, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(FTHeap));
		ipc_mutex = CreateMutexA(NULL, FALSE, FREETRACK_MUTEX);

		m_ftThread = new std::thread(&WWFreetrack::FTRead, this);

		m_initialized = true;

		return TRUE;
	}

	void Destroy()
	{
		if (m_initialized)
		{
			m_initialized = false;

			if (m_ftThread)
			{
				m_ftThread->join();
				delete m_ftThread;
				m_ftThread = nullptr;
			}
		}
	}

	void FTRead()
	{
		while (m_initialized)
		{
			if (ipc_mutex && WaitForSingleObject(ipc_mutex, 16) == WAIT_OBJECT_0) {
				memcpy(&m_ftData, &ipc_heap, sizeof(m_ftData));
				if (ipc_heap->data.DataID > (1 << 29))
					ipc_heap->data.DataID = 0;
				ReleaseMutex(ipc_mutex);
			}
		}
	}

	FTData *GetFTData()
	{
		return m_ftData;
	}
};

