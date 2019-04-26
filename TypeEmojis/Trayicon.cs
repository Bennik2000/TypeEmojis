using System;
using System.Drawing;
using System.Windows.Forms;

namespace TypeEmojis
{
    public class TrayIcon
    {
        private NotifyIcon NotifyIcon { get; set; }

        public void Show()
        {   
            NotifyIcon = new NotifyIcon
            {
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Exit", OnMenuExitClick),
                }),
                Text = "TypeEmojis is running"
            };


            var iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico"))?.Stream;
            if (iconStream != null)
            {
                NotifyIcon.Icon = new Icon(iconStream);
            }

            NotifyIcon.Visible = true;
        }

        public void Hide()
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
        }

        private void OnMenuExitClick(object sender, EventArgs eventArgs)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
