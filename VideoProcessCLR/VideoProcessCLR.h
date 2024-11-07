#pragma once

#include <utility>
//using namespace System;
typedef unsigned char byte;

namespace VideoProcessCLR {
	public ref class VideoProcessCLR
	{
	public:
		static byte* Expansion(byte* pBitmap, int width, int height, int bytesPerPixel); // 팽창
		static byte* Shrinkage(byte* pBitmap, int width, int height, int bytesPerPixel); // 수축
		static byte* Smoothing(byte* pBitmap, int width, int height, int bytesPerPixel); // 히스토그램 평활화
		static byte* Binization(byte* pBitmap, int width, int height, int bytesPerPixel); // 이진화
		static byte* Gaussion(byte* pBitmap, int width, int height, int bytesPerPixel); // 가우스필터
		static byte* Laplace(byte* pBitmap, int width, int height, int bytesPerPixel); // 라플라스 필터
		static System::Drawing::Point Matching(byte* originalBitmap, int originalWidth, int originalHeight, int originalBytePerPixel, 
			byte* templateBitmap, int templateWidth, int templateHeight, int templateBytePerPixel); // 템플릿 매칭
	};
}
