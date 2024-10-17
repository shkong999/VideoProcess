using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VideoProcess.Model
{
    public class PictureProcess
    {
        // 이진화
        public Bitmap Binization(Bitmap bitmap)
        {
            // 평균 rgb 계산
            int totalR = 0, totalG = 0, totalB = 0;
            int pixelCount = bitmap.Width * bitmap.Height;
            for(int i = 0; i < bitmap.Width; i++)
            {
                for(int j = 0; j < bitmap.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    totalR += color.R;
                    totalG += color.G;
                    totalB += color.B;
                }
            }

            int averageR = totalR / pixelCount;
            int averageG = totalG / pixelCount;
            int averageB = totalR / pixelCount;

            int average = (averageR + averageG + averageB) / 3;
            //Color thresholdColor = Color.FromArgb(average);
            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // 이진화
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    if(color.R > average || color.G > average || color.B > average)
                    {
                        resultBitmap.SetPixel(i, j, Color.White);
                    }
                    else
                    {
                        resultBitmap.SetPixel(i, j, Color.Black);
                    }
                }
            }
            return resultBitmap;
        }
    }
}
