using System.Windows;

namespace Frontend.View
{
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string message, string title = "", bool showOkButton = true)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
            this.Title = title;
            OkButton.Visibility = showOkButton ? Visibility.Visible : Visibility.Collapsed;

            if (!showOkButton)
            {
                this.KeyDown += (s, e) =>
                {
                    if (e.Key == System.Windows.Input.Key.Escape)
                        this.Close();
                };
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public static void Show(string message, string title = "", bool showOkButton = true)
        {
            CustomMessageBox messageBox = new CustomMessageBox(message, title, showOkButton);
            messageBox.ShowDialog();
        }

        public static bool ShowWithResult(string message, string title = "Message")
        {
            CustomMessageBox messageBox = new CustomMessageBox(message, title, true);
            return messageBox.ShowDialog() ?? false;
        }
    }
}