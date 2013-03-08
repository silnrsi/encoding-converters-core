

/* this ALWAYS GENERATED file contains the IIDs and CLSIDs */

/* link this file in with the server and any clients */


 /* File created by MIDL compiler version 7.00.0555 */
/* at Tue Jan 29 16:50:08 2013
 */
/* Compiler settings for IcuEC.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 7.00.0555 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


#ifdef __cplusplus
extern "C"{
#endif 


#include <rpc.h>
#include <rpcndr.h>

#ifdef _MIDL_USE_GUIDDEF_

#ifndef INITGUID
#define INITGUID
#include <guiddef.h>
#undef INITGUID
#else
#include <guiddef.h>
#endif

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        DEFINE_GUID(name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8)

#else // !_MIDL_USE_GUIDDEF_

#ifndef __IID_DEFINED__
#define __IID_DEFINED__

typedef struct _IID
{
    unsigned long x;
    unsigned short s1;
    unsigned short s2;
    unsigned char  c[8];
} IID;

#endif // __IID_DEFINED__

#ifndef CLSID_DEFINED
#define CLSID_DEFINED
typedef IID CLSID;
#endif // CLSID_DEFINED

#define MIDL_DEFINE_GUID(type,name,l,w1,w2,b1,b2,b3,b4,b5,b6,b7,b8) \
        const type name = {l,w1,w2,{b1,b2,b3,b4,b5,b6,b7,b8}}

#endif !_MIDL_USE_GUIDDEF_

MIDL_DEFINE_GUID(IID, LIBID_IcuECLib,0x1E74D903,0xAD86,0x43F8,0xB3,0x9F,0x96,0x65,0x3D,0x7E,0x17,0x0D);


MIDL_DEFINE_GUID(CLSID, CLSID_IcuECTransliterator,0x0BEB0A3E,0x1AEC,0x45CB,0x8A,0x64,0x12,0xAE,0xED,0xD8,0xDF,0xF7);


MIDL_DEFINE_GUID(CLSID, CLSID_IcuECTransConfig,0xCB989D4B,0xEDED,0x4F76,0x9B,0xD0,0x47,0x75,0x23,0x6D,0x6A,0x69);


MIDL_DEFINE_GUID(CLSID, CLSID_IcuECConverter,0x284DC09D,0xDD56,0x48DE,0x8E,0x1E,0x98,0x64,0x6C,0x8D,0x01,0x63);


MIDL_DEFINE_GUID(CLSID, CLSID_IcuECConvConfig,0x2AF35540,0x247E,0x464A,0xBA,0x74,0xE6,0x43,0x27,0xC0,0xEE,0x43);


MIDL_DEFINE_GUID(CLSID, CLSID_IcuECRegex,0xD8D1254E,0xF585,0x424D,0x9B,0x46,0x7E,0x05,0x0A,0x1F,0x09,0x7C);


MIDL_DEFINE_GUID(CLSID, CLSID_IcuECRegexConfig,0xCE2FD02D,0x257F,0x4E43,0xAA,0x31,0x06,0x80,0x23,0xA0,0xF7,0x9F);

#undef MIDL_DEFINE_GUID

#ifdef __cplusplus
}
#endif



