/**
 * ECDriver.cpp
 * 
 * ECDriver is a library of unmanaged code that embeds Mono to call
 * the C# EncConverters core, starting with the EncConverters class in
 * SilEncConverters40.dll.
 * It implements nearly the same interface as the Windows ECDriver,
 * but uses only narrow UTF8 streams, not wide ("W") UTF16 streams.
 *
 * When building this file, be sure to link with mono-2.0
 * Also define LIBDIR, for example -DLIBDIR=/usr/lib/encConverters.
 *
 * Created by Jim Kornelsen on 29-Oct-2011.
 *
 * 25-Jan-2012 JDK  Specify MONO_PATH to find other assemblies.
 * 27-Mar-2012 JDK  Fixed bug: Maps require std::string to do string comparison.
 * 01-Jun-2013 JDK  Fixed crash: Don't call methGetMapByName from InitConv().
 * 03-Jun-2013 JDK  Added EncConverterAddConverter().
 * 06-Jun-2013 JDK  Fail gracefully if a C# exception occurs.
 * 10-Jun-2013 JDK  Display C# exceptions in an alert window.
 */

#include "ecdriver.h"

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/mono-config.h>
#include <stdio.h>
#include <stdlib.h>
#include <string>       // std::string
#include <cstring>      // strcmp
#include <unistd.h>     // execl
#include <sys/wait.h>
#include <map>

// Path to EncConverters assembly, based on LIBDIR defined by compiler flag.
#define STRINGIFY(x) #x             // turn x into "x"
#define TOSTRING(x) STRINGIFY(x)    // expand macro x into its value
const char * ASSEMBLYFILE  = TOSTRING(LIBDIR) "/SilEncConverters40.dll";
const char * MONO_PATH = "MONO_PATH=" TOSTRING(LIBDIR);
const char * MONO_REG  = "MONO_REGISTRY_PATH=" TOSTRING(REGROOT) "/registry";

// Uncomment the following line for verbose debugging output.
#define VERBOSE_DEBUGGING

bool loaded = false;  // true when methods have been loaded
MonoDomain * domain                  = NULL;
MonoClass  * ecsClass                = NULL;
MonoObject * ecsObj                  = NULL;
MonoMethod * methAddConv             = NULL;
MonoMethod * methAutoSelect          = NULL;
MonoMethod * methConvert             = NULL;
MonoMethod * methGetConverterName    = NULL;
MonoMethod * methGetDirectionForward = NULL;
MonoMethod * methSetDirectionForward = NULL;
MonoMethod * methGetMapByName        = NULL;
MonoMethod * methGetNormalizeOutput  = NULL;
MonoMethod * methSetNormalizeOutput  = NULL;
MonoMethod * methMakeWindowGoAway    = NULL;
MonoMethod * methToString            = NULL;
std::map<std::string, MonoObject *> mapECs;  // a map of EC objects
void * noArgs[0];   // when no arguments need to be passed

//******************************************************************************
/**
 * Show a message box like the Windows MessageBox call.
 */
void ShowAlert(char * sMessage)
{
    fprintf(stderr, "%s\n", sMessage);  // Print on command line as well.

    // Zenity chokes on seeming markup, so replace markup characters with '?'
    for (int i = 0; sMessage[i] != '\0'; i++)
    {
        if (sMessage[i] == '<' || sMessage[i] == '>' || sMessage[i] == '&')
            sMessage[i] = '?';
    }

    int pID = fork();
    if (pID == 0)   // child
    {
        execl("/usr/bin/zenity", "/usr/bin/zenity", "--error", "--title",
              "Caught exception", "--text", sMessage, (char *) NULL);
        fprintf(stderr, "ECDriver: error executing zenity\n");
        exit(1);
    }
    else if (pID < 0)
    {
        fprintf(stderr, "ECDriver: failed to fork\n");
    }
    else    // parent
    {
        fprintf(stderr, "ECDriver: Waiting for alert window to close...\n");
        wait(NULL);
        fprintf(stderr, "ECDriver: Window has been closed.\n");
    }
}

//******************************************************************************
void DisplayException(MonoObject * mException)
{
    MonoObject * other_exc = NULL;
    MonoString * sException = mono_object_to_string (
                              mException, &other_exc);
    char * sTemp = mono_string_to_utf8(sException);
    ShowAlert(sTemp);
    mono_free(sTemp);
}

//******************************************************************************
/**
 * Embed Mono, then load C# EncConverter classes and methods.
 */
void LoadClasses(void)
{
    if (loaded) return;
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Loading Mono and SEC classes.\n");
    fprintf(stderr, "ECDriver: %s.\n", MONO_PATH);
    fprintf(stderr, "ECDriver: %s.\n", MONO_REG);
#endif
    putenv((char *)MONO_PATH);
    putenv((char *)MONO_REG);
    domain = mono_jit_init(ASSEMBLYFILE);
    mono_config_parse(NULL);  // prevents System.Drawing.GDIPlus exception
    MonoAssembly *assembly = mono_domain_assembly_open (domain, ASSEMBLYFILE);
    if (assembly) {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "ECDriver: Got assembly %s.\n", ASSEMBLYFILE);
#endif
    } else {
        fprintf(stderr, "ECDriver: Could not open assembly %s. Please verify the location.\n", ASSEMBLYFILE);
        return;
    }
    MonoImage* image = mono_assembly_get_image(assembly);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Got image\n");
#endif

    // Load class from assembly

    ecsClass = mono_class_from_name (
               image, "SilEncConverters40", "EncConverters");
    if (ecsClass == NULL) {
        fprintf(stderr, "ECDriver: Could not get the class. Perhaps ECInterfaces.dll is missing.\n");
        return;
    }
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Got class handle\n");
#endif
    // This line will crash if ECInterfaces.dll is not found.
    ecsObj = mono_object_new(domain, ecsClass);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Calling EncConverters constructor.\n");
#endif
    mono_runtime_object_init(ecsObj);   // call SEC constructor
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Finished calling ECs constructor.\n");
#endif

    // Get method pointers from class

    void * iter = NULL;
    MonoMethod * m = NULL;
    while ((m = mono_class_get_methods (ecsClass, &iter))) {
        const char * methName = mono_method_get_name(m);
#ifdef VERBOSE_DEBUGGING
        //fprintf(stderr, "    method %s\n", methName);
#endif
        if (strcmp (methName, "AutoSelect") == 0) {
            methAutoSelect = m;
        } else if (strcmp (methName, "Add") == 0) {
            methAddConv = m;
        } else if (strcmp (methName, "MakeSureTheWindowGoesAway") == 0) {
            methMakeWindowGoAway = m;
        } else if (strcmp (methName, "get_Item") == 0) {
            // In EncConverters.cs, the declaration is:
            // public new IEncConverter this[object mapName]
            methGetMapByName = m;
        }
    }
    if (methAutoSelect == NULL) {
        fprintf(stderr, "ECDriver: Error! Could not get method(s).\n");
        return;
    }

    MonoClass * ecClass = mono_class_from_name(
                          image, "SilEncConverters40", "EncConverter");
    iter = NULL;
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Getting EncConverter class methods.\n");
#endif
    while ((m = mono_class_get_methods (ecClass, &iter))) {
        const char * methName = mono_method_get_name(m);
#ifdef VERBOSE_DEBUGGING
        //fprintf(stderr, "    method %s\n", methName);
#endif
        if (strcmp (methName, "get_Name") == 0) {
            methGetConverterName = m;
        } else if (strcmp (methName, "get_DirectionForward") == 0) {
            methGetDirectionForward = m;
        } else if (strcmp (methName, "set_DirectionForward") == 0) {
            methSetDirectionForward = m;
        } else if (strcmp (methName, "get_NormalizeOutput") == 0) {
            methGetNormalizeOutput = m;
        } else if (strcmp (methName, "set_NormalizeOutput") == 0) {
            methSetNormalizeOutput = m;
        } else if (strcmp (methName, "ToString") == 0) {
            methToString = m;
        } else if (strcmp (methName, "Convert") == 0) {
            methConvert = m;
        }
    }
    if (methConvert == NULL) {
        fprintf(stderr, "ECDriver: Error! Could not get method(s).\n");
        return;
    }
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Got methods.\n");
#endif

    loaded = true;
}

//******************************************************************************
void Cleanup(void)
{
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: cleanup BEGIN\n");
#endif
    if (!loaded) return;
    loaded=false;
    mono_jit_cleanup(domain);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: cleanup END\n");
#endif
}

//******************************************************************************
bool IsEcInstalled(void)
{
    LoadClasses();
    return loaded;
}

//******************************************************************************
/**
 * pEC should be the address of a pointer which will be set to result object.
 * returns 0 for success, -1 for failure, -6 for exception
 * This is an internal method, not to be called from other code.
 */
int GetEncConverter(const char * sConverterName, MonoObject ** pEC)
{
    if (mapECs.find(sConverterName) != mapECs.end())
    {
#ifdef VERBOSE_DEBUGGING
        fprintf(stderr, "ECDriver: Got converter %s.\n", sConverterName);
#endif
        *pEC = mapECs[sConverterName];
        return 0;
    }
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Couldn't get converter %s.\n", sConverterName);
    fprintf(stderr, "ECDriver: Available converters: ");
    std::map<std::string, MonoObject *>::iterator i = mapECs.begin();
    for( ; i != mapECs.end(); ++i )
    {
        fprintf(stderr, "'%s', ", i->first.c_str());
    }
    fprintf(stderr, "\n");
    fprintf(stderr, "ECDriver: Getting map..\n");
#endif
    MonoString * mConverterName = mono_string_new (domain, sConverterName);
    void * args1[1];
    args1[0] = mConverterName;
    MonoObject * mException = NULL;
    *pEC = mono_runtime_invoke(methGetMapByName, ecsObj, args1, &mException);
    if (mException != NULL) {
        fprintf(stderr, "ECDriver: An exception was thrown getting map.\n");
        mono_runtime_invoke(methMakeWindowGoAway, ecsObj, noArgs, NULL);
        DisplayException(mException);
        return /*ErrStatus.Exception*/ -6;
    }
    if (*pEC != NULL)
    {
        mapECs[sConverterName] = *pEC;
        return 0;
    }
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Did not find map.\n");
#endif
    return -1;
}

//******************************************************************************
int EncConverterSelectConverter (
    char * sConverterName, bool & bDirectionForward, int & eNormOutputForm)
{
    LoadClasses();
    if (!loaded) return -1;

    int convType = 0;   // convType Unknown, defined in ECInterfaces.cs
    void * args1[1];
    args1[0] = &convType;
    MonoObject * mException = NULL;
    MonoObject *pEC = mono_runtime_invoke(
                         methAutoSelect, ecsObj, args1, &mException);
    mono_runtime_invoke(methMakeWindowGoAway, ecsObj, noArgs, NULL);
    if (mException != NULL) {
        fprintf(stderr,
                "ECDriver: An exception was thrown during selection.\n");
        DisplayException(mException);
        return /*ErrStatus.Exception*/ -6;
    }
    if (pEC == NULL) {
        fprintf(stderr, "ECDriver: Did not get converter.\n");
        return -1;
    }
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Got converter.\n");
#endif

    MonoString * mConverterName = (MonoString*) mono_runtime_invoke (
                                  methGetConverterName, pEC, noArgs, NULL);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: invoked methGetConverterName.\n");
#endif
    char * sTemp = mono_string_to_utf8(mConverterName);
    strcpy(sConverterName, sTemp);
    mono_free(sTemp);
#ifdef VERBOSE_DEBUGGING
    fprintf (stderr, "ECDriver: Converter name is '%s'\n", sConverterName);
#endif

    MonoObject * mResult = mono_runtime_invoke (
                           methGetDirectionForward, pEC, noArgs, NULL);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: invoked methGetDirectionForward.\n");
#endif
    bDirectionForward = *(bool*)mono_object_unbox(mResult);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Value of bool is: %s\n",
            (bDirectionForward)?"true":"false");
#endif

    mResult = mono_runtime_invoke (
              methGetNormalizeOutput, pEC, noArgs, NULL);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: invoked methGetNormalizeOutput.\n");
#endif
    eNormOutputForm = *(int*) mono_object_unbox(mResult);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Value of int is: %d\n", eNormOutputForm);
#endif

    mapECs[sConverterName] = pEC;

    return 0;
}

//******************************************************************************
int EncConverterInitializeConverter(
    const char * sConverterName, bool bDirectionForward, int eNormOutputForm)
{
    LoadClasses();
    if (!loaded) return -1;

    MonoObject * pEC = NULL;
    int err = GetEncConverter(sConverterName, &pEC);
    if (err == -6)
        return /*ErrStatus.Exception*/ -6;
    else if (err == -1 || pEC == 0)
        return /*ErrStatus.NameNotFound*/ -7;
 
    void * args2[1];
    args2[0] = &bDirectionForward;
    mono_runtime_invoke(methSetDirectionForward, pEC, args2, NULL);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: invoked methSetDirectionForward.\n");
#endif

    void * args3[1];
    args3[0] = &eNormOutputForm;
    mono_runtime_invoke(methSetNormalizeOutput, pEC, args3, NULL);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: invoked methSetNormalizeOutput.\n");
#endif

    return 0;
}

//******************************************************************************
int EncConverterAddConverter(
    const char * sConverterName,
    const char * sConverterSpec,
    int          conversionType,
    const char * sLeftEncoding,
    const char * sRightEncoding,
    int          processType)
{
    LoadClasses();
    if (!loaded) return -1;

    MonoObject * pEC = NULL;
    int err = GetEncConverter(sConverterName, &pEC);
    if (err == -6)
        return /*ErrStatus.Exception*/ -6;
    if (pEC != 0)
    {
        fprintf(stderr, "ECDriver: converter %s is already available.\n",
                sConverterName);
        return 0;
    }

    MonoString * mConverterName = mono_string_new(domain, sConverterName);
    MonoString * mConverterSpec = mono_string_new(domain, sConverterSpec);
    MonoString * mLeftEncoding  = mono_string_new(domain, sLeftEncoding);
    MonoString * mRightEncoding = mono_string_new(domain, sRightEncoding);
    void * args1[6];
    args1[0] = mConverterName;
    args1[1] = mConverterSpec;
    args1[2] = &conversionType;
    args1[3] = mLeftEncoding;
    args1[4] = mRightEncoding;
    args1[5] = &processType;
    MonoObject * mException = NULL;
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Calling methAddConv.\n");
#endif
    mono_runtime_invoke (methAddConv, ecsObj, args1, &mException);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Invoked methAddConv.\n");
#endif

    if (mException != NULL) {
        fprintf(stderr,
                "ECDriver: An exception was thrown while adding converter.\n");
        mono_runtime_invoke(methMakeWindowGoAway, ecsObj, noArgs, NULL);
        DisplayException(mException);
        return /*ErrStatus.Exception*/ -6;
    } 
    err = GetEncConverter(sConverterName, &pEC);
    if (err == -6)
        return /*ErrStatus.Exception*/ -6;
    else if (err == -1 || pEC == 0)
    {
        fprintf(stderr, "ECDriver: failed to add converter %s.\n",
                sConverterName);
        return -1;
    }
    return 0;
}

//******************************************************************************
/*
 When calling this function, provide a writable buffer for sOutput.
 If the result is bigger than nOutputLen, the result will be truncated.
 */
int EncConverterConvertString (
    const char * sConverterName,
    const char * sInput,
    char *       sOutput,
    int          nOutputLen)
{
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: EncConverterConvertString BEGIN.\n");
#endif
    LoadClasses();
    if (!loaded) return -1;

    MonoObject * pEC = NULL;
    int err = GetEncConverter(sConverterName, &pEC);
    if (err == -6)
        return /*ErrStatus.Exception*/ -6;
    else if (err == -1 || pEC == 0)
        return /*ErrStatus.NameNotFound*/ -7;
 
    MonoString * mInput = mono_string_new(domain, sInput);
    void * args1[1];
    args1[0] = mInput;
    MonoObject * mException = NULL;
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Calling methConvert.\n");
#endif
    MonoString * mOutput = (MonoString *) mono_runtime_invoke (
                           methConvert, pEC, args1, &mException);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: Method invoked.\n");
#endif
    if (mException != NULL) {
        fprintf(stderr,
                "ECDriver: An exception was thrown during conversion.\n");
        mono_runtime_invoke(methMakeWindowGoAway, ecsObj, noArgs, NULL);
        DisplayException(mException);
        return /*ErrStatus.Exception*/ -6;
    }
    char * sTemp = mono_string_to_utf8(mOutput);
    strncpy(sOutput, sTemp, nOutputLen);
    sOutput[nOutputLen - 1] = '\0'; // make sure string is null-terminated
    mono_free(sTemp);
#ifdef VERBOSE_DEBUGGING
    //fprintf(stderr, "ECDriver: Result is: %s\n", sOutput);
    fprintf(stderr, "ECDriver: Got result.\n");
#endif
    return 0;
}

//******************************************************************************
int EncConverterConverterDescription (
    const char * sConverterName, char * sDescription, int nDescriptionLen)
{
    LoadClasses();
    if (!loaded) return -1;

    MonoObject * pEC = NULL;
    int err = GetEncConverter(sConverterName, &pEC);
    if (err == -6)
        return /*ErrStatus.Exception*/ -6;
    else if (err == -1 || pEC == 0)
        return /*ErrStatus.NameNotFound*/ -7;

    MonoString * mDescription = (MonoString *) mono_runtime_invoke (
                                methToString, pEC, noArgs, NULL);
#ifdef VERBOSE_DEBUGGING
    fprintf(stderr, "ECDriver: invoked methToString.\n");
#endif
    char * sTemp = mono_string_to_utf8(mDescription);
    strncpy(sDescription, sTemp, nDescriptionLen);
    sDescription[nDescriptionLen - 1] = '\0'; // make sure string is null-terminated
    mono_free(sTemp);
#ifdef VERBOSE_DEBUGGING
    //fprintf(stderr, "ECDriver: Result is: %s\n", sDescription);
    fprintf(stderr, "ECDriver: got description.\n");
#endif

    return 0;
}
