//
// An example of how to call ECDriver dynamically from another application.
// The advantage of linking dynamically is that the application does not
// have to be compiled or linked with ECDriver files when building.
//
// To run, set LD_LIBRARY_PATH to the location of libecdriver.so,
// for example, export LD_LIBRARY_PATH=.

#include <dlfcn.h>
#include <stdio.h>

typedef int  (*funcSelectConverter_t)(char *, bool &, int &);
typedef void (*funcCleanup_t)(void);

int main(int argc, char **argv)
{
    funcSelectConverter_t funcSelectConverter;
    funcCleanup_t         funcCleanup;

    void* handle = dlopen("libecdriver.so.1", RTLD_LAZY);
    //void* handle = dlopen("./libecdriver.so", RTLD_NOW);  // better for debugging
    if (!handle) {
        fputs (dlerror(), stderr);
        return(-1);
    }
    fprintf(stderr, "Got library handle.\n");
    funcSelectConverter = (funcSelectConverter_t)
                          dlsym(handle, "EncConverterSelectConverter");
    funcCleanup         = (funcCleanup_t)
                          dlsym(handle, "Cleanup");

    char sConverterName[1000];
    bool bDirectionForward = true;
    int eNormFormOutput = 0;
    fprintf(stderr, "Calling EncConverterSelectConverter.\n");
    int err = (*funcSelectConverter)
              (sConverterName, bDirectionForward, eNormFormOutput);
    if (err == 0) {
        printf("ok\n");
        printf("got %s\n", sConverterName);
    } else {
        printf("not ok\n");
    }
    getchar();
    (*funcCleanup)();
    fprintf(stderr, "main() finished.\n");
    getchar();
    return 0;
}
