#include "stdafx.h"
#include "CppLib.h"

/// <summary>
/// Left input channel sample
/// </summary>
int iChannel0LeftIn;

/// <summary>
/// Left output channel sample
/// </summary>
int iChannel0LeftOut;

/// <summary>
/// Right input channel sample
/// </summary>
int iChannel0RightIn;

/// <summary>
/// Right output channel sample
/// </summary>
int iChannel0RightOut;

/// <summary>
/// Function invoked from C# and passing left channel and right channel
/// sound samples arrays. Simulates the environment of the EZ-KIT device
/// by invoking Process_Data() function which can be used exactly as
/// Process_Data() function known from VisualDSP "Talkthrough" sample project.
/// ################ DO NOT MODIFY #################
/// </summary>
/// <param name="lChannel">The left channel data array.</param>
/// <param name="rChannel">The right channel data array.</param>
/// <param name="dataSize">Size of the data arrays.</param>
void ProcessCpp(short* lChannel, short* rChannel, int dataSize)
{
	for (int i = 0; i < dataSize; i++) 
	{
		iChannel0LeftIn = lChannel[i];
		iChannel0RightIn = rChannel[i];

		Process_Data();

		lChannel[i] = iChannel0LeftOut;
		rChannel[i] = iChannel0RightOut;
	}
}

void Process_Data(void)
{
	// TODO: Add your sound effect here
	EchoSample();
}

////////////////////////////////////////////////////////////////////////////
/////////////// BEGIN OF TEST ECHO EFFECT //////////////////////////////////
////////////////////////////////////////////////////////////////////////////
#define BUFFOR_SIZE 5000

int Buffor[BUFFOR_SIZE] = { 0 };
int Buffor_Pozycja = 0;

void EchoSample(void)
{
	int value = (iChannel0RightIn + iChannel0LeftIn) / 2;
	Buffor[Buffor_Pozycja] = value;

	iChannel0LeftOut = value / 2 + GetFromBuffer(1) / 4;
	iChannel0RightOut = iChannel0LeftOut;

	Buffor_Pozycja = (Buffor_Pozycja + 1) % BUFFOR_SIZE;
}

int GetFromBuffer(int index)
{
	return Buffor[(Buffor_Pozycja + index) % BUFFOR_SIZE];
}
////////////////////////////////////////////////////////////////////////////
/////////////// END OF TEST ECHO EFFECT ////////////////////////////////////
////////////////////////////////////////////////////////////////////////////