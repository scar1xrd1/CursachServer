using MahApps.Metro.Controls;
using Microsoft.EntityFrameworkCore.Metadata;
using Server.Classes;
using Server.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Server
{
    public partial class MainWindow : MetroWindow
    {
        SERVER server = new SERVER();

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token;

        ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ViewModel(itemListBox);
            DataContext = viewModel;
            token = cts.Token;
            StartListen();

            TryUpdateUsers();
        }

        void TryUpdateUsers()
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    itemListBox.ItemsSource = db.Users.ToList();
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        async void StartListen()
        {
            await Task.Run(StartListenAsync);
        }

        async Task StartListenAsync()
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    string received = await server.udp.ReceiveAsync();
                    //MessageBox.Show(received);

                    if (received == "updateusers") Application.Current.Dispatcher.Invoke(TryUpdateUsers);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            cts.Cancel();
            Process.GetCurrentProcess().Kill();
        }

        private async void Client_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemListBox.SelectedItem != null)
            {
                viewModel.IsLoading = true;
                viewModel.EmptyProcessVisibility = Visibility.Hidden;
                string login = ((User)itemListBox.SelectedItem).Login;

                await Task.Run(() => TryLoadUserInfo(login));

                viewModel.IsLoading = false;
            }
        }

        async Task TryLoadUserInfo(string login)
        {
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    var user = db.Users.FirstOrDefault(u => u.Login == login);
                    if (user != null)
                    {
                        var processes = user.GetForbiddenProcesses();
                        if (processes != null)
                        {
                            viewModel.Processes = new ObservableCollection<Process>(processes);
                            viewModel.EmptyProcessVisibility = Visibility.Hidden;
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                viewModel.EmptyProcessVisibility = Visibility.Visible;
                                viewModel.StateImageSource = new BitmapImage(new Uri("Images/emptyProcess.png", UriKind.Relative));
                                viewModel.StateLabelContent = "Список запрещенных процессов пуст";
                            });
                        }         
                    }
                }
            }
            catch { }
        }
    }
}