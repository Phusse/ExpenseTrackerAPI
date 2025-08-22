import React, { useState, useEffect } from 'react';
import Navbar from '../components/Navbar';
import dashboardService from '../services/dashboardService';

const DashboardPage = () => {
  const [summary, setSummary] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    dashboardService.getDashboardSummary()
      .then(response => {
        setSummary(response.data.data);
      })
      .catch(err => {
        setError('Failed to fetch dashboard summary.');
      });
  }, []);

  return (
    <div className="min-h-screen bg-gray-100">
      <Navbar />
      <main className="py-10">
        <div className="max-w-7xl mx-auto sm:px-6 lg:px-8">
          <h2 className="text-2xl font-bold mb-6">Dashboard</h2>
          {error && <p className="text-red-500">{error}</p>}
          {summary ? (
            <div>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                <div className="bg-white p-6 rounded-lg shadow">
                  <h3 className="text-lg font-medium">Total Expenses</h3>
                  <p className="text-3xl font-bold">${summary.totalExpenses}</p>
                </div>
                <div className="bg-white p-6 rounded-lg shadow">
                  <h3 className="text-lg font-medium">Total Savings</h3>
                  <p className="text-3xl font-bold">${summary.totalSavings}</p>
                </div>
              </div>
              <div>
                <h3 className="text-xl font-bold mb-4">Budgets</h3>
                <div className="bg-white shadow overflow-hidden sm:rounded-lg">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Category
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Budgeted
                        </th>
                        <th scope="col" className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                          Spent
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {summary.budgets.map(budget => (
                        <tr key={budget.category}>
                          <td className="px-6 py-4 whitespace-nowrap">{budget.category}</td>
                          <td className="px-6 py-4 whitespace-nowrap">${budget.budgeted}</td>
                          <td className="px-6 py-4 whitespace-nowrap">${budget.spent}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          ) : (
            <p>Loading...</p>
          )}
        </div>
      </main>
    </div>
  );
};

export default DashboardPage;
