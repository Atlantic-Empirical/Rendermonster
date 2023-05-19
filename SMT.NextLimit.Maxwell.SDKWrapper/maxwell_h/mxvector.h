#ifndef VECTOR_H
#define VECTOR_H

#if defined (_MAC_PPC) || defined (_MAC_MACH)
#pragma cpp_extensions on
#endif

template< class Cprecision >
class Cvector2DT
{       
public:

    union
    {
        struct
        {
            Cprecision  x;
            Cprecision  y;
        };

        struct
        {
            Cprecision  u;
            Cprecision  v;
        };

        Cprecision      pValues[2];
        Cprecision      pChannels[2];
    };
    
    void    setZero( )
    {
        x = 0.0;
        y = 0.0;
    }

    void assign( double vx, double vy )
    {
        x = vx;
        y = vy;
    }

    void assign( const Cvector2DT &v )
    {
        x = v.x;
        y = v.y;
    }

    // COMPARE //

    bool    operator < ( const Cvector2DT &vector )
    {
        return ( x < vector.x || ( x == vector.x && ( y < vector.y ) ) );
    }

    bool    operator > ( const Cvector2DT &vector )
    {
        return ( x > vector.x || ( x == vector.x && ( y > vector.y ) ) );
    }

    bool operator == ( const Cvector2DT &v )
    {
        return ( x == v.x && y == v.y );
    }
};

template< class Cprecision >
class CvectorT
{
public:

    union
    {
        struct
        {
            Cprecision  x;
            Cprecision  y;
            Cprecision  z;
        };

        struct
        {
            Cprecision  r;
            Cprecision  g;
            Cprecision  b;
        };

        struct
        {
            Cprecision  u;
            Cprecision  v;
            Cprecision  w;
        };

        Cprecision      pValues[3];
        Cprecision      pChannels[3];
    };
    
    void    setZero( )
    {
        x = 0.0;
        y = 0.0;
        z = 0.0;
    }

    // ASSIGN //

    void assign( double vx, double vy, double vz )
    {
        x = (Cprecision)vx;
        y = (Cprecision)vy;
        z = (Cprecision)vz;
    }

    void assign( const CvectorT &v )
    {
        x = (Cprecision)v.x;
        y = (Cprecision)v.y;
        z = (Cprecision)v.z;
    }

    // COMPARE //

    bool    operator < ( const CvectorT &vector )
    {
        return ( x < vector.x || ( x == vector.x && ( y < vector.y || ( y == vector.y && z < vector.z ) ) ) );
    }

    bool    operator > ( const CvectorT &vector )
    {
        return ( x > vector.x || ( x == vector.x && ( y > vector.y || ( y == vector.y && z > vector.z ) ) ) );
    }

    bool operator == ( const CvectorT &v )
    {
        return ( x == v.x && y == v.y && z == v.z );
    }
};

class Cvector2D: public Cvector2DT<double>
{
};

class CfVector2D: public Cvector2DT<float>
{
};

class Cvector: public CvectorT<double>
{
};

class CfVector: public CvectorT<float>
{
};

typedef Cvector2D Cpoint2D;
typedef CfVector2D CfPoint2D;
typedef Cvector Cpoint;
typedef CfVector CfPoint;

#endif