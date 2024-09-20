using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notes.AttributeTags;
using Notes.Models;
using Notes.Models.DTOs;
using Notes.Repository;
using TafeWeatherStudiesAPI.Models.DTOs;

namespace Notes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository repository)
        {
            _userRepository = repository;
        }

        /// <summary>
        /// Adds a Single User to the Mongo Database
        /// </summary>
        /// 
        /// <remarks>
        /// Only accessable by the Teacher Role
        /// Requires an email that is not in the system already
        /// Role must be TEACHER/STUDENT/SENSOR
        /// </remarks>
        /// 
        /// <param name="userDto">DTO Maps the user input against the User Database object 
        /// and adds missing info before committing to the database</param>
        /// <returns></returns>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/Users/AddUser
        [HttpPost("AddUser")]
        [ApiKey(Roles.TEACHER)]
        public IActionResult PostUser(UserDTO userDto) 
        {            
            if (!Enum.TryParse(userDto.Role.ToUpper(), out Roles userRoleType)) 
            {
                return BadRequest("Invalid USER ROLE provided.");
            }
            var user = new ApiUser 
            { 
                UserName = userDto.UserName,
                Email = userDto.Email,
                Role = userRoleType.ToString() 
            };
            var result = _userRepository.CreateUser(user);
            
            if (result == false) 
            {
                return BadRequest("Error. A user with this email already exists");
            }
            return CreatedAtAction("PostUser",user);
        }
        /// <summary>
        /// Deletes one user based on provided ID
        /// </summary>
        /// 
        /// <remarks>
        /// Id field must be exactly 24 characters long
        /// </remarks>
        /// 
        /// <param name="id">Mongo DB objectID _id representing the desired user document in the database</param>
        /// <returns></returns>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/Users/DeleteUserById
        [HttpDelete("DeleteUserById")]
        [ApiKey(Roles.TEACHER)]
        public ActionResult  DeleteUser(string id) 
        {
            if (id.Length != 24)
            {
                return BadRequest("Invalid Id, Object IDs must be 24 characters");
            }
            _userRepository.DeleteUserById(id);
            return Ok();
        }
        /// <summary>
        /// Deletes All students within the provided Date range
        /// </summary>
        /// 
        /// <remarks>
        /// Start date must be before End date - Throws bad request (400)
        /// </remarks>
        /// 
        /// <param name="start">Date/Time Must be provided</param>
        /// <param name="end">Date/Time Must be provided</param>
        /// 
        /// <returns></returns>
        /// 
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/Users/DeleteStudentsByDate
        [HttpDelete("DeleteStudentsByDate")]
        [ApiKey(Roles.TEACHER)]
        public ActionResult DeleteManyStudents(DateTime start, DateTime end) 
        {
            if (start > end)
            {
                return BadRequest("start date must be BEFORE end date");
            }
            _userRepository.DeleteStudentsByDate(start,end);
            return Ok();
        }
        /// <summary>
        /// Changes The Provided user role with another role for all users of that role between a date range
        /// </summary>
        /// 
        /// <remarks>
        /// Both Roles input by user must be different and match system defined roles
        /// Start date provided must be before end date provided
        /// </remarks>
        /// 
        /// <param name="roleIn">Role to be changed. Role objects defined by the enum(TEACHER/STUDENT/SENSOR)</param>
        /// <param name="roleOut">Role to be changed to. Role objects defined by the enum(TEACHER/STUDENT/SENSOR)</param>
        /// <param name="start">Date/Time Must be provided</param>
        /// <param name="end">Date/Time Must be provided</param>
        /// 
        /// <returns></returns>
        /// 
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorised</response>
        /// <response code="403">Access Denied</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        //api/Users/ChangeUsersRoleByDate
        [HttpPatch("ChangeUsersRoleByDate")]
        [ApiKey(Roles.TEACHER)]
        public ActionResult ChangeUsersRoleByDate(string roleIn, string roleOut, DateTime start, DateTime end) 
        {
            if (!Enum.TryParse(roleIn.ToUpper(), out Roles userRoleTypeIn))
            {
                return BadRequest($"Invalid USER ROLE provided.({roleIn} is not an allowed role).");
            }
            if (!Enum.TryParse(roleOut.ToUpper(), out Roles userRoleTypeOut))
            {
                return BadRequest($"Invalid USER ROLE provided.({roleOut} is not an allowed role).");
            }
            if (roleIn.Equals(roleOut))
            {
                return BadRequest($"Invalid input: Roles Match.");
            }
            if (start > end)
            {
                return BadRequest("Start date must be BEFORE end date.");
            }
            _userRepository.UpdateUserRoleByDate(userRoleTypeIn, userRoleTypeOut, start, end);
            return Ok();
        }
        
    }
}
