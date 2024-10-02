using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicaShoppingAppApi.Data.Implementation
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IAppDbContext _appDbContext;

        public AuthRepository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public User ValidateUser(string username)
        {
            User? user = _appDbContext.Users.FirstOrDefault(c => c.LoginId.ToLower() == username.ToLower() || c.Email == username.ToLower());
            return user;
        }

        public IEnumerable<User> GetAllUsers(int page, int pageSize, string? search, string sortOrder)
        {
            int skip = (page - 1) * pageSize;
            IQueryable<User> query = _appDbContext.Users
                .Include(c => c.SecurityQuestion)
                 .Where(c => !c.IsAdmin);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.LoginId.Contains(search));
            }

            switch (sortOrder.ToLower())
            {
                case "asc":
                    query = query.OrderBy(c => c.Name).ThenBy(c => c.LoginId);
                    break;
                case "desc":
                    query = query.OrderByDescending(c => c.Name).ThenByDescending(c => c.LoginId);
                    break;
                default:
                    query = query.OrderBy(c => c.Name);
                    break;
            }

            return query
                .Skip(skip)
                .Take(pageSize)
                .ToList();
        }

        public int TotalUsers(string? search)
        {
            IQueryable<User> query = _appDbContext.Users.Where(c => !c.IsAdmin);
            


            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.LoginId.Contains(search));
            }

            return query.Count();
        }
  

        public User? GetUser(int id)
        {
            var user = _appDbContext.Users
                .FirstOrDefault(c => c.UserId == id);
            return user;
        }

        public bool DeleteUser(int id)
        {
            var result = false;
            var contact = _appDbContext.Users.Find(id);
            if (contact != null)
            {
                _appDbContext.Users.Remove(contact);
                _appDbContext.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool RegisterUser(User user)

        {
            var result = false;
            if (user != null)
            {
                _appDbContext.Users.Add(user);
                _appDbContext.SaveChanges();

                return true;
            }
            return result;
        }

        public bool UserExists(string loginId, string email)
        {
            if (_appDbContext.Users.Any(c => c.LoginId.ToLower() == loginId.ToLower() || c.Email.ToLower() == email.ToLower()))
            {
                return true;
            }
            return false;

        }

        public IEnumerable<SecurityQuestion> GetAllSecurityQuestions()
        {
            var allQuestions = _appDbContext.SecurityQuestions.ToList();
            return allQuestions;
        }
        public bool UpdateUser(User user)
        {
            var result = false;
            if (user != null)
            {
                _appDbContext.Users.Update(user);
                _appDbContext.SaveChanges();
                result = true;
            }
            return result;
        }
    }
}
