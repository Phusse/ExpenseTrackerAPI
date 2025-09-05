import { useEffect, useState } from "react";
import { savingGoalService } from "../services/saving-goal-service";
import NavBar from "../components/Navbar";
import { Link } from "react-router-dom";
import type { CreateSavingGoalResponse } from "../dtos/saving-goals/create-saving-goal-response";
import "./SavingsPage.css";

const SavingsPage = () => {
  const [goals, setGoals] = useState<CreateSavingGoalResponse[]>([]);
  const [error, setError] = useState<string>("");

  useEffect(() => {
    fetchGoals();
  }, []);

  const fetchGoals = async () => {
    try {
      const response = await savingGoalService.getAll();
      setGoals(response.data || []);
    } catch (err: any) {
      setError("Failed to fetch savings goals.");

      if (err.response && err.response.status === 404) {
        setGoals([]);
      }
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await savingGoalService.deleteById(id);
      fetchGoals();
    } catch {
      setError("Failed to delete saving goal.");
    }
  };

  return (
    <div className="savings">
      <NavBar />
      <main className="savings__main">
        <div className="savings__container">
          <div className="savings__header">
            <h1 className="savings__title">Savings Goals</h1>
            <Link to="/savings/add" className="savings__add-button">
              Add Goal
            </Link>
          </div>
          {error && <p className="savings__error">{error}</p>}
          <div className="savings__grid">
            {goals.length > 0 ? (
              goals.map((goal) => (
                <div key={goal.id} className="goal-card">
                  <h2 className="goal-card__title">{goal.title}</h2>
                  <p className="goal-card__description">{goal.description}</p>
                  <div className="goal-card__meta">
                    <span className="goal-card__amount">
                      ${goal.currentAmount} / ${goal.targetAmount}
                    </span>
                    <span className="goal-card__status">{goal.status}</span>
                  </div>
                  <div className="goal-card__progress">
                    <div
                      className="goal-card__progress-bar"
                      style={{
                        width: `${(goal.currentAmount / goal.targetAmount) * 100}%`,
                      }}
                    ></div>
                  </div>
                  <div className="goal-card__actions">
                    <button className="goal-card__action goal-card__action--contribute">
                      Contribute
                    </button>
                    <Link
                      to={`/savings/edit/${goal.id}`}
                      className="goal-card__action goal-card__action--edit"
                    >
                      Edit
                    </Link>
                    <button
                      onClick={() => handleDelete(goal.id)}
                      className="goal-card__action goal-card__action--delete"
                    >
                      Delete
                    </button>
                  </div>
                </div>
              ))
            ) : (
              <p>No savings goals found.</p>
            )}
          </div>
        </div>
      </main>
    </div>
  );
};

export default SavingsPage;
