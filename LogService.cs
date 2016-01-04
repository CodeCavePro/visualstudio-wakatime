using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WakaTime.VisualStudio
{
    class LogService : ILogService
    {
        private static IVsOutputWindowPane _wakatimeOutputWindowPane;

        private static IVsOutputWindowPane WakatimeOutputWindowPane
        {
            get { return _wakatimeOutputWindowPane ?? (_wakatimeOutputWindowPane = GetWakatimeOutputWindowPane()); }
        }

        private static IVsOutputWindowPane GetWakatimeOutputWindowPane()
        {
            var outputWindow = Package.GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
            if (outputWindow == null) return null;

            var outputPaneGuid = new Guid(GuidList.GuidWakatimeOutputPane.ToByteArray());
            IVsOutputWindowPane windowPane;

            outputWindow.CreatePane(ref outputPaneGuid, "Wakatime", 1, 1);
            outputWindow.GetPane(ref outputPaneGuid, out windowPane);

            return windowPane;
        }

        public void Log(string message)
        {
            var outputWindowPane = WakatimeOutputWindowPane;
            if (outputWindowPane == null)
                return;

            outputWindowPane.OutputString(message);
        }
    }
}
