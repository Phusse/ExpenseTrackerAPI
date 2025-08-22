import api from './api';

const getDashboardSummary = () => {
  return api.get('/dashboard/summary');
};

const dashboardService = {
  getDashboardSummary,
};

export default dashboardService;
