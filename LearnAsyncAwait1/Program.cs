using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnAsyncAwait1
{
    /// <summary>
    /// The Program
    /// </summary>
    class Program
    {
        /// <summary>
        /// The Main Program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Create a worker class intance
            var Worker = new WorkerClass();

            ConsoleKeyInfo keyInfo = new ConsoleKeyInfo();
            WriteMenu();

            while ((keyInfo.KeyChar != 'q') && (keyInfo.KeyChar != 'Q'))
            {
                keyInfo = Console.ReadKey();
                Console.WriteLine();
                switch (keyInfo.KeyChar)
                {
                    case 's':
                    case 'S':
                        // Syncronous
                        Worker.LongRunningOperation(3000);
                        break;

                    case 'a':
                    case 'A':
                        // Async
                        Worker.LongRunningOperationAsync(3000);
                        break;

                    case 'l':
                    case 'L':
                        // Async with Semaphore Lock (sequentializes calls)
                        Worker.LongRunningOperationWithLockAsync(3000);
                        break;

                    case 'r':
                    case 'R':
                        // Long Running Cancellable Operation
                        Worker.AbortCancellationTokenSource = new System.Threading.CancellationTokenSource(10000);
                        Worker.LongRunningCancellableOperation(3000, Worker.AbortCancellationTokenSource.Token);
                        break;

                    case 'c':
                    case 'C':
                        // Long Running Cancellable Operation
                        Worker.AbortCancellationTokenSource.Cancel();
                        break;


                    case 'e':
                    case 'E':
                        // Clear Console
                        Console.Clear();
                        WriteMenu();
                        break;

                    default:
                        break;
                }
            }

        }

        private static void WriteMenu()
        {
            Console.WriteLine("Press a command key:");
            Console.WriteLine("====================");
            Console.WriteLine("Q - Quit");
            Console.WriteLine("C - Erase (Clear) Console");
            Console.WriteLine("S - Syncronous Demo");
            Console.WriteLine("A - Asyncronous Demo");
            Console.WriteLine("L - Asyncronous Demo with Semaphore Lock");
            Console.WriteLine("R - Asyncronous Demo with Long Running Cancellable Operation");
            Console.WriteLine("C - Cancel Long Running Cancellable Operation");
            Console.WriteLine("============================================================");

        }

    }

    
}
