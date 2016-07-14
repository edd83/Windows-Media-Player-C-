using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;

namespace MyWindowsMediaPlayer
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private Playlist _currentList = new Playlist("music");
        private Playlist _library;
        private bool isRunning = false;
        private bool librarySelected = false;
        private string default_path = Environment.GetEnvironmentVariable("LOCALAPPDATA") + @"\myWMPLibrary.xml";

        public MainWindow()
        {
            InitializeComponent();
            this._library = Playlist.load(default_path);
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = new TimeSpan(1);
            _timer.Start();
            this.buttonAdd.Visibility = Visibility.Hidden;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
           
           timelineSlider.Value = MediaPlayer.Position.TotalSeconds;
           videoElementTime.Text = MediaPlayer.Position.Hours.ToString() + ":" + MediaPlayer.Position.Minutes.ToString() + ":" + MediaPlayer.Position.Seconds.ToString();
        }

        private void buttonPlay_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Play();
        }

        private void buttonSaveLib_Click(object sender, RoutedEventArgs e)
        {
            Playlist.save(default_path, this._library);
            MessageBox.Show("Library successfully saved !");
        }

        private void buttonPause_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Pause();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Stop();
        }
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".xml";
            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                Playlist.save(filename, _currentList);
            }
        }
        
        private void updateListBox(Playlist playlist)
        {
            ListBox.Items.Clear();
            if (playlist.getNbElement() > 0)
                playlist.items.ForEach(x => ListBox.Items.Add(x.title));
        }

        private void buttonLibrary_Click(object sender, RoutedEventArgs e)
        {
           
            if (this.librarySelected)
            {
                this.buttonAdd.Visibility = Visibility.Hidden;
                this.librarySelected = false;
                updateListBox(this._currentList);
            }
            else
            {
                this.buttonAdd.Visibility = Visibility.Visible;
                this._library = Playlist.load(default_path);
                updateListBox(this._library);
                this.librarySelected = true;
            }
        }

         private void buttonOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
             dlg.DefaultExt = ".xml";
             dlg.Filter = "Xml documents (.xml)|*.xml";
             if (dlg.ShowDialog() == true)
             {
                 _currentList = Playlist.load(dlg.FileName);
                 updateListBox(_currentList);
             }
        }

        private void speedRatioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.SpeedRatio = e.NewValue;
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = e.NewValue / 100;
        }

        private void MyWindowMediaPlayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void MyWindowMediaPlayer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
        }

        // MediaPlayer functions
        private void MediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            if (_currentList.Next())
            {
                Media newSong = _currentList.getCurrentItem();
                if (newSong != null)
                {
                    MediaPlayer.Source = new Uri(newSong.path);
                    MediaPlayer.Play();
                    isRunning = true;
                }
            }
        }

        private void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (_currentList.type != "picture")
            {
               System.Windows.Duration duration =  MediaPlayer.NaturalDuration;
                if (duration.HasTimeSpan)
                {
                    timelineSlider.Maximum = duration.TimeSpan.Seconds;
                }
            }
        }

        // ListBox Functions
        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            string path = "";
            string[] files = (string[])(e.Data.GetData(DataFormats.FileDrop, false));
            foreach (string file in files)
            {
                path = System.IO.Path.GetFullPath(file).ToString();
                ListBox.Items.Add(System.IO.Path.GetFileNameWithoutExtension(path));
                if (this.librarySelected)
                    this._library.AddSong(path);
                else
                    _currentList.AddSong(path);
            }
            Media mySong;
            if (this.librarySelected)
                mySong = this._library.getCurrentItem();
            else
                mySong = _currentList.getCurrentItem();
            if (mySong != null && isRunning == false)
            {
                MediaPlayer.Source = new Uri(mySong.path);
                MediaPlayer.Play();
                this.isRunning = true;
            }
        }

        private void Playlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MediaPlayer.Stop();
            this.isRunning = false;
            if (ListBox.SelectedItem != null)
            {
                string title = ListBox.SelectedItem.ToString();
                Media mySong;
                if (this.librarySelected)
                {
                    this._library.setIndex(this._library.getIndexByTitle(title));
                    mySong = this._library.getCurrentItem();
                }
                else
                {
                    _currentList.setIndex(_currentList.getIndexByTitle(title));
                    mySong = _currentList.getCurrentItem();
                }
                if (mySong != null)
                {
                    MediaPlayer.Source = new Uri(mySong.path);
                    MediaPlayer.Play();
                    isRunning = true;
                }
            }
        }

        private void timelineSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
