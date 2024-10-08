﻿using System.Windows;
using Frontend.Model;
using Frontend.View;
using System.Windows.Controls;
using Frontend.ViewModel;
using Frontend.Utilities;
using Frontend.Resources;

namespace Frontend.View
{

    public partial class RegisterView : Window
    {
        private LoginVM vm;

        internal RegisterView(LoginVM vm)
        {
            InitializeComponent();
            this.vm = vm;
            Title = "Register menu";
            DataContext = vm;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            vm.Email = "";
            vm.Password = "";
            LoginView loginWindow = new LoginView(vm);
            loginWindow.Show();
            Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            UserModel? user = vm.Register();
            if (user == null)
            {
                MessageDisplayer.DisplayError(vm.ErrorMessage);
                return;
            }
            UserView uv = new UserView(user);
            uv.Show();
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
}