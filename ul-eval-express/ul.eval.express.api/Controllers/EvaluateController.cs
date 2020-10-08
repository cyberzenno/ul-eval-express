using Microsoft.AspNetCore.Mvc;
using ul.eval.express.data;

namespace ul.eval.express.api.Controllers
{
    [ApiController]
    public class EvaluateController : ControllerBase
    {
        private EvalService _evalSerice;

        public EvaluateController()
        {
            _evalSerice = new EvalService();
        }

        [Route("api/pulse")]
        public IActionResult GetPulse()
        {
            return Ok("It's Alive!");
        }

        [Route("api/evaluate")]
        public IActionResult Post([FromBody] string expression)
        {
            var cleanExpression = _evalSerice.CleanExpression(expression);

            if (string.IsNullOrEmpty(cleanExpression))
            {
                return BadRequest("Unable to extract expression");
            }

            var result = _evalSerice.Evaluate(cleanExpression);

            return new JsonResult(result);
        }
    }
}
