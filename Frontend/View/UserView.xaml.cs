
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
            Title = user.Email;
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
                BoardModel board = vm.GetBoard(model,""+BoardListView.SelectedItem.ToString());
                BoardView boardView = new BoardView(board);
                boardView.Show();
                Close();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LoginView loginWindow = new LoginView(model.Controller);
            loginWindow.Show();
            Close();
        }
    }
}
