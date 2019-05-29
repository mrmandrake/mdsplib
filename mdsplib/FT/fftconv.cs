using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace mdsplib.FT
{

    class FFTConvolver
    {
        protected Complex[] fftIr;

        public void Init(int len, double[] ir)
        {
            int lenMin = 0;
            int lenMax = 0;
            int blockSize = lenMin + (new System.Random().Next(100000) % (1 + (lenMax - lenMin)));

            fftIr = ir.FFT();

            /*
            reset();
            if (blockSize == 0)
                return false;

            // Ignore zeros at the end of the impulse response because they only waste computation time
            while (irLen > 0 && ::fabs(ir[irLen - 1]) < 0.000001f)
                --irLen;

            if (irLen == 0)
                return true;

            _blockSize = NextPowerOf2(blockSize);
            _segSize = 2 * _blockSize;
            _segCount = static_cast<size_t>(::ceil(static_cast<float>(irLen) / static_cast<float>(_blockSize)));
            _fftComplexSize = audiofft::AudioFFT::ComplexSize(_segSize);

            // FFT
            _fft.init(_segSize);
            _fftBuffer.resize(_segSize);

            // Prepare segments
            for (size_t i = 0; i < _segCount; ++i)
                _segments.push_back(new SplitComplex(_fftComplexSize));

            // Prepare IR
            for (size_t i = 0; i < _segCount; ++i)
            {
                SplitComplex* segment = new SplitComplex(_fftComplexSize);
                const size_t remaining = irLen - (i * _blockSize);
                const size_t sizeCopy = (remaining >= _blockSize) ? _blockSize : remaining;
                CopyAndPad(_fftBuffer, &ir[i * _blockSize], sizeCopy);
                _fft.fft(_fftBuffer.data(), segment->re(), segment->im());
                _segmentsIR.push_back(segment);
            }

            // Prepare convolution buffers  
            _preMultiplied.resize(_fftComplexSize);
            _conv.resize(_fftComplexSize);
            _overlap.resize(_blockSize);

            // Prepare input buffer
            _inputBuffer.resize(_blockSize);
            _inputBufferFill = 0;

            // Reset current position
            _current = 0;
            */
        }

        public double[] Process(double[] input)
        {
            /*
            int processed = 0;
            while (processed < input.Length)
            {
                const bool inputBufferWasEmpty = (_inputBufferFill == 0);
                const size_t processing = std::min(len - processed, _blockSize - _inputBufferFill);
                const size_t inputBufferPos = _inputBufferFill;

                ::memcpy(_inputBuffer.data() + inputBufferPos, input + processed, processing * sizeof(Sample));

                // Forward FFT
                CopyAndPad(_fftBuffer, &_inputBuffer[0], _blockSize);
                _fft.fft(_fftBuffer.data(), _segments[_current]->re(), _segments[_current]->im());

                // Complex multiplication
                if (inputBufferWasEmpty)
                {
                    _preMultiplied.setZero();
                    for (size_t i = 1; i < _segCount; ++i)
                    {
                        const size_t indexIr = i;
                        const size_t indexAudio = (_current + i) % _segCount;
                        ComplexMultiplyAccumulate(_preMultiplied, *_segmentsIR[indexIr], *_segments[indexAudio]);
                    }
                }
                _conv.copyFrom(_preMultiplied);
                ComplexMultiplyAccumulate(_conv, *_segments[_current], *_segmentsIR[0]);

                // Backward FFT
                _fft.ifft(_fftBuffer.data(), _conv.re(), _conv.im());

                // Add overlap
                Sum(output + processed, _fftBuffer.data() + inputBufferPos, _overlap.data() + inputBufferPos, processing);

                // Input buffer full => Next block
                _inputBufferFill += processing;
                if (_inputBufferFill == _blockSize)
                {
                    // Input buffer is empty again now
                    _inputBuffer.setZero();
                    _inputBufferFill = 0;

                // Save the overlap
                ::memcpy(_overlap.data(), _fftBuffer.data() + _blockSize, _blockSize * sizeof(Sample));

                    // Update current segment
                    _current = (_current > 0) ? (_current - 1) : (_segCount - 1);
                }

                processed += processing;
            }
            */

            return null;
        }
    }
}
