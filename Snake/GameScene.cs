//
// GameScene.cs
//
// Author:
//       Yauheni Pakala <evgeniy.pakalo@gmail.com>
//
// Copyright (c) 2018 Yauheni Pakala
//
using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace Snake
{
    public class GameScene : SKScene
    {
        SKLabelNode gameLogo;
        SKLabelNode bestScore;
        SKShapeNode playButton;

        GameManager game;


        public GameScene(CGSize size) : base(size)
        {
            InitializeMenu();

            game = new GameManager();
        }

        public override void DidMoveToView(SKView view)
        {
            
        }

        public override void Update(double currentTime)
        {
            // Called before each frame is rendered
        }

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
            foreach (UITouch touch in touches.ToArray<UITouch>())
            {
                var location = touch.LocationInNode(this);
                var touchedNode = GetNodesAtPoint(location);

                foreach (SKNode node in touchedNode)
                {
                    if (node.Name == "play_button")
                    {
                        StartGame();
                    }
                }
            }
        }

		void InitializeMenu()
        {
            //Create game title 
            gameLogo = new SKLabelNode("ArialRoundedMTBold");
            gameLogo.ZPosition = 1;
            gameLogo.Position = new CGPoint(
                x: Frame.GetMidX(),
                y: Frame.GetMidY() + (Frame.Size.Height / 2f) - 200);
            gameLogo.FontSize = 60;
            gameLogo.Text = "SNAKE";
            gameLogo.FontColor = UIColor.Red;
            AddChild(gameLogo);

            //Create best score label
            bestScore = new SKLabelNode("ArialRoundedMTBold");
            bestScore.ZPosition = 1;
            bestScore.Position = new CGPoint(
                x: Frame.GetMidX(),
                y: gameLogo.Position.Y - 50);
            bestScore.FontSize = 40;
            bestScore.Text = "Best Score: 0";
            bestScore.FontColor = UIColor.White;
            AddChild(bestScore);

            //Create play button
            playButton = new SKShapeNode();
            playButton.Name = "play_button";
            playButton.ZPosition = 1;
            playButton.Position = new CGPoint(
                x: 0,
                y: (Frame.Size.Height / -2f) + 200);
            playButton.FillColor = UIColor.Cyan;

            var topCorner = new CGPoint(
                x: Frame.GetMidX() - 50,
                y: Frame.GetMidY() + 50);
            var bottomCorner = new CGPoint(
                x: Frame.GetMidX() - 50,
                y: Frame.GetMidY() - 50);
            var middle = new CGPoint(
                x: Frame.GetMidX() + 50,
                y: Frame.GetMidY());
            var path = new CGPath();
            path.AddLineToPoint(topCorner.X, topCorner.Y);
            path.AddLines(new []{ topCorner, bottomCorner, middle });
            playButton.Path = path;
            AddChild(playButton);
        }

        void StartGame()
        {
            Console.WriteLine("Start game");

            gameLogo.RunAction(SKAction.MoveTo(
                new CGPoint(
                    x: Frame.GetMidX() - 50,
                    y: Frame.GetMidY() + 600),
                0.5),
                () => gameLogo.Hidden = true);

            bestScore.RunAction(SKAction.MoveTo(
                new CGPoint(
                    x: Frame.GetMidX(),
                    y: Frame.GetMidY() + (Frame.Size.Height / -2) + 20),
                0.4));

            playButton.RunAction(SKAction.ScaleTo(0, 0.3),
                () => playButton.Hidden = true);
        }
    }
}
