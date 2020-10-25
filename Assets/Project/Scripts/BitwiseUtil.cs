using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BitwiseUtil
{
    public static int GetBiggestBit( int flags )
    {
        // https://www.includehelp.com/c-programs/find-the-highest-bit-set-for-any-given-integer.aspx

        int count = 0;
        int store = -1;
	    
        while( flags != 0)
        {
            //if current bit is set
		    if( ( flags & 1 ) == 1 ) 
                
                store = count; 

            //right shift
		    flags = flags >> 1; 
		    
            count ++;
	    }

        if( store == -1 ) return 0;

        return store;
    }

}