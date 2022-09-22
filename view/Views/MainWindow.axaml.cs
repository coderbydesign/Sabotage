using System;
using System.ComponentModel;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using Sabotage.ViewModels;
using Sabotage;

namespace Sabotage.Views
{
    public partial class MainWindow : Window
    {
        public static List<Button> buttons = new List<Button>();
        private static Button startBtn;
        private static TextBlock oppMsg;
        private static Grid board;
        private static Panel splashHdr;
        private static Panel gamePlayHdr;
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            startBtn = this.GetControl<Button>("startButton");
            oppMsg = this.GetControl<TextBlock>("opponentMessage");
            board = this.GetControl<Grid>("grid");
            splashHdr = this.GetControl<Panel>("splashHeader");
            gamePlayHdr = this.GetControl<Panel>("gamePlayHeader");

            foreach(Button button in this.GetControl<Grid>("grid").GetVisualChildren()) {
                buttons.Add(button);
            }
        }

        private void PlayTurn(object sender, RoutedEventArgs e)
        {
            Button sourceButton = (Button)sender;
            sourceButton.Background = new SolidColorBrush(new Color(255, 255, 234, 108));
            int xCoord = ((int)sourceButton.Bounds.X) / ((int)sourceButton.Bounds.Width);
            int yCoord = ((int)sourceButton.Bounds.Y) / ((int)sourceButton.Bounds.Height);

            ClientSend.Fire(xCoord, yCoord);
        }

        public static void Connected() {
            startBtn.IsVisible = false;
            oppMsg.IsVisible = true;
        }

        public static void GameReady() {
            // we may be able to refactor this all to use
            // styles which set visiblity, and add/remove visible styles
            oppMsg.IsVisible = false;
            board.IsVisible = true;
            splashHdr.IsVisible = false;
            gamePlayHdr.IsVisible = true;
        }

        public static void ReceiveFire() {
            int x = GameLogic.xReceived;
            int y = GameLogic.yReceived;
            Console.WriteLine($"({x}, {y}) has been hit by the enemy!");
        }

        public static void UpdateTarget() {
            int x = GameLogic.xTarget;
            int y = GameLogic.yTarget;
            bool hitShip = GameLogic.isOccupiedTarget;

            Console.WriteLine($"({x}, {y}) was targeted. Ship Present? {hitShip}");

            if (hitShip) {
                buttons[(x*10) + (y*1)].Background = new SolidColorBrush(new Color(255, 255, 0, 0));
            } else {
                buttons[(x*10) + (y*1)].Background = new SolidColorBrush(new Color(255, 0, 0, 255));
            }
        }
    }
}
