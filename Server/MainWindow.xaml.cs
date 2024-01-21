using MahApps.Metro.Controls;
using Server.Classes;
using Server.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            //MessageBox.Show(Process.GetCurrentProcess().ProcessName);

            Task.Run(PeriodicUpdateInformation);
            Task.Run(PeriodicUpdateProcesses);
        }

        void TryUpdateUsers()
        {
            try
            {
                //MessageBox.Show("try");
                using (DatabaseContext db = new DatabaseContext())
                {
                    viewModel.Users = new ObservableCollection<User>(db.Users.ToList());
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
                viewModel.ProcessVisibility = Visibility.Hidden;
                viewModel.StateUserVisibility = Visibility.Hidden;
                viewModel.ForbiddenProcesses.Clear();
                viewModel.Processes.Clear();
                string login = ((User)itemListBox.SelectedItem).Login;

                await Task.Run(() => TryLoadUserInfo(login));

                viewModel.IsLoading = false;
            }
        }

        async Task PeriodicUpdateInformation()
        {
            while(true)
            {
                await Task.Delay(1000);

                try
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        var users = db.Users.ToList();
                        foreach(var user in users) 
                            Application.Current.Dispatcher.Invoke(() => { if (!viewModel.Users.Any(u => u.Login == user.Login)) viewModel.Users.Add(user); });
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            foreach(var user in viewModel.Users)
                            {
                                if (users.FirstOrDefault(u => u.Login == user.Login) == null)
                                {
                                    if(server.CurrentLogin == user.Login) viewModel.ProcessVisibility = Visibility.Hidden;                                    
                                    viewModel.Users.Remove(user);
                                    viewModel.ForbiddenProcesses.Clear();
                                    viewModel.Processes.Clear();
                                    viewModel.StateUserVisibility = Visibility.Visible;
                                }
                            }                                
                        });
                    }
                }
                catch { }
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
                        if (processes != null && processes.Count > 0)
                        {
                            viewModel.ProcessVisibility = Visibility.Visible;
                            //viewModel.Processes = new ObservableCollection<MyProcess>(processes);
                            //viewModel.EmptyProcessVisibility = Visibility.Hidden;
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                viewModel.ProcessVisibility = Visibility.Visible;
                                viewModel.EmptyProcessVisibility = Visibility.Visible;
                                viewModel.StateImageSource = new BitmapImage(new Uri("Images/emptyProcess.png", UriKind.Relative));
                                viewModel.StateLabelContent = "Список пуст";
                            });
                        }
                    }
                }
                server.CurrentLogin = login;
            }
            catch { }
        }

        async Task PeriodicUpdateProcesses()
        {
            while (true)
            {
                if (server.CurrentLogin != null)
                {
                    await Task.Delay(1000);

                    //var allProcesses = Process.GetProcesses().DistinctBy(p => p.ProcessName);
                    //List<MyProcess> myProcesses = new List<MyProcess>();
                    //foreach (var process in allProcesses) { myProcesses.Add(new MyProcess() { ProcessName = process.ProcessName, ProcessId = process.Id.ToString() }); }

                    //viewModel.Processes = new ObservableCollection<MyProcess>(myProcesses);

                    try
                    {
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            var user = db.Users.FirstOrDefault(u => u.Login == server.CurrentLogin);
                            //var allProcesses = user.GetAllProcesses();
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                if(user.AllProcesses != null) viewModel.Processes = new ObservableCollection<MyProcess>(user.GetAllProcesses());
                                if (user.ForbiddenProcesses != null && user.GetForbiddenProcesses().Count > 0)
                                {
                                    viewModel.EmptyProcessVisibility = Visibility.Hidden;
                                    viewModel.ForbiddenProcesses = new ObservableCollection<MyProcess>(user.GetForbiddenProcesses());
                                }
                                else if(user.ForbiddenProcesses != null && user.GetForbiddenProcesses().Count == 0)
                                {
                                    viewModel.EmptyProcessVisibility = Visibility.Visible;
                                    viewModel.ForbiddenProcesses = new ObservableCollection<MyProcess>(user.GetForbiddenProcesses());
                                }
                            });


                            //if(user.AllProcesses == null) user.AllProcesses = new List<MyProcess>(myProcesses);
                            //if(db.Users.FirstOrDefault(u => u.Login == client.Login).AllProcesses != null)
                            //    db.Users.FirstOrDefault(u => u.Login == client.Login).AllProcesses.Clear();                            
                            //db.Users.FirstOrDefault(u => u.Login == client.Login).AllProcesses = myProcesses;
                            db.SaveChanges();
                        }

                        //await client.SendServer("updateusers");
                    }
                    catch { }
                }
            }
        }

        private void ProcessItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is MyProcess process)
            {
                try
                {
                    if (process.ProcessName == "Client")
                    {
                        MessageBox.Show("Это шутка какая-то?");
                        return;
                    }

                    Task.Run(() =>
                    {
                        using (var db = new DatabaseContext())
                        {
                            var user = db.Users.FirstOrDefault(u => u.Login == server.CurrentLogin);
                            if (user.ForbiddenProcesses != null)
                            {
                                var forbProcesses = user.GetForbiddenProcesses();
                                if (forbProcesses.FirstOrDefault(p => p.ProcessName == process.ProcessName) != null) return;
                                forbProcesses.Add(process);
                                user.ForbiddenProcesses = JsonSerializer.Serialize(forbProcesses);
                            }
                            else user.ForbiddenProcesses = JsonSerializer.Serialize(new List<MyProcess>() { process });
                            Application.Current.Dispatcher.Invoke(() => 
                            {
                                viewModel.ForbiddenProcesses = new ObservableCollection<MyProcess>(user.GetForbiddenProcesses());
                                viewModel.EmptyProcessVisibility = Visibility.Hidden;
                            });
                            db.SaveChanges();
                        }
                    });
                }
                catch { }
            }
        }

        private void ForbiddenProcessItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem listBoxItem && listBoxItem.DataContext is MyProcess process)
            {
                try
                {
                    Task.Run(() =>
                    {
                        using (var db = new DatabaseContext())
                        {
                            var user = db.Users.FirstOrDefault(u => u.Login == server.CurrentLogin);
                            if (user.ForbiddenProcesses != null)
                            {
                                var forbProcesses = user.GetForbiddenProcesses();
                                var result = forbProcesses.FirstOrDefault(p => p.ProcessName == process.ProcessName);
                                forbProcesses.Remove(result);
                                user.ForbiddenProcesses = JsonSerializer.Serialize(forbProcesses);
                            }
                            Application.Current.Dispatcher.Invoke(() => 
                            { 
                                viewModel.ForbiddenProcesses = new ObservableCollection<MyProcess>(user.GetForbiddenProcesses());                                
                            });
                            db.SaveChanges();
                        }
                    });
                }
                catch { }
            }
        }

        private async void Unhide_Click(object sender, RoutedEventArgs e)
        {
            await server.SendClient($"unhide{server.CurrentLogin}");
        }
    }
}