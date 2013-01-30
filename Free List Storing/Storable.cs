using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GELib
{
    /// <summary>
    /// Holds a reference to the Stack this was taken from so that the Return() extension method (from Storable_Extensions)
    /// can now where to return the Storable. Suspend() is the function that will clean up the object
    /// from its old state
    /// </summary>
    public interface Storable:Killable
    {
                
        Stack<object> My_Store{get;set;}
        void Suspend();
    }

    public interface Killable
    {

        bool Killed { get; set; }
        
    }

    public static class Storable_Extensions
    {
        //An extension method uses the public interface of a type yet it is still accessed using the dot notation
        // Making the Storable interface an abstract class would make it difficult to use with C# not supporting
        // multiple inheritance
        public static void Return(this Storable s)
        {
            if (s.My_Store != null)
            {
                s.Suspend();
                s.My_Store.Push(s);
            }
            
        }
    }
}
