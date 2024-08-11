
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Frontend.Model;
using Frontend.ViewModel;
using IntroSE.Kanban.Backend.ServiceLayer;



namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : Window
    {
        public UserVM vm;
        public UserModel model;
     
        public UserView(UserModel user)
        {
            InitializeComponent();
            vm = new UserVM(user);
            
            //DataContext =vm;
            //Title = $"Welcome '{user.Email.Split("@")[0]}'!";
            //BoardListView.ItemsSource = user.Boards;

            DataContext = vm;
            Title = user.Email;
            BoardListView.ItemsSource = vm.UserBoards;

            model = user;
        }

        public UserView(BoardModel board)
        {
            InitializeComponent();
            vm = new UserVM(board);
            model = new UserModel(board.Controller, board.UserModelEmail, board.Controller.GetUserBoards(board.UserModelEmail));
            Title = board.UserModelEmail;
            BoardListView.ItemsSource = vm.UserBoards;
            DataContext =vm;
        }

        private void BoardListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BoardListView.SelectedItem != null)
            { 
                //BoardModel board = vm.GetBoard(model, "" + BoardListView.SelectedItem.ToString().Split(":")[1]);

                //BoardModel board = vm.GetBoard(model.Email,""+BoardListView.SelectedItem.ToString());
                BoardModel board = vm.GetBoard(model, "" + BoardListView.SelectedItem);

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
            newBoardName.ShowDialog();
            if (newBoardName.ClosedByUser) return;
            string userInput = newBoardName.UserInput;
            if (userInput == null || userInput == "")
            {
                MessageBox.Show("No input was given");
                return;
            }
            
            string res = vm.Controller.CreateNewBoard(model.Email, userInput);
            if (res != "null") MessageBox.Show($"The board '{userInput}' was created!");
            else MessageBox.Show(res);
        }

        private void Delete_Board(object sender, RoutedEventArgs e)
        {
            InputDialog newBoardName = new InputDialog("Deleting a new board", "Please enter the name of the board you want to delete");
            newBoardName.ShowDialog();
            if (newBoardName.ClosedByUser) return;
            string userInput = newBoardName.UserInput;
            if (userInput == null || userInput == "")
            {
                MessageBox.Show("No input was given");
                return;
            }

            string res = vm.Controller.DeleteBoard(model.Email, userInput);
            if (res != "null") MessageBox.Show($"The board '{userInput}' was Deleted!");
            else MessageBox.Show(res);
        }
    }
}
