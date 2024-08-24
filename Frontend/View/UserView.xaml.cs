
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
            
            //DataContext =vm;
            //Title = $"Welcome '{user.Email.Split("@")[0]}'!";
            //BoardListView.ItemsSource = user.Boards;

            DataContext = vm;
            Title = user.Email;
            BoardListView.ItemsSource = vm.UserBoards;

            model = user;
            SoundManager.PlaySound(SoundManager.SoundEffect.Login);
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
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
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
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            vm.LogoutUser(model.Email);
            if (vm.ErrorMessage != string.Empty)
            {
                MessageBox.Show(vm.ErrorMessage);    
            }
            
            SoundManager.PlaySound(SoundManager.SoundEffect.Logout);
            MessageBox.Show("You Logout successfully");
            LoginView loginWindow = new LoginView(model.Controller);
            loginWindow.Show();
            Close();
        }

        private void Create_Board(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            InputDialog newBoardName = new InputDialog("Creating a new board", "Please enter the name of the new board");
            newBoardName.ShowDialog();
            if (newBoardName.ClosedByUser) return;
            string userInput = newBoardName.UserInput;
            if (userInput == null || userInput == "")
            {
                SoundManager.PlaySound(SoundManager.SoundEffect.Error);
                MessageBox.Show("No input was given");
                SoundManager.PlaySound(SoundManager.SoundEffect.Click);
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
                SoundManager.PlaySound(SoundManager.SoundEffect.Error);
                MessageBox.Show(ex.Message);
                SoundManager.PlaySound(SoundManager.SoundEffect.Click);
                return;
            }
            catch (Exception ex)
            {
                // do nothing
            }

            vm.UserBoards.Add(new BoardModel(model.Controller, model.Email, userInput, model.Email, new List<string> { model.Email }));
            MessageBox.Show($"The board '{userInput}' was created!");
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
        }

        private void Delete_Board(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            InputDialog newBoardName = new InputDialog("Deleting a new board", "Please enter the name of the board you want to delete");
            newBoardName.ShowDialog();
            if (newBoardName.ClosedByUser) return;
            string userInput = newBoardName.UserInput;
            if (userInput == null || userInput == "")
            {
                SoundManager.PlaySound(SoundManager.SoundEffect.Error);
                MessageBox.Show("No input was given");
                SoundManager.PlaySound(SoundManager.SoundEffect.Click);
                return;
            }
            BoardModel boardToDelete;
            try
            {
                boardToDelete = vm.Controller.GetUserBoards(model.Email).First(x => x.BoardName == userInput);
                if (boardToDelete.Owner != model.Email) throw new Exception("You can't delete a board you are not the owner of!");
            }
            catch (Exception ex)
            {
                //board doesnt exist
                SoundManager.PlaySound(SoundManager.SoundEffect.Error);
                MessageBox.Show(ex.Message);
                SoundManager.PlaySound(SoundManager.SoundEffect.Click);
                return;
            }

            vm.UserBoards.Remove(vm.UserBoards.Where(x => x.BoardName == userInput).Single());
            MessageBox.Show($"The board '{userInput}' was Deleted!");
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
        }
    }
}
