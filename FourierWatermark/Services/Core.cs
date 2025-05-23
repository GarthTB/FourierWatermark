using FourierWatermark.Models;
using OpenCvSharp;

namespace FourierWatermark.Services;

internal static class Core
{
    /// <summary>
    /// Main processing function.
    /// </summary>
    internal static bool Process(
        Mat image, Mat watermark, Config config, string outputPath)
    => Lazy.Try($"process the image to be saved to {outputPath}", () =>
        {
            var factorSpectrum = watermark.ToFactorSpectrum(
                image.Size(), config.Opacity);
            var watermarked = Watermark(image, factorSpectrum);
            var rescaled = watermarked.Denormalize01(config);
            if (!rescaled.Save(outputPath, config))
                throw new Exception("Failed to save the image");
        });

    /// <summary>
    /// Watermark the image in the frequency domain with the given factor spectrum.
    /// </summary>
    private static Mat Watermark(Mat image, Mat factorSpectrum)
    {
        var factorChannels = factorSpectrum.Split();
        var inChannels = image.Normalize01().Split();
        var outChannels = Enumerable.Range(0, inChannels.Length)
            .Select(i => new Mat())
            .ToArray();

        if (factorChannels.Length > 1
            && factorChannels.Length != inChannels.Length)
            throw new ArgumentException("Watermark must be single-channel" +
                " or have the same number of channels as the image.");

        _ = Parallel.For(0, inChannels.Length, c =>
        {
            // 实、虚部准备
            Mat complex = new(),
                planeReal = inChannels[c],
                planeImag = Mat.Zeros(planeReal.Size(), MatType.CV_32F);
            Cv2.Merge([planeReal, planeImag], complex);

            // 正向 DFT
            Cv2.Dft(complex, complex, DftFlags.None);

            // 拆出实、虚部
            Mat[] dftPlanes = complex.Split();
            Mat realPart = dftPlanes[0];
            Mat imagPart = dftPlanes[1];

            // 计算幅度与相位，在幅度上乘以水印，然后复原实、虚部
            Mat mag = new(), angle = new();
            Cv2.CartToPolar(realPart, imagPart, mag, angle);
            Mat factor = factorChannels.Length == 1
                ? factorChannels[0]
                : factorChannels[c];
            Cv2.Multiply(mag, factor, mag);
            Cv2.PolarToCart(mag, angle, realPart, imagPart);

            // 合并实、虚部
            Mat complex2 = new();
            Cv2.Merge([realPart, imagPart], complex2);

            // 反向 IDFT
            Cv2.Dft(complex2, complex2, DftFlags.Inverse | DftFlags.Scale);

            // 拆出结果实部
            Mat[] invPlanes = complex2.Split();
            invPlanes[0].ConvertTo(outChannels[c], inChannels[c].Type());
        });

        // 合并通道并返回
        Mat result = new();
        Cv2.Merge(outChannels, result);
        return result;
    }
}
