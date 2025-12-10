# Currency Converter API

**Название проекта:** `Convertation `  
**Основное назначение:** Веб-сервис для конвертации валют с сохранением истории операций.

---

## Описание проекта

**Convertation ** — это RESTful веб-сервис, реализованный на ASP.NET Core, предназначенный для конвертации валют по фиксированным курсам с возможностью сохранения истории всех операций. Сервис ориентирован на разработчиков, которые хотят интегрировать простой и надёжный конвертер валют в свои приложения (веб, мобильные, боты и т.д.).

**Целевая аудитория:**
- Разработчики backend- и frontend-приложений
- Финтех-стартапы и сервисы
- Обучающие проекты и прототипы
- Тестировщики API

---

## Основные функции

| Метод | Endpoint | Описание |
|------|----------|---------|
| `POST` | `/api/conversions` | Выполняет конвертацию валюты. Принимает `from`, `to`, `amount`. Возвращает результат и сохраняет запись. |
| `GET` | `/api/conversions` | Возвращает список всех сохранённых конвертаций. |
| `GET` | `/api/conversions/{id}` | Получить конвертацию по ID. |
| `GET` | `/api/conversions/rates` | Получить актуальные курсы всех валют относительно **RUB** (1 единица валюты = X RUB). |

### Структура проекта
Convertation /
├── Controllers/
│   └── ConversionsController.cs
├── Data/
│   └── FileDbContext.cs
├── Models/
│   ├── Conversion.cs
│   └── ConversionRequest.cs
├── Services/
│   ├── ConversionService.cs
│   └── IConversionService.cs
├── wwwroot/data/
│   └── conversions.json (создаётся автоматически)
└── Program.cs

### Пример запроса на конвертацию:
POST /api/conversions
Content-Type: application/json
{
  "fromCurrency": "USD",
  "toCurrency": "EUR",
  "amount": 100
}