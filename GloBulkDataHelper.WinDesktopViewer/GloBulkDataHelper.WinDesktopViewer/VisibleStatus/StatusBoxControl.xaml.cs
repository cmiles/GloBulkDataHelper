using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GloBulkDataHelper.WinDesktopViewer.VisibleStatus
{
    public sealed partial class StatusBoxControl : UserControl
    {
        [AutoNotify] private VisibleStatusModel _model;

        public StatusBoxControl()
        {
            InitializeComponent();
        }
    }
}