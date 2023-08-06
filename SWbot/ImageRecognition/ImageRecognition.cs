using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace SWbot.ImageRecognition;

public class ImageRecognition
{
    public static Rectangle Contains(string sourceFilePath, string templateFilePath, double threshold)
    {
        // Загрузка исходного изображения
        using (Image<Bgr, byte> sourceColor = new Image<Bgr, byte>(sourceFilePath))
        {
            // Преобразование исходного изображения в оттенки серого
            using (Image<Gray, byte> sourceGrey = sourceColor.Convert<Gray, byte>())
            {
                // Загрузка шаблонного изображения в оттенках серого
                using (Image<Gray, byte> templateGrey = new Image<Gray, byte>(templateFilePath))
                {
                    // Создание объекта для результата сопоставления
                    using (Image<Gray, float> result = sourceGrey.MatchTemplate(templateGrey, TemplateMatchingType.CcoeffNormed))
                    {
                        // Поиск наилучшего совпадения
                        
                        result.MinMax(out _, out double[] maxVal, out _, out Point[] maxLoc);


                        // Вычисление степени сходства
                        double similarity = maxVal[0] * 100;

                        // Сравнение с пороговым значением и определение совпадения
                        if (similarity >= threshold)
                        {
                            return new Rectangle(maxLoc[0].X, maxLoc[0].Y, templateGrey.Width, templateGrey.Height);
                        }
                        else
                        {
                            return Rectangle.Empty;
                        }
                    }
                }
            }
        }
    }
}