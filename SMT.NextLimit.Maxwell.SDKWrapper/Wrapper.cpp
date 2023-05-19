//http://weblogs.asp.net/kennykerr/archive/2005/06/20/414010.aspx
//http://www.experts-exchange.com/Programming/Languages/CPP/Q_20025284.html
//http://www.google.com/search?hl=en&rls=com.microsoft%3A*&q=SafeArrayPutElement+E_OUTOFMEMORY
//http://www.geocities.com/Jeff_Louie/safearray.html
//http://msdn.microsoft.com/en-us/library/z6cfh6e6.aspx#cpcondefaultmarshalingforarraysanchor3
//http://blogs.technolog.nl/eprogrammer/archive/2005/11/24/402.aspx


// Wrapper.cpp : Implementation of CWrapper

#include "stdafx.h"
#include "Wrapper.h"
#include "maxwell_h\maxwell.h"

// CWrapper

static byte pErrorCallBack(byte type, const char * pMethod, const char * pError, const void * pValue)
{
	return byte();
}

int DllExport CWrapper::GetCameraNames(LPCTSTR MXSPath, SAFEARRAY ** pOutStuff)
{
	USES_CONVERSION;  // enables use of ATL conversion macro A2W
	HRESULT hr = S_OK;

	// Create an instance of Cmaxwell and read the MXS file
	Cmaxwell* pMaxwell = new Cmaxwell( &pErrorCallBack );
	Cmaxwell::CoptionsReadMXS mxsOptions;
	//mxsOptions.enableOnly(Cmaxwell::CoptionsReadMXS.READ_CAMERAS);
	const char * scenePath = (const char*) MXSPath;
	pMaxwell->readMXS( scenePath, mxsOptions );

	// Create SafeArray of VARIANT BSTRs
	SAFEARRAYBOUND aDim[1];		// a one dimensional array
	aDim[0].lLbound = 0;		// Visual Basic arrays start with index 0
	aDim[0].cElements = 0;
	*pOutStuff = SafeArrayCreate(VT_BSTR, 1, aDim);  // create a 1D SafeArray of VARIANTS
	LONG cameraNumber[1] = {0};

	// Iterate through cameras
	Cmaxwell::Ccamera::Citerator it;
	Cmaxwell::Ccamera camera = it.first( pMaxwell );

	while ( !camera.isNull() )
	{
		aDim[0].lLbound = 0;
		aDim[0].cElements = cameraNumber[0] + 1;
		SafeArrayRedim(*pOutStuff, aDim);

		real fl, ft, shut, filw, filh, iso, angle, pixelaspect, zClipMin, zClipMax, shiftLensX, shiftLensY;
		const char* diaph;
		dword nSteps, blades, fps, xres, yres;
		const char* cameraName = camera.getValues( nSteps, shut, filw, filh, iso, &diaph, angle, blades, fps, xres, yres, pixelaspect );
		//OutputDebugString(A2W(cameraName));

		VARIANT vOut;
		VariantInit(&vOut);
		vOut.vt= VT_BSTR;  // set type
		vOut.bstrVal = ::SysAllocString(A2W(cameraName)); // system wide "new"

		hr = SafeArrayPutElement(*pOutStuff, cameraNumber, vOut.bstrVal);
		if ( S_OK != hr) { // "correctly" copies VARIANT
			SafeArrayDestroy(*pOutStuff); // does a deep destroy on error
			return hr;
		}

		//if ( pMaxwell->getActiveCamera().getPointer() == camera.getPointer() )
		//{
		//	// This camera was the active one. Do whatever you need.
		//}
		camera = it.next();
		cameraNumber[0]++;
	}
	return S_OK;
}

int DllExport CWrapper::Test(LPCTSTR MXSPath, SAFEARRAY ** pCameraNames)
{
	USES_CONVERSION;  // enables use of ATL conversion macro A2W
	HRESULT hr = S_OK;

	// Create an instance of Cmaxwell and read the MXS file
	Cmaxwell* pMaxwell = new Cmaxwell( &pErrorCallBack );
	Cmaxwell::CoptionsReadMXS mxsOptions;
	//mxsOptions.enableOnly(Cmaxwell::CoptionsReadMXS.READ_CAMERAS);
	const char * scenePath = (const char*) MXSPath;
	pMaxwell->readMXS( scenePath, mxsOptions );

	// Create SafeArray of VARIANT BSTRs
	SAFEARRAYBOUND aDim[1];		// a one dimensional array
	aDim[0].lLbound = 0;		// Visual Basic arrays start with index 0
	aDim[0].cElements = 0;
	*pCameraNames = SafeArrayCreate(VT_BSTR, 1, aDim);  // create a 1D SafeArray of VARIANTS
	LONG cameraNumber[1] = {0};

	// Iterate through cameras
	Cmaxwell::Ccamera::Citerator it;
	Cmaxwell::Ccamera camera = it.first( pMaxwell );

	while ( !camera.isNull() )
	{
		aDim[0].lLbound = 0;
		aDim[0].cElements = cameraNumber[0] + 1;
		SafeArrayRedim(*pCameraNames, aDim);

		real fl, ft, shut, filw, filh, iso, angle, pixelaspect, zClipMin, zClipMax, shiftLensX, shiftLensY;
		const char* diaph;
		dword nSteps, blades, fps, xres, yres;
		const char* cameraName = camera.getValues( nSteps, shut, filw, filh, iso, &diaph, angle, blades, fps, xres, yres, pixelaspect );
		//OutputDebugString(A2W(cameraName));

		VARIANT vOut;
		VariantInit(&vOut);
		vOut.vt= VT_BSTR;  // set type
		vOut.bstrVal = ::SysAllocString(A2W(cameraName)); // system wide "new"

		hr = SafeArrayPutElement(*pCameraNames, cameraNumber, vOut.bstrVal);
		if ( S_OK != hr) { // "correctly" copies VARIANT
			SafeArrayDestroy(*pCameraNames); // does a deep destroy on error
			return hr;
		}

		//if ( pMaxwell->getActiveCamera().getPointer() == camera.getPointer() )
		//{
		//	// This camera was the active one. Do whatever you need.
		//}
		camera = it.next();
		cameraNumber[0]++;
	}
	return S_OK;
}
