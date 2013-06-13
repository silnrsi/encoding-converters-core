// GuesserEC.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include "GuesserEC.h"

#include <map>          // use a 'map' stl class to contain the guesser instances.
#include "CorGuess.h"

#ifdef _MANAGED
#pragma managed(push, off)
#endif

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
    return TRUE;
}

#ifdef _MANAGED
#pragma managed(pop)
#endif

using namespace std;
typedef map<LPVOID, Guesser*>  GuesserMap;
GuesserMap  m_mapGuessers;

Guesser*    GetGuesser(LPVOID lpThis)
{
    Guesser* pGuesser = NULL;
    GuesserMap::iterator iter = m_mapGuessers.find(lpThis);
    if (iter != m_mapGuessers.end())
        pGuesser = iter->second;
    return pGuesser;
}

int OpenCorpus( LPVOID lpThis )
{
    // create a new Guesser instance and keep track of it based on the name given (clients
    //  must insure lpThis is unique)
    m_mapGuessers[lpThis] = new Guesser();
	return 0;
}

GUESSEREC_API int ResetCorpus( LPVOID lpThis )
{
    Guesser* pGuesser = GetGuesser(lpThis);
    if (pGuesser != NULL)
	{
        pGuesser->ClearAll();
		m_mapGuessers.erase(lpThis);
        delete pGuesser;
    }
	return 0;
}

GUESSEREC_API int AddPairToCorpus
(
    LPVOID  lpThis,         // name of the corpus to use (in case of multiple instances)
    LPCTSTR	lpszSrcString,  // source word to be added to your corpus for guessing
    LPCTSTR	lpszTgtString   // target word to be added to your corpus for guessing
)
{
    Guesser* pGuesser = GetGuesser(lpThis);
    if (pGuesser == NULL)
    {
        OpenCorpus(lpThis);
        pGuesser = GetGuesser(lpThis);
        if (pGuesser == NULL)
            return -1;
    }
    
    pGuesser->AddCorrespondence(lpszSrcString, lpszTgtString);
	return 0;
}

GUESSEREC_API int MakeAGuess
(
    LPVOID  lpThis,
	LPCTSTR	lpszInputString, 
	LPTSTR	lpszOutputString, 
	int*	pnNumCharsOut
)
{
    Guesser* pGuesser = GetGuesser(lpThis);
    if (pGuesser != NULL)
    {
        if (pGuesser->bTargetGuess(lpszInputString, &lpszOutputString))
        {
        	*pnNumCharsOut = (int)_tcslen(lpszOutputString);
            return 0;
        }
    }

    // otherwise, just return the data unchanged
    *pnNumCharsOut = (int)_tcslen(lpszInputString);    
	memcpy(lpszOutputString, lpszInputString, *pnNumCharsOut * sizeof(TCHAR));
	return 0;
}

