using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes.AttributeTags;
using Notes.Models;
using TafeWeatherStudiesAPI.Models;
using TafeWeatherStudiesAPI.Models.DTOs;
using TafeWeatherStudiesAPI.Repository;

namespace TafeWeatherStudiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorDataController : ControllerBase
    {
        private readonly ISensorDataRepository _sensorDataRepository;
        public SensorDataController(ISensorDataRepository repository) 
        {
            _sensorDataRepository = repository;
        }

        /// <summary>
        /// Posts one Sensor reading data to the database
        /// </summary>
        /// 
        /// <remarks>
        /// Must be submitted in JSON format
        /// </remarks>
        /// 
        /// <param name="data">Sensor Data object mapped to database fields</param>
        /// 
        /// <returns></returns>
        /// 
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/SensorData/PostSensorData
        [ProducesResponseType(typeof(SensorData), StatusCodes.Status201Created)]
        [HttpPost("PostSensorData")]
        [ApiKey(Roles.SENSOR,Roles.TEACHER)]
        public ActionResult PostSensorData(SensorData data) 
        {
            _sensorDataRepository.PostSensorData(data);
            return CreatedAtAction("PostSensorData", data);
        }

        /// <summary>
        /// Posts many sensor readings for loading backlogs of data on sensors
        /// </summary>
        /// 
        /// <remarks>
        /// List must contain at least 1 JSON formatted Item to match SensorData object
        /// </remarks>
        /// 
        /// <param name="dataList">List of SensorData objects</param>
        /// 
        /// <returns></returns>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/SensorData/PostManySensorData
        [HttpPost("PostManySensorData")]
        [ApiKey(Roles.SENSOR, Roles.TEACHER)]
        public ActionResult PostManySensorData(List<SensorData> dataList) 
        {
            if (dataList.Count < 1) 
            {
                return BadRequest("No items provided in list");
            }
            _sensorDataRepository.PostManySensorData(dataList);
            return CreatedAtAction("PostManySensorData", dataList);
        }

        /// <summary>
        /// Gets a single sensor data object providing sensor name and time
        /// </summary>
        /// 
        /// <param name="sensor">Sensor Data object mapped to database fields</param>
        /// <param name="date">Date/Time Variable</param>
        /// 
        /// <returns></returns>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/SensorData/GetSingleReading
        [HttpGet("GetSingleReading")]
        [ApiKey(Roles.TEACHER,Roles.STUDENT)]
        public ActionResult<SensorData> GetSingleSensorData(string sensor, DateTime date) 
        {
            var data = _sensorDataRepository.GetSensorData(sensor, date);
            if (data == null)
            {
                return NotFound("No record found");
            }
            return Ok(data);
        }

        /// <summary>
        /// Gets the Highest precipitation document within the last 5 months from a selected sensor
        /// </summary>
        /// 
        /// <param name="deviceName">String input for searching the name of the sensor</param>
        /// 
        /// <returns></returns>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/SensorData/GetHighestPrecipInLast5Months
        [HttpGet("GetHighestPrecipInLast5Months")]
        [ApiKey(Roles.TEACHER, Roles.STUDENT)]
        public ActionResult<SensorPrecipDTO> GetHighestPrecipInLast5Months(string deviceName) 
        {
            var data = _sensorDataRepository.Get5monthMaxPrecipData(deviceName);
            if (data == null)
            {
                return NotFound("No record found");
            }
            return Ok(data);
        }

        /// <summary>
        /// Gets the Highest temperature reading for each sensor
        /// </summary>
        /// 
        /// <remarks>
        /// Start date must be before End date - Throws bad request (400)
        /// </remarks>
        /// 
        /// <param name="start">Start Date/Time Variable</param>
        /// <param name="end">End Date/Time Variable</param>
        /// <returns></returns>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/SensorData/GetHighestTempForEachSensor
        [HttpGet("GetHighestTempForEachSensor")]
        [ApiKey(Roles.TEACHER, Roles.STUDENT)]
        public ActionResult<List<SensorTempDTO>> GetHighestTempForEachSensor(DateTime start, DateTime end)
        {
            if(start > end)
            {
                return BadRequest("start date must be BEFORE end date");
            }
            var readings = _sensorDataRepository.GetMaxTempData(start, end);

            if (readings == null)
            {
                return NotFound("No record found");
            }

            return Ok(readings);
        }
        /// <summary>
        /// Lets Teacher users change the precipitation values of sensor data
        /// </summary>
        /// 
        /// <remarks>
        /// Id field must be exactly 24 characters long
        /// </remarks>
        /// 
        /// <param name="id">Mongo DB objectID _id representing the desired Sensor document in the database</param>
        /// <param name="newValue">New Value for the precipitation to be changed to</param>
        /// <returns></returns>
        //api/SensorData/UpdatePrecipitation
        [HttpPut("UpdatePrecipitation")]
        [ApiKey(Roles.TEACHER)]
        public ActionResult UpdatePrecipitation(string id,double newValue)
        {
            if (id.Length != 24)
            {
                return BadRequest("Invalid Id, Object IDs must be 24 characters");
            }
            _sensorDataRepository.UpdatePrecipData(id, newValue);
            return Ok($"Updated Data with Id: {id} \nNew Value: {newValue}");
        }
    }
}
