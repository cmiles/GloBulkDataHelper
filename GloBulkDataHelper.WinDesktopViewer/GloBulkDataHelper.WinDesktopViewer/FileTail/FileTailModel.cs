using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GloBulkDataHelper.WinDesktopViewer.Threading;
using GloBulkDataHelper.WinDesktopViewer.VisibleStatus;

namespace GloBulkDataHelper.WinDesktopViewer.FileTail
{
    public class FileTailModel
    {
        [AutoNotify] private bool _fileExists;
        [AutoNotify] private string _fileFullName;
        [AutoNotify] private ObservableCollection<StatusItem> _items;
        [AutoNotify] private VisibleStatusModel _statusContext;

        public FileTailModel()
        {
            StatusContext = new VisibleStatusModel();
        }

        private FileTailMonitor Monitor { get; set; }

        private void FileMonitorOnFileCreated(IFileTailMonitor obj)
        {
            FileFullName = obj.FilePath;
        }

        private void FileMonitorOnFileDeleted(IFileTailMonitor obj)
        {
            FileFullName = string.Empty;
            FileExists = false;
        }

        private void FileMonitorOnFileRenamed(IFileTailMonitor arg1, string arg2)
        {
            FileFullName = arg2;
        }

        private void FileMonitorOnFileUpdated(IFileTailMonitor arg1, string arg2)
        {
            StatusContext.FireAndForgetInBackground(async () =>
            {
                await ThreadSwitcher.ResumeForegroundAsync();
                Items.Add(new StatusItem {Status = arg2});
            });
        }

        public async Task Load(string fileFullName, string fileName)
        {
            Monitor?.Dispose();
            Monitor = null;

            FileFullName = fileFullName;

            FileExists = File.Exists(fileFullName);

            Monitor = new FileTailTimedMonitor(fileFullName, Encoding.UTF8, TimeSpan.FromSeconds(5));
            Monitor.FileUpdated += FileMonitorOnFileUpdated;
            Monitor.FileDeleted += FileMonitorOnFileDeleted;
            Monitor.FileCreated += FileMonitorOnFileCreated;
            Monitor.FileRenamed += FileMonitorOnFileRenamed;
        }
    }

    public class StatusItem
    {
        public string Status { get; set; }
    }
}