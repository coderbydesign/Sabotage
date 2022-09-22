using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Parsers.Nodes;
using Avalonia.Media;
using ReactiveUI;

namespace Sabotage.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _startButtonText = "Let's Play SABOTAGE!";

        public string StartButtonText
        {
            get => _startButtonText;
            set 
            {
                _startButtonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartButtonText)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    
        // Start the game
        public void StartSabotage()
        {
            StartButtonText = "connecting...";
            Client client = new Client();
            client.ConnectToServer();
            GameLogic.InitializeBoard();
        }
    }
}
