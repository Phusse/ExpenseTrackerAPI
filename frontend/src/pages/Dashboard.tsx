import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { BarChart3, TrendingUp, TrendingDown, DollarSign, Loader2, Plus, Sparkles, ArrowRight, PiggyBank, Receipt, Target, Percent } from 'lucide-react';
import { dashboardService, type DashboardSummary } from '../services/dashboardService';
import { analyticsService, type PredictiveInsights, type Achievement } from '../services/analyticsService';
import { incomeService, type IncomeSummary } from '../services/incomeService';
import { SpendingChart } from '../components/SpendingChart';
import { TopSpendingWidget } from '../components/TopSpendingWidget';
import { MonthComparison } from '../components/MonthComparison';
import { CategoryBreakdownChart } from '../components/CategoryBreakdownChart';
import { AchievementBadges } from '../components/AchievementBadges';
import { FinancialHealthScoreWidget } from '../components/FinancialHealthScoreWidget';
import { AIInsightsWidget } from '../components/AIInsightsWidget';
import { authService } from '../services/authService';
import { useSettings } from '../context/SettingsContext';
import { HealthScoreModal } from '../components/HealthScoreModal';
import type { FinancialHealthScore } from '../services/analyticsService';

// Mobile-optimized stat card
const StatCard = ({ title, value, icon: Icon, trend, color = 'primary' }: any) => {
    const colors: Record<string, string> = {
        primary: 'from-primary/20 to-indigo-500/20 border-primary/20 text-primary',
        secondary: 'from-secondary/20 to-teal-500/20 border-secondary/20 text-secondary',
        danger: 'from-rose-500/20 to-orange-500/20 border-rose-500/20 text-rose-400',
        warning: 'from-amber-500/20 to-yellow-500/20 border-amber-500/20 text-amber-400',
    };

    return (
        <div className="glass-card p-4 min-w-[140px] md:min-w-0">
            <div className={`w-10 h-10 rounded-xl bg-gradient-to-br ${colors[color]} border flex items-center justify-center mb-3`}>
                <Icon className="w-5 h-5" />
            </div>
            <p className="text-xs text-gray-400 mb-1">{title}</p>
            <p className="text-lg font-bold text-white">{value}</p>
            {trend && (
                <div className={`flex items-center gap-1 mt-1 text-xs ${trend === 'up' ? 'text-secondary' : 'text-rose-400'}`}>
                    {trend === 'up' ? <TrendingUp className="w-3 h-3" /> : <TrendingDown className="w-3 h-3" />}
                    <span>{trend === 'up' ? '+12%' : '-5%'}</span>
                </div>
            )}
        </div>
    );
};

// Quick action for mobile
const QuickAction = ({ icon: Icon, label, to, color }: any) => (
    <Link
        to={to}
        className="flex flex-col items-center gap-2 p-4 glass-card active:scale-95 transition-transform"
    >
        <div className={`w-12 h-12 rounded-full ${color} flex items-center justify-center`}>
            <Icon className="w-6 h-6 text-white" />
        </div>
        <span className="text-xs text-gray-400">{label}</span>
    </Link>
);

// Transaction item for mobile
const TransactionItem = ({ transaction, formatCurrency }: any) => {
    let displayDate = 'No date';
    if (transaction.dateOfExpense) {
        const parts = transaction.dateOfExpense.split('T')[0].split('-');
        if (parts.length === 3) {
            const date = new Date(parseInt(parts[0]), parseInt(parts[1]) - 1, parseInt(parts[2]));
            displayDate = date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
        }
    }

    return (
        <div className="flex items-center gap-3 p-3 rounded-xl bg-white/5 active:bg-white/10 transition-colors">
            <div className="w-10 h-10 rounded-xl bg-slate-800 flex items-center justify-center flex-shrink-0">
                <TrendingDown className="w-5 h-5 text-rose-400" />
            </div>
            <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-white truncate">{transaction.description || 'Expense'}</p>
                <p className="text-xs text-gray-500">{displayDate}</p>
            </div>
            <span className="text-rose-400 font-semibold text-sm">-{formatCurrency ? formatCurrency(transaction.amount) : `₦${transaction.amount.toLocaleString()}`}</span>
        </div>
    );
};


export const Dashboard = () => {
    const [summary, setSummary] = useState<DashboardSummary | null>(null);
    const [incomeSummary, setIncomeSummary] = useState<IncomeSummary | null>(null);
    const [predictions, setPredictions] = useState<PredictiveInsights | null>(null);
    const [achievements, setAchievements] = useState<Achievement[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [showHealthModal, setShowHealthModal] = useState(false);
    const [healthScore, setHealthScore] = useState<FinancialHealthScore | null>(null);
    const user = authService.getCurrentUser();
    const { formatCurrency } = useSettings();

    useEffect(() => {
        const fetchDashboard = async () => {
            try {
                const [data, income, preds, achievementData] = await Promise.all([
                    dashboardService.getSummary(),
                    incomeService.getSummary(),
                    analyticsService.getPredictions(),
                    analyticsService.getAchievements()
                ]);
                setSummary(data);
                setIncomeSummary(income);
                setPredictions(preds);
                setAchievements(achievementData);
            } catch (err) {
                console.error('Failed to fetch dashboard data', err);
                setError('Failed to load dashboard data');
            } finally {
                setLoading(false);
            }
        };

        fetchDashboard();
    }, []);

    const handleHealthClick = (score: FinancialHealthScore) => {
        setHealthScore(score);
        setShowHealthModal(true);
    };

    const getGreeting = () => {
        const hour = new Date().getHours();
        if (hour < 12) return 'Good morning';
        if (hour < 17) return 'Good afternoon';
        return 'Good evening';
    };

    const isEmptyAccount = !summary || (
        summary.totalExpenses === 0 &&
        summary.totalSavings === 0 &&
        (!summary.recentTransactions || summary.recentTransactions.length === 0)
    );

    const calculateCategorySpending = () => {
        if (!summary?.recentTransactions) return [];
        const categoryMap: { [key: string]: number } = {};
        summary.recentTransactions.forEach((t: any) => {
            const category = t.category || 'Miscellaneous';
            categoryMap[category] = (categoryMap[category] || 0) + t.amount;
        });
        const total = Object.values(categoryMap).reduce((sum, val) => sum + val, 0);
        const colors = ['#3b82f6', '#8b5cf6', '#ec4899', '#f59e0b', '#10b981', '#06b6d4', '#14b8a6', '#64748b'];
        return Object.entries(categoryMap)
            .map(([category, amount], index) => ({
                category,
                amount,
                percentage: total > 0 ? Math.round((amount / total) * 100) : 0,
                color: colors[index % colors.length]
            }))
            .sort((a, b) => b.amount - a.amount);
    };

    const monthComparison = {
        currentMonth: {
            expenses: summary?.totalExpenses || 0,
            savings: summary?.totalSavings || 0,
            budgetUsed: summary?.budgets && summary.budgets.length > 0
                ? Math.round((summary.budgets.reduce((sum: number, b: any) => sum + b.spentAmount, 0) /
                    summary.budgets.reduce((sum: number, b: any) => sum + b.budgetedAmount, 0)) * 100)
                : 0
        },
        lastMonth: {
            expenses: (summary?.totalExpenses || 0) * 1.12,
            savings: (summary?.totalSavings || 0) * 0.85,
            budgetUsed: 88
        }
    };

    const categorySpending = calculateCategorySpending();
    const categoryChartData = categorySpending.slice(0, 6).map(c => ({
        name: c.category,
        value: c.amount,
        color: c.color
    }));

    if (loading) {
        return (
            <div className="flex h-[60vh] items-center justify-center">
                <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
        );
    }

    if (error) {
        return (
            <div className="p-4 bg-danger/10 border border-danger/20 rounded-lg text-danger">
                {error}
            </div>
        );
    }

    return (
        <div className="space-y-6 pb-4">
            {/* Header */}
            <div>
                <h1 className="text-xl md:text-2xl font-bold text-white flex items-center gap-2">
                    {getGreeting()}, {user?.name?.split(' ')[0] || 'there'}!
                    <Sparkles className="w-5 h-5 text-yellow-500" />
                </h1>
                <p className="text-sm text-gray-400 mt-1">
                    {isEmptyAccount
                        ? "Let's get started with your finances"
                        : "Here's your financial overview"
                    }
                </p>
            </div>

            {/* Quick Actions - Mobile only */}
            {isEmptyAccount && (
                <div className="grid grid-cols-4 gap-3 md:hidden">
                    <QuickAction icon={Receipt} label="Expense" to="/expenses" color="bg-gradient-to-br from-blue-500 to-blue-600" />
                    <QuickAction icon={DollarSign} label="Income" to="/income" color="bg-gradient-to-br from-emerald-500 to-emerald-600" />
                    <QuickAction icon={Target} label="Budget" to="/budgets" color="bg-gradient-to-br from-violet-500 to-violet-600" />
                    <QuickAction icon={PiggyBank} label="Goal" to="/goals" color="bg-gradient-to-br from-pink-500 to-pink-600" />
                </div>
            )}

            {/* Stat Cards - Horizontal scroll on mobile, grid on desktop */}
            <div className="stat-scroll-container">
                <StatCard
                    title="Monthly Income"
                    value={incomeSummary?.totalMonthlyIncome ? formatCurrency(incomeSummary.totalMonthlyIncome) : formatCurrency(0)}
                    icon={DollarSign}
                    trend={incomeSummary?.totalMonthlyIncome ? 'up' : null}
                    color="secondary"
                />
                <StatCard
                    title="Expenses"
                    value={formatCurrency(summary?.totalExpenses || 0)}
                    icon={TrendingDown}
                    trend={!isEmptyAccount ? 'down' : null}
                    color="danger"
                />
                <StatCard
                    title="Net Flow"
                    value={incomeSummary ? formatCurrency(incomeSummary.netCashFlow) : formatCurrency(0)}
                    icon={BarChart3}
                    trend={incomeSummary?.netCashFlow && incomeSummary.netCashFlow >= 0 ? 'up' : 'down'}
                    color="primary"
                />
                <StatCard
                    title="Savings Rate"
                    value={incomeSummary?.savingsRate ? `${incomeSummary.savingsRate.toFixed(0)}%` : '0%'}
                    icon={Percent}
                    color="warning"
                />
                <StatCard
                    title="Savings"
                    value={formatCurrency(summary?.totalSavings || 0)}
                    icon={PiggyBank}
                    trend={!isEmptyAccount ? 'up' : null}
                    color="secondary"
                />
                <StatCard
                    title="All-Time"
                    value={incomeSummary?.totalAllTimeIncome ? formatCurrency(incomeSummary.totalAllTimeIncome) : formatCurrency(0)}
                    icon={TrendingUp}
                    color="primary"
                />
            </div>

            {/* AI Insights - Hidden on mobile, shown on desktop */}
            {!isEmptyAccount && predictions && (
                <div className="hidden md:block">
                    <AIInsightsWidget insights={predictions} formatCurrency={formatCurrency} />
                </div>
            )}

            {/* Financial Health - Simplified on mobile */}
            {!isEmptyAccount && (
                <FinancialHealthScoreWidget onClick={handleHealthClick} />
            )}

            {/* Recent Transactions - Mobile optimized */}
            {!isEmptyAccount && summary?.recentTransactions && summary.recentTransactions.length > 0 && (
                <div>
                    <div className="flex items-center justify-between mb-4">
                        <h2 className="text-lg font-bold text-white">Recent Transactions</h2>
                        <Link to="/expenses" className="text-primary text-sm flex items-center gap-1">
                            View all <ArrowRight className="w-4 h-4" />
                        </Link>
                    </div>
                    <div className="space-y-2">
                        {summary.recentTransactions.slice(0, 5).map((t: any) => (
                            <TransactionItem key={t.id} transaction={t} formatCurrency={formatCurrency} />
                        ))}
                    </div>
                </div>
            )}

            {/* Charts - Desktop only */}
            {!isEmptyAccount && (
                <div className="hidden md:grid md:grid-cols-2 gap-6">
                    <div className="glass-card-elevated p-6 min-h-[350px]">
                        <SpendingChart data={summary?.dailyTrend || []} formatCurrency={formatCurrency} />
                    </div>
                    {categorySpending.length > 0 && (
                        <CategoryBreakdownChart data={categoryChartData} formatCurrency={formatCurrency} />
                    )}
                </div>
            )}

            {/* Top Spending Widget */}
            {!isEmptyAccount && categorySpending.length > 0 && (
                <TopSpendingWidget categories={categorySpending} formatCurrency={formatCurrency} />
            )}

            {/* Month Comparison - Desktop only */}
            {!isEmptyAccount && (
                <div className="hidden md:grid md:grid-cols-2 gap-6">
                    <MonthComparison
                        currentMonth={monthComparison.currentMonth}
                        lastMonth={monthComparison.lastMonth}
                        formatCurrency={formatCurrency}
                    />
                    {achievements.filter(a => a.earned || a.progress).length > 0 && (
                        <AchievementBadges achievements={achievements} />
                    )}
                </div>
            )}

            {/* Budget Overview */}
            {!isEmptyAccount && summary?.budgets && summary.budgets.length > 0 && (
                <div className="glass-card p-4 md:p-6">
                    <div className="flex items-center justify-between mb-4">
                        <h3 className="font-semibold text-white">Budget Overview</h3>
                        <Link to="/budgets" className="text-primary text-sm flex items-center gap-1">
                            View all <ArrowRight className="w-4 h-4" />
                        </Link>
                    </div>
                    <div className="space-y-4">
                        {summary.budgets.slice(0, 3).map((budget: any) => {
                            const percentage = Math.min((budget.spentAmount / budget.budgetedAmount) * 100, 100);
                            const isOver = budget.spentAmount > budget.budgetedAmount;

                            return (
                                <div key={budget.id}>
                                    <div className="flex justify-between text-sm mb-1">
                                        <span className="text-white font-medium">{budget.category}</span>
                                        <span className="text-gray-400">
                                            {formatCurrency(budget.spentAmount)} / {formatCurrency(budget.budgetedAmount)}
                                        </span>
                                    </div>
                                    <div className="h-2 bg-slate-800 rounded-full overflow-hidden">
                                        <div
                                            className={`h-full transition-all ${isOver ? 'bg-rose-500' : 'bg-gradient-to-r from-primary to-indigo-500'}`}
                                            style={{ width: `${percentage}%` }}
                                        />
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            )}

            {/* Goals Overview */}
            {!isEmptyAccount && summary?.savingGoals && summary.savingGoals.length > 0 && (
                <div className="glass-card p-4 md:p-6">
                    <div className="flex items-center justify-between mb-4">
                        <h3 className="font-semibold text-white">Saving Goals</h3>
                        <Link to="/goals" className="text-primary text-sm flex items-center gap-1">
                            View all <ArrowRight className="w-4 h-4" />
                        </Link>
                    </div>
                    <div className="space-y-4">
                        {summary.savingGoals.slice(0, 3).map((goal: any) => {
                            const percentage = Math.min((goal.currentAmount / goal.targetAmount) * 100, 100);

                            return (
                                <div key={goal.title}>
                                    <div className="flex justify-between text-sm mb-1">
                                        <span className="text-white font-medium">{goal.title}</span>
                                        <span className="text-gray-400">{percentage.toFixed(0)}%</span>
                                    </div>
                                    <div className="h-2 bg-slate-800 rounded-full overflow-hidden">
                                        <div
                                            className="h-full bg-gradient-to-r from-secondary to-teal-400"
                                            style={{ width: `${percentage}%` }}
                                        />
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            )}

            {/* Empty State - Welcome */}
            {isEmptyAccount && (
                <div className="glass-card p-6 text-center">
                    <div className="w-16 h-16 mx-auto rounded-full bg-gradient-to-br from-primary/20 to-accent/20 flex items-center justify-center mb-4">
                        <Sparkles className="w-8 h-8 text-primary" />
                    </div>
                    <h3 className="text-lg font-bold text-white mb-2">Welcome to Expensify</h3>
                    <p className="text-sm text-gray-400 mb-6">
                        Start by adding your first expense or income to see your financial insights.
                    </p>
                    <div className="flex flex-col sm:flex-row gap-3 justify-center">
                        <Link
                            to="/expenses"
                            className="btn-glow text-white"
                        >
                            <Receipt className="w-4 h-4 mr-2" />
                            Add Expense
                        </Link>
                        <Link
                            to="/income"
                            className="btn-glow-secondary text-white"
                        >
                            <DollarSign className="w-4 h-4 mr-2" />
                            Add Income
                        </Link>
                    </div>
                </div>
            )}

            {/* FAB for quick add - Mobile only */}
            {!isEmptyAccount && (
                <Link to="/expenses" className="fab md:hidden">
                    <Plus className="w-6 h-6 text-white" />
                </Link>
            )}

            <HealthScoreModal
                isOpen={showHealthModal}
                onClose={() => setShowHealthModal(false)}
                score={healthScore}
            />
        </div>
    );
};
