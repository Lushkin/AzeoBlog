namespace NS.SpaceShooter.Models.Animations
{
    using Microsoft.Xna.Framework.Graphics;

    public class DoubleAnimation
    {
        public DoubleAnimation(Texture2D spriteSheet, float frameTime, int totalXFrames, int totalYFrames)
        {
            SpriteSheet = spriteSheet;
            FrameTime = frameTime;
            FrameXIndex = 0;
            FrameYIndex = 0;
            TotalXFrames = totalXFrames;
            TotalYFrames = totalYFrames;
            FrameWidth = spriteSheet.Width / totalXFrames;
            FrameHeight = spriteSheet.Height / totalYFrames;
        }


        public Texture2D SpriteSheet { get; set; }
        public float FrameTime { get; set; }
        public int FrameXIndex { get; set; }
        public int FrameYIndex { get; set; }
        public int TotalXFrames { get; set; }
        public int TotalYFrames { get; set; }
        public int FrameHeight { get; set; }
        public int FrameWidth { get; set; }
    }
}
