namespace Bullets
    open System
    open Microsoft.Xna.Framework
    open Behaviors
    open Shots
    open Operators
    open Lifted
    module Behavior_Modifying=
      let scale w (BH(f)) = sample (fun t->   f(w*t)) 
      
      
      let inline start_at (time:float32) (BH(f)) = sample(fun t-> if (t<time) then None else f(t))
      
      let inline end_at (time:float32) (BH(f)) = sample(fun t-> if (t>time) then None else f(t))
      
      let inline shift n (BH(f))  = sample(fun t-> f(t-n))
      
      let inline lerp_shot_behaviors f1 f2 (shot1:Behavior<Shot option>) (shot2:Behavior<Shot option>)=
       let inner= (sample (fun t1->Some((t1-f1)/(f2-f1))))
       let tmp=lerp1 (shot1) (shot2) (inner)
       sample(
         fun t->
           if (t<f1)
           then (shot1.Get t)
           else if (t>f2)
           then (shot2.Get t)
           else 
           (tmp.Get t)
            
         )
         
     
            
          
         
      //let inline (<*!**>) f1 f2  s1 s2=   lerp_shot_behaviors f1 f2  s1 s2

