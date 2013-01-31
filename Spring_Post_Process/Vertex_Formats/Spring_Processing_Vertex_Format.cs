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
    /// <summary>
    /// Represents the vertices of just a full screen quad that will send texture coordinates to pixel shaders via interpolation.
    /// There the coordinates will, in the hlsl pixel shader code, represent a mass point via a  1-1 bijection
	// (hopefully unless something is too unstable with how the hardware does the POINT sample texture addressing)
    /// 
    /// </summary>
    public struct Spring_Processing_Vertex_Format
    {
        /// <summary>
        /// The normalized coordinates of the vertices of a full screen quad covering the entire screen
        /// The idea is that as the full screen quad gets drawn on top the render surface,
        /// the tx-coordinates of its edges will be interpolated to the pixel buffer
        /// so that for each pixel, which will correspond to a point mass in the spring-mass grid,
        /// the interpolated tx coords will act as an index to the array of point masses
        /// and the pixel shader can use that index to perform spring force calculation, and velocity/position integration for that
        /// particular point mass.
        /// 
		
		
		/// (
        /// This vertex format is also used in gathering forces drawn as quads in geometrical parts of the grid
        /// (parts of the grid as in geometrically, that is the drawn force is put in some geometrical location 
        /// and is supposed to act to any mass point currently placed within it).
        /// 
        /// - All the quads representing forces are drawn to an 'accumulator' render surface using their geometrical location.
        /// - Then the pixel shaders receive the interpolated tx coords of the full screen quad,
        /// - each pixel/fragment getting the "adress" of a single point mass, 
		//  - the pixel shader reads the current position of the point mass 
        /// - the pixel shader also reads forces that have been additively blended to the "geometrical" accumulator render surface, 
        /// - then outputs these gathered forces
        /// - so that subsequent hlsl effects will add the hooke law forces and then perform the integration
		///  See the Spring_Force_Gather hlsl effect
        /// )
		
        /// </summary>
        public Vector3 full_screen_quad_vertices_normalized;

        /// <summary>
        /// The TX coordinates of the vertices of a full screen quad. When interpolated they represent a point mass to a pixel shader
        /// 
        /// </summary>
        public Vector2 text_corner;



        public Spring_Processing_Vertex_Format(Vector3 position_screen_corner_in_clip, Vector2 text_corner/*, Vector2 mass_index*/)
        {
            this.full_screen_quad_vertices_normalized = position_screen_corner_in_clip;
            this.text_corner = text_corner;
            
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                 new VertexElement(sizeof(float) * (3), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                
             );
        public const int SizeInBytes = sizeof(float) * (3 + 2  );
    }


   

}
