// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the GUESSEREC_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// GUESSEREC_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef GUESSEREC_EXPORTS
#define GUESSEREC_API __declspec(dllexport)
#else
#define GUESSEREC_API __declspec(dllimport)
#endif

extern "C"
{
    // call this for each source/target pair to populate the corpus
    GUESSEREC_API int AddPairToCorpus
    (
        LPVOID  lpThis,         // identifier of the corpus (in case of multiple instances)
        LPCTSTR lpszSrcString,  // source word to be added to the corpus for guessing
        LPCTSTR lpszTgtString   // target word to be added to the corpus for guessing
    );
     
    // call this to make a guess of the target word given a source word
    GUESSEREC_API int MakeAGuess
    (
        LPVOID  lpThis,           // identifier of the corpus (in case of multiple instances)
        LPCTSTR lpszInputString,  // source word whose target word is to be guessed
        LPTSTR  lpszOutputString, // pointer to the output buffer to write result in guess
        int*    pnNumCharsOut     // (in) # of chars in the output buffer; (out) fill in with the number of chars in the result
    );
     
    // call this to free up the resources in the DLL (e.g. when closing the project)
    GUESSEREC_API int ResetCorpus( LPVOID lpThis );
}
