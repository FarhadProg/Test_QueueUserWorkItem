using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Concurrent;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Test_QueueUserWorkItem
{
    [Target("MyFirst")]
    public sealed class MyTarget : TargetWithContext
    {
        private ManualResetEvent _doneEvent;
        private delegate void SelectWriteMethod(LogEventInfo logEvent);
        private SelectWriteMethod selector;
       
        public ConcurrentDictionary<string, string> FilePathPatternCollection { get; set; }

        [RequiredParameter]
        public string FilePathPattern { get; set; }
        [RequiredParameter]
        public string LogDirectory { get; set; }

    


        public MyTarget()
        {
            if (FilePathPattern == null)
                FilePathPattern = @"C:\LogsTMP\Log_#.log";
            if(LogDirectory == null)
                LogDirectory = @"C:\LogsTMP\";

            FilePathPatternCollection = new ConcurrentDictionary<string, string>();
          
            IncludeEventProperties = true;          
            try
            {
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
               selector = this.WriteToLog;

            }
            catch (Exception ex)
            {
                selector = this.DontWriteToLog;
                Console.WriteLine("В конструктре класса произошла ошибка " + ex.Message);
            }



        }


        public static void ThreadPoolCallback(Object threadContext)
        {
            int threadIndex = (int)threadContext;
            
            Console.WriteLine($"Thread {threadIndex} result calculated...");
           
        }

        protected override void Write(LogEventInfo logEvent)
        {

            selector(logEvent);
        }

        private void WriteToLog(LogEventInfo logEvent)
        {
            try
            {

                IDictionary<string, object> logProperties = GetAllProperties(logEvent);
                string threadId = logProperties["@threadId"].ToString();
                Console.WriteLine("Таргет в потоке {0} начал работу...", threadId);
                string fullPathToFile = GetFileName(threadId);


                string logMessage = RenderLogEvent(Layout, logEvent);
                
                using (StreamWriter sw = new StreamWriter(fullPathToFile, true))
                {
                    sw.WriteLine(logMessage);

                }
                Console.WriteLine("Таргет в потоке {0} закончил работу...",  threadId);
            }
            catch (Exception ex)
            {

                Console.WriteLine("При записи сообщения в лог произошла ошибка: {0}", ex.Message);
            }
            finally
            {
              
                _doneEvent.Set();
            }
        }
        private void DontWriteToLog(LogEventInfo logEvent)
        {

        }
        private string  GetFileName(string threadID)
        {
            return(FilePathPatternCollection.GetOrAdd(threadID, FilePathPattern.Replace("#", threadID)));
             
        }


    }
}
