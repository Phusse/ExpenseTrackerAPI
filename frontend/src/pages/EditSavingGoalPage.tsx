import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { savingGoalService } from "../services/saving-goal-service";
import type { UpdateSavingGoalRequest } from "../dtos/saving-goals/update-saving-goal-request";
import NavBar from "../components/NavBar";
import "./EditSavingGoalPage.css"

const EditSavingGoalPage: React.FC = () => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [targetAmount, setTargetAmount] = useState("");
  const [deadline, setDeadline] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();

  useEffect(() => {
    if (!id) return;

    const fetchGoal = async () => {
      try {
        const response = await savingGoalService.getById(id);
        const goal = response.data;

        if (!goal) {
          setError("Saving goal not found.");
          return;
        }

        setTitle(goal.title);
        setDescription(goal.description || "");
        setTargetAmount(goal.targetAmount.toString());
        setDeadline(goal.deadline ?? "");
      } catch {
        setError("Failed to fetch saving goal details.");
      }
    };

    fetchGoal();
  }, [id]);


  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!id) return;

    const updatedGoal: UpdateSavingGoalRequest = {
      title,
      description,
      targetAmount: parseFloat(targetAmount),
      deadline,
    };

    try {
      await savingGoalService.update(id, updatedGoal);
      navigate("/savings");
    } catch {
      setError("Failed to update saving goal.");
    }
  };

  return (
    <div className="edit-goal">
      <NavBar />
      <main className="edit-goal__main">
        <div className="edit-goal__container">
          <h1 className="edit-goal__title">Edit Saving Goal</h1>
          {error && <p className="edit-goal__error">{error}</p>}
          <form onSubmit={handleSubmit} className="edit-goal__form">
            <div className="edit-goal__field">
              <label htmlFor="title" className="edit-goal__label">
                Name
              </label>
              <input
                id="title"
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                required
                className="edit-goal__input"
              />
            </div>
            <div className="edit-goal__field">
              <label htmlFor="description" className="edit-goal__label">
                Description
              </label>
              <textarea
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                className="edit-goal__input edit-goal__input--textarea"
              />
            </div>
            <div className="edit-goal__field">
              <label htmlFor="targetAmount" className="edit-goal__label">
                Target Amount
              </label>
              <input
                id="targetAmount"
                type="number"
                value={targetAmount}
                onChange={(e) => setTargetAmount(e.target.value)}
                required
                className="edit-goal__input"
              />
            </div>
            <div className="edit-goal__field">
              <label htmlFor="deadline" className="edit-goal__label">
                Target Date
              </label>
              <input
                id="deadline"
                type="date"
                value={deadline}
                onChange={(e) => setDeadline(e.target.value)}
                required
                className="edit-goal__input"
              />
            </div>
            <div className="edit-goal__actions">
              <button type="submit" className="edit-goal__button">
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
