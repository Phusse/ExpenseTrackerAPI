import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import expenseService from '../services/expenseService';
import metadataService from '../services/metadataService';
import Navbar from '../components/Navbar';

const AddExpensePage = () => {
  const [description, setDescription] = useState('');
  const [amount, setAmount] = useState('');
  const [date, setDate] = useState(new Date().toISOString().slice(0, 10));
  const [category, setCategory] = useState('');
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    metadataService.getExpenseCategories()
      .then(response => {
        setCategories(response.data.data || []);
        if (response.data.data && response.data.data.length > 0) {
          setCategory(response.data.data[0].value);
        }
      })
      .catch(err => {
        setError('Failed to fetch categories.');
      });
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    const newExpense = {
      description,
      amount: parseFloat(amount),
      date,
      category,
    };

    try {
      await expenseService.createExpense(newExpense);
      navigate('/expenses');
    } catch (err) {
      setError('Failed to add expense.');
    }
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <main className="py-10">
        <div className="max-w-7xl mx-auto sm:px-6 lg:px-8">
          <h1 className="text-2xl font-bold mb-6">Add Expense</h1>
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
                Add Expense
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
};

export default AddExpensePage;
