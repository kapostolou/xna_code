namespace Bullets
 open Behaviors  
 open System
 open Shots
 open Microsoft.Xna.Framework
 open Help
  module Operators=
         
         let inline in_rigid (r:IRigid) p= 
            let looking= r.Facing_Direction()
            let angle= (float32) (Math.Atan2((float)looking.Y , (float)looking.X))
            let transform= Matrix.CreateRotationZ(angle)
            r.Position() + (transform  -**-  p)
         
         let inline transfer_shot (p:Shot) (r:Shot)= 
            {Pos=r.Pos+ (Matrix.CreateRotationZ(r.Angle)  -**- p.Pos);
              Angle=(add_angles r.Angle  p.Angle)}
         let inline transfer_shot_light (p:Shot) (r:Shot)= 
            {Pos=r.Pos+ (Matrix.CreateRotationZ(r.Angle)  -**- p.Pos);
              Angle=(p.Angle)}
         let add_v_shot (v:Vector2) sh =
          {Pos=v+sh.Pos; Angle=sh.Angle}
         
         let inline Lerp1 shot2 this (c:float32)=
                    {Pos=(Help.lerp_vector2 this.Pos shot2.Pos c);Angle=this.Angle} 
             
        
      
        
         let inline from_tupple ((tup1:Behavior<'a>),(tup2:Behavior<'b>))=
          sample ( fun f->
          match (tup1.Get f),(tup2.Get f) with
          |Some (x), Some(y)->Some(x,y)
          |_->None)
       
         //not needed?
         //let inline (|&???&>) (arg:Behavior<'a option>) (f:Behavior<('a option->'b option) option>)  = 
         //  sample (
         //           fun n-> 
         //            match (f.Get   n) with
          //           |Some(fin)->(arg.Get n) |> fin 
          //           |_->None
         //         )
                    
         //let inline (|&?&>) (arg:Behavior<'a option>) (f:Behavior< ('a option)>->Behavior<('b option)>)  = 
          //arg|>f
          
         let inline (|&&>) a b = 
          (lift_2 (|>)) a b
         
         
         let check1=  (|&&>)
         //let check2=  (|&???&>)
         
         //let huiyhi= (<&?&|>>)           
         //let inline (<+&&>) vec vp =  (vec (<+>) ) <&&> vp
         //let inline (<***&&>) mat vp =  (mat (<***>) ) <&&> vp
         //let inline (<*!*&&>) r vp =  (r  (<*!*>) ) <&&> vp
          //let inline transform_ (matr:Matrix) (points:Particles.vector_pattern)  = Seq.map (fun p->(Vector2.Transform(p,matr))) points
           //let _pertube vec (points:Particles.vector_pattern)  = Seq.map (fun p->p+vec) points
         //let inline (<+&>) vec vp = Behaviors.lift_2 _pertube vec vp
         
         //let inline (<&&!>) f points = Behaviors.lift_2 Seq.map f points
         //let inline (<&&>) f points = sample (fun n-> Seq.map f (points))
         //let inline (<&&|>>) (point:Behavior<'a>) (f:Behavior<'a->'b>)  = sample (fun n-> (get point n) |> (get f  n ) )
         
         
         //let inline (<*&>) vec vp = Behaviors.lift_2 transform_ vec vp
         
         //let inline (<*!*>) r p = Behaviors.lift_2  (lift_options(in_rigid)) r p
         //let inline (<*?*>) r p = Behaviors.lift_2 (lift_options(in_shot)) r p
         //let inline (<*>) m n = Behaviors.lift_2 (lift_options(*)) m n
         //let inline (<+>) m v = Behaviors.lift_2  (lift_options(+)) m v
         //let inline (<->) m v = Behaviors.lift_2  (lift_options(-)) m v
         //let inline (</>) m v = Behaviors.lift_2  (lift_options(/)) m v
         //let inline (<**>) m v = Behaviors.lift_2  (lift_options(-**-)) m v
         //let inline (<***>) m v = Behaviors.lift_2 (lift_options(-***-)) m v

        

