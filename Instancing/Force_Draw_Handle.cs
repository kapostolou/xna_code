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

namespace GELib.Graphics.Generic_Instancing.Force
{
    
    // This class is used by the generic instancing template.
    // Since it isn't a struct and c# gets slow when relying on the garbage collector and/or allocating objects a lot,
    // it implements an interface for objects allocated during the initialization and then recycled (Storable).
    
    // The RenderHandle generic interface is the one that the generic instancing template supports,
    // it in general means that the class can create instances (which are directly used in vertex buffers) when asked to.
    
    // The Force_Draw_instance parameter is the vertex format struct of the instances
    
    // The general use of this class is as follows
    // the Spring_Grid_Force_Placement_Manager class that is viewed by the rest of the game code
    // accepts Force_Draw_handle objects in a dynamic array structure,
    //that is from the game code, you add a force to the grid by picking a Force_Draw_Handle Handle from the "store" (an array of recyclable objects impementing the storable interface),
    //configuring Handle using the Set_Force_Handle function,
    //adding Handle to the Spring_Grid_Force_Placement_Manager's dynamic array (in C# they're called List),
    //and then the manager supplies this dynamic array to the Generic Instancing template,
    //which for each handle in the list calls the Render function supplying the handle with the template's internal data structures and asking the handle to fill them with instances
    //The way those classes are put together in this template is a little different than what it would be in C++
    //but C# generics are kind of limited and this was the easiest way to get it to work

    public class Force_Draw_Handle : Render_Handle<Force_Draw_Instance>, Storable
    {
        //the following 4 members specify how the force quad to be drawn is geometrically placed, how strong the force will be
        //and the direction the object placing the force was moving,
        //all 4 are also repeated in the Instance struct that gets sent via a vertex buffer to the GPU. See the Spring_Draw_Forces_I hlsl effect for details
        public Vector2 Position;
        public float Halfaxis;
        public float Strength;
        public Vector2 Direction;

        //whether the handle should be ignored by the template code
        public bool Show { get; set; }
        
        //the name is actually not quite right, it specifies how many instances the handle is to add
        //it is used for motion blurring in other types of handles
        public int Motion_Blurs { get; set; }
        
        //an id specifing in which of the batches (slots) that the generic instancing template draws
        //this handle will add instances to
        public int Slot_Id { get; set; }

        // from Storable: Indicates if the handle should be recycled and added back to the Force_Draw_Handle store
        public bool Killed { get; set; }

        //from Storable: A stack the object should be pushed to when recycled, when "suspended"
        //The store is the type of data structure I've seen called as a "free list", which uses a stack
        
        public Stack<object> My_Store { get; set; }
        
        //from Storable: This is called when the object gets suspended, it optionally contains recycling code so that when the object
        //is picked again it won't contain values from its previous use.
        public void Suspend() { }


        public Force_Draw_Handle()
        {
            Show = true;
            Motion_Blurs = 1;
        }

        /// <summary>
        /// Called when adding a handle to the handles list. 
        /// This list as well as its deallocation are discussed in more detail in Spring_Grid_Force_Placement_Manager
        /// </summary>
        
        public void Set_Force_Handle(Vector2 Position, float Halfaxis, float Strength, Vector2 Direction)
        {
            //The hlsl script uses npormalized coordinates and scaling is a cheap way to get there from the world coords the code placing
            //the handle is using
            this.Position = Position / 50;
            this.Halfaxis = Halfaxis / 50;
            this.Strength = Strength;
            this.Direction = Direction;
        }



        // Called by the generic instancing template.
        // The template provides the Instance arrays where the handle is supposed to put instances to, based on its slot(priority) code
        // The second argument is the place where the handle should add the amount of instances it actually produced
        // (this could be refactored better)
        public void Render(
          Force_Draw_Instance[][] instance_data_by_material,
          int[] total_number_of_instances_for_this_material
          )
        {
            for (int i = 0; i < Motion_Blurs; i++)
            {

                //Hopefully the optimizer deals with this piece of code efficiently
                //the Instance structs aren't allocated/deallocated dynamically
                //and calling them to update based on the handle produced errors from the typing system
                
                var rend =
                    instance_data_by_material
                    [this.Slot_Id]
                    [total_number_of_instances_for_this_material[this.Slot_Id]];


               

                rend.Position = Position;
                rend.Direction = Direction;
                rend.Halfaxis = Halfaxis;
                rend.Magnitude = Spring_Grid_Force_Placement_Manager.flip?Strength:-Strength;
                instance_data_by_material
                    [this.Slot_Id]
                    [total_number_of_instances_for_this_material[this.Slot_Id]++] = rend;

            }

        }


    }
}
