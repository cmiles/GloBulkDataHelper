using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GloBulkDataHelper.WinDesktopViewer.Threading
{
    public struct DispatcherThreadSwitcher : INotifyCompletion
    {
        private readonly SynchronizationContext _context;

        internal DispatcherThreadSwitcher(SynchronizationContext uiContext)
        {
            _context = uiContext;
        }

        public bool IsCompleted => _context == SynchronizationContext.Current;

        public DispatcherThreadSwitcher GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
        }

        public void OnCompleted(Action continuation)
        {
            _context.Post(x => continuation(), null);
        }
    }
}