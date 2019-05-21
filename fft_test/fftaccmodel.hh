#pragma once

#include <complex>
#include <math.h>
#include "./xilinx/xfft_v9_1_bitacc_cmodel.h"

using namespace std;

#define FFT_LENGTH 1024
#define _DIR 0

typedef double FFT_INPUT_T;
typedef double FFT_OUTPUT_T;

class fft {
public:
	static void execute(complex<FFT_INPUT_T>  xn[FFT_LENGTH], complex<FFT_OUTPUT_T> xk[FFT_LENGTH])
	{	
		// Log2 of FFT length
		int fft_length = FFT_LENGTH;
		int NFFT = 10;
		const int samples = 1 << NFFT;

		// Build up the C model generics structure
		xilinx_ip_xfft_v9_1_generics generics;
		generics.C_NFFT_MAX = 10;
		generics.C_ARCH = 3;
		generics.C_HAS_NFFT = 0;
		generics.C_INPUT_WIDTH = 32;
		generics.C_TWIDDLE_WIDTH = 25;
		generics.C_HAS_SCALING = 1;
		generics.C_HAS_BFP = 1;
		generics.C_HAS_ROUNDING = 1;
		generics.C_USE_FLT_PT = 1;

		// Create an FFT state object
		xilinx_ip_xfft_v9_1_state* state = xilinx_ip_xfft_v9_1_create_state(generics);

		int stages = 0;
		if ((generics.C_ARCH == 2) || (generics.C_ARCH == 4)) // radix-2
			stages = NFFT;
		else // radix-4 or radix-22
			stages = (NFFT + 1) / 2;

		double* xn_re = (double*)malloc(samples * sizeof(double));
		double* xn_im = (double*)malloc(samples * sizeof(double));
		int* scaling_sch = (int*)malloc(stages * sizeof(int));
		double* xk_re = (double*)malloc(samples * sizeof(double));
		double* xk_im = (double*)malloc(samples * sizeof(double));

		// Check the memory was allocated successfully for all arrays
		if (xn_re == NULL || xn_im == NULL ||
			scaling_sch == NULL ||
			xk_re == NULL || xk_im == NULL)
		{
			std::cerr << "Couldn't allocate memory for input and output data arrays - dying" << std::endl;
			exit(3);
		}

		// Set pointers in input and output structures
		xilinx_ip_xfft_v9_1_inputs inputs;
		SetupInput(inputs, xn_re, xn_im, scaling_sch, NFFT, stages, samples, xn);
		// Set sizes of output structure arrays

		xilinx_ip_xfft_v9_1_outputs outputs;
		outputs.xk_re = xk_re;
		outputs.xk_im = xk_im;
		outputs.xk_re_size = samples;
		outputs.xk_im_size = samples;

		Debug(generics, inputs, stages, outputs);

		int result = xilinx_ip_xfft_v9_1_bitacc_simulate(state, inputs, &outputs);
		if (result != 0)
		{
			std::cerr << "An error occurred when simulating the FFT core: return code " << result << std::endl;
			exit(4);
		}

		FillOutput(samples, NFFT, outputs, xk);

		// Status
		//if (CONFIG_T::scaling_opt == ip_fft::block_floating_point)
		//	blkexp.range(c * 8 + 7, c * 8) = outputs.blk_exp;
		//else if (CONFIG_T::ovflo && (CONFIG_T::scaling_opt == ip_fft::scaled))
		//	overflow.range(c, c) = outputs.overflow;

		// Release memory used for input and output arrays
		free(xn_re);
		free(xn_im);
		free(scaling_sch);
		free(xk_re);
		free(xk_im);

		// Destroy FFT state to free up memory
		xilinx_ip_xfft_v9_1_destroy_state(state);
	}
private:
	static void SetupInput(xilinx_ip_xfft_v9_1_inputs& inputs, double* xn_re, double* xn_im, int* scaling_sch, int NFFT, int stages, const int& samples, std::complex<double>* xn)
	{
		inputs.xn_re = xn_re;
		inputs.xn_im = xn_im;
		inputs.scaling_sch = scaling_sch;
		// Store in inputs structure
		inputs.nfft = NFFT;
		// config data
		inputs.direction = _DIR;
		unsigned scaling = 0;

		for (int i = 0; i < stages; i++)
		{
			inputs.scaling_sch[i] = scaling & 0x3;
			scaling >>= 2;
		}

		inputs.scaling_sch_size = stages;
		for (int i = 0; i < samples; i++)
		{
			complex<FFT_INPUT_T> din = xn[i];
			inputs.xn_re[i] = (double)din.real();
			inputs.xn_im[i] = (double)din.imag();
			std::cout << "xn[" << i << ": xn_re = " << inputs.xn_re[i] << " xk_im = " << inputs.xn_im[i] << endl;
		}

		inputs.xn_re_size = samples;
		inputs.xn_im_size = samples;
	}

	static void FillOutput(const int& samples, int NFFT, xilinx_ip_xfft_v9_1_outputs& outputs, std::complex<double>* xk)
	{
		// Output data
		for (int i = 0; i < samples; i++)
		{
			complex<FFT_OUTPUT_T> dout;
			unsigned addr_reverse = 0;
			for (int k = 0; k < NFFT; ++k)
			{
				addr_reverse <<= 1;
				addr_reverse |= (i >> k) & 0x1;
			}
			unsigned addr = i;
			dout = complex<FFT_OUTPUT_T>(outputs.xk_re[addr], outputs.xk_im[addr]);
			xk[i] = dout;
			cout << "xk[" << i << ": xk_re = " << outputs.xk_re[addr] << " xk_im = " << outputs.xk_im[addr] << endl;
		}
	}

	static void Debug(xilinx_ip_xfft_v9_1_generics & generics, xilinx_ip_xfft_v9_1_inputs & inputs, int stages, xilinx_ip_xfft_v9_1_outputs & outputs)
	{
		///////////////////////////////////////////////////////////////////////////////
		/// Debug
		std::cout << "About to call the C model with:" << std::endl;
		std::cout << "Generics:" << std::endl;
		std::cout << "  C_NFFT_MAX = " << generics.C_NFFT_MAX << std::endl;
		std::cout << "  C_ARCH = " << generics.C_ARCH << std::endl;
		std::cout << "  C_HAS_NFFT = " << generics.C_HAS_NFFT << std::endl;
		std::cout << "  C_INPUT_WIDTH = " << generics.C_INPUT_WIDTH << std::endl;
		std::cout << "  C_TWIDDLE_WIDTH = " << generics.C_TWIDDLE_WIDTH << std::endl;
		std::cout << "  C_HAS_SCALING = " << generics.C_HAS_SCALING << std::endl;
		std::cout << "  C_HAS_BFP = " << generics.C_HAS_BFP << std::endl;
		std::cout << "  C_HAS_ROUNDING = " << generics.C_HAS_ROUNDING << std::endl;
		std::cout << "  C_USE_FLT_PT = " << generics.C_USE_FLT_PT << std::endl;

		std::cout << "Inputs structure:" << std::endl;
		std::cout << "  nfft = " << inputs.nfft << std::endl;
		printf("  xn_re[0] = %e\n", inputs.xn_re[0]);
		std::cout << "  xn_re_size = " << inputs.xn_re_size << std::endl;
		printf("  xn_im[0] = %e\n", inputs.xn_im[0]);
		std::cout << "  xn_im_size = " << inputs.xn_im_size << std::endl;

		for (int i = stages - 1; i >= 0; --i)
			std::cout << "  scaling_sch[" << i << "] = " << inputs.scaling_sch[i] << std::endl;

		std::cout << "  scaling_sch_size = " << inputs.scaling_sch_size << std::endl;
		std::cout << "  direction = " << inputs.direction << std::endl;

		std::cout << "Outputs structure:" << std::endl;
		std::cout << "  xk_re_size = " << outputs.xk_re_size << std::endl;
		std::cout << "  xk_im_size = " << outputs.xk_im_size << std::endl;

		// Run the C model to generate output data
		std::cout << "Running the C model..." << std::endl;
		///////////////////////////////////////////////////////////////////////////////
	}
};