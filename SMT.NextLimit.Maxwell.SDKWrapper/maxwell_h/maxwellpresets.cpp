#ifndef _WIN32
	#pragma cpp_extensions on
#endif

#ifndef MAXWELL_PRESETS_H
#define MAXWELL_PRESETS_H

#include <stdio.h>
#include <string.h>
#include "maxwell.h"

#if defined (_MACOSX) || defined (_MAC_PPC)
#define _stricmp strcasecmp
#endif

//==============================================================================
//==============================================================================
byte setDisplacementMap (  
							Cmaxwell::Cmaterial& material,
							char* displacementMapPath, 
							real& offset,
							real& height,
							byte& invert,
							dword uvChannelIndex )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
   mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
  mvMap.pFileName = displacementMapPath;
  mvMap.uvwChannel = uvChannelIndex;
  mvMap.uIsTiled = 1;
  mvMap.vIsTiled = 1;
  mvMap.scale.assign( 1.0, 1.0 );
  mvMap.offset.assign( 0.0, 0.0 );
  mvMap.invert = invert;
  mvMap.doGammaCorrection = 0;
  mvMap.useAbsoluteUnits = 0;
    
	real precision = 16.0;
	real smoothing = 0.0;

	byte isOk = material.enableDisplacement( true );
//  if ( isOk ) return material.setDisplacement( mvMap, precision, offset, height, smoothing, true, false );
if ( isOk ) return material.setDisplacement( mvMap, precision, offset, height, smoothing, true, true );//pasa de la precision y del smoothing
	return isOk;
}
//==============================================================================
//==============================================================================
byte setReflectanceNdAndAbbe ( 
					Cmaxwell::Creflectance& reflectance, 
					real& nd, 
					real& abbe )
{
	Cmaxwell::CmultiValue mv;
	real   pNdAbbe[ 2 ];
	pNdAbbe[0] = nd;
	pNdAbbe[1] = abbe;
	mv.pID = "ior.real";
	mv.pType = "nd,abbe";
	mv.pParameter = pNdAbbe;
	byte isOk = reflectance.setIOR( mv );
	if ( isOk ) return reflectance.setActiveIOR( mv );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setReflectanceRGB ( 
					Cmaxwell::Creflectance& reflectance, 
					Crgb& color )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
	mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_RGB;
	mvMap.rgb.assign( color );
	byte isOk = reflectance.setColor( "color", mvMap );
	if ( isOk ) return reflectance.setActiveColor( "color", mvMap );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setReflectance90RGB (
					Cmaxwell::Creflectance& reflectance, 
					Crgb& color )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
	mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_RGB;
	mvMap.rgb.assign( color );
	byte isOk = reflectance.setColor( "color.tangential", mvMap );
	if ( isOk ) return reflectance.setActiveColor( "color.tangential", mvMap );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setReflectanceRGBMap ( 
					Cmaxwell::Creflectance& reflectance, 
					char* colorMapPath, 
					dword uvChannelIndex ) 
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
    mvMap.pFileName = colorMapPath;
    mvMap.uvwChannel = uvChannelIndex;
    mvMap.uIsTiled = 1;
    mvMap.vIsTiled = 1;
    mvMap.scale.assign( 1.0, 1.0 );
    mvMap.offset.assign( 0.0, 0.0 );
    mvMap.invert = 0;
    mvMap.doGammaCorrection = 1;
    mvMap.useAbsoluteUnits = 0;
    byte isOk = reflectance.setColor( "color", mvMap );
	if ( isOk ) return reflectance.setActiveColor( "color", mvMap ); 
	return isOk;
}
//==============================================================================
//==============================================================================
byte setReflectance90RGBMap ( 
					Cmaxwell::Creflectance& reflectance, 
					char* colorMapPath, 
					dword uvChannelIndex ) 
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
    mvMap.pFileName = colorMapPath;
    mvMap.uvwChannel = uvChannelIndex;
    mvMap.uIsTiled = 1;
    mvMap.vIsTiled = 1;
    mvMap.scale.assign( 1.0, 1.0 );
    mvMap.offset.assign( 0.0, 0.0 );
    mvMap.invert = 0;
    mvMap.doGammaCorrection = 1;
    mvMap.useAbsoluteUnits = 0;
    byte isOk = reflectance.setColor( "color.tangential", mvMap );
	if ( isOk ) return reflectance.setActiveColor( "color.tangential", mvMap ); 
	return isOk;
}
//==============================================================================
//==============================================================================
byte setReflectanceRGBXmit ( 
					Cmaxwell::Creflectance& reflectance, 
					Crgb& color ) 
{
	Cmaxwell::CmultiValue::Cmap mvMap;
	mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_RGB;
	mvMap.rgb.assign( color );
	byte isOk = reflectance.setColor( "transmittance.color", mvMap );
	if ( isOk ) return reflectance.setActiveColor( "transmittance.color", mvMap );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setReflectanceRGBXmitMap ( 
					Cmaxwell::Creflectance& reflectance, 
					char* xmitColorMapPath, 
					byte invert,
					dword uvChannelIndex ) 
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
    mvMap.pFileName = xmitColorMapPath;
    mvMap.uvwChannel = uvChannelIndex;
    mvMap.uIsTiled = 1;
    mvMap.vIsTiled = 1;
    mvMap.scale.assign( 1.0, 1.0 );
    mvMap.offset.assign( 0.0, 0.0 );
    mvMap.invert = invert;
    mvMap.doGammaCorrection = 1;
    mvMap.useAbsoluteUnits = 0;
    byte isOk = reflectance.setColor( "transmittance.color", mvMap );
	if ( isOk ) return reflectance.setActiveColor( "transmittance.color", mvMap ); 
	return isOk;
}
//==============================================================================
//==============================================================================
byte setBsdfRoughness ( 
					Cmaxwell::Cbsdf& bsdf, 
					real& roughness )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_VALUE;
    mvMap.value = roughness;
    byte isOk = bsdf.setColor( "roughness", mvMap );
    if ( isOk ) return bsdf.setActiveColor( "roughness", mvMap ); 
	return isOk;
}
//==============================================================================
//==============================================================================
byte setBsdfRoughnessMap ( 
					Cmaxwell::Cbsdf& bsdf, 
					char* roughnessMapPath,
					real roughness,
					dword uvChannelIndex )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
    mvMap.pFileName = roughnessMapPath;
    mvMap.value = roughness;
    mvMap.uvwChannel = uvChannelIndex;
    mvMap.uIsTiled = 1;
    mvMap.vIsTiled = 1;
    mvMap.scale.assign( 1.0, 1.0 );
    mvMap.offset.assign( 0.0, 0.0 );
    mvMap.invert = 0;
    mvMap.doGammaCorrection = 0;
    mvMap.useAbsoluteUnits = 0;
    byte isOk = bsdf.setColor( "roughness", mvMap );
    if (isOk ) return bsdf.setActiveColor( "roughness", mvMap ); 
	return isOk;
} 
//==============================================================================
//==============================================================================
byte setBsdfBumpStrength ( 
					Cmaxwell::Cbsdf& bsdf, 
					real& strength )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_VALUE;
    mvMap.value = strength;
    byte isOk = bsdf.setColor( "bump", mvMap );
    if ( isOk ) return bsdf.setActiveColor( "bump", mvMap ); 
	return isOk;
}
//==============================================================================
//==============================================================================
byte setBsdfBumpMap ( 
					Cmaxwell::Cbsdf& bsdf, 
					char* bumpMapPath,
					real &strength,
					dword uvChannelIndex )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
    mvMap.value = strength;
    mvMap.pFileName = bumpMapPath;
    mvMap.uvwChannel = uvChannelIndex;
    mvMap.uIsTiled = 1;
    mvMap.vIsTiled = 1;
    mvMap.scale.assign( 1.0, 1.0 );
    mvMap.offset.assign( 0.0, 0.0 );
    mvMap.invert = 0;
    mvMap.doGammaCorrection = 0;
    mvMap.useAbsoluteUnits = 0;
    byte isOk = bsdf.setColor( "bump", mvMap );
    if (isOk ) return bsdf.setActiveColor( "bump", mvMap ); 
	return isOk;
}
//==============================================================================
//==============================================================================
byte setBasicWeight ( 
					Cmaxwell::CmaterialBasic& basic, 
					real& weight )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_VALUE;
    mvMap.value = weight;
    byte isOk = basic.setWeight( mvMap );
    if ( isOk ) return basic.setActiveWeight( mvMap ); 
	return isOk;
}
//==============================================================================
//==============================================================================
byte setBasicWeightMap ( 
					Cmaxwell::CmaterialBasic& basic, 
					char* weightMapPath, 
					byte& invertWeightMap, 
					dword& uvChannelIndex )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
    mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
    mvMap.pFileName = weightMapPath;
    mvMap.uvwChannel = uvChannelIndex;
    mvMap.uIsTiled = 1;
    mvMap.vIsTiled = 1;
    mvMap.scale.assign( 1.0, 1.0 );
    mvMap.offset.assign( 0.0, 0.0 );
    mvMap.invert = invertWeightMap;
    mvMap.doGammaCorrection = 0;
    mvMap.useAbsoluteUnits = 0;
    byte isOk = basic.setWeight( mvMap );
    if (isOk ) return basic.setActiveWeight( mvMap ); 
	return isOk;
}
//==============================================================================
//==============================================================================
//==============================================================================
//==============================================================================
byte setEmitterPairColorRGBWattsAndBla (
						Cmaxwell::CmaterialEmitter& emitter,
						Crgb& rgb,
						real& watts,
						real& luminousEfficacy
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "rgb";
	emitterActivePair.pLuminance ="wattsAndLuminousEfficacy";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.rgb.assign( rgb );
  emitterPair.watts = watts;
  emitterPair.luminousEfficacy = luminousEfficacy;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorRGBLuminousPower (
						Cmaxwell::CmaterialEmitter& emitter,
						Crgb& rgb,
						real& lumens
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "rgb";
	emitterActivePair.pLuminance ="luminousPower";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.rgb.assign( rgb );
  emitterPair.luminousPower = lumens;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorRGBIlluminance (
						Cmaxwell::CmaterialEmitter& emitter,
						Crgb& rgb,
						real& lux
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "rgb";
	emitterActivePair.pLuminance ="illuminance";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.rgb.assign( rgb );
  emitterPair.illuminance = lux;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorRGBLuminousIntensity (
						Cmaxwell::CmaterialEmitter& emitter,
						Crgb& rgb,
						real& candelas
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "rgb";
	emitterActivePair.pLuminance ="luminousIntensity";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.rgb.assign( rgb );
  emitterPair.luminousIntensity = candelas;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorRGBLuminance (
						Cmaxwell::CmaterialEmitter& emitter,
						Crgb& rgb,
						real& candelasm2
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "rgb";
	emitterActivePair.pLuminance ="luminance";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
	emitterPair.rgb.assign( rgb );
	emitterPair.luminance = candelasm2;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorXYZWattsAndBla (
						Cmaxwell::CmaterialEmitter& emitter,
						Cxyz& xyz,
						real& watts,
						real& luminousEfficacy
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "xyz";
	emitterActivePair.pLuminance ="wattsAndLuminousEfficacy";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
	emitterPair.xyz.assign( xyz );
	emitterPair.watts = watts;
	emitterPair.luminousEfficacy = luminousEfficacy;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorXYZLuminousPower (
						Cmaxwell::CmaterialEmitter& emitter,
						Cxyz& xyz,
						real& lumens
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "xyz";
	emitterActivePair.pLuminance ="luminousPower";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
	emitterPair.xyz.assign( xyz );
	emitterPair.luminousPower = lumens;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorXYZIlluminance (
						Cmaxwell::CmaterialEmitter& emitter,
						Cxyz& xyz,
						real& lux
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "xyz";
	emitterActivePair.pLuminance ="illuminance";
  
	Cmaxwell::CmultiValue::CemitterPair emitterPair;
	emitterPair.xyz.assign( xyz );
	emitterPair.illuminance = lux;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorXYZLuminousIntensity (
						Cmaxwell::CmaterialEmitter& emitter,
						Cxyz& xyz,
						real& candelas
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "xyz";
	emitterActivePair.pLuminance ="luminousIntensity";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.xyz.assign( xyz );
  emitterPair.luminousIntensity = candelas;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorXYZLuminance (
						Cmaxwell::CmaterialEmitter& emitter,
						Cxyz& xyz,
						real& candelasm2
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "xyz";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.xyz.assign( xyz );
  emitterPair.luminance = candelasm2;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorTemperatureWattsAndBla (
						Cmaxwell::CmaterialEmitter& emitter,
						real& temperature,
						real& watts,
						real& luminousEfficacy
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "temperature";
	emitterActivePair.pLuminance ="wattsAndLuminousEfficacy";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.temperature = temperature;
  emitterPair.watts = watts;
  emitterPair.luminousEfficacy = luminousEfficacy;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorTemperatureLuminousPower (
						Cmaxwell::CmaterialEmitter& emitter,
						real& temperature,
						real& lumens
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "temperature";
	emitterActivePair.pLuminance ="luminousPower";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.temperature = temperature;
  emitterPair.luminousPower = lumens;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorTemperatureIlluminance (
						Cmaxwell::CmaterialEmitter& emitter,
						real& temperature,
						real& lux
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "temperature";
	emitterActivePair.pLuminance ="illuminance";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.temperature = temperature;
  emitterPair.illuminance = lux;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorTemperatureLuminousIntensity (
						Cmaxwell::CmaterialEmitter& emitter,
						real& temperature,
						real& candelas
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "temperature";
	emitterActivePair.pLuminance ="luminousIntensity";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.temperature = temperature;
  emitterPair.luminousIntensity = candelas;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte setEmitterPairColorTemperatureLuminance (
						Cmaxwell::CmaterialEmitter& emitter,
						real& temperature,
						real& candelasm2
						)
{
	Cmaxwell::CmaterialEmitter::Cpair emitterActivePair;
	emitterActivePair.pEmissionColor = "temperature";
	emitterActivePair.pLuminance ="luminance";

	Cmaxwell::CmultiValue::CemitterPair emitterPair;
  emitterPair.temperature = temperature;
  emitterPair.luminance = candelasm2;

	byte isOk = emitter.setPair ( emitterPair );
	if ( isOk ) return emitter.setActivePair ( emitterActivePair );
	return isOk;
}
//==============================================================================
//==============================================================================
byte getEmitterTypeAndParams ( Cmaxwell::CmaterialEmitter& emitter, char* outStr )
{
	Cmaxwell::CmultiValue mValue;
	Cmaxwell::CmaterialEmitter::Cpair eAcPair;
	Cmaxwell::CmultiValue::CemitterPair ePair;

	if ( !outStr )
		outStr = new char[ 256 ];

	byte isOk = emitter.getActiveMultiValue( mValue );
	if ( isOk ) isOk = emitter.getPair ( ePair );
	else 
		return 0;
	if ( isOk ) isOk = emitter.getActivePair ( eAcPair );
	else
		return 0;
	if ( !strcmp ( mValue.pType, "pair" ) )
	{//color = "rgb", "xyz", "temperature"
		sprintf ( outStr, "pair" );
		if ( !strcmp ( eAcPair.pEmissionColor, "rgb" ) )
			sprintf ( outStr, "%s.%s.%f.%f.%f", outStr, eAcPair.pEmissionColor, 
																				ePair.rgb.r, ePair.rgb.g, ePair.rgb.b );
		else if ( !strcmp ( eAcPair.pEmissionColor, "xyz" ) )
			sprintf ( outStr, "%s.%s.%f.%f.%f", outStr, eAcPair.pEmissionColor, 
																				ePair.xyz.x, ePair.xyz.y, ePair.xyz.z );
		else if ( !strcmp ( eAcPair.pEmissionColor, "temperature" ) )
			sprintf ( outStr, "%s.%s.%f", outStr, eAcPair.pEmissionColor, ePair.temperature );
		else
			return 0;

		if ( !strcmp ( eAcPair.pLuminance, "wattsAndLuminousEfficacy" ) )
			sprintf ( outStr, "%s.%s.%f.%f", outStr, eAcPair.pLuminance, ePair.watts, ePair.luminousEfficacy );
		else if ( !strcmp ( eAcPair.pLuminance, "luminance" ) )
			sprintf ( outStr, "%s.%s.%f", outStr, eAcPair.pLuminance, ePair.luminance );
		else if ( !strcmp ( eAcPair.pLuminance, "luminousIntensity" ) )
			sprintf ( outStr, "%s.%s.%f", outStr, eAcPair.pLuminance, ePair.luminousIntensity );
		else if ( !strcmp ( eAcPair.pLuminance, "illuminance" ) )
			sprintf ( outStr, "%s.%s.%f", outStr, eAcPair.pLuminance, ePair.illuminance );
		else if ( !strcmp ( eAcPair.pLuminance, "luminousPower" ) )
			sprintf ( outStr, "%s.%s.%f", outStr, eAcPair.pLuminance, ePair.luminousPower );
		else
			return 0;

		return 1;
	}
	else if ( !strcmp ( mValue.pType, "temperature" ) )
	{
		real temperature;
		isOk = emitter.getTemperature ( temperature );
		if ( !isOk )
			return 0;
		sprintf ( outStr, "temperature.%f", temperature );
			return 1;
	}
	else if ( !strcmp ( mValue.pType, "mxi" ) )
	{
		Cmaxwell::CmultiValue::Cmap map;
		isOk = emitter.getMXI ( map );
		if ( !isOk )
			return 0;
		sprintf ( outStr, "mxi.%s", map.pFileName );
			return 1;
	}
	else if ( !strcmp ( mValue.pType, "file" ) )
	{
		return 0;
	}
	else
	{
		return 0;
	}
	return 0;
}
//==============================================================================
//==============================================================================
byte setEmitterInputToPair ( Cmaxwell::CmaterialEmitter& emitter )
{
	Cmaxwell::CmultiValue mv0;
	mv0.pType = "pair";
	return emitter.setActiveMultiValue ( mv0 );
}
//==============================================================================
//==============================================================================
byte setEmitterInputToTemperature ( Cmaxwell::CmaterialEmitter& emitter )
{
	Cmaxwell::CmultiValue mv0;
	mv0.pType = "temperature";
	return emitter.setActiveMultiValue ( mv0 );
}
//==============================================================================
//==============================================================================
byte setEmitterInputToMXI ( Cmaxwell::CmaterialEmitter& emitter )
{
	Cmaxwell::CmultiValue mv0;
	mv0.pType = "mxi";
	return emitter.setActiveMultiValue ( mv0 );
}
//==============================================================================
//==============================================================================
byte setEmitterInputToFile ( Cmaxwell::CmaterialEmitter& emitter )
{
	Cmaxwell::CmultiValue mv0;
	mv0.pType = "file";
	return emitter.setActiveMultiValue ( mv0 );
}
//==============================================================================
//==============================================================================
byte getPresetTemperature( 
					const char *pType, 
					real& temperature )
{

    if ( pType == NULL || _stricmp( pType, "" ) == 0 )
    {
        return 0;
    }
    else if ( _stricmp( pType, "A" ) == 0 ) 
    {
        temperature = 2856.0;
    }
    else if ( _stricmp( pType, "B" ) == 0 ) 
    {
        temperature = 4900.0;
    }
    else if ( _stricmp( pType, "C" ) == 0 ) 
    {				    
        temperature = 6800.0;
    }
    else if ( _stricmp( pType, "COOL WHITE" ) == 0 ) 
    {				    
        temperature = 3400.0;
    }
    else if ( _stricmp( pType, "D65" ) == 0 ) 
    {
        temperature = 6500.0;
    }
    else if ( _stricmp( pType, "D75" ) == 0 ) 
    {			
        temperature = 7500.0;
    }
    else
    {
        return 0;
    }
	return 1;
}
//==============================================================================
//==============================================================================
byte	setEmitterMxi(
					Cmaxwell::CmaterialEmitter& emitter,
					char *pMxiPath, 
					dword uvwChannel )
{
	Cmaxwell::CmultiValue::Cmap mvMap;
	mvMap.pFileName = pMxiPath;
	mvMap.uvwChannel = uvwChannel;
	mvMap.uIsTiled = 1;
	mvMap.vIsTiled = 1;
	mvMap.scale.assign ( 1.0, 1.0 );
	mvMap.offset.assign ( 0.0, 0.0 );
	return emitter.setMXI( mvMap );
}
//==============================================================================
//==============================================================================
byte	Cmaxwell::CmaterialEmitter::setMxi( 
					char *pMxiPath, 
					dword uvwChannel )
{    
	return setEmitterMxi( *this, pMxiPath, uvwChannel );
}
//==============================================================================
//==============================================================================
byte	Cmaxwell::Cmaterial::setDiffuse( 
					Crgb &reflectanceRGB, 
					char *pReflectancePath, 
					dword reflectanceChannelUVW, 
					char *pBumpPath, 
					dword bumpChannelUVW )
{
/*
    free( );
    CmaterialBasic      basic = addBasic( );
    Cbsdf               bsdf = basic.createBsdf( );            
    Creflectance        reflectance = bsdf.getReflectance( );

    CmultiValue         mv;

    mv.uIsTiled = 1;
    mv.vIsTiled = 1;
    mv.scale.assign( 1.0, 1.0 );
    mv.offset.assign( 0.0, 0.0 );
    mv.invert = 0;
    mv.doGammaCorrection = 1;
    mv.useAbsoluteUnits = 0;

    mv.pID = "color";
    if ( pReflectancePath == NULL )
    {
        mv.pType = "rgb";
        mv.pParameter = &reflectanceRGB;
    }
    else
    {   
        mv.pType = "bitmap";
        mv.pFileName = pReflectancePath;
        mv.uvwChannel = reflectanceChannelUVW;
    }
     
    reflectance.setMultiValue( mv );

    if ( pBumpPath != NULL )
    {
        mv.pID = "bump";
        mv.pType = "bitmap";
        mv.pFileName = pBumpPath;
        mv.uvwChannel = bumpChannelUVW;

        bsdf.setMultiValue( mv );
    }
*/
    return 1;
}
//==============================================================================
//==============================================================================
byte	Cmaxwell::Cmaterial::setPlastic( 
					Crgb &reflectanceLambertian, 
					Crgb &reflectanceGlossy, 
					real roughnessGlossy,
					char *pReflectancePath, 
					dword reflectanceChannelUVW,
					char *pReflectanceGlossyPath, 
					dword reflectanceGlossyChannelUVW,
					char *pBumpPath, 
					dword bumpChannelUVW,
					char *pRoughnessPath, 
					dword roughnessChannelUVW )
{
/*    CmaterialBasic      basic;
    Cbsdf               bsdf;
    Creflectance        reflectance;
    CmultiValue         mv;

    mv.uIsTiled = 1;
    mv.vIsTiled = 1;
    mv.scale.assign( 1.0, 1.0 );
    mv.offset.assign( 0.0, 0.0 );
    mv.invert = 0;
    mv.doGammaCorrection = 1;
    mv.useAbsoluteUnits = 0;

	free( );

    setBasicCombinateMode( "ADDITIVE" );
    basic = addBasic( );
    
    // LAMBERTIAN BSDF
    
    bsdf = basic.createBsdf( );
    reflectance = bsdf.getReflectance( );

    mv.pID = "color";
    if ( pReflectancePath == NULL )
    {
        mv.pType = "rgb";
        mv.pParameter = &reflectanceLambertian;
    }
    else
    {   
        mv.pType = "bitmap";
        mv.pFileName = pReflectancePath;                    
        mv.uvwChannel = reflectanceChannelUVW;
    }

    reflectance.setMultiValue( mv );

    if ( pBumpPath != NULL )
    {
        mv.pID = "bump";
        mv.pType = "bitmap";
        mv.pFileName = pBumpPath;
        mv.uvwChannel = bumpChannelUVW;

        bsdf.setMultiValue( mv );
    }

    // GLOSSY BSDF

    bsdf = basic.createBsdf( );
    reflectance = bsdf.getReflectance( );

    mv.pID = "color";
    if ( pReflectanceGlossyPath == NULL )
    {
        mv.pType = "rgb";
        mv.pParameter = &reflectanceGlossy;
    }
    else
    {
        mv.pType = "bitmap";
        mv.pFileName = pReflectanceGlossyPath;
        mv.uvwChannel = reflectanceGlossyChannelUVW;
    }

    reflectance.setMultiValue( mv );

    if ( pBumpPath != NULL )
    {
        mv.pID = "bump";
        mv.pType = "bitmap";
        mv.pFileName = pBumpPath;
        mv.uvwChannel = bumpChannelUVW;

        bsdf.setMultiValue( mv );
    }

    mv.pID = "roughness";
    if ( pRoughnessPath != NULL )
    {
        roughnessGlossy = 100.0;
        mv.pType = "bitmap";
        mv.pFileName = pRoughnessPath;
        mv.uvwChannel = roughnessChannelUVW;
        bsdf.setMultiValue( mv );
    }
    
    mv.pType = "value";
    mv.pParameter = &roughnessGlossy;
    bsdf.setMultiValue( mv );
*/
    return 1;
}
//==============================================================================
//==============================================================================
byte	Cmaxwell::Cmaterial::setMetal( 
					Crgb &reflectanceGlossy, 
					real roughnessGlossy,
					char *pReflectanceGlossyPath, 
					dword reflectanceGlossyChannelUVW,
                    char *pBumpPath, 
					dword bumpChannelUVW,
					char *pRoughnessPath, 
					dword roughnessChannelUVW )

{
/*
    CmaterialBasic      basic;
    Cbsdf               bsdf;
    Creflectance        reflectance;
    CmultiValue         mv;

    mv.uIsTiled = 1;
    mv.vIsTiled = 1;
    mv.scale.assign( 1.0, 1.0 );
    mv.offset.assign( 0.0, 0.0 );
    mv.invert = 0;
    mv.doGammaCorrection = 1;
    mv.useAbsoluteUnits = 0;

	free( );

    basic = addBasic( );
  
    // GLOSSY BSDF

    bsdf = basic.createBsdf( );
    reflectance = bsdf.getReflectance( );

    mv.pID = "color";
    if ( pReflectanceGlossyPath == NULL )
    {
        mv.pType = "rgb";
        mv.pParameter = &reflectanceGlossy;
    }
    else
    {
        mv.pType = "bitmap";
        mv.pFileName = pReflectanceGlossyPath;
        mv.uvwChannel = reflectanceGlossyChannelUVW;
    }

    reflectance.setMultiValue( mv );

    if ( pBumpPath != NULL )
    {
        mv.pID = "bump";
        mv.pType = "bitmap";
        mv.pFileName = pBumpPath;
        mv.uvwChannel = bumpChannelUVW;

        bsdf.setMultiValue( mv );
    }

    mv.pID = "roughness";
    if ( pRoughnessPath != NULL )
    {
        roughnessGlossy = 100.0;
        mv.pType = "bitmap";
        mv.pFileName = pRoughnessPath;
        mv.uvwChannel = roughnessChannelUVW;
        bsdf.setMultiValue( mv );
    }
    
    mv.pType = "value";
    mv.pParameter = &roughnessGlossy;
    bsdf.setMultiValue( mv );
*/
    return 1;
}
//==============================================================================
//==============================================================================
byte	Cmaxwell::Cmaterial::setDielectric( 
					Crgb &reflectanceGlossy, 
					Crgb &transmittanceGlossy,
					real absorptionDistance, 
					real roughnessGlossy,
					char *pReflectanceGlossyPath, 
					dword reflectanceGlossyChannelUVW,
					char *pTransmittanceGlossyPath, 
					dword transmittanceGlossyChannelUVW,
					char *pBumpPath, 
					dword bumpChannelUVW,
					char *pRoughnessPath, 
					dword roughnessChannelUVW )
{
 /*
    CmaterialBasic      basic;
    Cbsdf               bsdf;
    Creflectance        reflectance;
    CmultiValue         mv;   

    if ( setMetal( reflectanceGlossy, 
				   roughnessGlossy, 
				   pReflectanceGlossyPath, reflectanceGlossyChannelUVW,
                   pBumpPath, bumpChannelUVW, 
				   pRoughnessPath, roughnessChannelUVW ) == 0 )
    {
        return 0;
    }    
    
    basic = getBasic( 0 );
    bsdf = basic.getBsdf( );    
    reflectance = bsdf.getReflectance( );

    reflectance.setAbsorptionDistance( "meters", absorptionDistance );
    
    mv.pID = "transmittance.color";
    if ( pTransmittanceGlossyPath == NULL )
    {
        mv.pType = "rgb";
        mv.pParameter = &transmittanceGlossy;
    }
    else
    {
        mv.pType = "bitmap";
        mv.pFileName = pTransmittanceGlossyPath;
        mv.uvwChannel = transmittanceGlossyChannelUVW;
        mv.uIsTiled = 1;
        mv.vIsTiled = 1;
        mv.scale.assign( 1.0, 1.0 );
        mv.offset.assign( 0.0, 0.0 );
        mv.invert = 0;
        mv.doGammaCorrection = 1;
        mv.useAbsoluteUnits = 0;
    }

    reflectance.setMultiValue( mv );
*/
    return 1;
}
//==============================================================================
//==============================================================================
#endif