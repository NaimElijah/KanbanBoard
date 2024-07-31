using System.Windows;
using Frontend.Model;
using Frontend.View;
using System.Windows.Controls;
using Frontend.ViewModel;

namespace Frontend.View;

public partial class RegisterView : Window
{
    private LoginVM vm;

    internal RegisterView(LoginVM vm)
    {
        InitializeComponent();
        this.vm = vm;
        DataContext = vm;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        LoginView loginWindow = new LoginView(vm);
        loginWindow.Show();
        Close();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        UserModel? user = vm.Register();
        if (user == null)
        {
            MessageBox.Show(vm.ErrorMessage);
            return;
        }

        MessageBox.Show("Registered successfully");
        UserView uv = new UserView(user);
        uv.Show();
        Close();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        vm.Password = PasswordBox.Password;
    }
}