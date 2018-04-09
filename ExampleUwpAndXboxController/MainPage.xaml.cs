using System;
using System.Threading.Tasks;
using Windows.Gaming.Input;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ExampleUwpAndXboxController
{
    public sealed partial class MainPage : Page
    {
private MainViewModel _viewModel = new MainViewModel();

/// <summary>
/// Ermöglicht eine Schleife Asynchron zum Haupteil der Anwendung auszuführen.
/// </summary>
private Task _run;

/// <summary>
/// Hauptklasse für das einlesen des Xbox Controllers.
/// </summary>
private Gamepad _gamePad;

/// <summary>
/// Wird nur für den zufälligen Farbwechsel verwendet.
/// </summary>
private Random _random = new Random();

public MainPage()
{
    this.InitializeComponent();

    this.DataContext = this._viewModel;

    Gamepad.GamepadAdded += this.Gamepad_GamepadAdded;
    Gamepad.GamepadRemoved += this.Gamepad_GamepadRemoved;

    this.MovingPoint.Fill = new SolidColorBrush(Colors.Black);
}

/// <summary>
/// Wenn der Xbox Controller getrennt wird.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
private void Gamepad_GamepadRemoved(object sender, Gamepad e) => this._gamePad = null;

/// <summary>
/// Wird ausgeführt, wenn ein Xbox Controller verbunden wurde.
/// </summary>
/// <param name="sender"></param>
/// <param name="e">Übergibt die Instanz der verbundenen Xbox Controllers.</param>
private void Gamepad_GamepadAdded(object sender, Gamepad e)
{
    this._gamePad = e;

    this._run = new Task(this.ReadingGamepad);
    this._run.Start();
}

/// <summary>
/// Liest die den Xbox Controller aus. Für den Vorgang wird eine Schleife verwendet.
/// </summary>
private async void ReadingGamepad()
{
    while (true)
    {
        if(this._gamePad == null) { break; }

        // Aktuelle Eingabe erfassen.
        GamepadReading gp = this._gamePad.GetCurrentReading();
                
        // UI und der Task laufen in verschiedenen Ebenen. 
        // Daher muss über den Dispatcher die Eingabe Daten 
        // und die Daten aus dem ViewModel Bindung Async behandelt werden.
        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        {
            // Linken Stick einlesen
            var left = gp.LeftThumbstickX;
            var top = gp.LeftThumbstickY * -1;

            // Abstand zum Rand
            var distanceToBorder = 30;

            // Eingabe zu einer Position auf der Anwendung umrechnen.
            var halfLeft = (this.ActualWidth - (distanceToBorder * 2)) / 2d;
            left = halfLeft + (left * halfLeft);
            var halfTop = (this.ActualHeight - distanceToBorder * 2) / 2d;
            top = halfTop + (top * halfTop);

            // verhindern, dass der Punkt den Rand nicht erreicht.
            if (left < distanceToBorder) { left = distanceToBorder; }
            if (left > this.ActualWidth) { left = this.ActualWidth; }
            if (top < distanceToBorder) { top = distanceToBorder; }
            if (top > this.ActualHeight - distanceToBorder) { top = this.ActualHeight - distanceToBorder; }

            // Position als Text Ausgeben mit bis zu zwei Stellen hinter dem Komma.
            this._viewModel.TextPosition = $"LEFT: {left:N2}, TOP: {top:N2}";

            // leider kein Binding, daher die simple zuweisung der Ziel Position und Farbe
            this.MovingPoint.Margin = new Thickness(left, top, 0, 0);
                    
            // Farbe wechseln
            if (gp.Buttons == GamepadButtons.A)
            {
                // verkürzte die schreibweise
                byte f() => (byte)this._random.Next(0, 255);
                this.MovingPoint.Fill = new SolidColorBrush(Color.FromArgb(255, f(), f(), f()));
            }
        });
          
        // 2 Millisekunden interval
        await Task.Delay(2);
    }
}
    }
}
