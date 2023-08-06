using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AdvancedSharpAdbClient;
using AForge.Imaging;
using Emgu.CV;
using Emgu.CV.Structure;
using Tesseract;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

public class TesseractManager
{
    public string _tessdataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
    
    public Point FindWordOnImage(Bitmap bitmap, string wordToFind)
    {
        using (var engine = new TesseractEngine(@_tessdataDirectory, "rus", EngineMode.Default))
        {
            using (var page = engine.Process(PixConverter.ToPix(bitmap)))
            {
                Bitmap visualImage = new Bitmap(bitmap);

                using (var resultIterator = page.GetIterator())
                {
                    resultIterator.Begin();

                    do
                    {
                        var word = resultIterator.GetText(PageIteratorLevel.Word);
                        if (word.Equals(wordToFind, StringComparison.OrdinalIgnoreCase))
                        {
                            if (resultIterator.TryGetBoundingBox(PageIteratorLevel.Word, out var box))
                            {

                                
                                var x = (box.X1 + box.X2) / 2;
                                var y = (box.Y1 + box.Y2) / 2;
                                
                                using (var graphics = Graphics.FromImage(visualImage))
                                {
                                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                                    float radius = 10.0f; // Размер круга (радиус) в пикселях
                                    int diameter = (int)(radius * 2);
                                    float xCenter = x - radius; // Используем координату центра контура
                                    float yCenter = y - radius; // Используем координату центра контура
                                    graphics.FillEllipse(Brushes.Red, xCenter, yCenter, diameter, diameter);
                                }


                                visualImage.Save("visual_screenshot.png", ImageFormat.Png);
                                
                                return new Point(x, y);
                            }
                        }
                    } while (resultIterator.Next(PageIteratorLevel.Word));
                }
            }
        }

        return Point.Empty;
    }
    
    // Метод для сканирования текста на скриншоте
    public string RecognizeText(Bitmap image)
    {
        string tessdataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");

        using (var engine = new TesseractEngine($"{tessdataDirectory}\\", "rus", EngineMode.Default))
        {
            using (var img = PixConverter.ToPix(image))
            {
                using (var page = engine.Process(img))
                {
                    return page.GetText();
                }
            }
        }
    }
}