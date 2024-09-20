using MongoDB.Bson.Serialization.Attributes;

namespace TafeWeatherStudiesAPI.Models.DTOs
{
    public class SensorPrecipDTO
    {
        [BsonElement("Device Name")]
        public string DeviceName { get; set; }

        [BsonElement("Precipitation mm/h")]
        public double Precipitation { get; set; }

        public DateTime Time { get; set; }

    }
}
