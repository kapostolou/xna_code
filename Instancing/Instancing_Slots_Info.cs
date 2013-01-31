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

namespace GELib
{

    /// <summary>
    /// Used by the Generic Instancing template to configure what each of its slots will contain.
    /// 
    /// This class isn't supposed to be used directly but subclassed, with the subclass constructor filling up all the data members.
    /// </summary>
    public class Instancing_Slots_Info
    {
        //How many slots to use
        public int Number_Of_Slots;
        
        //A "model" for each slot (see the MY_Model class's comments)
        public MY_Model[] slots;

        public Instancing_Slots_Info(int total)
        {
            Number_Of_Slots = total;
            slots = new MY_Model[Number_Of_Slots];
        }
    }
	
	 //This configures the generic Instancing slots used when rendering of force sprites
    class Force_Draw_Info : Instancing_Slots_Info
    {
        public Force_Draw_Info()
            : base(1)
        {
            //Just one slot (and the model class Force_By_A_Bullet really just uses a quad)
            this.slots[0] = new Model_Types.Force.Force_By_A_Bullet();
            

        }
    }

}
