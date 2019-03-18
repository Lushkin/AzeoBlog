namespace NS.SpaceShooter.Models.Animations
{
    using Microsoft.Xna.Framework.Graphics;

    public class SimpleAnimation
    {
        public SimpleAnimation(Texture2D spriteSheet, float frameTime, int totalFrames)
        {
            SpriteSheet = spriteSheet;
            FrameTime = frameTime;
            FrameIndex = 0;
            TotalFrames = totalFrames;
            FrameWidth = spriteSheet.Width / totalFrames;
            FrameHeight = spriteSheet.Height;
        }


        public Texture2D SpriteSheet { get; set; }
        public float FrameTime { get; set; }
        public int FrameIndex { get; set; }
        public int TotalFrames { get; set; }
        public int FrameHeight { get; set; }
        public int FrameWidth { get; set; }
    }
}
