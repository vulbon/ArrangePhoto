using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using photo.exif;

namespace ArrangePhoto
{
    enum Mode
    {
        Month = 0,
        Day = 1
    }

    class Program
    {
        private static Mode mode = Mode.Month;

        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            DirectoryInfo directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            FileInfo[] jpgFiles = directory.GetFiles("*.jpg", SearchOption.TopDirectoryOnly);

            int jpgCount = 0;
            int mp4Count = 0;

            foreach (FileInfo file in jpgFiles)
            {
                DateTime d = GetDatetimeByFileName(file);
                string targetPath = createDirectory(d);
                moveFileToNewFolder(file, targetPath);
                jpgCount++;
            }

            FileInfo[] mp4Files = directory.GetFiles("*.mp4", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in mp4Files)
            {
                DateTime d = GetDatetimeByFileName(file);
                string targetPath = createDirectory(d);
                moveFileToNewFolder(file, targetPath);
                mp4Count++;
            }
            sw.Stop();
            Console.WriteLine($"move image {jpgCount}, move mp4 {mp4Count}");
            Console.WriteLine($"use time {sw.ElapsedMilliseconds} ms");
            Console.ReadLine();
        }

        private static string createDirectory(DateTime fileDate)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), mode == Mode.Day ? fileDate.ToString("yyyyMMdd") : fileDate.ToString("yyyyMM"));
            Directory.CreateDirectory(path);
            return path;
        }

        private static DateTime GetDatetimeByFileName(FileInfo file)
        {
            string datetimeString = file.Name.Split('_')[1];
            return DateTime.ParseExact(datetimeString.Substring(0, 8), "yyyyMMdd", null);
        }

        private static DateTime GetDateTimeByExif(FileInfo file)
        {
            Parser parser = new Parser();
            var data = parser.Parse(file.FullName);
            data.ToList().ForEach(Console.WriteLine);

            return DateTime.ParseExact(data.FirstOrDefault().ToString(), "yyyyMMdd", null);
        }

        private DateTime GetDateTimeByModifyTime(FileInfo file)
        {
            return file.LastWriteTime;
        }

        private DateTime GetDateTimeByCreateTime(FileInfo file)
        {
            return file.CreationTime;
        }

        private static void moveFileToNewFolder(FileInfo file, string newPath)
        {
            file.MoveTo(Path.Combine(newPath, file.Name));
        }
    }
}
