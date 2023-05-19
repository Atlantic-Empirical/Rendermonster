#include "maxwelllights.h"

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
														Cmaxwell::Cobject& hemisphere )
{
	byte isOk = 1;
	real radiusX = sizeX *sceneScale * 0.5;
	real radiusY = sizeY *sceneScale * 0.5;
	real radiusZ = sizeZ *sceneScale * 0.5;

	dword nVertices = 2 + ( hemisphereSegments - 2 ) * hemisphereSegments;
	dword nTriangulos = 2 * hemisphereSegments + ( hemisphereSegments - 3 ) * 2 * hemisphereSegments;
	dword nNormales = 0;
	hemisphere = pScene->createMesh ( name, nVertices, nNormales, nTriangulos, 1 );

	Cpoint pos;
	pos.assign ( 0.0 , radiusY,  0.0 );//Polo norte
	isOk = hemisphere.setVertex ( 0, 0, pos );

	pos.assign ( 0.0, 0.0, 0.0 );//Polo sur
	isOk = hemisphere.setVertex ( nVertices - 1, 0, pos );

	dword latSegments = hemisphereSegments - 1;
	real deltaLat =  pi / ( ( real ) 2.0*latSegments );

	//Vertices
	dword iVertex = 1;
	for ( dword i = 0; i < latSegments - 1; i++ )
	{
		real lat0 = pi/2 - deltaLat - ( deltaLat * i );

		for ( dword j = 0; j < hemisphereSegments; j++ )
		{
			real lon0 = (2 * pi /(real)hemisphereSegments)* j;

			real x0 = radiusX*cosf( lat0 ) * cosf( lon0 );			
			real y0 = radiusY*sinf( lat0 );
			real z0 = radiusZ*cosf( lat0 ) * sinf( lon0 );
			
			pos.assign ( x0, y0, z0 );
			isOk = hemisphere.setVertex ( iVertex, 0, pos );
			iVertex++;
		}
	}

	//Caras
//Casquete superior
	for ( dword i = 0; i < hemisphereSegments; i++ )
	{
		dword i2 = i+2;
		if ( i == hemisphereSegments - 1 ) i2 = 1;
		isOk = hemisphere.setTriangle ( i, 0, i+1, i2, 0, 0, 0 );		
	}

	dword coronas = latSegments - 2;
	dword iFace = hemisphereSegments;
	for ( dword i = 0; i < coronas; i++ )
	{
		for ( dword j = 0; j < hemisphereSegments; j++ )
		{
			dword vOff = 1 + i*hemisphereSegments;
			dword i1 = vOff + j;
			dword i2 = vOff + hemisphereSegments + j;
			dword i3 = vOff + hemisphereSegments + j + 1;
			if ( j == hemisphereSegments - 1 ) i3 = vOff + hemisphereSegments;
			dword i4 = vOff + j + 1;
			if ( j == hemisphereSegments - 1 ) i4 = vOff;

			isOk = hemisphere.setTriangle ( iFace, i1, i2, i4, 0, 0, 0 );
			iFace++;
			isOk = hemisphere.setTriangle ( iFace, i2, i3, i4, 0, 0, 0 );
			iFace++;
		}
	}

//Disco inferior
	dword vO = 1+hemisphereSegments*(hemisphereSegments-3);
	for ( dword i = 0; i < hemisphereSegments; i++ )
	{
		dword i2 = i + vO + 1;
		if ( i == hemisphereSegments - 1 ) i2 = vO;
		isOk = hemisphere.setTriangle ( iFace, i + vO, nVertices-1, i2, 0, 0, 0 );		
		iFace++;
	}

	isOk = hemisphere.setBase ( base );
	Cbase pivot;
	pivot.initCanonic ();
	isOk = hemisphere.setPivot( pivot );

	isOk = hemisphere.setHideToCamera( hideToCamera );
	isOk = hemisphere.setHideToReflectionsRefractions( hideToRays );
	isOk = hemisphere.setMaterial( emitMat );

	return isOk;
}

/*****************************************************************************/
/*****************************************************************************/
byte CreateCubeLight ( Cmaxwell* pScene, char* name, Cbase& base, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real sizeY, real sizeZ, 
											bool hideToCamera, bool hideToRays,
											Cmaxwell::Cobject& cube )
{
	byte isOk = 1;

	sizeX *= sceneScale * 0.5;
	sizeY *= sceneScale * 0.5;
	sizeZ *= sceneScale * 0.5;

	dword nVertices = 8;
	dword nTriangulos = 12;
	dword nNormales = 0;
	cube = pScene->createMesh ( name, nVertices, nNormales, nTriangulos, 1 );

	Cpoint pos;
	pos.assign ( sizeX, sizeY, -sizeZ );
	isOk = cube.setVertex ( 0, 0, pos );

	pos.assign ( -sizeX, sizeY, -sizeZ );
	isOk = cube.setVertex ( 1, 0, pos );

	pos.assign ( -sizeX, -sizeY, -sizeZ );
	isOk = cube.setVertex ( 2, 0, pos );

	pos.assign ( sizeX, -sizeY, -sizeZ );
	isOk = cube.setVertex ( 3, 0, pos );

	pos.assign ( sizeX, sizeY, sizeZ  );
	isOk = cube.setVertex ( 4, 0, pos );

	pos.assign ( -sizeX, sizeY, sizeZ );
	isOk = cube.setVertex ( 5, 0, pos );

	pos.assign ( -sizeX, -sizeY, sizeZ );
	isOk = cube.setVertex ( 6, 0, pos );

	pos.assign ( sizeX, -sizeY, sizeZ );
	isOk = cube.setVertex ( 7, 0, pos );

	isOk = cube.setTriangle ( 0, 0, 1, 2, 0, 0, 0 );		
	isOk = cube.setTriangle ( 1, 0, 2, 3, 0, 0, 0 );		
	
	isOk = cube.setTriangle ( 2, 0, 4, 1, 0, 0, 0 );		
	isOk = cube.setTriangle ( 3, 1, 4, 5, 0, 0, 0 );		
	
	isOk = cube.setTriangle ( 4, 2, 1, 5, 0, 0, 0 );		
	isOk = cube.setTriangle ( 5, 5, 6, 2, 0, 0, 0 );		
	
	isOk = cube.setTriangle ( 6, 6, 3, 2, 0, 0, 0 );		
	isOk = cube.setTriangle ( 7, 6, 7, 3, 0, 0, 0 );		

	isOk = cube.setTriangle ( 8, 0, 3, 4, 0, 0, 0 );		
	isOk = cube.setTriangle ( 9, 3, 7, 4, 0, 0, 0 );
			
	isOk = cube.setTriangle ( 10, 4, 6, 5, 0, 0, 0 );		
	isOk = cube.setTriangle ( 11, 4, 7, 6, 0, 0, 0 );		

	isOk = cube.setBase ( base );
	Cbase pivot;
	pivot.initCanonic ();
	isOk = cube.setPivot( pivot );

	isOk = cube.setHideToCamera( hideToCamera );
	isOk = cube.setHideToReflectionsRefractions( hideToRays );
	isOk = cube.setMaterial( emitMat );

	return isOk;

}
/*****************************************************************************/
/*****************************************************************************/
byte CreateRectangleLight ( Cmaxwell* pScene, char* name, Cbase& base, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real sizeZ, 
											bool hideToCamera, bool hideToRays,
											Cmaxwell::Cobject& rectangle )
{
	byte isOk = 1;

	sizeX *= sceneScale * 0.5;
	sizeZ *= sceneScale * 0.5;

	dword nVertices = 4;
	dword nTriangulos = 2;
	dword nNormales = 0;
	rectangle = pScene->createMesh ( name, nVertices, nNormales, nTriangulos, 1 );

	Cpoint pos;
	pos.assign ( sizeX, 0.0, sizeZ );
	isOk = rectangle.setVertex ( 0, 0, pos );

	pos.assign ( sizeX, 0.0, -sizeZ );
	isOk = rectangle.setVertex ( 1, 0, pos );

	pos.assign ( -sizeX, 0.0, -sizeZ );
	isOk = rectangle.setVertex ( 2, 0, pos );

	pos.assign ( -sizeX, 0.0, sizeZ );
	isOk = rectangle.setVertex ( 3, 0, pos );

	isOk = rectangle.setTriangle ( 0, 0, 1, 2, 0, 0, 0 );		
	isOk = rectangle.setTriangle ( 1, 0, 2, 3, 0, 0, 0 );		

	isOk = rectangle.setBase ( base );
	Cbase pivot;
	pivot.initCanonic ();
	isOk = rectangle.setPivot( pivot );

	isOk = rectangle.setHideToCamera( hideToCamera );
	isOk = rectangle.setHideToReflectionsRefractions( hideToRays );
	isOk = rectangle.setMaterial( emitMat );

	return isOk;
}

/*****************************************************************************/
/*****************************************************************************/
byte CreateDiscLight ( Cmaxwell* pScene, char* name, Cbase& base, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real sizeZ, dword discSegments, 
											bool hideToCamera, bool hideToRays,
											Cmaxwell::Cobject& disco )
{
	byte isOk = 1;

	sizeX *= sceneScale * 0.5;
	sizeZ *= sceneScale * 0.5;

	dword nVertices = 1 + discSegments;
	dword nTriangulos = discSegments;
	dword nNormales = 0;
	disco = pScene->createMesh ( name, nVertices, nNormales, nTriangulos, 1 );

	Cpoint pos;
	pos.assign ( 0.0, 0.0, 0.0 );//Centro del disco
	disco.setVertex ( 0, 0, pos );

	//Vertices
	dword iVertex = 1;
	for ( dword j = 0; j < discSegments; j++ )
	{
		real lon0 = (2 * pi /(real)discSegments)* j;

		real x0 = sizeX * cosf( lon0 );			
		real y0 = 0.0;
		real z0 = sizeZ * sinf( lon0 );			
		
		pos.assign ( x0, y0, z0 );
		isOk = disco.setVertex ( iVertex, 0, pos );
		iVertex++;
	}

	//Caras
	for ( dword i = 0; i < discSegments; i++ )
	{
		dword i2 = i+2;
		if ( i == discSegments - 1 ) i2 = 1;
		isOk = disco.setTriangle ( i, 0, i2, i+1, 0, 0, 0 );		
	}

	isOk = disco.setBase ( base );
	Cbase pivot;
	pivot.initCanonic ();
	isOk = disco.setPivot( pivot );

	isOk = disco.setHideToCamera( hideToCamera );
	isOk = disco.setHideToReflectionsRefractions( hideToRays );
	isOk = disco.setMaterial( emitMat );

	return isOk;
}
/*****************************************************************************/
/*****************************************************************************/
byte CreateCylinderLight ( Cmaxwell* pScene, char* name, Cbase& base, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real height, real sizeZ, dword cylinderSegments, 
											bool hideToCamera, bool hideToRays,
											Cmaxwell::Cobject& cylinder )
{
	byte isOk = 1;

	sizeX *= sceneScale * 0.5;
	height *= sceneScale;
	sizeZ *= sceneScale * 0.5;

	if ( cylinderSegments < 3 ) cylinderSegments = 3;
	isOk = DoCylinder ( pScene, name, cylinderSegments, sizeX, height, sizeZ, cylinder );

	isOk = cylinder.setBase ( base );
	Cbase pivot;
	pivot.initCanonic ();
	isOk = cylinder.setPivot( pivot );

	isOk = cylinder.setHideToCamera( hideToCamera );
	isOk = cylinder.setHideToReflectionsRefractions( hideToRays );
	isOk = cylinder.setMaterial( emitMat );

	return isOk;
}

/*****************************************************************************/
/*****************************************************************************/
byte CreateOmniLight ( Cmaxwell* pScene, char* name, const Cpoint& position, 
											Cmaxwell::Cmaterial& emitMat, real sceneScale, 
											real sizeX, real sizeY, real sizeZ, dword sphereSegments, 
											bool hideToCamera, bool hideToRays, 
											Cmaxwell::Cobject& sphere )
{
	byte isOk = 1;

	sizeX *= sceneScale;// * 0.5;
	sizeY *= sceneScale;// * 0.5;
	sizeZ *= sceneScale;// * 0.5;

	if ( sphereSegments < 5 ) sphereSegments = 5;
	isOk = DoSphere ( pScene, name, sphereSegments, sizeX, sizeY, sizeZ, sphere );

  Cbase base;
  base.initCanonic();
  base.origin.assign( position );
	isOk = sphere.setBase ( base );

	Cbase pivot;
	pivot.initCanonic ();
	isOk = sphere.setPivot( pivot );

	isOk = sphere.setHideToCamera( hideToCamera );
	isOk = sphere.setHideToReflectionsRefractions( hideToRays );

  if( emitMat.isNull() )
  {
    createDefaultEmitter( pScene, name, emitMat );
  }
	isOk = sphere.setMaterial( emitMat );

	return isOk;
}

/*****************************************************************************/
/*****************************************************************************/
byte CreateConicalSpotLight( Cmaxwell* pScene, char* name, Cbase& base,
											Cmaxwell::Cmaterial& emitMat, Cmaxwell::Cmaterial& shadeMat, 
											real sceneScale, real sizeX, real shadeHeight, real sizeZ, 
											real fallOff, 
											dword shadeSegments, bool hideToCamera, bool hideToRays, 
											Cmaxwell::Cobject& shade, Cmaxwell::Cobject& emitter )
{
	byte isOk = 1;
	shadeHeight *= sceneScale;
	sizeX *= sceneScale * 0.5;
	sizeZ *= sceneScale * 0.5;
	real minRadio = __min ( sizeX, sizeZ );
	if ( fallOff < 0.0 ) fallOff = 0.0;
	else if ( fallOff > 1.0 ) fallOff = 1.0;
	real ballOffset = fallOff * shadeHeight;
	ballOffset = __max ( 0.1 * shadeHeight, ballOffset );
	real ballRadius = 0.8 * ballOffset * minRadio / shadeHeight;
	ballRadius = __min ( ballRadius, minRadio * 0.5 );

	if ( shadeSegments < 3 ) shadeSegments = 3;
	char shdName[256];
	sprintf ( shdName, "%s_shade", name );		

	isOk = DoCone ( pScene, shdName, shadeSegments, sizeX, shadeHeight, sizeZ, shade );
	isOk = shade.setBase ( base );
	Cbase pivot;
	pivot.initCanonic ();
	isOk = shade.setPivot( pivot );

	isOk = shade.setHideToCamera( hideToCamera );
	isOk = shade.setHideToReflectionsRefractions( hideToRays );
  if( shadeMat.isNull() )
  {
    createDefaultShade( pScene, name, shadeMat );
  }
  isOk = shade.setMaterial( shadeMat );

	char sphName[256];
	sprintf ( sphName, "%s_emitter", name );		
	dword lightBallSegments = 24;
	
	isOk = DoSphere ( pScene, sphName, lightBallSegments, ballRadius, ballRadius, ballRadius, 
										emitter, ballOffset );	
	isOk = emitter.setBase ( base );
	isOk = emitter.setPivot( pivot );

	isOk = emitter.setHideToCamera( hideToCamera );
	isOk = emitter.setHideToReflectionsRefractions( hideToRays );

  if( emitMat.isNull() )
  {
    createDefaultEmitter( pScene, name, emitMat );
  }
	isOk = emitter.setMaterial( emitMat );
	return isOk;
}


/*****************************************************************************/
/*****************************************************************************/
byte CreateCylindricalSpotLight( Cmaxwell* pScene, char* name, Cbase& base,
											Cmaxwell::Cmaterial& emitMat, Cmaxwell::Cmaterial& shadeMat, 
											real sceneScale, real sizeX, real shadeHeight, real sizeZ, 
											dword shadeSegments, bool hideToCamera, bool hideToRays, 
											Cmaxwell::Cobject& shade, Cmaxwell::Cobject& emitter )
{
	byte isOk = 1;
	if ( shadeSegments < 3 ) shadeSegments = 3;
	shadeHeight *= sceneScale;
	sizeX *= sceneScale * 0.5;
	sizeZ *= sceneScale * 0.5;
	shadeHeight = fabs ( shadeHeight );//kobarrrde
	sizeX = fabs ( sizeX );
	sizeZ = fabs ( sizeZ );
	real emitterRadius = 0.05 * __min ( sizeX, sizeZ );
	dword nVertices = 1 + 2 * shadeSegments;
	dword nTriangulos = 3 * shadeSegments;
	dword nNormales = 0;

	char shdName[256];
	sprintf ( shdName, "%s_shade", name );		
	shade = pScene->createMesh ( shdName, nVertices, nNormales, nTriangulos, 1 );

	Cpoint pos;
	pos.assign ( 0.0, 0.0 , 0.0 );//Centro de la tapa
	shade.setVertex ( 0, 0, pos );

	//Vertices
	dword iVertex = 1;
	if ( shadeSegments != 4 )
	{
		for ( dword j = 0; j < shadeSegments; j++ )
		{
			real lon0 = (2 * pi /(real)shadeSegments)* j;

			real x0 = sizeX* cosf( lon0 );			
			real y0 = 0.0;
			real z0 = sizeZ* sinf( lon0 );
			
			pos.assign ( x0, y0, z0 );
			isOk = shade.setVertex ( iVertex, 0, pos );
			pos.assign ( x0, y0 + shadeHeight, z0 );
			isOk = shade.setVertex ( iVertex + shadeSegments, 0, pos );
			iVertex++;
		}
	}
	else
	{
		real x0 = sizeX;
		real y0 = 0.0;
		real z0 = sizeZ;
		pos.assign ( x0, y0, z0 );
		isOk = shade.setVertex ( iVertex, 0, pos );
		pos.assign ( x0, y0 + shadeHeight, z0 );
		isOk = shade.setVertex ( iVertex + shadeSegments, 0, pos );
		iVertex++;
		pos.assign ( x0, y0, -z0 );
		isOk = shade.setVertex ( iVertex, 0, pos );
		pos.assign ( x0, y0 + shadeHeight, -z0 );
		isOk = shade.setVertex ( iVertex + shadeSegments, 0, pos );
		iVertex++;
		pos.assign ( -x0, y0, -z0 );
		isOk = shade.setVertex ( iVertex, 0, pos );
		pos.assign ( -x0, y0 + shadeHeight, -z0 );
		isOk = shade.setVertex ( iVertex + shadeSegments, 0, pos );
		iVertex++;
		pos.assign ( -x0, y0, z0 );
		isOk = shade.setVertex ( iVertex, 0, pos );
		pos.assign ( -x0, y0 + shadeHeight, z0 );
		isOk = shade.setVertex ( iVertex + shadeSegments, 0, pos );
		iVertex++;
	}
//Caras
//Tapa superior
	for ( dword i = 0; i < shadeSegments; i++ )
	{
		dword i2 = i+2;
		if ( i == shadeSegments - 1 ) i2 = 1;
		shade.setTriangle ( i, 0, i2, i+1, 0, 0, 0 );		
	}

	dword iFace = shadeSegments;
	for ( dword j = 0; j < shadeSegments; j++ )
	{
		dword vOff = 1;
		dword i1 = vOff + j;
		dword i2 = vOff + shadeSegments + j;
		dword i3 = vOff + shadeSegments + j + 1;
		if ( j == shadeSegments - 1 ) i3 = vOff + shadeSegments;
		dword i4 = vOff + j + 1;
		if ( j == shadeSegments - 1 ) i4 = vOff;

		shade.setTriangle ( iFace, i1, i4, i2, 0, 0, 0 );
		iFace++;
		shade.setTriangle ( iFace, i2, i4, i3, 0, 0, 0 );
		iFace++;
	}


	isOk = shade.setBase ( base );
	Cbase pivot;
	pivot.initCanonic ();
	isOk = shade.setPivot( pivot );

	isOk = shade.setHideToCamera( hideToCamera );
	isOk = shade.setHideToReflectionsRefractions( hideToRays );

  if( shadeMat.isNull() )
  {
    createDefaultShade( pScene, name, shadeMat );
  }
	isOk = shade.setMaterial( shadeMat );

	char emtName[256];
	sprintf ( emtName, "%s_emitter", name );		

	real filamentLen = 0.75 * shadeHeight;
	isOk = DoCylinder ( pScene, emtName, 24, 
							emitterRadius, filamentLen, emitterRadius, emitter, 
							0.51 * filamentLen );
	isOk = emitter.setBase ( base );
	isOk = emitter.setPivot( pivot );

	isOk = emitter.setHideToCamera( hideToCamera );
	isOk = emitter.setHideToReflectionsRefractions( hideToRays );

  if( emitMat.isNull() )
  {
    createDefaultEmitter( pScene, name, emitMat );
  }
	isOk = emitter.setMaterial( emitMat );
	return isOk;
}

/*****************************************************************************/
/*****************************************************************************/
byte DoSphere ( Cmaxwell* pScene, char* name, dword sphereSegments, 
									real sizeX, real sizeY, real sizeZ, Cmaxwell::Cobject& sphere,  
									real centerOffsetY )
{
	byte isOk = 1;
	dword nVertices = 2 + ( sphereSegments - 2 ) * sphereSegments;
	dword nTriangulos = 2 * sphereSegments + ( sphereSegments - 3 ) * 2 * sphereSegments;
	dword nNormales = 0;
	sphere = pScene->createMesh ( name, nVertices, nNormales, nTriangulos, 1 );

	Cpoint pos;
	pos.assign ( 0.0, sizeY + centerOffsetY, 0.0 );//Polo norte
	isOk = sphere.setVertex ( 0, 0, pos );

	pos.assign ( 0.0, -sizeY + centerOffsetY, 0.0 );//Polo sur
	isOk = sphere.setVertex ( nVertices - 1, 0, pos );

	dword latSegments = sphereSegments - 1;
	real deltaLat = pi / ( ( real ) latSegments );

	//Vertices
	dword iVertex = 1;
	for ( dword i = 0; i < latSegments - 1; i++ )
	{
		real lat0 = pi/2 - deltaLat - ( deltaLat * i );

		for ( dword j = 0; j < sphereSegments; j++ )
		{
			real lon0 = (2 * pi /(real)sphereSegments)* j;

			real x0 = sizeX*cosf( lat0 ) * cosf( lon0 );			
			real y0 = sizeY*sinf( lat0 );
			real z0 = sizeZ*cosf( lat0 ) * sinf( lon0 );
			
			pos.assign ( x0, y0 + centerOffsetY, z0 );
			isOk = sphere.setVertex ( iVertex, 0, pos );
			iVertex++;
		}
	}

	//Caras
//Casquete superior
	for ( dword i = 0; i < sphereSegments; i++ )
	{
		dword i2 = i+2;
		if ( i == sphereSegments - 1 ) i2 = 1;
		sphere.setTriangle ( i, i+1, 0, i2, 0, 0, 0 );		
		//sphere.setTriangle ( i, 0, i+1, i2, 0, 0, 0 );		
	}

	dword coronas = latSegments - 2;
	dword iFace = sphereSegments;
	for ( dword i = 0; i < coronas; i++ )
	{
		for ( dword j = 0; j < sphereSegments; j++ )
		{
			dword vOff = 1 + i*sphereSegments;
			dword i1 = vOff + j;
			dword i2 = vOff + sphereSegments + j;
			dword i3 = vOff + sphereSegments + j + 1;
			if ( j == sphereSegments - 1 ) i3 = vOff + sphereSegments;
			dword i4 = vOff + j + 1;
			if ( j == sphereSegments - 1 ) i4 = vOff;

			isOk = sphere.setTriangle ( iFace, i2, i1, i4, 0, 0, 0 );
			//isOk = sphere.setTriangle ( iFace, i1, i2, i4, 0, 0, 0 );
			iFace++;
			isOk = sphere.setTriangle ( iFace, i3, i2, i4, 0, 0, 0 );
			//isOk = sphere.setTriangle ( iFace, i2, i3, i4, 0, 0, 0 );
			iFace++;
		}
	}

//Casquete inferior
	dword vO = 1+sphereSegments*(sphereSegments-3);
	for ( dword i = 0; i < sphereSegments; i++ )
	{
		dword i2 = i + vO + 1;
		if ( i == sphereSegments - 1 ) i2 = vO;
		isOk = sphere.setTriangle ( iFace, nVertices-1, i + vO, i2, 0, 0, 0 );		
		//isOk = sphere.setTriangle ( iFace, i + vO, nVertices-1, i2, 0, 0, 0 );		
		iFace++;
	}
	return isOk;
}


/*****************************************************************************/
/*****************************************************************************/
byte DoCone ( Cmaxwell* pScene, char* name, dword coneBaseSegments, 
							real radioX, real height, real radioZ, Cmaxwell::Cobject& cone )
{
	byte isOk = 1;
	dword nVertices = 1 + coneBaseSegments;
	dword nTriangulos = coneBaseSegments;
	dword nNormales = 0;
	cone = pScene->createMesh ( name, nVertices, nNormales, nTriangulos, 1 );

	Cpoint pos;
	pos.assign ( 0.0, 0.0, 0.0 );//Vertice del cono
	isOk = cone.setVertex ( 0, 0, pos );

	//Vertices
	dword iVertex = 1;
	if ( coneBaseSegments != 4 )
	{
		for ( dword j = 0; j < coneBaseSegments; j++ )
		{
			real lon0 = (2 * pi /(real)coneBaseSegments)* j;

			real x0 = radioX * cosf( lon0 );			
			real y0 = height;
			real z0 = radioZ * sinf( lon0 );			
			
			pos.assign ( x0, y0, z0 );
			isOk = cone.setVertex ( iVertex, 0, pos );
			iVertex++;
		}
	}
	else
	{
		real x0 = radioX;
		real y0 = height;
		real z0 = radioZ;
		pos.assign ( x0, y0, z0 );
		isOk = cone.setVertex ( iVertex, 0, pos );
		iVertex++;
		pos.assign ( x0, y0, -z0 );
		isOk = cone.setVertex ( iVertex, 0, pos );
		iVertex++;
		pos.assign ( -x0, y0, -z0 );
		isOk = cone.setVertex ( iVertex, 0, pos );
		iVertex++;
		pos.assign ( -x0, y0, z0 );
		isOk = cone.setVertex ( iVertex, 0, pos );
		iVertex++;
	}
	//Caras
	for ( dword i = 0; i < coneBaseSegments; i++ )
	{
		dword i2 = i+2;
		if ( i == coneBaseSegments - 1 ) i2 = 1;
		//isOk = cone.setTriangle ( i, 0, i+1, i2, 0, 0, 0 );		
		isOk = cone.setTriangle ( i, i+1, 0, i2, 0, 0, 0 );		
	}
	return isOk;
}




/*****************************************************************************/
/*****************************************************************************/
byte DoCylinder ( Cmaxwell* pScene, char* name, dword cylinderSegments, 
									real radioX, real height, real radioZ, Cmaxwell::Cobject& cylinder,
									real centerOffsetHeight )
{
	dword nVertices = 2 + 2 * cylinderSegments;
 	dword nTriangulos = 4 * cylinderSegments;
	dword nNormales = 0;
	cylinder = pScene->createMesh ( name, nVertices, nNormales, nTriangulos, 1 );

	Cpoint pos;
	pos.assign ( 0.0, height/2.0 + centerOffsetHeight, 0.0 );//Polo norte
	byte isOk = cylinder.setVertex ( 0, 0, pos );

	pos.assign ( 0.0, -height/2.0 + centerOffsetHeight, 0.0 );//Polo sur
	isOk = cylinder.setVertex ( nVertices - 1, 0, pos );

	//Vertices
	dword iVertex = 1;

	for ( dword j = 0; j < cylinderSegments; j++ )
	{
		real lon0 = (2 * pi /(real)cylinderSegments)* j;

		real x0 = radioX* cosf( lon0 );			
		real y0 = height/2.0;
		real z0 = radioZ* sinf( lon0 );
		
		pos.assign ( x0 , y0 + centerOffsetHeight, z0 );
		isOk = cylinder.setVertex ( iVertex, 0, pos );
		pos.assign ( x0, -y0 + centerOffsetHeight, z0 );
		isOk = cylinder.setVertex ( iVertex + cylinderSegments, 0, pos );
		iVertex++;
	}


	//Caras
//Tapa superior
	for ( dword i = 0; i < cylinderSegments; i++ )
	{
		dword i2 = i+2;
		if ( i == cylinderSegments - 1 ) i2 = 1;
		isOk = cylinder.setTriangle ( i, i2, 0, i+1, 0, 0, 0 );		
		//isOk = cylinder.setTriangle ( i, 0, i2, i+1, 0, 0, 0 );		
	}


	dword iFace = cylinderSegments;
	for ( dword j = 0; j < cylinderSegments; j++ )
	{
		dword vOff = 1;
		dword i1 = vOff + j;
		dword i2 = vOff + cylinderSegments + j;
		dword i3 = vOff + cylinderSegments + j + 1;
		if ( j == cylinderSegments - 1 ) i3 = vOff + cylinderSegments;
		dword i4 = vOff + j + 1;
		if ( j == cylinderSegments - 1 ) i4 = vOff;

		isOk = cylinder.setTriangle ( iFace, i4, i1, i2, 0, 0, 0 );
		//isOk = cylinder.setTriangle ( iFace, i1, i4, i2, 0, 0, 0 );
		iFace++;
		isOk = cylinder.setTriangle ( iFace, i4, i2, i3, 0, 0, 0 );
		//isOk = cylinder.setTriangle ( iFace, i2, i4, i3, 0, 0, 0 );
		iFace++;
	}

//Tapa inferior
	dword vO = 1 + cylinderSegments;
	for ( dword i = 0; i < cylinderSegments; i++ )
	{
		dword i2 = i + vO + 1;
		if ( i == cylinderSegments - 1 ) i2 = vO;
		isOk = cylinder.setTriangle ( iFace, i2, i + vO, nVertices-1, 0, 0, 0 );		
		//isOk = cylinder.setTriangle ( iFace, i + vO, i2, nVertices-1, 0, 0, 0 );		
		iFace++;
	}

	return isOk;
}


/*****************************************************************************/
/*****************************************************************************/
byte createDefaultEmitter( Cmaxwell* pScene, char* emitterName, Cmaxwell::Cmaterial& newEmitterMat )
{
  byte isOk = 0;

 	char auxEmitterName[256];
	sprintf ( auxEmitterName, "%s_emitter", emitterName );

  newEmitterMat = pScene->createMaterial( emitterName, true );
	isOk = newEmitterMat.setEmpty( );
  Cmaxwell::CmaterialEmitter newEmitter = newEmitterMat.createEmitter();
  isOk = newEmitter.setState( true );  

  Cmaxwell::CmultiValue multivalue;
  multivalue.pType = "pair";
  isOk = newEmitter.setActiveMultiValue( multivalue ); 

  Cmaxwell::CmultiValue::CemitterPair emitterPair;
  Crgb emitterColor;
	emitterColor.assign ( 0.99, 0.99, 0.99 );
  emitterPair.rgb.assign( emitterColor );
  emitterPair.watts = 10000.0;
  emitterPair.luminousEfficacy = 12.7;
  newEmitter.setPair( emitterPair );

  Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
  emitterActivePair.pEmissionColor = "rgb";
  emitterActivePair.pLuminance = "wattsAndLuminousEfficacy";
  isOk = newEmitter.setActivePair( emitterActivePair );
  return ( isOk );

}
/*****************************************************************************/
/*****************************************************************************/
byte createDefaultShade( Cmaxwell* pScene, char* shadeName, Cmaxwell::Cmaterial& newShade )
{
  // Creates a basic default material and returns it as a reference
	char auxShadeName[256];
	sprintf ( auxShadeName, "%s_shade", shadeName );

  newShade = pScene->createMaterial( auxShadeName, true );
  Cmaxwell::CmaterialBasic newBasic = newShade.addBasic( 0 );    
  Cmaxwell::Cbsdf newBsdf = newBasic.createBsdf();
  newBasic.setName( "basic" );
	Cmaxwell::Creflectance reflectance = newBsdf.getReflectance();
	Cmaxwell::CmultiValue::Cmap mvMap;
	mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_RGB;
	mvMap.rgb.assign( 0.7, 0.7, 0.7 );
  reflectance.setColor( "color", mvMap );
  return( reflectance.setActiveColor( "color", mvMap ) );
}
/*****************************************************************************/
/*****************************************************************************/
