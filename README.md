# Expense Tracker API

A lightweight and extensible ASP.NET Core Web API for tracking expenses. Built with clean architecture principles, this API allows users to create, read, update, filter, and delete expenses. Ideal for personal finance applications or budget management tools.

---

## 🚀 Features

- Create and store expenses with details like amount, category, and payment method.
- Fetch all expenses or by ID.
- Filter expenses by date, amount range, or category.
- Update and delete individual or all expenses.
- Clean RESTful routing with versioning (`api/v1/expense/...`)
- Connected to MySQL using Entity Framework Core.
- Swagger support for testing and documentation.

---

## 🛠 Tech Stack

- **Backend**: ASP.NET Core 8 Web API
- **Database**: MySQL (via Railway or Render)
- **ORM**: Entity Framework Core
- **Docs**: Swagger (Swashbuckle)

---

## 📦 Endpoints Overview

| Method | Endpoint                      | Description                     |
|--------|-------------------------------|---------------------------------|
| POST   | `/api/v1/expense`             | Create a new expense            |
| GET    | `/api/v1/expense/{id}`        | Get expense by ID               |
| GET    | `/api/v1/expense/getall`      | Get all expenses                |
| GET    | `/api/v1/expense/filter`      | Filter expenses by query params |
| PUT    | `/api/v1/expense/{id}`        | Update an existing expense      |
| DELETE | `/api/v1/expense/{id}`        | Delete an expense by ID         |
| DELETE | `/api/v1/expense/all`         | Delete all expenses             |

---

## 🧪 Filtering Parameters

You can filter expenses using the following query parameters:

- `startDate`: `yyyy-MM-dd`
- `endDate`: `yyyy-MM-dd`
- `minAmount`: decimal
- `maxAmount`: decimal
- `exactAmount`: decimal
- `category`: string
