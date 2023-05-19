#ifndef BASE_H
#define BASE_H

#include "defines.h"

class Cbase
{
public:

    Cpoint      origin;
    Cvector     xAxis;
    Cvector     yAxis;
    Cvector     zAxis;

    Cbase( )
    {
    };

    Cbase( Cpoint &_origin, Cvector &_xAxis, Cvector &_yAxis, Cvector &_zAxis )
    {
        origin = _origin;
        xAxis = _xAxis;
        yAxis = _yAxis;
        zAxis = _zAxis;
    }
    
    void initCanonic( )
    {
        origin.assign( 0.0, 0.0, 0.0 );
        xAxis.assign( 1.0, 0.0, 0.0 );
        yAxis.assign( 0.0, 1.0, 0.0 );
        zAxis.assign( 0.0, 0.0, 1.0 );
    }
};

#endif