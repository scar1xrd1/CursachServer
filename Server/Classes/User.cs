using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;

namespace Server.Classes
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string? ForbiddenProcesses { get; set; }
        public string? AllProcesses { get; set; }

        public User() { }
        public User(string login, string password, string passwordSalt)
        {
            Login = login;
            Password = password;
            PasswordSalt = passwordSalt;
        }

        public List<MyProcess>? GetForbiddenProcesses()
        {
            try { return JsonSerializer.Deserialize<List<MyProcess>>(ForbiddenProcesses); }
            catch { return null; }
        }

        public List<MyProcess>? GetAllProcesses()
        {
            try { return JsonSerializer.Deserialize<List<MyProcess>>(AllProcesses); }
            catch { return null; }
        }
    }
}
