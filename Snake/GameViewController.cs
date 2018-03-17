//
// GameViewController.cs
//
// Author:
//       Yauheni Pakala <evgeniy.pakalo@gmail.com>
//
// Copyright (c) 2018 Yauheni Pakala
//
using System;

using SpriteKit;
using UIKit;

namespace Snake
{
    public partial class GameViewController : UIViewController
    {
        protected GameViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Configure the view.
            var skView = (SKView)View;
            skView.ShowsFPS = true;
            skView.ShowsNodeCount = true;
            /* Sprite Kit applies additional optimizations to improve rendering performance */
            skView.IgnoresSiblingOrder = true;

            // Create and configure the scene.
            var scene = new GameScene(skView.Bounds.Size)
            {
                ScaleMode = SKSceneScaleMode.AspectFill
            };

            // Present the scene.
            skView.PresentScene(scene);
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? UIInterfaceOrientationMask.AllButUpsideDown : UIInterfaceOrientationMask.All;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }
    }
}
