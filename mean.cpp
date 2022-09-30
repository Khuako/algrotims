#include <iostream>
#include <ctime>
using namespace std;
int counter=0;
void quickSort(int* a, int first, int last) {
	int i = first, j = last, x = a[(first + last) / 2];
	counter += 5;
	do {
		while (a[i] < x) {
			i++;
			counter++;
		}
		while (a[j] > x) {
			j--;
			counter++;
		}
		if (i <= j)
		{
			if (i < j)
			{
				int tmp = a[i];
				a[i] = a[j];
				a[j] = tmp;
				counter += 3;
			}
			i++;
			j--;
			counter += 2;
		}
	} while (i <= j);
	if (i < last)
		quickSort(a, i, last);
	if (first < j)
		quickSort(a, first, j);
}

void print(int* a, int n) {
	for (int i = 0; i < n; i++)
		cout << a[i] << " ";
	cout << endl;
}

int main() {
	cout << "MEAN:" << endl;
	srand(time(0));
	for (int n = 100; n <= 1000; n += 100) {
		for (int k = 1; k <= 10000; k++) {
			int* a = new int[n];
			for (int i = 0; i < n; i++)
				a[i] = rand() % 100;
			quickSort(a, 0, n - 1);
		}
		cout << "N = " << n << ": " << counter / 10000 << endl;
		counter = 0;
	}
}
