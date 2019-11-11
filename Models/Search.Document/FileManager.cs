using System;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Collections.Concurrent;
namespace Search.Document
{
    /// <summary>
    /// Manager uses to allows sharing of files access to multiple threat
    /// </summary>
    public sealed class FileManager
    {
        /// <summary>
        /// Lazy implementation of file manager
        /// </summary>
        /// <typeparam name="FileManager"></typeparam>
        /// <returns>A single instance of FileManager</returns>
        private static readonly Lazy<FileManager> lazy = new Lazy<FileManager>(() => new FileManager(), true);

        /// <summary>
        /// Access FileManager singleton instance
        /// </summary>
        /// <value></value>
        public static FileManager Instance { get { return lazy.Value; } }

        /// <summary>
        /// Thread-safe dictionary maps paths to files
        /// </summary>
        private ConcurrentDictionary<string, File> files;

        /// <summary>
        /// Constructor
        /// </summary>
        private FileManager()
        {
            files = new ConcurrentDictionary<string, File>();
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += CleanUp;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        /// <summary>
        /// Create a view which enable file reading
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public MemoryMappedViewStream GetFile(string path)
        {
            path = Path.GetFullPath(path);

            if (!files.ContainsKey(path))
            {
                files.TryAdd(path, new File(path));
            }

            File temp;
            files.TryGetValue(path, out temp);
            temp.timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            files.TryUpdate(path, temp, temp);

            return temp.GetMemoryMappedFile().CreateViewStream();
        }

        /// <summary>
        /// Release files that haven't been access after a certain time
        /// </summary>
        /// <param name="source">Object that call this method</param>
        /// <param name="e">Event that was fired</param>
        private void CleanUp(Object source, ElapsedEventArgs e)
        {
            List<string> filesToBeRemove = new List<string>();
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            foreach (string path in this.files.Keys)
            {
                File file;
                files.TryGetValue(path, out file);
                if (currentTime - file.timeStamp > 60)
                {
                    filesToBeRemove.Add(path);
                }
            }

            foreach (string path in filesToBeRemove)
            {
                File file;
                files.TryRemove(path, out file);
                file.Dispose();
            }
        }

        /// <summary>
        /// Internal representation of a file
        /// </summary>
        private class File : IDisposable
        {
            /// <summary>
            /// Keep track of when file was last accessed  
            /// </summary>
            /// <value></value>
            public long timeStamp { get; set; }

            /// <summary>
            /// Memory space that stored file 
            /// </summary>
            private MemoryMappedFile memoryMappedFile;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="path">path to file on disk</param>
            public File(string path)
            {
                memoryMappedFile = MemoryMappedFile.CreateFromFile(Path.GetFullPath(path));
                timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            /// <summary>
            /// Release resources
            /// </summary>
            public void Dispose()
            {
                memoryMappedFile.Dispose();
            }

            /// <summary>
            /// Return reference to memory space
            /// </summary>
            /// <returns></returns>
            public MemoryMappedFile GetMemoryMappedFile()
            {
                return this.memoryMappedFile;
            }
        }
    }
}