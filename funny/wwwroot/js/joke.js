document.addEventListener("DOMContentLoaded", function () {
    var theme = null;

    // Находим именно нашу кнопку на странице
    var button = document.getElementById('btn-joke') || document.querySelector('button');

    if (button) {
        button.onclick = function () {
            var random = Math.round(Math.random());
            var h1 = document.querySelector('h1');
            var body = document.querySelector('body');
            var h6 = document.querySelector('h6');

            if (random) {
                if (h1) h1.innerHTML = 'YES';
                if (body) {
                    if (theme === false) {
                        body.classList.remove('red');
                        body.classList.add('green');
                    }
                    if (theme === null) body.classList.add('green');
                }
                theme = true;
            } else {
                if (h1) h1.innerHTML = 'NO';
                if (body) {
                    if (theme === true) {
                        body.classList.remove('green');
                        body.classList.add('red');
                    }
                    if (theme === null) body.classList.add('red');
                }
                theme = false;
            }

            if (h6) {
                h6.classList.toggle('not-visible');
                setTimeout(function () {
                    h6.classList.toggle('not-visible');
                }, 400);
            }
        };
    }
});
