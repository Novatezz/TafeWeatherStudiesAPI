using MongoDB.Bson.Serialization.Attributes;

namespace TafeWeatherStudiesAPI.Models.DTOs
{
    public class SensorTempDTO
    {
        [BsonElement("Device Name")]
        public string DeviceName { get; set; }
       
        public DateTime Time { get; set; }

        [BsonElement("Temperature (°C)")]
        public double Temperature { get; set; }
    }
}
