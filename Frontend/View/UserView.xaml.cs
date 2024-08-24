
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Frontend.Model;
using Frontend.Utilities;
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
            DataContext = vm;
            Title = $"Welcome '{user.Email.Split('@')[0]}'!";
            model = user;

            //BoardListView.ItemsSource = vm.UserBoards;
        }

        public UserView(BoardModel board)
        {
            InitializeComponent();
            vm = new UserVM(board);
            model = new UserModel(board.Controller, board.UserModelEmail, board.Controller.GetUserBoards(board.UserModelEmail));
            Title = board.UserModelEmail;
            DataContext = vm;
            //BoardListView.ItemsSource = vm.UserBoards;
        }


        /*    private void BoardListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            }*/

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
            BoardModel boardToAdd;
            try
            {
                boardToAdd = vm.Controller.GetUserBoards(model.Email).First(x => x.BoardName == userInput);
                throw new AmbiguousMatchException($"A board named '{userInput}' already exist for this user!");
            }
            catch (AmbiguousMatchException ex)
            {
                //board already exist
                MessageBox.Show(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                // do nothing
            }
            vm.UserBoards.Add(new BoardModel(model.Controller, model.Email, userInput, model.Email, new List<string> { model.Email }));
            MessageBox.Show($"The board '{userInput}' was created!");
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
            BoardModel boardToDelete;
            try
            {
                boardToDelete = vm.Controller.GetUserBoards(model.Email).First(x => x.BoardName == userInput);
            }
            catch (Exception ex)
            {
                //board doesnt exist
                MessageBox.Show($"A board named '{userInput}' doesn't exist for this user!");
                return;
            }

            vm.UserBoards.Remove(vm.UserBoards.Where(x => x.BoardName == userInput).Single());
            MessageBox.Show($"The board '{userInput}' was Deleted!");
        }
        private void Selcted_Board_Button_Click(object sender, RoutedEventArgs e)
        {
            // Assuming this is the click handler for the Board button
            Button boardButton = sender as Button;
            BoardModel selectedBoard = boardButton.DataContext as BoardModel;

            if (selectedBoard != null)
            {
                BoardView boardView = new BoardView(selectedBoard);
                boardView.Show();
                Close();
            }
        }
    }
}
