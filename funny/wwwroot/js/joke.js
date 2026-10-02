document.addEventListener("DOMContentLoaded", function () {
    var theme = null;

    // Читаем массив строк, который Razor передал в объект window внутри Index.cshtml
    var jokes = window.jokeAnswers || [];
    console.log("Доступно шуток из C# логики:", jokes.length, jokes);

    var generateResponse = function () {
        if (jokes.length === 0) {
            console.warn("Массив шуток пуст! Проверьте передачу данных из контроллера.");
            return;
        }

        // Генерируем случайное число (индекс) от 0 до длины массива шуток
        var randomIndex = Math.floor(Math.random() * jokes.length);
        console.log("Выбран случайный индекс массива:", randomIndex);

        // Достаем текст шутки по сгенерированному индексу
        var jokeText = jokes[randomIndex];

        // Находим элементы страницы
        var h1 = document.querySelector('h1');
        var body = document.querySelector('body');
        var h6 = document.querySelector('h6');

        // Выводим текст шутки в тег h1
        if (h1) h1.innerHTML = jokeText;

        // Ваша оригинальная логика переключения цветов (зеленый / красный)
        // Для примера привяжем зеленый цвет к четным индексам, а красный к нечетным
        var isGreenTheme = (randomIndex % 2 === 0);

        if (isGreenTheme) {
            if (body) {
                if (theme === false) {
                    body.classList.remove('red');
                    body.classList.add('green');
                }
                if (theme === null) body.classList.add('green');
            }
            theme = true;
        } else {
            if (body) {
                if (theme === true) {
                    body.classList.remove('green');
                    body.classList.add('red');
                }
                if (theme === null) body.classList.add('red');
            }
            theme = false;
        }

        // Анимация текста "Updated" (h6)
        if (h6) {
            h6.classList.toggle('not-visible');
            setTimeout(function () {
                h6.classList.toggle('not-visible');
            }, 400);
        }
    };

    // Привязываем клик строго к нашей кнопке
    var button = document.getElementById('btn-joke') || document.querySelector('button');
    if (button) {
        button.onclick = generateResponse;
    }
});
