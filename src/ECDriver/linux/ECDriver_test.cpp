//
// ECDriver_test.cpp
//
// Test code to verify that the ECDriver works correctly.
//
// Created by Jim Kornelsen on May 30 2013.
//

#include <stdio.h>
#include <string.h>
#include "ecdriver.h"

typedef bool (*funcTest_t)(void);

bool assertEqual(char * s1, char * s2, int len=-1)
{
    if (len == -1) {
        len = strlen(s2);
    }
    if (strncmp(s1, s2, len) == 0) {
        printf("+");
        return true;
    } else {
        printf("-");
        return false;
    }
}
bool assertIn(char * s1, char * s2)
{
    if (strstr(s1, s2) == NULL) {
        printf("-");
        return false;
    } else {
        printf("+");
        return true;
    }
}


bool test1()
{
    fprintf(stderr, "test1() BEGIN\n");
    if (IsEcInstalled())
    {
        char sConverterName[] = "capsTest";
        bool bDirectionForward = true;
        int eNormFormOutput = 0;
        if (EncConverterInitializeConverter (
            sConverterName, bDirectionForward, eNormFormOutput) == 0)
        {
            char sDescription[1000];
            EncConverterConverterDescription(
                sConverterName, sDescription, 1000);
            //fprintf(stderr, "Description is %s.", sDescription);
            if (!assertIn(sDescription, (char *)"Name: 'capsTest'"))
                return false;

            // input data is unicode bytes (UTF-8)
            //const char * sInput = "abCde";
            const char sInput[] = "abCde";
            char sOutput[1000];
            EncConverterConvertString (
                sConverterName, sInput, sOutput, 1000);
            // sOutput contains the unicode result (UTF-8)
            //fprintf(stderr, "Result is %s.", sOutput);
            if (!assertEqual(sOutput, (char *)"ABCDE")) return false;
            return true;
        }
    }
    return false;
}

/*
void test2(void)
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
*/

int main(int argc, char **argv)
{
    for (int repeat = 0; repeat < 100; repeat++)
        // TODO: Make this loop into an actual test.
        //
        // It works if we uncomment this line!
        // Clearly we are dealing with a memory problem.
        //fprintf(stderr, "Repeat %d\n", repeat);
        test1();
    printf("Done!\n");
    return 0;
    int num_failed      = 0;
    const int NUM_TESTS = 1;
    funcTest_t testFunctions[NUM_TESTS];
    testFunctions[0] = &test1;
    for (int testnum = 0; testnum < NUM_TESTS; testnum++)
    {
        fprintf(stderr, "Running test %d... ", testnum);
        funcTest_t func = testFunctions[testnum];
        bool ok = (*func)();
        if (ok) {
            fprintf(stderr, " ok\n");
        } else {
            fprintf(stderr, " FAILED\n");
            num_failed++;
        }
    }
    if (num_failed == 0) {
        printf("\nAll %d tests passed.\n", NUM_TESTS);
    } else {
        printf("\n%d of %d tests failed.\n", num_failed, NUM_TESTS);
    }
    return 0;
}
