using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    public class UserModel : NotifiableModelObject
    {
        private string email;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }

        //    private ObservableCollection<BoardModel> boards;
        // public ObservableCollection<BoardModel> Boards
        private ObservableCollection<BoardModel> boards;
        public ObservableCollection<BoardModel> Boards
        {
            get => boards;
            set
            {
                boards = value;
                RaisePropertyChanged("Boards");
            }
        }

        internal BackendController controller;

        public UserModel(BackendController backendController, string email, ObservableCollection<BoardModel> boards): base(backendController)
        {
            controller = backendController;
            this.boards = boards;
            this.email = email;
            Boards.CollectionChanged += HandleChange;
        }


        private void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            //read more here: https://stackoverflow.com/questions/4279185/what-is-the-use-of-observablecollection-in-net/4279274#4279274
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (BoardModel board in e.OldItems)
                {

                    Controller.DeleteBoard(Email, board.BoardName);
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (BoardModel board in e.NewItems)
                {
                    Controller.CreateNewBoard(Email, board.BoardName);
                }
            }
        }

        internal BoardModel GetBoard(string boardName)
        {
            return boards.FirstOrDefault(x => x.BoardName == boardName);
        }
    }
}
