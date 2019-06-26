using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NLog;

namespace Test_QueueUserWorkItem
{
    class Program
    {
        static void Main()
        {
            try
            {
                int numberOfThreads = 20;
                var doneEvents = new ManualResetEvent[numberOfThreads];
                for (int i = 0; i < numberOfThreads; i++)
                {
                    doneEvents[i] = new ManualResetEvent(false);
                    ThreadPool.QueueUserWorkItem(JobForAThread);
                }
                    
                WaitHandle.WaitAll(doneEvents);
                //Console.WriteLine("All calculations are complete.");
                Console.WriteLine("Логирование {0} потоков завершено", numberOfThreads);
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }

        }

        static void JobForAThread(object state)
        {
            try
            {
                
                GenerateLog();

            }
            catch (Exception ex)
            {
                Console.WriteLine("В потоке " + Thread.CurrentThread.ManagedThreadId.ToString() +
                    " произошла ошибка:" + ex.Message);
            }

        }

        static void GenerateLog()
        {
            try
            {

                Logger log = LogManager.GetCurrentClassLogger();
                log.Trace("trace message in thread {0}", Thread.CurrentThread.ManagedThreadId);
                log.Debug("debug message in thread {0}", Thread.CurrentThread.ManagedThreadId);
                log.Info("info message in thread {0}", Thread.CurrentThread.ManagedThreadId);
                log.Warn("warn message in thread {0}", Thread.CurrentThread.ManagedThreadId);
                log.Error("error message in thread {0}", Thread.CurrentThread.ManagedThreadId);
                log.Fatal("fatal message in thread {0}", Thread.CurrentThread.ManagedThreadId);
            }

            catch (Exception ex)
            {
                throw ex;

            }
        }
    }
}
