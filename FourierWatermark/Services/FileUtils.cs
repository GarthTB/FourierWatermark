using FourierWatermark.Models;
using OpenCvSharp;
using Tomlyn;

namespace FourierWatermark.Services;

internal static class FileUtils
{
    /// <summary>
    /// Parse the config file and return a Config object.
    /// </summary>
    internal static Config GetConfig()
    {
        var configFile = Path.Combine(AppContext.BaseDirectory, "config.toml");
        if (!File.Exists(configFile))
            throw new FileNotFoundException("Config file not found.");

        var toml = File.ReadAllText(configFile);
        return new(Toml.ToModel(toml));
    }

    /// <summary>
    /// Get all inputs from the command line arguments or user input.
    /// </summary>
    internal static ((Mat, string)[] inputs, Mat watermark) GetInputs(
        string[] args, Config config)
    {
        (Mat, string)[] inputs = [];
        Mat watermark = new();

        if (args.Length != 2
            || !Lazy.Try("parse input arguments", () =>
            {
                inputs = GetImagesAndOutputPaths(args[0], config);
                watermark = Cv2.ImRead(args[1], ImreadModes.Unchanged);
            }))
        {
            inputs = GetImagesAndOutputPaths(config);
            watermark = GetWatermarkImage();
        }

        return (inputs, watermark);
    }

    /// <summary>
    /// Get the input images from the user and generate output paths.
    /// </summary>
    private static (Mat, string)[] GetImagesAndOutputPaths(Config config)
    {
        Console.WriteLine("Please enter the path of an image file or directory:");
        for ((Mat, string)[] inputs = []; ; Console.WriteLine("Please try again."))
            if (Lazy.Try("collect input images",
                () => inputs = GetImagesAndOutputPaths(
                    Console.ReadLine() ?? "",
                    config)))
                return inputs;
    }

    /// <summary>
    /// Get the watermark image from the user.
    /// </summary>
    private static Mat GetWatermarkImage()
    {
        Console.WriteLine("Please enter the path of a watermark image file:");
        for (Mat watermark = new(); ; Console.WriteLine("Please try again."))
            if (Lazy.Try("read watermark image",
                () => watermark = Cv2.ImRead(
                    Console.ReadLine() ?? "",
                    ImreadModes.Unchanged)))
                return watermark;
    }

    /// <summary>
    /// Collect all input images from the input path.
    /// </summary>
    private static (Mat, string)[] GetImagesAndOutputPaths(string path, Config config)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
            throw new FileNotFoundException("Input path not found.");

        var filePaths = Directory.Exists(path)
            ? Directory.GetFiles(path)
            : [path];

        var result = filePaths.AsParallel()
            .Select(filePath =>
            {
                Mat image = new();
                var outputPath = Lazy.Try($"read image from {filePath} ",
                    () => image = Cv2.ImRead(filePath, ImreadModes.Unchanged))
                    ? GetOutputPath(filePath, config)
                    : "";
                return (image, outputPath);
            })
            .Where(x => x.outputPath.Length > 0)
            .ToArray();

        return result.Length == 0
            ? throw new ArgumentException("No valid input images found.")
            : result;
    }

    /// <summary>
    /// Get the output path for the watermarked image.
    /// </summary>
    private static string GetOutputPath(string inputPath, Config config)
    {
        var dir = Path.GetDirectoryName(inputPath)
            ?? throw new ArgumentException("Invalid input path.");

        var fileName = Path.GetFileNameWithoutExtension(inputPath);
        var outputPath = Path.Combine(dir, $"{fileName}_wm.{config.OutputExtension}");
        for (int i = 2; File.Exists(outputPath); i++)
            outputPath = Path.Combine(dir, $"{fileName}_wm_{i}.{config.OutputExtension}");
        return outputPath;
    }
}
