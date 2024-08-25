using Frontend.Model;
using Frontend.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

            //int totalTaskNum = board.BacklogTasks.Count + board.InProgressTasks.Count + board.DoneTasks.Count;
            //Title = $"This is '{board.User.Email}'s board named: '{board.Name}'. Number of tasks: {totalTaskNum}";

            Title = board.BoardName;
            MembersBorder.ItemsSource = vm.Members;
            BacklogTasks.ItemsSource = vm.Backlog;
            InProgressTasks.ItemsSource = vm.InProgress;
            DoneTasks.ItemsSource = vm.Done;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            UserView userWindow = new UserView(model);
            userWindow.Show();
            this.Close();
        }
    }
}