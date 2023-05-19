#ifndef _RAWIMAGE_H_
#define _RAWIMAGE_H_

#include <string.h>

// buffer flag options
#define BUFFER8		1
#define BUFFER32	2

// states
#define RAWIMAGE_BUFFER_CREATED  0
#define RAWIMAGE_BUFFER_GIVEN    1

#ifdef _MACOSX
#include <Carbon/Carbon.h>
#endif

class RawImage
{
	int mWidth;
	int mHeight;
  int mBitDepth;            // 8, 16 bits per color channel
  int mPixDepth;            // 24, 32 bits per pixel
	unsigned char *mData;
	unsigned short int *sData;
 	float* fData;
  int mBufState;          // buffer state

	// supported formats
	bool LoadPng( char *fileName );
	bool SavePng( char *fileName );
	bool LoadTif( char *fileName );
	bool SaveTif( char *fileName );
	bool SaveTif32( char *fileName );
	bool LoadJpeg( char *fileName );
	bool SaveJpeg( char *fileName, int quality );
	bool LoadTga( char *fileName );
	bool SaveTga( char *fileName );
  bool LoadBmp( char *fileName );
  bool SaveBmp( char *fileName );
  bool LoadPpm( char *fileName );
  bool SavePpm( char *fileName );
  bool LoadJP2( char *fileName );
  bool SaveJP2( char *fileName );
  bool LoadHDR( char *fileName );
  bool SaveHDR( char *fileName );
  bool SaveHDR32( char *fileName );
  bool LoadEXR( char *fileName );
  bool SaveEXR( char *fileName );

public:

	RawImage();
	
  // buffer is initialized
  // @ in
  // width = image width
  // height = image height
  // _mBitDepth = bits per color channel (default 8)
  // _mPixDepth = bits per pixel (default 24)
  RawImage( int width, int height, 
						int _mBitDepth=8, int _mPixDepth=24 );

  // buffer is provided
  // @ in
  // width = image width
  // height = image height
  // buffer = image buffer (rgbrgb)
  // _mBitDepth = bits per color channel (default 8)
  // _mPixDepth = bits per pixel (default 24)
	RawImage( int width, int height, void* buffer, 
            int _mBitDepth, int _mPixDepth );

	// constructor
  // @ in
  // source = image source
	RawImage( RawImage *source );
  
	~RawImage();
  void free();

	// returns the width in pixels
  int width(void) const;

	// returns the height in pixels
  int height(void) const;

  // returns the pixdepth (24, 32)
  int pixdepth(void) const;

  // sets the pixdepth
  void setPixDepth( int pixdepth ) { mPixDepth = pixdepth; }

	// returns the bitdepth
  int bitdepth(void) const;

  // sets the bitdepth
  void setBitDepth( int bitdepth ) { mBitDepth = bitdepth; }

  
	// returns the number of color planes
	int cplanes( void ) { return (mPixDepth/mBitDepth); }

	// returns the RGB(A) buffer
  unsigned char *getBuffer(void);

	// returns the 32-bit unit buffer or NULL if it is not allocated
	float *getBuffer32( void );

  // manual initialization of the 32-bit uint buffer
  float *initBuffer32( void );

	// returns the 16-bit buffer or NULL if it is not allocated
  unsigned short int* getBuffer16( void );

  // read the image from disk
  // @in
  // fileName: file to read
  // @out
  // success true/false
  bool read( char *fileName );

#ifdef _MACOSX
#if defined(_LP64_) || !defined(__BIG_ENDIAN__)
  bool readWithQuartz(char *fileName, int *newMaxHeight=NULL, int *newMaxWidth=NULL);
  bool writeWithQuartz(char *fileName, char *ext);
#else
	bool readWithQuickTime(char *fileName);
  bool readAndScaleWithQuickTime(char *fileName, int *newHeight, int *newWidth);
#endif
#endif
#if defined(_MACOSX) && !defined(REALFLOW)
  bool readDefault(char *fileName);
  bool writeDefault(char* fileName, unsigned char bufferSelect=BUFFER8 );
#endif
	
  // write the image buffer to disk
  // @in
  // filename: output file name
  // bufferSelect:  BUFFER8 writes the RGB buffer
  //                BUFFER32 writes the 32-bit float buffer
  // @out
  // success true/false
  bool write(char* fileName, unsigned char bufferSelect=BUFFER8 );
	
  // flip image vertically
  // @out
  // success true/false
  bool flipY(void);
	
  // create (32-bit float) buffer from (RGB buffer)
  void convertHDRRGB( void );
  
  // expand RGB buffer from 24 (RGB) to 32 (RGBA)
  void convertRGBA( void );
  
  // swap R and B 
  void swapRGB( void );

  // scale
  // @in
	// newWidth: new image width
	// newHeight: new image height (if zero, aspect ratio is preserved)
  void scale( int newWidth, int newHeight = 0 );

	// format validation
	static bool isSupported( char *filename, int bitdepth );
};

class RawMovieData;
class RawMovie
{
	RawMovieData *mData;

public:

	RawMovie();
	~RawMovie();

	bool Open(char *fileName, int width, int height, int fps);
	void PutImage(RawImage &ri);
	//void GetImage(RawImage &ri);
	void Close(void);
};

#endif
