#pragma once

//using namespace System;
typedef unsigned char byte;

namespace VideoProcessCLR {
	public ref class VideoProcessCLR
	{
	public:
		static byte* Smoothing(byte* pBitmap, int width, int height, int bytePerPixel);
		static byte* Binization(byte* pBitmap, int width, int height, int bytePerPixel);
	};
}
