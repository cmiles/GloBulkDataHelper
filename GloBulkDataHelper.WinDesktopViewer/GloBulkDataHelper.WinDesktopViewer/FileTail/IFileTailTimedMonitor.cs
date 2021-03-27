using System;

namespace GloBulkDataHelper.WinDesktopViewer.FileTail
{
    public interface IFileTailTimedMonitor : IFileTailMonitor
    {
        TimeSpan TimerInterval { get; set; }
    }
}