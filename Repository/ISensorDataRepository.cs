using TafeWeatherStudiesAPI.Models;
using TafeWeatherStudiesAPI.Models.DTOs;

namespace TafeWeatherStudiesAPI.Repository
{
    public interface ISensorDataRepository
    {
        List<SensorData> GetAllSensorData();

        //Temp/pressure/radiation/precipitation at sensor with given date
        SensorGeneralDTO GetSensorData(string deviceName,DateTime date);
        SensorPrecipDTO Get5monthMaxPrecipData(string deviceName);
        List<SensorTempDTO> GetMaxTempData(DateTime start, DateTime end);

        //insert 1 reading
        void PostSensorData(SensorData data);
        void UpdatePrecipData(string id,double newValue);

        //void UpdateSensorData(string id,SensorData data);
        //void DeleteSensorData(string id);


        List<SensorData> SearchSensorData(string search);

        //Max temp (c) recorded at all stations for given date/time range
        //List<SensorData> MaxTempByDateRange(DateTime start,DateTime end);

        //insert multiple readings
        void PostManySensorData(List<SensorData> dataList);


        //void DeleteByDate(DateTime start,DateTime end);
        //void UpdateBasedOnDate(DateTime start,DateTime end);
    }
}
