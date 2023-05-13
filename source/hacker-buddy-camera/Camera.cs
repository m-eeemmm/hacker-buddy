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

        public Camera(ITargetBlock<Bitmap> source)
        {
            _Source = source;
        }

        public void TakePhotoAsync(int sleepTime = 1000, int count = 10)
        {
            if (isTakingPhoto)
                return;
            isTakingPhoto = true;
            photoTaking = true;
            Task.Run(() =>
            {
                using var capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
                if (!capture.IsOpened())
                    return;

                //capture.FrameWidth = 1920;
                //capture.FrameHeight = 1048;
                capture.AutoFocus = true;

                var mat = new Mat();

                int i = 0;
                while (photoTaking && i++ < count)
                {
                    capture.Read(mat);
                    mat = DetectFace(mat);

                    if (mat.Empty())
                        break;
                    if (window == null) window = new Window();
                    window.ShowImage(mat.Clone());
                    _Source.Post(BitmapConverter.ToBitmap(mat));

                    Cv2.WaitKey(sleepTime);
                }
            }).ContinueWith((t) =>
            {
                isTakingPhoto = false;
                photoTaking = false;
                window?.Close();
            });
        }

        private Mat DetectFace(Mat mat)
        {

            Mat result;

            if (mat.Empty())
                return mat;
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
                foreach (Rect face in faces.OrderByDescending(x => x.Width))
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
                    Mat matFace = new Mat(gray, new Rect(face.X, face.Y, face.Width, face.Height));
                    //matFace = matFace.Resize(new OpenCvSharp.Size(48, 48),0,0, InterpolationFlags.Area);
                    return matFace;
                }
            }
            return result;
        }

        public void Stop()
        {
            photoTaking = false;
        }
    }
}