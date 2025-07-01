# 💸 Expense Tracker API

A lightweight and extensible **ASP.NET Core 8 Web API** for tracking personal expenses, setting savings goals, and monitoring budgets. Built with **clean architecture**, it supports email notifications, PDF reports, and a dashboard summary — perfect for personal finance apps.

---

## 🚀 Features

- 📥 **Create and manage expenses**: amount, category, payment method, description, etc.
- 🧠 **Savings goals**: Set and track monthly/yearly saving targets.
- 📊 **Dashboard**: Get a monthly overview of spending, saving, and budget health.
- 📨 **Email & PDF reports**: Email or download PDF summaries of filtered expenses.
- 🧾 **Budget thresholds**: Track spending against budgets and get email alerts.
- 🔐 **JWT Authentication**: Secure login and logout support.
- 📧 **Email notifications**:
  - Welcome Email
  - Login/Logout Alerts
  - Budget threshold alert
  - Monthly summary *(Planned)*
  - Savings goal progress *(Planned)*

---

## 🛠 Tech Stack

| Layer      | Tech                          |
|------------|-------------------------------|
| Backend    | ASP.NET Core 8 Web API        |
| Database   | MySQL + Entity Framework Core |
| Email      | Postmark                      |
| Reports    | QuestPDF                      |
| Auth       | JWT Tokens                    |
| Docs       | Swagger (Swashbuckle)         |

---

## 📦 Key Endpoints

| Method | Endpoint                              | Description                          |
|--------|---------------------------------------|--------------------------------------|
| POST   | `/api/v1/expense`                     | Create a new expense                 |
| GET    | `/api/v1/expense/{id}`                | Get expense by ID                    |
| GET    | `/api/v1/expense/getall`              | Get all expenses                     |
| GET    | `/api/v1/expense/filter`              | Filter expenses                      |
| DELETE | `/api/v1/expense/{id}`                | Delete an expense                    |
| DELETE | `/api/v1/expense/all`                 | Delete all expenses                  |
| POST   | `/api/v1/expenses/email-report`       | Email a PDF report of expenses       |
| POST   | `/api/v1/expenses/print`              | Download a PDF report                |
| GET    | `/api/v1/dashboard/summary`           | Get monthly dashboard summary        |
| POST   | `/api/v1/savinggoal`                  | Create savings goal                  |
| GET    | `/api/v1/savinggoal/user`             | Get user savings goals               |
| DELETE | `/api/v1/logout`                      | Log out and invalidate session       |

---

## 📧 Email Notifications

| Trigger                  | Status       |
|--------------------------|--------------|
| Welcome Email            | ✅ Implemented |
| Login Alert              | ✅ Implemented |
| Logout Notification      | ✅ Implemented |
| Budget Threshold Alert   | ✅ Implemented |
| Monthly Summary          | 🔜 Planned    |
| Savings Goal Progress    | 🔜 Planned    |

---

## 📊 Dashboard Summary

Returns:

```json
{
  "totalExpenses": 20000,
  "totalSavings": 5000,
  "budgets": [
    {
      "category": "Food",
      "budgeted": 10000,
      "spent": 7500
    },
    {
      "category": "Transport",
      "budgeted": 8000,
      "spent": 6500
    }
  ]
}
