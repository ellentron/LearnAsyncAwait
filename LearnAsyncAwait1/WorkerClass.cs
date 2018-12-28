using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LearnAsyncAwait1
{
    /// <summary>
    /// Woker Class 
    /// </summary>
    internal class WorkerClass
    {
        #region Private Members

        #endregion

        #region Constructor
        internal WorkerClass()
        {
            AbortCancellationTokenSource = new CancellationTokenSource();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Abort Cancellation Token Source
        /// </summary>
        internal CancellationTokenSource AbortCancellationTokenSource { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// LongRunningOperation - Syncronous
        /// </summary>
        /// <param name="duration_ms"></param>
        internal void LongRunningOperation(int duration_ms)
        {
            Console.WriteLine($"SomeLongWork starting will work for {duration_ms} ms");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while(watch.ElapsedMilliseconds < duration_ms)
            {; }
            watch.Stop();
            Console.WriteLine($"SomeLongWork Finished.");
        }

        /// <summary>
        /// Simulates a long running piece of work Async
        /// </summary>
        /// <param name="duration_ms">Work duration in milli seconds</param>
        internal async Task LongRunningOperationAsync(int duration_ms)
        {
            Console.WriteLine($"SomeLongWorkAsync starting will work for {duration_ms} ms");
            await Task.Delay(duration_ms);
            Console.WriteLine($"SomeLongWorkAsync Finished.");
        }

        //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Simulates a long running piece of work using a Semaphore Lock on a critical section. Execution of consequent calls is forsed to be sequential.
        /// </summary>
        /// <param name="duration_ms">Work duration in milli seconds</param>
        internal async Task LongRunningOperationWithLockAsync(int duration_ms)
        {
            // I am using here Christopher Demicoli's article https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
            // on Async Waiting inside C# Locks:

            //Asynchronously wait to enter the Semaphore. If no-one has been granted access to the Semaphore, code execution will proceed, otherwise this thread waits here until the semaphore is released 
            await semaphoreSlim.WaitAsync();

            try
            {
                Console.WriteLine($"SomeLongWorkWithLockAsync starting will work for {duration_ms} ms");
                await Task.Delay(duration_ms);
                Console.WriteLine($"SomeLongWorkWithLockAsync Finished.");
            }
            finally
            {
                // When the task is ready, release the semaphore. 
                // It is vital to ALWAYS release the semaphore when we are ready, 
                // or else we will end up with a Semaphore that is forever locked.
                // This is why it is important to do the Release within a try...finally clause; 
                // program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }
        }

        internal async Task<bool> LongRunningCancellableOperation(int duration_ms, CancellationToken cancellationToken)
        {
            bool retVal = false;
            //Task<bool> task = null;

            await Task.Run(async () =>
            {
                Console.WriteLine($"LongRunningCancellableOperation starting will work for {duration_ms} ms");
                var watch = System.Diagnostics.Stopwatch.StartNew();
                while (watch.ElapsedMilliseconds < duration_ms)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        //throw new TaskCanceledException(task);
                        break;
                    }
                    await Task.Delay(300);

                    //Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine($"watch.ElapsedMilliseconds = {watch.ElapsedMilliseconds} ms");
                    Console.CursorTop--;
                }
                watch.Stop();
                Console.WriteLine($"LongRunningCancellableOperation stopped after {watch.ElapsedMilliseconds} ms");

                if (cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine($"LongRunningCancellableOperation cancellationToken.IsCancellationRequested !!!");
                }
                else
                {
                    Console.WriteLine($"LongRunningCancellableOperation Finished.");
                    retVal = true; // Finished Successfuly
                }
            });

            return retVal;
        }
        #endregion
    }
}
