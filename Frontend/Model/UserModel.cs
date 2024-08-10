using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private List<string> boards;
        public List<string> Boards
        {
            get => boards;
            set
            {
                boards = value;
                RaisePropertyChanged("Boards");
            }
        }

        internal BackendController controller;

        public UserModel(BackendController backendController, string email, List<string> boards): base(backendController)
        {
            this.controller = backendController;
            this.boards = boards;
            this.email = email;
        }
    }
}
