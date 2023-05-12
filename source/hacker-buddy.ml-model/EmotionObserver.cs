using Hacker_buddy_ml_model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Hacker_buddy_ml_model
{
    internal class EmotionObserver
    {
        private ISourceBlock<Bitmap> _Source;

        public EmotionObserver(ISourceBlock<Bitmap> source)
        {
            _Source = source;
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

        internal async void RunAsync()
        {
            while (await _Source.OutputAvailableAsync())
            {
                Bitmap data = await _Source.ReceiveAsync();
                var path = SavePhote(data);
                MakeAGuess(path);
            }
        }


        static void MakeAGuess(string imagePath)
        {
            // Create single instance of sample data from first line of dataset for model input
            //string imagePath = GetSnapshot().Result;

            MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
            {
                ImageSource = imagePath,
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = MLModel1.Predict(sampleData);

            Console.WriteLine($"ImageSource: {imagePath}\n");
            Console.Write($"\nPredicted Label value: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{predictionResult.Prediction}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Predicted Label scores:");
            for (int i = 0; i < predictionResult.ClassificationLabels.Length; i++)
            {
                if (predictionResult.Score[i] >= 0.3)
                    Console.ForegroundColor = ConsoleColor.Cyan;
                if (predictionResult.Score[i] >= 0.5)
                    Console.ForegroundColor = ConsoleColor.Green;
                if (predictionResult.Score[i] < 0.3)
                    Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\t[{predictionResult.ClassificationLabels[i]}|{Math.Round((decimal)predictionResult.Score[i], 1)}]");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n\n");
        }
    }
}
