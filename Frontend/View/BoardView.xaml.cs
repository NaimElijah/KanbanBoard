using Frontend.Model;
using Frontend.Utilities;
using Frontend.View;
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
            MembersListBox.ItemsSource = model.Members;
            BacklogList.ItemsSource = vm.Backlog;
            InProgressList.ItemsSource = vm.InProgress;
            DoneList.ItemsSource = vm.Done;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SoundManager.PlaySound(SoundManager.SoundEffect.Click);
            UserView userWindow = new UserView(model);
            userWindow.Show();
            this.Close();
        }
    }
}