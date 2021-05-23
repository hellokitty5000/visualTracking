using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dontbreakeyeball
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        VideoCapture capture;
        Mat frame;
        Bitmap image;
        private Thread camera;
        bool isCameraRunning = false;
        const int SLICES_COUNT = 10;
        const int THRESHOLD = 40;

        private void CaptureCamera()
        {
            camera = new Thread(new ThreadStart(CaptureCameraCallback));
            camera.Start();
        }
        private void calcMatch(Mat image1, Mat image2)
        {
            // var difference = Cv2.Absdiff(image1, image2);
        }

        private void CaptureCameraCallback()
        {

            frame = new Mat();
            capture = new VideoCapture();
            capture.Open(0);

            if (capture.IsOpened())
            {
                Mat oldframe = new Mat();
                capture.Read(oldframe);
                Cv2.CvtColor(oldframe, oldframe, ColorConversionCodes.RGB2GRAY);
                while (isCameraRunning)
                {
                    //use www.robindavid.fr/opencv-tutorial/motion-detection-with-opencv.html
                    capture.Read(frame);
                    //Cv2.Threshold(); maybe check out the contour approximation if something goes wrong
                    //Mat hierarchy = new Mat();
                    //Cv2.FindContours(frame, out Mat[] contours, hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone);
                    //Mat[] regions = new Mat[8];
                    //for (int i = 0; i < 8; i++)
                    //{
                    //    hierarchy.SubMat(0, hierarchy.Height, i * hierarchy.Width / 8, (i + 1) * hierarchy.Width / 8);
                    //}

                    Mat grayscale = new Mat();
                    Cv2.CvtColor(frame, grayscale, ColorConversionCodes.RGB2GRAY);

                    Mat absDiff = new Mat();
                    
                    Cv2.Absdiff(oldframe, grayscale, absDiff);
                    absDiff.Blur(new OpenCvSharp.Size(30, 30));

                    Mat threshold = new Mat();
                    Cv2.Threshold(absDiff, threshold, 40, 255, ThresholdTypes.Binary);
                    //find the one with the most difference
                    //turn to that mat
                    //only turn if the 


                    //double sum = Cv2.Sum(threshold).Val0;


                    Mat colorOutput = new Mat();



                    Cv2.BitwiseAnd(frame, threshold.CvtColor(ColorConversionCodes.GRAY2BGR), colorOutput);

                    //Cv2.PutText(colorOutput, $"Sum: {sum}", new OpenCvSharp.Point(1, 200), HersheyFonts.HersheyComplex, 2, Scalar.WhiteSmoke);
                    /*use below functions
                    // Cv2.Circle()
                    //cv2.threshold()
                    Cv2.FindContours()
                    Cv2.DrawContours()
                    To Do List:
                    figure out how to implement the below functions
                    note that the video capture is now on the webcam
                    draw the black and white value of the image from 
                    frame functions like threshold, erode, and dilate 
                    some of those functions are arbitrary. dw.
                    you will use rotated bouding boxes to complete
                    the project. good luck future aidan you will need it
                    this stuff is tough.
                    */

                    // frame.FindContours(out var points)
                    // frame.DrawContours();

                    //define region of interest as a slice
                    //

                    Scalar[] sum = new Scalar[SLICES_COUNT];
                    Mat[] mats = new Mat[SLICES_COUNT];
                    Mat biggestChange = new Mat();
                    int position = 0;

                    //threshoolding each mat to THRESHOLD because that is the smallest value with nearly no static
                    Cv2.Blur(threshold, threshold, new OpenCvSharp.Size(30, 30));
                    for (int i = 0; i < mats.Length; i++)
                    {
                        //creates SLICES_COUNT mats evently divided along the width for horizontal motion
                        mats[i] = new Mat(threshold,
                            new Rect(threshold.Width * i / SLICES_COUNT, 0, threshold.Width / SLICES_COUNT, threshold.Height));

                        sum[i] = Cv2.Sum(mats[i]).Val0;
                        //iteratively find the slice with the largest change for exclusive display later
                        var currentSum = Cv2.Sum(mats[i]).Val0;
                        var biggestSum = Cv2.Sum(biggestChange).Val0;
                        if (currentSum > biggestSum)
                        {
                            //store biggest change sum
                            biggestChange = mats[i];
                            position = i;
                        }

                    }


                    Cv2.PutText(colorOutput, $" slice {position}", new OpenCvSharp.Point(1, 100), HersheyFonts.HersheyComplex, 2, Scalar.WhiteSmoke);


                    Cv2.PutText(colorOutput, $" {(int) Math.Log(sum[0].Val0 + 1)}, {(int) Math.Log(sum[1].Val0 + 1)}", new OpenCvSharp.Point(1, 150), HersheyFonts.HersheyComplex, 1, Scalar.WhiteSmoke);
                    Cv2.PutText(colorOutput, $" {(int) Math.Log(sum[2].Val0 + 1)}, {(int) Math.Log(sum[3].Val0 + 1)}", new OpenCvSharp.Point(1, 200), HersheyFonts.HersheyComplex, 1, Scalar.WhiteSmoke);
                    Cv2.PutText(colorOutput, $" {(int) Math.Log(sum[4].Val0 + 1)}, {(int) Math.Log(sum[5].Val0 + 1)}", new OpenCvSharp.Point(1, 250), HersheyFonts.HersheyComplex, 1, Scalar.WhiteSmoke);
                    Cv2.PutText(colorOutput, $" {(int) Math.Log(sum[6].Val0 + 1)}, {(int) Math.Log(sum[7].Val0 + 1)}", new OpenCvSharp.Point(1, 300), HersheyFonts.HersheyComplex, 1, Scalar.WhiteSmoke);
                    Cv2.PutText(colorOutput, $" {(int) Math.Log(sum[8].Val0 + 1)}, {(int) Math.Log(sum[9].Val0 + 1)}", new OpenCvSharp.Point(1, 350), HersheyFonts.HersheyComplex, 1, Scalar.WhiteSmoke);


                    oldframe = grayscale.Clone();
                    //dont worry abt it VV
                    image = BitmapConverter.ToBitmap(colorOutput);
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = image;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.Equals("Start"))
            {
                CaptureCamera();
                button1.Text = "Stop";
                isCameraRunning = true;
            }
            else
            {
                capture.Release();
                button1.Text = "Start";
                isCameraRunning = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}