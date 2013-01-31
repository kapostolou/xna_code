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
	//Currently only the Position0 data are used (and those only in scaling an unrotated quad), ignore the other field
	//This format is used in creating the vertexbuffer of the mesh (really just a quad) of the "force shape" being placed on top of the grid
    public struct Draw_Forces_Vertex_Format
    {
        public Vector3 position_screen_corner_in_clip;
       
	    //UNUSED
        public Vector2 Force;


        public static Draw_Forces_Vertex_Format Create_From_World_Position(Vector3 pos, Vector2 force)
        {
            return new Draw_Forces_Vertex_Format(new Vector3(pos.X / 50, pos.Y / 50, 0), force);
        }
        public Draw_Forces_Vertex_Format(Vector3 position_screen_corner_in_clip, Vector2 force)
        {
            this.position_screen_corner_in_clip = position_screen_corner_in_clip;
            this.Force = force;

        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * (3), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)

             );
        public const int SizeInBytes = sizeof(float) * (3 + 2);
    }


}
