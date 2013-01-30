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
using GELib.Geometry.Geometric_Objects;

namespace GELib.Graphics.Generic_Instancing.Force
{
    /// <summary>
    /// Specializes the template for instancing, with the handle  and instance struct types used specifically for drawing the spring
	/// USes inheritance to create an alias for the template instantation
    /// </summary>
    public class Force_Quad_Instances_Manager : Instances_Manager_Generic<Force_Draw_Handle, Force_Draw_Instance>
    {
        public Force_Quad_Instances_Manager(GraphicsDevice graphics_device, Instancing_Slots_Info info)
            : base(graphics_device, Force_Draw_Instance.VertexDeclaration, Force_Draw_Instance.Size_In_Bytes, info)
        {
        }
    }
}
