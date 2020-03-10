using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VRS_POC.Services
{
    public class FrameInfo
    {
        public int Height;
        public int Width;
    }
    public class CameraImageService
    {
        VideoCapture capture;
        Mat frame;
        Bitmap image;
        private Thread camera;
        int _camNumber;
        public bool isCameraRunning;
        public event EventHandler<FrameInfo> NewFrameAvailable;
        MemoryStream _memoryStream;
        int _framePause = 160;
        CascadeClassifier faceClassifier;
        bool detectFaces = true;
        int numberOfFacesDetected = 0;
        public FrameInfo LastFrame { get; set; }

        public CameraImageService()
        {
            camera = null;
            _memoryStream = new MemoryStream();
            faceClassifier = new CascadeClassifier("haarcascade_frontalface_alt2.xml");
        }

        public void StartCapture(int camNumber, int framePause = 200)
        {
            if (camera != null)
            {
                isCameraRunning = false;
                camera.Join();
            }
            _framePause = framePause;
            isCameraRunning = true;
            camera = new Thread(new ThreadStart(CaptureCameraCallback));
            _camNumber = camNumber;
            camera.Start();
        }

        public byte[] GetImage()
        {
            return _memoryStream.ToArray();
        }

        public Mat GetFrame()
        {
            return frame;
        }

        public void StopCapture()
        {
            if (camera == null)
            {
                return;
            }
            isCameraRunning = false;
        }

        private void CaptureCameraCallback()
        {
            frame = new Mat();
            capture = new VideoCapture(_camNumber);
            capture.FrameHeight = 240;
            capture.FrameWidth = 320;
            capture.Open(0);


            if (capture.IsOpened())
            {
                while (isCameraRunning)
                {
                    capture.Read(frame);
                    _memoryStream.Seek(0, SeekOrigin.Begin);
                    if (detectFaces)
                    {
                        var frame2 = frame.Clone();
                        var faces = GetFaces(frame);
                        foreach (var face in faces)
                        {
                            frame2.Rectangle(face, Scalar.Green);
                        }
                        numberOfFacesDetected = faces.Length;
                        frame2.WriteToStream(_memoryStream, ".png");
                    } else
                    {
                        frame.WriteToStream(_memoryStream, ".png");
                    }
                    NewFrameAvailable?.Invoke(this, LastFrame);
                    Thread.Sleep(_framePause);
                }
                capture.Dispose();
                camera = null;
            }
        }

        public Rect[] GetFaces(Mat cameraFrame)
        {
            return faceClassifier.DetectMultiScale(
                    cameraFrame, 1.08, 2, HaarDetectionType.ScaleImage, new OpenCvSharp.Size(30, 30));
        }

        public int GetNumberOfFaces()
        {
            return numberOfFacesDetected;
        }

        public byte[] CaptureImage()
        {
            _memoryStream.Seek(0, SeekOrigin.Begin);
            frame.WriteToStream(_memoryStream);
            return _memoryStream.ToArray();
        }
    }
}
