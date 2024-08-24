using Frontend.Utilities;
using System.Windows;


namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public bool ClosedByUser = false;
        public string UserInput { get; private set; }

        public InputDialog(string title, string message)
        {
            InitializeComponent();
            this.Title = title;
            this.PromptTextBlock.Text = message;
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            UserInput = InputTextBox.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            this.DialogResult = false;
            MessageBox.Show("Operation canceled.");
            ClosedByUser = true;
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            this.Close();
        }
    }
}
