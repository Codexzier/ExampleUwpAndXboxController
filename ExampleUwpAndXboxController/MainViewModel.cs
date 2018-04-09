using System.ComponentModel;
using Windows.UI.Xaml;

namespace ExampleUwpAndXboxController
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _textPosition;
        public string TextPosition {
            get => this._textPosition;
            set
            {
                this._textPosition = value;
                this.OnPropertyChanged(nameof(this.TextPosition));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyname) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
    }
}
