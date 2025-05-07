using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MouseHighlighter
{
    public static class IconGenerator
    {
        public static Icon GenerateIcon()
        {
            int size = 32; // Icon size
            using (Bitmap bmp = new Bitmap(size, size))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Set high quality rendering
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                // Clear background (transparent)
                g.Clear(Color.Transparent);

                // Background glow effect
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(1, 1, size - 2, size - 2);
                    using (var glow = new System.Drawing.Drawing2D.PathGradientBrush(path))
                    {
                        glow.CenterColor = Color.FromArgb(255, 255, 200, 0); // Bright orange-yellow
                        glow.SurroundColors = new Color[] { Color.FromArgb(0, Color.Orange) };
                        g.FillPath(glow, path);
                    }
                }

                // Draw cursor
                int cursorSize = (int)(size * 0.65); // Slightly larger cursor
                int offset = (size - cursorSize) / 2;
                
                // Create cursor shape
                Point[] cursorPoints = new Point[]
                {
                    new Point(offset, offset),                    // Top point
                    new Point(offset, offset + cursorSize),       // Bottom point
                    new Point(offset + cursorSize/2, offset + cursorSize - cursorSize/3) // Inner point
                };

                // Draw cursor shadow
                Point[] shadowPoints = cursorPoints.Select(p => new Point(p.X + 2, p.Y + 2)).ToArray();
                using (var shadowBrush = new SolidBrush(Color.FromArgb(100, Color.Black)))
                {
                    g.FillPolygon(shadowBrush, shadowPoints);
                }

                // Draw cursor with strong outline
                using (var outlinePen = new Pen(Color.Black, 2f))
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPolygon(brush, cursorPoints);
                    g.DrawPolygon(outlinePen, cursorPoints);
                }

                // Draw outer highlight ring
                int outerRingSize = (int)(size * 0.8);
                int outerRingOffset = (size - outerRingSize) / 2;
                using (var outerPen = new Pen(Color.FromArgb(255, 255, 165, 0), 2.5f)) // Orange
                {
                    outerPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    outerPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    g.DrawEllipse(outerPen, outerRingOffset, outerRingOffset, outerRingSize, outerRingSize);
                }

                // Draw inner highlight ring
                int innerRingSize = (int)(size * 0.65);
                int innerRingOffset = (size - innerRingSize) / 2;
                using (var innerPen = new Pen(Color.FromArgb(255, 255, 215, 0), 1.5f)) // Gold
                {
                    innerPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    innerPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    g.DrawEllipse(innerPen, innerRingOffset, innerRingOffset, innerRingSize, innerRingSize);
                }

                // Convert to icon
                IntPtr hIcon = bmp.GetHicon();
                return Icon.FromHandle(hIcon);
            }
        }

        public static void SaveIconToFile(string path)
        {
            using (var icon = GenerateIcon())
            using (var fs = new FileStream(path, FileMode.Create))
            {
                icon.Save(fs);
            }
        }
    }
}
