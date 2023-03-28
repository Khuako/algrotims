// ConsoleApplication4.cpp : Этот файл содержит функцию "main". Здесь начинается и заканчивается выполнение программы.
//Cравнить по времени сортировку ряда (сортировка массива однопроцессорная и многопроцессорная)


#include <iostream>
#include <omp.h>
#include<ctime>
#include<chrono>
using namespace std;
using namespace std::chrono;
void BubbleSortParallel(int Mass[], int n) {
	int i, j;
#pragma omp parallel for default(none) shared(Mass,n) private(i)
	for (i = 0; i < n; i++) {
		if (i % 2 == 0) {
#pragma omp parallel for 
			for (j = 0; j < n - 1; j+=2) {
				if (Mass[j] > Mass[j + 1]) swap(Mass[j], Mass[j + 1]);
			}
		}
		else {
#pragma omp parallel for
			for (j = 1; j < n - 1; j += 2) {
				if (Mass[j] > Mass[j + 1]) swap(Mass[j], Mass[j + 1]);
			}
		}
	}
}
void BubbleSortDefault(int Mass[], int n) {
	int i, j;
	for (i = 0; i < n; i++) {
		if (i % 2 == 0) {
			for (j = 0; j < n - 1; j++) {
				if (Mass[j] > Mass[j + 1]) swap(Mass[j], Mass[j + 1]);
			}
		}
		else {
			for (j = 1; j < n - 1; j += 2) {
				if (Mass[j] > Mass[j + 1]) swap(Mass[j], Mass[j + 1]);
			}
		}
	}
}
int main()
{
	srand(time(0));
	setlocale(LC_ALL, "russian");
	const int n = 10000;
	int Mass[n];
	int Mass1[n];

	for (int i = 0; i < n; i++) {
		Mass[i] = rand()%10000;
		Mass1[i] = Mass[i];
	}
	//С использование многопоточности
	auto t0 = high_resolution_clock::now();
	BubbleSortParallel(Mass, n);
	auto t1 = high_resolution_clock::now();
	cout << " Время выполнения с многопоточностью: " << duration_cast<milliseconds>(t1 - t0).count() << "miliseconds" << endl << endl;
	//Без использования многопоточности
	 t0 = high_resolution_clock::now();
	BubbleSortDefault(Mass1, n);
	 t1 = high_resolution_clock::now();
	cout <<" Время выполнения Без многопоточности : " << duration_cast<milliseconds>(t1 - t0).count() << "miliseconds" << endl << endl;
}

