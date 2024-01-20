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
using Server.Classes;

namespace Server.Data
{
    public class ViewModel : INotifyPropertyChanged
    {
        ListBox itemListBox;

        ObservableCollection<User>? users = new ObservableCollection<User>();
        ObservableCollection<Process>? processes = new ObservableCollection<Process>();
        bool isLoading = false;
        Visibility emptyProcessVisibility = Visibility.Visible;
        BitmapImage stateImageSource = new BitmapImage(new Uri("Images/emptyUser.png", UriKind.Relative));
        string stateLabelContent = "Пользователь не выбран";

        public ObservableCollection<User>? Users { get => users; set { users = value; OnPropertyChanged(nameof(Users)); } }
        public ObservableCollection<Process>? Processes { get => processes; set { processes = value; OnPropertyChanged(nameof(Processes)); } }
        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }
        public Visibility EmptyProcessVisibility { get => emptyProcessVisibility; set { emptyProcessVisibility = value; OnPropertyChanged(nameof(EmptyProcessVisibility)); } }
        public BitmapImage StateImageSource { get => stateImageSource; set { stateImageSource = value; OnPropertyChanged(nameof(StateImageSource)); } }
        public string StateLabelContent { get => stateLabelContent; set { stateLabelContent = value; OnPropertyChanged(nameof(StateLabelContent)); } }

        public ViewModel(ListBox listBox)
        {
            itemListBox = listBox;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
