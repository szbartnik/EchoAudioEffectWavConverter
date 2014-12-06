#pragma once

#define API extern "C" __declspec(dllexport)

API void ProcessCpp(short* lChannel, short* rChannel, int dataSize);
