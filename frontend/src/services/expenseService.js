import api from './api';

const getAllExpenses = () => {
  return api.get('/expense/getall');
};

const getExpenseById = (id) => {
  return api.get(`/expense/${id}`);
};

const createExpense = (expense) => {
  return api.post('/expense', expense);
};

const updateExpense = (id, expense) => {
  return api.put(`/expense/${id}`, expense);
};

const deleteExpense = (id) => {
  return api.delete(`/expense/${id}`);
};

const expenseService = {
  getAllExpenses,
  getExpenseById,
  createExpense,
  updateExpense,
  deleteExpense,
};

export default expenseService;
