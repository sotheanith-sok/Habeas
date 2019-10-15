using System;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Collections.Concurrent;
namespace Search.Document
{
    public sealed class FileManager
    {
        private static readonly Lazy<FileManager> lazy = new Lazy<FileManager>(() => new FileManager(), true);

        public static FileManager Instance { get { return lazy.Value; } }

        private ConcurrentDictionary<string, File> files;

        private FileManager()
        {
            files = new ConcurrentDictionary<string, File>();
            System.Timers.Timer timer = new System.Timers.Timer(5000);
            timer.Elapsed += CleanUp;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
        public MemoryMappedViewStream GetFile(string path)
        {
            path = Path.GetFullPath(path);

            if (!files.ContainsKey(path))
            {
                files.TryAdd(path, new File(path));
            }

            File temp;
            files.TryGetValue(path, out temp);
            temp.timeStamp=DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            files.TryUpdate(path,temp, temp);

            return temp.GetMemoryMappedFile().CreateViewStream();
        }

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
                files.TryRemove(path,out file);
                file.Dispose();
            }
        }

        private class File : IDisposable
        {
            public long timeStamp { get; set; }

            private MemoryMappedFile memoryMappedFile;
            public File(string path)
            {
                memoryMappedFile = MemoryMappedFile.CreateFromFile(Path.GetFullPath(path));
                timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            public void Dispose()
            {
                memoryMappedFile.Dispose();
            }

            public MemoryMappedFile GetMemoryMappedFile()
            {
                return this.memoryMappedFile;
            }
        }
    }
}