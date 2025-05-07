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

                // Clear background (transparent)
                g.Clear(Color.Transparent);

                // Draw cursor
                int cursorSize = size - 8;
                Point[] cursorPoints = new Point[]
                {
                    new Point(4, 4),                    // Top point
                    new Point(4, 4 + cursorSize),       // Bottom point
                    new Point(4 + cursorSize/3, 4 + cursorSize - cursorSize/3) // Inner point
                };

                // Draw cursor outline
                using (var pen = new Pen(Color.Black, 2))
                {
                    g.DrawPolygon(pen, cursorPoints);
                }

                // Fill cursor
                using (var brush = new SolidBrush(Color.White))
                {
                    g.FillPolygon(brush, cursorPoints);
                }

                // Draw highlight circle
                using (var pen = new Pen(Color.Yellow, 2))
                {
                    g.DrawEllipse(pen, 
                        4 + cursorSize/4, 
                        4 + cursorSize/4, 
                        cursorSize/2, 
                        cursorSize/2);
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
