using Microsoft.EntityFrameworkCore;
using SKAT_Interface.Data;
using SKAT_Interface.Models.DbUsersModels;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SKAT_Interface.Shared.SessionListComponent; // Для TestSessionView и SessionStatus enum

namespace SKAT_Interface.Services
{
    public class SessionService
    {
        private readonly DbUsersContext _dbContext; // Единый DbContext

        public SessionService(DbUsersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TestSessionView>> GetSessionsForUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal.Identity == null || !claimsPrincipal.Identity.IsAuthenticated)
            {
                return new List<TestSessionView>();
            }

            var userName = claimsPrincipal.Identity.Name; // Это Username/Login
            // Получаем пользователя вместе с его GroupId, но саму Group можно не грузить здесь, если она не нужна дальше
            var user = await _dbContext.Users
                                 .Select(u => new { u.Login, u.Role, u.GroupId }) // Выбираем только нужные поля
                                 .FirstOrDefaultAsync(u => u.Login == userName);

            if (user == null)
            {
                return new List<TestSessionView>();
            }

            IQueryable<Session> sessionsQuery;

            if (claimsPrincipal.IsInRole("Преподаватель"))
            {
                // Преподаватель видит все сессии
                sessionsQuery = _dbContext.Sessions;
            }
            else if (claimsPrincipal.IsInRole("Обучающийся"))
            {
                // Обучающийся видит сессии, назначенные его группе
                var userGroupId = user.GroupId;

                // Используем навигационное свойство Groups в Session,
                // которое EF Core должен разрешить через связующую таблицу GroupSessions
                sessionsQuery = _dbContext.Sessions
                    .Where(s => s.Groups.Any(g => g.GroupId == userGroupId));
            }
            else
            {
                return new List<TestSessionView>();
            }

            var sessions = await sessionsQuery
                                    .OrderByDescending(s => s.DateStart) // Используем DateStart из вашей модели Session
                                    .Select(s => new TestSessionView
                                    {
                                        Id = s.SessionId, // Используем SessionId
                                        Name = $"{s.SessionType ?? "Сессия"} от {s.DateStart:dd.MM.yyyy}", // Пример генерации имени
                                        // Статус сессии - это отдельный вопрос.
                                        // Ваша модель Session не имеет поля Status.
                                        // Его нужно будет вычислять или добавить в модель Session.
                                        // Пока поставим NotStarted как заглушку.
                                        Status = DetermineSessionStatus(s) // Нужен метод для определения статуса
                                    })
                                    .ToListAsync();
            return sessions;
        }

        // Пример метода для определения статуса сессии
        // Адаптируйте эту логику под ваши бизнес-правила
        private SKAT_Interface.Shared.SessionListComponent.SessionStatus DetermineSessionStatus(Session session)
        {
            var now = DateTime.UtcNow; // Или DateTime.Now, если работаете с локальным временем
            if (now < session.DateStart)
            {
                return SKAT_Interface.Shared.SessionListComponent.SessionStatus.NotStarted;
            }
            else if (now >= session.DateStart && now <= session.DateFinish)
            {
                // Здесь можно добавить проверку, начал ли пользователь сессию, если есть такая информация
                return SKAT_Interface.Shared.SessionListComponent.SessionStatus.InProgress; // Или NotStarted, если не начал
            }
            else // now > session.DateFinish
            {
                // Здесь можно добавить проверку, завершил ли пользователь сессию
                return SKAT_Interface.Shared.SessionListComponent.SessionStatus.Completed; // Или InProgress, если не завершил вовремя
            }
        }
    }
}