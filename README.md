# mdsplib

DFT / iDFT & FFT / iFFT lib
Based on excellent DSPLIB of Steve Hageman

example:

double amplitude = 1.0; double frequency = 32768;
UInt32 length = 1024; 
double samplingRate = 131072;
double[] inputSignal = DSP.Generate.ToneSampling(amplitude, frequency, samplingRate, length);

// Apply window to the Input Data & calculate Scale Factor
double[] wCoefs = DSP.Window.Coefficients(DSP.Window.Type.Hamming, length);
double[] wInputData = DSP.Math.Multiply(inputSignal, wCoefs);
double wScaleFactor = DSP.Window.ScaleFactor.Signal(wCoefs);

// Instantiate & Initialize a new DFT
FFT fft = new FFT();
fft.Initialize(length);

// Call the FFT and get the scaled spectrum back
Complex[] cSpectrum = fft.Direct(wInputData);

// Convert the complex spectrum to note: Magnitude Squared Format
// See text for the reasons to use Mag^2 format.
double[] lmSpectrum = DSP.ConvertComplex.ToMagnitude(cSpectrum);

// Properly scale the spectrum for the added window
lmSpectrum = DSP.Math.Multiply(lmSpectrum, wScaleFactor);

// For plotting on an XY Scatter plot generate the X Axis frequency Span
double[] freqSpan = fft.FrequencySpan(samplingRate);