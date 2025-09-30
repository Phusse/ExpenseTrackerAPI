import { useEffect, useState } from "react";
import type { DashboardSummaryResponse } from "../dtos/dashboards/dashboard-summary-response";
import { dashboardService } from "../services/dashboard-service";
import "./DashboardPage.css";
import NavBar from "../components/NavBar";

const DashboardPage: React.FC = () => {
  const [summary, setSummary] = useState<DashboardSummaryResponse | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    const controller = new AbortController();

    const fetchSummary = async () => {
      try {
        const response = await dashboardService.getDashboardSummary(controller.signal);
        setSummary(response.data ?? null);
      } catch (err: any) {
        if (err.name === "CanceledError") {
          console.log("Dashboard request cancelled");
          return;
        }
        setError("Failed to fetch dashboard summary.");
      }
    };

    fetchSummary();

    return () => {
      controller.abort(); // cancel request on cleanup
    };
  }, []);
  
  // useEffect(() => {
  //   const fetchSummary = async () => {
  //     try {
  //       const response = await dashboardService.getDashboardSummary();
  //       setSummary(response.data ?? null);
  //     } catch {
  //       setError("Failed to fetch dashboard summary.");
  //     }
  //   };

  //   fetchSummary();
  // }, []);

  return (
    <div className="dashboard">
      <NavBar />
      <main className="dashboard__main">
        <div className="dashboard__container">
          <h2 className="dashboard__title">Dashboard</h2>
          {error && <p className="dashboard__error">{error}</p>}
          {summary ? (
            <div>
              <div className="dashboard__cards">
                <div className="dashboard__card">
                  <h3 className="dashboard__card-title">Total Expenses</h3>
                  <p className="dashboard__card-value">
                    ${summary.totalExpenses}
                  </p>
                </div>
                <div className="dashboard__card">
                  <h3 className="dashboard__card-title">Total Savings</h3>
                  <p className="dashboard__card-value">
                    ${summary.totalSavings}
                  </p>
                </div>
              </div>

              <div className="dashboard__budgets">
                <h3 className="dashboard__section-title">Budgets</h3>
                <div className="dashboard__table-wrapper">
                  <table className="dashboard__table">
                    <thead>
                      <tr>
                        <th className="dashboard__th">Category</th>
                        <th className="dashboard__th">Budgeted</th>
                        <th className="dashboard__th">Spent</th>
                      </tr>
                    </thead>
                    <tbody>
                      {summary.budgets.map((budget) => (
                        <tr key={budget.category}>
                          <td className="dashboard__td">{budget.category}</td>
                          <td className="dashboard__td">${budget.budgetedAmount}</td>
                          <td className="dashboard__td">${budget.spentAmount}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          ) : (
            <p className="dashboard__loading">Loading...</p>
          )}
        </div>
      </main>
    </div>
  );
};

export default DashboardPage;
