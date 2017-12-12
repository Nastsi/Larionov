
#include "cuda_runtime.h"
#include "device_launch_parameters.h"
#include <stdio.h>
#include <iostream>
#include <time.h>
#include <random>
#include <Windows.h>
#include <stdlib.h>
using namespace std;

__global__ void Addition(double* answer, double* mass1, double* mass2, int* n)
{
	int row = blockIdx.x * 10000 + threadIdx.x;
	int column = blockIdx.y * 10000 + threadIdx.y;
	int border = n[0] * n[0];
	answer[row * n[0] + column] = 0;
	for (int p = 0; p < n[0]; p++)
	{
		answer[row * n[0] + column] = (mass1[row * n[0] + p] + mass2[p * n[0] + column]);
	}
}

double* Addition_CPU(double* mass1, double* mass2, int* n)
{
	double* answer = new double[n[0] * n[0]];
	for (int p = 0; p < n[0]; p++)
	{
		for (int q = 0; q < n[0]; q++)
		{
			answer[p * n[0] + q] = 0;
			for (int r = 0; r < n[0]; r++)
			{
				answer[p * n[0] + q] = (mass1[(p * n[0] + r)] + mass2[r * n[0] + q]);
			}
		}
	}
	return answer;
}

void main()
{
	int e = 1;
	int start;
	int* n = new int[1];
	n[0] = 1000;
	srand((unsigned)time(NULL));
	//printf("Generating first matr\n");
	start = GetTickCount();
	double* mass1 = new double[n[0] * n[0]];
	for (int p = 0; p < n[0] * n[0]; p++)
	{
		mass1[p] = (double)rand() / (double)rand();
	}
	//printf("Matr 1 ready\n");
	//printf("Generating time: %i\n", GetTickCount() - start);
	//printf("\nGenerating second massive\n");
	double* mass2 = new double[n[0] * n[0]];
	start = GetTickCount();
	for (int p = 0; p < n[0] * n[0]; p++)
	{
		mass2[p] = (double)rand() / (double)rand();
	}
	//printf("Matr 2 ready\n");
	//printf("Generating time: %i\n\n", GetTickCount() - start);

	printf("CPU working\n");
	start = GetTickCount();
	double* answer_CPU = Addition_CPU(mass1, mass2, n);
	int CPU_time = GetTickCount() - start;
	printf("CPU compute time: %i\n\n", CPU_time);


	printf("GPU working\n");
	start = GetTickCount();
	double* cuda_answer;
	cudaMalloc(&cuda_answer, sizeof(double) * n[0] * n[0]);
	double* cuda_mass1;
	cudaMalloc(&cuda_mass1, sizeof(double) * n[0] * n[0]);
	cudaMemcpy(cuda_mass1, mass1, sizeof(double) * n[0] * n[0], cudaMemcpyHostToDevice);
	double* cuda_mass2;
	cudaMalloc(&cuda_mass2, sizeof(double) * n[0] * n[0]);
	cudaMemcpy(cuda_mass2, mass2, sizeof(double) * n[0] * n[0], cudaMemcpyHostToDevice);
	int* cuda_n;
	cudaMalloc(&cuda_n, sizeof(int));
	cudaMemcpy(cuda_n, n, sizeof(int), cudaMemcpyHostToDevice);
	double* answer = new double[n[0] * n[0]];

	Addition << <1,1000 >> >(cuda_answer, cuda_mass1, cuda_mass2, cuda_n);
	cudaDeviceSynchronize();
	cudaMemcpy(answer, cuda_answer, sizeof(double) * n[0] * n[0], cudaMemcpyDeviceToHost);
	int GPU_time = GetTickCount() - start;
	printf("GPU compute time: %i\n", GPU_time);


	bool correct = true;
	for (int p = 0; p < n[0] * n[0]; p++)
	{
		if (abs(answer[p] - answer_CPU[p] > e))
		{
			correct = false;
			break;
		}
	}
	if (correct)
	{
		printf("\nAnswers are equal\n");
	}
	else
	{
		printf("\nAnswers aren't equal\n");
	}
	printf("\nCoefficient: %f\n", ((double)CPU_time / (double)GPU_time));
	scanf("%d");
}
