using System;
using EnvDTE;
using EnvDTE80;

namespace WakaTime.VisualStudio
{
    class WakaTimeVsPlugin : WakaTimeIdePlugin<DTE2>
    {
        private DocumentEvents _docEvents;
        private WindowEvents _windowEvents;
        private SolutionEvents _solutionEvents;

        private ILogService _logService;
        private EditorInfo _editorInfo;
        private bool _disposed;

        public WakaTimeVsPlugin(DTE2 editor) : base(editor)
        { }

        public override ILogService GetLogger()
        {
            if (_logService == null)
            {
                _logService = new LogService();
            }

            return _logService;
        }

        public override EditorInfo GetEditorInfo()
        {
            if (_editorInfo == null)
            {
                _editorInfo = new EditorInfo
                {
                    Name = "visualstudio",
                    Version = Version.Parse(editorObj.Version),
                    PluginName = "visualstudio-wakatime",
                    PluginVersion = typeof(EditorInfo).Assembly.GetName().Version
                };
            }

            return _editorInfo;
        }

        public override string GetActiveSolutionPath()
        {
            return (editorObj.Solution == null)
                ? null
                : editorObj.Solution.FullName;
        }

        public override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _docEvents.DocumentOpened -= DocEventsOnDocumentOpened;
                _docEvents.DocumentSaved -= DocEventsOnDocumentSaved;
                _windowEvents.WindowActivated -= WindowEventsOnWindowActivated;
                _solutionEvents.Opened -= SolutionEventsOnOpened;
            }

            _disposed = true;
        }

        public override void BindEditorEvents()
        {
            _docEvents = editorObj.Events.DocumentEvents;
            _windowEvents = editorObj.Events.WindowEvents;
            _solutionEvents = editorObj.Events.SolutionEvents;

            // setup event handlers
            _docEvents.DocumentOpened += DocEventsOnDocumentOpened;
            _docEvents.DocumentSaved += DocEventsOnDocumentSaved;
            _windowEvents.WindowActivated += WindowEventsOnWindowActivated;
            _solutionEvents.Opened += SolutionEventsOnOpened;
        }

        #region Event Handlers

        private void DocEventsOnDocumentOpened(Document document)
        {
            OnDocumentOpened(document.FullName);
        }

        private void DocEventsOnDocumentSaved(Document document)
        {
            OnDocumentChanged(document.FullName);
        }

        private void WindowEventsOnWindowActivated(Window gotFocus, Window lostFocus)
        {
            var document = editorObj.ActiveWindow.Document;
            if (document != null)
                OnDocumentOpened(document.FullName);
        }

        private void SolutionEventsOnOpened()
        {
            var solutionName = GetActiveSolutionPath();
            OnSolutionOpened(solutionName);
        }

        #endregion
    }
}
