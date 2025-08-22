import api from './api';

const getExpenseCategories = () => {
  return api.get('/metadata/expense-categories');
};

const metadataService = {
  getExpenseCategories,
};

export default metadataService;
