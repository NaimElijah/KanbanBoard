using Frontend.Model;
using Frontend.View;
using Frontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Frontend.View
{
    public partial class BoardView : Window
    {
        BoardVM vm;
        BoardModel model;

        public BoardView(BoardModel board)
        {
            InitializeComponent();
            vm = new BoardVM(board);
            DataContext = vm;
            model = board;
            Title = board.Name;
            BacklogList.ItemsSource = vm.Backlog;
            InProgressList.ItemsSource = vm.InProgress;
            DoneList.ItemsSource = vm.Done;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            UserView userWindow = new UserView(model.User);
            userWindow.Show();
            this.Close();
        }
    }
}