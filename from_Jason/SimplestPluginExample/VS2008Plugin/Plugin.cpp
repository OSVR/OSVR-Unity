
#if _MSC_VER // this is defined when compiling with Visual Studio
#define EXPORT_API __declspec(dllexport) // Visual Studio needs annotating exported functions with this
#else
#define EXPORT_API // XCode does not need annotating exported functions, so define is empty
#endif

// ------------------------------------------------------------------------
// Plugin itself


// Link following functions C-style (required for plugins)
extern "C"
{

// The functions we will call from Unity.
//
const EXPORT_API char*  PrintHello(){
	return "Hello";
}

int EXPORT_API PrintANumber(){
	return 5;
}

int EXPORT_API AddTwoIntegers(int a, int b) {
	return a + b;
}

float EXPORT_API AddTwoFloats(float a, float b) {
	return a + b;
}

} // end of export C block
