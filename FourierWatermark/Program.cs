using FourierWatermark.Services;

namespace FourierWatermark;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Fourier Watermark!");
        Console.WriteLine("Version: 1.1.0 (20250425)");
        Console.WriteLine("Author: GarthTB <g-art-h@outlook.com>");
        Console.WriteLine("Repo: https://github.com/GarthTB/FourierWatermark");

        _ = Lazy.Try("watermark", () =>
            {
                Console.WriteLine("Loading configuration...");
                var config = FileUtils.GetConfig();
                Console.WriteLine("Configuration loaded.");

                (var imagesAndPaths, var watermark) =
                    FileUtils.GetInputs(args, config);
                Console.WriteLine($"Processing {imagesAndPaths.Length} images...");

                int successes = 0, failures = 0;
                foreach (var (image, outputPath) in imagesAndPaths)
                {
                    if (Core.Process(image, watermark, config, outputPath))
                        successes++;
                    else failures++;
                    Console.Write(
                        $"\r{successes} successes, {failures} failures (skipped).");
                }
                Console.WriteLine("\nDone.");
            });

        Console.WriteLine("Press any key to exit...");
        _ = Console.ReadKey();
    }
}
