using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalFilter.Models;
using SignalFilter.Services;

namespace SignalFilter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISignalSource _source;
    private readonly SignalAggregator _aggregator = new();
    private CancellationTokenSource? _cancellToken;

    public ObservableCollection<SignalRecord> Records { get; } = new();
    public ICollectionView RecordsView { get; }

    [ObservableProperty] 
    private bool _isRunning;
    [ObservableProperty] 
    private string _frequencyFilter = string.Empty;
    [ObservableProperty] 
    private string _bandwidthFilter = string.Empty;
    [ObservableProperty] 
    private string _snrFilter = string.Empty;

    public MainViewModel(ISignalSource source)
    {
        _source = source;
        BindingOperations.EnableCollectionSynchronization(Records, new object());
        RecordsView = CollectionViewSource.GetDefaultView(Records);
        RecordsView.Filter = ApplyFilter;
    }

    partial void OnFrequencyFilterChanged(string value)
    {

    }
    
    partial void OnBandwidthFilterChanged(string value)
    {

    }
    partial void OnSnrFilterChanged(string value)
    {

    }

    private bool ApplyFilter(object obj)
    {
        if (obj is not SignalRecord r)
            return false;

        if (!string.IsNullOrWhiteSpace(FrequencyFilter) && !r.FrequencyMhz.ToString("F3").Contains(FrequencyFilter, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(BandwidthFilter) && !r.BandwidthKhz.ToString("F3").Contains(BandwidthFilter, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!string.IsNullOrWhiteSpace(SnrFilter) && !r.Snr.ToString("F3").Contains(SnrFilter, StringComparison.OrdinalIgnoreCase))
            return false;

        return true;
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        _cancellToken = new CancellationTokenSource();


        await foreach (var signal in _source.ReadSignalsAsync(_cancellToken.Token))
        {
            _aggregator.Process(signal);

            var record = _aggregator.Records[^1];
            if (!Records.Contains(record))
                Application.Current.Dispatcher.Invoke(() => Records.Add(record));
        }
    }

    [RelayCommand]
    private void Stop()
    {
        if (_cancellToken != null)
            _cancellToken.Cancel();
    }
}
