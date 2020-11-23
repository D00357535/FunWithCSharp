using System;
using System.Windows;//
using System.Windows.Controls;//
using System.Windows.Controls.Primitives;//
using System.Windows.Input;//
using System.Windows.Threading;//
using Microsoft.Win32;//
using System.IO;//
using System.Collections.Generic;//

namespace WpfTutorialSamples.Audio_and_Video
{
	public partial class AudioVideoPlayerCompleteSample : Window
	{
		private bool mediaPlayerIsPlaying = false;
		private bool userIsDraggingSlider = false;
		private string LOADED_SKIP_FILE = @"";
		List<double> skipTimes = new List<double>();

		public AudioVideoPlayerCompleteSample()
		{
			InitializeComponent();

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(1);
			timer.Tick += timer_Tick;
			timer.Start();
		}

		private void timer_Tick(object sender, EventArgs e)
		{
			if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
			{
				sliProgress.Minimum = 0;
				sliProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
				sliProgress.Value = mePlayer.Position.TotalSeconds;
			}
			if (sliProgress.Value >= sliProgress.Maximum)
			{
				mePlayer.Position = TimeSpan.FromSeconds(0);
			}
			for (int i = 0; i <= skipTimes.Count - 1; i += 2)
			{
				if (sliProgress.Value >= skipTimes[i] && sliProgress.Value < skipTimes[i + 1])
				{
					break;
				}
				else if (sliProgress.Value <= skipTimes[i])
				{
					TimeSpan time = TimeSpan.FromSeconds(skipTimes[i]);
					mePlayer.Position = time;
					break;
				}
				else if (sliProgress.Value > skipTimes[skipTimes.Count - 1])
				{
					//makes it so it doesn't end: mePlayer.Position = TimeSpan.FromMilliseconds(mePlayer.NaturalDuration.TimeSpan.TotalSeconds);
					mePlayer.Stop();
					mediaPlayerIsPlaying = false;
				}

			}
		}

		//for (int i = 0; i <= skipTimes.Count-1; i += 2)
          //  {
			//	if (sliProgress.Value >= skipTimes[i] && sliProgress.Value<skipTimes[i + 1])
              //  {
				//	TimeSpan time = TimeSpan.FromSeconds(skipTimes[i + 1]);
			//mePlayer.Position = time;
              //  }
			//}

	private void Button_Click(object sender,EventArgs e)
        {
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Media files (*.mp4;)|*.mp4";
			if (fileDialog.ShowDialog() == true)
            {
				mePlayer.Source = new Uri(fileDialog.FileName);
            }
        }

		private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Media files (*.mp3;*.mpg;*.mpeg)|*.mp3;*.mpg;*.mpeg|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == true)
				mePlayer.Source = new Uri(openFileDialog.FileName);
		}

		private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
		}

		private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			mePlayer.Play();
			mediaPlayerIsPlaying = true;
		}

		private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mediaPlayerIsPlaying;
		}

		private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			mePlayer.Pause();
		}

		private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = mediaPlayerIsPlaying;
		}

		private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			mePlayer.Stop();
			mediaPlayerIsPlaying = false;
		}

		private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
		{
			userIsDraggingSlider = true;
		}

		private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			userIsDraggingSlider = false;
			mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value);
		}

		private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			lblProgressStatus.Text = TimeSpan.FromSeconds(sliProgress.Value).ToString(@"hh\:mm\:ss");
		}

		private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			mePlayer.Volume += (e.Delta > 0) ? 0.1 : -0.1;
		}


        private void PullSkips(object sender, RoutedEventArgs e)
        {
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == true)
			{

				using (var reader = new StreamReader(openFileDialog.FileName))
				{
					string line;
					while ((line = reader.ReadLine()) != null)
					{
						char[] delimiters = { ' ', '\t' };
						string[] words = line.Split(delimiters);
						Label start = new Label();
						Label end = new Label();
						start.Content = words[0];
						end.Content = words[1];
						StartTime.Items.Add(start);
						EndTime.Items.Add(end);
						skipTimes.Add(Convert.ToDouble(words[0]));
						skipTimes.Add(Convert.ToDouble(words[1]));
					}
				}

			}
		}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (sliProgress.Value <= 10)
            {
				mePlayer.Position = TimeSpan.FromSeconds(0);
            }
            else
            {
				mePlayer.Position = TimeSpan.FromSeconds(sliProgress.Value - 10);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
			mePlayer.SpeedRatio = 0.25;
        }
    }
}