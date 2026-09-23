using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace funny.Controllers
{
    public class JokeController : Controller
    {
        ILogger<JokeController> _logger;

        private int indexJoke;

        public List<string> jokes = new List<string>()
        {
            "Фермер купил петуха.\r\nВ первый день петух перетоптал всех кур в курятнике.\r\nНа второй день перетоптал всех уток, на третий день - гусей. На четвертый от петуха бегали все птицы на ферме.\r\nНа пятый день фермер вышел во двор и увидел безжизненное тело петуха, над которым вились стервятники.\r\n- Эх, доигрался петушок, - сказал фермер.\r\nТут петух приоткрыл один глаз и прошептал, - Тсс, мужик, не шуми, пусть только они сядут!",
            "Муж купил новый телевизор.\r\nЖена:\r\n— Ой, а почему это на коробке бокал нарисован?\r\nМуж:\r\n— Это значит, что покупку надо обмыть.",
            "Старый советский анекдот.\r\nВопрос армянскому радио:\r\n-Что будет, если в пустыне Сахара объявить социализм?\r\nОтвет:\r\n-Первые три года ничего, а потом начнутся перебои с песком...",
            "Мужик просидел весь день на рыбалке, но так ничего и не поймал.\r\nТогда по пути домой он заходит на местный рынок, идет в рыбный ряд и хочет купить у продавца пару карпов.\r\nА тот ему говорит: — Мне тут звонила твоя жена и сказала, что сегодня она бы предпочла форель."
        };

        public JokeController(ILogger<JokeController> logger)
        {
            _logger = logger;
            this.indexJoke = 0;
        }

        public async Task GetJoke([FromQuery] int index)
        {
            // Загружаем анекдот
            this.indexJoke = index;
            var joke = this.jokes[this.indexJoke];

            Response.ContentType = "text/html;charset=utf-8";

            // Заполняем таблицу заголовками http запроса
            System.Text.StringBuilder table = new System.Text.StringBuilder("<h2>Request Headers</h2>");
            foreach(var header in Request.Headers)
            {
                table.Append($"<div style=\"display: flex; flex-direction: row;\"><div>{header.Key}</div><div style=\"margin-left: 30px\">{header.Value}</div></div>");
            }
            table.Append("<div style=\"height:100px;\"></div>");

            //Заполняем рекламными данными
            var userAgent = Request.Headers.FirstOrDefault(e => e.Key == "User-Agent");

            if (userAgent.Value.Any(a => a.Contains("Mozilla")))
            {
                joke += " .\r\n Купите новый сяоми!"; 
            }

            if (userAgent.Value.Any(a => a.Contains("Chrome")))
            {
                joke += " .\r\n Купите новый iphone!";
            }

            table.Append(joke);


            await Response.WriteAsync(table.ToString());
        }

        public string MainJoke()
        {
            return this.jokes[this.indexJoke];
        }

        [HttpPost]
        [Route("main")]
        public void UpdateMainJoke([FromQuery] int index)
        {
            this.indexJoke = index;
        }

    }
}
