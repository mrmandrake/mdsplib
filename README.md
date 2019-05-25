# mdsplib

DFT & FFT / iFFT & STFT / iSTFT lib

examples:

generic DFT on a Saw signal:
Complex saw = Generate.Saw(440, 44100, 345, 1000).DFT();

Create 1024 sample of a simple sine wave at 110Hz, 11000Hz sampling frequency:
double[] sine = Generate.Sine(110, 11000,1024);

FFT on Hann windowed square wave:
Complex[] spectrum = Generate.Square(110,11000,1024,1000).Window(Window.Type.Hann).FFT();

Reconstruction of signal:
double[] reconst = Generate.Square(110,11000,1024,1000).Window(Window.Type.Hann).FFT().iFFT().RealPart();

STFT:
Generate.Square(110,11000,1023,1000).Window().STFT().iSTFT()
