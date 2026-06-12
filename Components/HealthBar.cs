using System;
using System.Drawing;

namespace Dungeon_Siege
{
    public class HealthBar
    {
        public int MaxValue { get; set; }
        public int CurrentValue { get; set; }
        public int BarWidth { get; set; }
        public int BarHeight { get; set; }
        public int YOffset { get; set; }
        public Brush FillBrush { get; set; }
        public Brush BackgroundBrush { get; set; }

        public HealthBar(int maxValue, Brush fillBrush, Brush backgroundBrush, int barWidth = 40, int barHeight = 4, int yOffset = -10)
        {
            MaxValue = maxValue;
            CurrentValue = maxValue;
            FillBrush = fillBrush;
            BackgroundBrush = backgroundBrush;
            BarWidth = barWidth;
            BarHeight = barHeight;
            YOffset = yOffset;
        }

        public void Sync(int currentValue)
        {
            CurrentValue = currentValue;
        }

        public void Draw(Graphics g, int entityX, int entityY)
        {
            g.FillRectangle(BackgroundBrush, entityX, entityY + YOffset, BarWidth, BarHeight);
            if (MaxValue <= 0) return;

            int fillWidth = (int)(BarWidth * (Math.Max(0, CurrentValue) / (double)MaxValue));
            if (fillWidth > 0)
                g.FillRectangle(FillBrush, entityX, entityY + YOffset, fillWidth, BarHeight);
        }
    }
}
