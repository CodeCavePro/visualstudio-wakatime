using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace WakaTime
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidWakaTimePkgString)]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]
    public sealed class WakaTimePackage : Package, IDisposable
    {
        #region Fields

        private static DTE2 _objDte;
        private static WakaTimeVsPlugin _idePlugin;
        private bool _disposed;

        #endregion

        #region Startup/Cleanup

        protected override void Initialize()
        {
            Task.Run(() =>
            {
                InitializeAsync();
            });
        }

        private void InitializeAsync()
        {
            _objDte = (DTE2)GetService(typeof(DTE));
            _idePlugin = new WakaTimeVsPlugin(_objDte);

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs == null)
                return;

            // Create the command for the menu item.
            var menuCommandId = new CommandID(GuidList.GuidWakaTimeCmdSet, (int)PkgCmdIdList.UpdateWakaTimeSettings);
            var menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
            mcs.AddCommand(menuItem);
        }

        ~WakaTimePackage()
        {
            Dispose(false);
        }

        #endregion

        #region Properties

        public bool IsDisposed
        {
            get { return _disposed; }
        }

        #endregion

        #region Methods

        private static void MenuItemCallback(object sender, EventArgs e)
        {
            try
            {
                _idePlugin.SettingsPopup();
            }
            catch (Exception ex)
            {
                Logger.Error("MenuItemCallback", ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_idePlugin != null)
                    _idePlugin.Dispose();
            }

            _disposed = true;
        }

        public void Dispose() // Implement IDisposable
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
