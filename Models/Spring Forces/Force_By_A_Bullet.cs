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

namespace GELib.Models.Model_Types.Force
{
    class Force_By_A_Bullet:MY_Model
    {
        public Force_By_A_Bullet()
            {
                var effect = CM.game1.Content.Load<Effect>("Spring/Spring_Draw_Forces_I").Clone();
                effect.CurrentTechnique = effect.Techniques[0];

                mesh = GELib.Meshes.Force.Draw_Forces.draw_force_vertexbuffer;
                index_buffer = GELib.Meshes.Force.Draw_Forces.draw_force_Index_Buffer;
                
                effect_wrapper = new Help.My_Effect(effect);
                blend = BlendState.Additive;
            }
        

    }
}
