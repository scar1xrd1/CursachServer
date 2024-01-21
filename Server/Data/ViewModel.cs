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
        ObservableCollection<MyProcess>? processes = new ObservableCollection<MyProcess>();
        ObservableCollection<MyProcess>? forbiddenProcesses = new ObservableCollection<MyProcess>();
        bool isLoading = false;
        Visibility emptyProcessVisibility = Visibility.Visible;
        Visibility processVisibility = Visibility.Hidden;
        BitmapImage stateImageSource = new BitmapImage(new Uri("Images/emptyUser.png", UriKind.Relative));
        string stateLabelContent = "Пользователь не выбран";

        public ObservableCollection<User>? Users { get => users; set { users = value; OnPropertyChanged(nameof(Users)); } }
        public ObservableCollection<MyProcess>? Processes { get => processes; set { processes = value; OnPropertyChanged(nameof(Processes)); } }
        public ObservableCollection<MyProcess>? ForbiddenProcesses { get => forbiddenProcesses; set { forbiddenProcesses = value; OnPropertyChanged(nameof(ForbiddenProcesses)); } }
        public bool IsLoading { get => isLoading; set { isLoading = value; OnPropertyChanged(nameof(IsLoading)); } }
        public Visibility EmptyProcessVisibility { get => emptyProcessVisibility; set { emptyProcessVisibility = value; OnPropertyChanged(nameof(EmptyProcessVisibility)); } }
        public Visibility ProcessVisibility { get => processVisibility; set { processVisibility = value; OnPropertyChanged(nameof(ProcessVisibility)); } }
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
