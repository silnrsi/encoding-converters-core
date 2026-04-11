#pragma once

LPCTSTR clpszSpellFixerProgramID = _T("SpellingFixer30.SpellingFixer");

#include "CIEncConverter.h" // for CIEncConverter

#import "SpellingFixer30.tlb" raw_interfaces_only
using namespace SpellingFixer30;

class CSpellFixerWord : public CComPtr<_SpellFixerWord>
{
public:
    CSpellFixerWord(_SpellFixerWord* pSFW)
        : CComPtr<_SpellFixerWord>(pSFW)
    {
    }

    CString Value()
    {
        CComBSTR strValue;
        p->get_Value(&strValue);
        return CString(strValue);
    }
};

class CCscProject : public CComPtr<_CscProject>
{
public:
    CCscProject() {};
	
    bool SelectProject()
    {
        CComPtr<_SpellingFixer> pSF;
        pSF.CoCreateInstance(clpszSpellFixerProgramID);

        bool bRet = false;
        if( !!pSF )
            bRet = (pSF->SelectProject(&p) == S_OK);
        return bRet;
    }

    bool AddWordToCheckList(const CString &strWord, bool bTrim, const CString &strContext, WordCheckResult& eResult)
    {
        CComVariant varContext(strContext);
        eResult = WordCheckResult::WordCheckResult_Unknown;
        return (p->AddWordToCheckList(strWord.AllocSysString(), (bTrim) ? VARIANT_TRUE : VARIANT_FALSE, varContext, &eResult) == S_OK);
    }
    
    CIEncConverter SpellFixerEncConverter()
    {
        IEncConverter* pIEC;
        p->get_SpellFixerEncConverter(&pIEC);
        CIEncConverter aIEC(pIEC);
        return aIEC;
    }

    void QueryForSpellingCorrectionsBulk()
    {
        p->QueryForSpellingCorrectionsBulk();
    }

    bool EditDictionary()
    {
        return (p->EditDictionary() == S_OK);
    }

    bool EditSpellingFixes()
    {
        return (p->EditSpellingFixes() == S_OK);
    }

    CSpellFixerWord QueryForSpellingCorrection(const CString& strWord, const CString& strContext, bool bTrim, SpellFixResult& sfr)
    {
        sfr = SpellingFixer30::SpellFixResult::SpellFixResult_Cancel;

        CComBSTR bstrWord(strWord);
        _SpellFixerWords* pSFWs = 0;
        if (p->GetAmbiguousWords(bstrWord, &pSFWs) == S_OK)
        {
            _SpellFixerWord* pSFW = 0;
            if (p->QueryForSpellingCorrection(bstrWord, pSFWs, strContext.AllocSysString(), (bTrim) ? VARIANT_TRUE : VARIANT_FALSE, &pSFW, &sfr) == S_OK)
            {
                CSpellFixerWord aSFW(pSFW);
                return aSFW;
            }
        }

        return NULL;
    }

    bool IsAvailable() { return !!p; }
};


