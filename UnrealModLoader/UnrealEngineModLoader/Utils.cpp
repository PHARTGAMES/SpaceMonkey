#include "Utils.h"
#include <algorithm>
#include <cctype>

bool Utils::CaseInsensitiveStringCompare(const std::string& str1, const std::string& str2)
{
    // Convert both strings to lowercase
    std::string lowerStr1 = str1;
    std::string lowerStr2 = str2;

    std::transform(lowerStr1.begin(), lowerStr1.end(), lowerStr1.begin(), ::tolower);
    std::transform(lowerStr2.begin(), lowerStr2.end(), lowerStr2.begin(), ::tolower);

    // Compare the lowercase strings
    return lowerStr1 == lowerStr2;
}