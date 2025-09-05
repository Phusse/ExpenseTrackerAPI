import { useEffect, useState } from "react";
import type { CreateExpenseResponse } from "../dtos/expenses/create-expense-response";
import { expenseService } from "../services/expense-service";
import { Link } from "react-router-dom";
import "./ExpensePage.css"
import NavBar from "../components/NavBar";

const ExpensesPage: React.FC = () => {
  const [expenses, setExpenses] = useState<CreateExpenseResponse[]>([]);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchExpenses();
  }, []);

  const fetchExpenses = async () => {
    try {
      const response = await expenseService.getAll();
      setExpenses(response.data ?? []);
    } catch (err: any) {
      setError("Failed to fetch expenses.");
      if (err.response && err.response.status === 404) {
        setExpenses([]);
      }
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await expenseService.deleteById(id);
      fetchExpenses();
    } catch {
      setError("Failed to delete expense.");
    }
  };

  return (
    <div className="expenses-page">
      <NavBar />
      <main className="expenses-page__main">
        <div className="expenses-page__container">
          <div className="expenses-page__header">
            <h1 className="expenses-page__title">Expenses</h1>
            <Link to="/expenses/add" className="expenses-page__add-btn">
              Add Expense
            </Link>
          </div>

          {error && <p className="expenses-page__error">{error}</p>}

          <div className="expenses-page__table-container">
            <table className="expenses-table">
              <thead>
                <tr>
                  <th>Description</th>
                  <th>Amount</th>
                  <th>Category</th>
                  <th>Date</th>
                  <th className="text-right">Actions</th>
                </tr>
              </thead>
              <tbody>
                {expenses.length > 0 ? (
                  expenses.map((expense) => (
                    <tr key={expense.id}>
                      <td>{expense.description}</td>
                      <td>${expense.amount}</td>
                      <td>{expense.category}</td>
                      <td>
                        {new Date(expense.dateOfExpense).toLocaleDateString(undefined, {
                          year: "numeric",
                          month: "short",
                          day: "numeric",
                        })}
                      </td>
                      <td className="expenses-table__actions">
                        <Link
                          to={`/expenses/edit/${expense.id}`}
                          className="expenses-table__edit"
                        >
                          Edit
                        </Link>
                        <button
                          onClick={() => handleDelete(expense.id)}
                          className="expenses-table__delete"
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={5} className="expenses-table__empty">
                      No expenses found.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      </main>
    </div>
  );
};

export default ExpensesPage;
