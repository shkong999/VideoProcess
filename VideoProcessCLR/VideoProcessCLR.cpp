#include "pch.h"
#include "VideoProcessCLR.h"
#include <cmath>
#include <vector>
#include<iostream>
#include<algorithm>


// 히스토그램 평활화
byte* VideoProcessCLR::VideoProcessCLR::Smoothing(byte* pBitmap, int width, int height, int bytePerPixel)
{
	int totalPixel = width * height;
	double cumulative = 0.0;
	array<double>^ histogram = gcnew array<double>(256);
	array<double>^ cdfHistogram = gcnew array<double>(256);
	array<int>^ equalHistogram = gcnew array<int>(256);

	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width;x++)
		{
			byte* pixel = pBitmap + (y * width + x) * bytePerPixel;
			byte histValue = (bytePerPixel == 4) ? (byte)((pixel[2] + pixel[1] + pixel[0]) / 3) : pixel[0];
			histogram[histValue]++;
		}
	}

	// 히스토그램 확률, 누적 히스토그램 계산
	for (int i = 0; i < histogram->Length; i++)
	{
		histogram[i] /= totalPixel;
		cumulative += histogram[i];
		cdfHistogram[i] = cumulative;
		equalHistogram[i] = static_cast<int>(std::round(cdfHistogram[i] * 255));
	}

	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			byte* pixel = pBitmap + (y * width + x) * bytePerPixel;
			byte histValue = (bytePerPixel == 4) ? (byte)((pixel[2] + pixel[1] + pixel[0]) / 3) : pixel[0];
			pBitmap[y * width + x] = (byte)equalHistogram[histValue];
		}
	}
	return pBitmap;
}

// 이진화
byte* VideoProcessCLR::VideoProcessCLR::Binization(byte* pBitmap, int width, int height, int bytePerPixel)
{
	int totalPixel = width * height;
	float sum = 0;
	array<int>^ histogram = gcnew array<int>(256);
	
	// 히스토그램 계산
	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width;x++)
		{
			byte* pixel = pBitmap + (y * width + x) * bytePerPixel;
			byte histValue = (bytePerPixel == 4) ? (byte)((pixel[2] + pixel[1] + pixel[0]) / 3) : pixel[0];
			histogram[histValue]++;
		}
	}

	for (int i = 0; i < 256; i++)
	{
		sum += i * histogram[i];
	}

	// 오츠 임계값 계산
	float sumB = 0;
	int wB = 0;
	int wF = 0;
	float maxVariance = 0;
	byte threshold = 0;

	for (int t = 0; t < 256; t++)
	{
		wB += histogram[t]; // 배경 픽셀 수
		if (wB == 0) continue;

		wF = totalPixel - wB; // 전경 픽셀 수
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

	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			byte* pixel = pBitmap + (y * width + x) * bytePerPixel;
			byte gray = (bytePerPixel == 4) ? (byte)((pixel[2] + pixel[1] + pixel[0]) / 3) : pixel[0];
			byte newColor = gray > threshold ? (byte)255 : (byte)0;
			if (bytePerPixel == 1)
			{
				pBitmap[(y * width + x) * bytePerPixel + 0] = newColor;
			}
			else if (bytePerPixel == 4)
			{
				pBitmap[(y * width + x) * bytePerPixel + 1] = newColor; // Green
				pBitmap[(y * width + x) * bytePerPixel + 2] = newColor; // Red
				pBitmap[(y * width + x) * bytePerPixel + 3] = 255; // Alpha
			}
		}
	}

	return pBitmap;
}

byte* VideoProcessCLR::VideoProcessCLR::Gaussion(byte* pBitmap, int width, int height, int bytesPerPixel)
{
	double sigma = 5;
	double sum = 0.0f;
	// 커널 사이즈
	int size = 13;
	std::vector<std::vector<float>> kernel(size, std::vector<float>(size));
	int radius = (size - 1) / 2;

	// 가우시안 필터(커널) 생성
	for (int y = -radius; y <= radius; y++)
	{
		for (int x = -radius; x <= radius; x++)
		{
			/*double value = exp(-((x * x) + (y * y)) / (2 * sigma * sigma));
			value = kernel[y + radius][x + radius];
			sum += value;*/
			kernel[y + radius][x + radius] = exp(-(x * x + y * y) / (2 * sigma * sigma));
			sum += kernel[y + radius][x + radius];
		}
	}

	// 가우시안 커널 정규화
	for (int i = 0; i < size; i++)
	{
		for (int j = 0; j < size; j++)
		{
			kernel[i][j] = kernel[i][j] / sum;
		}
	}

	// 적용
	for (int y = 0; y < height; y++)
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

						double kernelValue = kernel[kernelX + radius][kernelY + radius];
						value += kernelValue * pixelValue;
					}
				}
			}
			value = std::min(255.0, std::max(0.0, value));

			int outputIndex = y * width * bytesPerPixel + x * bytesPerPixel;
			pBitmap[outputIndex] = static_cast<byte>(value);

			if (bytesPerPixel == 4)
			{
				pBitmap[outputIndex + 1] = static_cast<byte>(value);
				pBitmap[outputIndex + 2] = static_cast<byte>(value);
				pBitmap[outputIndex + 3] = 255;
			}
		}
	}

	return pBitmap;
}
