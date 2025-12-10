KursV - Веб-сервис управления курсами валют.

Данный сервис избавляет пользователей от необходимости постоянно вручную проверять курсы валют на сайтах банков или в приложениях. 
Сервис решает задачу централизованного хранения и управления актуальными курсами валют. 
Он позволяет:
- Хранить актуальные курсы валют относительно базовой валюты (RUB)
- Быстро получать информацию о текущих курсах
- Динамически обновлять курсы валют

Сервис предназначен для людей, которые хотят купить валюту по лучшему курсу.

СТРУКТУРА ПРОЕКТА:
KursV/

├── Controllers/                     # HTTP контроллеры

│   ├── ExchangeController.cs

│   └── UserController.cs

├── Models/                          # Модели данных

│   ├── ExchangeRate.cs

│   ├── AddRateRequest.cs

│   └── UserInfo.cs

├── Services/                         # Бизнес-логика

│   ├── IExchangeRateService.cs

│   └── ExchangeRateService.cs

├── Program.cs                        # Точка входа

└── appsettings.json                  # Конфигурация

API ЭНДПОИНТЫ

Модуль Exchange (/api/Exchange)

GET	/api/Exchange	- Получить все курсы валют

GET	/api/Exchange/{code} - Получить курс по коду

POST	/api/Exchange - Добавить новую валюту

PUT	/api/Exchange/{code}	- Обновить курс валюты

Модуль User (/api/User)

GET	/api/User/info	- Получить всех пользователей

POST	/api/User/info	- Добавить пользователя

ПРИМЕРЫ ЗАПРОСА

Добавление новой валюты:

POST /api/Exchange

Content-Type: application/json

{
 
  "currencyCode": "GBP",
 
  "currencyName": "Фунт стерлингов",
 
  "rate": 114.80

}


