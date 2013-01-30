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
using GELib.Graphics.Generic_Instancing.Force;
namespace GELib
{
   /// <summary>
   /// This class is used to place forces on the spring grid being simulated on the GPU
   /// The forces are placed using sprites drawn with hardware instancing
   /// The gameplay code and the Post_Process object guding the effect use an object of this class (the game code via a static singleton)
   /// It is supposed to get constructed by the Spring Effect Post Process object
   /// </summary>
    public class Spring_Grid_Force_Placement_Manager
    {
        public static bool flip = false;
        //Used as an entry point by game code
        public static Spring_Grid_Force_Placement_Manager Singleton;

        #region public interface
        
        /// <summary>
        /// Manages the GPU instancing of the sprites that contain
        /// the forces placed on the grid. The manager's type is an alias for a generic template for instancing, specialized for
        /// the placement of force-sprites (quads, but it will be generalized to other models) on the GPU RenderTarget      
        /// </summary>
 
        public Force_Quad_Instances_Manager Instances;
        
        /// <summary>
        /// The handler objects for force-sprite drawing are added on this c# list (resizable vector)
        /// updated, used to generate the instancing data, and then removed in batch by setting a kill flag 
        /// and using a RemoveAll(predicate) function
        /// </summary>
        public List<Force_Draw_Handle> Forces;
        #endregion

        #region needed for drawing
        
       

        /// <summary>
        /// The HDRBlendable render surface where the force-sprites are drawn. 
        /// the "by_position" is there cause the shader computing the hooke's law forces
        /// works per "index in the grid" rather than by geometrical position.
        /// 
        /// That is this surface contains forces placed geometrically and an effect gathers them to point masses being located near.
        /// A second shader uses the forces gathered to a point mass
        /// and adds hooke forces. 
        /// </summary>
        private RenderTarget2D Force_By_Position_Accumulator;

        #endregion

        public Spring_Grid_Force_Placement_Manager(Spring_Effect spring_effect)
        {
           
            this.Force_By_Position_Accumulator = spring_effect.Force_By_Position_Accumulator;
            Forces = new List<Force_Draw_Handle>(500);
            Singleton = this;
           
            
            
            Instances = new Force_Quad_Instances_Manager(CM.game1.GraphicsDevice, MY_Model_Slot_Info_Multiton.Spring_Grid_Force_Drawing_Slot_Info);



        }



        /// Gets called by the post process effect object whenever the
        /// acquired force-sprites should get drawn
        /// Since the whole effect gets drawn 4 times per frame to make the simulation more stable,
        /// the end of the current frame (when the current sprite-force-handler data are not anymore needed)
        /// has to be specified by the caller in the clear_forces argument.
        public void Draw_All_The_Forces(bool clear_forces)
        {




            
            
            //The rendertarget is an hdrblendable which means it can additively blend the
            //floating point  data of the forces, 
            //except with some loss of precision
            CM.game1.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            CM.game1.GraphicsDevice.BlendState = BlendState.Additive;
            CM.game1.GraphicsDevice.SetRenderTarget(Force_By_Position_Accumulator);


            //This is where we could also call some kind of update function
            //on the handles, as in, they won't get deleted per frame but kept till
            //a predicate is false, at each frame updating their state to
            //produce instances accordingly. This is not currently done for the force sprite handles.


            ///Call the generic instance manager to draw the force-sprites.
            ///This includes extracting instances from the handlers we've added in the Forces list
            Instances.Draw(CM.game1.GraphicsDevice);


            // the frame is over and the force list gets cleared
            if (clear_forces)
            {
                for (int i = 0; i < Forces.Count; i++)
                {
                    //This returns the handle to the free list of handles where it was taken from
                    //Memory allocation isn't done using the new operator cause garbage collection can slow things down a lot.
                    Forces[i].Return();

                }
                Forces.Clear();
            }


            //used for debugging
            //Force.GetData(debug_se. debug_Forces);


        }


        
        /// <summary>
        ///  Adds handles that contain info for generating force sprite instances
        /// The information that the handle uses could potentialy be modified to contain a "model id" specifing what mesh gets drawn with instances (so far I'm only using quads).
        /// This would be a "slot" of the generic instancing template as described in its source file.
        /// </summary>
        /// <param name="Position"> where to draw, since the effect runs on normalized coordinates this gets scaled by 50 then clamped</param>
        /// <param name="Direction"> supposed to indicate what direction the object placing the force on the grid was moving</param>
        /// <param name="Halfaxis"> size of the square quad</param>
        /// <param name="strength"> How strong the force should be</param>
        public void Add_Force(Vector2 Position, Vector2 Direction, float Halfaxis, float strength)
        {

            //The handle gets taken from a pool (a free list) of recycled objects, RATHER than allocated using the new operator
            var handle = GELib.Storing.Object_Pools.Force_Handlers.Get();

            //normally a constructor would be used but the object has already been allocated during the initialization
            handle.Set_Force_Handle(Position, Halfaxis, strength, Direction);
            
            //The Forces list of handles is later taken by the Generic Instancing template and used to produce instances
            Forces.Add(handle);
        }


    }
}
