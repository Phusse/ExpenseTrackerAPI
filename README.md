# Expense Tracker - Full Stack Application

A modern, full-featured expense tracking application with advanced analytics, budgeting, and goal-setting capabilities.

## 🌟 Features

### Core Functionality
- **Expense Tracking** - Record and categorize all your expenses
- **Budget Management** - Set monthly budgets per category
- **Saving Goals** - Create and track savings goals with automatic contributions
- **Dashboard Analytics** - Comprehensive overview of your financial health

### Advanced Features (Phase 3)
- **Financial Health Score** - AI-powered 0-100 scoring system
- **Predictive Analytics** - Month-end spending forecasts, budget warnings, goal predictions
- **Pattern Detection** - Recurring expenses, anomaly detection, category trends
- **Smart Insights** - Actionable financial recommendations

## 🛠️ Tech Stack

### Backend
- **.NET 8** - Web API framework
- **PostgreSQL** - Database
- **Entity Framework Core** - ORM
- **JWT Authentication** - Secure authentication

### Frontend
- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **Recharts** - Data visualization

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- PostgreSQL 14+

### Backend Setup
```bash
cd backend
dotnet ef database update
dotnet watch run
```

### Frontend Setup
```bash
cd frontend
npm install
cp .env.example .env.local
npm run dev
```

## 🌐 Deployment

- **Frontend**: Deploy to Vercel (see `frontend/vercel.json`)
- **Backend**: Deploy to Railway/Render/Azure (see `backend/HOSTING.md`)

### Required Environment Variables

**Backend:**
```env
ConnectionStrings__DefaultConnection=your-postgres-connection
JwtSettings__SecretKey=your-32-char-secret
Cors__AllowedOrigins__0=https://your-frontend.vercel.app
```

**Frontend:**
```env
VITE_API_BASE_URL=https://your-backend-url.com
```

## 📊 API Endpoints

| Endpoint | Description |
|----------|-------------|
| `POST /api/auth/register` | Register user |
| `POST /api/auth/login` | Login user |
| `GET /api/expenses` | Get expenses |
| `GET /api/budgets` | Get budgets |
| `GET /api/savinggoals` | Get goals |
| `GET /api/analytics/health-score` | Financial health score |
| `GET /api/analytics/forecast` | Spending forecast |
| `GET /api/dashboard/summary` | Dashboard data |

## 📝 License

MIT License

---

**Happy Tracking! 💰📊**
