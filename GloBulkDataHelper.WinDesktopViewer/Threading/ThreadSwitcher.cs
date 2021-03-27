using System.Threading;

namespace GloBulkDataHelper.WinDesktopViewer.Threading
{
    public class ThreadSwitcher
    {
        public static SynchronizationContext PinnedUiContext { get; set; }

        public static SynchronizationContext PinnedOrCurrentUiDispatcher()
        {
            return PinnedUiContext ?? SynchronizationContext.Current;
        }

        // For both WPF and Windows Forms
        public static ThreadPoolThreadSwitcher ResumeBackgroundAsync()
        {
            return new();
        }

        public static DispatcherThreadSwitcher ResumeForegroundAsync(SynchronizationContext dispatcher)
        {
            return new(dispatcher);
        }

        public static DispatcherThreadSwitcher ResumeForegroundAsync()
        {
            return new(PinnedOrCurrentUiDispatcher());
        }
    }
}