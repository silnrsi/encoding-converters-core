//
// An example of how to call ECDriver from another application.
// The code here is simpler than the dynamic approach.
// However the ecdriver.h header file must be included when compiling,
// and libecdriver.so must be included during linking.
//

#include <stdio.h>
#include "ecdriver.h"

int main(int argc, char **argv)
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
    return 0;
}
