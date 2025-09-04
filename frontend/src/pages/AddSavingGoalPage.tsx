import { useState } from "react";
import { useNavigate } from "react-router-dom";
import type { CreateSavingGoalRequest } from "../dtos/saving-goals/create-saving-goal-request";
import { savingGoalService } from "../services/saving-goal-service";
import NavBar from "../components/NavBar";
import "./AddSavingGoalPage.css"

const AddSavingGoalPage = () => {
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [targetAmount, setTargetAmount] = useState('');
  const [targetDate, setTargetDate] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const newGoal: CreateSavingGoalRequest = {
      title: name,
      description: description,
      targetAmount: parseFloat(targetAmount),
      deadline: targetDate,
    };

    try {
      await savingGoalService.create(newGoal);
      navigate('/savings');
    } catch (err) {
      setError('Failed to add saving goal.');
    }
  };

  return (
    <div className="add-goal">
      <NavBar />
      <main className="add-goal__main">
        <div className="add-goal__container">
          <h1 className="add-goal__title">Add Saving Goal</h1>
          {error && <p className="add-goal__error">{error}</p>}
          <form onSubmit={handleSubmit} className="add-goal__form">
            <div className="add-goal__field">
              <label htmlFor="name" className="add-goal__label">Name</label>
              <input
                id="name"
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                required
                className="add-goal__input"
              />
            </div>
            <div className="add-goal__field">
              <label htmlFor="description" className="add-goal__label">Description</label>
              <textarea
                id="description"
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                className="add-goal__input add-goal__input--textarea"
              />
            </div>
            <div className="add-goal__field">
              <label htmlFor="targetAmount" className="add-goal__label">Target Amount</label>
              <input
                id="targetAmount"
                type="number"
                value={targetAmount}
                onChange={(e) => setTargetAmount(e.target.value)}
                required
                className="add-goal__input"
              />
            </div>
            <div className="add-goal__field">
              <label htmlFor="targetDate" className="add-goal__label">Target Date</label>
              <input
                id="targetDate"
                type="date"
                value={targetDate}
                onChange={(e) => setTargetDate(e.target.value)}
                required
                className="add-goal__input"
              />
            </div>
            <div className="add-goal__actions">
              <button type="submit" className="add-goal__button">Add Goal</button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
};

export default AddSavingGoalPage;
