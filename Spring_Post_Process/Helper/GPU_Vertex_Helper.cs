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


namespace GELib.Help
{

    //This helper class gets initialized with the grid's size, which is also the same as the size of most of the surfaces used in the effect
    //(texels in those and point masses map 1-1) and then provides functions that return the UV or clip coordinates of a mass point specified
    //by row-column integer indices.

    public class GPU_Vertex_Helper
    {
        #region 

        //The number of point masses in each direction
        int sizex;
        int sizey;


        Vector2
            
            //This helps in targeting the center of a texel in the UV coordinates (the texel being identified by integer indices)
            centering_offset_uv_coords,

            //This helps in targeting the center of a texel in the clip coordinates (the texel being identified by integer indices)
            centering_offset_clip_coords,
            
            //These jump between neighbouring texels in clip coordinates
            step_right_clip_coords,
            step_down_clip_coords,

            //These jump between neighbouring texels in uv coordinates
            step_right_uv_coords,
            step_down_uv_coords;
        
        //The above vectors get initialized by the constructor supplying the size of the grid in point masses

        //The top left vertex of a full screen quad
        Vector2 Top_Left_vertex_in_clip_coordinates = new Vector2(-1, 1);

        #endregion


        #region Helpers
        /// <summary>
        /// These functions return the UV/clip coordinates of a texel from their row/column integer address
        /// The row column addresses start (0, 0 ) at the top left texel then proceed right and down
        /// </summary>
        
        //Returns the upper left vertex clip coordinates of the texel
        public Vector3 Clip_Coords_of_point_UL(int i, int j)
        {
            var res = Top_Left_vertex_in_clip_coordinates  + i * step_right_clip_coords + j * step_down_clip_coords;
            return new Vector3(res.X, res.Y, 0.2f);
        }

        //Returns the center point clip coordinates of the texel
        public Vector3 Clip_Coords_of_point_Center(int i, int j)
        {
            var res = Top_Left_vertex_in_clip_coordinates + centering_offset_clip_coords + i * step_right_clip_coords + j * step_down_clip_coords;
            return new Vector3(res.X, res.Y, 0.2f);
        }

        public Vector3 World_Coords_of_point_UL(int i, int j)
        {
            var res = Top_Left_vertex_in_clip_coordinates /*+ centering_offset_rc*/ + i * step_right_clip_coords + j * step_down_clip_coords;
            return new Vector3(50 * res.X, 50 * res.Y, -10f);
        }
        public Vector3 World_Coords_of_point_Center(int i, int j)
        {
            var res = Top_Left_vertex_in_clip_coordinates + centering_offset_clip_coords + i * step_right_clip_coords + j * step_down_clip_coords;
            return new Vector3(50 * res.X, 50 * res.Y, -10f);
        }

        //Returns the upper left vertex uv coordinates of the texel
        public Vector2 TX_Coords_of_point_UL(int i, int j)
        {
            var res = new Vector2(0, 0) + i * step_right_uv_coords + j * step_down_uv_coords;
            return res;
        }

        //Returns the center point uv coordinates of the texel
        public Vector2 TX_Coords_of_point_Center(int i, int j)
        {
            var res = new Vector2(0, 0) + centering_offset_uv_coords + i * step_right_uv_coords + j * step_down_uv_coords;
            return res;
        }

       


        
        //Used in a loops, gets the texel's neighbours based on an integer from 0-3
        public Vector2 Find_neighbour_in_tx_coords(int i)
        {
            switch (i)
            {
                case 0: return new Vector2((1 / (float)sizex),0);
                case 1: return new Vector2(0,-(1 / (float)sizey));
                //case 2: return new Vector2(0, (1 / (float)sizex));
                //case 3: return new Vector2((1 / (float)sizex), 0);
                default: throw new Exception();
            }
        }


        static Matrix texture_to_clip = new Matrix(2, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public static Vector2 Convert_tx_coords_to_clip(Vector2 tx_coords)
        {
            return new Vector2(2 * tx_coords.X, -2 * tx_coords.Y) + new Vector2(-1, 1);
            //return Vector2.Transform(tx_coords, texture_to_clip) + new Vector2(-1, 1);
        }
        
        #endregion


        public GPU_Vertex_Helper(int sizex,int sizey)
        {
            //Initialize the variables using the grid's size

            this.sizex = sizex;
            this.sizey = sizey;
            Matrix m = new Matrix(2, 0, 0, 0, 0, -2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            centering_offset_uv_coords = new Vector2(1 / (2f * sizex), 1 / (2f * sizey));
            centering_offset_clip_coords = Vector2.Transform(centering_offset_uv_coords, m);
            step_right_clip_coords = new Vector2(2 * 1 / (float)sizex, 0);
            step_down_clip_coords = new Vector2(0, -2 * 1 / (float)sizey);
            step_right_uv_coords = new Vector2(1 / (float)sizex, 0);
            step_down_uv_coords = new Vector2(0, 1 / (float)sizey);
        }
    }
}
