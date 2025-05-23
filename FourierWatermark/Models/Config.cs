using Tomlyn.Model;

namespace FourierWatermark.Models;

/// <summary>
/// Configuration for the watermarking process.
/// </summary>
internal class Config
{
    /// <summary>
    /// Parse the configuration from a TOML table.
    /// </summary>
    internal Config(TomlTable tomlTable)
    {
        Opacity = (double)tomlTable["opacity"];
        OutputBitDepth = Convert.ToInt32(tomlTable["output_bit_depth"]);
        OutputExtension = (string)tomlTable["output_extension"];
        JpegQuality = Convert.ToInt32(tomlTable["jpeg_quality"]);
        PngCompression = Convert.ToInt32(tomlTable["png_compression"]);
        TiffCompression = Convert.ToInt32(tomlTable["tiff_compression"]);
        WebPQuality = Convert.ToInt32(tomlTable["webp_quality"]);
    }

    /// <summary>
    /// Opacity of the watermark.
    /// </summary>
    internal double Opacity { get; private set; }

    /// <summary>
    /// Only for png and tiff, 8 or 16
    /// </summary>
    internal int OutputBitDepth { get; private set; }

    /// <summary>
    /// jpg, png, tif, webp, etc.
    /// </summary>
    internal string OutputExtension { get; private set; } = "";

    /// <summary>
    /// Only for jpg
    /// </summary>
    internal int JpegQuality { get; private set; }

    /// <summary>
    /// Only for png, from 0 to 9
    /// </summary>
    internal int PngCompression { get; private set; }

    /// <summary>
    /// Only for tiff, 1 for no compression, 5 for LZW,
    /// 8 for ZIP, 32773 for PackBits
    /// </summary>
    internal int TiffCompression { get; private set; }

    /// <summary>
    /// Only for webp, lossless when above 100
    /// </summary>
    internal int WebPQuality { get; private set; }
}
