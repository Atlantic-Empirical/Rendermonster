#ifndef FLAGS_H
#define FLAGS_H

#include "defines.h"

template< class type >
class Cflags  
{
public:
    
    type    bits;

    Cflags( )
    {
        bits = 0;
    }   

    void clear( )
    {
        bits = 0;
    }

    
    type get( )
    {
        return bits;
    }
    
    void enable( type mask )
    {
        bits |= mask;
    }

    void enable( type mask, bool enableFlag )
    {
        if ( enableFlag ) 
        {
            bits |= mask;
        }
        else 
        {
            bits &= ~mask;
        }
    }

    void enableOnly( type mask )
    {
        bits = mask;
    }

    void enableMask( type mask, type value )
    {
        disable( mask );
        bits |= mask & value;
    }

    void disable( type mask )
    {
        bits &= ~mask;
    }

    bool    isAnyEnabled( )
    {
        return ( bits != 0 );
    }

    bool    isAnyEnabled( type mask )
    {
        return ( ( bits & mask ) != 0 );
    }

    bool isEnabled( type mask )
    {
        return ( ( bits & mask ) != 0 );
    }
    
    bool isEnabled( type mask, type value )
    {
        return ( ( bits & mask ) == value );
    }   

    bool isEnabledOnly( type mask )
    {
        return ( bits == mask );
    }
    
    bool isDisabled( type mask )
    {
        return ( ( bits & mask ) == 0 );
    }

    bool isDisabled( type mask, type value )
    {
        return ( ( bits & mask ) != value );
    }   

    bool areEnabled( type mask )
    {
        return ( ( bits & mask ) == mask );
    }
    
    void invert( type mask )
    {
        bits ^= mask;
    }

    void update( type mask, type activate )
    {
        if ( activate != 0 ) 
        {
            bits |= mask;
        }
        else 
        {
            bits &= ~mask;
        }
    }

    type isEqual( Cflags<type> flags, type mask )
    {
        return ( ( bits & mask ) == ( flags.bits & mask ) );
    }
};

#endif