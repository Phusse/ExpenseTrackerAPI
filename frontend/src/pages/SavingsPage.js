import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import savingGoalService from '../services/savingGoalService';
import Navbar from '../components/Navbar';

const SavingsPage = () => {
  const [goals, setGoals] = useState([]);
  const [error, setError] = useState('');

  useEffect(() => {
    fetchGoals();
  }, []);

  const fetchGoals = () => {
    savingGoalService.getAllSavingGoals()
      .then(response => {
        setGoals(response.data.data || []);
      })
      .catch(err => {
        setError('Failed to fetch savings goals.');
        if (err.response && err.response.status === 404) {
          setGoals([]);
        }
      });
  };

  const handleDelete = (id) => {
    savingGoalService.deleteSavingGoal(id)
      .then(() => {
        fetchGoals();
      })
      .catch(err => {
        setError('Failed to delete saving goal.');
      });
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <main className="py-10">
        <div className="max-w-7xl mx-auto sm:px-6 lg:px-8">
          <div className="flex justify-between items-center mb-6">
            <h1 className="text-2xl font-bold">Savings Goals</h1>
            <Link to="/savings/add" className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700">
              Add Goal
            </Link>
          </div>
          {error && <p className="text-red-500">{error}</p>}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {goals.length > 0 ? goals.map(goal => (
              <div key={goal.id} className="bg-white shadow rounded-lg p-6">
                <h2 className="text-xl font-bold mb-2">{goal.name}</h2>
                <p className="text-gray-600 mb-4">{goal.description}</p>
                <div className="flex justify-between items-center mb-4">
                  <span className="text-lg font-semibold">${goal.currentAmount} / ${goal.targetAmount}</span>
                  <span className="text-sm font-medium text-gray-500">{goal.status}</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2.5">
                  <div className="bg-indigo-600 h-2.5 rounded-full" style={{ width: `${(goal.currentAmount / goal.targetAmount) * 100}%` }}></div>
                </div>
                <div className="mt-6 flex justify-end space-x-4">
                  <button className="text-sm font-medium text-green-600 hover:text-green-900">Contribute</button>
                  <Link to={`/savings/edit/${goal.id}`} className="text-sm font-medium text-indigo-600 hover:text-indigo-900">Edit</Link>
                  <button onClick={() => handleDelete(goal.id)} className="text-sm font-medium text-red-600 hover:text-red-900">Delete</button>
                </div>
              </div>
            )) : (
              <p>No savings goals found.</p>
            )}
          </div>
        </div>
      </main>
    </div>
  );
};

export default SavingsPage;
