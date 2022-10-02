#pragma once
#include <vector>
#include <thread>
#include <chrono>
#include "Debug.h"

#if defined( _WINDOWS )
#include <windows.h>
#endif

enum __declspec(dllexport) WWSharedMemType
{
	WWSharedMem_Read,
	WWSharedMem_Write,
};

class __declspec(dllexport) WWSharedMemory
{
public:
	const char *m_memoryName;
	const char *m_mutexName;
	int m_memorySize;
	bool m_initialized = false;
	HANDLE m_memMap;
	HANDLE m_mutex;
	void* m_mappedView = NULL;
	void* m_memoryCopy = NULL;
	WWSharedMemType m_type = WWSharedMemType::WWSharedMem_Read;
	std::thread* m_thread = NULL;

	WWSharedMemory(const char* a_memoryName, const char* a_mutexName, WWSharedMemType a_type, void *a_memoryCopy, int a_memorySize)
	{
		m_memoryName = a_memoryName;
		m_mutexName = a_mutexName;
		m_memoryCopy = a_memoryCopy;
		m_type = a_type;
		m_memorySize = a_memorySize;
		m_initialized = false;

		switch (m_type)
		{
			case WWSharedMemType::WWSharedMem_Read:
			{
				//m_memMap = OpenFileMappingA(FILE_MAP_ALL_ACCESS,
				//	FALSE,
				//	(LPCSTR)m_memoryName);

				m_memMap = CreateFileMappingA(INVALID_HANDLE_VALUE,
					NULL,
					PAGE_READWRITE,
					0,
					m_memorySize,
					(LPCSTR)m_memoryName);

				if (m_memMap == NULL)
				{
					Debug::Log("Failed to open %s for read\n", a_memoryName);
					return;
				}

				m_mappedView = (void*)MapViewOfFile(m_memMap, FILE_MAP_ALL_ACCESS, 0, 0, m_memorySize);
				m_mutex = CreateMutexA(NULL, FALSE, m_mutexName);
//				m_thread = new std::thread(&WWSharedMemory::ThreadRead, this);

				if(m_mappedView != NULL)
					Debug::Log("Opened %s for read\n", a_memoryName);
				else
					Debug::Log("Failed to open %s for read\n", a_memoryName);



				break;
			}
			case WWSharedMemType::WWSharedMem_Write:
			{
				m_memMap = CreateFileMappingA(INVALID_HANDLE_VALUE,
					NULL,
					PAGE_READWRITE,
					0,
					m_memorySize,
					(LPCSTR)m_memoryName);

				if (m_memMap == NULL)
				{
					Debug::Log("Failed to open %s for write\n", a_memoryName);
					return;
				}

				m_mappedView = (void*)MapViewOfFile(m_memMap, FILE_MAP_ALL_ACCESS, 0, 0, m_memorySize);
				m_mutex = CreateMutexA(NULL, FALSE, m_mutexName);
//				m_thread = new std::thread(&WWSharedMemory::ThreadWrite, this);


				if (m_mappedView != NULL)
					Debug::Log("Opened %s for write\n", a_memoryName);
				else
					Debug::Log("Failed to open %s for write\n", a_memoryName);

				break;
			}
		}

		m_initialized = true;
	}

	void Destroy()
	{
		if (m_initialized)
		{
			m_initialized = false;

			if (m_thread)
			{
				m_thread->join();
				delete m_thread;
				m_thread = nullptr;
			}
		}
	}

	void ThreadRead()
	{
		while (m_initialized)
		{
			if (m_mutex)
			{
				DWORD result = WaitForSingleObject(m_mutex, INFINITE);
				if (result == WAIT_OBJECT_0)
				{
					//					memcpy(m_memoryCopy, m_mappedView, m_memorySize);
					CopyMemory(m_memoryCopy, m_mappedView, m_memorySize);
					ReleaseMutex(m_mutex);
				}
			}
		}
	}

	void ThreadWrite()
	{
		while (m_initialized)
		{
			if (m_mutex)
			{
				DWORD result = WaitForSingleObject(m_mutex, INFINITE);
				if (result == WAIT_OBJECT_0)
				{
//					memcpy(m_mappedView, m_memoryCopy, m_memorySize);
					CopyMemory(m_mappedView, m_memoryCopy, m_memorySize);
					ReleaseMutex(m_mutex);
				}
			}
		}
	}

	void Read()
	{
		if (m_mutex)
		{
			DWORD result = WaitForSingleObject(m_mutex, INFINITE);
			if (result == WAIT_OBJECT_0)
			{
				//					memcpy(m_memoryCopy, m_mappedView, m_memorySize);
				CopyMemory(m_memoryCopy, m_mappedView, m_memorySize);
				ReleaseMutex(m_mutex);
			}
		}
	}

	void Write()
	{
		if (m_mutex)
		{
			DWORD result = WaitForSingleObject(m_mutex, INFINITE);
			if (result == WAIT_OBJECT_0)
			{
				//					memcpy(m_mappedView, m_memoryCopy, m_memorySize);
				CopyMemory(m_mappedView, m_memoryCopy, m_memorySize);
				ReleaseMutex(m_mutex);
			}
		}
	}

	void ReadUnsafe()
	{
		CopyMemory(m_memoryCopy, m_mappedView, m_memorySize);
	}

	void WriteUnsafe()
	{
		CopyMemory(m_mappedView, m_memoryCopy, m_memorySize);
	}

	void LockMutex()
	{
		DWORD result = WaitForSingleObject(m_mutex, INFINITE);
	}

	void UnlockMutex()
	{
		ReleaseMutex(m_mutex);
	}



	bool IsInitialized()
	{
		return m_initialized;
	}

	bool WaitMutex()
	{
		return (m_mutex && WaitForSingleObject(m_mutex, 16) == WAIT_OBJECT_0);
	}

	void FreeMutex()
	{
		ReleaseMutex(m_mutex);
	}

};

