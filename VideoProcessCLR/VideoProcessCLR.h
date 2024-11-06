#pragma once

#include <utility>
//using namespace System;
typedef unsigned char byte;

namespace VideoProcessCLR {
	public ref class VideoProcessCLR
	{
	public:
		
		static byte* Expansion(byte* pBitmap, int width, int height, int bytesPerPixel);
		static byte* Shrinkage(byte* pBitmap, int width, int height, int bytesPerPixel);
		static byte* Smoothing(byte* pBitmap, int width, int height, int bytesPerPixel);
		static byte* Binization(byte* pBitmap, int width, int height, int bytesPerPixel);
		static byte* Gaussion(byte* pBitmap, int width, int height, int bytesPerPixel);
		static byte* Laplace(byte* pBitmap, int width, int height, int bytesPerPixel);
		static System::Drawing::Point Matching(byte* originalBitmap, int originalWidth, int originalHeight, int originalBytePerPixel, byte* templateBitmap, int templateWidth, int templateHeight, int templateBytePerPixel);
	};
}
