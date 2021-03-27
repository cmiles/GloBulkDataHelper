using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GloBulkDataHelper.WinDesktopViewer.Threading;
using Serilog;

namespace GloBulkDataHelper.WinDesktopViewer.VisibleStatus
{
    public class VisibleStatusModel
    {
        [AutoNotify] private bool _blockUi;

        [AutoNotify] private string _currentStatus;

        [AutoNotify] private string _currentStatusExplanation;

        [AutoNotify] private bool _currentStatusIsBusy;

        [AutoNotify] private ObservableCollection<string> _progress;


        [AutoNotify] private VisibleStatusState _statusState;

        public async void FireAndForgetInBackground(Func<Task> taskToRun)
        {
            await ThreadSwitcher.ResumeBackgroundAsync();

            try
            {
                CurrentStatusIsBusy = true;
                await taskToRun();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error Running FireAndForget");
                CurrentStatus = "Error";
            }

            CurrentStatusIsBusy = false;
        }
    }
}