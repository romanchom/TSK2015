using System.Threading;

public class Barrier {
	public Barrier(uint count) {
		threadCount = count;
		mutex = new Semaphore(1, 1);
		sem = new Semaphore(0, (int) count);
		sem2 = new Semaphore(0, (int)count);
	}

	readonly uint threadCount;
	volatile uint current = 0;
	Semaphore mutex;
	Semaphore sem;
	Semaphore sem2;


	public void Wait() {
		mutex.WaitOne();
		current++;
		if (current == threadCount) sem.Release((int)threadCount);
		mutex.Release();
		sem.WaitOne();

		mutex.WaitOne();
		current--;
		if (current == 0) sem2.Release((int)threadCount);
		mutex.Release();
		sem2.WaitOne();
	}
}
