using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Threading.Tasks.Dataflow;

namespace hacker_buddy_camera
{
    public class Camera
    {
        private ITargetBlock<Bitmap> _Source;
        private bool photoTaking = false;
        private bool isTakingPhoto = false;
        Window window;
        Window show;

        public Camera(ITargetBlock<Bitmap> source)
        {
            _Source = source;
        }

        public void TakePhotoAsync(int sleepTime = 1000, int count = 10)
        {

            Task.Run(() =>
            {
                if (isTakingPhoto)
                    return;
                isTakingPhoto = true;
                photoTaking = true;

                using var capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
                if (!capture.IsOpened())
                    return;

                capture.FrameWidth = 400;
                capture.FrameHeight = 800;
                capture.AutoFocus = true;

                var mat = new Mat();

                int i = 0;
                while (photoTaking && i++ < count)
                {
                    capture.Read(mat);
                    if (show == null)
                    {
                        show = new Window("Face Detection", WindowFlags.FullScreen);
                        Thread.Sleep(100);
                    }
                    show.Move(150, 0);
                    var faceRecognition = DetectFace(mat);
                    //Mat clippy = Mat.FromImageData(File.ReadAllBytes(@"C:\Repos\hacker-buddy\source\hacker-buddy-app\Data\Img\sad.png"),
                    //    ImreadModes.Color);
                    //m
                    //Mat merge = mat.Add(Mat.FromImageData(File.ReadAllBytes(@"C:\Repos\hacker-buddy\source\hacker-buddy-app\Data\Img\sad.png"), 
                    //    ImreadModes.Color ));
                    show.ShowImage(faceRecognition.Item1);
                    //show.ShowImage(merge);


                    if (faceRecognition.Item2.Empty())
                        break;
                    if (window == null) window = new Window();

                    window.Move(0, 0);
                    window.Resize(100, 100);
                    window.ShowImage(faceRecognition.Item2.Clone());
                    _Source.Post(BitmapConverter.ToBitmap(faceRecognition.Item2));

                    Cv2.WaitKey(sleepTime);

                    
                }
                isTakingPhoto = false;
                photoTaking = false;
                Cv2.DestroyAllWindows();
            });
        }

        private (Mat, Mat) DetectFace(Mat mat)
        {
            Mat result;
            Mat matFace = mat.EmptyClone();

            if (mat.Empty())
                return (mat, mat);

            using (var src = mat.Clone())
            using (var gray = new Mat())
            {
                result = src.Clone();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
                
                // Detect faces
                using var cascade = new CascadeClassifier(@".\Data\Text\haarcascade_frontalface_default.xml");
                Rect[] faces = cascade.DetectMultiScale(
                    gray, 1.1, 3, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(48, 48));

                // Render all detected faces
                foreach (Rect face in faces.OrderBy(x => x.Height))
                {
                    var center = new OpenCvSharp.Point
                    {
                        X = (int)(face.X + face.Width * 0.5),
                        Y = (int)(face.Y + face.Height * 0.5)
                    };
                    var axes = new OpenCvSharp.Size
                    {
                        Width = (int)(face.Width * 0.5),
                        Height = (int)(face.Height * 0.5)
                    };
                    Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(0, 255, 0), 4);

                    matFace = new Mat(gray, new Rect(face.X, face.Y, face.Width, face.Height));
                    matFace = matFace.Resize(new OpenCvSharp.Size(100, 100), 0, 0, InterpolationFlags.LinearExact);
                }
            }
            return (result, matFace);
        }

        public void Stop()
        {
            photoTaking = false;
        }
    }
}