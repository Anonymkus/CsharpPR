using System.Diagnostics;

namespace P323 {
    internal class Program{
        static void Main()
        {
            Console.WriteLine("Перед использованием программы убедитесь, что ваши файлы находятся в папке files");
            string path = "files";

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            string[] allFiles = Directory.GetFiles(path);

            Console.WriteLine($"Время выполнения синхронно - {Time(() => countChar(allFiles))} мс");
            Console.WriteLine($"Время выполнения асинхронно - {Time(() => countCharAsync(allFiles).Wait())} мс");
            Console.WriteLine($"Время выполнения многопоточно - {Time(() => countCharThread(allFiles))} мс");
        }

        static long Time(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        static void countChar(string[] allFiles)
        {
            var count = 0;

            foreach (var file in allFiles) {
                var content = File.ReadAllText(file);
                count += content.Length;
            }

            Console.WriteLine($"Количество символов в файлах - {count}");
        }

        static async Task countCharAsync(string[] allFiles)
        {
            var count = 0;

            foreach (var file in allFiles) {
                var content = await File.ReadAllTextAsync(file);
                count += content.Length;
            }

            Console.WriteLine($"Количество символов в файлах - {count}");
        }

        static void countCharThread(string[] allFiles)
        {
            var count = 0;
            var id = 0;
            object lockObj = new object();

            Thread[] threads = new Thread[allFiles.Length];

            foreach (var file in allFiles) {
                threads[id] = new Thread(() => {
                    var content = File.ReadAllText(file);
                    lock (lockObj) {
                        count += content.Length;
                    }
                });
                threads[id].Start();
                id+=1;
            }

            foreach (var thread in threads) {
                thread.Join();
            }

            Console.WriteLine($"Количество символов в файлах - {count}");
        }
    }
}
