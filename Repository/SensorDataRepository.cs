using MongoDB.Bson;
using MongoDB.Driver;
using Notes.Services;
using System.Text.RegularExpressions;
using TafeWeatherStudiesAPI.Models;
using TafeWeatherStudiesAPI.Models.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TafeWeatherStudiesAPI.Repository
{
    public class SensorDataRepository : ISensorDataRepository
    {
        private readonly IMongoCollection<SensorData> _data;
        public SensorDataRepository(MongoConnectionBuilder connection)
        {
            _data = connection.GetDatabase().GetCollection<SensorData>("SensorData");
        }

        public SensorPrecipDTO Get5monthMaxPrecipData(string deviceName)
        {
            var regexTerm = Regex.Escape(deviceName);
            var builder = Builders<SensorData>.Filter;

            var filter = builder.Regex(n => n.DeviceName, new BsonRegularExpression(regexTerm, "i")) &
                         builder.Gte(n => n.Time, DateTime.Now.AddMonths(-5));
            var sort = Builders<SensorData>.Sort.Descending(n => n.Precipitation);
            var projection = Builders<SensorData>.Projection.Exclude(n => n._id)
                                                      .Include(n => n.DeviceName)
                                                      .Include(n => n.Precipitation)
                                                      .Include(n => n.Time);

            return _data.Aggregate().Match(filter).Sort(sort).Project<SensorPrecipDTO>(projection).FirstOrDefault();
        }

        public List<SensorData> GetAllSensorData()
        {
            throw new NotImplementedException();
        }

        public List<SensorTempDTO> GetMaxTempData(DateTime start, DateTime end)
        {
            var collection = _data.AsQueryable();
            var results = collection.OrderByDescending(n => n.Temperature)
                                    .Where(n => n.Time > start && n.Time < end)
                                    .GroupBy(n => n.DeviceName)
                                    .Select(n => new SensorTempDTO
                                    {
                                        DeviceName = n.First().DeviceName,
                                        Temperature = n.First().Temperature,
                                        Time = n.First().Time
                                    }).ToList();

            return results;
        }

        public SensorGeneralDTO GetSensorData(string deviceName, DateTime date)
        {
            var regexTerm = Regex.Escape(deviceName);
            var builder = Builders<SensorData>.Filter;

            var filter = builder.Regex(n => n.DeviceName, new BsonRegularExpression(regexTerm, "i")) &
                         builder.Gte(n => n.Time ,date.AddMinutes(-1)) &
                         builder.Lte(n => n.Time, date.AddMinutes(1));
            var projection = Builders<SensorData>.Projection.Exclude(n => n._id)
                                                      .Include(n => n.DeviceName)
                                                      .Include(n => n.Temperature)
                                                      .Include(n => n.AtmosphericPressure)
                                                      .Include(n => n.SolarRadiation)
                                                      .Include(n => n.Precipitation)
                                                      .Include(n => n.Time);

            return _data.Aggregate().Match(filter).Project<SensorGeneralDTO>(projection).FirstOrDefault();
        }

        public void PostManySensorData(List<SensorData> dataList)
        {
            _data.InsertMany(dataList);
        }

        public void PostSensorData(SensorData data)
        {
            _data.InsertOne(data);
        }

        public List<SensorData> SearchSensorData(string search)
        {
            throw new NotImplementedException();
        }

        public void UpdatePrecipData(string id, double newValue)
        {
            ObjectId objId = ObjectId.Parse(id);
            var filter = Builders<SensorData>.Filter.Eq(n => n._id, objId);
            var builder = Builders<SensorData>.Update;
            var update = builder.Set(n => n.Precipitation, newValue);
            _data.UpdateOneAsync(filter, update);
        }
    }
}
