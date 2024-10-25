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
         *  이진 이미지로 변환 > 구조적 요소 정의 > 팽창 적용
         */
        public unsafe Bitmap Expansion(byte* pBitmap, Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
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
                }
            }
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

            /*bool check = CheckBinary(pNewBitmap, newBitmap.Width, newBitmap.Height, bytesPerPixel);*/

            // 비트맵 잠금 해제
            newBitmap.UnlockBits(bmpData);
            return newBitmap;
        }
    }
}