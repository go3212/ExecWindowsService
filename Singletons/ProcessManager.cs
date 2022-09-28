using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ExecWindowsService.Singletons
{
    public class Process : System.Diagnostics.Process
    {
        public Process(string path)
        {
            List<string> splittedPath = path.Split('\\').ToList();
            this.StartInfo.FileName = splittedPath.Last();
            if (splittedPath.Count > 1)
            {
                splittedPath.RemoveAt(splittedPath.Count - 1);
                this.StartInfo.UseShellExecute = true;
                this.StartInfo.WorkingDirectory = String.Join('\\', splittedPath);
            }
        }
    }

    public interface IProcessManager : IDisposable
    {
        int StartProcess(string path);
    }

    public class ProcessManager : IProcessManager
    {
        private LinkedList<Process> processes = new LinkedList<Process>();
        private bool disposed = false;

        public ProcessManager()
        {

        }

        // Permite iniciar un proceso.
        // Pre: un path valido.
        // Post: se devuelve el identificador del proceso.
        public int StartProcess(string path)
        {
            try
            {
                var process = new Process(path);
                process.Start();
                processes.AddLast(process);
                return process.Id;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    foreach (Process process in processes)
                    {
                        if (!process.HasExited)
                            process.Kill();
                    }

                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;
            }
        }


        ~ProcessManager()
        {
            // Debemos matar todos los procesos.
            Dispose(disposing: false);
        }
    }
}
