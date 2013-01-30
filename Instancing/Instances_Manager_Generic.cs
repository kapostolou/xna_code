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
using GELib.Geometry.Geometric_Objects;

namespace GELib.Graphics
{
    
  
   


    /// <summary>
    /// 
    /// The Handle Type parameter should be a class, objects of which will be contained in the List that the
    /// Put_Instances_To_Buffers function accepts.
    /// 
    /// Each handle object is supposed to represent a command for the generation of one or more instances. 
    /// The Render_Handle interface contains a function called on the handle to providing it the part of the instance array
    /// where the handle should place the instances.
    /// 
    /// The Instance is also the vertex format of the instances batched and then sent to the GPU
    /// 
    /// So a Handle will be more or less a command to put instances on a certain slot, and the Instance will be the instances themselves
    /// 
    /// The slot is supposed to be an id to one of the sequence of draw calls this instantation of the Generic Instancing Template supports.
    /// It can also be seen as a draw order priority 
    /// 
    /// That is for each slot we assign
    /// 1) A Mesh, Blending Mode etc., a "model" 
    /// 2) a dynamic array where the Instance structs will be gathered.
    /// 3) a draw call during rendering that draws the whole Instance array using instancing
    /// 
    /// Each handle/command will draw on a particular slot.
    /// </summary>
    /// <typeparam name="Handle"></typeparam>
    /// <typeparam name="Instance"></typeparam>
    public class Instances_Manager_Generic<Handle, Instance>
        where Handle : Render_Handle<Instance>, new()
        where Instance : struct
    {
        //This contains the info of what mesh/blending mode etc each slot will use
        Instancing_Slots_Info Instancing_slots_info;

        //How many slots this instancing manager supports
        int Slot_Count;

        //For each slot create an array of Instances
        public Instance[][] instance_data_by_slot;
        
        
        //A dynamic vertex buffer that will get updated with the instances. One for each slot
        private DynamicVertexBuffer[] instancing_vertex_buffers_by_slot; 
        
        //A count of the number of instances added at each slot
        public int[] total_number_of_instances_for_this_slot;

        public Instances_Manager_Generic(
            GraphicsDevice graphics_device,
            VertexDeclaration vertexdeclaration,
            int instance_vb_size,
            Instancing_Slots_Info info
            )
        {
            this.Instancing_slots_info = info;
            this.Slot_Count = Instancing_slots_info.Number_Of_Slots;
            instancing_vertex_buffers_by_slot = new DynamicVertexBuffer[Slot_Count];
            total_number_of_instances_for_this_slot = new int[Slot_Count];

            instance_data_by_slot = new Instance[Slot_Count][];
            for (int i = 0; i < Slot_Count; i++)
            {
                instance_data_by_slot[i] = new Instance[1000];
            }
            for (int i = 0; i < Slot_Count; i++)
            {
                instancing_vertex_buffers_by_slot[i] = new DynamicVertexBuffer(graphics_device, vertexdeclaration,
                                                5000, BufferUsage.WriteOnly);
            }






        }



        public void Draw(GraphicsDevice graphics_device)
        {
            //each instancing manager is supposed to work on a a different surface
            //so that clearing the buffer won't destroy the data of other effects
            graphics_device.Clear(Color.Black);

            for (int current_slot = 0; current_slot < Slot_Count; current_slot++)
            {
                Draw_Slot(graphics_device, current_slot);
            }
           
        }

        public void Draw_Slot(GraphicsDevice graphics_device, int slot)
        {

            //check if there have been any instances added at this slot
            if (total_number_of_instances_for_this_slot[slot] < 1) return;
            
            //****ignore the if clause it's there for when the code gets generalized in the future
            if (Instancing_slots_info.slots[slot].just_mesh)
            {

                //set up the rendering call based on the information for this slot contained in the instancing_info object
                graphics_device.Indices = Instancing_slots_info.slots[slot].index_buffer;
                graphics_device.BlendState = Instancing_slots_info.slots[slot].blend;
                instancing_vertex_buffers_by_slot[slot].SetData(instance_data_by_slot[slot], 0, total_number_of_instances_for_this_slot[slot]);


                graphics_device.SetVertexBuffers(
                   
                   new VertexBufferBinding(Instancing_slots_info.slots[slot].mesh, 0, 0),
                   new VertexBufferBinding(instancing_vertex_buffers_by_slot[slot], 0, 1));


                //the effect wrapper is a class wrapping an XNA effect giving some extra properties that
                //directly access some of effect's parameters (instead of using a string dictionary)
                var effect_wrapper = Instancing_slots_info.slots[slot].effect_wrapper;
                if (effect_wrapper.World != null) effect_wrapper.World.SetValue(Matrix.Identity);
                if (effect_wrapper.View != null) effect_wrapper.View.SetValue(Camera.ActiveCamera.View);
                if (effect_wrapper.Projection != null) effect_wrapper.Projection.SetValue(Camera.ActiveCamera.Projection);
                
                //the instanced draw call
                //TODO: in the future it should get the primitivecount etc from the slot_info object
                effect_wrapper.effect.CurrentTechnique.Passes[0].Apply();
                graphics_device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               4, 0,
                                                               2, total_number_of_instances_for_this_slot[slot]);
            }





        }



        //Fills the buffers to be sent for instancing based on a List of Handles,
        //each handle containing info on where (which slot) and how many instances to put
        public void Put_Instances_to_Buffers(List<Handle> Handles)
        {
            //reeset the counts of instances to zero
            for (int i = 0; i < Slot_Count; i++)
            {
                total_number_of_instances_for_this_slot[i] = 0;
            }

            // for each object in the Handle list, ask it to place instances giving it the references to your
            //instance and insance-count arrays
            for (int j = 0; j < Handles.Count; j++)
            {

                var add_this = Handles[j];
                if (!add_this.Show) { continue; }


                add_this.Render(this.instance_data_by_slot, this.total_number_of_instances_for_this_slot);
            }


        }







    }
        


   


}
