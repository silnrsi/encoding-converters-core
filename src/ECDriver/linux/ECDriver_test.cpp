//
// ECDriver_test.cpp
//
// Test code to verify that the ECDriver works correctly.
// A converter will be added to the repository.
// Results will be given in STDERR, and other messages in STDOUT.
//
// Created by Jim Kornelsen on May 30 2013.
//
// 03-Jun-13 JDK  Automatically add converter.
//

#include <stdio.h>
#include <string.h>
#include "ecdriver.h"

const char convName_toUpper[] = "testECDriver_toUpper";

typedef bool (*funcTest_t)(void);

bool assertEqual(char * s1, char * s2, int len=-1)
{
    if (len == -1) {
        len = strlen(s2);
    }
    if (strncmp(s1, s2, len) == 0) {
        fprintf(stderr, "+");
        return true;
    } else {
        fprintf(stderr, "-");
        printf("'%s' != '%s'\n", s1, s2);
        return false;
    }
}
bool assertContains(char * s1, char * s2)
{
    if (strstr(s1, s2) == NULL) {
        fprintf(stderr, "-");
        printf("'%s' does not contain '%s'\n", s1, s2);
        return false;
    } else {
        fprintf(stderr, "+");
        return true;
    }
}

bool addTestConverter()
{
    const int Unicode_to_from_Unicode = 4;
    const int ICUTransliteration      = 0x0004;
    if (IsEcInstalled())
    {
        int err = EncConverterAddConverter (
            convName_toUpper,
            "Any-Upper",
            Unicode_to_from_Unicode,
            "",
            "",
            ICUTransliteration);
        return (err == 0);
    }
    printf("Cannot find ECDriver.\n");
    return false;
}

bool testCaps()
{
    if (IsEcInstalled())
    {
        const bool bDirectionForward = true;
        const int eNormFormOutput = 0;
        if (EncConverterInitializeConverter (
            convName_toUpper, bDirectionForward, eNormFormOutput) == 0)
        {
            char sDescription[1000];
            EncConverterConverterDescription(
                convName_toUpper, sDescription, 1000);
            //fprintf(stderr, "Description is %s.", sDescription);
            char sExpected[256];    // what we expect the results to contain
            strcpy(sExpected, "Name: '");
            strcat(sExpected, convName_toUpper);
            strcat(sExpected, "'");
            if (!assertContains(sDescription, sExpected))
                return false;

            // input data is unicode bytes (UTF-8)
            const char sInput[] = "abCde";
            char sOutput[1000];
            EncConverterConvertString (
                convName_toUpper, sInput, sOutput, 1000);
            // sOutput contains the unicode result (UTF-8)
            //fprintf(stderr, "Result is %s.", sOutput);
            if (!assertEqual(sOutput, (char *)"ABCDE")) return false;

            return true;
        }
        else
        {
            printf("Error initializing converter.\n");
            return false;
        }
    }
    printf("Cannot find ECDriver.\n");
    return false;
}

// This test needs to be checked manually.
void testAutoSelect(void)
{
    char sConverterName[1000];
    bool bDirectionForward = true;
    int eNormFormOutput = 0;

    fprintf(stderr, "Calling SelectConverter.\n");
    int err = EncConverterSelectConverter (
              sConverterName, bDirectionForward, eNormFormOutput);
    if (err == 0) {
        printf("ok\n");
        printf("got %s\n", sConverterName);
    } else {
        printf("not ok\n");
    }
}

// Repeat calls many times to verify there are not memory problems.
bool testRepeat()
{
    for (int repeat = 0; repeat < 5; repeat++)
    {
        if (IsEcInstalled())
        {
            char sConverterName[1000];
//#define CALL_AUTOSELECT   // uncomment to call EncConverters.AutoSelect()
#ifdef CALL_AUTOSELECT 
            bool bDirectionForward = true;
            int eNormFormOutput = 0;
            if (EncConverterSelectConverter (
                sConverterName, bDirectionForward, eNormFormOutput) == 0)
#else
            strcpy(sConverterName, convName_toUpper);
            const bool bDirectionForward = true;
            const int eNormFormOutput = 0;
            if (EncConverterInitializeConverter (
                sConverterName, bDirectionForward, eNormFormOutput) == 0)
#endif
            {
                for (int repeat = 0; repeat < 100; repeat++)
                {
                     // input data is unicode bytes (UTF-8)
                    const char sInput[] = "abCde";
                    char sOutput[1000];
                    EncConverterConvertString (
                        sConverterName, sInput, sOutput, 1000);
                    // sOutput contains the unicode result (UTF-8)
                    //fprintf(stderr, "Result is %s.", sOutput);
                    if (!assertEqual(sOutput, (char *)"ABCDE")) return false;

                    const char sInput2[] = "defgHi";
                    char sOutput2[1000];
                    EncConverterConvertString (
                        sConverterName, sInput2, sOutput2, 1000);
                    if (!assertEqual(sOutput2, (char *)"DEFGHI")) return false;
                }
            }
        }
        else
        {
            printf("Cannot find ECDriver.\n");
            return false;
        }
    }
    return true;
}

int main(int argc, char **argv)
{
    fprintf(stderr, "ECDriver_test.main()\n");
    if (!addTestConverter())
        return -1;
    int num_failed      = 0;
    const int NUM_TESTS = 2;
    funcTest_t testFunctions[NUM_TESTS];
    testFunctions[0] = &testCaps;
    testFunctions[1] = &testRepeat;
    for (int testnum = 0; testnum < NUM_TESTS; testnum++)
    {
        fprintf(stderr, "Running test %d... ", testnum + 1);
        funcTest_t func = testFunctions[testnum];
        bool ok = (*func)();
        if (ok) {
            fprintf(stderr, " ok\n");
        } else {
            fprintf(stderr, " FAILED\n");
            num_failed++;
        }
    }
    fprintf(stderr, "\n\n"); // stdout results may perhaps flush out after this.
    if (num_failed == 0) {
        printf("All %d tests passed.\n", NUM_TESTS);
    } else {
        printf("%d of %d tests failed.\n", num_failed, NUM_TESTS);
    }
    Cleanup();
    return 0;
}
