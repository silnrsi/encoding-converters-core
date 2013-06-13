//
// example_shared.cpp
//
// Created by Jim Kornelsen in 2011.
//
// An example of how to call ECDriver from another application.
// The ecdriver.h header file must be included when compiling,
// and libecdriver.so must be included during linking.
//

#include <stdio.h>
#include "ecdriver.h"

int main(int argc, char **argv)
{
    if (IsEcInstalled())
    {
        char sConverterName[1000];
        bool bDirectionForward = true;
        int eNormFormOutput = 0;

        printf("Calling SelectConverter.\n");
        int err = EncConverterSelectConverter (
                  sConverterName, bDirectionForward, eNormFormOutput);
        if (err == 0) {
            printf("got %s\n", sConverterName);
        } else {
            fprintf(stderr, "SelectConverter() failed.\n");
            return 1;
        }
        char sDescription[1000];
        EncConverterConverterDescription(
            sConverterName, sDescription, 1000);
        printf("Description is %s.", sDescription);

        // input data is unicode bytes (UTF-8)
        //const char * sInput = "abCde";
        const char sInput[] = "abCde";
        char sOutput[1000];
        EncConverterConvertString (
            sConverterName, sInput, sOutput, 1000);
        // sOutput contains the unicode result (UTF-8)
        printf("Result is %s.", sOutput);
    }
    else
    {
        fprintf(stderr, "Cannot find EncConverters.\n");
        return 1;
    }
    return 0;
}
