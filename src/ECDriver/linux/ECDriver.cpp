/**
 * ECDriver.cpp
 * 
 * ECDriver is a shared library of unmanaged code that embeds Mono to call
 * the C# EncConverters core, starting with the EncConverters class in
 * SilEncConverters40.dll.
 *
 * When building this file, be sure to link with mono-2.0
 *
 * Rewritten for Mono and Linux by Jim Kornelsen 29-Oct-2011.
 *
 * 25-Jan-2012 JDK  Specify MONO_PATH to find other assemblies.
 * 27-Mar-2012 JDK  Fixed bug: Maps require std::string to do string comparison.
 *
 */

#include "ecdriver.h"

#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <mono/metadata/mono-config.h>
#include <stdio.h>
#include <stdlib.h>
#include <string>       // for std::string
#include <cstring>      // for strcmp
#include <map>

//const char * ASSEMBLYFILE = "SilEncConverters40.dll";     // located in working directory
const char * ASSEMBLYFILE  = "/usr/lib/encConverters/SilEncConverters40.dll";
const char * ASSEMBLIESDIR = "MONO_PATH=/usr/lib/encConverters";  // look for other SEC assemblies here

bool loaded=false;  // true when methods have been loaded
MonoDomain * domain                  = NULL;
MonoClass  * ecsClass                = NULL;
MonoObject * ecsObj                  = NULL;
MonoMethod * methAutoSelect          = NULL;
MonoMethod * methMakeWindowGoAway    = NULL;
MonoMethod * methGetDirectionForward = NULL;
MonoMethod * methSetDirectionForward = NULL;
MonoMethod * methGetNormalizeOutput  = NULL;
MonoMethod * methSetNormalizeOutput  = NULL;
MonoMethod * methGetConverterName    = NULL;
MonoMethod * methGetMapByName        = NULL;
MonoMethod * methToString            = NULL;
MonoMethod * methConvert             = NULL;
std::map<std::string, MonoObject *> m_mapECs;  // a map of EC objects
void * noArgs[0];   // when no arguments need to be passed

// Embed Mono, then load C# EncConverter classes and methods.
void LoadClasses(void)
{
    if (loaded) return;
    fprintf(stderr, "ECDriver: Loading Mono and SEC classes.\n");

    putenv((char *)ASSEMBLIESDIR);
    domain = mono_jit_init(ASSEMBLYFILE);
    mono_config_parse (NULL);   // This prevents System.Drawing.GDIPlus from throwing an exception.
    MonoAssembly *assembly = mono_domain_assembly_open (domain, ASSEMBLYFILE);
    if (assembly) {
        fprintf(stderr, "ECDriver: Got assembly %s.\n", ASSEMBLYFILE);
    } else {
        fprintf(stderr, "ECDriver: Could not open assembly %s. Please verify the location.\n", ASSEMBLYFILE);
        return;
    }
    MonoImage* image = mono_assembly_get_image(assembly);
    fprintf(stderr, "ECDriver: Got image\n");

    // Load class from assembly

    ecsClass = mono_class_from_name (
               image, "SilEncConverters40", "EncConverters");
    if (ecsClass == NULL) {
        fprintf(stderr, "ECDriver: Could not get the class. Perhaps ECInterfaces.dll is missing.\n");
        return;
    }
    fprintf(stderr, "ECDriver: Got class handle\n");
    // Note: this line will crash if ECInterfaces.dll is not found.
    ecsObj = mono_object_new(domain, ecsClass);
    fprintf(stderr, "ECDriver: Calling EncConverters constructor.\n");
    mono_runtime_object_init(ecsObj);   // call SEC constructor
    fprintf(stderr, "ECDriver: Finished calling ECs constructor.\n");

    // Get method pointers from class

    void * iter = NULL;
    MonoMethod * m = NULL;
    while ((m = mono_class_get_methods (ecsClass, &iter))) {
        const char * methName = mono_method_get_name(m);
        //printf("    method %s\n", methName);
        if (strcmp (methName, "AutoSelect") == 0) {
            methAutoSelect = m;
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
    fprintf(stderr, "ECDriver: Getting EncConverter class methods.\n");
    while ((m = mono_class_get_methods (ecClass, &iter))) {
        const char * methName = mono_method_get_name(m);
        //printf("    method %s\n", methName);
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
    fprintf(stderr, "ECDriver: Got methods.\n");

    loaded = true;
}

void Cleanup(void)
{
    fprintf(stderr, "ECDriver: cleanup BEGIN\n");
    if (!loaded) return;
    loaded=false;
    /*
    // The following code causes a crash, so perhaps it's not necessary.
    mono_free(ecsObj);
    fprintf(stderr, "ECDriver: freed ecsObj\n");
    std::map<std::string, MonoObject *>::iterator i = m_mapECs.begin();
    for( ; i != m_mapECs.end(); ++i )
    {
        fprintf(stderr, "ECDriver: deleting %s\n", i->first.c_str());
        MonoObject* p = i->second;
        //delete p;
        mono_free(p);
    }
    fprintf(stderr, "ECDriver: cleaning up mono\n");
    */
    mono_jit_cleanup(domain);
    fprintf(stderr, "ECDriver: cleanup END\n");
}

bool IsEcInstalled(void)
{
    LoadClasses();
    return loaded;
}

void SetEncConverter(const char * sConverterName, MonoObject* pEC)
{
    if (m_mapECs.find(sConverterName) != m_mapECs.end())
    {
        MonoObject* p = m_mapECs[sConverterName];
        //delete p;     // I think this will crash
        //mono_free(p); // I think this will crash
        p = NULL;
    }
    m_mapECs[sConverterName] = pEC;
}

MonoObject* GetEncConverter(const char * sConverterName)
{
    if (m_mapECs.find(sConverterName) != m_mapECs.end())
    {
        fprintf(stderr, "ECDriver: Got converter %s.\n", sConverterName);
        return m_mapECs[sConverterName];
    }
    fprintf(stderr, "ECDriver: Couldn't get converter %s.\n", sConverterName);
    fprintf(stderr, "ECDriver: Available converters: ");
    std::map<std::string, MonoObject *>::iterator i = m_mapECs.begin();
    for( ; i != m_mapECs.end(); ++i )
    {
        fprintf(stderr, "'%s', ", i->first.c_str());
    }
    fprintf(stderr, "\n");
    return 0;
}

int EncConverterSelectConverter (
    char * sConverterName, bool & bDirectionForward, int & eNormOutputForm)
{
    LoadClasses();
    if (!loaded) return -1;

    int convType = 0;   // convType Unknown, defined in ECInterfaces.cs
    void * args1[1];
    args1[0] = &convType;
    MonoObject *ecObj = mono_runtime_invoke(
                         methAutoSelect, ecsObj, args1, NULL);
    mono_runtime_invoke(methMakeWindowGoAway, ecsObj, noArgs, NULL);
    if (ecObj == NULL) {
        fprintf(stderr, "ECDriver: Did not get converter.\n");
        return -1;
    }
    fprintf(stderr, "ECDriver: Got converter.\n");

    MonoString * mConverterName = (MonoString*) mono_runtime_invoke (
                                  methGetConverterName, ecObj, noArgs, NULL);
    fprintf(stderr, "ECDriver: invoked methGetConverterName.\n");
    char * sTemp = mono_string_to_utf8(mConverterName);
    strcpy(sConverterName, sTemp);
    mono_free(sTemp);
    fprintf (stderr, "ECDriver: Converter name is '%s'\n", sConverterName);

    MonoObject * mResult = mono_runtime_invoke (
                           methGetDirectionForward, ecObj, noArgs, NULL);
    fprintf(stderr, "ECDriver: invoked methGetDirectionForward.\n");
    bDirectionForward = *(bool*)mono_object_unbox(mResult);
    fprintf(stderr, "ECDriver: Value of bool is: %s\n",
            (bDirectionForward)?"true":"false");

    mResult = mono_runtime_invoke (
              methGetNormalizeOutput, ecObj, noArgs, NULL);
    fprintf(stderr, "ECDriver: invoked methGetNormalizeOutput.\n");
    eNormOutputForm = *(int*) mono_object_unbox(mResult);
    fprintf(stderr, "ECDriver: Value of int is: %d\n", eNormOutputForm);

    SetEncConverter((const char *)sConverterName, ecObj);

    return 0;
}

int EncConverterInitializeConverter(
    const char * sConverterName, bool bDirectionForward, int eNormOutputForm)
{
    LoadClasses();
    if (!loaded) return -1;

    MonoString * mConverterName = mono_string_new (domain, sConverterName);
    void * args1[1];
    args1[0] = mConverterName;
    fprintf(stderr, "ECDriver: Getting map..\n");
    MonoObject *ecObj = mono_runtime_invoke(
                        methGetMapByName, ecsObj, args1, NULL);
    if (ecObj == NULL) {
        fprintf(stderr, "ECDriver: Did not find map.\n");
        return /*NameNotFound*/ -7;
    }
    fprintf(stderr, "ECDriver: Got converter.\n");

    void * args2[1];
    args2[0] = &bDirectionForward;
    mono_runtime_invoke(methSetDirectionForward, ecObj, args2, NULL);
    fprintf(stderr, "ECDriver: invoked.\n");

    void * args3[1];
    args3[0] = &eNormOutputForm;
    mono_runtime_invoke(methSetNormalizeOutput, ecObj, args3, NULL);
    fprintf(stderr, "ECDriver: invoked.\n");

    SetEncConverter(sConverterName, ecObj);
    return 0;
}

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
    LoadClasses();
    if (!loaded) return -1;

    MonoObject* pEC = GetEncConverter(sConverterName);
    if (pEC == 0) {
        fprintf(stderr, "ECDriver: Getting map..\n");
        MonoString * mConverterName = mono_string_new (domain, sConverterName);
        void * args1[1];
        args1[0] = mConverterName;
        pEC = mono_runtime_invoke(methGetMapByName, ecsObj, args1, NULL);
        if (pEC == NULL) {
            fprintf(stderr, "ECDriver: Did not find map.\n");
            return /*NameNotFound*/ -7;
        }
        SetEncConverter(sConverterName, pEC);
    }

    MonoString * mInput = mono_string_new(domain, sInput);
    void * args2[1];
    args2[0] = mInput;
    fprintf(stderr, "ECDriver: Calling methConvert.\n");
    MonoObject * mResult = mono_runtime_invoke(methConvert, pEC, args2, NULL);
    fprintf(stderr, "ECDriver: Method invoked.\n");
    MonoString * mOutput = (MonoString*)mResult;
    strncpy(sOutput, mono_string_to_utf8(mOutput), nOutputLen);
    sOutput[nOutputLen - 1]= '\0';  // make sure the string is null-terminated
    fprintf(stderr, "ECDriver: Result is: %s\n", sOutput);

    return 0;
}

int EncConverterConverterDescription (
    const char * sConverterName, char * sDescription, int nDescriptionLen)
{
    LoadClasses();
    if (!loaded) return -1;

    MonoObject * pEC = GetEncConverter(sConverterName);
    if (pEC == 0)
        return /*NameNotFound*/ -7;

    MonoObject *mResult = mono_runtime_invoke(methToString, pEC, noArgs, NULL);
    fprintf(stderr, "ECDriver: invoked.\n");
    MonoString *mDescription = (MonoString*)mResult;
    strcpy(sDescription, mono_string_to_utf8(mDescription));
    fprintf(stderr, "ECDriver: Result is: %s\n", sDescription);

    return 0;
}

/*
// This code can be used for testing ECDriver. It is not used when
// building as a library.
int main()
{
    fprintf(stderr, "ECDriver: main() BEGIN\n");
    if (IsEcInstalled())
    {
        char sConverterName[1000];
        bool bDirectionForward = true;
        int eNormFormOutput = 0;
        if (EncConverterSelectConverter (
            sConverterName, bDirectionForward, eNormFormOutput) == 0)
        {
            char sDescription[1000];
            EncConverterConverterDescription(
                sConverterName, sDescription, 1000);
            fprintf(stderr, "ECDriver: Description is %s.", sDescription);

            // input data is unicode bytes (UTF-8)
            //const char * sInput = "abCde";
            const char sInput[] = "abCde";
            char sOutput[1000];
            EncConverterConvertString (
                sConverterName, sInput, sOutput, 1000);
            // sOutput contains the unicode result (UTF-8)
            fprintf(stderr, "ECDriver: Result is %s.", sOutput);
        }
    }
    //cleanup();
    fprintf(stderr, "ECDriver: main() END\n");
    return 0;
}
*/

