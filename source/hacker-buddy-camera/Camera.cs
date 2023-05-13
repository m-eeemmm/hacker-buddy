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
        public static string CurrentMoodDescription;
        public static string CurrentMood;
        private readonly string _RelativePath = @"..\..\..\..\hacker-buddy-app\Data\Img";

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

                capture.FrameWidth = 1920;
                capture.FrameHeight = 1280;
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
                    show.Move(250, 0);
                    var faceRecognition = DetectFace(mat);
                    var largeFace = faceRecognition.Item1;
                    if (!string.IsNullOrWhiteSpace(CurrentMoodDescription) && !faceRecognition.Item2.Empty())
                    {
                        var clippyPaht = Path.Combine(_RelativePath, $"{CurrentMood}.png");

                        Mat clippy = new Mat(clippyPaht);
                        //Mat clippy0 = clippy.Resize(faceRecognition.Item1.Size(), 0, 0, InterpolationFlags.Area);
                        //Mat clippy1 = clippy.Resize(new OpenCvSharp.Size(150, 350), 0, 0, InterpolationFlags.Area);
                        
                        var x_offset = 30;
                        var y_offset = 175;
                        var x_end = x_offset + clippy.Width;
                        var y_end = y_offset + clippy.Height;

                        largeFace[new Rect(x_offset, y_offset, clippy.Width, clippy.Height)] = clippy;

                        var text = Cv2.GetTextSize(CurrentMoodDescription, HersheyFonts.HersheyScriptSimplex, 1, 1, out var bottomLine);
                        faceRecognition.Item3.X -= text.Width / 2;
                        faceRecognition.Item3.Y += text.Height + bottomLine * 2;
                        largeFace.PutText(CurrentMoodDescription, faceRecognition.Item3, HersheyFonts.HersheyScriptSimplex, 0.9, new Scalar(58, 52, 235), 2);
                    }

                    //    ImreadModes.Color);
                    //m
                    //Mat merge = mat.Add(Mat.FromImageData(File.ReadAllBytes(@"C:\Repos\hacker-buddy\source\hacker-buddy-app\Data\Img\sad.png"), 
                    //    ImreadModes.Color ));
                    show.ShowImage(largeFace);
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

        private (Mat, Mat, OpenCvSharp.Point) DetectFace(Mat mat)
        {
            Mat result;
            Mat matFace = mat.EmptyClone();
            OpenCvSharp.Point faceBottom = new OpenCvSharp.Point(0, 0);

            if (mat.Empty())
                return (mat, mat, faceBottom);

            using (var src = mat.Clone())
            using (var gray = new Mat())
            {
                result = src.Clone();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                // Detect faces
                using var cascade = new CascadeClassifier(@".\Data\Text\haarcascade_frontalface_default.xml");
                Rect[] faces = cascade.DetectMultiScale(
                    gray, 1.08, 2, HaarDetectionTypes.FindBiggestObject, new OpenCvSharp.Size(30, 30));

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
                        Height = (int)(face.Height * 0.65)
                    };
                    Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(0, 255, 0), 4);

                    matFace = new Mat(gray, new Rect(face.X, face.Y, face.Width, face.Height));
                    matFace = matFace.Resize(new OpenCvSharp.Size(100, 100), 0, 0, InterpolationFlags.LinearExact);
                    faceBottom = new OpenCvSharp.Point(center.X, face.Y + face.Height);
                }
            }
            return (result, matFace, faceBottom);
        }

        public static void SetCurrentMood(DateTime item1, string item2, float item3)
        {
            CurrentMoodDescription = $"{item1:T} {item2} ({(int)(item3 * 100)}%)";
            CurrentMood = item2;
        }

        public void Stop()
        {
            photoTaking = false;
        }
    }
}