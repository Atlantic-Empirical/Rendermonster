// dllmain.h : Declaration of module class.

class CMaxwellSDKWrapperModule : public CAtlDllModuleT< CMaxwellSDKWrapperModule >
{
public :
	DECLARE_LIBID(LIBID_MaxwellSDKWrapperLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_MAXWELLSDKWRAPPER, "{F8A6FE3A-3276-4343-8540-765414DCE3E2}")
};

extern class CMaxwellSDKWrapperModule _AtlModule;
