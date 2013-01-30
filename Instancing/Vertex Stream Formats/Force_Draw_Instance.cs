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

namespace GELib.Graphics.Generic_Instancing.Force
{

    //Used as vertex data in an instancing stream
    public struct Force_Draw_Instance 
    {
        /// <summary>
        /// Where the force should be drawn, in normalized coordinates
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// Shows what direction the object placing the force was moving
        /// </summary>
        public Vector2 Direction;
        
        /// <summary>
        /// Normalized Coordinates size of the quad (later generally a mesh) that will get drawn 
        /// </summary>
        public float Halfaxis;

        /// <summary>
        /// A float modifying the force's magnitude
        /// </summary>
        public float Magnitude;
       

        public static VertexDeclaration VertexDeclaration = new VertexDeclaration
      (

          new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
          new VertexElement(8, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2),
          new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3),
          new VertexElement(20, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 4)
         

      );

        public static int Size_In_Bytes = sizeof(float) * (6);


    }
}
