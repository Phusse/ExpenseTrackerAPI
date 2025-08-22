import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import ExpensesPage from './pages/ExpensesPage';
import AddExpensePage from './pages/AddExpensePage';
import EditExpensePage from './pages/EditExpensePage';
import SavingsPage from './pages/SavingsPage';
import AddSavingGoalPage from './pages/AddSavingGoalPage';
import EditSavingGoalPage from './pages/EditSavingGoalPage';
import PrivateRoute from './components/PrivateRoute';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Protected Routes */}
          <Route element={<PrivateRoute />}>
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/expenses" element={<ExpensesPage />} />
            <Route path="/expenses/add" element={<AddExpensePage />} />
            <Route path="/expenses/edit/:id" element={<EditExpensePage />} />
            <Route path="/savings" element={<SavingsPage />} />
            <Route path="/savings/add" element={<AddSavingGoalPage />} />
            <Route path="/savings/edit/:id" element={<EditSavingGoalPage />} />
          </Route>

          <Route path="*" element={<Navigate to="/login" />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
