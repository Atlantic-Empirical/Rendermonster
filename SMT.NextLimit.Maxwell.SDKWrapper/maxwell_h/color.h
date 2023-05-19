#ifndef MAXWELL_COLOR_H
#define MAXWELL_COLOR_H

#include "defines.h"

class   Crgb;

template< dword nChannels >
class Crgb8T
{
public:

    byte   pChannels[nChannels];

    inline byte & r( ) { return pChannels[0]; }
    inline byte & g( ) { return pChannels[1]; }
    inline byte & b( ) { return pChannels[2]; }
    inline byte & a( ) { return pChannels[3]; }

    void    assign( byte cr, byte cg, byte cb )
    {
        r( ) = cr;
        g( ) = cg;
        b( ) = cb;
    }

    void    assign( byte cr, byte cg, byte cb, byte ca )
    {
        r( ) = cr;
        g( ) = cg;
        b( ) = cb;
        a( ) = ca;
    }
};

class Crgb8: public Crgb8T<3>
{
public:

    void    toRGB( Crgb &rgb );
    dword   getSummatory( );
};

class Crgb: public CvectorT<float>
{
friend class Cxyz;
friend class Chsv;
friend class CspectrumComplete;
friend class Cspectrum;
friend class Cyiq;

public:

    dword   get( );
    void    gammaCorrectionRec709( real gamma );
    void    invGammaCorrectionRec709( real gamma );
    void    toRGB8( Crgb8 &rgb8 );
    void    toXYZ( Cxyz *pXYZ ); 
    void    toHSV( Chsv *pHSV ); // devuelve: h entre [0,360], s entre[0,1], v entre[0,1]
    void    toHsv( float *h, float *s, float *v );
    void    toYIQ( Cyiq &yiq );
    void    toReflectanceSpectrum( CspectrumComplete *pSpectrum, real maxReflectance );
    void    toReflectanceSpectrum( Cspectrum &s, real maxReflectance );
    void    clip( );
    bool    constrain( ); // true si el color ha sido modificado para encajar en el gamut false, ya estaba en el gamut
};

class Cxyz: public CvectorT<double>
{
public:

    void    toRGB( Crgb *pRgb );
};

#endif
