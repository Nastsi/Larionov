
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>
#include <math.h>
#include <time.h>
#include <stdlib.h>

void calculateCpuMax(float* x, int N, float* counter) {
	float max = x[0];
	for (int i = 0; i < N; i++) {
		if (x[i] > max && i % 2 == 0) {
			max = x[i];
		}
	}
	*counter = max;
}

__global__ void calculateGpuMax(int n, float* x, float* counter)
{
	int threadCount = 1000;
	int index = threadIdx.x;
	int startIndex = index * threadCount;
	int endIndex = (index + 1) * threadCount;

	float max = x[startIndex];
	for (int i = startIndex + 1; i < endIndex && i < n; i++) {
		if (x[i] > max && i % 2 == 0) {
			max = x[i];
		}
	}
	counter[index] = max;
}

int main(void)
{
	int N = 100000000;

	float *array_h;
	float* cpuMax = (float*)malloc(sizeof(float));

	clock_t start, end;
	float timeElapsed;
	float timeElapsedGpu;

	printf("Number of elements = %i\n", N);

	srand(time(NULL));

	array_h = (float*)malloc(N * sizeof(float));

	for (int i = 0; i < N; i++) {
		array_h[i] = rand() % 256;
	}

	start = clock();

	*cpuMax = 0;

	calculateCpuMax(array_h, N, cpuMax);

	end = clock();

	timeElapsed = (double)(end - start) / CLOCKS_PER_SEC * 1000;

	printf("CPU max even = %.0f\n", *cpuMax);
	printf("CPU time = %.3fms\n", timeElapsed);

	float *counter_h;
	float *array_d;
	float *counter_d;

	int threadCount = 1000;

	counter_h = (float*)malloc(threadCount * sizeof(float));

	cudaMalloc(&array_d, N * sizeof(float));
	cudaMalloc(&counter_d, threadCount * sizeof(float));

	*counter_h = 0;

	cudaMemcpy(array_d, array_h, N * sizeof(float), cudaMemcpyHostToDevice);
	cudaMemcpy(counter_d, counter_h, threadCount * sizeof(float), cudaMemcpyHostToDevice);

	start = clock();

	calculateGpuMax << <1, threadCount >> >(N, array_d, counter_d);

	cudaDeviceSynchronize();

	cudaMemcpy(counter_h, counter_d, threadCount * sizeof(float), cudaMemcpyDeviceToHost);

	float gpuMax = counter_h[0];

	for (int i = 1; i < threadCount; i++) {
		if (counter_h[i] > gpuMax) {
			gpuMax = counter_h[i];
		}
	}

	free(counter_h);

	cudaFree(array_d);
	cudaFree(counter_d);

	end = clock();

	timeElapsedGpu = (double)(end - start) / CLOCKS_PER_SEC * 1000;

	printf("GPU max even = %0.f\n", gpuMax);
	printf("GPU time = %.3fms\n", timeElapsedGpu);

	printf("Speed Coefficient = %.3f\n", timeElapsed / timeElapsedGpu);

	free(array_h);
	free(cpuMax);

	system("pause");

	return 0;
}

