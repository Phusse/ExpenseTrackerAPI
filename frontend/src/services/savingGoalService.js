import api from './api';

const getAllSavingGoals = (includeArchived = false) => {
  return api.get(`/savings/getall?includeArchived=${includeArchived}`);
};

const getSavingGoalById = (id) => {
  return api.get(`/savings/${id}`);
};

const createSavingGoal = (savingGoal) => {
  return api.post('/savings', savingGoal);
};

const updateSavingGoal = (id, savingGoal) => {
  return api.put(`/savings/${id}`, savingGoal);
};

const archiveSavingGoal = (id, archive = true) => {
  return api.patch(`/savings/${id}/archive?archiveGoal=${archive}`);
};

const deleteSavingGoal = (id) => {
  return api.delete(`/savings/${id}`);
};

const addContribution = (contribution) => {
  return api.post('/savings/contribute', contribution);
};

const savingGoalService = {
  getAllSavingGoals,
  getSavingGoalById,
  createSavingGoal,
  updateSavingGoal,
  archiveSavingGoal,
  deleteSavingGoal,
  addContribution,
};

export default savingGoalService;
