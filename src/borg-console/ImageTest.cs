using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;

namespace borg_console
{
    static class Topographical
    {
        private const string basePath = @"c:\tempx";
        public static void Run()
        {
            var maxDepth = .100m;
            var workWidth = 5m;
            var workHeight = 5m;
            var lastWorkX = -99999m;
            var lastWorkY = -99999m;
            using (var img = Image.Load(System.IO.Path.Combine(basePath, "inhalegull.jpg")))
            {
                var widthPerPixel = workWidth / img.Width;
                var heightPerPixel = workHeight / img.Height;
                var lines = new System.Collections.Generic.List<string>();
                for (var x = 0; x < img.Width; x++)
                {
                    for (var y = 0; y < img.Height; y++)
                    {
                        var pixel = img[x, y];
                        var averageColor = (pixel.R + pixel.G + pixel.B) / 3;
                        var depth = ((255.0m - averageColor) / 255.0m) * maxDepth;

                        var workX = x * widthPerPixel;
                        var workY = y * heightPerPixel;

                        if (lastWorkX != workX || lastWorkY != workY)
                        {
                            var line = $"G1 X{workX} Y{workY} Z-{depth} F100";
                            lines.Add(line);
                        }

                        lastWorkX = workX;
                        lastWorkY = workY;
                    }
                }

                System.IO.File.WriteAllLines(System.IO.Path.Combine(basePath, "output.gcode"), lines);
            }
        }
    }
    static class ImageTest
    {
        private const string basePath = @"c:\tempx";
        public static void MainX(string[] args)
        {
            System.IO.Directory.CreateDirectory("output");
            using (var img = Image.Load(System.IO.Path.Combine(basePath, "inhalegull.jpg")))
            {
                // as generate returns a new IImage make sure we dispose of it
                using (Image<Rgba32> destRound = img.Clone(x => x.ConvertToAvatar(new Size(200, 200), 20)))
                {
                    destRound.Save(System.IO.Path.Combine(basePath, "output/fb.png"));
                }

                using (Image<Rgba32> destRound = img.Clone(x => x.ConvertToAvatar(new Size(200, 200), 100)))
                {
                    destRound.Save(System.IO.Path.Combine(basePath, "output/fb-round.png"));
                }

                using (Image<Rgba32> destRound = img.Clone(x => x.ConvertToAvatar(new Size(200, 200), 150)))
                {
                    destRound.Save(System.IO.Path.Combine(basePath, "output/fb-rounder.png"));
                }

                using (Image<Rgba32> destRound = img.CloneAndConvertToAvatarWithoutApply(new Size(200, 200), 150))
                {
                    destRound.Save(System.IO.Path.Combine(basePath, "output/fb-rounder-without-apply.png"));
                }


                // the original `img` object has not been altered at all.
            }
        }

        // 1. The short way: 
        // Implements a full image mutating pipeline operating on IImageProcessingContext<Rgba32>
        // We need the dimensions of the resized image to deduce 'IPathCollection' needed to build the corners,
        // so we implement an "inline" image processor by utilizing 'ImageExtensions.Apply()'
        private static IImageProcessingContext<Rgba32> ConvertToAvatar(this IImageProcessingContext<Rgba32> processingContext, Size size, float cornerRadius)
        {
            return processingContext.Resize(new ResizeOptions
            {
                Size = size,
                Mode = ResizeMode.Crop
            }).Apply(i => ApplyRoundedCorners(i, cornerRadius));
        }

        // 2. A more verbose way, avoiding 'Apply()':
        // First we create a resized clone of the image, then we draw the corners on that instance with Mutate().
        private static Image<Rgba32> CloneAndConvertToAvatarWithoutApply(this Image<Rgba32> image, Size size, float cornerRadius)
        {
            Image<Rgba32> result = image.Clone(
                ctx => ctx.Resize(
                    new ResizeOptions
                    {
                        Size = size,
                        Mode = ResizeMode.Crop
                    }));

            ApplyRoundedCorners(result, cornerRadius);
            return result;
        }

        // This method can be seen as an inline implementation of an `IImageProcessor`:
        // (The combination of `IImageOperations.Apply()` + this could be replaced with an `IImageProcessor`)
        public static void ApplyRoundedCorners(Image<Rgba32> img, float cornerRadius)
        {
            IPathCollection corners = BuildCorners(img.Width, img.Height, cornerRadius);

            var graphicOptions = new GraphicsOptions(true)
            {
                AlphaCompositionMode = PixelAlphaCompositionMode.DestOut // enforces that any part of this shape that has color is punched out of the background
            };
            // mutating in here as we already have a cloned original
            // use any color (not Transparent), so the corners will be clipped
            img.Mutate(x => x.Fill(graphicOptions, Rgba32.LimeGreen, corners));
        }

        public static IPathCollection BuildCorners(int imageWidth, int imageHeight, float cornerRadius)
        {
            // first create a square
            var rect = new RectangularPolygon(-0.5f, -0.5f, cornerRadius, cornerRadius);

            // then cut out of the square a circle so we are left with a corner
            IPath cornerTopLeft = rect.Clip(new EllipsePolygon(cornerRadius - 0.5f, cornerRadius - 0.5f, cornerRadius));

            // corner is now a corner shape positions top left
            //lets make 3 more positioned correctly, we can do that by translating the orgional artound the center of the image

            float rightPos = imageWidth - cornerTopLeft.Bounds.Width + 1;
            float bottomPos = imageHeight - cornerTopLeft.Bounds.Height + 1;

            // move it across the width of the image - the width of the shape
            IPath cornerTopRight = cornerTopLeft.RotateDegree(90).Translate(rightPos, 0);
            IPath cornerBottomLeft = cornerTopLeft.RotateDegree(-90).Translate(0, bottomPos);
            IPath cornerBottomRight = cornerTopLeft.RotateDegree(180).Translate(rightPos, bottomPos);

            return new PathCollection(cornerTopLeft, cornerBottomLeft, cornerTopRight, cornerBottomRight);
        }
    }
}
