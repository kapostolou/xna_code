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
    public class Polygon_Tree_Node
    {
        //Used to specify a normal or a vertex depending on whether the tree node is a leaf
        public int code;
        public bool division_direction;

        //codes to the polygon's normal desribing the piece of unit circle this node corresponds too (when not a leaf node)
        //the left part start is the earliest going ccw from the polygon's 0 normal
        //left part end is in the middle and with left part start id defines the subpiece that goes left on the tree
        //left part end with right part end define the right subpiece
        public int left_part_start_normal;
        public int left_part_end_normal;
        public int right_part_end_normal;
        
        //the tree structure's links
        public Polygon_Tree_Node link_l;
        public Polygon_Tree_Node link_r;
       
        //the polygon this structure belongs to
        public Polygon polygon;

        public Polygon_Tree_Node(Polygon poly)
        {
            polygon = poly;
            left_part_start_normal = -1;
            left_part_end_normal = -1;
            right_part_end_normal = -1;
        }
       

        public void Draw(int level)
        {
            //Draw all the normals in the root call
            if (level == 0)
            {
                for (int i = 0; i < polygon.Normals.Count; i++)
                {
                    General_VPC_Buffer.Draw_Direction(Vector3.Transform(polygon.Normals[i], Matrix.Transpose(polygon.Orientation_Matrix)), Color.Blue, 1);
                }
            }

            //return without dawing anything and end the recursion if the node is a leaf
            if ((link_l == null) && (link_r == null)) return;
            if (!this.division_direction) return;

            //draw the early ccw and late ccw parts of the arc in different colors
            General_VPC_Buffer.Draw_Arc(4 - 0.4f * level, -polygon.Angle + polygon.Beginning_Normal_Angle + polygon.Angles[this.left_part_start_normal], -polygon.Angle + polygon.Beginning_Normal_Angle + polygon.Angles[this.left_part_end_normal], Color.Red);
            General_VPC_Buffer.Draw_Arc(4 - 0.4f * level, -polygon.Angle + polygon.Beginning_Normal_Angle + polygon.Angles[this.left_part_end_normal], -polygon.Angle + polygon.Beginning_Normal_Angle + polygon.Angles[this.right_part_end_normal], Color.Blue);
            
            //recurse
            if ((link_l != null)) link_l.Draw(level + 1);
            if ((link_r != null)) link_r.Draw(level + 1);
        }








        //This wraps the  _Extreme_Vertex_In_Direction function by sending the direction to the local space and moving the result back to world space
        //and also by caching the computation of the direction's "ccw starting from the 0 normal" angle
        public Vector3 Extreme_Vertex_In_Direction(Vector3 direction)
        {
            direction = polygon.World_To_Local(direction);
            float dir_angle = polygon.Find_Adjusted_Angle(direction);
            var ret = _Extreme_Vertex_In_Direction(dir_angle, 0);
            return polygon.Local_To_World(ret);
        }




        ///Gets the polygon's extreme vertex in the specified direction
        ///(the direction is specified converted it to an adjusted angle startig from the 0 normal and going ccw)
        ///
        ///Geometrically for any direction the extreme part of the polygon will either be a single vertex or an edge
        ///
        ///In the case of the edge as an extreme element only one of its end vertices is returned
        ///since for collision detection etc. what is usually needed is how far the vertex projects in the direction
        ///(when an edge is extreme it is normal to the direction and then all its points project the same as the
        ///returned vertex)
        ///
        ///the level code is only really needed for "drawing" the query and this could have been done better via a callback etc.
        ///
        ///The function is recursive
        private Vector3 _Extreme_Vertex_In_Direction(float dir_angle, int level = 0)
        {
            //the predicate indicates a leaf, which means the recursion stops
            if ((link_l == null) && (link_r == null)) return polygon.Points[code];
            else
            {

                //this predicate indicates that the node only contains one normal
                // (it only works if the predicate (link_l == null) && (link_r == null) is already true)
                //if this is true then the query compares the direction's angle to the normal's angle
                // and if it is more ccw starting from the 0 normal the vertex (leaf) is to the right
                // if its is less ccw starting from the 0 normal the vertex (leaf) is to the left
                if (this.left_part_start_normal == -1)
                {
                    var test_angle = polygon.Angles[this.code];
                    var angle = dir_angle;
                    if (angle >= test_angle) return link_l._Extreme_Vertex_In_Direction(dir_angle);
                    else return link_r._Extreme_Vertex_In_Direction(dir_angle);
                }
                else
                {
                    //here the node doesn't have just one normal
                    // in this case it contains the ccw starting from the 0 normal angles of 3 normals
                    // and the direction is supposed to be querried against the left piece defined by the first two
                    //and the right piece defined by the last two normals

                    var start_left_test = polygon.Angles[this.left_part_start_normal];
                    var start_right_test = polygon.Angles[this.left_part_end_normal];
                    var angle = dir_angle;
                    var is_Left_edge_ok = angle >= start_left_test;
                    var is_Right_edge_ok = angle <= start_right_test;
                    
                    //Draw the arc chosen in yellow
                    if (is_Left_edge_ok && is_Right_edge_ok)
                    {

                        if (this.left_part_start_normal != -1 && this.left_part_end_normal != -1)
                            General_VPC_Buffer.Draw_Arc(4 - 0.4f * level - 0.07f, -polygon.Angle + polygon.Beginning_Normal_Angle + polygon.Angles[this.left_part_start_normal], -polygon.Angle + polygon.Beginning_Normal_Angle + polygon.Angles[this.left_part_end_normal], Color.Yellow);

                        return link_l._Extreme_Vertex_In_Direction(dir_angle, level + 1);

                    }
                    else
                    {
                        if (this.left_part_end_normal != -1 && this.right_part_end_normal != -1)
                            General_VPC_Buffer.Draw_Arc(4 - 0.4f * level - 0.07f, -polygon.Angle + polygon.Beginning_Normal_Angle + polygon.Angles[this.left_part_end_normal], -polygon.Angle + polygon.Beginning_Normal_Angle + polygon.Angles[this.right_part_end_normal], Color.Yellow);

                        return link_r._Extreme_Vertex_In_Direction(dir_angle, level + 1);

                    }
                }
            }
        }

      
    }
}
