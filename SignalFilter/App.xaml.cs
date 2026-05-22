using System.Windows;
using SignalFilter.Services;
using SignalFilter.ViewModels;

namespace SignalFilter;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var vm = new MainViewModel(new MockSignalService());
        var window = new MainWindow { DataContext = vm };
        window.Show();
    }
}
