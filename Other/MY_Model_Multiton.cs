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
    
    public class MY_Model_Multiton
    {

       

        
            public static MY_Model  Colored ;
            public static MY_Model  SimpleCircle ;
            public static MY_Model Enemy ;
            public static MY_Model LaserBlastPlayer ;
            public static MY_Model Torus;
            
        



        
     
        
        
        public static void Initialize()
        {
           Colored = new Models.Model_Types.Colored();
           SimpleCircle = new Models.Model_Types.SimpleCircle();
           Enemy = new Models.Model_Types.Enemy();
           LaserBlastPlayer = new Models.Model_Types.LaserBlastPlayer();
           Torus = new Models.Model_Types.Torus();
          

           

        }

     }
    
}
