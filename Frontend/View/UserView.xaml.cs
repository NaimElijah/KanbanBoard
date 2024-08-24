
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
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
            LoadBoards();
        }

        public UserView(BoardModel board)
        {
            InitializeComponent();
            vm = new UserVM(board);
            model = new UserModel(board.Controller, board.UserModelEmail, board.Controller.GetUserBoards(board.UserModelEmail));
            Title = board.UserModelEmail;
            DataContext = vm;
            LoadBoards();
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
            if (string.IsNullOrWhiteSpace(userInput))
            {
                MessageBox.Show("No input was given");
                return;
            }
            BoardModel boardToAdd;
            try
            {
                // Check if the board already exists
                var existingBoard = vm.UserBoards.FirstOrDefault(x => x.BoardName == userInput);
                if (existingBoard != null)
                {
                    throw new InvalidOperationException($"A board named '{userInput}' already exists for this user!");
                }

                // Create and add the new board
                var newBoard = new BoardModel(model.Controller, model.Email, userInput, model.Email, new List<string> { model.Email });
                vm.UserBoards.Add(newBoard);
                LoadBoards(); // Refresh the boards
                MessageBox.Show($"The board '{userInput}' was created!");
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Delete_Board(object sender, RoutedEventArgs e)
        {

            e.Handled = true;

            // Get the button that was clicked
            Button deleteButton = sender as Button;
            if (deleteButton == null) return;

            // Get the parent button (the main board button)
            Button boardButton = deleteButton.TemplatedParent as Button;
            if (boardButton == null) return;

            // Get the BoardModel from the DataContext of the board button
            BoardModel boardToDelete = boardButton.DataContext as BoardModel;
            if (boardToDelete == null)
            {
                MessageBox.Show("Unable to identify the board to delete.");
                return;
            }

            // Confirm deletion with the user
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete the board '{boardToDelete.BoardName}'?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Remove the board from the ViewModel's collection
                    vm.UserBoards.Remove(boardToDelete);
                    LoadBoards() ;

                    MessageBox.Show($"The board '{boardToDelete.BoardName}' was deleted successfully.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the board: {ex.Message}");
                }
            }

        }
        private void Selcted_Board_Button_Click(object sender, RoutedEventArgs e)
        {
            var boardButton = sender as Button;
            var selectedBoard = boardButton?.DataContext as BoardModel;

            if (selectedBoard != null)
            {
                var boardView = new BoardView(selectedBoard);
                boardView.Show();
                Close();
            }
        }

        private void LoadBoards()
        {
            // Assuming vm.UserBoards is an observable collection of BoardModel
            var boards = vm.UserBoards.ToList(); // Convert to list if needed
            CreateBoardButtons(boards);
        }

        private void CreateBoardButtons(List<BoardModel> boards)
        {
            // Ensure the ItemsControl's ItemsSource is set correctly
            var boardButtons = boards.Select(board => new Button
            {
                Style = (Style)FindResource("BoardsButtonStyle"),
                //Background = new SolidColorBrush(ColorConverter.ConvertFromString("{ TemplateBinding Background }")),
                //Foreground = new SolidColorBrush(Colors.Pink),
                Width = 150, // Use consistent width and height
                Height = 150,
                DataContext = board, // Bind DataContext to the board
         
                Content = new StackPanel
                {
                    Children =
            {
                new TextBlock
                {
                    Text = board.BoardName,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White)
                },
                new TextBlock
                {
                    Text = board.Owner,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.White)
                }
            }
                }
            }).ToList();

            foreach (var button in boardButtons) button.Click += Selcted_Board_Button_Click;

            // Add "Add Board" button
            var addButton = new Button
            {
                Style = (Style)FindResource("AddBoardButtonStyle"),
               /* Background = (Color)FindResource(),
                Foreground = new SolidColorBrush(Colors.Pink),
                Width = 150,
                Height = 150*/
            };

            // Hook up the click event for "Add Board" button
            addButton.Click += AddBoard_Click;

            // Add all buttons to a collection
            var allButtons = boardButtons;
            allButtons.Add(addButton);

            // Set the ItemsSource for the ItemsControl
            BoardItemsControl.ItemsSource = allButtons;
        }

        private void AddBoard_Click(object sender, RoutedEventArgs e)
        {
            // Create a new board when the "Add Board" button is clicked
            Create_Board(sender, e);
        }

    }
}
