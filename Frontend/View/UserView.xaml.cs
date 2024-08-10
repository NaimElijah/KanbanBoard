
using System.Windows;
using System.Windows.Controls;
using Frontend.Model;
using Frontend.ViewModel;



namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : Window
    {
        UserVM vm;
        UserModel model;

        public UserView(UserModel user)
        {
            InitializeComponent();
            vm = new UserVM(user);
            DataContext =vm;
            Title = $"Welcome '{user.Email.Split("@")[0]}'!";
            BoardListView.ItemsSource = user.Boards;
            model = user;
        }

        public UserView(BoardModel board)
        {
            InitializeComponent();
            vm = new UserVM(board.Controller);
            DataContext =vm;
        }

        private void BoardListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoardListView.SelectedItem != null)
            { 
                BoardModel board = vm.GetBoard(model, "" + BoardListView.SelectedItem.ToString().Split(":")[1]);
                BoardView boardView = new BoardView(board);
                boardView.Show();
                Close();
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            vm.LogoutUser(model.Email);
            if (vm.ErrorMessage != string.Empty)
            {
                MessageBox.Show(vm.ErrorMessage);    
            }
                       
            MessageBox.Show("You Logout successfully");

            LoginView loginWindow = new LoginView(model.Controller);
            loginWindow.Show();
            Close();
        }

        private void Create_Board(object sender, RoutedEventArgs e)
        {
            InputDialog newBoardName = new InputDialog("Creating a new board", "Please enter the name of the new board");
            if (newBoardName.ShowDialog() == true)
            {
                string userInput = newBoardName.UserInput;
                MessageBox.Show($"The board '{userInput}' was created!");
            }
            else
            {
                MessageBox.Show("Creation canceled.");
                return;
            }
        }

    }
}
