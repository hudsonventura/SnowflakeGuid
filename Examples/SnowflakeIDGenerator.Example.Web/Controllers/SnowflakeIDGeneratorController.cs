using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SnowflakeID.Example.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SnowflakeIDGeneratorController : ControllerBase
    {
        private readonly ISnowflakeIDGenerator _snowflakeIDGenerator;

        public SnowflakeIDGeneratorController(ISnowflakeIDGenerator snowflakeIDGenerator)
        {
            _snowflakeIDGenerator = snowflakeIDGenerator;
        }

        [HttpGet]
        [Route("[action]")]
        public ulong GetCode()
        {
            return _snowflakeIDGenerator.GetCode();
        }

        [HttpGet]
        [Route("[action]")]
        public string GetCodeString()
        {
            return _snowflakeIDGenerator.GetCodeString();
        }

        [HttpGet]
        [Route("[action]")]
        public Snowflake GetSnowflake()
        {
            return _snowflakeIDGenerator.GetSnowflake();
        }

        [HttpGet]
        [Route("[action]")]
        public IEnumerable<Snowflake> GetSnowflakes([FromQuery, BindRequired] int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                yield return _snowflakeIDGenerator.GetSnowflake();
            }
        }

        [HttpGet]
        [Route("[action]")]
        public object GetConfiguration()
        {
            return new
            {
                _snowflakeIDGenerator.ConfiguredMachineId,
                _snowflakeIDGenerator.ConfiguredEpoch,
            };
        }
    }
}
