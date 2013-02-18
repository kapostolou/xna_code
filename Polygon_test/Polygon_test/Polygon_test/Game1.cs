using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Polygon_Test_Library;


namespace Polygon_test
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Effect World_Effect;
        Polygon poly;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = General_VPC_Buffer.PIXELS;
            graphics.PreferredBackBufferHeight = General_VPC_Buffer.PIXELS;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

       

       
       
       
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        /// 
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            General_VPC_Buffer.device = GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("SpriteFont1");
            World_Effect = Content.Load<Effect>("World_Effect").Clone();
            World_Effect.Parameters["World"].SetValue(Matrix.Identity);
            World_Effect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(0, 0, -10), new Vector3(0, 0, 0), new Vector3(0, 1, 0)));
            World_Effect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(-10, 10, 6, 1000));
            GraphicsDevice.BlendState = BlendState.Additive;

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

        
            poly = new Polygon();
            List<Vector3> points;
        
            points = new List<Vector3>();
            //points.Add(new Vector3(1, 1, 0));
            //points.Add(new Vector3(-1, 1, 0));
            //points.Add(new Vector3(-1, -1, 0));
            //points.Add(new Vector3(1, -1, 0));

            points.Add(new Vector3(1, 1, 0));
            points.Add(new Vector3(0.5f, 1.5f, 0));
            points.Add(new Vector3(-0.5f, 1.5f, 0));
            points.Add(new Vector3(-1, 1, 0));
            points.Add(new Vector3(-1, -1, 0));
            points.Add(new Vector3(-0.5f, -1.5f, 0));
            points.Add(new Vector3(0.5f, -1.5f, 0));
            points.Add(new Vector3(1, -1, 0));

            poly.Set(points);
            
            
           
        }

      

        bool wait_mouse_to_depress = false;
        List<Vector3> points_gui = new List<Vector3>();
        bool points_gui_complete = false;
        Vector3 Last_Mouse_Position;

        /// <summary>
        /// This gets called by the main loop as an update step
        /// 
        /// </summary>
        protected override void Update(GameTime gameTime)
        {

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)   this.Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Y == ButtonState.Pressed) poly.Angle += 0.075f;
            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) poly.Angle -= 0.075f;
            var mouseStateCurrent = Mouse.GetState();
            var X_ = mouseStateCurrent.X;
            var Y_ = mouseStateCurrent.Y;
            Last_Mouse_Position = Helper.Convert_To_Vector(X_, Y_);

            // Move the sprite to the current mouse position when the left button is pressed
            if (!points_gui_complete)
                if (!wait_mouse_to_depress)
                {
                    if (mouseStateCurrent.LeftButton == ButtonState.Pressed)
                    {
                        var X = mouseStateCurrent.X;
                        var Y = mouseStateCurrent.Y;
                        wait_mouse_to_depress = true;
                        var point = Helper.Convert_To_Vector(X, Y);
                        Last_Mouse_Position = point;
                        if ((points_gui.Count == 0) || ((point - points_gui[0]).LengthSquared() > 0.1f))
                            points_gui.Add(point);
                        else
                        {
                            points_gui_complete = true;
                        }
                    }
                }
                else
                {

                    if (mouseStateCurrent.LeftButton != ButtonState.Pressed)
                    {
                        wait_mouse_to_depress = false;
                    }
                }


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        
        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.BlendState = BlendState.Additive;
           
            GraphicsDevice.Clear(Color.Black);

            World_Effect.Techniques[0].Passes[0].Apply();
            poly.Draw();
            var lftStickMove = new Vector3(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, 0);
            General_VPC_Buffer.Draw_Segment(new Vector3(0, 0.0f, 0), lftStickMove, Color.Yellow);
            lftStickMove.Normalize();
            var extreme = poly.Subdivision.Extreme_Vertex_In_Direction(lftStickMove);

            General_VPC_Buffer.Draw_Segment(new Vector3(0, 0.0f, 0), extreme, Color.White);

            if (points_gui_complete != true && points_gui.Count > 0)
            {
                for (int i = 0; i < points_gui.Count - 1; i++)
                {
                    General_VPC_Buffer.Draw_Segment(points_gui[i], points_gui[i + 1], Color.White);
                }
                General_VPC_Buffer.Draw_Segment(points_gui[points_gui.Count - 1], Last_Mouse_Position, Color.White);
            }
            if (points_gui_complete)
            {
                poly.Set(points_gui);
                points_gui.Clear();
                points_gui_complete = false;
           
            }
           
            poly.Subdivision.Draw(0);

            spriteBatch.Begin();
            if(lftStickMove.LengthSquared()>0.0001f)
            spriteBatch.DrawString(font,
                "Direction : "+lftStickMove + "  \n"+"Extreme Vertex :  "+ extreme+"  \n"+"Projection Distance : "+Vector3.Dot(lftStickMove,extreme), 
                new Vector2(3,3), Color.White,
                0, new Vector2(3, 3), 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();



            base.Draw(gameTime);
        }

    }
}
