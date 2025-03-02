#pragma once
#include "UEVRGameConfig.h"
#include "string"
#include "GPSimple.h"


inline void from_json(const json& j, UEVRGameConfig*& config)
{
	if (j.is_null()) {
		config = nullptr;
		return;
	}

	auto type = (j.at("m_plugin_type").get<std::string>());

	if (type.compare("GPSimple") == 0)
	{
		auto* simple = new GPSimpleConfig();
		from_json(j, *simple);
		config = simple;
	}

}