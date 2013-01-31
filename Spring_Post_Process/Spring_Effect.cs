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

    public class Spring_Effect : Post_Proccess
    {
        GraphicsDevice device;

        //Number of mass points in the grid's x direction
        const int sizex = 128;
        //Number of mass points in the grid's y direction
        const int sizey = 128;
        const int number_of_inner_grid_points = (sizex - 1) * (sizey - 1);



        
        //How many times per frame the spring simulation should run
        const int steps_per_frame = 16;

		
        //A helper class configured to perform some calculations used in setting up
        //the vertices of the vertexbuffers used in this post process effect
        Help.GPU_Vertex_Helper helper;

        //This is where the game adds "handles" that generate instanced quads containing forces placed on the grid.
        Spring_Grid_Force_Placement_Manager force_placement_manager;
        
      

       
        # region debug
        //Used in retrieving the GPU contents for debugging
        Vector4[] debug_Positions;
        Vector4[] debug_Velocities;
        Vector4[] debug_Forces;
        #endregion

        #region buffers

        //The XNA vertex buffer containing the quad that will be transformed and drawn as the graphical depiction of a each "spring"
        VertexBuffer draw_lines_buffer;
        
        //The XNA vertex buffer that contains the screen wide quad that passes the "indexes"(a texture coordinate) of the point masses to pixel shaders
        private VertexBuffer full_screen_with_index_info_buffer;

        
        //Initialize the above VertexBuffers
        private Spring_Processing_Vertex_Format[] __full_screen_with_index_info_buffer;
        private Spring_Drawing_Vertex_Format[] __draw_lines_buffer;

        //An index buffer indexing the 4 vertices of a quad
        IndexBuffer index_buffer;
        
        
        #endregion
        #region effects

        
        //This effect performs the point mass positions integration
        Effect Update_Position_effect;
        //This effect performs the point mass velocities integration
        Effect Update_Velocity_effect;
        //Calculates the hooke spring forces
        Effect Hooke_Force_effect;
        //Gathers forces placed geometrically on the grid, to mass points, by comparing the point's position to the area the force covers
        Effect Gather_Forces_to_indices;
        //The effect that draws the spring grid
        Effect Draw_the_Grid_effect;
        
        
        
        #endregion
        #region Surfaces

        // The original positions of the point masses, can be used in the force calculations to add some force to send each point to its original position
        RenderTarget2D Initial_Position;
        
        //The mass point positions of the previous frame
		//In this and most of the other surfaces, each texel in the surface is supposed to correspond to a mass point in the grid (as ordered by their equilibrium positions)
        //that is the texels correspond to "grid array indices" (of mass points) with the values in the texel giving the geometrical coordinates of that index
        RenderTarget2D Position0;

        //Where this frame's positions will be calculated and then written
        RenderTarget2D Position1;

        //The velocities of the previous frame
        RenderTarget2D Velocity0;

        //Where this frame's velocities will be calculated
        RenderTarget2D Velocity1;
        
        //The force that each mass point will be influenced by during this frame
        //Notice that this represents the force per index (that is per point)
        //while the next surface has forces accumulated by "geometrical area".
        //A gather step gets those forces to the by index accumulator
        //by reading each index's current position and then reading what's stored in the forces by position accumulator
        RenderTarget2D Force_By_Index;
        
        //This is the surface where the "force-quads" (or even other models than quads) get placed
        public RenderTarget2D Force_By_Position_Accumulator;
        
        


        # endregion
        
        #region Initialization

        private void Create_Vertex_Buffers()
        {
           
            __draw_lines_buffer = new Spring_Drawing_Vertex_Format[4 * 4 * number_of_inner_grid_points];
            
            //Prepare the vertexbuffer that gives indices (by using the uv coords) to texels, the index acting as an identifier for a point mass
			//See the GPU_Vertex_Helper class for a description of the helper methods
            __full_screen_with_index_info_buffer = new Spring_Processing_Vertex_Format[4];
            __full_screen_with_index_info_buffer[0] = new Spring_Processing_Vertex_Format(helper.Clip_Coords_of_point_UL(1, 1), helper.TX_Coords_of_point_Center(1, 1));
            __full_screen_with_index_info_buffer[1] = new Spring_Processing_Vertex_Format(helper.Clip_Coords_of_point_UL(1, sizey - 1), helper.TX_Coords_of_point_Center(1, sizey - 1));
            __full_screen_with_index_info_buffer[2] = new Spring_Processing_Vertex_Format(helper.Clip_Coords_of_point_UL(sizex - 1, sizey - 1), helper.TX_Coords_of_point_Center(sizex - 1, sizey - 1));
            __full_screen_with_index_info_buffer[3] = new Spring_Processing_Vertex_Format(helper.Clip_Coords_of_point_UL(sizex - 1, 1), helper.TX_Coords_of_point_Center(sizex - 1, 1));



            //Prepare the buffer containing the quad that will be drawn to depict a spring
            
            int counter = 0;
            
            for (int i = 0; i < sizex * sizey; i++)
            {
                var x = i % sizex;
                var y = i / sizey;


                if ( (x == sizex - 1) || (sizey - 1 == y)) continue;
                for (int k = 0; k < 2; k++)
                {
                    var ii = counter++;
                    var add = helper.find_neighbour_in_tx_coords2(k);
                    __draw_lines_buffer[ii * 4] = new Spring_Drawing_Vertex_Format(helper.Clip_Coords_of_point_UL(1, 1), new Vector2(0, 0), helper.TX_Coords_of_point_Center(x, y), helper.TX_Coords_of_point_Center(x, y) + add);
                    __draw_lines_buffer[ii * 4 + 1] = new Spring_Drawing_Vertex_Format(helper.Clip_Coords_of_point_UL(1, sizey - 2), new Vector2(0, 1), helper.TX_Coords_of_point_Center(x, y), helper.TX_Coords_of_point_Center(x, y) + add);
                    __draw_lines_buffer[ii * 4 + 2] = new Spring_Drawing_Vertex_Format(helper.Clip_Coords_of_point_UL(sizex - 2, sizey - 2), new Vector2(1, 1), helper.TX_Coords_of_point_Center(x, y), helper.TX_Coords_of_point_Center(x, y) + add);
                    __draw_lines_buffer[ii * 4 + 3] = new Spring_Drawing_Vertex_Format(helper.Clip_Coords_of_point_UL(sizex - 2, 1), new Vector2(1, 0), helper.TX_Coords_of_point_Center(x, y), helper.TX_Coords_of_point_Center(x, y) + add);

                }
            }


            


            //Prepare an index buffer assigning consecutive 4-uples of vertices to 2 triangles
            uint[] indices = new uint[4 * number_of_inner_grid_points * 6];

            for (int i = 0; i < 4 * number_of_inner_grid_points; i++)
            {
                indices[i * 6 + 0] = (uint)(i * 4 + 0);
                indices[i * 6 + 1] = (uint)(i * 4 + 1);
                indices[i * 6 + 2] = (uint)(i * 4 + 2);
                indices[i * 6 + 3] = (uint)(i * 4 + 0); ;// (i * 4 + 1);
                indices[i * 6 + 4] = (uint)(i * 4 + 2); ; //(i * 4 + 1);
                indices[i * 6 + 5] = (uint)(i * 4 + 3); ;// (i * 4 + 1);
            }

            index_buffer = new IndexBuffer(device, typeof(uint), indices.Length, BufferUsage.WriteOnly);

            index_buffer.SetData(indices);



            
            draw_lines_buffer = new VertexBuffer(device, Spring_Drawing_Vertex_Format.VertexDeclaration,
                                                 (sizex - 1) * (sizey - 1) *2 * 4, BufferUsage.WriteOnly);
            draw_lines_buffer.SetData(0, __draw_lines_buffer, 0, (sizex - 1) * (sizey - 1) * 2 * 4, Spring_Drawing_Vertex_Format.SizeInBytes);


            

            full_screen_with_index_info_buffer = new VertexBuffer(device, Spring_Processing_Vertex_Format.VertexDeclaration,
                                                 4, BufferUsage.WriteOnly);
            full_screen_with_index_info_buffer.SetData(0, __full_screen_with_index_info_buffer, 0, 4, Spring_Processing_Vertex_Format.SizeInBytes);

           

        }



        private void Create_Debug_Structures()
        {
            debug_Positions = new Vector4[sizex * sizey];
            debug_Velocities = new Vector4[sizex * sizey];
            debug_Forces = new Vector4[sizex * sizey];
        }

        private void Create_Effects()
        {
           
            Update_Position_effect = CM.game1.Content.Load<Effect>("Spring/Spring_Position");


            Update_Velocity_effect = CM.game1.Content.Load<Effect>("Spring/Spring_Velocity");
            Update_Velocity_effect.Parameters["Force"].SetValue(Force_By_Index);



            Hooke_Force_effect = CM.game1.Content.Load<Effect>("Spring/Spring_Force");
            Hooke_Force_effect.Parameters["Initial_Position"].SetValue(Initial_Position);

            
            Gather_Forces_to_indices = CM.game1.Content.Load<Effect>("Spring/Spring_Gather_Forces_To_Indexes");
            Gather_Forces_to_indices.Parameters["Force_Spatial_Accumulator"].SetValue(Force_By_Position_Accumulator);



            Draw_the_Grid_effect = CM.game1.Content.Load<Effect>("Spring/Spring_Draw_Grid");

            
            Draw_the_Grid_effect.CurrentTechnique = Draw_the_Grid_effect.Techniques[0];
            Update_Position_effect.CurrentTechnique = Update_Position_effect.Techniques[0];
            Update_Velocity_effect.CurrentTechnique = Update_Velocity_effect.Techniques[0];
            Hooke_Force_effect.CurrentTechnique = Hooke_Force_effect.Techniques[0];
            Gather_Forces_to_indices.CurrentTechnique = Gather_Forces_to_indices.Techniques[0];

            

        }


        /// <summary>
        /// Creates the surfaces used by the effect
        /// </summary>
        private void Create_Surfaces()
        {
            //Initialize the positions and velocities of the point masses
            Vector4[] initial_Positions = new Vector4[sizex * sizey];
            Vector4[] initial_Velocities = new Vector4[sizex * sizey];
            for (int i = 0; i < sizex * sizey; i++)
            {
                var x = i % sizex;
                var y = i / sizey;

                var coords = helper.Clip_Coords_of_point_UL(x, y);
                initial_Positions[i] = new Vector4(coords.X, coords.Y, 0, 0);


            }

            for (int i = 0; i < sizex * sizey; i++)
            {


                initial_Velocities[i] = new Vector4(0, 0, 0, 1);// + new Vector2(0, 0));


            }


            var frame_buffer_format = device.PresentationParameters.BackBufferFormat;

            //Something that has to do with gpu implementation details, it's explained in the comment lines below
            var format_choice =  SurfaceFormat.HdrBlendable;

            //Most of the surfaces use the vector4 format, though it would probably also work using just vector2
            //The gpu can't additively blend using IEEE floats 
            //so certain buffers that need to be written in additive blending for the effect to work,
            //namely those gathering forces geometrically and per index,
            //they instead use a surface suitable for hdr rendering that XNA provides
            //which seems accurate enough, and supports additive blending
            Initial_Position = new RenderTarget2D(device, sizex, sizey, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Position0 = new RenderTarget2D(device, sizex, sizey, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Position1 = new RenderTarget2D(device, sizex, sizey, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Velocity0 = new RenderTarget2D(device, sizex, sizey, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Velocity1 = new RenderTarget2D(device, sizex, sizey, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Force_By_Index = new RenderTarget2D(device, sizex, sizey, false, SurfaceFormat.HdrBlendable, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Force_By_Position_Accumulator = new RenderTarget2D(device, sizex, sizey, false, SurfaceFormat.HdrBlendable, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            
            
            
            Initial_Position.SetData(initial_Positions);
            Position0.SetData(initial_Positions);
            Position1.SetData(initial_Positions);
            Velocity0.SetData(initial_Velocities);
            Velocity1.SetData(initial_Velocities);
        }

        #endregion
        #region util

        /// <summary>
        /// Sets the effects so as to use the right surfaces for the current spring simulation step. 
		/// In other words it handles the read/write buffer switches 
        /// 
        /// </summary>
        /// <param name="Position_n"> Returns the Surface in which to write the new positions</param>
        /// <param name="Velocity_n"> Returns the Surface in which to write the new velocities</param>
        /// <param name="i"> Which of the simulation frame sub steps are we in</param>
        private void Set_Surfaces(out RenderTarget2D Position_n, out RenderTarget2D Velocity_n, int i)
        {
            
            Position_n = (CM.Frame * steps_per_frame + i) % 2 == 0 ? Position0 : Position1;
            Velocity_n = (CM.Frame * steps_per_frame + i) % 2 == 0 ? Velocity0 : Velocity1;
            var Position_o = (CM.Frame * steps_per_frame + i) % 2 == 1 ? Position0 : Position1;
            var Velocity_o = (CM.Frame * steps_per_frame + i) % 2 == 1 ? Velocity0 : Velocity1;
            Update_Position_effect.Parameters["Old_Position"].SetValue(Position_o);
            Update_Position_effect.Parameters["Velocity"].SetValue(Velocity_n);

            Update_Velocity_effect.Parameters["Old_Velocity"].SetValue(Velocity_o);

            Hooke_Force_effect.Parameters["Old_Position"].SetValue(Position_o);
            Hooke_Force_effect.Parameters["Old_Velocity"].SetValue(Velocity_o);

            Draw_the_Grid_effect.Parameters["Position"].SetValue(Position_n);

            Gather_Forces_to_indices.Parameters["Old_Position"].SetValue(Position_o);
        }


        /// <summary>
        /// It does what its long name says, it supposedly refactors the pattern of use of the surfaces of the spring effect, I will probably replace it with something more general
        /// </summary>
        /// <param name="renderTarget">Switch to this surface</param>

        /// <param name="use_this_blending"></param>
        /// <param name="clear_the_target"></param>
        void Draw_With_The_Current_Vertex_and_Index_Buffer_And_The_Provided_Surface_And_Effect
            (RenderTarget2D draw_on_this_surface,
            Effect effect,
            BlendState use_this_blending,
            bool clear_the_target = false)
        {
            device.SetRenderTarget(draw_on_this_surface);
            if (clear_the_target) device.Clear(Color.Black);
            device.BlendState = use_this_blending;
            effect.CurrentTechnique = effect.Techniques[0];

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                 0, 4,
                                                 0, 2);

            }


        }
        #endregion


        public Spring_Effect(GraphicsDevice gd)
        {
            this.device = gd;

            helper = new Help.GPU_Vertex_Helper(sizex, sizey);
            Create_Vertex_Buffers();
            Create_Surfaces();
            force_placement_manager = new Spring_Grid_Force_Placement_Manager(this);
            Create_Effects();
            Create_Debug_Structures();

        }




       
        public void Draw()
        {
        }

        
        public void Pre_Draw()
        {
		    // This call gets the generic instancing code to produce all the instances using the currently inserted handles
            this.force_placement_manager.Instances.Put_Instances_to_Buffers(this.force_placement_manager.Forces);
            
            
            //The set surfaces helper function will put the current simulation step's to be updated position & velocity buffers here
            RenderTarget2D Position_n;
            RenderTarget2D Velocity_n;

            // Repeat for all the simulation steps of this frame
            for (int i = 0; i < steps_per_frame; i++)
            {

                //First we'll draw everything the force_placement_manager has gathered to this surface (and the manager will use instancing)
                
				
				
				//This surface is set on the device before calling the next helper function cause XNA throws a message when attempting
                //to set a surface to a texture when it is used as a "frame buffer" (the current render target)
                device.SetRenderTarget(this.Force_By_Position_Accumulator);
                
                //Find out which surface will get the new position and velocities vs containing the previous step's
                Set_Surfaces(out Position_n, out Velocity_n, i);

                


                
                
                
               
                
                //Ask the manager of the force placement to draw them all
                // the argument tells the manager whether he should clear the dynamic array of "force handles" (what produces the instances)
                // it has gathered by the game code placing forces on the grid.
                
				//It should only be cleared during the last sub frame step of the simulation in this frame
                
				//This call internally sets buffers etc. we only clear the buffer externally 
                force_placement_manager.Draw_All_The_Forces( i == steps_per_frame - 1);


                device.Indices = index_buffer;
                
				//Set the vertexbuffer to the one containing the full screen quad sending the "tx coord identifier" of the mass point
                //to a corresponding texel.
                //It will get used for all the draw calls that perform the simulation,
                //until the final call that actually draws quads for every spring
                device.SetVertexBuffer(full_screen_with_index_info_buffer);
               

                
               

                
                //This will run the effect that gathers the forces from their geometrical areas
                //to the indexes of the mass points whose current position is in that area
                Draw_With_The_Current_Vertex_and_Index_Buffer_And_The_Provided_Surface_And_Effect(this.Force_By_Index,
                                  this.Gather_Forces_to_indices, BlendState.Opaque, true
                                  );

                //This runs the shaders that calculate the hooke forces between the displaced mass points
                //the effect already has the textures etc. it uses set during its initialization
                //and possibly changed if needed in the set_surfaces helper function
                //the blending mode is set to additive (on an HDRBlendable surface)
                //so that the hooke forces don't overwrite the forces put on the point mass in the previous call
                Draw_With_The_Current_Vertex_and_Index_Buffer_And_The_Provided_Surface_And_Effect(this.Force_By_Index,
                                   Hooke_Force_effect, BlendState.Additive
                                   );




                // Velocities get updated (the correct surfaces for the effect where set in the set surfaces helper)
                Draw_With_The_Current_Vertex_and_Index_Buffer_And_The_Provided_Surface_And_Effect(Velocity_n,
                                  Update_Velocity_effect, BlendState.Opaque
                                  );

                
                // Positions get updated (the correct surfaces for the effect where set in the set surfaces helper)
                Draw_With_The_Current_Vertex_and_Index_Buffer_And_The_Provided_Surface_And_Effect(Position_n,
                                 Update_Position_effect, BlendState.Opaque
                                 );

               
            }


            // The simulation finished, now a quad gets drawn for each spring in the array
            //the buffer that will be used knows for each of the spring-quads vertice
            //the "start mass point" and "end mass point" of the string, which are obviously neighbours on the grid.
            //The shader will read their positions and adjust the quad so that it will be drawn on screen as a line connecting
            //the two mass points
            device.SetVertexBuffer(this.draw_lines_buffer);
            device.SetRenderTarget(Post_Procces_Manager.bloom.sceneRenderTarget);
            Draw_the_Grid_effect.Parameters["ViewProjection"].SetValue(Camera.ActiveCamera.View * Camera.ActiveCamera.Projection);
            device.BlendState = BlendState.Additive;
            device.Clear(Color.Black);


            //This kind of foreach loop is an xna thing.
            
			//The call draws 2 triangles per quad per connection(string)
            
            foreach (EffectPass pass in this.Draw_the_Grid_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
              
            
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                                                 0, (sizex - 1) * (sizey - 1) * 2 * 4,
                                                 0, (sizex - 1) * (sizey - 1) * 2 * 2);

            }




            return;


        }




    }

    
}
