using System.Windows;

namespace TypeEmojis
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private KeyboardEventProcessor _keyboardEventProcessor;
        private TrayIcon _trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var fileReader = new EmojiListFileReader("emojis.cfg");
            
            _keyboardEventProcessor = new KeyboardEventProcessor(fileReader.KeyValueDictionary);
            _keyboardEventProcessor.Start();

            _trayIcon = new TrayIcon();
            _trayIcon.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _keyboardEventProcessor.Stop();
            _trayIcon.Hide();
        }
    }
}
