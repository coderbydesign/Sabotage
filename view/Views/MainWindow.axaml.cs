using System;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Sabotage.ViewModels;

namespace Sabotage.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void PlayTurn(object sender, RoutedEventArgs e)
        {
            Button sourceButton = (Button)sender;
            sourceButton.Background = new SolidColorBrush(new Color(255, 255, 234, 108));
            // foreach(PropertyDescriptor descriptor in TypeDescriptor.GetProperties(sender))
            // {
            //     string name = descriptor.Name;
            //     object value = descriptor.GetValue(sender);
            //     Console.WriteLine("{0}={1}", name, value);
            // }
        }
    }
}