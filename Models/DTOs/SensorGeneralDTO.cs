using MongoDB.Bson.Serialization.Attributes;

namespace TafeWeatherStudiesAPI.Models.DTOs
{
    public class SensorGeneralDTO
    {
        [BsonElement("Device Name")]
        public string DeviceName { get; set; }

        [BsonElement("Precipitation mm/h")]
        public double Precipitation { get; set; }

        public DateTime Time { get; set; }

        [BsonElement("Temperature (°C)")]
        public double Temperature { get; set; }

        [BsonElement("Atmospheric Pressure (kPa)")]
        public double AtmosphericPressure { get; set; }

        [BsonElement("Solar Radiation (W/m2)")]
        public double SolarRadiation { get; set; }
    }
}
