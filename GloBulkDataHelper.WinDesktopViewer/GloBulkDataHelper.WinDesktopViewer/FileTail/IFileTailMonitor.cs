using System;
using System.Text;
using System.Threading.Tasks;

namespace GloBulkDataHelper.WinDesktopViewer.FileTail
{
    /// <summary>
    ///     Implemented by classes that monitor a file for changes and report them.
    /// </summary>
    public interface IFileTailMonitor : IDisposable
    {
        /// <summary>
        ///     Gets or sets a value indicating whether a buffer is used read the changes in blocks.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a buffer is used; otherwise, <c>false</c>.
        /// </value>
        bool BufferedRead { get; set; }

        /// <summary>
        ///     Gets the path of the file being monitored.
        /// </summary>
        /// <value>
        ///     The file path.
        /// </value>
        string FilePath { get; }

        /// <summary>
        ///     Gets or sets the length of the read buffer.
        /// </summary>
        /// <value>
        ///     The length of the read buffer.
        /// </value>
        int ReadBufferSize { get; set; }

        /// <summary>
        ///     Updates the encoding used to read the file.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        void ChangeEncoding(Encoding encoding);

        /// <summary>
        ///     Occurs when the file being monitored is recreated.
        /// </summary>
        event Action<IFileTailMonitor> FileCreated;

        /// <summary>
        ///     Occurs when the file being monitored is deleted.
        /// </summary>
        event Action<IFileTailMonitor> FileDeleted;

        /// <summary>
        ///     Occurs when the file being monitored is renamed.
        /// </summary>
        event Action<IFileTailMonitor, string> FileRenamed;

        /// <summary>
        ///     Occurs when the file being monitored is updated.
        /// </summary>
        event Action<IFileTailMonitor, string> FileUpdated;

        /// <summary>
        ///     Refreshes the <see cref="IFileTailMonitor" /> checking for any changes.
        /// </summary>
        Task Refresh();
    }
}