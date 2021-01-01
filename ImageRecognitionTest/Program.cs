using System;
using System.Drawing;
using System.IO;

namespace ImageRecognitionTest
{
    /// <summary>
    /// Code below detects a single colored circle with the width of 1 pixel in a picture.
    /// The code iterates until it finds the first pixel in a circle while iterating each x pixel in y row.
    /// When pixel is found, we know its the top row of the circle, we can therefore iterate down to the bottom
    /// of the circle to determine center y coordinate and radius/diameter.
    /// While at the bottom we measure the bottom row in pixels and with that info we can get the x coordinate.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            CreateImage();

            using (var image = new Bitmap("image.png"))
            {
                Color backgroundColor = image.GetPixel(0, 0);
                bool foundAnomaly = false;

                for (int y = 0; y < image.Size.Height; y++)
                {
                    for (int x = 0; x < image.Size.Width; x++)
                    {
                        if (backgroundColor != image.GetPixel(x, y))
                        {
                            foundAnomaly = true;
                            Color? anomalyColor = image.GetPixel(x, y);
                            float centerY = FindVerticalCenter(image, y, x, anomalyColor);
                            float centerX = FindHorizontalCenter(image, y, x, anomalyColor);
                            MarkCenter(image, centerY, centerX);
                        }

                        backgroundColor = image.GetPixel(x, y);
                        if (foundAnomaly) break;
                    }
                    if (foundAnomaly) break;
                }

                image.Save("result.png");
            }

            Console.WriteLine("Process completed.");
        }

        private static void MarkCenter(Bitmap bmp, float yCoord, float xCoord)
        {
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.FillRectangle(Brushes.Blue, xCoord, yCoord, 2, 2);
            }
        }

        private static float FindHorizontalCenter(Bitmap bmp, int y, int x, Color? anomalyColor)
        {
            float xCoord = 0;
            for (int w = x; w < bmp.Size.Width; w++)
            {
                if (anomalyColor != bmp.GetPixel(w, y))
                {
                    xCoord = x + (w - x) / 2;
                    break;
                }
            }

            return xCoord;
        }

        private static float FindVerticalCenter(Bitmap bmp, int y, int x, Color? anomalyColor)
        {
            float yCoord = 0;
            int allowedLineThickness = 15;
            for (int h = y + allowedLineThickness; h < bmp.Size.Height; h++)
            {
                if (anomalyColor == bmp.GetPixel(x, h))
                {
                    float radius = (h - y) / (float)2;
                    yCoord = y + radius;
                    break;
                }
            }

            return yCoord;
        }

        private static void CreateImage()
        {
            if (!File.Exists("image.png"))
            {
                Console.WriteLine("Image not found. Creating 'image.png'");
                using (var bmp = new Bitmap(100, 100))
                {
                    using (var gr = Graphics.FromImage(bmp))
                    {
                        gr.FillRectangle(Brushes.White, 0, 0, 100, 100);
                        gr.DrawEllipse(Pens.Red, 10, 10, 80, 80);
                    }

                    bmp.Save("image.png");
                }
            }
        }
    }
}
