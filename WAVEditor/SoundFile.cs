using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace WAVEditor
{
    class SoundFile
    {   
        private MLApp.MLApp matlab;
        private string executepath = System.IO.Directory.GetCurrentDirectory();  // path of our executing .exe program

        // sound file infor
        public string soundFileName;
        public string soundFileExtension;

        // data attributes
        public double ampmaxvalue;  // max value of the amplitude
        public double ampminvalue;  // min value of the amplitude

        // stores the max and min values of the data
        public double x_time_max;
        public double x_time_min;
        public double y_time_max;
        public double y_time_min;
        public double x_freq_max;
        public double x_freq_min;
        public double y_freq_max;
        public double y_freq_min;

        // MATLAB related variables
        public double fs;           // used by MLWavInfo
        public double ch;            // used by MLWavInfo
        public double totSamp;       // used by MLWavInfo
        public double dur;           // used by MLWavInfo
        public double bits;          // used by MLWavInfo
        public double[,] soundData;     // used by MLReadWavFile
        public double[,] soundDataFResp; // used by MLFreqResp
        public double[,] soundDataFreq;  // used by MLFreqResp
        public double fc;           // used by MLWavFilter -- cutoff frequency;

        //constructor
        public SoundFile() : this(null, null)
        {
            // this is our default constructor
        }

        public SoundFile(string name, string exten)
        {
            this.soundFileName = name;
            this.soundFileExtension = exten;
            this.ampmaxvalue = 0;
            this.ampminvalue = 0;

            this.x_time_max = 0;
            this.x_time_min = 0;
            this.y_time_max = 0;
            this.y_time_min = 0;
            this.x_freq_max = 0;
            this.x_freq_min = 0;
            this.y_freq_max = 0;
            this.y_freq_min = 0;

            matlab = new MLApp.MLApp();

            if(string.Compare(name, null) != 0)
                readSoundFileData(); // now read the data from the file
        }

        private string MLFunctionPath()
        {
            string currpath = System.IO.Directory.GetCurrentDirectory();
            return(currpath.Substring(0, currpath.LastIndexOf("\\bin\\")) + "\\MLFunctions");
        }

        private void readSoundFileData()
        {
            if (soundFileName == null)
            {
                System.Windows.Forms.MessageBox.Show("Error reading sound file data");
                return; // no sound file is currently loaded
            }
 
            // Read the Header Info
            object result1 = null;

            try
            {
                string pathMLFunction = MLFunctionPath();
                string changenewpath = "cd " + pathMLFunction;
                string changeoldpath = "cd " + executepath;
                matlab.Execute(@changenewpath);
                matlab.Feval("MLWavInfo", 5, out result1, this.soundFileName);
  
                matlab.Execute(@changeoldpath);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error here" + ex.Message + "--" + ex.InnerException);
            }
            object[] res1 = result1 as object[];
            fs = (double)res1[0];
            fc = fs;   // default cutoff frequency will be same as the sampling frequency
            ch = (double)res1[1];
            totSamp = (double)res1[2];
            dur = (double)res1[3];
            bits = (double)res1[4];

            ///////////////////////////////////////////////////////////////////////////
            // Now Read the Wave File as time based data
            ///////////////////////////////////////////////////////////////////////////
            object result = null;
            try
            {
                string pathMLFunction = MLFunctionPath();
                string changenewpath = "cd " + pathMLFunction;
                string changeoldpath = "cd " + executepath;
                matlab.Execute(@changenewpath);
                matlab.Feval("MLReadWavFile", 2, out result, this.soundFileName);

                matlab.Execute(@changeoldpath);
                            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error here" + ex.Message + "--" + ex.InnerException);
            }
            object[] res = result as object[];
            soundData = (double[,])res[0];
            if (fs != (double)res[1])
            {
                System.Windows.Forms.MessageBox.Show("MLWavInfo and MLReadWavFile sample rate values are different");
            }

            // find macx and min values for the time plot
            for (double i = 0; i < totSamp; i++)
            {
                double temp1 = soundData[(int)i, 0];
                double temp2 = soundData[(int)i, 0];
                if (temp1 < y_time_min)
                    y_time_min = temp1;
                if (temp2 > y_time_max)
                    y_time_max = temp2; 
            }
            x_time_min = 0;
            x_time_max = dur;

            //////////////////////////////////////////////////////////////
            // Now determine the frequency and frequency response data
            //////////////////////////////////////////////////////////////
            object result2 = null;
             try
            {
                string pathMLFunction = MLFunctionPath();
                string changenewpath = "cd " + pathMLFunction;
                string changeoldpath = "cd " + executepath;
                matlab.Execute(@changenewpath);
                matlab.Feval("MLFreqResp", 2, out result2, this.soundFileName);

                matlab.Execute(@changeoldpath);
                            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error here" + ex.Message + "--" + ex.InnerException);
            }
            object[] res2 = result2 as object[];
            soundDataFResp = (double[,])res2[0];
            soundDataFreq = (double[,])res2[1];

            // find max and min values for the freq plots
            // Only plots the half sample due to symmetry
           for (double i = 0; i < totSamp / 2.0; i++)
           {
               double temp1 = soundDataFResp[(int)i, 0];
               double temp2 = soundDataFResp[(int)i, 0];
               double temp3 = soundDataFreq[0, (int)i ];
               double temp4 = soundDataFreq[0, (int)i ];
               if (temp1 < y_freq_min)
                   y_freq_min = temp1;
               if (temp2 > y_freq_max)
                   y_freq_max = temp2;
               if (temp3 < x_freq_min)
                   x_freq_min = temp3;
               if (temp4 > x_freq_max)
                   x_freq_max = temp4;
           }
        }
        public Series SeriesDataFTT(SoundFile soundFile)
        {
            double t, x, y;

            // Add a series with some data points.
            Series sinSeries = new Series();
            sinSeries.ChartType = SeriesChartType.Line;

            for (t = 0; t < (double)(soundFile.totSamp / 2.0); t++)
            {
                x = soundFile.soundDataFreq[0, (int)t];
                y = soundFile.soundDataFResp[(int)t, 0];
                sinSeries.Points.AddXY(x, y);
            }
            return sinSeries;
        }
        public Series SeriesDataTime(SoundFile soundFile)
        {
            double t, x, y;

            // Add a series with some data points.
            Series sinSeries = new Series();
            sinSeries.ChartType = SeriesChartType.Line;


            double time_incr = (soundFile.dur / soundFile.totSamp);
            for (t = 0; t < (double)(soundFile.totSamp); t++)
            {
                x = t * time_incr;
                y = soundFile.soundData[(int)t, 0];
                sinSeries.Points.AddXY(x, y);
            }

            return sinSeries;
        }

        public void playSoundFile()
        {
            object result = null;
            try
            {
                string pathMLFunction = MLFunctionPath();
                string changenewpath = "cd " + pathMLFunction;
                string changeoldpath = "cd " + executepath;
                matlab.Execute(@changenewpath);
                matlab.Feval("MLPlayWavFile", 0, out result, this.soundFileName);

                matlab.Execute(@changeoldpath);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error in playSoundFile()" + ex.Message + "--" + ex.InnerException);
            }
        }

        public static string MLWavDirPath()
        {
            string currpath = System.IO.Directory.GetCurrentDirectory();
            return (currpath.Substring(0, currpath.LastIndexOf("\\bin\\")) + "\\WAVSamples");
        }
        public void makeFiltered(string passtype, double newfc, string fname)
        {
            // pass -- 0 = low pass, 1 = high pass
            // newfs = new sample frequency
            // new fc = new cutoff
            // fname = new filename
            int pass = passtype.Equals("High") ? 1 : 0;

            object dummy;

            this.fc = newfc;  // record the cutoff frequency

            string wavIn = this.soundFileName;
            string pathWavDir = MLWavDirPath();
            string wavOut = pathWavDir + "\\" + fname + ".wav";

            string pathMLFunction = MLFunctionPath();

            string changenewpath = "cd " + pathMLFunction;
            string changeoldpath = "cd " + executepath;
            matlab.Execute(@changenewpath);
            matlab.Feval("MLFilter", 0, out dummy, wavIn, wavOut, pass, fc);

            matlab.Execute(@changeoldpath);
        }
    }
}
