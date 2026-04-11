#pragma once

#import "SilEncConverters22.tlb" raw_interfaces_only
using namespace SilEncConverters22;

class CIEncConverter : public CComPtr<IEncConverter>
{
public:
    CIEncConverter(IEncConverter* pIEC)
        : CComPtr<IEncConverter>(pIEC)
    {
    };

    CString Convert(const CString& strInput)
    {
        CComBSTR strOutput;
        p->Convert(strInput.AllocSysString(), &strOutput);
        return CString(strOutput);
    }
};
