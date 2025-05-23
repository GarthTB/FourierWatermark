using FourierWatermark.Models;
using OpenCvSharp;

namespace FourierWatermark.Services;

internal static class ImageUtils
{
    /// <summary>
    /// Converts the watermark image to a factor spectrum.
    /// </summary>
    internal static Mat ToFactorSpectrum(
        this Mat image, Size size, double opacity)
    {
        Mat factorSpectrum = new();
        image.ConvertTo(factorSpectrum, MatType.CV_32FC(image.Channels()));
        return factorSpectrum.Resize(size)
            .Normalize(1 - opacity, 1, NormTypes.MinMax);
    }

    /// <summary>
    /// Rescales an image to the range [0,1].
    /// </summary>
    internal static Mat Normalize01(this Mat image)
    {
        if (image.Type() == MatType.CV_32F)
            return image;

        Mat normalized = new(image.Size(), MatType.CV_32F);
        switch (image.Depth())
        {
            case 0: image.ConvertTo(normalized, MatType.CV_32F, 1.0 / 255.0); break;
            case 2: image.ConvertTo(normalized, MatType.CV_32F, 1.0 / 65535.0); break;
            default: throw new ArgumentException("Invalid MatType.");
        }
        return normalized;
    }

    /// <summary>
    /// Rescales a 32F [0,1] image to the specified MatType.
    /// </summary>
    internal static Mat Denormalize01(this Mat image, Config config)
    {
        Mat denormalized = new();
        if (config.OutputExtension is "tif" or "tiff" or "png")
            if (config.OutputBitDepth == 16)
                image.ConvertTo(denormalized, MatType.CV_16U, 65535.0);
            else if (config.OutputBitDepth == 8)
                image.ConvertTo(denormalized, MatType.CV_8U, 255.0);
            else throw new ArgumentException("Invalid TiffBitDepth.");

        else if (config.OutputExtension is "jpg" or "jpeg" or "webp")
            image.ConvertTo(denormalized, MatType.CV_8U, 255.0);
        else throw new ArgumentException("Invalid MatType.");

        return denormalized;
    }

    /// <summary>
    /// Saves the image to the specified path with the specified encoding parameters.
    /// </summary>
    internal static bool Save(this Mat image, string path, Config config)
    => config.OutputExtension switch
    {
        "jpg" or "jpeg" => image.SaveImage(path,
            new ImageEncodingParam(
                ImwriteFlags.JpegQuality,
                config.JpegQuality)),
        "png" => image.SaveImage(path,
            new ImageEncodingParam(
                ImwriteFlags.PngCompression,
                config.PngCompression)),
        "tif" or "tiff" => image.SaveImage(path,
            new ImageEncodingParam(
                ImwriteFlags.TiffCompression,
                config.TiffCompression)),
        "webp" => image.SaveImage(path,
            new ImageEncodingParam(
                ImwriteFlags.WebPQuality,
                config.WebPQuality)),
        _ => throw new ArgumentException("Invalid output extension.")
    };
}
