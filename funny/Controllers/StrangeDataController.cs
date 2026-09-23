using funny.Data;
using funny.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
        private readonly AppDbContext _context; 

        public StrangeDataController(ILogger<StrangeDataController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
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




        [HttpPost("CreateService")]
        public async Task<IActionResult> CreateService([FromBody] DigitalService service)
        {
            if (service == null) return BadRequest("Данные услуги пусты.");

            _context.DigitalServices.Add(service);
            await _context.SaveChangesAsync();
            return Ok(service);
        }

        [HttpPut("UpdateService/{id}")]
        public async Task<IActionResult> UpdateService(long id, [FromBody] DigitalService updatedService)
        {
            var service = await _context.DigitalServices.FindAsync(id);
            if (service == null) return NotFound("Услуга не найдена.");

            service.Name = updatedService.Name;
            service.Description = updatedService.Description;
            service.Price = updatedService.Price;

            await _context.SaveChangesAsync();
            return Ok(service);
        }

        [HttpDelete("DeleteService/{id}")]
        public async Task<IActionResult> DeleteService(long id)
        {
            var service = await _context.DigitalServices.FindAsync(id);
            if (service == null) return NotFound("Услуга не найдена.");

            _context.DigitalServices.Remove(service);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Услуга с ID {id} успешно удалена." });
        }

        [HttpGet("GetAllServicesJson")]
        public async Task<IActionResult> GetAllServicesJson()
        {
            var services = await _context.DigitalServices.ToListAsync();
            return Ok(services);
        }


        // Маршрут: /StrangeData/GetDigitalList
        [HttpGet("GetDigitalList")]
        public async Task<IActionResult> GetDigitalList([FromQuery] DigitalListRequest request)
        {
            try
            {
                IQueryable<DigitalService> query = _context.DigitalServices;

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    string filterLower = request.Filter.ToLower();
                    query = query.Where(s => s.Name.ToLower().Contains(filterLower)
                                          || s.Description.ToLower().Contains(filterLower));
                }

                query = request.Sorted?.ToLower() switch
                {
                    "name_desc" => query.OrderByDescending(s => s.Name),
                    "price_asc" => query.OrderBy(s => s.Price),
                    "price_desc" => query.OrderByDescending(s => s.Price),
                    _ => query.OrderBy(s => s.Name) 
                };

                var services = await query.ToListAsync();

                var htmlBuilder = new StringBuilder();
                htmlBuilder.Append("<ul class='digital-services-list'>");

                if (services.Count == 0)
                {
                    htmlBuilder.Append("<li class='no-data'>Услуги не найдены</li>");
                }
                else
                {
                    foreach (var service in services)
                    {
                        htmlBuilder.Append($"<li class='service-item' data-id='{service.Id}'>");
                        htmlBuilder.Append($"  <h3 class='service-name'>{service.Name}</h3>");
                        htmlBuilder.Append($"  <p class='service-description'>{service.Description}</p>");
                        htmlBuilder.Append($"  <span class='service-price'>Цена: {service.Price.ToString("F2", CultureInfo.InvariantCulture)} руб.</span>");
                        htmlBuilder.Append("</li>");
                    }
                }

                htmlBuilder.Append("</ul>");

                return Content(htmlBuilder.ToString(), "text/html; charset=utf-8");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при генерации HTML списка услуг");
                return StatusCode(500, $"Внутренняя ошибка сервера: {ex.Message}");
            }
        }





    }
}
