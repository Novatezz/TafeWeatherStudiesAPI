using Notes.Models;

namespace Notes.Repository
{
    public interface IUserRepository
    {
        bool CreateUser(ApiUser user);
        void UpdateLastLogin(string apiKey);
        bool AuthenticateUser(string apiKey, params Roles[] requiredRoles);


        void DeleteUserById(string id);
        void DeleteStudentsByDate(DateTime start, DateTime end);
        void UpdateUserRoleByDate(Roles roleIn, Roles roleOut, DateTime start, DateTime end);

    }
}
