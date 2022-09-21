using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Markup.Parsers.Nodes;
using Avalonia.Media;
using ReactiveUI;

namespace Sabotage.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _startButtonText = "START GAME!";
        private bool _connected = false;

        public string StartButtonText
        {
            get => _startButtonText;
            set 
            {
                _startButtonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartButtonText)));
            }
        }
        
        public bool Connected
        {
            get => _connected;
            set 
            {
                _connected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Connected)));
            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
    
        // Start the game
        public void StartSabotage()
        {
            StartButtonText = "CONNECTING...";
            Thread.Sleep(2000);
            Connected = true;
        }
    }
}