using System.Drawing;
using AdvancedSharpAdbClient;

public class AdbManager
{
    private DeviceData _device;

    public AdbManager(DeviceData device)
    {
        _device = device;
    }

    // Метод для выполнения скриншота
    public async Task CaptureScreenshotAsync(AdbClient adbClient, string imagePath)
    {
        var receiver = new ConsoleOutputReceiver();
        await adbClient.ExecuteRemoteCommandAsync($"screencap /sdcard/screenshot.png", _device, receiver);
        await adbClient.ExecuteRemoteCommandAsync($"pull /sdcard/screenshot.png C:\\{imagePath}", _device, receiver);
    }

    
    public async Task TapButton(Point location, AdbClient adbClient, Bitmap toImage)
    {
        // Преобразование координат для устройства
        int deviceScreenWidth = 1280; // Замените на реальные размеры экрана вашего устройства
        int deviceScreenHeight = 720;
        int deviceX = location.X * deviceScreenWidth / toImage.Width;
        int deviceY = location.Y * deviceScreenHeight / toImage.Height;

        var receiver = new ConsoleOutputReceiver();
        await adbClient.ExecuteRemoteCommandAsync($"input tap {deviceX} {deviceY}", _device, receiver);
    }
    
    public async Task TapButton2(Point location, AdbClient adbClient)
    {
        var receiver = new ConsoleOutputReceiver();

        await adbClient.ExecuteRemoteCommandAsync($"input tap {location.X} {location.Y}", _device, receiver);
    }
}