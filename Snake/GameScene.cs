//
// GameScene.cs
//
// Author:
//       Yauheni Pakala <evgeniy.pakalo@gmail.com>
//
// Copyright (c) 2018 Yauheni Pakala
//

using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace Snake
{
    public struct CellObj
    {
        public SKShapeNode Node;
        public int X;
        public int Y;

        public CellObj(SKShapeNode node, int x, int y)
        {
            Node = node;
            X = x;
            Y = y;
        }
    }

    public struct Pos
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Pos(int x, int y)
        {
            X = x;
            Y = y;
        }

		public override bool Equals(object obj)
		{
            var p = (Pos)obj;
            return (X == p.X && Y == p.Y);
		}
	}

    public class GameScene : SKScene
    {
        SKLabelNode gameLogo;
        SKShapeNode playButton;
        SKShapeNode gameBG;
        public SKLabelNode currentScore;
        public SKLabelNode bestScore;

        GameManager game;

        public List<Pos> playerPositions = new List<Pos>();
        public List<CellObj> gameArray = new List<CellObj>();

        public CGPoint scorePos;


        public GameScene(CGSize size) : base(size)
        {
            game = new GameManager(this);

            InitializeMenu();

            InitializeGameView();
        }

        public override void DidMoveToView(SKView view)
        {
            var swipeRight = new UISwipeGestureRecognizer(SwipeR);
            swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
            view.AddGestureRecognizer(swipeRight);
                
            var swipeLeft = new UISwipeGestureRecognizer(SwipeL);
            swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
            view.AddGestureRecognizer(swipeLeft);

            var swipeUp = new UISwipeGestureRecognizer(SwipeU);
            swipeUp.Direction = UISwipeGestureRecognizerDirection.Up;
            view.AddGestureRecognizer(swipeUp);

            var swipeDown = new UISwipeGestureRecognizer(SwipeD);
            swipeDown.Direction = UISwipeGestureRecognizerDirection.Down;
            view.AddGestureRecognizer(swipeDown);
        }

        public override void Update(double currentTime)
        {
            game.Update(currentTime);
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
            UpdateBestScoreLabel();
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
            path.AddLines(new[] { topCorner, bottomCorner, middle });
            playButton.Path = path;
            AddChild(playButton);
        }

        void UpdateBestScoreLabel() => bestScore.Text = $"Best Score: {game.SavedBestScore}";

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
                    y: Frame.GetMidY() + (Frame.Size.Height / -2f) + 20),
                0.4));

            playButton.RunAction(SKAction.ScaleTo(0, 0.3),
                () => playButton.Hidden = true);

            gameBG.SetScale(0);
            currentScore.SetScale(0);
            gameBG.Hidden = false;
            currentScore.Hidden = false;
            gameBG.RunAction(SKAction.ScaleTo(1, 0.4));
            currentScore.RunAction(SKAction.ScaleTo(1, 0.4));

            game.InitGame();
        }

        void InitializeGameView()
        {
            currentScore = new SKLabelNode("ArialRoundedMTBold");
            currentScore.ZPosition = 1;
            currentScore.Position = new CGPoint(
                x: Frame.GetMidX(), 
                y: Frame.GetMidY() + (Frame.Size.Height / -2f) + 60);
            currentScore.FontSize = 40;
            currentScore.Hidden = true;
            currentScore.Text = "Score: 0";
            currentScore.FontColor = UIColor.White;
            AddChild(currentScore);

            var width = Frame.Size.Width - 50;
            var height = Frame.Size.Height - 50;
            var rect = new CGRect(
                x: Frame.GetMidX() - width / 2f,
                y: Frame.GetMidY() - height / 2f,
                width: width,
                height: height);
            gameBG = SKShapeNode.FromRect(rect: rect, cornerRadius: 0.02f);
            gameBG.FillColor = UIColor.DarkGray;
            gameBG.ZPosition = 2;
            gameBG.Hidden = true;
            AddChild(gameBG);

            CreateGameBoard(width: width, height: height);
        }

        void CreateGameBoard(nfloat width, nfloat height)
        {
            var cellWidth = 12;
            var numRows = 40;
            var numCols = 20;
            var x = (width / -2f) + (cellWidth / 2f);
            var y = (height / 2f) - (cellWidth / 2f);

            //loop through rows and columns, create cells
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    var cellNode = SKShapeNode.FromRect(new CGSize(width: cellWidth, height: cellWidth));
                    cellNode.StrokeColor = UIColor.Black;
                    cellNode.ZPosition = 2;
                    cellNode.Position = new CGPoint(
                        x: Frame.GetMidX() + x,
                        y: Frame.GetMidY() + y);
                    //add to array of cells -- then add to game board
                    gameArray.Add(new CellObj(cellNode, i, j));
                    gameBG.AddChild(cellNode);
                    //iterate x
                    x += cellWidth;
                }
                //reset x, iterate y
                x = (width / -2f) + (cellWidth / 2f);
                y -= cellWidth;
            }
        }

        public void Finish()
        {
            currentScore.RunAction(SKAction.ScaleTo(0, 0.4), () =>
            {
                currentScore.Hidden = true;
            });

            gameBG.RunAction(SKAction.ScaleTo(0, 0.4), () =>
            {
                gameBG.Hidden = true;
                gameLogo.Hidden = false;

                gameLogo.RunAction(SKAction.MoveTo(
                    new CGPoint(
                        x: Frame.GetMidX(),
                        y: Frame.GetMidY() + (Frame.Size.Height / 2f) - 200),
                    0.5), () =>
                    {
                        playButton.Hidden = false;
                        playButton.RunAction(SKAction.ScaleTo(1, 0.3));

                        UpdateBestScoreLabel();

                        bestScore.RunAction(SKAction.MoveTo(
                        new CGPoint(
                            x: gameLogo.Position.X,
                            y: gameLogo.Position.Y - 50),
                        0.3));
                    });
            });
        }

        void SwipeR() => game.Swipe(3);
        void SwipeL() => game.Swipe(1);
        void SwipeU() => game.Swipe(2);
        void SwipeD() => game.Swipe(4);
    }
}
