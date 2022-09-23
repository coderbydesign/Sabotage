using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
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
        public static List<Button> minimapButtons = new List<Button>();
        private static Button startBtn;
        private static TextBlock oppMsg;
        private static Grid board;
        private static Panel splashHdr;
        private static Panel gamePlayHdr;
        private static TextBlock turnTracker;
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            startBtn = this.GetControl<Button>("startButton");
            oppMsg = this.GetControl<TextBlock>("opponentMessage");
            board = this.GetControl<Grid>("grid");
            splashHdr = this.GetControl<Panel>("splashHeader");
            gamePlayHdr = this.GetControl<Panel>("gamePlayHeader");
            turnTracker = this.GetControl<TextBlock>("turnTrackerBlock");

            foreach(Button button in this.GetControl<Grid>("grid").GetVisualChildren()) {
                buttons.Add(button);
            }

            foreach(Button button in this.GetControl<Grid>("minimap").GetVisualChildren()) {
                minimapButtons.Add(button);
            }
        }

        public static void RenderMinimap() {
            for(int x = 0; x < GameLogic.boardSize; x++) {
                for(int y = 0; y < GameLogic.boardSize; y++) {
                    if (GameLogic.board[x, y].isOccupied) {
                        minimapButtons[(x*10) + (y*1)].Background = new SolidColorBrush(new Color(255, 190, 0, 0));
                    } else {
                        minimapButtons[(x*10) + (y*1)].Background = new SolidColorBrush(new Color(255, 39, 87, 154));
                    }
                }
            }
        }

        private void PlayTurn(object sender, RoutedEventArgs e)
        {
            if (GameLogic.myTurn) {
                Button sourceButton = (Button)sender;
                sourceButton.Background = new SolidColorBrush(new Color(255, 255, 234, 108));
                int xCoord = ((int)sourceButton.Bounds.X) / ((int)sourceButton.Bounds.Width);
                int yCoord = ((int)sourceButton.Bounds.Y) / ((int)sourceButton.Bounds.Height);

                GameLogic.Fire(xCoord, yCoord);
            } else {
                Console.WriteLine("It's not your turn!");
            }
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
            turnTracker.IsVisible = true;
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
            Button btn = buttons[(x * 10) + (y * 1)];
            PathIcon hitIcon = btn.GetLogicalChildren().OfType<PathIcon>().FirstOrDefault(i => i.Classes.Contains("hitIcon"));

            Console.WriteLine($"({x}, {y}) was targeted. Ship Present? {hitShip}");
            if (hitShip) {
                hitIcon.IsVisible = true;
                btn.Background = new SolidColorBrush(new Color(255, 190, 0, 0));
            } else {
                btn.Background = new SolidColorBrush(new Color(255, 39, 87, 154));
            }
        }

        public static void UpdateTurnTracker() {
            bool myTurn = GameLogic.myTurn;

            if(myTurn) turnTracker.Text = "My Turn";
            else turnTracker.Text = "Opponent's Turn";
            turnTracker.Background = null;
        }

        public static void ServiceDown(string serviceName) {
            turnTracker.Text = $"You took the {serviceName} service down!!!";
            turnTracker.Background = new SolidColorBrush(new Color(255, 190, 0, 0));
        }
    }
}
