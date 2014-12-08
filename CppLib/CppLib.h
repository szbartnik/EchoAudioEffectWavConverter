#pragma once

#define API extern "C" __declspec(dllexport)

void Process_Data(void);
void EchoSample(void);
int GetFromBuffer(int index);
API void ProcessCpp(short* lChannel, short* rChannel, int dataSize);
