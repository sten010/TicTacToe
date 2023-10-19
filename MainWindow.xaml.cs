using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TicTacToe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Play _Play;
        private Play PlayClass
        {
            get
            {
                if (_Play != null)
                {
                    return _Play;
                }
                _Play = new Play
                {
                    Main = this
                };
                return _Play;
            }
            set
            {
                if (_Play != value)
                {
                    _Play = value;
                }
            }
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(NamePlayer.Text))
            {
                Start.IsEnabled = false;
                return;
            }
            else
            {
                Start.IsEnabled = true;
            }
            TextBlock typeItem = (TextBlock)ImagePictorCombo.SelectedItem;
            if (typeItem.Text == "Крестик")
            {
                PlayClass.StartPlay(@"/Image/RedCircle.png", @"/Image/GreenCross.png");
            }
            else
            {
                PlayClass.StartPlay(@"/Image/GreenCross.png", @"/Image/RedCircle.png");
            }
           
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NamePlayer.Text))
            {
                Reset.IsEnabled = false;
                return;
            }
            else
            {
                Reset.IsEnabled = true;
            }
            PlayClass.ResetGame();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var image = (Image)sender;
            PlayClass.PlayMove(image);
        }
        private void NamePlayer_KeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(NamePlayer.Text))
            {
                Start.IsEnabled = true;
            }
            else
            {
                Start.IsEnabled = false;
            }
        }
    }
    public class Play
    {
        public MainWindow Main { get; set; }

        private List<int> playerList = new();
        private List<int> botList = new();
        private bool finish = false;
        private string BotImage ;
        private string PlayerImage ;


        readonly List<int[]> ArrayList = new()
            {
                new int[] { 1, 2, 3 },
                new int[] { 4, 5, 6 },
                new int[] { 7, 8, 9 },
                new int[] { 1, 4, 7 },
                new int[] { 2, 5, 8 },
                new int[] { 3, 6, 9 },
                new int[] { 1, 5, 9 },
                new int[] { 3, 5, 7 }
            };
        public void PlayMove(Image image)
        {
            if (finish) return;
            PlayerMove(image);
            if (finish) return;
            BotMove(image);
        }
        private void PlayerMove(Image image)
        {
            LoadImage(image, PlayerImage);
            image.IsEnabled = false;
            playerList.Add(Convert.ToInt32(image.Tag));
            CheckWin(playerList, 1);
        }
        private void BotMove(Image image)
        {
            Main.TextControl.Content = "Ход бота";
            Main.IsEnabled = false;
            int botMove = 0;
            int analytibot = analytics(botList, playerList);
            int analytiPlayer = analytics(playerList, botList);
            if (analytiPlayer > 0)
            {
                botMove = analytiPlayer;
            }
            if (analytibot > 0)
            {
                botMove = analytibot;
            }
            if (analytiPlayer == 0 && analytibot == 0)
            {
                botMove = RandomCp(botList, playerList);
            }

            var controlRez = FindVisualChildren<Image>(Main);
            foreach (var item in controlRez)
            {
                if (item.Tag.ToString() == botMove.ToString())
                {
                    LoadImage(item, BotImage);
                    item.IsEnabled = false;

                }
            }
            botList.Add(botMove);
            Main.TextControl.Content = "Ход Игрока";
            Main.IsEnabled = true;
            CheckWin(botList, 0);
        }
        private void FinishRound(string winner)
        {
            Main.TextControl.Content = winner;
            Main.Start.IsEnabled = true;
            Main.Reset.IsEnabled = false;
            finish = true;
        }
        public void StartPlay(string botImage, string playerImage)
        {
            Main.TextControl.Content = "Старт";
            Main.Start.IsEnabled = false;
            Main.Reset.IsEnabled = true;
            Main.NamePlayer.IsEnabled = false;
            playerList.Clear();
            botList.Clear();
            finish = false;
            BotImage = botImage;
            PlayerImage = playerImage;
            foreach (Image tb in FindVisualChildren<Image>(Main))
            {
                LoadImage(tb, @"/Image/Black.png");
                tb.IsEnabled = true;
            }
        }
        private void LoadImage(Image image, string url)
        {
            image.Source = new BitmapImage(new Uri(url, UriKind.Relative));

        }
        public void ResetGame()
        {
            Main.TextControl.Content = "Сброс";
            Main.Start.IsEnabled = true;
            Main.Reset.IsEnabled = false;
            Main.NamePlayer.IsEnabled = true;

            foreach (Image tb in FindVisualChildren<Image>(Main))
            {
                LoadImage(tb, string.Empty);
            }

        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void CheckWin(List<int> values, int move)
        {
            for (int i = 0; i < ArrayList.Count; i++)
            {
                var tempArray = ArrayList[i];
                var firstType = values.Count(c => c.Equals(tempArray[0]));
                var secondType = values.Count(c => c.Equals(tempArray[1]));
                var thirdType = values.Count(c => c.Equals(tempArray[2]));
                if (firstType.Equals(1) && secondType.Equals(1) && thirdType.Equals(1))
                {
                    if (move == 1)
                    {
                        FinishRound(string.Format("«Вы выиграли, {0}!» ", Main.NamePlayer.Text.ToString()));
                    }
                    else
                    {
                        FinishRound(string.Format("Вы проиграли, {0}!» ", Main.NamePlayer.Text.ToString()));
                    }
                }
            }
            if (FindVisualChildren<Image>(Main).Count(c => c.IsEnabled) == 0)
            {
                FinishRound(string.Format("Ничья"));
            }
        }

        private int analytics(List<int> valueMove, List<int> valuesEnemy)
        {
            for (int i = 0; i < ArrayList.Count; i++)
            {
                var tempArray = ArrayList[i];

                var firstType = valueMove.Count(c => c.Equals(tempArray[0]));
                var secondType = valueMove.Count(c => c.Equals(tempArray[1]));
                var thirdType = valueMove.Count(c => c.Equals(tempArray[2]));

                if (firstType.Equals(1) && secondType.Equals(1))
                {
                    int rez = tempArray[2];
                    if (valuesEnemy.Count(c => c == rez) > 0) continue;
                    return rez;
                }
                if (firstType.Equals(1) && thirdType.Equals(1))
                {
                    int rez = tempArray[1];
                    if (valuesEnemy.Count(c => c == rez) > 0) continue;
                    return rez;
                }
                if (secondType.Equals(1) && thirdType.Equals(1))
                {
                    int rez = tempArray[0];
                    if (valuesEnemy.Count(c => c == rez) > 0) continue;
                    return rez;
                }
            }
            return 0;
        }
        private int RandomCp(List<int> valueMove, List<int> valuesEnemy)
        {
            //Создание объекта для генерации чисел
            Random rnd = new Random();
            while (true)
            {
                int value = rnd.Next(1, 9);
                var element = FindVisualChildren<Image>(Main).First(c => c.Tag.ToString() == value.ToString());
                if (valueMove.Count(c => c == value) == 0 && valuesEnemy.Count(c => c == value) == 0)
                {
                    return value;
                }
            }
        }
    }
}
