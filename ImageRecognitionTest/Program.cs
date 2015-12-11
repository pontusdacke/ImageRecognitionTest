using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace ImageRecognitionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateImage();

            // Load image file
            using (var bmp = new Bitmap("image.png"))
            {
                /* Code below detects a single colored circle with the width of 1 pixel in a picture.
                 * The code iterates until it finds the first pixel in a circle while iterating each x pixel in y row.
                 * When pixel is found, we know its the top row of the circle, we can therefore iterate down to the bottom
                 * of the circle to determine center y coordinate and radius/diameter.
                 * While at the bottom we measure the bottom row in pixels and with that info we can get the x coordinate.
                 */

                Color c = bmp.GetPixel(0, 0); // Get the first pixel
                bool foundAnomaly = false; // Flag if we found a circle. Program can only detect one circle.

                // Iterate through the image pixels
                for (int y = 0; y < bmp.Size.Height; y++)
                {
                    for (int x = 0; x < bmp.Size.Width; x++)
                    {
                        if (c != bmp.GetPixel(x, y)) // Is the previous pixel a different color from the current?
                        {
                            foundAnomaly = true;
                            Color? anomalyColor = bmp.GetPixel(x, y);
                            float yCoord = 0;

                            // Find radius and vertical center coordinate
                            float radius = 0;
                            int allowedLineThickness = 15; // Also the minimum diameter
                            for (int h = y + allowedLineThickness; h < bmp.Size.Height; h++)
                            {
                                if (anomalyColor == bmp.GetPixel(x, h))  // Detects color anomaly
                                {
                                    radius = (h - y) / (float)2; // radius is half the diameter hence divide by 2

                                    // Find the vertical center
                                    yCoord = y + radius;

                                    break; // We dont need to iterate more
                                }
                            }

                            // Find the horizontal center coordinate
                            float xCoord = 0;
                            for (int w = x; w < bmp.Size.Width; w++)
                            {
                                if (anomalyColor != bmp.GetPixel(w, y)) // Detects color anomaly
                                {
                                    xCoord = x + ((w - x) / 2);
                                    break; // We dont need to iterate more
                                }
                            }

                            // Draw dot in center of circle
                            using (var gr = Graphics.FromImage(bmp))
                            {
                                gr.FillRectangle(Brushes.Blue, xCoord, yCoord, 1, 1); // DrawRectangle draws 2 pixels minimum, FillRectangle draws 1 pixel
                            }
                        }

                        c = bmp.GetPixel(x, y); // Get the current pixel
                        if (foundAnomaly) break; // Program can only detect 1 circle
                    }
                    if (foundAnomaly) break; // Program can only detect 1 circle
                }

                // Save the image to "out.png"
                var path = "out.png";
                bmp.Save(path);
            }

            Console.WriteLine("Process completed.");
            Console.ReadLine();
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

                    var path = "image.png";
                    bmp.Save(path);
                }
            }
        }
    }
}
