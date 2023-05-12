using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Threading.Tasks.Dataflow;

namespace hacker_buddy_camera
{
    public class Camera
    {
        private ITargetBlock<Bitmap> _Source;

        public Camera(ITargetBlock<Bitmap> source)
        {
            _Source = source;
        }

        public Bitmap TakePhoto()
        {
            //Bitmap bitmap;

            //using var capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
            //if (!capture.IsOpened())
            //    return null;

            ////capture.FrameWidth = 1920;
            ////capture.FrameHeight = 1280;
            ////capture.AutoFocus = true;

            //var mat = new Mat();

            //if (capture.IsOpened())
            //{
            //    capture.Read(mat);
            //    bitmap = BitmapConverter.ToBitmap(mat);
            //    return bitmap;
            //}
            return null;
        }

        public void TakePhotoAsync(int sleepTime = 10)
        {
            Task.Run(() =>
            {
                var window = new Window();
                using var capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
                if (!capture.IsOpened())
                    return;

                capture.FrameWidth = 200;
                capture.FrameHeight = 200;
                capture.AutoFocus = true;

                var mat = new Mat();

                while (true)
                {
                    capture.Read(mat);
                    mat = DetectFace(mat);

                    if (mat.Empty())
                        break;

                    window.ShowImage(mat);
                    _Source.Post(BitmapConverter.ToBitmap(mat));
                    int c = Cv2.WaitKey(sleepTime);
                    if (c >= 0)
                    {
                        break;
                    }
                }
            });
        }

        private Mat DetectFace(Mat mat)
        {

            Mat result;

            using (var src = mat.Clone())
            using (var gray = new Mat())
            {
                result = src.Clone();
                Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

                // Detect faces
                using var cascade = new CascadeClassifier(@".\Data\Text\haarcascade_frontalface_default.xml");
                Rect[] faces = cascade.DetectMultiScale(
                    gray, 1.08, 2, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30));

                // Render all detected faces
                foreach (Rect face in faces)
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
                    Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(255, 0, 255), 4);
                    Mat matFace = new Mat(src, new Rect(face.X, face.Y, face.Width, face.Height));
                    return matFace;
                }
            }
            return result;
        }

        public string SavePhote(Bitmap bitmap, string path = null)
        {
            if (path == null)
            {
                string folder = @".\samplepics";
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                path = Path.Combine(folder, $"{Guid.NewGuid()}.jpg");
            }

            bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            return path;
        }
    }
}