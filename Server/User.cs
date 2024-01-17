using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;

namespace Server
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string? ForbiddenProcesses { get; set; }
        public string? AllProcesses { get; set; }

        public User() { }
        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public List<Process>? GetForbiddenProcesses()
        {
            try { return JsonSerializer.Deserialize<List<Process>>(ForbiddenProcesses); }
            catch { return null; }
        }

        public List<Process>? GetAllProcesses()
        {
            try { return JsonSerializer.Deserialize<List<Process>>(AllProcesses); }
            catch { return null; }
        }
    }
}
