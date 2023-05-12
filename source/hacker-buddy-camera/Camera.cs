using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;

namespace hacker_buddy_camera
{
    public class Camera
    {
        public Bitmap TakePhoto()
        {
            Bitmap bitmap;

            using var capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
            if (!capture.IsOpened())
                return null;

            capture.FrameWidth = 1920;
            capture.FrameHeight = 1280;
            capture.AutoFocus = true;

            var mat = new Mat();

            if (capture.IsOpened())
            {
                capture.Read(mat);
                bitmap = BitmapConverter.ToBitmap(mat);
                return bitmap;
            }
            return null;
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