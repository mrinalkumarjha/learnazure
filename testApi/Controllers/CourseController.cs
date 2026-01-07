using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using testApi.Models;
using testApi.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace testApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly SqlServerExample _sqlServer;

        public CourseController(IConfiguration config, SqlServerExample sqlServer)
        {
            _config = config;
            _sqlServer = sqlServer;
        }


        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourse(CancellationToken ct)
        {

            try
            {
                string sql = "SELECT * from  Course";
                var course = await _sqlServer.ExecuteQueryAsync<Course>(sql);
                return Ok(course);
            }
            catch (System.Exception)
            {

                throw;
            }


        }


        [HttpPost("course")]
        public async Task<IActionResult> AddCourse(string courseName, string courseDesc)
        {

            try
            {
                const string sql = @"INSERT INTO Course (Name, Description) VALUES (@Name, @Description)";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Name", courseName ?? (object)DBNull.Value),
                    new SqlParameter("@Description", courseDesc ?? (object)DBNull.Value)
                };

                var rows = await _sqlServer.ExecuteNonQueryAsync(sql, parameters);

                if (rows == 0)
                    return BadRequest("Insert failed");

                return Ok("Course inserted successfully");
            }
            catch (System.Exception)
            {

                throw;
            }


        }

        [HttpDelete("course")]
        public async Task<IActionResult> DeleteCourse(int courseId)
        {

            try
            {
                const string sql = @"Delete from Course where courseId = @courseId";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@courseId", courseId)
                };

                var rows = await _sqlServer.ExecuteNonQueryAsync(sql, parameters);

                if (rows == 0)
                    return BadRequest("Delete failed");

                return Ok("Course deleted successfully");
            }
            catch (System.Exception)
            {

                throw;
            }


        }

    }
}
