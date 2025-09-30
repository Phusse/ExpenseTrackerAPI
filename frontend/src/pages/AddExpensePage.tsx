import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import type { EnumOptionResponse } from "../dtos/metadata/enum-option-response";
import { metadataService } from "../services/metadata-service";
import type { CreateExpenseRequest } from "../dtos/expenses/create-expense-request";
import { expenseService } from "../services/expense-service";
import NavBar from "../components/NavBar";
import "./AddExpensePage.css";

const AddExpensePage: React.FC = () => {
  const [description, setDescription] = useState<string>("");
  const [amount, setAmount] = useState<string>("");
  const [date, setDate] = useState<string>(
    new Date().toISOString().slice(0, 10)
  );
  const [category, setCategory] = useState<string>("");
  const [paymentMethod, setPaymentMethod] = useState<string>("");
  const [categories, setCategories] = useState<EnumOptionResponse[]>([]);
  const [paymentMethods, setPaymentMethods] = useState<EnumOptionResponse[]>([]);
  const [error, setError] = useState<string>("");

  const navigate = useNavigate();

  useEffect(() => {
    metadataService
      .getExpenseCategories()
      .then((response) => {
        const data = response.data ?? [];
        setCategories(data);
        if (data.length > 0) {
          setCategory(data[0].value);
        }
      })
      .catch(() => {
        setError("Failed to fetch categories.");
      });

    metadataService
      .getPaymentMethods()
      .then((response) => {
        const data = response.data ?? [];
        setPaymentMethods(data);
        if (data.length > 0) {
          setPaymentMethod(data[0].value);
        }
      })
      .catch(() => {
        setError("Failed to fetch payment methods.");
      });
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const newExpense: CreateExpenseRequest = {
      description,
      amount: parseFloat(amount),
      dateOfExpense: date,
      category,
      paymentMethod,
    };

    try {
      await expenseService.create(newExpense);
      navigate("/expenses");
    } catch {
      setError("Failed to add expense.");
    }
  };

  return (
    <div className="add-expense">
      <NavBar />
      <main className="add-expense__main">
        <div className="add-expense__container">
          <h1 className="add-expense__title">Add Expense</h1>
          {error && <p className="add-expense__error">{error}</p>}

          <form onSubmit={handleSubmit} className="add-expense__form">
            <div className="add-expense__field">
              <label htmlFor="description" className="add-expense__label">
                Description
              </label>
              <input
                id="description"
                type="text"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                required
                className="add-expense__input"
              />
            </div>

            <div className="add-expense__field">
              <label htmlFor="amount" className="add-expense__label">
                Amount
              </label>
              <input
                id="amount"
                type="number"
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                required
                className="add-expense__input"
              />
            </div>

            <div className="add-expense__field">
              <label htmlFor="date" className="add-expense__label">
                Date
              </label>
              <input
                id="date"
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
                required
                className="add-expense__input"
              />
            </div>

            <div className="add-expense__field">
              <label htmlFor="category" className="add-expense__label">
                Category
              </label>
              <select
                id="category"
                value={category}
                onChange={(e) => setCategory(e.target.value)}
                required
                className="add-expense__select"
              >
                {categories.map((cat) => (
                  <option key={cat.value} value={cat.value}>
                    {cat.value}
                  </option>
                ))}
              </select>
            </div>

            <div className="add-expense__field">
              <label htmlFor="paymentMethod" className="add-expense__label">
                Payment Method
              </label>
              <select
                id="paymentMethod"
                value={paymentMethod}
                onChange={(e) => setPaymentMethod(e.target.value)}
                required
                className="add-expense__select"
              >
                {paymentMethods.map((pm) => (
                  <option key={pm.value} value={pm.value}>
                    {pm.value}
                  </option>
                ))}
              </select>
            </div>

            <div className="add-expense__actions">
              <button type="submit" className="add-expense__submit">
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
