namespace Bullets
 open Behaviors  
 open System
 open Help
 open Microsoft.Xna.Framework
 
 module Shots=
        let v (x, y )= Vector2(x,y)
        type public IRigid =
            abstract Position : unit -> Vector2
            abstract Facing_Direction: unit->Vector2
          

         type vector_pattern= seq<Vector2>
         type public  Shot=
            {Pos:Vector2;Angle:float32}
            static member make_shots f (points:vector_pattern)    =
                    Seq.map f points
            static member default_shot p = {Pos=p;Angle=0.0f}
            static member  make_default_shots (points:vector_pattern)  =
                    Shot.make_shots Shot.default_shot points
            member this.Always = always(this)//Behaviors.sample(fun f -> Some(this))       
        
         let make_shot  o p =
          {Pos=p; Angle=o}
        
         let shot= lift_2 make_shot 
        
        
         
        
        
     
        
       
         
         
        
         
         
       
        
         
        
         
        
        
        
        
       


