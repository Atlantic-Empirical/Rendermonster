#ifndef MAXWELL_DEFINES_H
#define MAXWELL_DEFINES_H

#ifndef NULL
    #define NULL                0
#endif

///////////////////////////////////////////////////
//////////////// CUSTOM DEFINITIONS ///////////////
///////////////////////////////////////////////////

///////////////// COMPILER OPTIONS ////////////////

//#define COMPILER_MICROSOFT
//#define COMPILER_INTEL
//#define COMPILER_GCC
//#define COMPILER_CODEWARRIOR

#ifdef COMPILER_MICROSOFT
    //#pragma warning( disable : 4018 4101 4244 4305 )
    #define X86_PLATFORM
#endif

///////////////////////////////////////////////////
////////////// INTERNAL DEFINITIONS ///////////////
///////////////////////////////////////////////////

#define SINGLE_GEOMETRY_PRECISION

#if defined (COMPILER_GCC) || defined (COMPILER_CODEWARRIOR)
    typedef double                  real;       
    typedef unsigned long long int  qword;
    typedef unsigned int            dword;
    typedef unsigned short          word;
    typedef unsigned char           byte;
#else
    #ifdef VISUAL_STUDIO_6
        typedef _int64                  qword;
    #else
        typedef unsigned _int64         qword;
    #endif

    typedef double              real;   
    typedef unsigned long       dword;
    typedef unsigned short      word;
    typedef unsigned char       byte;
#endif

#define ExtractByte( x ) ( *(byte *)x )
#define ExtractWord( x ) ( *(word *)x )
#define ExtractDword( x ) ( *(dword *)x )
#define ExtractQword( x ) ( *(qword *)x )
#define ExtractFloat( x ) ( *(float *)x )
#define ExtractReal( x ) ( *(real *)x )

extern  void BlockCode( );
#define CHECK( x ) { if ( !( x ) ) BlockCode( ); }

#ifdef _DEBUG
    #define CHECK_DEBUG( x ) CHECK( x )
#else
    #define CHECK_DEBUG( x )
#endif

#endif
