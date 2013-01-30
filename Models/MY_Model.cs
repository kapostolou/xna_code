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

    // This class is not of much use right now. XNA already has a class used to load and manipulate 3d Models
    // the MY_Model class is eventually going to either encapsulate or reproduce it
    // however so far I am mostly using just quads and for efficiency concerns (which might actually turn out to be non valid)
    // I am using something that doesn't have any tree based hierarchy or functionality in general. 
	// The silly name is there to indicate that it's not really doing anything.
    public class MY_Model
    {

       
        public Texture2D Texture;
        
        // The My_Effect class encapsulates an XNA effect making some of its arguments C# properties
		// XNA has EffectParameter objects accessed from the effect object by a string dictionary, this Wrapper
		// gives direct reference to some of those EffectParameters
        public Help.My_Effect effect_wrapper;
        
        public IndexBuffer index_buffer;
        
        public BlendState blend;
        public VertexBuffer mesh;
        
        //if the just_mesh flag is not set then the model will use a Model_Mesh_Part which is an XNA object that
        //more or less encapsulates the data needed for a single draw call on part of the mesh of an object
        public ModelMeshPart mesh_part;
        public bool just_mesh;
        

        
        public MY_Model()
        {
            just_mesh = true;
            this.mesh = Meshes.Render.OBB_Textured.obb_vb;
            this.index_buffer = Meshes.Render.OBB_Textured.index_buffer;
        }

    }

}
