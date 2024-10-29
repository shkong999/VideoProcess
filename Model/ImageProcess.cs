using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Drawing.Color;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace VideoProcess.Model
{
    public class ImageProcess
    {
        // 이진 이미지 확인
        public unsafe bool CheckBinary(byte* pBitmap, int width, int height, int stride)
        {
            for(int y =0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    byte pixelValue = pBitmap[y * stride + x];

                    // 0 : 검은색 255 : 흰색
                    if(pixelValue != 0 && pixelValue != 255)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /* 팽창
         *  구조적 요소(커널) 정의 > 팽창 적용
         */
        public unsafe Bitmap Expansion(byte* pBitmap, Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int bytesPerPixel = 0;

            if (bitmap.PixelFormat == PixelFormat.Format32bppRgb || bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                bytesPerPixel = 4;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                bytesPerPixel = 1;
            }

            // 새로운 비트맵 생성
            Bitmap newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (bytesPerPixel == 4)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else if (bytesPerPixel == 1)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            }

            BitmapData bmpData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.WriteOnly, newBitmap.PixelFormat);
            byte* pNewBitmap = (byte*)bmpData.Scan0.ToPointer();

            // 구조 요소 (3x3 커널)
            int[,] kernel = new int[,]
            {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
            };

            for(int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // 최대값 저장
                    byte maxValue = 0;

                    // kernel size > 3x3
                    for (int kernelX = -1; kernelX <= 1; kernelX++)
                    {
                        for (int kernelY = -1; kernelY <= 1; kernelY++)
                        {
                            int newX = x + kernelX;
                            int newY = y + kernelY;

                            // 경계 체크
                            if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                            {
                                int pixelIndex = newY * width * bytesPerPixel + newX * bytesPerPixel;
                                byte pixelValue = pBitmap[pixelIndex];
                                if (pixelValue > maxValue)
                                {
                                    maxValue = pixelValue;
                                }
                            }
                        }
                    }
                    int outputIndex = y * width * bytesPerPixel + x * bytesPerPixel;
                    pNewBitmap[outputIndex] = maxValue;
                    if(bytesPerPixel == 4)
                    {
                        pNewBitmap[outputIndex + 1] = maxValue;
                        pNewBitmap[outputIndex + 2] = maxValue;
                        pNewBitmap[outputIndex + 3] = 255;
                    }
                }
            }
            newBitmap.UnlockBits(bmpData);

            return newBitmap;
        }

        /* 수축
         * 
         */
        public unsafe Bitmap Shrinkage(byte* pBitmap, Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int bytesPerPixel = 0;

            if (bitmap.PixelFormat == PixelFormat.Format32bppRgb || bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                bytesPerPixel = 4;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                bytesPerPixel = 1;
            }

            // 새로운 비트맵 생성
            Bitmap newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (bytesPerPixel == 4)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else if (bytesPerPixel == 1)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            }

            BitmapData bmpData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.WriteOnly, newBitmap.PixelFormat);
            byte* pNewBitmap = (byte*)bmpData.Scan0.ToPointer();

            // 구조 요소 (3x3 커널)
            int[,] kernel = new int[,]
            {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
            };

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // 최소값 저장
                    byte minValue = 255;

                    // kernel size > 3x3
                    for (int kernelX = -1; kernelX <= 1; kernelX++)
                    {
                        for (int kernelY = -1; kernelY <= 1; kernelY++)
                        {
                            int newX = x + kernelX;
                            int newY = y + kernelY;

                            // 경계 체크
                            if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                            {
                                int pixelIndex = newY * width * bytesPerPixel + newX * bytesPerPixel;
                                byte pixelValue = pBitmap[pixelIndex];
                                if (pixelValue < minValue)
                                {
                                    minValue = pixelValue;
                                }
                            }
                        }
                    }
                    int outputIndex = y * width * bytesPerPixel + x * bytesPerPixel;
                    pNewBitmap[outputIndex] = minValue;
                    if (bytesPerPixel == 4)
                    {
                        pNewBitmap[outputIndex + 1] = minValue;
                        pNewBitmap[outputIndex + 2] = minValue;
                        pNewBitmap[outputIndex + 3] = 255;
                    }
                }
            }
            newBitmap.UnlockBits(bmpData);

            return newBitmap;
        }

        /* 히스토그램 평활화
         *  이미지의 히스토그램 생성 > 누적 분포 히스토그램 계산 > 누적 값을 정규화 > 이미지 생성
         * 
         */
        public unsafe Bitmap Smoothing(byte* pBitmap, Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            double[] histogram = new double[256];
            double[] cdfHistogram = new double[256];
            int[] equalHistogram = new int[256];

            int bytesPerPixel = 0;
            if (bitmap.PixelFormat == PixelFormat.Format32bppRgb)
            {
                bytesPerPixel = 4;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                bytesPerPixel = 1;
            }

            // 새로운 비트맵 생성
            Bitmap newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (bytesPerPixel == 4)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else if (bytesPerPixel == 1)
            {
                newBitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
                ColorPalette palette = newBitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i); // 흑백 팔레트
                }
                newBitmap.Palette = palette;
            }

            // 비트맵 데이터를 잠금
            BitmapData bmpData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.WriteOnly, newBitmap.PixelFormat);
            byte* pNewBitmap = (byte*)bmpData.Scan0.ToPointer();

            // 히스토그램 생성
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte* pixel = pBitmap + (y * width + x) * bytesPerPixel;
                    byte histValue = 0;

                    if (bytesPerPixel == 4)
                    {
                        // 평균값 계산
                        histValue = (byte)((pixel[2] + pixel[1] + pixel[0]) / 3);
                    }
                    else if (bytesPerPixel == 1)
                    {
                        histValue = (byte)pixel[0];
                    }

                    histogram[histValue]++;
                }
            }

            // 히스토그램의 확률 계산
            int totalPixel = bitmap.Width * bitmap.Height;
            for (int i = 0; i < histogram.Length; i++)
            {
                histogram[i] = histogram[i] / totalPixel;
            }

            // 누적 히스토그램 계산
            cdfHistogram[0] = histogram[0];
            for (int i = 1; i < histogram.Length; i++)
            {
                cdfHistogram[i] = (cdfHistogram[i - 1] + histogram[i]);
            }

            // 누적 히스토그램에 최빈값을 곱한 뒤 평활화 히스토그램 계산
            for (int i = 0; i < cdfHistogram.Length; i++)
            {
                equalHistogram[i] = (int)Math.Round(cdfHistogram[i] * 255);
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte* pixel = pBitmap + (y * width + x) * bytesPerPixel;
                    byte histValue = 0;

                    if (bytesPerPixel == 4)
                    {
                        histValue = (byte)((pixel[2] + pixel[1] + pixel[0]) / 3);
                    }
                    else if (bytesPerPixel == 1)
                    {
                        histValue = pixel[0];
                    }

                    pNewBitmap[y * newBitmap.Width + x] = (byte)equalHistogram[histValue];
                }
            }
            // 비트맵 잠금 해제
            newBitmap.UnlockBits(bmpData);
            return newBitmap;
        }


        /* 이진화
         *  이미지를 그레이 스케일로 변환 > 히스토그램 > 임계값 계산 > 그레이 스케일한 이미지로 이진화
         */
        public unsafe Bitmap Binarization(byte* pBitmap, Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int[] histogram = new int[256];

            int bytesPerPixel = 0;
            if (bitmap.PixelFormat == PixelFormat.Format32bppRgb)
            {
                bytesPerPixel = 4;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                bytesPerPixel = 1;
            }

            // 새로운 비트맵 생성
            Bitmap newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (bytesPerPixel == 4)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else if (bytesPerPixel == 1)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            }

            // 비트맵 데이터를 잠금
            BitmapData bmpData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.WriteOnly, newBitmap.PixelFormat);
            byte* pNewBitmap = (byte*)bmpData.Scan0.ToPointer();

            // 히스토그램 계산
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte* pixel = pBitmap + (y * width + x) * bytesPerPixel;
                    byte gray = 0;

                    if (bytesPerPixel == 4)
                    {
                        gray = (byte)((pixel[2] + pixel[1] + pixel[0]) / 3);
                    }
                    else if (bytesPerPixel == 1)
                    {
                        gray = (byte)pixel[0];
                    }

                    // 새로운 비트맵에 픽셀 설정
                    pNewBitmap[(y * width + x) * bytesPerPixel + 0] = gray;
                    if (bytesPerPixel == 4)
                    {
                        pNewBitmap[(y * width + x) * bytesPerPixel + 1] = gray; // Green
                        pNewBitmap[(y * width + x) * bytesPerPixel + 2] = gray; // Red
                        pNewBitmap[(y * width + x) * bytesPerPixel + 3] = 255; // Alpha
                    }

                    histogram[gray]++;
                }
            }

            // 전체 픽셀 수
            int totalPixels = width * height;

            float sum = 0;
            for (int i = 0; i < 256; i++)
                sum += i * histogram[i];

            float sumB = 0;
            int wB = 0, wF = 0;
            float maxVariance = 0;
            byte threshold = 0;

            for (int t = 0; t < 256; t++)
            {
                wB += histogram[t]; // 배경 픽셀 수
                if (wB == 0) continue;

                wF = totalPixels - wB; // 전경 픽셀 수
                if (wF == 0) break;

                sumB += (float)(t * histogram[t]);

                float mB = sumB / wB; // 배경의 평균
                float mF = (sum - sumB) / wF; // 전경의 평균

                // 분산 계산
                float betweenVariance = (float)wB * (float)wF * (mB - mF) * (mB - mF);

                // 최대 분산을 찾고 임계값 저장
                if (betweenVariance > maxVariance)
                {
                    maxVariance = betweenVariance;
                    threshold = (byte)t;
                }
            }

            // 이진화 수행
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte* pixel = pBitmap + (y * width + x) * bytesPerPixel;

                    // 픽셀 인덱스가 유효한지 체크
                    if (pixel < pBitmap || pixel + bytesPerPixel > pBitmap + width * height * bytesPerPixel)
                    {
                        continue;
                    }

                    byte gray = 0;
                    if (bytesPerPixel == 4)
                    {
                        gray = (byte)((pixel[2] + pixel[1] + pixel[0]) / 3);
                    }
                    else if (bytesPerPixel == 1)
                    {
                        gray = (byte)pixel[0];
                    }

                    byte newColor = gray > threshold ? (byte)255 : (byte)0;

                    // 새로운 비트맵에 픽셀 설정
                    pNewBitmap[(y * width + x) * bytesPerPixel + 0] = newColor;
                    if (bytesPerPixel == 4)
                    {
                        pNewBitmap[(y * width + x) * bytesPerPixel + 1] = newColor; // Green
                        pNewBitmap[(y * width + x) * bytesPerPixel + 2] = newColor; // Red
                        pNewBitmap[(y * width + x) * bytesPerPixel + 3] = 255; // Alpha
                    }
                }
            }

            // 비트맵 잠금 해제
            newBitmap.UnlockBits(bmpData);
            return newBitmap;
        }


        /* 가우시안 필터
         *  가우시안 커널(홀수)을 생성 > 가중 평균 계산(커널 값을 곱한 후 새로우 픽셀 값 결정)
         */
        public unsafe Bitmap Gaussion(byte* pBitmap, Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int bytesPerPixel = 0;

            if (bitmap.PixelFormat == PixelFormat.Format32bppRgb || bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                bytesPerPixel = 4;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                bytesPerPixel = 1;
            }

            // 새로운 비트맵 생성
            Bitmap newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (bytesPerPixel == 4)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else if (bytesPerPixel == 1)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                ColorPalette palette = newBitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i); // 흑백 팔레트
                }
                newBitmap.Palette = palette;
            }

            BitmapData bmpData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.WriteOnly, newBitmap.PixelFormat);
            byte* pNewBitmap = (byte*)bmpData.Scan0.ToPointer();

            int[] histogram = new int[256];
            // 히스토그램 생성
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte* pixel = pBitmap + (y * width + x) * bytesPerPixel;
                    byte histValue = 0;

                    if (bytesPerPixel == 4)
                    {
                        // 평균값 계산
                        histValue = (byte)((pixel[2] + pixel[1] + pixel[0]) / 3);
                    }
                    else if (bytesPerPixel == 1)
                    {
                        histValue = (byte)pixel[0];
                    }

                    histogram[histValue]++;
                }
            }

            double totalPixel = width * height;
            double totalSum = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                totalSum += i * histogram[i];
            }
            double average = totalSum / totalPixel;

            // 분산 계산
            double varianceSum = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                varianceSum += histogram[i] * Math.Pow(i - average, 2);
            }
            double variance = varianceSum / totalPixel;

            // 표준편차 계산
            double standardDeviation = Math.Sqrt(variance);

            // 가우시안 커널 생성
            int kernelSize = 6 * (int)standardDeviation;
            // 짝수일 경우 중앙점을 위해 홀수로 변경
            if (standardDeviation / 2 != 0)
            {
                kernelSize = kernelSize + 1;
            }

            double[,] kernel = new double[kernelSize, kernelSize];
            int radius = (kernelSize - 1) / 2;
            double sum = 0;
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double value = (2 * Math.PI * variance) * Math.Exp(-((x * x) + (y * y)) / (2 * variance));
                    kernel[x + radius, y + radius] = value;
                    sum += value;
                }
            }

            // 가우시안 커널 정규화
            for (int i = 0; i < kernelSize; i++)
            {
                for (int j = 0; j < kernelSize; j++)
                {
                    kernel[i, j] = kernel[i, j] / sum;
                }
            }

            // 가우시안 커널 적용
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    double value = 0;

                    for (int kernelY = -radius; kernelY <= radius; kernelY++)
                    {
                        for (int kernelX = -radius; kernelX <= radius; kernelX++)
                        {
                            int newX = x + kernelX;
                            int newY = y + kernelY;

                            if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                            {
                                int pixelIndex = newY * width * bytesPerPixel + newX * bytesPerPixel;
                                byte pixelValue = pBitmap[pixelIndex];

                                double kernelValue = kernel[kernelX + radius, kernelY + radius];
                                value += kernelValue * pixelValue;
                            }
                        }
                    }
                    int outputIndex = y * width * bytesPerPixel + x * bytesPerPixel;
                    pNewBitmap[outputIndex] = (byte)Math.Min(255, Math.Max(0, value));

                    if (bytesPerPixel == 4)
                    {
                        pNewBitmap[outputIndex + 1] = (byte)Math.Min(255, Math.Max(0, value));
                        pNewBitmap[outputIndex + 2] = (byte)Math.Min(255, Math.Max(0, value));
                        pNewBitmap[outputIndex + 3] = 255;
                    }
                }
            });
            
            // 비트맵 잠금 해제
            newBitmap.UnlockBits(bmpData);
            return newBitmap;
        }

        /* 라플라스 필터
         */
        public unsafe Bitmap Laplace (byte *pBitmap, Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            int bytesPerPixel = 0;

            if (bitmap.PixelFormat == PixelFormat.Format32bppRgb || bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                bytesPerPixel = 4;
            }
            else if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                bytesPerPixel = 1;
            }

            // 새로운 비트맵 생성
            Bitmap newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (bytesPerPixel == 4)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else if (bytesPerPixel == 1)
            {
                newBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                ColorPalette palette = newBitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i); // 흑백 팔레트
                }
                newBitmap.Palette = palette;
            }

            BitmapData bmpData = newBitmap.LockBits(new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.WriteOnly, newBitmap.PixelFormat);
            byte* pNewBitmap = (byte*)bmpData.Scan0.ToPointer();

            // 라플라시안 커널 (3x3)
            int[,] kernel = new int[,]
            {
                {0, -1, 0},
                {-1, 4, -1},
                {0, -1, 0}
            };
            /*{
            { -1, -1, -1 },
            { -1, 8, -1 },
            { -1, -1, -1 }
            };*/

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    double value = 0;

                    // kernel size > 3x3
                    for (int kernelX = -1; kernelX <= 1; kernelX++)
                    {
                        for (int kernelY = -1; kernelY <= 1; kernelY++)
                        {
                            int newX = x + kernelX;
                            int newY = y + kernelY;

                            // 경계 체크
                            if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                            {
                                int pixelIndex = newY * width * bytesPerPixel + newX * bytesPerPixel;
                                byte pixelValue = pBitmap[pixelIndex];

                                double kernelValue = kernel[kernelX + 1, kernelY + 1];
                                value += kernelValue * pixelValue;
                            }
                        }
                    }
                    int outputIndex = y * width * bytesPerPixel + x * bytesPerPixel;
                    pNewBitmap[outputIndex] = (byte)Math.Min(255, Math.Max(0, value));

                    if (bytesPerPixel == 4)
                    {
                        pNewBitmap[outputIndex + 1] = (byte)Math.Min(255, Math.Max(0, value));
                        pNewBitmap[outputIndex + 2] = (byte)Math.Min(255, Math.Max(0, value));
                        pNewBitmap[outputIndex + 3] = 255;
                    }
                }
            }

            // 비트맵 잠금 해제
            newBitmap.UnlockBits(bmpData);
            return newBitmap;
        }
    }
}