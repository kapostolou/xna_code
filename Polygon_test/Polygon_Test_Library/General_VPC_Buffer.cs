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
    public class General_VPC_Buffer
    {
        public static GraphicsDevice device;
        const int TOTAL_QUAD_VERTICES = 2400;
        
        //It doesn't really get modularized with the vertex buffer, it is the backbuffer size in pixels
        public const int PIXELS = 800;

        //Contains points 
        public static Vector3[] buffer_v = new Vector3[500];
        //Points get transformed to elements of this array, and this array is used with the index buffer to draw rectangles
        public static VertexPositionColor[] buffer = new VertexPositionColor[4000];
        public static int[] index_buffer_quad = new int[TOTAL_QUAD_VERTICES];
       

        static General_VPC_Buffer()
        {

            //Populate the index buffer so that each 2 consecutive vertices in the vertex buffer define a rectangle   
            for (int i = 0; i < TOTAL_QUAD_VERTICES / 6; i++)
            {
                index_buffer_quad[i * 6] = i * 4;
                index_buffer_quad[i * 6 + 1] = i * 4 + 1;
                index_buffer_quad[i * 6 + 2] = i * 4 + 2;
                index_buffer_quad[i * 6 + 3] = i * 4;
                index_buffer_quad[i * 6 + 4] = i * 4 + 2;
                index_buffer_quad[i * 6 + 5] = i * 4 + 3;
            }

           
        }





        public static void Draw_Arc(float radius, float start, float end, Color color)
        {
            //A slow method to draw circle arcs, I wrote the program in 2 days though, it divides a circle
            //in linear segments for each 2 * Math.PI / 720 degree step, and draws the segment
            
            float step = (float)(2 * Math.PI / 720);
            float current_angle = start;
            while (current_angle < end)
            {
                var point0 = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationZ(current_angle)) * radius;
                current_angle += step;
                var point1 = Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationZ(current_angle)) * radius;
                Draw_Segment(point0, point1, color);
            }
        }

        public static void Draw_Segment(Vector3 start, Vector3 end, Color Color)
        {

            //Gets 2 points, finds the normal of the vector between them, rotates it by 90 degrees, scales it and adds it 
            //to the points to create the 4 vertices of a rectangle that will be drawn for this segment
            //I could have drawn DirectX Lines but the XNA API doesn't let you control the width
            //In addition normally the draw call would only be called after all the frame's segments have been batched
            int vertex_counter = 0;
            var width = 0.01f;
            var point1 = end;
            var point0 = start;
            var vector = point1 - point0;
            if (vector.LengthSquared() < 0.0000000001f) return;
            var katheti = new Vector3(vector.Y, -vector.X, 0);
            katheti.Normalize();
            katheti *= -1 * width;
            General_VPC_Buffer.buffer[vertex_counter++] = new VertexPositionColor(point0 - katheti, Color);
            General_VPC_Buffer.buffer[vertex_counter++] = new VertexPositionColor(point0 + katheti, Color);
            General_VPC_Buffer.buffer[vertex_counter++] = new VertexPositionColor(point1 + katheti, Color);
            General_VPC_Buffer.buffer[vertex_counter++] = new VertexPositionColor(point1 - katheti, Color);



            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, buffer, 0, 4, index_buffer_quad, 0, 2);



        }


        public static void Draw_Direction(Vector3 dir, Color Color, float dir_scale)
        {
            //similar to Draw_Segment
            int vertex_counter = 0;
            var width = 0.01f * dir_scale;
            var point1 = dir_scale * dir; ;
            var point0 = Vector3.Zero;
            var vector = point1 - point0;
            if (vector.LengthSquared() < 0.0000000001f) return;
            var katheti = new Vector3(vector.Y, -vector.X, 0);
            katheti.Normalize();
            katheti *= -1 * width;
            General_VPC_Buffer.buffer[vertex_counter++] = new VertexPositionColor(point0 - katheti, Color);
            General_VPC_Buffer.buffer[vertex_counter++] = new VertexPositionColor(point0 + katheti, Color);
            General_VPC_Buffer.buffer[vertex_counter++] = new VertexPositionColor(point1 + katheti, Color);
            General_VPC_Buffer.buffer[vertex_counter++] = new VertexPositionColor(point1 - katheti, Color);



            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, buffer, 0, 4, index_buffer_quad, 0, 2);



        }



      


    }
}
