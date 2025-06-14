using Microsoft.EntityFrameworkCore;
using SKAT_Interface.Data; // Где находится DbOneContext
using SKAT_Interface.Models.DbUsersModels; // Где находится модель User
using System.Threading.Tasks;

namespace SKAT_Interface.Services
{
    public class AuthService
    {
        private readonly DbUsersContext _context; // Используем контекст, где лежит таблица пользователей

        public AuthService(DbUsersContext context) // Внедряем нужный DbContext
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            // Находим пользователя по логину. Используйте актуальное имя свойства для Username.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == username);

            if (user != null)
            {
                // ПРЯМОЕ СРАВНЕНИЕ ПАРОЛЕЙ - КРАЙНЕ НЕБЕЗОПАСНО!
                // В реальном приложении здесь должна быть проверка хэша пароля.
                // Используйте актуальное имя свойства для Password.
                if (user.Password == password)
                {
                    return user;
                }
            }
            return null;
        }
    }
}