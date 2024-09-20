using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Notes.Models
{
    public class ApiUser
    {
        [BsonId]
        [JsonIgnore]
        public ObjectId _id { get; set; }
        public string ObjId => _id.ToString();
        public string UserName {  get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role {  get; set; } = Roles.STUDENT.ToString();
        public string ApiKey { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime LastAccess { get; set; }
    }
}
