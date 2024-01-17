using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Server
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<User>? Users { get; set; }
        public ObservableCollection<Process>? Processes { get; set; }

        ListBox itemListBox;

        bool isLoading = false;
        Visibility emptyProcessVisibility = Visibility.Visible;
        BitmapImage stateImageSource = new BitmapImage(new Uri("Images/emptyUser.png", UriKind.Relative));
        string stateLabelContent = "Пользователь не выбран";

        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }
        public Visibility EmptyProcessVisibility { get => emptyProcessVisibility; set { emptyProcessVisibility = value; OnPropertyChanged(nameof(EmptyProcessVisibility)); } }
        public BitmapImage StateImageSource { get => stateImageSource; set { stateImageSource = value; OnPropertyChanged(nameof(StateImageSource)); } }
        public string StateLabelContent { get => stateLabelContent; set { stateLabelContent = value; OnPropertyChanged(nameof(StateLabelContent)); } }

        public ViewModel(ListBox listBox) 
        {
            itemListBox = listBox;
            using (DatabaseContext db = new DatabaseContext())
            {
                if(db.Users != null)
                {
                    itemListBox.ItemsSource = db.Users.ToList();
                }                
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
