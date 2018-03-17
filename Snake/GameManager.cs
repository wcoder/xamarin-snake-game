//
// GameManager.cs
//
// Author:
//       Yauheni Pakala <evgeniy.pakalo@gmail.com>
//
// Copyright (c) 2018 Yauheni Pakala
//

using System;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Snake
{
    public class GameManager
    {
        readonly GameScene scene;

        int currentScore;

        double nextTime = 0;
        double timeExtension = 0.15;

        int playerDirection = 4;

        Random random = new Random();


        public GameManager(GameScene scene)
        {
            this.scene = scene;
        }

        public void InitGame()
        {
            scene.playerPositions.Add(new Pos(10, 10));
            scene.playerPositions.Add(new Pos(10, 11));
            scene.playerPositions.Add(new Pos(10, 12));

            RenderChange();

            GenerateNewPoint();
        }

        void RenderChange()
        {
            foreach (var cell in scene.gameArray)
            {
                if (scene.playerPositions.Any(x => x.X == cell.X && x.Y == cell.Y))
                {
                    cell.Node.FillColor = UIColor.Cyan;
                }
                else
                {
                    cell.Node.FillColor = UIColor.Clear;

                    if (IsHeadOnEat(cell.Y, cell.X))
                    {
                        cell.Node.FillColor = UIColor.Red;
                    }
                }
            }
        }

        void GenerateNewPoint()
        {
            (var randomX, var randomY) = GenPointPos();

            while (scene.playerPositions.Any(p =>
                p.X == Math.Round(randomX) &&
                p.Y == Math.Round(randomY)))
            {
                (randomX, randomY) = GenPointPos();
            }

            scene.scorePos = new CGPoint(x: randomX, y: randomY);
        }

        public void Update(double time)
        {
            if (Math.Abs(nextTime) < 0)
            {
                nextTime = time + timeExtension;
            }
            else
            {
                if (time >= nextTime)
                {
                    nextTime = time + timeExtension;

                    UpdatePlayerPosition();

                    CheckForScore();

                    CheckForDeath();

                    FinishAnimation();
                }
            }
        }

        void UpdatePlayerPosition()
        {
            var xChange = -1;
            var yChange = 0;

            switch (playerDirection)
            {
                case 1:
                    //left
                    xChange = -1;
                    yChange = 0;
                    break;
                case 2:
                    //up
                    xChange = 0;
                    yChange = -1;
                    break;
                case 3:
                    //right
                    xChange = 1;
                    yChange = 0;
                    break;
                case 4:
                    //down
                    xChange = 0;
                    yChange = 1;
                    break;
                case 0:
                    //dead
                    xChange = 0;
                    yChange = 0;
                    break;
                default:
                    break;
            }

            if (scene.playerPositions.Count > 0)
            {
                var start = scene.playerPositions.Count - 1;
                while (start > 0)
                {
                    scene.playerPositions[start] = scene.playerPositions[start - 1];
                    start -= 1;
                }
                scene.playerPositions[0] = new Pos(scene.playerPositions[0].X + yChange, scene.playerPositions[0].Y + xChange);
            }

            if (scene.playerPositions.Count > 0)
            {

                var pos = scene.playerPositions[0];

                var x = pos.Y;
                var y = pos.X;

                if (y > 40)
                {
                    pos.X = 0;
                }
                else if (y < 0)
                {
                    pos.X = 40;
                }
                else if (x > 20)
                {
                    pos.Y = 0;
                }
                else if (x < 0)
                {
                    pos.Y = 20;
                }
            }

            RenderChange();
        }

        void CheckForScore()
        {
            if (scene.playerPositions.Count > 0)
            {
                var x = scene.playerPositions[0].X;
                var y = scene.playerPositions[0].Y;

                // TODO:
                if (IsHeadOnEat(y, x))
                {
                    currentScore += 1;
                    scene.currentScore.Text = $"Score: {currentScore}";
                    GenerateNewPoint();

                    // add to snake
                    scene.playerPositions.Add(scene.playerPositions.Last());
                    scene.playerPositions.Add(scene.playerPositions.Last());
                    scene.playerPositions.Add(scene.playerPositions.Last());
                }
            }
        }

        void CheckForDeath()
        {
            if (scene.playerPositions.Count > 0)
            {
                var arrayOfPositions = scene.playerPositions.ToList();
                var headOfSnake = arrayOfPositions[0];

                arrayOfPositions.RemoveAt(0);

                if (arrayOfPositions.Any(p => p.Equals(headOfSnake)))
                {
                    playerDirection = 0;
                }
            }
        }

        public void Swipe(int id)
        {
            if (!(id == 2 && playerDirection == 4) &&
                !(id == 4 && playerDirection == 2))
            {
                if (!(id == 1 && playerDirection == 3) &&
                    !(id == 3 && playerDirection == 1))
                {
                    if (playerDirection != 0)
                    {
                        playerDirection = id;
                    }
                }
            }
        }

        void FinishAnimation()
        {
            if (playerDirection == 0 && scene.playerPositions.Count > 0)
            {
                var hasFinished = true;
                var headOfSnake = scene.playerPositions[0];

                foreach (var position in scene.playerPositions)
                {
                    if (!headOfSnake.Equals(position))
                    {
                        hasFinished = false;
                    }
                }

                if (hasFinished)
                {
                    Console.WriteLine("End game");

                    UpdateScore();

                    playerDirection = 4;

                    //animation has completed
                    scene.scorePos = CGPoint.Empty;
                    scene.playerPositions.Clear();
                    RenderChange();

                    //return to menu
                    scene.Finish();
                }
            }
        }

        void UpdateScore()
        {
            var bestScore = SavedBestScore;
            if (currentScore > bestScore)
            {
                NSUserDefaults.StandardUserDefaults.SetInt(currentScore, AppDelegate.BEST_SCORE_KEY);
            }
            currentScore = 0;
            scene.currentScore.Text = "Score: 0";
            scene.bestScore.Text = $"Best Score: {SavedBestScore}";
        }

        (double x, double y) GenPointPos() =>
            (random.NextDouble() * 19,  // cols
            random.NextDouble() * 39);  // rows

        public nint SavedBestScore =>
            NSUserDefaults.StandardUserDefaults.IntForKey(AppDelegate.BEST_SCORE_KEY);

        bool IsHeadOnEat(int x, int y) =>
            Math.Round(scene.scorePos.X) == x &&
            Math.Round(scene.scorePos.Y) == y;
    }
}
