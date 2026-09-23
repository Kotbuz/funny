using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using funny.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace funny.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StrangeDataController : ControllerBase
    {
        private static readonly List<ConnectDialogColaVM> _ColaOrdersDbInMemory = new List<ConnectDialogColaVM>();
        private static readonly List<ConnectDialogPizzaVM> _PizzaOrdersDbInMemory = new List<ConnectDialogPizzaVM>();
        private static long _ColaCurrentId = 1; 
        private static long _PizzaCurrentId = 1; 

        private readonly ILogger<StrangeDataController> _logger;

        public StrangeDataController(ILogger<StrangeDataController> logger)
        {
            _logger = logger;
        }

        [HttpPost("SendCola")]
        public IActionResult SendCola([FromBody] ConnectDialogColaVM colaVM)
        {
            if (colaVM == null)
                return BadRequest("Данные заказа не могут быть пустыми.");

            _logger.LogInformation("Получен заказ колы. Вкус: {Tasty}, Объем: {Volume}, Имя: {Name}, Телефон: {Phone}",
                colaVM.Tasty, colaVM.Volume, colaVM.Name, colaVM.Phone);

            colaVM.Id = _ColaCurrentId++;

            _ColaOrdersDbInMemory.Add(colaVM);

            return Ok(colaVM.Id);
        }
        [HttpPost("SendPizza")]
        public IActionResult SendPizza([FromBody] ConnectDialogPizzaVM pizzaVM)
        {
            if (pizzaVM == null)
                return BadRequest("Данные заказа не могут быть пустыми.");

            _logger.LogInformation("Получен заказ пиццы. Размер: {Size}, Опции: {Options}, Толщина: {Thickness}",
                pizzaVM.Size, pizzaVM.Options, pizzaVM.Thickness);

            pizzaVM.Id = _PizzaCurrentId++;

            _PizzaOrdersDbInMemory.Add(pizzaVM);

            return Ok(pizzaVM.Id);
        }
        [HttpPost("HelpPolice")]
        public IActionResult HelpPolice([FromBody] ConnectDialogPolice? request)
        {
            string userAgent = Request.Headers["User-Agent"].ToString();

            _logger.LogInformation("Поступил запрос на вызов полиции. User-Agent: {UserAgent}", userAgent);
            bool isChrome = userAgent.Contains("Chrome")
                            && !userAgent.Contains("Edg")
                            && !userAgent.Contains("OPR");
            if (isChrome)
            {
                _logger.LogWarning("Пользователь использует Chrome. Полиция выехала!");
                return Ok(new { message = "Помощь уже в пути. Полиция выехала к пользователю Chrome!" });
            }
            else
            {
                _logger.LogInformation("В помощи отказано: пользователь сидит не через Chrome.");
                return BadRequest(new { message = "Отказано в помощи. Мы помогаем только пользователям Chrome." });
            }
        }

    }
}
