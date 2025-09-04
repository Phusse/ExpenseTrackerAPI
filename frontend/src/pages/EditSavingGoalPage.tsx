import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import savingGoalService from '../services/savingGoalService';
import Navbar from '../components/Navbar';

const EditSavingGoalPage = () => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [targetAmount, setTargetAmount] = useState('');
  const [targetDate, setTargetDate] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { id } = useParams();

  useEffect(() => {
    savingGoalService.getSavingGoalById(id)
      .then(response => {
        const goal = response.data.data;
        setName(goal.name);
        setDescription(goal.description);
        setTargetAmount(goal.targetAmount);
        setTargetDate(new Date(goal.targetDate).toISOString().slice(0, 10));
      })
      .catch(err => {
        setError('Failed to fetch saving goal details.');
      });
  }, [id]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    const updatedGoal = {
      id,
      name,
      description,
      targetAmount: parseFloat(targetAmount),
      targetDate,
    };

    try {
      await savingGoalService.updateSavingGoal(id, updatedGoal);
      navigate('/savings');
    } catch (err) {
      setError('Failed to update saving goal.');
    }
  };

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <main className="py-10">
        <div className="max-w-7xl mx-auto sm:px-6 lg:px-8">
          <h1 className="text-2xl font-bold mb-6">Edit Saving Goal</h1>
          {error && <p className="text-red-500 mb-4">{error}</p>}
          <form onSubmit={handleSubmit} className="max-w-lg bg-white p-8 rounded-lg shadow">
            <div className="mb-4">
              <label htmlFor="name" className="block text-gray-700">Name</label>
              <input
                id="name"
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
                className="w-full mt-1 px-3 py-2 border rounded-md"
              />
            </div>
            <div className="mb-4">
              <label htmlFor="description" className="block text-gray-700">Description</label>
              <textarea
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                className="w-full mt-1 px-3 py-2 border rounded-md"
              />
            </div>
            <div className="mb-4">
              <label htmlFor="targetAmount" className="block text-gray-700">Target Amount</label>
              <input
                id="targetAmount"
                type="number"
                value={targetAmount}
                onChange={(e) => setTargetAmount(e.target.value)}
                required
                className="w-full mt-1 px-3 py-2 border rounded-md"
              />
            </div>
            <.div className="mb-4">
              <label htmlFor="targetDate" className="block text-gray-700">Target Date</label>
              <input
                id="targetDate"
                type="date"
                value={targetDate}
                onChange={(e) => setTargetDate(e.target.value)}
                required
                className="w-full mt-1 px-3 py-2 border rounded-md"
              />
            </div>
            <div className="flex justify-end">
              <button type="submit" className="px-4 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700">
                Update Goal
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
};

export default EditSavingGoalPage;
