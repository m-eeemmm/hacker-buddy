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
    public class EmotionObserver
    {
        private ISourceBlock<Bitmap> _Source;
        private ITargetBlock<Tuple<DateTime, string, float>> _Target;

        public EmotionObserver(ISourceBlock<Bitmap> source, ITargetBlock<Tuple<DateTime, string, float>> target)
        {
            _Source = source;
            _Target = target;
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

        public async void RunAsync()
        {
            while (await _Source.OutputAvailableAsync())
            {
                Bitmap data = await _Source.ReceiveAsync();
                var path = SavePhote(data);
                _Target.Post(MakeAGuess(path));
            }
        }

        private Tuple<DateTime, string, float> MakeAGuess(string imagePath)
        {
            // Create single instance of sample data from first line of dataset for model input
            //string imagePath = GetSnapshot().Result;

            MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
            {
                ImageSource = imagePath,
            };



            // Make a single prediction on the sample data and print results
            var predictionResult = MLModel1.Predict(sampleData);

            try
            {
                //if (File.Exists(imagePath))
                //    File.Delete(imagePath);
            }
            catch { }


            (int left, int top) = Console.GetCursorPosition();
            Console.SetCursorPosition(0, top);
            Console.Write("                                                                                                   ");
            Console.SetCursorPosition(0, top);
            List<Tuple<string, float>> predictions = new List<Tuple<string, float>>();
            for (int i = 0; i < predictionResult.ClassificationLabels.Length; i++)
            {
                predictions.Add(new Tuple<string, float>(predictionResult.ClassificationLabels[i], predictionResult.Score[i]));
            }
            var bestMatch = predictions.OrderByDescending(x => x.Item2).First();

            Console.Write($"{bestMatch.Item1}:{(int)(bestMatch.Item2 * 100)}%");

            //if (predictionResult.Score.Max() >= 0.6)
            //    Console.Write(predictionResult.Prediction);
            //else
            //    Console.Write($"UNDEFINED ({string.Join(",", predictions.OrderBy(x => x.Item2).Select(x => x.Item1))}");

            //Console.WriteLine($"ImageSource: {imagePath}\n");
            //Console.Write($"\nPredicted Label value: ");
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine($"{predictionResult.Prediction}");
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine($"Predicted Label scores:");
            //for (int i = 0; i < predictionResult.ClassificationLabels.Length; i++)
            //{
            //    if (predictionResult.Score[i] >= 0.3)
            //        Console.ForegroundColor = ConsoleColor.Cyan;
            //    if (predictionResult.Score[i] >= 0.5)
            //        Console.ForegroundColor = ConsoleColor.Green;
            //    if (predictionResult.Score[i] < 0.3)
            //        Console.ForegroundColor = ConsoleColor.White;
            //    Console.WriteLine($"\t[{predictionResult.ClassificationLabels[i]}|{Math.Round((decimal)predictionResult.Score[i], 1)}]");
            //}
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.WriteLine("\n\n");

            return Tuple.Create(DateTime.Now, bestMatch.Item1, bestMatch.Item2);
        }
    }
}
