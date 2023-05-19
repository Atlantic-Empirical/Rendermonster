// Wrapper.h : Declaration of the CWrapper

#pragma once
#include "resource.h"       // main symbols

#include "MaxwellSDKWrapper_i.h"


#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

#define DllExport      __declspec( dllexport )

// CWrapper

class ATL_NO_VTABLE CWrapper :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CWrapper, &CLSID_Wrapper>,
	public IDispatchImpl<IWrapper, &IID_IWrapper, &LIBID_MaxwellSDKWrapperLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CWrapper()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_WRAPPER)


BEGIN_COM_MAP(CWrapper)
	COM_INTERFACE_ENTRY(IWrapper)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}

public:

	int DllExport GetCameraNames(LPCTSTR MXSPath, SAFEARRAY ** pCameraNames);
	int DllExport Test(LPCTSTR MXSPath, SAFEARRAY ** pOutStuff);
};

OBJECT_ENTRY_AUTO(__uuidof(Wrapper), CWrapper)
