using Frontend.Model;
using Frontend.View;
using IntroSE.Kanban.Frontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IntroSE.Kanban.Frontend.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        private LoginVM vm;

        public LoginWindow()
        {
            InitializeComponent();
            vm = new LoginVM();
            DataContext = vm;
        }

        public LoginWindow(BackendController controller)
        {
            InitializeComponent();
            vm = new LoginVM(controller);
            DataContext = vm;
        }

        internal LoginWindow(LoginVM vm)
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
}
