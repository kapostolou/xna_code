namespace Bullets
 open Behaviors  
 open System
 open Microsoft.Xna.Framework
 open Shots
  module public Basic_Behaviors=
         let rand=new Random()
         let _uniform ()= (float32)(2.0*(rand.NextDouble()-0.5))
         let _binomial ()= (float32) (rand.NextDouble()-rand.NextDouble())
         let __cos x=(float32)( Math.Cos ((float)x))
         let _cos (w:float32) (n:float32)=  (float32)(Math.Cos((float)w*(float)n))
        
        
        
         let uniform=sample(fun t->Some(_uniform()))
         let binomial=sample(fun t-> Some(_binomial()))
         let cos w= sample(fun t-> Some(_cos w t))
         let COS= sample(fun t-> Some(__cos  t))
         let time=sample(fun t->Some(t))
         
