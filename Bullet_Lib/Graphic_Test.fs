namespace Bullets
 open System
 open System.Windows.Forms
 open System.Drawing
 open Behaviors 
 open Microsoft.Xna.Framework
 open Shots
 open Operators
 open Basic_Behaviors
 open Geometric
 open Behavior_Modifying
 open Lifted
  module Graphic_Test=
        let mutable index=0;
       
        let inner_pattern=circle 6 (always(Vector2(0.0f,0.0f))) (always(10.0f)) (seq{yield always(Vector2(0.0f,0.0f))})
        let multiple=always(10.0f)<*>uniform<*> always(Vector2.UnitX)
        let tmp= ((<+>) multiple)
        let tmp2=Geometric.circle 10 (always(Vector2(300.0f,300.0f))) (always(80.0f)) inner_pattern
        let atmp2=always(tmp2)
        let bobo = fun i-> i|> (start_at 10.0f) |>(end_at 90.0f)
        
        let pattern =   Seq.map   tmp (tmp2) 
        //let tran= Seq.map Behaviors.make_seq pattern
        let pattern2= Seq.map ( bobo)  (pattern)
        let other_pattern= always({Pos=Vector2.Zero;Angle=0.0f})|> (start_at 10.0f) |>(end_at 90.0f)
        let final_pattern = pattern2 |> Seq.map (shot (always(0.0f))) 
        //let final_pattern'=seq{for i in final_pattern do yield! (repeat 3 (always(Vector2(0.0f,-45.0f))) i)} |> Seq.map  (lerp_shot_behaviors 10.0f 40.0f other_pattern)
        let final_pattern'=final_pattern|> (repeat_seq 3 (always(Vector2(0.0f,-45.0f))))  |> Seq.map  (lerp_shot_behaviors 10.0f 40.0f other_pattern)
        let final_pattern23=final_pattern'
        let form = new Form(Size=new Size(600,600),Text="Hello Whuiorld WinForms")
        let setupMenu () =
            let menu = new MenuStrip()
            let button_p = new Button(Text="previous", Dock=DockStyle.Left)
            let button_n = new Button(Text="next", Dock=DockStyle.Right)
            let text= new TextBox(Dock=DockStyle.Bottom)
            button_p.Click.Add(fun _ -> index<- index-1;text.Text<-string(index);form.Invalidate())
            button_n.Click.Add(fun _ ->index<- index+1;text.Text<-string(((float32)index));form.Invalidate())
            button_p, button_n, text

        let drawPoint (g:Graphics) (p:Point) =
          g.DrawEllipse(Pens.Red, p.X - 2, p.Y - 2, 4, 4)

        let paint (g:Graphics)=
          //drawPoint g (Point(200,200))
          for i in final_pattern'   do
           let where = (i.Get ((float32)index))
           //printf "%A" i
           //for point in where do
           match where with 
           |Some(x)->drawPoint g (Point((int)(x.Pos.X),(int)x.Pos.Y))
           |None ->() |>ignore

        do 
            Application.EnableVisualStyles()
            let p,n, c=setupMenu ()
            form.Controls.Add(p)
            form.Controls.Add(n)
            form.Controls.Add(c)
            form.Paint.Add(fun e -> paint(e.Graphics))
            form.Show()  
            Application.Run(form)

