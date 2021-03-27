using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GloBulkDataHelper.WinDesktopViewer.FileTail
{
    public abstract class FileTailMonitor : IFileTailMonitor
    {
        private const int DefaultBufferSize = 1048576;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private Encoding _encoding;

        private bool _fileExists;
        private FileInfo _fileInfo;
        private bool _isDisposed;
        private Stream _stream;
        private StreamReader _streamReader;

        protected FileTailMonitor(string filePath, Encoding encoding)
        {
            encoding ??= Encoding.UTF8;

            FilePath = filePath;
            _encoding = encoding;

            _fileExists = File.Exists(FilePath);

            if (_fileExists)
                OpenFile(filePath);
        }

        public string WaitLineBuffer { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether a buffer is used read the changes in blocks.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a buffer is used; otherwise, <c>false</c>.
        /// </value>
        public bool BufferedRead { get; set; } = true;

        /// <summary>
        ///     Updates the encoding used to read the file.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        public void ChangeEncoding(Encoding encoding)
        {
            _semaphoreSlim.Wait();

            try
            {
                _encoding = encoding;
                OpenFile(FilePath);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <summary>
        ///     Occurs when the file being monitored is recreated.
        /// </summary>
        public event Action<IFileTailMonitor> FileCreated;

        /// <summary>
        ///     Occurs when the file being monitored is deleted.
        /// </summary>
        public event Action<IFileTailMonitor> FileDeleted;

        /// <summary>
        ///     Gets the path of the file being monitored.
        /// </summary>
        /// <value>
        ///     The file path.
        /// </value>
        public string FilePath { get; protected set; }

        /// <summary>
        ///     Occurs when the file being monitored is renamed.
        /// </summary>
        public event Action<IFileTailMonitor, string> FileRenamed;

        /// <summary>
        ///     Occurs when the file being monitored is updated.
        /// </summary>
        public event Action<IFileTailMonitor, string> FileUpdated;

        /// <summary>
        ///     Gets or sets the length of the read buffer.
        /// </summary>
        /// <value>
        ///     The length of the read buffer.
        /// </value>
        public int ReadBufferSize { get; set; } = DefaultBufferSize;

        public virtual async Task Refresh()
        {
            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                _fileInfo.Refresh();

                var fileDidExist = _fileExists;
                _fileExists = _fileInfo.Exists;

                if (fileDidExist && !_fileExists)
                {
                    // File has been deleted
                    OnFileDeleted();
                }
                else if (!fileDidExist && _fileExists)
                {
                    // File has been created
                    OpenFile(FilePath);
                    OnFileCreated();
                }

                if (_streamReader == null)
                    return;

                var baseStream = _streamReader.BaseStream;

                if (baseStream.Position > _fileInfo.Length)
                    // File is smaller than the current position, 
                    // seek to the end
                    baseStream.Seek(0, SeekOrigin.End);

                if (_streamReader.EndOfStream)
                    return;

                if (BufferedRead)
                {
                    var buffer = new char[ReadBufferSize];
                    int charCount;

                    while ((charCount =
                        await _streamReader.ReadAsync(buffer, 0, ReadBufferSize).ConfigureAwait(false)) > 0)
                    {
                        var appendedContent = new string(buffer, 0, charCount);

                        if (!string.IsNullOrEmpty(appendedContent))
                            OnFileUpdated(appendedContent);
                    }
                }
                else
                {
                    var appendedContent = await _streamReader.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(appendedContent))
                    {
                        var currentWaitBuffer = WaitLineBuffer + appendedContent;

                        var lines = currentWaitBuffer.Split(
                            new[] {"\r\n", "\r", "\n"},
                            StringSplitOptions.RemoveEmptyEntries
                        );

                        if (!lines.Any())
                        {
                            WaitLineBuffer = currentWaitBuffer;
                            return;
                        }

                        foreach (var loopLines in lines[..^1]) OnFileUpdated(loopLines);

                        if (currentWaitBuffer.EndsWith("\n") || currentWaitBuffer.EndsWith("\n"))
                        {
                            OnFileUpdated(lines[^1]);
                            WaitLineBuffer = string.Empty;
                        }
                        else
                        {
                            WaitLineBuffer = lines[^1];
                        }
                    }
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources;
        ///     <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                DisposeStream();

            _isDisposed = true;
        }

        private void DisposeStream()
        {
            _streamReader?.Dispose();
            _stream?.Dispose();
        }

        ~FileTailMonitor()
        {
            Dispose(false);
        }

        protected virtual void OnFileCreated()
        {
            FileCreated?.Invoke(this);
        }

        protected virtual void OnFileDeleted()
        {
            FileDeleted?.Invoke(this);
        }

        protected virtual void OnFileRenamed(string newName)
        {
            FileRenamed?.Invoke(this, newName);
        }

        protected virtual void OnFileUpdated(string updatedContent)
        {
            FileUpdated?.Invoke(this, updatedContent);
        }

        private void OpenFile(string filePath)
        {
            _fileInfo = new FileInfo(filePath);

            // Dispose existing stream
            DisposeStream();

            // File is opened for read only, and shared for read, write and delete
            _stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            _streamReader = new StreamReader(_stream, _encoding);

            // Start at the end of the file
            _streamReader.BaseStream.Seek(0, SeekOrigin.End);
        }
    }
}