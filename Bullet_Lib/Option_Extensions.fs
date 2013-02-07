namespace Bullets
 open Microsoft.Xna.Framework
 open System
 module Option_Extensions=
    let inline lift_option f a =
           match a with
           |Some(x)-> Some(f x)
           |_ ->None  
    let inline lift_options f a b=
           match a, b with
           |Some(x), Some(y) -> Some(f x y)
           |_ ->None
    let inline lift_options3 f a b c=
           match a, b, c  with
           |Some(x), Some(y),Some(z) -> Some(f x y z)
           |_ ->None
 

