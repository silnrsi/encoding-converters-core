

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


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


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__


#ifndef __IcuEC_h__
#define __IcuEC_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IcuECTransliterator_FWD_DEFINED__
#define __IcuECTransliterator_FWD_DEFINED__

#ifdef __cplusplus
typedef class IcuECTransliterator IcuECTransliterator;
#else
typedef struct IcuECTransliterator IcuECTransliterator;
#endif /* __cplusplus */

#endif 	/* __IcuECTransliterator_FWD_DEFINED__ */


#ifndef __IcuECTransConfig_FWD_DEFINED__
#define __IcuECTransConfig_FWD_DEFINED__

#ifdef __cplusplus
typedef class IcuECTransConfig IcuECTransConfig;
#else
typedef struct IcuECTransConfig IcuECTransConfig;
#endif /* __cplusplus */

#endif 	/* __IcuECTransConfig_FWD_DEFINED__ */


#ifndef __IcuECConverter_FWD_DEFINED__
#define __IcuECConverter_FWD_DEFINED__

#ifdef __cplusplus
typedef class IcuECConverter IcuECConverter;
#else
typedef struct IcuECConverter IcuECConverter;
#endif /* __cplusplus */

#endif 	/* __IcuECConverter_FWD_DEFINED__ */


#ifndef __IcuECConvConfig_FWD_DEFINED__
#define __IcuECConvConfig_FWD_DEFINED__

#ifdef __cplusplus
typedef class IcuECConvConfig IcuECConvConfig;
#else
typedef struct IcuECConvConfig IcuECConvConfig;
#endif /* __cplusplus */

#endif 	/* __IcuECConvConfig_FWD_DEFINED__ */


#ifndef __IcuECRegex_FWD_DEFINED__
#define __IcuECRegex_FWD_DEFINED__

#ifdef __cplusplus
typedef class IcuECRegex IcuECRegex;
#else
typedef struct IcuECRegex IcuECRegex;
#endif /* __cplusplus */

#endif 	/* __IcuECRegex_FWD_DEFINED__ */


#ifndef __IcuECRegexConfig_FWD_DEFINED__
#define __IcuECRegexConfig_FWD_DEFINED__

#ifdef __cplusplus
typedef class IcuECRegexConfig IcuECRegexConfig;
#else
typedef struct IcuECRegexConfig IcuECRegexConfig;
#endif /* __cplusplus */

#endif 	/* __IcuECRegexConfig_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 



#ifndef __IcuECLib_LIBRARY_DEFINED__
#define __IcuECLib_LIBRARY_DEFINED__

/* library IcuECLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_IcuECLib;

EXTERN_C const CLSID CLSID_IcuECTransliterator;

#ifdef __cplusplus

class DECLSPEC_UUID("0BEB0A3E-1AEC-45CB-8A64-12AEEDD8DFF7")
IcuECTransliterator;
#endif

EXTERN_C const CLSID CLSID_IcuECTransConfig;

#ifdef __cplusplus

class DECLSPEC_UUID("CB989D4B-EDED-4F76-9BD0-4775236D6A69")
IcuECTransConfig;
#endif

EXTERN_C const CLSID CLSID_IcuECConverter;

#ifdef __cplusplus

class DECLSPEC_UUID("284DC09D-DD56-48DE-8E1E-98646C8D0163")
IcuECConverter;
#endif

EXTERN_C const CLSID CLSID_IcuECConvConfig;

#ifdef __cplusplus

class DECLSPEC_UUID("2AF35540-247E-464A-BA74-E64327C0EE43")
IcuECConvConfig;
#endif

EXTERN_C const CLSID CLSID_IcuECRegex;

#ifdef __cplusplus

class DECLSPEC_UUID("D8D1254E-F585-424D-9B46-7E050A1F097C")
IcuECRegex;
#endif

EXTERN_C const CLSID CLSID_IcuECRegexConfig;

#ifdef __cplusplus

class DECLSPEC_UUID("CE2FD02D-257F-4E43-AA31-068023A0F79F")
IcuECRegexConfig;
#endif
#endif /* __IcuECLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


