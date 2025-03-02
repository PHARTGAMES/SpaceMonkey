#pragma once
#define _CRT_SECURE_NO_WARNINGS
#include <fstream>
#include <sstream>
#include <iostream>
#include "nlohmann/json.hpp"
using json = nlohmann::json;


template<typename T>
inline T* jsonHelperLoadFile(const char* a_basePath, const char* a_filename)
{
	try
	{
		T* output = nullptr;// new T();

		std::string filePath = std::string(a_basePath) + "/" + a_filename;


		FILE* file = std::fopen(filePath.c_str(), "rb");
		if (!file)
		{
			std::cerr << "Error opening file " << filePath << std::endl;
			return output;
		}

		// Get the length of the file
		std::fseek(file, 0, SEEK_END);
		long length = std::ftell(file);
		std::rewind(file);

		// Allocate memory for the char string and read the file contents into it
		char* buffer = new char[length + 1];
		std::fread(buffer, 1, length, file);
		buffer[length] = '\0';

		// Close the file and return the pointer to the char string
		std::fclose(file);

		json j = json::parse(buffer);
//		j.get_to(*output);

		output = j.get<T*>();

		delete[] buffer;

		return output;

	}
	catch (std::exception* e)
	{
		return nullptr;
	}
}


template<typename T>
inline bool jsonHelperSaveFile(T* a_config, const char* a_basePath, const char* a_filename)
{
	// Serialize the object to JSON
	nlohmann::json jsonConfig = *a_config;

	// Check if the base path exists, and create it if it does not
	if (!std::filesystem::exists(a_basePath))
	{
		std::filesystem::create_directory(a_basePath);
	}

	// Combine the base path and file name
	std::string filePath = std::string(a_basePath) + "/" + a_filename;

	// Open the output file stream
	std::ofstream outFile(filePath);
	if (outFile.fail())
	{
		return false;
	}

	// Write the JSON to the file
	outFile << jsonConfig.dump(4);

	// Close the file stream
	outFile.close();

	return true;
}


#define JsonGetOptional(a_json, a_keyName, a_target, a_defaultValue) \
if (a_json.contains(a_keyName)) { \
    a_json.at(a_keyName).get_to(a_target); \
}\


