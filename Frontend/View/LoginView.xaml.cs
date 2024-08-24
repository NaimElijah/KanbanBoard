
﻿using System.Windows;
using System.Windows.Controls;
using Frontend.Model;
using Frontend.Resources;
using Frontend.Utilities;
using Frontend.View;
using Frontend.ViewModel;


namespace Frontend.View;

public partial class LoginView : Window
{
    private LoginVM vm;

    public LoginView()
    {
        InitializeComponent();
        vm = new LoginVM();
        Title = "Login menu";
        DataContext = vm;
        SoundManager.PlaySound(SoundManager.SoundEffect.Welcome);
    }

    public LoginView(BackendController controller)
    {
        InitializeComponent();
        vm = new LoginVM(controller);
        DataContext = vm;
    }

    internal LoginView(LoginVM vm)
    {
        InitializeComponent();
        this.vm = vm;
        DataContext = vm;
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        SoundManager.PlaySound(SoundManager.SoundEffect.Click);
        UserModel? user = vm.Login();
        if (user == null)
        {
            MessageDisplayer.DisplayError(vm.ErrorMessage);
            return;
        }
        UserView uv = new UserView(user);
        uv.Show();
        Close();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        SoundManager.PlaySound(SoundManager.SoundEffect.Click);
        vm.Email = "";
        vm.Password = "";
        RegisterView registerView = new RegisterView(vm);
        registerView.Show();
        Close();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        vm.Password = PasswordBox.Password;
        PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
    }

    private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(PasswordBox.Password))
        {
            PasswordPlaceholder.Visibility = Visibility.Hidden;
        }
    }

    private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(PasswordBox.Password))
        {
            PasswordPlaceholder.Visibility = Visibility.Visible;
        }
    }

    private void EmailBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        vm.Email = EmailTextBox.Text;
        PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Hidden;
    }

    private void EmailBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(EmailTextBox.Text))
        {
            EmailPlaceholder.Visibility = Visibility.Hidden;
        }
    }

    private void EmailBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(EmailTextBox.Text))
        {
            EmailPlaceholder.Visibility = Visibility.Visible;
        }
    }
}