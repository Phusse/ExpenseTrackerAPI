import { useEffect, useState } from "react";
import type { EnumOptionResponse } from "../dtos/metadata/enum-option-response";
import { useNavigate, useParams } from "react-router-dom";
import { metadataService } from "../services/metadata-service";
import type { CreateExpenseResponse } from "../dtos/expenses/create-expense-response";
import { expenseService } from "../services/expense-service";
import type { UpdateExpenseRequest } from "../dtos/expenses/update-expense-request";
import NavBar from "../components/NavBar";
import "./EditExpensePage.css"

const EditExpensePage: React.FC = () => {
  const [description, setDescription] = useState("");
  const [amount, setAmount] = useState("");
  const [date, setDate] = useState("");
  const [category, setCategory] = useState("");
  const [categories, setCategories] = useState<EnumOptionResponse[]>([]);
  const [error, setError] = useState("");

  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  useEffect(() => {
    const loadData = async () => {
      try {
        const categoriesRes = await metadataService.getExpenseCategories();
        setCategories(categoriesRes.data ?? []);

        if (!id) return;
        const expenseRes = await expenseService.getById(id);
        const expense: CreateExpenseResponse | null = expenseRes.data ?? null;

        if (expense) {
          setDescription(expense.description ?? "");
          setAmount(String(expense.amount));
          setDate(expense.dateOfExpense);
          setCategory(expense.category);
        }
      } catch {
        setError("Failed to fetch expense details.");
      }
    };

    loadData();
  }, [id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!id) return;

    const updatedExpense: UpdateExpenseRequest = {
      id: id,
      description: description,
      amount: parseFloat(amount),
      dateOfExpense: date,
      category: category,
    };

    try {
      await expenseService.update(updatedExpense);
      navigate("/expenses");
    } catch {
      setError("Failed to update expense.");
    }
  };

  return (
    <div className="edit-expense">
      <NavBar />
      <main className="edit-expense__main">
        <div className="edit-expense__container">
          <h1 className="edit-expense__title">Edit Expense</h1>
          {error && <p className="edit-expense__error">{error}</p>}
          <form onSubmit={handleSubmit} className="edit-expense__form">
            <div className="edit-expense__field">
              <label htmlFor="description" className="edit-expense__label">
                Description
              </label>
              <input
                id="description"
                type="text"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                required
                className="edit-expense__input"
              />
            </div>

            <div className="edit-expense__field">
              <label htmlFor="amount" className="edit-expense__label">
                Amount
              </label>
              <input
                id="amount"
                type="number"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                required
                className="edit-expense__input"
              />
            </div>

            <div className="edit-expense__field">
              <label htmlFor="date" className="edit-expense__label">
                Date
              </label>
              <input
                id="date"
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
                required
                className="edit-expense__input"
              />
            </div>

            <div className="edit-expense__field">
              <label htmlFor="category" className="edit-expense__label">
                Category
              </label>
              <select
                id="category"
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                required
                className="edit-expense__select"
              >
                {categories.map((cat) => (
                  <option key={cat.value} value={cat.value}>
                    {cat.label}
                  </option>
                ))}
              </select>
            </div>

            <div className="edit-expense__actions">
              <button type="submit" className="edit-expense__submit">
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
