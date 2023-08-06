using System.Drawing;
using AdvancedSharpAdbClient;
using AForge.Imaging;
using SWbot;
using SWbot.ImageRecognition;

namespace TestProject1;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test_Image_Template_Recognize()
    {
        var sourceFilePath = @"C:\Users\Robert\RiderProjects\SWbot\SWbot\img\test.png";
        var templateFilePath = $@"C:\Users\Robert\RiderProjects\SWbot\SWbot\img\test.png";
        var currentDirectory = Directory.GetCurrentDirectory();

        // Формирование относительного пути к папке img внутри проекта SWbot
        var relativePath = "..\\..\\..\\..\\SWbot\\img";

        // Получение полного пути к папке img
        var fullPath = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));

        var threshold = 0.8;

        var matchRect = ImageRecognition.Contains(sourceFilePath, templateFilePath, threshold);

        Assert.True(!matchRect.IsEmpty);
        if (!matchRect.IsEmpty)
        {
            Console.WriteLine("Шаблон обнаружен. Координаты и размеры области совпадения:");
            Console.WriteLine(
                $"X: {matchRect.X}, Y: {matchRect.Y}, Width: {matchRect.Width}, Height: {matchRect.Height}");
        }
        else
        {
            Console.WriteLine("Шаблон не обнаружен.");
        }
    }

    public async Task Test_Text_Search()
    {

        // Инициализация сервера ADB
        var adbServer = new AdbServer();
        adbServer.StartServer(@"C:\Program Files (x86)\ClockworkMod\Universal Adb Driver\adb.exe", false);

        // Получение списка устройств
        var client = new AdbClient();

        if (!(await client.GetDevicesAsync()).Any())
        {
            Console.WriteLine("Нет подключенных устройств.");
            return;
        }

        var device = client.GetDevices()
            .FirstOrDefault();

        var adbManager = new AdbManager(device);
        // await adbManager.CaptureScreenshotAsync(client, "screenshot.png");

        var img = await client.GetFrameBufferAsync(device, CancellationToken.None); // asynchronously

        var tesseractManager = new TesseractManager();
        var text = tesseractManager.RecognizeText(img.ToImage());

        Point? startButtonLocation = tesseractManager.FindWordOnImage(img.ToImage(), "Память");

        if (startButtonLocation.HasValue)
            await adbManager.TapButton(startButtonLocation.Value, client, img.ToImage());


    }

    public async Task Test_Find_Image()
    {
        var adbServer = new AdbServer();
        adbServer.StartServer(@"C:\Program Files (x86)\ClockworkMod\Universal Adb Driver\adb.exe", false);

        // Получение списка устройств
        var client = new AdbClient();

        if (!(await client.GetDevicesAsync()).Any())
        {
            Console.WriteLine("Нет подключенных устройств.");
            return;
        }

        var device = client.GetDevices()
            .FirstOrDefault();

        var adbManager = new AdbManager(device);
        await adbManager.CaptureScreenshotAsync(client, "screenshot.png");
        
        var imageToFindPath = Path.Combine(Directory.GetCurrentDirectory(), "img/test.png");
        var img = await client.GetFrameBufferAsync(device, CancellationToken.None); // asynchronously

        // Загрузка скриншота и иконки
        var iconToFind = new Bitmap(imageToFindPath);

        var screenshot = Utils.ConvertTo24BppRgb(img.ToImage());
        var imageToFind = Utils.ConvertTo24BppRgb(iconToFind);

        // Создание объекта для поиска шаблона

        // Поиск шаблона на скриншоте
        var matches = new TemplateMatch[] { };

        var vare = 0.96f;
        while (matches.Length == 0)
        {
            vare -= 0.05f;
            var templateMatching = new ExhaustiveTemplateMatching(vare);

            matches = templateMatching.ProcessImage(screenshot, imageToFind);
        }

        // Вывод результатов
        foreach (var match in matches)
        {
            var location = match.Rectangle;
            Console.WriteLine($"Искомая картинка найдена! Координаты: X={location.X}, Y={location.Y}");
            // Вы можете выполнить действия с найденной картинкой здесь, например, нажать на нее
            await adbManager.TapButton2(new Point(location.X, location.Y), client);
        }
    }
}