import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import expenseService from '../services/expenseService';
import metadataService from '../services/metadataService';
import Navbar from '../components/Navbar';

const EditExpensePage = () => {
  const [description, setDescription] = useState('');
  const [amount, setAmount] = useState('');
  const [date, setDate] = useState('');
  const [category, setCategory] = useState('');
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { id } = useParams();

  useEffect(() => {
    metadataService.getExpenseCategories()
      .then(response => {
        setCategories(response.data.data || []);
      })
      .catch(err => {
        setError('Failed to fetch categories.');
      });

    expenseService.getExpenseById(id)
      .then(response => {
        const expense = response.data.data;
        setDescription(expense.description);
        setAmount(expense.amount);
        setDate(new Date(expense.date).toISOString().slice(0, 10));
        setCategory(expense.category);
      })
      .catch(err => {
        setError('Failed to fetch expense details.');
      });
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    const updatedExpense = {
      id,
      description,
      amount: parseFloat(amount),
      date,
      category,
    };

    try {
      await expenseService.updateExpense(id, updatedExpense);
      navigate('/expenses');
    } catch (err) {
      setError('Failed to update expense.');
    }
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <main className="py-10">
        <div className="max-w-7xl mx-auto sm:px-6 lg:px-8">
          <h1 className="text-2xl font-bold mb-6">Edit Expense</h1>
          {error && <p className="text-red-500 mb-4">{error}</p>}
          <form onSubmit={handleSubmit} className="max-w-lg bg-white p-8 rounded-lg shadow">
            <div className="mb-4">
              <label htmlFor="description" className="block text-gray-700">Description</label>
              <input
                id="description"
                type="text"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                required
                className="w-full mt-1 px-3 py-2 border rounded-md"
              />
            </div>
            <div className="mb-4">
              <label htmlFor="amount" className="block text-gray-700">Amount</label>
              <input
                id="amount"
                type="number"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                required
                className="w-full mt-1 px-3 py-2 border rounded-md"
              />
            </div>
            <div className="mb-4">
              <label htmlFor="date" className="block text-gray-700">Date</label>
              <input
                id="date"
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
                required
                className="w-full mt-1 px-3 py-2 border rounded-md"
              />
            </div>
            <div className="mb-4">
              <label htmlFor="category" className="block text-gray-700">Category</label>
              <select
                id="category"
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                required
                className="w-full mt-1 px-3 py-2 border rounded-md"
              >
                {categories.map(cat => (
                  <option key={cat.value} value={cat.value}>{cat.name}</option>
                ))}
              </select>
            </div>
            <div className="flex justify-end">
              <button type="submit" className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700">
                Update Expense
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
};

export default EditExpensePage;
