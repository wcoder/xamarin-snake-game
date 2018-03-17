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
using Foundation;
using UIKit;

namespace Snake
{
    public class GameManager
    {
        readonly GameScene scene;

        double nextTime = 0;
        double timeExtension = 1;

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
                }
            }
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
                    Console.WriteLine(time);
                }
            }
        }
    }
}
