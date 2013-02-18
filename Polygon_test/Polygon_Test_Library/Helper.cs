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

namespace Polygon_Test_Library
{
    public class Helper
    {


        //Finds the angle from the right x axis vector (1,0) until the direction turning ccw and given in the 0 - TwoPi interval
        public static float Get_Zero_Two_Pi_Angle(Vector3 dir)
        {
            var TwoPi = (float)(2 * Math.PI);
            var angle = (float)Math.Atan2(dir.Y, dir.X);
            if (angle <= 0) angle += TwoPi;
            return angle;
        }

        //This one is supposed to work when processing the polygons normals sequentially
        //the process finds the first normal's angle
        //and then fixes (using this function) the angles of the other normals so that they are greater than the starting normal's
        //to simulate moving ccw through the unit circle
        public static float Fix_Angle(float angle, float begin_angle)
        {
            var TwoPi = (float)(2 * Math.PI);
            if (angle < begin_angle) angle += TwoPi;
            return angle;
        }



        /// <summary>
        /// What vertex code lies in the middle
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static int Middle(int v1, int v2)
        {
            var ret = v1 + (v2 - v1) / 2;
            return ret;
        }


        //Used by the UI code to scale and transform the pixel given by reading the MouseState
        // to a vector in the coordinate system used by the program
        public static Vector3 Convert_To_Vector(int x, int y)
        {
            var X = (1.0f / General_VPC_Buffer.PIXELS) * x;
            var Y = (1.0f / General_VPC_Buffer.PIXELS) * y;
            return 5 * UV_Vector_to_Clip_Vector(new Vector3(X, Y, 0));

        }
        public static Vector3 UV_Vector_to_Clip_Vector(Vector3 UV_v)
        {
            UV_v -= new Vector3(0.5f, 0.5f, 0);
            return new Vector3(2 * UV_v.X, -2 * UV_v.Y, 0);
        }

    }
}
