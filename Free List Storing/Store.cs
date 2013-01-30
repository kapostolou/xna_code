using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GELib
{
  
    /// <summary>
    /// This is used for the allocation of objects during initialization, and their recycling during runtime
    /// so as to avoid calling the new operator and making the garbage collector slow down the game.
    /// 
    /// Objects are created on an array, placed on a stack and popped ->used ->suspended ->pushed back on the stack
    /// For recycling objects of type T there needs to be a Store<T> object accessed from the game code via some multiton.
    /// 
    /// The Storable interface that the type T should implement, contains one field indicating whether the object isn't needed anymore
    /// (this isn't used here but should be used with the RemoveAll on a dynamic array of Storables)
    /// and a function that does the Suspending (cleaning up the old state).
    /// The name of the Suspend function implies that it is supposed to be called as the objects get RemoveAll'd in a batch
    /// (I guess this would help with the cache usage instead of them being "revived" 1 by 1 in the actual game code)
    /// 
    /// The game generally relies on dynamic arrays with no RemoveAt calls (which would produce too much copying)
    /// but a final RemoveAll, and so far using the dynamic arrays versus some other type of data structure (such as a linked
    /// list with pointers on the objects themselves) doesn't seem to cause performance issues
    /// 
    /// Each Storable object also has a member with the stack it was taken from so that the client game code won't have to know
    /// where to return it.
    /// 
    /// Objects are in the arrays are  initialized mode with their parameter less constructor
	/// When suspended they are supposed to release resources ai requests etc. and get back to their initialized state
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Store<T> where T : class,Storable,new()
    {
        
        private Stack<object> free_stack;
        T[] array;
        public int Left;
        public int Used = 0;
        public int Size;
        public Store(int size)
        {
            
            Size = size;
            
            array = new T[size];
            free_stack = new Stack<object>(size);
            for (int i = 0; i < size; i++)
            {
                var item = new T();
                 array[i] = item;
                
            }
            for (int i = 0; i < size; i++)
                free_stack.Push(array[i]);
            for (int i = 0; i < size; i++)
                array[i].My_Store = free_stack;
            Left = size;
        }

       
        
        
        /// <summary>
        /// Returns the first ( supposed to be suspended) object from the stack
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            
            Left--; Used++;
            var ret = free_stack.Pop() as T;
            ret.Killed = false;
            return ret;
        }


        
    }

}
