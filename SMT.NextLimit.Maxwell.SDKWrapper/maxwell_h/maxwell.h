/////////////////////////////////////////////////////////////////
//              MAXWELL RENDER SDK                             //
/////////////////////////////////////////////////////////////////
// (c) 1998-2007 Next Limit Technologies, all rights reserved  //
/////////////////////////////////////////////////////////////////


#ifndef __MAXWELL_H
#define __MAXWELL_H

#include <float.h>
#include "mxvector.h"
#include "base.h"
#include "color.h"
#include "flags.h"

class Cscene;
class CrenderParameters;
class Cmaterial;

class Cmaxwell
{
public:

    class   Cgroup;
    class   Ccluster;
    class   Cobject;
    class   Ccamera;
    class   Cmaterial;
    class   CmaterialEmitter;
    class   CmaterialBasic;
    class   Creflectance;
    class   Ccoating;
    class   Cbsdf;

    friend class   Cgroup;
    friend class   Ccluster;
    friend class   Cobject;
    friend class   Ccamera;
    friend class   Cmaterial;
    friend class   CmaterialEmitter;
    friend class   CmaterialBasic;
    friend class   Creflectance;
    friend class   Ccoating;
    friend class   Cbsdf;    

private:

    ///////////////////////////////////////////////////////
    // Internal SDK data: [Not needed for plugins]
    bool    check__( const char *pString );
    void    modifyResolutionIfNeeded( dword &xRes, dword &yRes );
    ///////////////////////////////////////////////////////

    ///////////////////////////////////////////////////////
    friend class CmaxwellRender;
    friend class CrenderOptions;
    friend class Cmxcl;
    friend class Cscene;
    friend class CmaxwellUI;
    ///////////////////////////////////////////////////////

    //////////////////
    //    MEMBERS   //
    //////////////////

    CrenderParameters *     pRenderParameters;
    Cscene *                pScene;
    //Cflags<byte>            flags;
    bool                    successfullyInitiated;

    /////////////////////////////
    //    PRIVATE SUBCLASSES   //
    //  (Not used in plugins)  //
    /////////////////////////////
    class   Cpointer
    {
        friend class Cmaxwell;
        friend class Cgroup;
        friend class Ccluster;
        friend class Cobject;
        friend class Ccamera;
        friend class Cmaterial;
        friend class CmaterialEmitter;
        friend class CmaterialBasic;
        friend class Creflectance;
        friend class Ccoating;
        friend class Cbsdf;

        Cmaxwell *      pMaxwell;
        void *          pData;

        public:

        Cpointer( )
        {
            pMaxwell = NULL;
            pData = NULL;
        }

        bool    isNull( ) const
        {
            return ( pData == NULL );
        }

        void *  getPointer( ) const
        {
            return pData;
        }
    };

    class   CmaterialPointer: public Cpointer
    {
        friend class Cmaterial;
        friend class CmaterialEmitter;
        friend class CmaterialBasic;
        friend class Creflectance;
        friend class Ccoating;
        friend class Cbsdf;
    
        ::Cmaterial *     pMaterial;

    public:

        CmaterialPointer( )
        {
            pMaterial = NULL;
        }
    };
    
public:


    ///////////////////////////////
    //     PUBLIC SUBCLASSES     //
    //     ( Used in plugins)    //
    ///////////////////////////////

    // READ MXS OPTIONS class
    class CoptionsReadMXS: public Cflags<word>
    {
    public:

        enum
        {
            READ_RENDER_OPTIONS             = 0x0001,
            READ_CAMERAS                    = 0x0002,
            READ_ENVIRONMENT                = 0x0004,
            READ_GEOMETRY                   = 0x0008,
            READ_MATERIALS                  = 0x0010,
            READ_ALL                        = 0x001F,
            GEOMETRY_CONFLICT_RENAME_NEW    = 0x0000,
            GEOMETRY_CONFLICT_REMAIN_FIRST  = 0x0020,
            GEOMETRY_CONFLICT_OVERWRITE     = 0x0040,
            GEOMETRY_CONFLICT_ASK           = 0x0060,
            GEOMETRY_CONFLICT_FLAGS         = 0x0060,
            MATERIAL_CONFLICT_RENAME_NEW    = 0x0000,
            MATERIAL_CONFLICT_REMAIN_FIRST  = 0x0080,
            MATERIAL_CONFLICT_OVERWRITE     = 0x0100,
            MATERIAL_CONFLICT_ASK           = 0x0180,
            MATERIAL_CONFLICT_FLAGS         = 0x0180,
        };

        CoptionsReadMXS( )
        {
            enable( READ_ALL );
        }

        void  setMaterialConflict( const dword flags )
        {
            enableMask( MATERIAL_CONFLICT_FLAGS, flags );
        }

        void  setGeometryConflict( const dword flags )
        {
            enableMask( GEOMETRY_CONFLICT_FLAGS, flags );
        }
    };



    /////////////////////////////
    //     MATERIALS CLASSES   //
    /////////////////////////////
    class   CmultiValue
    {
        // Cmaxwell::CmultiValue is a container that can store different values for the same parameter
        // It is widely used to read / write materials. 
        // In example an IOR parameter can be set in two ways:
        // 1) As a pair of real numbers Nd(1.33) and Abbe(166)
        //    In this case pType = "nd,abbe";
        // 2) As a file (char*) that contains the IOR info
        //    In this case pType = "file";
        // In both cases multivalue.pID = "ior.real";


    friend class Cmaxwell;
    friend class CmaterialBasic;
    friend class Cbsdf;
    friend class Ccoating;
    friend class Creflectance;
        
    public:

        class   Cmap;
        class   CemitterPair;

        const char *    pID;
        const char *    pType;
        void *          pParameter;

        CmultiValue( const char *_pID = NULL, const char *_pType = NULL, void *_pParameter = NULL )
        {
            pID = _pID;
            pType = _pType;
            pParameter = _pParameter;
        }
    };
    
    
    class Cmaterial: public Cpointer
    {
    // Main material class.

    friend class Cmaxwell;
    friend class Cobject;
    friend class Cgroup;
    friend class Ccluster;

        bool    check__( const char *pString );
        void    setPointers( Cmaxwell *pMaxwell, void *pData );

    public:        

        //      read

        class   Citerator // Used to parse a list of materials
        {
            Cmaxwell *  pMaxwell;
            void *      pData;

        public:
            
            Citerator( );
            ~Citerator( );
            Cmaterial   first( Cmaxwell *pMaxwell );
            Cmaterial   next( );
        };

        Cmaterial           createCopy( );
        byte                free( ); // return 0->failed, 1->success ( removed )

        static byte         getVersion( const char *pFileName, float &version );

        ////////////////////////////////////////////////////////////////////
        //       OpenGL display (obsolete, [not used in plugins])           //
        byte                setRgbs( Crgb &rgbDiffuse, Crgb &rgbSpecular );
        byte                getRgbs( Crgb &rgbDiffuse, Crgb &rgbSpecular );
        ////////////////////////////////////////////////////////////////////

        byte                setName( const char *pName );
        const char *        getName( );
        const char *        getFileName( ); // NULL if the material is embed in a scene
        byte                setDescription( const char *pDescription );
        const char *        getDescription( );
        byte                forceToWriteIntoScene( ); // NOTE: this Material will be write into the MXS
        byte                isEmpty( byte &empty );
        byte                setEmpty( ); // Material not null but with no layers
        byte                belongToScene( bool &belong ); // Embed in a mxs scene
        byte                setDispersion( bool enabled ); // Dispersion OFF by default
        byte                getDispersion( bool &enabled );
        byte                setMatte( bool enabled ); // Matte OFF by default
        byte                getMatte( bool &enabled );
        byte                setMatteShadow( bool enabled ); // Matte Shadow OFF by default
        byte                getMatteShadow( bool &enabled );
        byte                setEditionMode( dword editionMode );
        byte                getEditionMode( dword &editionMode );

        byte                setDisplacement( CmultiValue::Cmap &map, real gain, real offset, real height, real smoothness, bool absoluteHeight, bool automaticGain );
        byte                getDisplacement( CmultiValue::Cmap &map, real &gain, real &offset, real &height, real &smoothness, bool &absoluteHeight, bool &automaticGain );
        byte                enableDisplacement( bool enable );
        byte                isDisplacementEnabled( bool &enabled );

        // MODE -> "BLENDING", "ADDITIVE"
        byte                setBasicCombinateMode( const char *pMode );
        const char *        getBasicCombinateMode( );

        ////////////
        // BASICS //
        ////////////
        CmaterialBasic      addBasic( dword index = 0xFFFFFFFF );
        byte                moveBasic( dword index1, dword index2 );
        CmaterialBasic      getBasic( dword index );
        byte                freeBasic( dword index );
        byte                getNumBasics( dword &nBasics );

        //////////////////////////////////////////////////////////////////////////////
        // EMBED MATERIALS (materials inside materials, [not used in plugins])
        byte                addLink( const char *pFileName );
        const char *        getLink( dword index );
        byte                freeLink( dword index );
        byte                getNumLinks( dword &nBasics );
        //////////////////////////////////////////////////////////////////////////////

        byte                write( const char *pFileName ); // Writes the material to disk

        ////////////////////////
        // EMITTERS FUNCTIONS //
        ////////////////////////

        CmaterialEmitter    createEmitter( );
        CmaterialEmitter    getEmitter( );
        byte                freeEmitter( );

        ///////////////////////
        // PREVIEW FUNCTIONS //
        ///////////////////////
        byte                setPreview( dword xRes, dword yRes, Crgb8 *pRGB );
        Crgb8 *             getPreview( dword &xRes, dword &yRes );

        byte                setTextureActive( const CmultiValue::Cmap &map );
        byte                getTextureActive( CmultiValue::Cmap &map );
        // If there is no active texture, the SDK returns the first map found
        // If there are no maps in the material, map.pFileName returns NULL

        dword               getNumberOfChannelsNeeded( );

        //////////////////////////////////////////////////////////////////////////////////////////////////
        // PRESET FUNCTIONS 
        // Auxiliar functions to create basic materials. 
        byte    setDiffuse( Crgb &reflectanceRGB, char *pReflectancePath, dword reflectanceChannelUVW, 
                            char *pBumpPath, dword bumpChannelUVW );
        
        byte    setPlastic( Crgb &reflectanceLambertian, Crgb &reflectanceGlossy, real roughnessGlossy,
                            char *pReflectancePath, dword reflectanceChannelUVW,
                            char *pReflectanceGlossyPath, dword reflectanceGlossyChannelUVW,
                            char *pBumpPath, dword bumpChannelUVW,
                            char *pRoughnessPath, dword roughnessChannelUVW );

        byte    setMetal( Crgb &reflectanceGlossy, real roughnessGlossy,
                          char *pReflectanceGlossyPath, dword reflectanceGlossyChannelUVW,
                          char *pBumpPath, dword bumpChannelUVW,
                          char *pRoughnessPath, dword roughnessChannelUVW );

        byte    setDielectric( Crgb &reflectanceGlossy, Crgb &transmittanceGlossy, 
                               real absorptionDistance, real roughnessGlossy,
                               char *pReflectanceGlossyPath, dword reflectanceGlossyChannelUVW,
                               char *pTransmittanceGlossyPath, dword transmittanceGlossyChannelUVW,
                               char *pBumpPath, dword bumpChannelUVW,
                               char *pRoughnessPath, dword roughnessChannelUVW );
        //////////////////////////////////////////////////////////////////////////////////////////////////
    };

    class   CmaterialEmitter: public CmaterialPointer
    {
        ////////////////////
        //  EMITTER LAYER //
        ////////////////////
        // A material can contain an emitter in the top (just one per material)
        // The emitter layer is created calling Cmaterial::createEmitter()

        bool    check__( const char *pString );

    public:

        byte    setState( bool enabled ); // To switch on/off the emitter
        byte    getState( bool &enabled );
        
        // When setting "mxi" values use:                
        byte    setMXI( CmultiValue::Cmap &map ); // map.type will be assumed as TYPE_BITMAP
        byte    getMXI( CmultiValue::Cmap &map ); // map.type will be assumed as TYPE_BITMAP
        byte    setMxi( char *pPathMXI, dword uvwChannel ); // setMXI simplified

        // When setting "temperature" value use:
        byte    setTemperature( real temperature );
        byte    getTemperature( real &temperature );

        // When setting "pair" values use:
        byte    setPair( CmultiValue::CemitterPair &pair );
        byte    getPair( CmultiValue::CemitterPair &pair );
        
        // to enable or check witch is enabled use:
        byte    setActiveMultiValue( CmultiValue &mv ); // input->   type = "mxi", "temperature", "pair"
        byte    getActiveMultiValue( CmultiValue &mv ); // output->  type == "mxi", "temperature", "pair"        

        class Cpair
        {
        public:

            const char *    pEmissionColor; // "rgb", "xyz", "temperature"
            const char *    pLuminance;     // "wattsAndLuminousEfficacy, "luminousPower", "illuminance", "luminousIntensity", "luminance"
        };
        // Set active pair is used to set the active types for CmultiValue::CemitterPair
        // Read CmultiValue::CemitterPair declaration to know how to fill a CemitterPair struct 
        byte    setActivePair( Cpair &pair );
        byte    getActivePair( Cpair &pair );
    };

    class   CmaterialBasic: public CmaterialPointer
    {
        ////////////////////
        //   BASIC LAYER  //
        ////////////////////
        
        // In example, to add a basic to a material (material created with Cmaterial

        //Cmaxwell::CmaterialBasic newBasic = material.addBasic( i );    
        //isOk = newBasic.setName( currentBasic->layerName.toAscii().constData() ); 

        // To set the weight as a number
        //mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_VALUE;
        //mvMap.value = 52.3;
        //newBasic.setWeight( mvMap );

        // To set the weight as a bitmap
        //mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
        //mvMap.pFileName = mapPath;
        //mvMap.uvwChannel = mapData.channel;
        //mvMap.uIsTiled = mapData.tileU;
        // ...etc...
        //newBasic.setWeight( mvMap );
        // Set the bitmap as the active value for weight parameter
        //newBasic.setActiveWeight( mvMap );

      bool    check__( const char *pString );

    public:     

        byte            setState( bool enabled ); // To switch on/off the basic layer
        byte            getState( bool &enabled );
        byte            setName( const char *pName );
        byte            getName( char **pName );
        
        byte            setWeight( CmultiValue::Cmap &map );
        byte            getWeight( CmultiValue::Cmap &map );
        
        // map.type = CmultiValue::Cmap::TYPE_VALUE, TYPE_BITMAP
        byte            setActiveWeight( CmultiValue::Cmap &map );
        byte            getActiveWeight( CmultiValue::Cmap &map );

        Ccoating        addCoating( dword index = 0xFFFFFFFF );
        byte            moveCoating( dword index1, dword index2 );
        Ccoating        getCoating( dword index );
        byte            freeCoating( dword index );
        byte            getNumCoatings( dword &nCoatings );
        
        Cbsdf           createBsdf( );
        byte            freeBsdf( );
        Cbsdf           getBsdf( );

        // Create SSS elements
        byte            enableSSS( Crgb rgbScattering, real scatteringFactor, Crgb rgbAbsorption, real absorptionFactor, real asymmetryFactor );
        byte            disableSSS( );
        byte            getSSS( bool &enabled, Crgb &rgbScattering, real &scatteringFactor, Crgb &rgbAbsorption, real &absorptionFactor, real &asymmetryFactor );
    };    

    class   Creflectance: public CmaterialPointer
    {
      /// Defines the reflectance of a bsdf or coating (reflectance/transmittance/IOR)
        bool    check__( const char *pString );

    public:

        byte            setActiveIorMode( byte complex );
        byte            getActiveIorMode( byte &complex );
        byte            setComplexIor( const char *pFileName );
        const char *    getComplexIor( );

        byte    setAbsorptionDistance( const char *pType, real distance );
        const char *    getAbsorptionDistance( real &distance );        

        // ** TYPE **
        // "nanometers"
        // "microns"
        // "millimeters"
        // "centimeters"
        // "decimeters"
        // "meters"
  
        // pID = "color.tangential", "color", "transmittance.color"
        byte    setColor( const char *pID, CmultiValue::Cmap &map ); 
        byte    getColor( const char *pID, CmultiValue::Cmap &map );
        
        // map.type = CmultiValue::Cmap::TYPE_RGB, TYPE_BITMAP
        byte    setActiveColor( const char *pID, CmultiValue::Cmap &map );
        byte    getActiveColor( const char *pID, CmultiValue::Cmap &map );
        
        // pType = "nd, abbe" <- real * ( real [2] )
        // pType = "file" <- char *pFileName
        byte    setIOR( CmultiValue &mv );
        byte    getIOR( CmultiValue &mv );
        byte    setActiveIOR( CmultiValue &mv );
        byte    getActiveIOR( CmultiValue &mv );

        byte    getReflectanceAndTransmittance( Crgb &r, Crgb &t, real roughness );
    };

    class   Ccoating: public CmaterialPointer
    {
        bool    check__( const char *pString );

    public:                 

        byte    setState( bool enabled );
        byte    getState( bool &enabled );
        byte    setName( const char *pName );
        byte    getName( char **pName );

        Creflectance    getReflectance( );        

        byte    setThickness( CmultiValue::Cmap &map );
        byte    getThickness( CmultiValue::Cmap &map );
        byte    setActiveThickness( CmultiValue::Cmap &map );
        byte    getActiveThickness( CmultiValue::Cmap &map );
        byte    setThicknessRange( real min, real max );
        byte    getThicknessRange( real &min, real &max );
    };
    
    class   Cbsdf: public CmaterialPointer
    {
        bool    check__( const char *pString );

    public:

        // In example to create a bsdf 
        // and set the color (reflectance at 0º) of a bsdf (with coatings is the same)
        // Cmaxwell::CmultiValue::Cmap mvMap;

        //Cmaxwell::CmaterialBasic newBasic = material.addBasic( 0 );   
        // Cmaxwell::Cbsdf newBsdf = newBasic.createBsdf();
        //Cmaxwell::Creflectance bsdfReflectance = newBsdf.getReflectance();    

        //mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_RGB;
        //mvMap.rgb.assign( color );
        //bsdfReflectance.setColor( "color", mvMap );
        //bsdfReflectance.setActiveColor( "color", mvMap ); //Set RGB as the active type

        // To set a bitmap
        //mvMap.type = Cmaxwell::CmultiValue::Cmap::TYPE_BITMAP;
        //mvMap.pFileName = mapPath;
        //mvMap.uvwChannel = mapData.channel;
        //mvMap.uIsTiled = mapData.tileU;
        // ...etc...
        //bsdfReflectance.setColor( "color", mvMap );
        //bsdfReflectance.setActiveColor( "color", mvMap ); //Set bitmap as the active type

        byte    setName( const char *pName );
        char *  getName( );

        byte    setState( bool enabled );
        byte    getState( bool &enabled );

        Creflectance    getReflectance( );        

        // pID = "// "roughness", "anisotropy", "bump", "angle"
        byte    setColor( const char *pID, CmultiValue::Cmap &map ); 
        byte    getColor( const char *pID, CmultiValue::Cmap &map ); 
        
        // pID = "// "roughness", "anisotropy", "bump", "angle"
        // map.type = CmultiValue::Cmap::TYPE_RGB, TYPE_BITMAP
        byte    setActiveColor( const char *pID, CmultiValue::Cmap &map );
        byte    getActiveColor( const char *pID, CmultiValue::Cmap &map );

        byte    setNormalMapState( bool enabled );
        byte    getNormalMapState( bool &enabled );
    };
    
    class Ccamera: public Cpointer    
    {
		friend class Cmaxwell;    
		friend class Cscene;

        bool    check__( const char *pString );
        void    setPointers( Cmaxwell *pMaxwell, void *pData );

		friend class Citerator;

    public:

        class   Citerator
        {
            Cmaxwell *  pMaxwell;
            void *      pData;

        public:
            
            Citerator( );
            ~Citerator( );
            Ccamera     first( Cmaxwell *pMaxwell );
            Ccamera     next( );
        };

        // setStep sets the camera parameters that can change in an animation
        // The static parameters that are set in Cmaxwell::addCamera can not be animated (resolution, film..)
        byte    setStep( dword iStep, Cpoint origin, Cpoint focalPoint, Cvector up, 
                         real focalLength, real fStop, byte focalLengthNeedCorrection = true );
        ///////////////////////////////////////////////////////////////////////////////////////////
        byte    getStep( dword iStep, Cpoint &origin, Cpoint &focalPoint, Cvector &up, real &focalLength, real &fStop );
        ///////////////////////////////////////////////////////////////////////////////////////////
        const char * getValues( dword &nSteps, real &shutter, real &filmWidth, real &filmHeight, real &iso, 
                                const char **pDiaphragmType, real &angle, dword &nBlades,
                                dword &fps, dword &xRes, dword &yRes, real &pixelAspect );
        ///////////////////////////////////////////////////////////////////////////////////////////
        byte    setCutPlanes( real &zNear, real &zFar, bool enabled );// 0.0 to 1E7.0, 0 by default
        byte    getCutPlanes( real &zNear, real &zFar, bool &enabled );
        ///////////////////////////////////////////////////////////////////////////////////////////
        byte    setShiftLens( real xShift, real yShift ); // -100.0 to 100.0, 0 by default
        byte    getShiftLens( real &xShift, real &yShift ); // -100.0 to 100.0
        ///////////////////////////////////////////////////////////////////////////////////////////
        byte    setHide( bool hide ); // To hide cameras when importing the scene in studio. Default = false
        byte    isHide( bool &hide );
        ///////////////////////////////////////////////////////////////////////////////////////////
        byte    setUserData( void *pData ); // [Not used by plugins]
        byte    getUserData( void **pData ); // [Not used by plugins]
        ///////////////////////////////////////////////////////////////////////////////////////////
        byte    setActive( ); // Sets the active camera when there is more than one
        byte    free( ); // return 0->failed, 1->success ( removed )
    };

    class Cobject: public Cpointer
    {   
    friend class Cmaxwell;
    friend class Cgroup;
    friend class Ccluster;

        bool    check__( const char *pString );
        void    setPointers( Cmaxwell *pMaxwell, void *pData );
        bool    isMaxwellSuccessfullyInitiated( );

    friend class Citerator;

      public:

        ////////////////////////////////////
        // INTERNAL FUNCTIONS
        // IMPORTANT: SetPointer: if pObject belongs to other Cmaxwell than used as 1st parameter consequences are imprevisible, like crashing
        byte    setPointer( Cmaxwell *pMaxwell, void *pObject );
        byte    free( ); // return 0->failed, 1->success ( removed )
        ////////////////////////////////////

        byte    mergeMeshes( const Cobject **pMeshes, dword nMeshes );

        //      write
        byte    setParent( Cobject parent ); // To set hierarchies of objects
        byte    setName( const char *pName );
        byte    setMaterial( Cmaterial material );
        byte    setProperties( byte doDirectCausticsReflection, byte doDirectCausticsRefraction,
                               byte doIndirectCausticsReflection, byte doIndirectCausticsRefraction );
        
        ////////////////////////
        // GEOMETRY FUNCTIONS
        ////////////////////////
        byte    addChannelUVW( dword &index, byte id = 0xFF );
        byte    setVertex( dword iVertex, dword iPosition, const Cpoint &point );
        byte    setNormal( dword iNormal, dword iPosition, const Cvector normal );
                
        byte    setTriangle( dword iTriangle, dword iVertex1, dword iVertex2, dword iVertex3,
                             dword iNormal1, dword iNormal2, dword iNormal3 );
        byte    setTriangleGroup( dword iTriangle, dword idGroup );
        byte    setTriangleUVW( dword iTriangle, dword iChannelID, float u1, float v1, float w1,
                                float u2, float v2, float w2, float u3, float v3, float w3 );
        byte    setTriangleMaterial( dword iTriangle, Cmaterial material );
        byte    setGroupMaterial( dword iGroup, Cmaterial material );
        byte    setBase( Cbase base );
        byte    setPivot( Cbase base );
        byte    setPosition( Cvector vector );
        byte    setRotation( Cvector vector );
        byte    setScale( Cvector vector );
        byte    setPivotPosition( Cvector vector );
        byte    setPivotRotation( Cvector vector );

        ////////////////////////
        // DISPLAY FUNCTIONS
        ////////////////////////
        byte    setHide( bool hide );
        byte    setHideToCamera( bool hide );
        byte    setHideToReflectionsRefractions( bool hide );
        byte    setHideToGI( bool hide );
        byte    excludeOfCutPlanes( bool exclude );

        byte    setUserData( void *pData );// [Not used by plugins]

        byte    setRgbs( Crgb &rgbDiffuse, Crgb &rgbSpecular ); // // [Not used by plugins] NOTE: only for meshes
        byte    getRgbs( Crgb &rgbDiffuse, Crgb &rgbSpecular ); // // [Not used by plugins] NOTE: only for meshes

        //////////////////////
        //  READ FUNCTIONS  //
        //////////////////////

        class   Citerator
        {
            Cmaxwell *  pMaxwell;
            void *      pData;

        public:
            
            Citerator( );
            ~Citerator( );
            Cobject     first( Cmaxwell *pMaxwell );
            Cobject     next( );
        };

        byte        isMesh( byte &isMesh );
        byte        isInstance( byte &isInstance );
        Cobject     getInstanced( );

        byte        getNumVertexes( dword &nVertexes );
        byte        getNumTriangles( dword &nTriangles );
        byte        getNumNormals( dword &nNormals );
        byte        getNumPositionsPerVertex( dword &nPositions );
        byte        getNumChannelsUVW( dword &nChannelsUVW );
        
        byte        getParent( Cobject &parent );
        byte        getName( char **pName );
        byte        getMaterial( Cmaterial &material );
        byte        getProperties( byte &doDirectCausticsReflection, byte &doDirectCausticsRefraction,
                                   byte &doIndirectCausticsReflection, byte &doIndirectCausticsRefraction );

        ////////////////////////////////////////////////////////////////////////////////////////
        byte        getVertex( dword iVertex, dword iPosition, Cpoint &point );
        byte        getNormal( dword iNormal, dword iPosition, Cvector &normal );
                
        byte        getTriangle( dword iTriangle, dword &iVertex1, dword &iVertex2, dword &iVertex3,
                                 dword &iNormal1, dword &iNormal2, dword &iNormal3 );
        byte        getTriangleGroup( dword iTriangle, dword &idGroup );
        byte        getTriangleUVW( dword iTriangle, dword iChannelID, float &u1, float &v1, float &w1,
                                    float &u2, float &v2, float &w2, float &u3, float &v3, float &w3 );
        
        byte        getTriangleMaterial( dword iTriangle, Cmaterial &material );
        byte        getGroupMaterial( dword iGroup, Cmaterial &material );
        
        byte        getBase( Cbase &base ) const;
        byte        getPivot( Cbase &base ) const;
        byte        getPosition( Cvector &vector ) const;
        byte        getRotation( Cvector &vector ) const;
        byte        getScale( Cvector &vector ) const;
        byte        getPivotPosition( Cvector &vector ) const;
        byte        getPivotRotation( Cvector &vector ) const;
        byte        isPosRotScaleInitialized( bool &init ) const;

        ////////////////////////////////////////////////////////////////////////////////////////
        byte        getHide( bool &hide );
        byte        getHideToCamera( bool &hide );
        byte        getHideToReflectionsRefractions( bool &hide );
        byte        getHideToGI( bool &hide );
        byte        isExcludedOfCutPlanes( bool &excluded );
        ////////////////////////////////////////////////////////////////////////////////////////
        byte        getUserData( void **pData );
        

        CfPoint *   getVertexesBuffer( );
        CfPoint *   getNormalsBuffer( );
        void *      getTrianglesBuffer( dword &interleaving );
        //void *      getChannelsUVWBuffer( dword iChannelID );
    };

    class Cgroup: public Cpointer
    {
      // Cgroup is used to group objects. 
      // [IMPORTANT] This class will become obsolete when hierarchies and NULL objects are working
    friend class Cmaxwell;
    friend class CobjectIterator;

        bool    check__( const char *pString );
        void    setPointers( Cmaxwell *pMaxwell, void *pData );
        void    getPointers( Cmaxwell **pMaxwellPointer, void **pDataPointer );

    public:

        class   Citerator
        {
            Cmaxwell *  pMaxwell;
            void *      pData;

        public:
            
            Citerator( );
            ~Citerator( );
            Cgroup  first( Cmaxwell *pMaxwell );
            Cgroup  next( );
        };

        class   CobjectIterator
        {
            Cmaxwell *  pMaxwell;
            void *      pGroup;
            void *      pData;

        public:
            
            CobjectIterator( );
            ~CobjectIterator( );
            const char *    first( Cgroup group );
            const char *    next( );
        };

        const char *    getName( );
        byte            addObject( Cobject object );
        byte            setMaterial( Cmaterial material );
        byte            getMaterial( Cmaterial &material );
    };

    class Ccluster: public Cpointer
    {
      // Ccluster is used to group triangles in an object so the user can recover a triangle selection in Maxwell Studio. 
    friend class Cmaxwell;
    friend class CobjectIterator;

        bool    check__( const char *pString );
        void    setPointers( Cmaxwell *pMaxwell, void *pData );
        void    getPointers( Cmaxwell **pMaxwellPointer, void **pDataPointer );

    public:

        class   Citerator
        {
            Cmaxwell *  pMaxwell;
            void *      pData;

        public:
            
            Citerator( );
            ~Citerator( );
            Ccluster  first( Cmaxwell *pMaxwell );
            Ccluster  next( );
        };

        class   Cobject: public ::Cmaxwell::Cobject
        {
        public:

            dword *         pTriangles;
            dword           nTriangles;

            Cobject( );
            bool            isNull( );
            const char *    getName( );
        };

        class   CobjectIterator
        {
            Cmaxwell *  pMaxwell;
            void *      pCluster;
            void *      pData;

        public:
            
            CobjectIterator( );
            ~CobjectIterator( );
            Cobject     first( Ccluster cluster );
            Cobject     next( );
        };

        const char *    getName( );
        byte            addObject( Cmaxwell::Cobject object, dword *pTriangles, dword nTriangles );
        byte            setMaterial( Cmaxwell::Cmaterial material );
    };

    class Cbitmap: public Cpointer // [NOT USED]
    {
    public:
    };


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
      
    enum typeCallBack // CALLBACK TYPES
    {
        CALLBACK_WARNING            = 0,
        CALLBACK_ERROR              = 1,
        CALLBACK_WRITE_USER_DATA    = 2,
        CALLBACK_READ_USER_DATA     = 3
    };

    // SCENE CONSTRUCTOR
    Cmaxwell( byte ( *pCallBack )( byte type, const char *pMethod, const char *pError, const void *pValue ) );
    ~Cmaxwell( );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////// GENERIC METHODS ///////////////////////////////////////////////  
    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    void    getVersion( char pVersion[64] );    
    float   getMxsVersion( );
    byte    isTextureExtensionSupported( const char *pExtension );
    void    setPluginID( const char *pID ); //Plugin identifier
    const char *    getPluginID( );

    void    enableProtection( bool enable ); // True by default. Disables the ability to export to other formats in Studio
    void    isProtectionEnabled( bool &enabled );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////// SCENE METHODS ///////////////////////////////////////////////    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    
    byte        setPreview( dword xRes, dword yRes, Crgb8 *pRGB ); // Preview of the scene stored in the file
    Crgb8 *     getPreview( dword &xRes, dword &yRes );
    void        freeGeometry( );
    void        freeScene( );
    byte        readParticlesFromDisk( const char *pName );

    // INPUT DATA TYPE
    //"YXZ", "LIGHTWAVE", "CINEMA"
    //"ZXY", "FORMZ", "3DSMAX"
    //"YZX", "MAXWELL", "MAYA", "XSI", "HOUDINI", "RHINO", "SOLIDWORKS", 
    byte        setInputDataType( const char *pInput ); 
    void        setSinglePrecisionOfGeometry( ); // Use floats instead doubles for geometry data
    byte        setSceneUserData( ); // [Not used in plugins] Callback will be called in writemxs writting and reading.

    // Group creation
    Cgroup      getGroup( const char *pName );
    Cgroup      addGroup( const char *pName );
    void        freeGroups( );

    // Clusters (triangle groups) creation
    Ccluster    addCluster( const char *pName );
    void        freeCluster( );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////// OBJECT METHODS ///////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    
//  byte    createUserObject( Cmaterial::Cpointer material, Cpoint &min, Cpoint &max, void *pUserObject, MXW_INTERSECTION_CALLBACK );   
    Cobject     getObject( const char *pObjectName );
    
    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////// MESH METHODS ///////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 
    
    // If nVertes, nNormals and nTriangles are equal to 0, a NULL object is created
    Cobject     createMesh( const char *pName, dword nVertexes, dword nNormals, dword nTriangles, dword nPositionsPerVertex );
    Cobject     mergeMeshes( const char *pName, const Cobject **pMeshes, dword nMeshes );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////// INSTANCEMENT METHODS //////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 

    Cobject     createInstancement( const char *pName, Cobject &object, Cbase &base ); // NOTE: object must be a mesh
 
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 
    ////////////////////////////////////// CAMERA METHODS ///////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 
    
    byte    createCamera( dword nCamerasPositions, real shutter, real filmWidth, real filmHeight, real iso, 
                          const char *pDiaphragmType, real angle, dword nBlades,
                          dword fps, dword xRes, dword yRes, real pixelAspect );
    
    byte    setStep( dword iStep, Cpoint origin, Cpoint focalPoint, Cvector up, 
                         real focalLength, real fStop, byte focalLengthNeedCorrection = true );

    byte    setCamera( dword idCamera, Cpoint origin, Cpoint focalPoint, Cvector up, 
                       real focalLength, real fStop, byte focalLengthNeedCorrection = true );

    void    setResolution( dword xRes, dword yRes );

    byte            setScreenRegion( dword x1, dword y1, dword x2, dword y2, const char *pType );

    // pType = "RGB", "BITMAPS"
    byte            setPath( const char *pType, const char *pPath );
    const char *    getPath( const char *pType );

    // addSearchingPath adds a path to look for missing textures, ior and r2 files.
    byte            addSearchingPath( const char *pPath );

    byte    setScreenRegionBitmap( Cbitmap bitmap ); // [Not used in plugins]

    /////////////////////////////////////////////////////////////////////////////////////////////////////// 
    ////////////////////////////////////// NEWCAMERA METHODS ///////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 
    
    Ccamera     addCamera( const char *pName, dword nSteps, real shutter, real filmWidth,
                           real filmHeight, real iso, const char *pDiaphragmType, real angle,
                           dword nBlades, dword fps, dword xRes, dword yRes, real pixelAspect );
    // addCamera is newer and it is recommended instead createCamera method

    Ccamera     getActiveCamera( );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// SKY METHODS /////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 
    
    byte    setSkyConstant( Crgb &color, real radianceScalar = 5.0, byte useSun = 0 );

    byte    setPhysicalSkyGeometry( real longitude, real latitude, int sm, dword dayOfYear, 
                                    real timeOfDay, real basisRotation );
    // Longitude: from -180.0 to 180.0; default = your city
    // Latitude: from -90.0 to 90.0; default = your city
    // Sm: from -12 to 12; default 0;
    // dayOfYear: from 1 to 365; default 100;
    // timeOfDay: from 0.0 to 24.0; default 17;
    // basisRotation: from 0 to 2PI radians (it is in radians)

    byte    setPhysicalSkySunAngles( real zenith, real azimuth );
    // It is an alternative way to set the sun position based on spheric angles
    // Call this function after setPhysicalSkyGeometry and longitude and latitude parameters will be overriden

    byte    setPhysicalSkySunDirection( Cvector dir );
    // It is an alternative way to set the sun position based on a direction
    // Call this function after setPhysicalSkyGeometry and longitude and latitude parameters will be overriden

    byte    setPhysicalSkySun( byte sunActive, real sunTemperature, real sunPowerScaleFactor, real planetReflectance );
    // SunActive: 0 or 1, by default sun is enabled (1)
    // sunTemperature: in kelvins. 
    // Default 5777. Range: 100 - 1000000
    // sunPowerScaleFactor: total power refered to the Earth's sun.  
    // For example a value of 0.8 would mean a sun emititng 0.8 times less energy than ours.
    // Default: 1.0. Range: Is allowed any value greater than 0.
    // planetReflectance: from 0 to 1

    byte    setPhysicalSkyAtmosphere( real ozone, real water, real angstromTurbidity, real wavelengthTurbidity,
                                      real aerosolAlbedo, real asymmetryFactor );

    // ozone: ( cms ) Default: 0.4 Range: Any value greated than 0 (usually between 0 and 1)
    // water: water vapor ( cms ) Default: 2.0 Range: Any value greated than 0
    // angstrom turbidity: coefficient Default: 0.04 Range: Any value greated than 0
    // wavelength turbidity exponent Default: 1.2 Range: Any value greated than 0
    // aerosolAlbedo: aerosol albedo Default: 0.8 Range: (0-1)
    // asimmetryFactor: "anisotropy" of aerosol. Default: 0.7. Range (-0.99999, 0.99999);


    byte    setActiveSky( const char *pActiveSky );
    // NULL <- no sky
    // "constant" <- constant sky
    // "physical" <- physical sky
    const char *    getActiveSky( );    
     // NULL <- no sky
    // "constant" <- constant sky
    // "physical" <- physical sky
   
    byte    getSkyConstant( Crgb &color, real &radianceScalar, byte &useSun );

    byte    getPhysicalSkyGeometry( real &longitude, real &latitude, int &sm, dword &dayOfYear, 
                                    real &timeOfDay, real &basisRotation, dword &sunPositionType );
    // sunPositionType: 
    // 0: lat/lon
    // 1: zenith/azimut angles
    // 2: direction 

    byte    getPhysicalSkySunAngles( real &zenith, real &azimuth );
    byte    getPhysicalSkySunDirection( Cvector &dir );
    byte    getPhysicalSkySun( byte &sunActive, real &sunTemperature, real &sunPowerScaleFactor, real &planetReflectance );
    byte    getPhysicalSkyAtmosphere( real &ozone, real &water, real &angstromTurbidity, real &wavelengthTurbidity,
                                      real &aerosolAlbedo, real &asimmetryFactor );


    // AUXILIAR SKY FUNCTIONS

    byte    saveSkyToHDR( char* path, const dword& xRes, const dword& yRes );
    // Saves current sky to an HDR file in the path "path" with the given resolution. Returns 1 if success

    byte    saveSkyToPreset( const char* path );
    // Saves current physical sky values to a sky preset file in "path"

    byte    loadSkyFromPreset( const char* path );  
    // Loads "path" preset file into the scene replacing current physical sky values 

    // Auxiliar functions to read the sky and paint it in viewport
    byte    getSunDirection( Cvector &dir );
    // Returns sun direction of the current sky values

    byte    getSkyColor( Crgb &hdrRgb, Crgb8 &ldrRgb, Cvector &dir );
    // Returns sky RGB color (in both high and low dynamic range) of the current sky values in the given direction 


    //-----Obsolete-----
    //byte    setPhysicalSky( real turbidity, real ozone, real water,  real longitude, real latitude,
    //                        int sm, dword dayOfYear /* 0 to 365 */, real timeOfDay, byte sunActive, real basisRotation );
    //void    setFog( real absorption, real scattering ); // Not used
    //byte    getPhysicalSky( real &turbidity, real &ozone, real &water, real &longitude, real &latitude, 
    //                        int &sm, dword &julianDay, real &timeOfDay, byte &sunActive, real &basisRotation );


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// ENVIRONMENT ///////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    byte    enableEnvironment( bool enable );
    byte    isEnvironmentEnabled( bool &enabled );


    // FUNCTION: setEnvironmentLayer / getEnvironmentLayer
    // Returns 1 if succeed. 
    // pLayerName sets the layer type
    // pLayerName = "background", "reflection", "refraction", "illumination"
    // pBitmapFileName
    // If "useSkyActive" is true, the sky is used instead this channel
    // sphericalMapping can only be set to false when pLayerName = "background"
    // Intensity range: From 0.0 to 1000.0 (default 1.0)
    // Tile range: From 0.0001 to 1000.0 (default 1.0)
    // Offset range: From 0.0 to 1.0 (default 0.0)


    byte    setEnvironmentLayer( const char *pLayerName, const char *pBitmapFileName, 
                                 bool useSkyActive, bool sphericalMapping, real intensity = 1.0, 
                                 real uTile = 1.0, real vTile = 1.0, 
                                 real uTileOffset = 0.0, real vTileOffset = 0.0 );

    byte    getEnvironmentLayer( const char *pLayerName, char *&pBitmapFileName, 
                                 bool &useSkyActive, bool &sphericalMapping, real &intensity, 
                                 real &uTile, real &vTile, real &uTileOffset, real &vTileOffset );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////// BITMAP METHODS //////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 

    // FUNCTION: readMxi 
    // Input: pPath (mxi file path)
    // Output: Returns a reference to a rgb pointer pRGB and resolution. Returns 1 if succeded.
    byte    readMxi ( const char *pPath, Crgb8 **pRGB, dword &xRes, dword &yRes );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////// MATERIAL METHODS ////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    // FUNCTION: readMaterial 
    // Input: pFileName (Material file path)
    // Output: Returns a Cmaterial pointer. 
    Cmaterial   readMaterial( const char *pFileName );
    // FUNCTION: createMaterial
    // Creates a material with the name pMaterialName and adds it to the scene if addToScene is true
    Cmaterial   createMaterial( const char *pMaterialName, bool addToScene = true );
    // FUNCTION: getMaterial 
    // Given the name of a material this function returns its Cmaterial pointer.
    Cmaterial   getMaterial( const char *pMaterialName );
    // FUNCTION: addMaterial 
    // Adds "material" into the scene and returns a pointer to the added material
    Cmaterial   addMaterial( Cmaterial &material );
	// FUNCTION: eraseUnusedMaterials
	// Erase unused materials from scen.e
	byte        eraseUnusedMaterials( );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////// RENDER METHODS /////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    byte    setRenderParameter( const char *pParameterName, dword size, const void *pParameterValue );
    byte    getRenderParameter( const char *pParameterName, dword size, void *pParameterValue );
        
    // Allowed render parameters are: (pParameterName, size, value)
    // "ENGINE", sizeof( char )*3, "RS0" or "RS1"
    // "NUM THREADS", sizeof( dword ), nThreads
		// "STOP TIME", sizeof ( dword ), time (in minutes)
		// "SAMPLING LEVEL", sizeof ( float ), sl
		// "USE MULTILIGHT",sizeof(byte), &doMultilight 
    // "DISABLE BITMAPS",sizeof(byte), &disableBitmaps 
    // "DO DIRECT LAYER",sizeof(byte), &doDirectLayer
    // "DO INDIRECT LAYER",sizeof(byte), &doIndirectLayer
		// "DO DIRECT REFLECTION CAUSTIC LAYER",sizeof(byte), &doDRFL
		// "DO INDIRECT REFLECTION CAUSTIC LAYER",sizeof(byte), &doIRFL 
		// "DO DIRECT REFRACTION CAUSTIC LAYER",sizeof(byte), &doDRFR 
    // "DO INDIRECT REFRACTION CAUSTIC LAYER",sizeof(byte), &doIRFR 
		// "DO RENDER CHANNEL",sizeof(byte), &doRenderChannel
		// "DO ALPHA CHANNEL",sizeof(byte), &doAlphaChannel 
    // "OPAQUE ALPHA",sizeof(byte), &doAlphaOpaqueChannel 
    // "DO IDOBJECT CHANNEL",sizeof(byte), &doIdObjectChannel 
    // "DO IDMATERIAL CHANNEL",sizeof(byte), &doIdMaterialChannel
		// "DO SHADOW PASS CHANNEL",sizeof(byte), &doShadowsChannel 
		// "DO VELOCITY CHANNEL",sizeof(byte), &doVelocityChannel 
		// "DO ZBUFFER CHANNEL",sizeof(byte), &doZbufferChannel 
    // "ZBUFFER RANGE", 2 * sizeof( real ), zRange
    // "VIGNETTING", sizeof( real ), &vignettingValue 
    // "SCATTERING_LENS", sizeof( real ), &scatteringValue 

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////// MXS METHODS //////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 

    byte    readMXS( const char *pPath, CoptionsReadMXS &mxsOptions );
    byte    writeMXS( const char *pPath = NULL ); 
    Crgb8 * readPreview( const char *pPath, dword &xResPreview, dword &yResPreview );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////// TONEMAPPING METHODS ////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 
    
        
    const char *  getActiveToneMapping( ); // "BASIC" or "ADVANCED", default "BASIC" [Plugins only use BASIC]
    byte    setToneMapping( real monitorGamma, real burn );  
    // // Burn from 0.0 to 1.0, default = 0.8
    // // monitorGamma 0.1 to 3.5, default = 2.2
    byte    getToneMapping( real &monitorGamma, real &burn ); 

    byte    setAdvancedToneMapping( real monitorGamma, real burn );
    byte    getAdvancedToneMapping( real &monitorGamma, real &burn );

    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////// GLARE METHODS //////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////

    byte    setDiffraction( real intensity, real frequency, const char *pApperture, const char *pObstacle = NULL );
    byte    getDiffraction( bool &isEnabled, real &intensity, real &frequency, const char **pApperture, const char **pObstacle );
    byte    enableDiffraction( );
    byte    disableDiffraction( );
    // // intensity, frequency from 0.0 to 1.0, default = 0.0


    ///////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////// OTHERS  METHODS ////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////// 

    void    computeFresnelGraph( float *pX, float *pY, dword n, real iorOutgoing );
    void    computeAbbeGraph( float *pX, float *pY, dword n, real nd, real abbe );
    byte    getCorrelatedColorTemperature( Crgb &rgb, real temperature );
    
    // Auxiliar read funtions
    byte    getNumMaterials( dword &nMaterials );
    byte    getNumObjects( dword &nObjects );
    byte    getNumCameras( dword &nCameras );
    byte    getNumClusters( dword &nClusters );
};

class Cmaxwell::CmultiValue::Cmap
{
  // CmultiValue::Cmap stores all the information of a map. 
public:
  
    enum
    {
        // A map can be set through a value (number) a plane color or a texture.
        // Most of the parameters can be set with 2 of this 3 types.
        // In example, a reflectance map can be set using an RGB or using a bitmap
        // Roughness, Anysotropy... can be set using a number or a bitmap.
        // The methods setActiveColor/getActiveColor set/get the active type when there is more than one
        TYPE_VALUE,
        TYPE_RGB,
        TYPE_BITMAP
    };

    enum
    {
        // Map interpolation.
        TYPE_INTERPOLATION_BOX, // Default
        TYPE_INTERPOLATION_CUADRATIC,
    };

    byte        type; // Default = TYPE_VALUE

    // value
    real        value; // Default = 0;

    // rgb
    Crgb        rgb; // Init to medium grey 127, 127, 127.

    // bitmap
    char *      pFileName;
    Cpoint2D    scale;
    Cpoint2D    offset;    
    dword       uvwChannel;
    byte        typeInterpolation;
    byte        uIsTiled;
    byte        vIsTiled;
    byte        invert;
    byte        doGammaCorrection;
    byte        useAbsoluteUnits;
    byte        normalMappingFlipRed;
    byte        normalMappingFlipGreen;
    byte        normalMappingFullRangeBlue;
    float       saturation; // range: [-1.0, 1.0]
    float       contrast;   // range: [-1.0, 1.0]
    float       brightness; // range: [-1.0, 1.0]
    float       clampMin;   // range: [0.0, 1.0]
    float       clampMax;   // range: [0.0, 1.0]
    

    Cmap( )
    {
        type = TYPE_VALUE;
        value = 0.0;
        rgb.assign( 0.5, 0.5, 0.5 );
        pFileName = NULL;
        uvwChannel = 0;
        uIsTiled = 1;
        vIsTiled = 1;
        scale.x = 1.0;
        scale.y = 1.0;
        offset.x = 0.0;
        offset.y = 0.0;
        invert = 0;
        doGammaCorrection = 0;
        useAbsoluteUnits = 0;
        normalMappingFlipRed = 0;
        normalMappingFlipGreen = 0;
        normalMappingFullRangeBlue = 0;
        typeInterpolation = TYPE_INTERPOLATION_BOX;
        saturation = 0.0;
        contrast = 0.0;
        brightness = 0.0;
        clampMin = 0.0;
        clampMax = 1.0;
    }
};

class Cmaxwell::CmultiValue::CemitterPair
{
public:
    // Regarding sdk, emitters are the most complex elements to create.
    // Emitters can be defined:
    // 1. Using color + luminance
    // 2. Using Temperature of emission
    // 3. Using an MXI texture

    // When emitters are set using color + luminance the color can be set:
    // 1.1.a Using RGB color
    // 1.1.b Using XYZ color
    // 1.1.c Using Correlated Temperature color

    // When emitters are set using color + luminance the luminance can be set:
    // 1.2.a Using Watts and efficacy
    // 1.2.b LuminousPower (Lumens)
    // 1.2.c illuminance (Lumens/m2)
    // 1.2.d luminousIntensity (Cd)
    // 1.2.b luminance (Cd/m2)

    // CmultiValue::CemitterPair is used to read/write each parameter

    //Cmaxwell::CmultiValue::CemitterPair emitterPair;
    //emitterPair.rgb.assign( rgb_color );
    //emitterPair.xyz.assign( xyz_color );
    //emitterPair.temperature = 6500.0;
    //emitterPair.watts = 40.0;
    //emitterPair.luminousEfficacy = 17.6;
    //emitterPair.luminousPower = 100.0;
    //emitterPair.illuminance = 100.0;
    //emitterPair.luminousIntensity = 100.0;
    //emitterPair.luminance = 100.0;
    //newEmitter.setPair( emitterPair );

    Crgb        rgb;                // "pair.color.rgb"
    Cxyz        xyz;                // "pair.color.xyz"
    real        temperature;        // "pair.color.temperature"

    real        watts;              // "pair.wattsAndLuminousEfficacy"
    real        luminousEfficacy;   // "pair.wattsAndLuminousEfficacy"

    real        luminousPower;      // "pair.luminousPower"
    real        illuminance;        // "pair.illuminance"
    real        luminousIntensity;  // "pair.luminousIntensity"
    real        luminance;          // "pair.luminance"

    CemitterPair( )
    {
        rgb.assign( 1.0, 1.0, 1.0 );
        xyz.assign( 1.0, 1.0, 1.0 );
        temperature = 6500;
        watts = 40.0;
        luminousEfficacy = 17.6;
        luminousPower = 100.0;
        illuminance = 100.0;
        luminousIntensity = 100.0;
        luminance = 100.0;
    }
};


#endif