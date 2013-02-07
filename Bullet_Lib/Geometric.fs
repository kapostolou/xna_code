namespace Bullets
 open Behaviors  
 open System
 open Microsoft.Xna.Framework
 open Shots
 open Operators
 open Lifted
 open Option_Extensions
 module public Geometric=
         let inline line (s:Vector2) (e:Vector2) n=
          //let length=(e-s).Length()
          let step=1.0f/((float32)n)
          seq{
            for i in 0..n-1 do
            yield always(s*(1.0f-step*(float32)i)+e*step)
            }
        
         let inline circle_line n (center:Behavior<Vector2 option>) (radius:Behavior<float32 option>) (start_r:Behavior<float32 option>) (end_r:Behavior<float32 option>) (points)=
          let step=1.0f/((float32)n)
          seq{
            for i in 0..n-1 do
               for j in points do
                let step=(end_r<->start_r)</>always((float32)n)
                let rot=start_r<+>always((float32)i)<*>step
                let adj1=always(new Vector2(0.0f,1.0f))<*>radius<+>j
                //let transform=always(lift_option(Matrix.CreateRotationZ:float32->Matrix))
                //let transform2=always(lift_option(Vector2.Transform:Vector2*Matrix->Vector2))
                
                let transform2'=lift_1 (Vector2.Transform:Vector2*Matrix->Vector2)
                let transform'=lift_1 (Matrix.CreateRotationZ:float32->Matrix)
                
                let adj2=rot |> ( transform')
                //let adj2'=rot |&?&> ( transform')
                
                //let final_adj= (from_tupple (adj1, adj2) )|&&> transform2 
                let final_adj'= (from_tupple (adj1, adj2) )|> transform2' 
                
                yield final_adj'<+>center
                  
            } 
            
         let circle  n (center:Behavior<Vector2 option>) (radius:Behavior<float32 option>)  points=
          circle_line  n center  radius (always (0.0f)) (always(2.0f*(float32)MathHelper.Pi)) points
         
         let inline repeat reps  (v:Behavior<Vector2 option>) (shot1:Behavior<Shot option>) =
                  seq{ for i in 0..reps do yield (always((float32)i)<*>v)<++>shot1}
         
         let inline repeat_seq reps  (v:Behavior<Vector2 option>) (shots:Behavior<Shot option> seq) =
                  seq{ for i in shots do yield! (repeat reps v i)}
   
         

