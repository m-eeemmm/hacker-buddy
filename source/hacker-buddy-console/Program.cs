﻿
// This file was auto-generated by ML.NET Model Builder. 

using hacker_buddy_camera;
using hacker_buddy_console;
using Hacker_buddy_ml_model;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace hacker_buddy_console
{
    class Program
    {
        private const bool WithCam = true;
        private static Camera _Cam;
        private static int _ReceivedAlarms;
        private static ConcurrentQueue<Tuple<DateTime, string, float>> _List;

        static void Main(string[] args)
        {
            Console.WriteLine("Using model to make single prediction -- Comparing actual Label with predicted Label from sample data...");
            Console.WriteLine("Press 'q' to quit\n\n");

            ConsoleKey result = ConsoleKey.Spacebar;
            BufferBlock<Bitmap> buffer = new BufferBlock<Bitmap>();

            BufferBlock<Tuple<DateTime, string, float>> results = new BufferBlock<Tuple<DateTime, string, float>>();
            _Cam = new Camera(buffer);
            EmotionObserver obs = new EmotionObserver(buffer, results);
            Controller controller = new Controller();
            controller.HackerSleepingEvent += Controller_HackerSleepingEvent;
            controller.HackerAwakenEvent += Controller_HackerAwakenEvent;
            Task.Run(() => controller.StartLogging(WithCam));
            obs.RunAsync();
            RunEscalationObserver(results);

            controller.SetupKeyboardHooks();
            do
            {
                Console.WriteLine("Press any key to make a guess");
                //Thread.Sleep(5000);

                // _Cam.TakePhotoAsync();

                result = Console.ReadKey().Key;
            } while (result != ConsoleKey.Q);
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }

        private static void Controller_HackerAwakenEvent(object? sender, EventArgs e)
        {
            _Cam.Stop();
            _ReceivedAlarms = 0;
            _List = new ConcurrentQueue<Tuple<DateTime, string, float>>();
        }

        private static void Controller_HackerSleepingEvent(object? sender, EventArgs e)
        {
            if (WithCam)
                _Cam.TakePhotoAsync(100, int.MaxValue);
        }

        static bool _ObserverOn = false;
        static async void RunEscalationObserver(ISourceBlock<Tuple<DateTime, string, float>> source)
        {

            Task.Run(() =>
            {
                if (_ObserverOn)
                    return;
                _ObserverOn = true;

                Tuple<DateTime, string, float> data;
                _List = new ConcurrentQueue<Tuple<DateTime, string, float>>();
                int maxBuffer = 9;
                while (source.OutputAvailableAsync().Result)
                {
                    try
                    {


                        //Console.WriteLine("Alarms: "+_ReceivedAlarms++);
                        data = source.ReceiveAsync().Result;
                        _List.Enqueue(data);
                        if (_List.Count > maxBuffer)
                        {
                            _List.TryDequeue(out _);
                        }

                        var tmpList = _List.ToList();
                        if (tmpList.Count > 5 && tmpList[0].Item1.Subtract(tmpList[tmpList.Count - 1].Item1) < TimeSpan.FromSeconds(15))
                        {
                            var topOccurence = tmpList.GroupBy(x => x.Item2).OrderByDescending(x => x.Count()).First();
                            if (topOccurence.Count() > 4)
                            {
                                var top = topOccurence.First();
                                ShowClippy(top.Item2);
                                Camera.SetCurrentMood(top.Item1, top.Item2, top.Item3);
                                _List = new ConcurrentQueue<Tuple<DateTime, string, float>>();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                _ObserverOn = false;
            });
        }

        private static void ShowClippy(string emotion)
        {
            Task.Run(() =>
            {
                var path = Path.GetFullPath(@"..\..\..\..\hacker-buddy-app\bin\Debug\net6.0-windows\hacker-buddy-app.exe");
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = path;
                processStartInfo.ArgumentList.Add(emotion);
                processStartInfo.WorkingDirectory = Path.GetDirectoryName(path);
                Process.Start(processStartInfo);
            });
        }
    }
}
