using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using testApi.Data;
using testApi.Utilities;

namespace testApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly MyDbContext _dbContext;

        public CourseController(IConfiguration config, MyDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }


        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourse(CancellationToken ct)
        {

            // try
            // {
            //     var ok = await _dbContext.Database.CanConnectAsync();
            //     return Ok(new { db = ok });
            // }
            // catch (Exception ex)
            // {
            //     throw ex;
            // }


            try
            {
                string baseConn = _config.GetConnectionString("Sql");
                using SqlConnection conn = await SqlConnectionFactory.CreateAsync(baseConn, ct);

                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT TOP (1) name FROM sys.tables ORDER BY name";
                var name = (string?)await cmd.ExecuteScalarAsync(ct);

                return Ok(new { table = name ?? "(none)" });
            }
            catch (System.Exception)
            {

                throw;
            }


        }

    }
}
