namespace Bullets
 open Behaviors  
 open System
 open Shots
 open Microsoft.Xna.Framework
 open Help
 open Operators
 
  module Lifted=
  
         let inline (|*???*>) r p = Behaviors.lift_2 (transfer_shot) r p
         let inline (|*??*>) r p = Behaviors.lift_2 (transfer_shot_light) r p
         let inline (<*>) m n = Behaviors.lift_2 (*) m n
         let inline (<+>) m v = Behaviors.lift_2  (+) m v
         let inline (<->) m v = Behaviors.lift_2  (-) m v
         let inline (</>) m v = Behaviors.lift_2  (/) m v
         let inline (<**>) m v = Behaviors.lift_2  (-**-) m v
         let inline (<***>) m v = Behaviors.lift_2 (-***-) m v
         
         let lerp1  = lift_3 Lerp1
         let inline (<++>) a b = lift_2 add_v_shot a b 
         
         
         
         
         
         let checktypes1=(|*???*>)
         let checktypes2=(|*??*>)
