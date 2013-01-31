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
    //Used as vertex data in the vertex buffer that gets sent to the GPU and transformed there into "line sprites"
    //between neigbouring point masses of the grid
    public struct Spring_Drawing_Vertex_Format
    {
        /// <summary>
        /// Just the vertices of a full screen normalized coordinates quad that will be modified in the Vertex Shader
        /// </summary>
        public Vector3 normalized_position;
        
        /// <summary>
        /// Indicates which corner of the drawn quad this vertex represents (tl, tr, bl etc.). This is actually not needed (you could use the normalized_position too)
        /// </summary>
        public Vector2 corner;
        
        /// <summary>
        /// The index (texture coordinate, texel) of the the first in the pair of spring array elements forming the drawn quad,
        /// This is also used in the Vertex Shader (Spring_Draw_Grid) to transform the normalized_position, and thus position the spring's obb
        /// </summary>
        public Vector2 point_mass_index;
        
        /// <summary>
        /// The index (texture coordinate) of the the second in the pair of spring array elements forming the drawn quad
        /// </summary>
        public Vector2 point_mass_index_2;


        //The last two correspond to/specify a spring from the grid

      

        public Spring_Drawing_Vertex_Format(Vector3 position_screen_corner_in_normalized,
            Vector2 text_corner, Vector2 mass_index, Vector2 mass_index_2)
        {
            this.normalized_position = position_screen_corner_in_normalized;
            this.corner = text_corner;
            this.point_mass_index = mass_index;
            this.point_mass_index_2 = mass_index_2;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * (3), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                 new VertexElement(sizeof(float) * (3 + 2), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
                 new VertexElement(sizeof(float) * (3 + 2 + 2), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2)
                
             );
        public const int SizeInBytes = sizeof(float) * (3 + 2 + 2 + 2 );
    }
}
