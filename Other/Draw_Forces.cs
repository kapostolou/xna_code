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

namespace GELib.Meshes.Force
{
    public class Draw_Forces
    {
        public static VertexBuffer draw_force_vertexbuffer;
        public static IndexBuffer draw_force_Index_Buffer;
        private static  Draw_Forces_Vertex_Format[] draw_force_vertices;
        public static void Initialize()
        {

            draw_force_Index_Buffer = new IndexBuffer(CM.game1.GraphicsDevice, typeof(ushort), 6, BufferUsage.WriteOnly);

            draw_force_Index_Buffer.SetData(new ushort[] { 0, 1, 2, 0, 2, 3 });
            draw_force_vertices = new Draw_Forces_Vertex_Format[4];
            draw_force_vertices[0] = new Draw_Forces_Vertex_Format(new Vector3(-1, 1, 0.5f), new Vector2(0, 0));
            draw_force_vertices[1] = new Draw_Forces_Vertex_Format(new Vector3(-1, -1, 0.5f), new Vector2(0, 1));
            draw_force_vertices[2] = new Draw_Forces_Vertex_Format(new Vector3(1, -1, 0.5f), new Vector2(1, 1));
            draw_force_vertices[3] = new Draw_Forces_Vertex_Format(new Vector3(1, 1, 0.5f), new Vector2(1, 0));
            draw_force_vertexbuffer = new VertexBuffer(CM.game1.GraphicsDevice, Draw_Forces_Vertex_Format.VertexDeclaration,
                                                  4, BufferUsage.WriteOnly);
            draw_force_vertexbuffer.SetData(0, draw_force_vertices, 0, 4, Draw_Forces_Vertex_Format.SizeInBytes);

        }
    }
}
