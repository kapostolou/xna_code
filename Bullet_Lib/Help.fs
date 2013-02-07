namespace Bullets
 open Microsoft.Xna.Framework
 open System
 module Help=
    let inline add_angles a b=
            let mutable res=a+b 
            while (res > 2.0f*(float32)Math.PI) do
              res<- res- 2.0f*(float32)Math.PI
            while (res < -2.0f*(float32)Math.PI) do
              res<- res+ 2.0f*(float32)Math.PI
            res
            
    let inline (-**-) (m:Matrix ) (v:Vector2)=Vector2.Transform(v,m)
    let inline (-***-) (m:Matrix ) (v:Matrix)=Matrix.Multiply(m,v)
    let inline lerp_vector2 (v1:Vector2) (v2:Vector2) c= 
            Vector2(MathHelper.Lerp(v1.X, v2.X, c),MathHelper.Lerp(v1.Y, v2.Y, c))
    let public a=5
            

