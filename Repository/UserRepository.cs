using MongoDB.Bson;
using MongoDB.Driver;
using Notes.Models;
using Notes.Services;

namespace Notes.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<ApiUser> _users;
        public UserRepository(MongoConnectionBuilder connection)
        {
            _users = connection.GetDatabase().GetCollection<ApiUser>("ApiUsers");
        }
        public bool AuthenticateUser(string apiKey, params Roles[] requiredRoles)
        {
            var filter = Builders<ApiUser>.Filter.Eq(u => u.ApiKey, apiKey);
            var user = _users.Find(filter).FirstOrDefault();
            if (user == null) 
            {
                return false;
            }
            if(IsAllowedRole(user.Role, requiredRoles) == false) 
            {
                return false;
            }
            return true;
        }

        public bool CreateUser(ApiUser user)
        {
            var filter = Builders<ApiUser>.Filter.Eq(u => u.Email, user.Email);
            var existingUser = _users.Find(filter).FirstOrDefault();
            if (existingUser != null)
            {
                return false;
            }

            user.ApiKey = Guid.NewGuid().ToString();
            user.Created = DateTime.UtcNow;
            user.LastAccess = DateTime.UtcNow;

            _users.InsertOne(user);
            return true;
        }

        public void DeleteStudentsByDate(DateTime start, DateTime end)
        {
            var builder = Builders<ApiUser>.Filter;
            var filter = builder.Gte(n => n.LastAccess, start) &
                         builder.Lte(n => n.LastAccess, end) &
                         builder.Where(n => n.Role == Roles.STUDENT.ToString());

            _users.DeleteManyAsync(filter);
        }

        public void DeleteUserById(string id)
        {
            ObjectId objId = ObjectId.Parse(id);
            var filter = Builders<ApiUser>.Filter.Eq(n => n._id, objId);
            _users.DeleteOneAsync(filter);
        }

        public void UpdateLastLogin(string apiKey)
        {
            var currentDate = DateTime.UtcNow;
            var filter = Builders<ApiUser>.Filter.Eq(u => u.ApiKey, apiKey);

            var update = Builders<ApiUser>.Update.Set(u => u.LastAccess, currentDate);
            _users.UpdateOne(filter, update);
        }

        public void UpdateUserRoleByDate(Roles roleIn, Roles roleOut, DateTime start, DateTime end)
        {
            var builder = Builders<ApiUser>.Filter;
            var filter = builder.Gte(n => n.Created, start) &
                         builder.Lte(n => n.Created, end) &
                         builder.Where(n => n.Role == roleIn.ToString());
            var update = Builders<ApiUser>.Update.Set(n => n.Role, roleOut.ToString());
            _users.UpdateMany(filter, update);
        }


        private bool IsAllowedRole(string userRole, Roles[] allowedRoles) 
        {
            if(!Enum.TryParse(userRole, out Roles userRoleType)) 
            {
                return false;            
            }
            foreach(var role in allowedRoles) 
            {
                if (userRoleType.Equals(role)) 
                {
                    return true;
                }
            }
            return false;
        }
    }
}
