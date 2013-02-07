namespace Bullets
    open System
    open Microsoft.Xna.Framework
    open Option_Extensions
    
    module Behaviors=
      type Behavior<'a>=
        | BH of (float32-> 'a )        
        member inline this.Get t =
           let (BH ( f ))=this
           f t
        member inline this.ToSeq =
           seq {yield this}
      
      //let inline get (BH ( f )) t  = 
        //f t
        
      let inline sample f = BH(f)
      
        
    
     
      
        
      let inline lift_1 f (x:Behavior<'a>) = sample (fun t-> 
          let xt= x.Get t
          
          (lift_option f) xt ) 
        
      let inline lift_2 f (x:Behavior<'a>) (y:Behavior<'b>)= sample (fun t-> 
          let xt= x.Get t
          let yt= y.Get t
          (lift_options f) xt yt) 
      let inline lift_3 f (x:Behavior<'a>) (y:Behavior<'b>) (z:Behavior<'c>)= sample (fun t-> 
          let xt= x.Get t
          let yt= y.Get t
          let zt= z.Get t
          (lift_options3 f) xt yt zt)      
          
      let inline always (r)= sample ( fun f-> Some(r))
     
      
        
      
      
      //let lift_3 f x y z= sample (fun t-> 
          //let xt= (get x t)
          //let yt= (get y t)
          //let zt= (get z t)
          //f xt yt zt)
      
     //let inline lift_options_tupple f (a, b)=
           //match a, b with
           //|Some(x), Some(y) -> Some(f (x, y))
           //|_ ->None
             
             
      
             
   
    
        
        //let inline get  (BH ( f )) t  = 
        //let (xx)=f t
        //Option.get( xx)
        //let inline always_opt (r)= sample ( fun f->r)