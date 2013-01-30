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
using GELib.Models;

namespace GELib
{

    public class Slot_Info_Multiton
    {




    

        public static Instancing_Slots_Info distort_slot_info;
        public static Instancing_Slots_Info Spring_Grid_Force_Drawing_Slot_Info;









        public static void Initialize()
        {
           
            distort_slot_info = new Slot_Info.Distort.Distort_Slot_Info_1();
            Spring_Grid_Force_Drawing_Slot_Info = new Slot_Info.Force.Force_Draw_Info();




        }

    }

}
