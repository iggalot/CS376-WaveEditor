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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using System.IO;



namespace WAVEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WAVChart chtTime = new WAVChart();
        private WAVChart chtFreq = new WAVChart();
        private WAVChart chtChart = new WAVChart();
        private bool FileIsLoaded = false;  // a counter to tell us if the file has been successfully loaded
        private System.Drawing.Color BackgroundColor;
        private System.Drawing.Color ForegroundColor;
        
        // A wrapper class for a chart object that stores the permanent max and min values of the chart at load time.
        class WAVChart : Chart
        {
           public double x_max = 0;
           public double x_min = 0;
           public double y_max = 0;
           public double y_min = 0;
           public Chart chart;

            //constructor
            public WAVChart()
           {
               chart = new Chart();
           }

        }

        // attributes
        private SoundFile soundFile = new SoundFile();  // stores the unfiltered raw sound data;

        private void cnvTimeChart_Loaded(object sender, RoutedEventArgs e)
        {
            {
                System.Windows.Forms.Integration.WindowsFormsHost host1 =
                     new System.Windows.Forms.Integration.WindowsFormsHost();
                host1.Child = chtTime.chart;
                // Add the charts to the canvas so it can be displayed.
                this.cnvChartTime.Children.Add(host1);
            }
        }

        private void cnvFreqChart_Loaded(object sender, RoutedEventArgs e)
        {
            {
                System.Windows.Forms.Integration.WindowsFormsHost host1 =
                     new System.Windows.Forms.Integration.WindowsFormsHost();
                host1.Child = chtFreq.chart;
                // Add the charts to the canvas so it can be displayed.
                this.cnvChartFreq.Children.Add(host1);
            }
        }

        private void PrepareChartInfo(ChartArea chartArea, Series series)
        {
            // For miscellaneous chart formatting
            chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;

            return;
        }

        private void ChangeForecolorAll(Chart chart)
        {
            chart.Titles[0].ForeColor = ForegroundColor;
            chart.ChartAreas[0].AxisX.LabelStyle.ForeColor = ForegroundColor;
            chart.ChartAreas[0].AxisX2.LabelStyle.ForeColor = ForegroundColor;
            chart.ChartAreas[0].AxisY.LabelStyle.ForeColor = ForegroundColor;
            chart.ChartAreas[0].AxisY2.LabelStyle.ForeColor = ForegroundColor;
            chart.Legends[0].ForeColor = ForegroundColor;
        }
        private void PlotData(SoundFile s)
        {
            double width = cnvChartTime.Width;
            double height = cnvChartTime.Height;

            chtTime.chart.ChartAreas.Clear();
            chtTime.chart.Legends.Clear();
            chtTime.chart.Series.Clear();
            chtTime.chart.Titles.Clear();

            //////////////////////////////////////////////////////////////////////////////////
            //plot the time domain response
            //////////////////////////////////////////////////////////////////////////////////
            ChartArea timeChartArea = new ChartArea();
            timeChartArea.Position = new ElementPosition(5, 5, 95, 95);
            chtTime.chart.ChartAreas.Add(timeChartArea);
            chtTime.chart.Titles.Add("Amplitude vs. Time");

            chtTime.chart.Series.Add(s.SeriesDataTime(s));
            chtTime.chart.Series[0].Name = "TimeResponse";
            chtTime.chart.Legends.Add(new Legend("Time1"));
            chtTime.chart.Series[0].Legend = "Time1";
            chtTime.chart.Series[0].IsVisibleInLegend = true;

            chtTime.chart.Width = (int)width;
            chtTime.chart.Height = (int)height;
            PrepareChartInfo(timeChartArea, chtTime.chart.Series[0]);
            chtTime.chart.ChartAreas[0].AxisX.Title = "Time (seconds)";
            chtTime.chart.ChartAreas[0].AxisY.Title = "Amplitude";
            chtTime.chart.ChartAreas[0].AxisY.LabelStyle.Format = "{0.00}";
            chtTime.chart.ChartAreas[0].AxisX.LabelStyle.Format = "{0.00}";
            chtTime.chart.ChartAreas[0].AxisY.Maximum = s.y_time_max;
            chtTime.chart.ChartAreas[0].AxisY.Minimum = s.y_time_min;
            chtTime.chart.ChartAreas[0].AxisX.Maximum = s.x_time_max;
            chtTime.chart.ChartAreas[0].AxisX.Minimum = s.x_time_min;
            chtTime.chart.ChartAreas[0].BackColor = BackgroundColor;
            ChangeForecolorAll(chtTime.chart);
         
            //////////////////////////////////////////////////////////////////////////////////
            //plot the frequency response
            //////////////////////////////////////////////////////////////////////////////////
            chtFreq.chart.ChartAreas.Clear();
            chtFreq.chart.Legends.Clear();
            chtFreq.chart.Series.Clear();
            chtFreq.chart.Titles.Clear();

            ChartArea freqChartArea = new ChartArea();
            freqChartArea.Position = new ElementPosition(5, 5, 95, 95);
            chtFreq.chart.ChartAreas.Add(freqChartArea);
            chtFreq.chart.Titles.Add("Freq vs. Response");

            chtFreq.chart.Series.Add(s.SeriesDataFTT(soundFile));
            chtFreq.chart.Series[0].Name = "FrequencyResponse";
            chtFreq.chart.Legends.Add(new Legend("Freq1"));
            chtFreq.chart.Series[0].Legend = "Freq1";
            chtFreq.chart.Series[0].IsVisibleInLegend = true;
            chtFreq.chart.Width = (int)width;
            chtFreq.chart.Height = (int)height;
            PrepareChartInfo(freqChartArea, chtFreq.chart.Series[0]);
            chtFreq.chart.ChartAreas[0].AxisX.Title = "Frequency";
            chtFreq.chart.ChartAreas[0].AxisY.Title = "Frequency Response";
            chtFreq.chart.ChartAreas[0].AxisY.LabelStyle.Format = "{0.00}";
            chtFreq.chart.ChartAreas[0].AxisX.LabelStyle.Format = "{0}";
            chtFreq.chart.ChartAreas[0].AxisY.Maximum = s.y_freq_max;
            chtFreq.chart.ChartAreas[0].AxisY.Minimum = s.y_freq_min;
            chtFreq.chart.ChartAreas[0].AxisX.Maximum = s.x_freq_max;
            chtFreq.chart.ChartAreas[0].AxisX.Minimum = s.x_freq_min;
            chtFreq.chart.ChartAreas[0].BackColor = BackgroundColor;
            ChangeForecolorAll(chtFreq.chart);

            //// FOR DEBUG PURPOSES
            //System.Windows.Forms.MessageBox.Show(
            //    "Time:   max: " + s.x_time_max + "  min: " + s.x_time_min + "\n" +
            //    "Ampl:   max: " + s.y_time_max + "  min: " + s.y_time_min + "\n" +
            //    "FResp:   max: " + s.y_freq_max + "  min: " + s.y_freq_min + "\n" +
            //    "Freq:   max: " + s.x_freq_max + "  min: " + s.x_freq_min + "\n");
            chtFreq.Refresh();
            chtTime.Refresh();
        }

        // The play button
        private void ButtonPlaySoundFile_Click(object sender, RoutedEventArgs e)
        {
                soundFile.playSoundFile();
        }

        // Menu function to save a converted image
        private void MenuChangeBackgroundColor_Click(object sender, RoutedEventArgs e)
        {
            if (!FileIsLoaded)
            {
                System.Windows.Forms.MessageBox.Show("Please load a file first!");
                return;
            }

            ColorDialog cd = new ColorDialog();
            var result = cd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.BackgroundColor = System.Drawing.Color.FromArgb(255, cd.Color.R, cd.Color.G, cd.Color.B);
                PlotData(soundFile);
            }
        }

        // Formatting changes to the plot areas
        private void MenuChangeForegroundColor_Click(object sender, RoutedEventArgs e)
        {
            if (!FileIsLoaded)
            {
                System.Windows.Forms.MessageBox.Show("Please load a file first!");
                return;
            }

            ColorDialog cd = new ColorDialog();
            var result = cd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.MessageBox.Show("ARGB: " + cd.Color.R + ":" + cd.Color.G + ":" + cd.Color.B);
                this.ForegroundColor = System.Drawing.Color.FromArgb(255, cd.Color.R, cd.Color.G, cd.Color.B);
                PlotData(soundFile);
            }

        }

        // Menu function to load an image
        private void LoadFile(object sender, RoutedEventArgs e)
        {
            string fileName = null;
            string extension = null;
           // string filepath = null;

            using (System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog())
            {
                string path = System.Environment.CurrentDirectory;
                openFileDialog1.InitialDirectory = System.IO.Path.GetFullPath(System.IO.Path.Combine(path, @"..\..\WAVSamples"));
                openFileDialog1.Filter = "Sound files (*.wav, *.mpg, *.mpeg) | *.wav; *.mpg; *.mpeg;";
                openFileDialog1.Title = "Load sound file ...";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = openFileDialog1.FileName;
                    extension = System.IO.Path.GetExtension(fileName);
                }
            }

            if (fileName == null)
            {
                System.Windows.Forms.MessageBox.Show("No file selected.");
                return;
            }
            spPlots.Visibility = System.Windows.Visibility.Visible;
            btnLoadSoundFile.Visibility = System.Windows.Visibility.Collapsed;

            // delete the previous soundfile and save the newly loaded one
            soundFile = null;   // delete our current soundfile
            soundFile = new SoundFile(fileName, extension);  // load our new soundfile  
            this.FileIsLoaded = true;
            UpdateAll();
        }
        private void UpdateAll()
        {
            PlotData(soundFile);
            cnvFileInfo.Text = soundFile.soundFileName; // update the display to show the file name
            cnvFileWAVInfo.Text = "Sample Rate(fs): " + soundFile.fs.ToString("F0") + "   Total Samples(totSamp): " + soundFile.totSamp.ToString("F0") + "   Cutoff Freq(fc): " + soundFile.fc.ToString("F0") +
                "\nDuration(dur): " + soundFile.dur.ToString("F2") + " sec" + "   Channels(ch): " + soundFile.ch.ToString("F0") + "   Bits(bits): " + soundFile.bits.ToString("F0");
        }
        
        private void MenuSoundFileLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadFile(sender, e);    
        }

        // Menu function to load an image
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

            // Menu function to load an image
        private void MenuHelp_Click(object sender, RoutedEventArgs e)
        {
            string strText = "";
            strText += "==========================================================================\n";
            strText += "WAVEditor v1.0\n";
            strText += "==========================================================================\n";
            strText += "LOAD SOUND FILE -- Opens a dialog to allow th user to select a WAV file\n";
            strText += "PLAY SOUND FILE -- Plays the currently loaded sound file\n";
            strText += "APPLY FILTER    -- Applies a low-pass or high-pass filter with a specified\n";
            strText += "                   cutoff frequency\n";
            strText += "----------------------------------------------\n";
            strText += "Features:\n";
            strText += "MODIFY Buttons  -- Allows the user to change the scale of the X and Y axes\n";
            strText += "                   of the plots\n";
            strText += "FORMAT Menu     -- Allows the user to change the foreground and background\n";
            strText += "                   colors of the plots\n";
            strText += "==========================================================================\n";
            System.Windows.Forms.MessageBox.Show(strText);
        }

        public void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            string strText = "";
            strText += "WAVEditor v1.0\n";
            strText += "by Jim Allen\n";
            strText += "Copyright July 2018\n";
            System.Windows.Forms.MessageBox.Show(strText);
        }

        private void ButtonModifyAxisTime_Click(object sender, RoutedEventArgs e)
        {
            txtModTimeAxisXMax.Text = chtTime.chart.ChartAreas[0].AxisX.Maximum.ToString();
            txtModTimeAxisXMin.Text = chtTime.chart.ChartAreas[0].AxisX.Minimum.ToString();
            txtModTimeAxisYMax.Text = chtTime.chart.ChartAreas[0].AxisY.Maximum.ToString();
            txtModTimeAxisYMin.Text = chtTime.chart.ChartAreas[0].AxisY.Minimum.ToString();

            modTimeX.Visibility = System.Windows.Visibility.Visible;
            modTimeY.Visibility = System.Windows.Visibility.Visible;
            btnModifyTime.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void closeTimeMod()
        {
            modTimeX.Visibility = System.Windows.Visibility.Collapsed;
            modTimeY.Visibility = System.Windows.Visibility.Collapsed;
            btnModifyTime.Visibility = System.Windows.Visibility.Visible;
        }
        private void ButtonModTimeOK_Click(object sender, RoutedEventArgs e)
        {
            double xmax = 0;
            double xmin = 0;
            double ymax = 0;
            double ymin = 0;

            if(!Double.TryParse(txtModTimeAxisXMax.Text, out xmax)){
                System.Windows.Forms.MessageBox.Show("Invalid XMax value.");
                return;
            }
            else
            {
                if (xmax > soundFile.x_time_max)
                    xmax = soundFile.x_time_max;
            }
            if (!Double.TryParse(txtModTimeAxisXMin.Text, out xmin))
            {
                System.Windows.Forms.MessageBox.Show("Invalid XMinalue.");
                return;
            }
            else
            {
                if (xmin < soundFile.x_time_min)
                    xmin = soundFile.x_time_min;
            }
            if (!Double.TryParse(txtModTimeAxisYMax.Text, out ymax))
            {
                System.Windows.Forms.MessageBox.Show("Invalid Ymax value.");
                return;
            } else 
            {
                if (ymax > soundFile.y_time_max)
                    ymax = soundFile.y_time_max;
            }
            if (!Double.TryParse(txtModTimeAxisYMin.Text, out ymin))
            {
                System.Windows.Forms.MessageBox.Show("Invalid Ymin value.");
                return;
            }
            else
            {
                if (ymin < soundFile.y_time_min)
                    ymin = soundFile.y_time_min;
            }

            // Error checking of input values
            if ((xmax <= xmin) || (xmin >= xmax))
            {
                xmax = soundFile.x_time_max;
                xmin = soundFile.x_time_min;
                System.Windows.Forms.MessageBox.Show("The max. X values must be larger than the min. X value.");
                return;
            }
            if ((ymax <= ymin) || (ymin >= ymax))
            {
                ymax = soundFile.y_time_max;
                ymin = soundFile.y_time_min;
                System.Windows.Forms.MessageBox.Show("The max. X values must be larger than the min. X value.");
                return;
            }
                
            // now set the values
            chtTime.chart.ChartAreas[0].AxisX.Maximum = xmax;
            chtTime.chart.ChartAreas[0].AxisX.Minimum = xmin;
            chtTime.chart.ChartAreas[0].AxisY.Maximum = ymax;
            chtTime.chart.ChartAreas[0].AxisY.Minimum = ymin;
 
            chtTime.chart.Refresh();

            closeTimeMod();
        }

        private void ButtonModTimeDefault_Click(object sender, RoutedEventArgs e)
        {
            chtTime.chart.ChartAreas[0].AxisX.Maximum = soundFile.x_time_max;
            chtTime.chart.ChartAreas[0].AxisX.Minimum = soundFile.x_time_min;
            chtTime.chart.ChartAreas[0].AxisY.Maximum = soundFile.y_time_max;
            chtTime.chart.ChartAreas[0].AxisY.Minimum = soundFile.y_time_min;

            chtTime.chart.Refresh();
            closeTimeMod();
        }
        private void ButtonModTimeCancel_Click(object sender, RoutedEventArgs e)
        {
            closeTimeMod();
        }
        private void ButtonModifyAxisFreq_Click(object sender, RoutedEventArgs e)
        {
            txtModFreqAxisXMax.Text = chtFreq.chart.ChartAreas[0].AxisX.Maximum.ToString();
            txtModFreqAxisXMin.Text = chtFreq.chart.ChartAreas[0].AxisX.Minimum.ToString();
            txtModFreqAxisYMax.Text = chtFreq.chart.ChartAreas[0].AxisY.Maximum.ToString();
            txtModFreqAxisYMin.Text = chtFreq.chart.ChartAreas[0].AxisY.Minimum.ToString();

            modFreqX.Visibility = System.Windows.Visibility.Visible;
            modFreqY.Visibility = System.Windows.Visibility.Visible;
            btnModifyFreq.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void closeFreqMod()
        {
            modFreqX.Visibility = System.Windows.Visibility.Collapsed;
            modFreqY.Visibility = System.Windows.Visibility.Collapsed;
            btnModifyFreq.Visibility = System.Windows.Visibility.Visible;
        }
        private void ButtonModFreqOK_Click(object sender, RoutedEventArgs e)
        {
            double xmax = 0;
            double xmin = 0;
            double ymax = 0;
            double ymin = 0;

            if (!Double.TryParse(txtModFreqAxisXMax.Text, out xmax))
            {
                System.Windows.Forms.MessageBox.Show("Invalid XMax value.");
                return;
            }
            else
            {
                if (xmax > soundFile.x_freq_max)
                    xmax = soundFile.x_freq_max;
            }
            if (!Double.TryParse(txtModFreqAxisXMin.Text, out xmin))
            {
                System.Windows.Forms.MessageBox.Show("Invalid XMinalue.");
                return;
            }
            else
            {
                if (xmin < soundFile.x_freq_min)
                    xmin = soundFile.x_freq_min;
            }
            if (!Double.TryParse(txtModFreqAxisYMax.Text, out ymax))
            {
                System.Windows.Forms.MessageBox.Show("Invalid Ymax value.");
                return;
            }
            else
            {
                if (ymax > soundFile.y_freq_max)
                    ymax = soundFile.y_freq_max;
            }
            if (!Double.TryParse(txtModFreqAxisYMin.Text, out ymin))
            {
                System.Windows.Forms.MessageBox.Show("Invalid Ymin value.");
                return;
            }
            else
            {
                if (ymin < soundFile.y_freq_min)
                    ymin = soundFile.y_freq_min;
            }

            // Error checking of input values
            if ((xmax <= xmin) || (xmin >= xmax))
            {
                xmax = soundFile.x_freq_max;
                xmin = soundFile.x_freq_min;
                System.Windows.Forms.MessageBox.Show("The max. X values must be larger than the min. X value.");
                return;
            }
            if ((ymax <= ymin) || (ymin >= ymax))
            {
                ymax = soundFile.y_freq_max;
                ymin = soundFile.y_freq_min;
                System.Windows.Forms.MessageBox.Show("The max. X values must be larger than the min. X value.");
                return;
            }

            // now set the values
            chtFreq.chart.ChartAreas[0].AxisX.Maximum = xmax;
            chtFreq.chart.ChartAreas[0].AxisX.Minimum = xmin;
            chtFreq.chart.ChartAreas[0].AxisY.Maximum = ymax;
            chtFreq.chart.ChartAreas[0].AxisY.Minimum = ymin;

            chtFreq.chart.Refresh();

            closeFreqMod();
        }

        private void ButtonModFreqDefault_Click(object sender, RoutedEventArgs e)
        {
            chtFreq.chart.ChartAreas[0].AxisX.Maximum = soundFile.x_freq_max;
            chtFreq.chart.ChartAreas[0].AxisX.Minimum = soundFile.x_freq_min;
            chtFreq.chart.ChartAreas[0].AxisY.Maximum = soundFile.y_freq_max;
            chtFreq.chart.ChartAreas[0].AxisY.Minimum = soundFile.y_freq_min;

            chtFreq.chart.Refresh();
            closeFreqMod();
        }

        private void ButtonModFreqCancel_Click(object sender, RoutedEventArgs e)
        {
            closeFreqMod();
        }

        private void ButtonFilterSoundFile_Click(object sender, RoutedEventArgs e)
        {
            spFilterInput.Visibility = System.Windows.Visibility.Visible;
            btnFilterOK.Visibility = System.Windows.Visibility.Hidden;
            btnPlaySoundFile.Visibility = System.Windows.Visibility.Collapsed;
            btnFilterSoundFile.Visibility = System.Windows.Visibility.Collapsed;
            btnModifyTime.Visibility = System.Windows.Visibility.Hidden;
            btnModifyFreq.Visibility = System.Windows.Visibility.Hidden;

            // display warnings
            if (lblFilenameStatus.Content == "")
            {
                lblFilenameStatus.Content = "Filename required";
                lblFilenameStatus.Foreground = System.Windows.Media.Brushes.Red;
            }

            if (lblCutoffFreqStatus.Content == "")
            {
                lblCutoffFreqStatus.Content = "Cutoff frequency value required";
                lblCutoffFreqStatus.Foreground = System.Windows.Media.Brushes.Red;
            }

            //display the acceptable fc range (fs/10) < fc < 4fs/10)
            lblMinFC.Content = "Min: " + (soundFile.fs / 10.0).ToString("F0");
            lblMaxFC.Content = "Max: " + (4.0 * soundFile.fs / 10.0).ToString("F0");
        }

        private void textFilenameChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            bool statusOK = false;

            if (string.IsNullOrWhiteSpace(txtFilterFileName.Text))
            {
                lblFilenameStatus.Content = "Filename required";
                lblFilenameStatus.Foreground = System.Windows.Media.Brushes.Red;
                statusOK = false;
            }

            // check that filename is not already in use in the directory
            string path = SoundFile.MLWavDirPath();
            string extension = ".wav";
            string newfilename = path + "\\" + txtFilterFileName.Text + extension;

            if (File.Exists(newfilename))
            {
                lblFilenameStatus.Content = "  Invalid! Filename is already used.";
                lblFilenameStatus.Foreground = System.Windows.Media.Brushes.Red;
                statusOK = false;
            }
            else
            {
                lblFilenameStatus.Content = "  OK";
                lblFilenameStatus.Foreground = System.Windows.Media.Brushes.Green;
                statusOK = true;
            }

            btnFilterOK.Visibility = (statusOK) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }

        private void textFCChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            bool statusOK = false;

            double newfc = 0;
            if (Double.TryParse(txtCutoffFreq.Text, out newfc))
            {
                if ((newfc < soundFile.fs / 10.0) || (newfc > (4 * soundFile.fs / 10.0)))
                {
                    lblCutoffFreqStatus.Content = "  Invalid!  Out of range.";
                    lblCutoffFreqStatus.Foreground = System.Windows.Media.Brushes.Red;
                    statusOK = false;
                }
                else
                {
                    lblCutoffFreqStatus.Content = "  OK";
                    lblCutoffFreqStatus.Foreground = System.Windows.Media.Brushes.Green;
                    statusOK = true;
                }
            }
            else
            {
                lblCutoffFreqStatus.Content = "  Invalid! Numeric value required.";
                lblCutoffFreqStatus.Foreground = System.Windows.Media.Brushes.Red;
                statusOK = false;
            }

            btnFilterOK.Visibility = (statusOK) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        private void ButtonFilterOK_Click(object sender, RoutedEventArgs e)
        {
            double newfc = 0;
            if (!Double.TryParse(txtCutoffFreq.Text, out newfc))
            {
                System.Windows.Forms.MessageBox.Show("Invalid cutoff frequency.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFilterFileName.Text))
            {
                System.Windows.Forms.MessageBox.Show("Filtered Filename is required!");
                return;
            }

            // check that filename is not already in use in the directory
            string path = SoundFile.MLWavDirPath();
            string extension = ".wav";
            string newfilename = path + "\\" + txtFilterFileName.Text + extension;

            if (File.Exists(newfilename))
            {
                System.Windows.Forms.MessageBox.Show("Filename already exists in directory " + path + "\nPlease try another filename");
            }

            string passtype = "";
            if(rbHighPass.IsChecked == true)
            {
                passtype = "High";
            }
            else
            {
                passtype = "Low";
            }

            soundFile.makeFiltered(passtype, newfc, txtFilterFileName.Text);

            // delete the previous soundfile and save the newly loaded one
            soundFile = null;   // delete our current soundfile
            soundFile = new SoundFile(newfilename, extension);  // load our new soundfile  
            UpdateAll();                                        // update our display for the new filtered file 

            btnPlaySoundFile.Visibility = System.Windows.Visibility.Visible;
            btnFilterSoundFile.Visibility = System.Windows.Visibility.Visible;
            spFilterInput.Visibility = System.Windows.Visibility.Collapsed;
            btnModifyTime.Visibility = System.Windows.Visibility.Visible;
            btnModifyFreq.Visibility = System.Windows.Visibility.Visible;
        }
        private void ButtonFilterCancel_Click(object sender, RoutedEventArgs e)
        {
            btnPlaySoundFile.Visibility = System.Windows.Visibility.Visible;
            btnFilterSoundFile.Visibility = System.Windows.Visibility.Visible;
            spFilterInput.Visibility = System.Windows.Visibility.Collapsed;
            btnModifyTime.Visibility = System.Windows.Visibility.Visible;
            btnModifyFreq.Visibility = System.Windows.Visibility.Visible;
        }
        public MainWindow()
        {
            InitializeComponent();
            BackgroundColor = System.Drawing.Color.FromArgb(255, 205, 217, 234);
            ForegroundColor = System.Drawing.Color.FromArgb(255, 50, 50, 50);
        }
    }
}
