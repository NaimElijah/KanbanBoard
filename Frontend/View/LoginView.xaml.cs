
﻿using System.Windows;
using System.Windows.Controls;
using Frontend.Model;
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
        DataContext = vm;

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
        UserModel? user = vm.Login();
        if (user == null)
        {
            MessageBox.Show(vm.ErrorMessage);
            return;
        }

        UserView uv = new UserView(user);
        uv.Show();
        Close();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        RegisterView registerView = new RegisterView(vm);
        registerView.Show();
        Close();
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        vm.Password = PasswordBox.Password;
    }
}