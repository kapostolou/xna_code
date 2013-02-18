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
    public class Polygon
    {


        //Adjust this to rotate the polygon
        public float Angle { get{return _angle;} set{_angle=value; _Orientation_Matrix=Matrix.CreateRotationZ(value); }}
        //Cahce the construction of the Oriantation_Matrix from the Angle
        public Matrix Orientation_Matrix {get{return _Orientation_Matrix;}}

        //The points defining the polygon : MUST be in ccw order
        public List<Vector3> Points;

        //The normals of the polygon's edges, normal i is the normal of the edge from points i to i+1 with wrap around at the last point
        public List<Vector3> Normals;
        
        //This is supposed to cache the Local Space angle of each normal starting from the 0 normal and going ccw
        public List<float> Angles;
        
        //References a hierarchical data structure that guides a query to a direction's most extreme vertex
        //by comparing against the angles (defined above) of normals
        public Polygon_Tree_Node Subdivision;

        //The number of points defining the polygon
        public int vertices_no;

        //The angle starting from the x axis going ccw of the 0 normal
        public float Beginning_Normal_Angle;


        /// <summary>
        /// Returns the angle starting from the polygon's 0 normal and going counterclockwise until specified the direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public float Find_Adjusted_Angle(Vector3 direction)
        {
            return Helper.Fix_Angle(Helper.Get_Zero_Two_Pi_Angle(direction), Beginning_Normal_Angle) - Beginning_Normal_Angle;
        }

        /// <summary>
        /// Use the Set function to correctly create the polygon and it's spatial decomposition
        /// </summary>
        public Polygon()
        {
            Points = new List<Vector3>();
            Normals = new List<Vector3>();
            Angles = new List<float>();
            Angle = 0;
        }

        /// <summary>
        /// Uses the specified points to initialize the polygon's data
        /// </summary>
        /// <param name="points_inserted"></param>
        public void Set(List<Vector3> points_inserted)
        {
            vertices_no = points_inserted.Count;
            if (points_inserted.Count < 3) throw new Exception("not enough points to make a polygon");
            var winding = Vector3.Cross(points_inserted[0], points_inserted[1]).Z;
            if (winding < 0)
                for (int i = 0; i < points_inserted.Count; i++) points_inserted[i] = new Vector3(-points_inserted[i].X, points_inserted[i].Y, 0);
            Points.Clear();
            Normals.Clear();
            Angles.Clear();
            for (int i = 0; i < points_inserted.Count; i++)
            {
                Points.Add(points_inserted[i]);
            }
            for (int i = 0; i < Points.Count - 1; i++)
            {
                var point1 = Points[i];
                var point2 = Points[i + 1];
                var vec = point2 - point1;
                var normal = Vector3.Cross(vec, Vector3.Backward);
                normal.Normalize();
                Normals.Add(normal);
            }
            var point1_ = Points[Points.Count - 1];
            var point2_ = Points[0];
            var vec_ = point2_ - point1_;
            var normal_ = Vector3.Cross(vec_, Vector3.Backward);
            normal_.Normalize();
            Normals.Add(normal_);
            var TwoPi = (float)(2 * Math.PI);
            var begin_angle = Helper.Get_Zero_Two_Pi_Angle(Normals[0]);
            Beginning_Normal_Angle = begin_angle;
            for (int i = 0; i < Normals.Count; i++)
            {
                var angle = Helper.Get_Zero_Two_Pi_Angle(Normals[i]);
                angle = Helper.Fix_Angle(angle, begin_angle);
                angle -= begin_angle;
                Angles.Add(angle);
            }
            Angles.Add(TwoPi);


            var res = Subdivide(0, vertices_no);
            this.Subdivision = res;
        }


        /// <summary>
        /// Sends the n vertex back to the beginning
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public int Vertex(int vertex)
        {
            return vertex % vertices_no;
        }


        /// <summary>
        /// Populate the Subdivision member with a spatial hierarchical structure that helps locate an extreme vertex in 
        /// log(Vertices_Number) steps
        /// 
        /// The arguments that start the recursion should be (0 ,total_vertices) and correspond to polygon edge normals
        /// that define a sub-piece of the unit disc
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private Polygon_Tree_Node Subdivide(int start, int end)
        {
                        

            bool is_subdivision_direction = true;
            
            //this is what will be returned upwards the recursion 
            var node = new Polygon_Tree_Node(this);
            
            //this predicate means we've reached a node that is just supposed to add 2 vertex leaf nodes and return
            if (end - start == 1)
            {
                node.code = start;
                var left_vertex_node = new Polygon_Tree_Node(this);
                left_vertex_node.code = this.Vertex(end);
                node.link_l = left_vertex_node;
                var right_vertex_node = new Polygon_Tree_Node(this);
                right_vertex_node.code = this.Vertex(start);
                node.link_r = right_vertex_node;

                return node;
            }

            //in the recursion's main case we just find the middle normal in this subpiece of the unit disc
            // (the middle in the sense of the sequence defined by the polygon edges outwards normals starting
            // at the vertex 0 - vertex 1 edge and going ccw)
            // then recursively call the function on the 2 sub pieces defined by start,middle   middle, end
            var middle = Helper.Middle(start, end);
            node.left_part_start_normal = start;
            node.left_part_end_normal = middle;
            node.right_part_end_normal = end;
            node.code = middle;
            node.link_l = Subdivide(start, middle);
            node.link_r = Subdivide(middle, end);
            node.division_direction = is_subdivision_direction;
            return node;






        }


        public void Draw()
        {
            var vertices = this.Points;
            if (vertices.Count < 2) return;
            Vector3 start, end;
            for (int i = 0; i < vertices.Count - 1; i++)
            {
                start = Local_To_World(vertices[i]);// Vector3.Transform(vertices[i], Matrix.Transpose(Orientation_Matrix));
                end = Local_To_World(vertices[i+1]); //Vector3.Transform(vertices[i + 1], Matrix.Transpose(Orientation_Matrix));
                General_VPC_Buffer.Draw_Segment(start, end, Color.White);


            }
            start = Local_To_World(vertices[vertices.Count - 1]); //Vector3.Transform(vertices[vertices.Count - 1], Matrix.Transpose(Orientation_Matrix));
            end = Local_To_World(vertices[0]);//Vector3.Transform(vertices[0], Matrix.Transpose(Orientation_Matrix));
                
            General_VPC_Buffer.Draw_Segment(start, end, Color.White);



        }

        //These 2 are supposed to be optimized by the compiler, they transform vectors from the local to the world coordinate system and vice versa
        public Vector3 Local_To_World(Vector3 vec)
        {
            return Vector3.Transform(vec, Matrix.Transpose(Orientation_Matrix));
        }
        public Vector3 World_To_Local(Vector3 vec)
        {
            return Vector3.Transform(vec, Orientation_Matrix);
        }

        //private members
        private float _angle;
        private Matrix _Orientation_Matrix;
        

    }

}
