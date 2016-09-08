using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Windows.Threading;

namespace Memory_Game
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Player> player = new List<Player>();
        private Player currentPlayer;
        private bool clickAllowed = true;
        private Button card1;
        private Button card2;
        private DispatcherTimer clickTimer = new DispatcherTimer();
        private List<ImageBrush> background = new List<ImageBrush>();
        private List<ImageBrush> cardBG = new List<ImageBrush>();

        public MainWindow()
        {
            InitializeComponent();
            Title = "Memory Game";
            ResizeMode = ResizeMode.NoResize;
            LoadBackground();
            LoadCardBG();
            CreateCards();
            clickTimer.Interval = new TimeSpan(0,0,2);
            clickTimer.Tick += ClickTimer_Tick;
        }

        private void ClickTimer_Tick(object sender, EventArgs e)
        {
            clickTimer.Stop();
            if (((Image)card1.Tag).Tag.ToString() == ((Image)card2.Tag).Tag.ToString())
            {
                c_Game.Children.Remove(card1);
                c_Game.Children.Remove(card2);
                currentPlayer.Score++;
                ShowScores();
                if (c_Game.Children.Count == 0)
                    Ranking();
            }
            else
            {
                TurnCard(card1);
                TurnCard(card2);
                if (currentPlayer.ID == player.Count)
                    currentPlayer = player[0];
                else
                    currentPlayer = player[currentPlayer.ID];
                ShowScores();
            }
            card1 = null;
            card2 = null;
            clickAllowed = true;
        }

        private void Ranking()
        {
            player.Sort();
            string ranking = "Ranking:\n\n";
            foreach (Player item in player)
            {
                ranking += item.ToString() + "\n";
            }
            Label _l = new Label();
            _l.Width = 600;
            _l.Height = 500;
            _l.FontSize = 36;
            _l.Foreground = Brushes.GreenYellow;
            _l.FontWeight = FontWeights.Bold;
            c_Game.Children.Add(_l);
            Canvas.SetLeft(_l, 100);
            Canvas.SetTop(_l, 10);
            _l.Content = ranking;
            MessageBox.Show("GAME OVER!");
            Close();
        }

        private void ShowScores()
        {
            foreach (Player item in player)
            {
                if (currentPlayer == item)
                    item.Lab.Foreground = Brushes.GreenYellow;
                else
                    item.Lab.Foreground = Brushes.White;
                item.Lab.Content = item.ToString();
            }
        }

        private void CreateCards()
        {
            Random rand = new Random();
            var _files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\Pictures");
            List<Image> _Images = new List<Image>();
            foreach (var item in _files)
            {
                for (int i = 0; i < 2; i++)
                {
                    Image _image = new Image();
                    _image.Source = new BitmapImage(new Uri(item));
                    _image.Tag = Path.GetFileNameWithoutExtension(item);
                    _Images.Add(_image);
                }
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int _num = rand.Next(0, _Images.Count);
                    Button b = new Button();
                    b.Width = 100;
                    b.Height = 100;
                    b.Background = cardBG[0];
                    b.MouseEnter += (s, e) => Mouse.OverrideCursor = Cursors.Hand;
                    b.MouseLeave += (s, e) => Mouse.OverrideCursor = Cursors.Arrow;
                    b.Tag = _Images[_num];                    
                    _Images.Remove(_Images[_num]);
                    c_Game.Children.Add(b);
                    Canvas.SetLeft(b, i * 100);
                    Canvas.SetTop(b, j * 100);
                }
            }
        }

        private void LoadCardBG()
        {
            var _files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\Card_BG");
            foreach (var item in _files)
            {
                cb_Card.Items.Add(Path.GetFileNameWithoutExtension(item));
                cardBG.Add(new ImageBrush(new BitmapImage(new Uri(item))));
            }
            cb_Card.SelectedItem = cb_Card.Items[0];
        }

        private void LoadBackground()
        {
            var _files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\Backgrounds");
            foreach (var item in _files)
            {
                cb_Background.Items.Add(Path.GetFileNameWithoutExtension(item));
                background.Add(new ImageBrush(new BitmapImage(new Uri(item))));
            }
            Background = background[0];
            cb_Background.SelectedItem = cb_Background.Items[0];
            for (int i = 2; i < 5; i++)
            {
                cb_Player.Items.Add(i + "Players");
            }
            cb_Player.SelectedItem = cb_Player.Items[0];
            ChangePlayers();           
        }

        private void ChangePlayers()
        {
            switch (Convert.ToInt32(cb_Player.SelectedIndex))
            {
                case 0:
                    tb_Name3.IsEnabled = false;
                    tb_Name4.IsEnabled = false;
                    break;
                case 1:
                    tb_Name3.IsEnabled = true;
                    tb_Name4.IsEnabled = false;
                    break;
                case 2:
                    tb_Name3.IsEnabled = true;
                    tb_Name4.IsEnabled = true;
                    break;
            }
        }

        private void cb_Background_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            Background = background[cb_Background.SelectedIndex];
        }

        private void cb_Card_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Button item in c_Game.Children)
            {
                item.Background = cardBG[cb_Card.SelectedIndex];
            }
        }

        private void cb_Player_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangePlayers();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            bool _start = true;      
            List<TextBox> _names = new List<TextBox> { tb_Name1, tb_Name2, tb_Name3, tb_Name4 };
            foreach (TextBox item in _names)
            {
                if (item.IsEnabled && item.Text == "")
                {
                    _start = false;
                    item.Background = Brushes.IndianRed;
                }
                else item.Background = Brushes.White;
            }
            if (_start)
                StartGame();
        }

        private void StartGame()
        {
            foreach (Button item in c_Game.Children)
            {
                item.Click += Card_Click;
            }
            List<TextBox> _names = new List<TextBox> { tb_Name1, tb_Name2, tb_Name3, tb_Name4 };
            int _top = 10;
            foreach (TextBox item in _names)
            {
                if (item.IsEnabled)
                {
                    Label _l = new Label();
                    _l.Width = 200;
                    _l.Height = 60;
                    _l.FontSize = 20;
                    _l.Foreground = Brushes.White;
                    _l.FontWeight = FontWeights.Bold;
                    c_Score.Children.Add(_l);
                    Canvas.SetLeft(_l, 10);
                    Canvas.SetTop(_l, _top);
                    _top += 60;
                    player.Add(new Player(item.Text,_l));
                }
            }               
            currentPlayer = player[0];                          
            c_Options.IsEnabled = false;
            ShowScores();
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (clickAllowed && ((Button)sender).Content == null)
            {
                if (card1 == null)
                {
                    card1 = (Button)sender;
                    TurnCard(card1);
                }
                else 
                {
                    card2 = (Button)sender;
                    TurnCard(card2);
                    clickAllowed = false;
                    clickTimer.Start();
                }
            }
        }

        private void TurnCard(Button button)
        {
            if(button.Content == null)
            {
                button.Background = Brushes.White;
                button.Content = button.Tag;
            }
            else
            {
                button.Content = null;
                button.Background = cardBG[cb_Card.SelectedIndex];
            }
        }
    }
}
