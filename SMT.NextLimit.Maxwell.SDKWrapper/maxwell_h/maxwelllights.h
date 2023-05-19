#ifndef _MAXWELL_LIGHTS_H_
#define _MAXWELL_LIGHTS_H_


#if defined(_MAC_PPC) || defined(_MACOSX)
		#pragma cpp_extensions on
#endif

#include "maxwell.h"
#ifndef pi
#define pi 	3.141592653589
#endif

//Input: (generales)nombre, posicion y orientacion ( matriz ), cmaxwell, scenescale, material emisor, 
//			 (particulares)Parametros para cada geometria emisora
//Output : objeto emisor con material emisor y en una posicion dada.


/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
byte CreateHemisphereLight ( Cmaxwell* pScene, char* name, Cbase& base, 
														Cmaxwell::Cmaterial& emitMat, real sceneScale,  
														real sizeX, real sizeY, real sizeZ, dword hemisphereSegments,
														bool hideToCamera, bool hideToRays,
														Cmaxwell::Cobject& hemisphere );
/*****************************************************************************/
/*****************************************************************************/
byte CreateCubeLight ( Cmaxwell* pScene, char* name, Cbase& base, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real sizeY, real sizeZ, 
											bool hideToCamera, bool hideToRays,
											Cmaxwell::Cobject& cube );
/*****************************************************************************/
/*****************************************************************************/
byte CreateRectangleLight ( Cmaxwell* pScene, char* name, Cbase& base, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real sizeZ, 
											bool hideToCamera, bool hideToRays,
											Cmaxwell::Cobject& rectangle );
/*****************************************************************************/
/*****************************************************************************/
byte CreateDiscLight ( Cmaxwell* pScene, char* name, Cbase& base, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real sizeZ, dword discSegments, 
											bool hideToCamera, bool hideToRays,
											Cmaxwell::Cobject& disco );
/*****************************************************************************/
/*****************************************************************************/
byte CreateCylinderLight ( Cmaxwell* pScene, char* name, Cbase& base, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real height, real sizeZ, dword cylinderSegments, 
											bool hideToCamera, bool hideToRays,
											Cmaxwell::Cobject& cylinder );
/*****************************************************************************/
/*****************************************************************************/
byte CreateOmniLight ( Cmaxwell* pScene, char* name, const Cpoint& position, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real sizeY, real sizeZ, dword sphereSegments, 
											bool hideToCamera, bool hideToRays, 
											Cmaxwell::Cobject& sphere );
/*****************************************************************************/
/*****************************************************************************/
byte CreateConicalSpotLight( Cmaxwell* pScene, char* name, Cbase& base,
											Cmaxwell::Cmaterial& emitMat, Cmaxwell::Cmaterial& shadeMat, 
											real sceneScale, real sizeX, real shadeHeight, real sizeZ, 
											real fallOff, 
											dword shadeSegments, bool hideToCamera, bool hideToRays, 
											Cmaxwell::Cobject& shade, Cmaxwell::Cobject& emitter );
/*****************************************************************************/
/*****************************************************************************/
byte CreateCylindricalSpotLight( Cmaxwell* pScene, char* name, Cbase& base,
											Cmaxwell::Cmaterial& emitMat, Cmaxwell::Cmaterial& shadeMat, 
											real sceneScale, real sizeX, real shadeHeight, real sizeZ, 
											dword shadeSegments, bool hideToCamera, bool hideToRays, 
											Cmaxwell::Cobject& shade, Cmaxwell::Cobject& emitter );
/*****************************************************************************/
/*****************************************************************************/
byte DoSphere ( Cmaxwell* pScene, char* name, dword sphereSegments, 
									real sizeX, real sizeY, real sizeZ, Cmaxwell::Cobject& sphere,  
									real centerOffsetY = 0.0 );
/*****************************************************************************/
/*****************************************************************************/
byte DoCone ( Cmaxwell* pScene, char* name, dword coneBaseSegments, 
							real radioX, real height, real radioZ, Cmaxwell::Cobject& cone );
/*****************************************************************************/
/*****************************************************************************/
byte DoCylinder ( Cmaxwell* pScene, char* name, dword cylinderSegments, 
									real radioX, real height, real radioZ, Cmaxwell::Cobject& cylinder,
									real centerOffsetHeight = 0.0 );
/*****************************************************************************/
/*****************************************************************************/
byte createDefaultEmitter( Cmaxwell* pScene, char* emitterName, Cmaxwell::Cmaterial& newEmitterMat );
/*****************************************************************************/
/*****************************************************************************/
byte createDefaultShade( Cmaxwell* pScene, char* shadeName, Cmaxwell::Cmaterial& newShade );
/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
#endif
