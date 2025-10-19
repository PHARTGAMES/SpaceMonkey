#include "pch.h"
#include "SpaceMonkeyTelemetryAPIImpl.h"
#include <windows.h>
#include "CMCustomUDPData.h"


uint8_t* SpaceMonkeyTelemetryAPIImpl::GetPacket()
{
	return m_packet;
}

void SpaceMonkeyTelemetryAPIImpl::SetPacket(uint8_t* a_packet)
{
	if (m_packet != nullptr)
	{
		free(m_packet);
		m_ownsPacket = false;
	}

	m_packet = a_packet;
	if (m_sharedMem != nullptr)
	{
		m_sharedMem->m_memoryCopy = m_packet;
	}
}



CMCustomUDPData* SpaceMonkeyTelemetryAPIImpl::InitSendSharedMemory(const std::string& a_packetFormatPath)
{
	m_installDir = SpaceMonkeyTelemetryAPI::GetInstallDir();
	std::string packetFormatPath = a_packetFormatPath;
	if (packetFormatPath.empty())
	{
		packetFormatPath = "PacketFormats\\defaultPacketFormat.xml";
	}

	m_frameData.Init(m_installDir + packetFormatPath);
	m_packetSize = m_frameData.GetSize();
	m_packet = (uint8_t*)malloc(m_packetSize);

	if (m_sharedMem == NULL)
	{
		m_sharedMem = new SharedMemory(SPACEMONKEY_TELEMETRY_FILENAME, SPACEMONKEY_TELEMETRY_MUTEX, SharedMemType::SharedMem_Write, (void*)m_packet, m_packetSize);
	}
	return &m_frameData;
}

CMCustomUDPData* SpaceMonkeyTelemetryAPIImpl::InitRecieveSharedMemory(const std::string& a_packetFormatPath)
{
	m_installDir = SpaceMonkeyTelemetryAPI::GetInstallDir();
	std::string packetFormatPath = a_packetFormatPath;
	if (packetFormatPath.empty())
	{
		packetFormatPath = "PacketFormats\\defaultPacketFormat.xml";
	}
	m_frameData.Init(m_installDir + packetFormatPath);
	m_packetSize = m_frameData.GetSize();
	m_packet = (uint8_t*)malloc(m_packetSize);

	if (m_sharedMem == NULL)
	{
		m_sharedMem = new SharedMemory(SPACEMONKEY_TELEMETRY_FILENAME, SPACEMONKEY_TELEMETRY_MUTEX, SharedMemType::SharedMem_Read, (void*)m_packet, m_packetSize);
	}

	return &m_frameData;
}

void SpaceMonkeyTelemetryAPIImpl::SendFrame()
{
	if (m_sharedMem == nullptr)
		return;

	memcpy(m_packet, m_frameData.GetBytes().data(), m_packetSize);

	m_sharedMem->Write();
}

void SpaceMonkeyTelemetryAPIImpl::RecieveFrame()
{
	if (m_sharedMem == nullptr)
		return;

	m_sharedMem->Read();

	m_frameData.FromBytes(m_packet, m_packetSize);
}

void SpaceMonkeyTelemetryAPIImpl::Deinit()
{
	if (m_sharedMem != nullptr)
	{
		m_sharedMem->Destroy();
		delete m_sharedMem;
		m_sharedMem = nullptr;
	}

	if (m_ownsPacket && m_packet)
	{
		free(m_packet);
	}
}

static bool IsOs64Bit()
{
	SYSTEM_INFO si = {};
	GetNativeSystemInfo(&si);
	WORD arch = si.wProcessorArchitecture;
	return arch == PROCESSOR_ARCHITECTURE_AMD64 ||
		arch == PROCESSOR_ARCHITECTURE_ARM64 ||
		arch == PROCESSOR_ARCHITECTURE_IA64;
}

static std::string ReadInstallPath(REGSAM samDesired)
{
	HKEY hKey = nullptr;
	const char* subkey = "SOFTWARE\\PHARTGAMES\\SpaceMonkeyTP";
	const char* value = "install_path";

	LONG st = RegOpenKeyExA(HKEY_LOCAL_MACHINE, subkey, 0, samDesired, &hKey);
	if (st != ERROR_SUCCESS) return std::string();

	DWORD type = 0;
	DWORD cb = 0;
	st = RegQueryValueExA(hKey, value, nullptr, &type, nullptr, &cb);
	if (st != ERROR_SUCCESS || (type != REG_SZ && type != REG_EXPAND_SZ)) {
		RegCloseKey(hKey);
		return std::string();
	}

	std::vector<char> buf(cb + 1, 0);
	st = RegQueryValueExA(hKey, value, nullptr, &type, reinterpret_cast<LPBYTE>(buf.data()), &cb);
	RegCloseKey(hKey);
	if (st != ERROR_SUCCESS) return std::string();

	return std::string(buf.data()); // REG_SZ is NUL-terminated
}


std::string SpaceMonkeyTelemetryAPI::GetInstallDir()
{
	if (IsOs64Bit()) 
	{
		// Prefer the 64-bit view on a 64-bit OS; fall back to 32-bit view.
		std::string s = ReadInstallPath(KEY_READ | KEY_WOW64_64KEY);
		if (!s.empty()) 
			return s;
		return ReadInstallPath(KEY_READ | KEY_WOW64_32KEY);
	}
	else 
	{
		// 32-bit OS
		return ReadInstallPath(KEY_READ);
	}

}

#ifdef __cplusplus
extern "C" {
#endif

	// Factory function to create an instance of the API.
	SMT_API SpaceMonkeyTelemetryAPI* SpaceMonkeyTelemetryAPI_Create()
	{
		return new SpaceMonkeyTelemetryAPIImpl();
	}

	// Function to destroy an instance of the API.
	SMT_API void SpaceMonkeyTelemetryAPI_Destroy(SpaceMonkeyTelemetryAPI* instance)
	{
		delete instance;
	}

	// Wrapper function for InitSendSharedMemory.
	SMT_API uint8_t* SpaceMonkeyTelemetryAPI_InitSendSharedMemory(SpaceMonkeyTelemetryAPI* instance, const char* a_packetFormatPath)
	{
		if (!instance)
		{
			return nullptr;
		}

		instance->InitSendSharedMemory(std::string(a_packetFormatPath));

		return instance->GetPacket();
	}

	// Wrapper function for InitRecieveSharedMemory.
	SMT_API uint8_t* SpaceMonkeyTelemetryAPI_InitRecieveSharedMemory(SpaceMonkeyTelemetryAPI* instance, const char* a_packetFormatPath)
	{
		if (!instance)
		{
			return nullptr;
		}

		instance->InitRecieveSharedMemory(std::string(a_packetFormatPath));

		return instance->GetPacket();

	}

	// Wrapper function for SendFrame.
	SMT_API void SpaceMonkeyTelemetryAPI_SendFrame(SpaceMonkeyTelemetryAPI* instance)
	{
		if (instance)
		{
			instance->SendFrame();
		}
	}

	// Wrapper function for RecieveFrame.
	SMT_API void SpaceMonkeyTelemetryAPI_RecieveFrame(SpaceMonkeyTelemetryAPI* instance)
	{
		if (instance)
		{
			instance->RecieveFrame();
		}
	}

	// Wrapper function for Deinit.
	SMT_API void SpaceMonkeyTelemetryAPI_Deinit(SpaceMonkeyTelemetryAPI* instance)
	{
		if (instance)
		{
			instance->Deinit();
		}
	}

	SMT_API void SpaceMonkeyTelemetryAPI_SetPacket(SpaceMonkeyTelemetryAPI* instance, uint8_t* packet)
	{
		if (instance)
		{
			instance->SetPacket(packet);
		}
	}


#ifdef __cplusplus
}
#endif