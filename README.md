# VKPostAnalyzer

Данный проект представляет собой веб-приложение, разработанное для сбора и анализа последних постов из социальной сети «ВКонтакте». Приложение получает 5 последних постов с личной страницы пользователя, подсчитывает вхождение одинаковых букв в тексте постов (сравнение осуществляется без учета регистра) и сортирует результаты по алфавиту. Все результаты сохраняются в базе данных PostgreSQL. Также в приложении реализована система логирования для отслеживания начала и окончания подсчета.

## Установка и запуск

### Предварительные требования

- установить Docker (отлично подойдет Docker Desktop)
- создать Ключи доступа в dev.vk.com

### Как создать ключ
- заходим на dev.vk.com
- создаем приложение
- в разделе Разработка/Ключи доступа можно создать "сервисный ключ" который позволит обращаться к API vk
![alt text](https://github.com/lehakurshev/VKPostAnalyzer/blob/main/pictures/1.png)

### Шаги установки

1. **Клонировать репозиторий:**
    ```
    git clone https://github.com/lehakurshev/VKPostAnalyzer.git
    cd <папка_проекта>
    ```

2. **Запустить приложение:**
    ```
    docker-compose up --build
    ```

3. **Можно и без Docker**
- создаем PostgreSQL бд с названием "vkanalyticsdb"
- в корневой папке `VkPostAnalyzer` создать файл .env с содержимым
    ```
    DB_HOST=localhost
    DB_PORT=5432
    DB_NAME=vkanalyticsdb
    DB_USER_NAME=postgres
    DB_PASSWORD=postgres
    API_VERSION=v3
    ```
- запускаем .net приложение как вам удобно

### Использование Swagger

- После запуска приложения, доступ к UI для взаимодействия с backend частью осуществляется через Swagger. Откройте браузер и перейдите по адресу:
    ```
    http://localhost:8080
    ```
![alt text](https://github.com/lehakurshev/VKPostAnalyzer/blob/main/pictures/2.png)


## Описание функциональности

- всего один endpoint
- вводим access_token ("сервисный ключ" который который получили в dev.vk.com)
- вводим id пользователя посты которого хотим посмотреть (напиример id самого стабильного паблика ```botay_suka```)

## Логирование

Лог-файл создается в \VkPostAnalyzer\WebApi\logs

## Тесты

чтоб запустиь тесты необходимо в корневой папке создать файл .env с содержимым `ACCESS_TOKEN={токен который мы получили в dev.vk.com}`