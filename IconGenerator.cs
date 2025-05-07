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

                // Draw glow effect
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(2, 2, size - 4, size - 4);
                    using (var glow = new System.Drawing.Drawing2D.PathGradientBrush(path))
                    {
                        glow.CenterColor = Color.FromArgb(180, Color.Yellow);
                        glow.SurroundColors = new Color[] { Color.FromArgb(0, Color.Yellow) };
                        g.FillPath(glow, path);
                    }
                }

                // Draw cursor
                int cursorSize = (int)(size * 0.6); // Slightly smaller cursor
                int offset = (size - cursorSize) / 2;
                
                // Create cursor shape
                Point[] cursorPoints = new Point[]
                {
                    new Point(offset, offset),                    // Top point
                    new Point(offset, offset + cursorSize),       // Bottom point
                    new Point(offset + cursorSize/2, offset + cursorSize - cursorSize/3) // Inner point
                };

                // Draw cursor shadow
                Point[] shadowPoints = cursorPoints.Select(p => new Point(p.X + 1, p.Y + 1)).ToArray();
                using (var shadowBrush = new SolidBrush(Color.FromArgb(80, Color.Black)))
                {
                    g.FillPolygon(shadowBrush, shadowPoints);
                }

                // Draw cursor outline and fill
                using (var pen = new Pen(Color.FromArgb(64, 64, 64), 1.5f))
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPolygon(brush, cursorPoints);
                    g.DrawPolygon(pen, cursorPoints);
                }

                // Draw highlight ring
                int ringSize = (int)(size * 0.7);
                int ringOffset = (size - ringSize) / 2;
                using (var pen = new Pen(Color.FromArgb(255, 220, 0), 2))
                {
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    g.DrawEllipse(pen, ringOffset, ringOffset, ringSize, ringSize);
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
