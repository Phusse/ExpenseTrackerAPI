import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { BarChart3, TrendingUp, TrendingDown, DollarSign, Loader2, Plus, Sparkles, ArrowRight, PiggyBank, Receipt, Target } from 'lucide-react';
import { dashboardService, type DashboardSummary } from '../services/dashboardService';
import { analyticsService, type FinancialHealthScore, type PredictiveInsights, type SpendingForecast } from '../services/analyticsService';
import { SpendingChart } from '../components/SpendingChart';
import { TopSpendingWidget } from '../components/TopSpendingWidget';
import { MonthComparison } from '../components/MonthComparison';
import { CategoryBreakdownChart } from '../components/CategoryBreakdownChart';
import { SmartInsights } from '../components/SmartInsights';
import { AchievementBadges } from '../components/AchievementBadges';
import { FinancialHealthScoreWidget } from '../components/FinancialHealthScoreWidget';
import { BudgetWarnings } from '../components/BudgetWarnings';
import { GoalPredictions } from '../components/GoalPredictions';
import { SmartRecommendations } from '../components/SmartRecommendations';
import { SpendingForecastWidget } from '../components/SpendingForecastWidget';
import { authService } from '../services/authService';

const StatCard = ({ title, value, icon: Icon, trend, isEmpty }: any) => (
    <div className="bg-surface border border-slate-800 rounded-xl p-6 hover:border-slate-700 transition-colors">
        <div className="flex justify-between items-start mb-4">
            <div className={`p-3 rounded-lg ${isEmpty ? 'bg-slate-800/50 text-slate-500' : 'bg-slate-900 text-primary'}`}>
                <Icon size={24} />
            </div>
            {!isEmpty && trend && (
                <span className={`flex items-center text-sm font-medium ${trend === 'up' ? 'text-emerald-500' : 'text-rose-500'}`}>
                    {trend === 'up' ? <TrendingUp size={16} /> : <TrendingDown size={16} />}
                </span>
            )}
        </div>
        <h3 className="text-slate-400 text-sm font-medium mb-1">{title}</h3>
        <p className={`text-2xl font-bold ${isEmpty ? 'text-slate-600' : 'text-white'}`}>{value}</p>
    </div>
);

const EmptyChartPlaceholder = () => (
    <div className="h-full flex flex-col items-center justify-center text-center p-6">
        <div className="w-16 h-16 rounded-full bg-gradient-to-tr from-primary/20 to-accent/20 flex items-center justify-center mb-4">
            <BarChart3 className="w-8 h-8 text-primary" />
        </div>
        <h3 className="text-white font-semibold mb-2">No Spending Data Yet</h3>
        <p className="text-gray-500 text-sm mb-4 max-w-xs">
            Start tracking your expenses to see beautiful charts and insights here.
        </p>
        <Link
            to="/expenses"
            className="inline-flex items-center gap-2 px-4 py-2 bg-primary/10 text-primary rounded-lg hover:bg-primary/20 transition-colors text-sm font-medium"
        >
            <Plus size={16} />
            Add First Expense
        </Link>
    </div>
);

const QuickActionCard = ({ icon: Icon, title, description, to, gradient }: any) => (
    <Link
        to={to}
        className="group bg-surface border border-slate-800 rounded-xl p-5 hover:border-slate-700 transition-all hover:scale-[1.02]"
    >
        <div className={`w-12 h-12 rounded-xl bg-gradient-to-tr ${gradient} flex items-center justify-center mb-4 group-hover:scale-110 transition-transform`}>
            <Icon className="w-6 h-6 text-white" />
        </div>
        <h4 className="text-white font-semibold mb-1">{title}</h4>
        <p className="text-gray-500 text-sm mb-3">{description}</p>
        <span className="text-primary text-sm font-medium inline-flex items-center gap-1 group-hover:gap-2 transition-all">
            Get Started <ArrowRight size={14} />
        </span>
    </Link>
);

export const Dashboard = () => {
    const [summary, setSummary] = useState<DashboardSummary | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const user = authService.getCurrentUser();

    useEffect(() => {
        const fetchDashboard = async () => {
            try {
                const data = await dashboardService.getSummary();
                setSummary(data);
            } catch (err) {
                console.error('Failed to fetch dashboard data', err);
                setError('Failed to load dashboard data');
            } finally {
                setLoading(false);
            }
        };

        fetchDashboard();
    }, []);

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

    // Calculate category spending for widgets
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

    // Generate smart insights
    const generateInsights = () => {
        const insights: any[] = [];

        if (summary) {
            // Budget insights
            if (summary.budgets && summary.budgets.length > 0) {
                const overBudget = summary.budgets.filter((b: any) => b.spentAmount > b.budgetedAmount);
                if (overBudget.length > 0) {
                    const first = overBudget[0];
                    const diff = first.spentAmount - first.budgetedAmount;
                    insights.push({
                        type: 'warning',
                        message: `Over budget on ${first.category} by ₦${diff.toFixed(0)}`
                    });
                }
            }

            // Savings insights
            if (summary.totalSavings > 0 && summary.totalExpenses > 0) {
                const savingsRate = (summary.totalSavings / (summary.totalExpenses + summary.totalSavings)) * 100;
                if (savingsRate > 20) {
                    insights.push({
                        type: 'success',
                        message: `Great job! You're saving ${savingsRate.toFixed(0)}% of your income`
                    });
                }
            }

            // Goals insight
            if (summary.savingGoals && summary.savingGoals.length > 0) {
                const activeGoals = summary.savingGoals.length;
                insights.push({
                    type: 'info',
                    message: `You have ${activeGoals} active saving goal${activeGoals > 1 ? 's' : ''} - keep pushing!`
                });
            }
        }

        return insights;
    };

    // Mock month comparison (in future, get from backend)
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

    // Mock achievements
    const achievements = [
        {
            id: '1',
            title: 'First Goal',
            description: 'Created your first saving goal',
            icon: 'target',
            earned: (summary?.savingGoals?.length || 0) > 0
        },
        {
            id: '2',
            title: 'Budget Master',
            description: 'Created 3 budgets',
            icon: 'trophy',
            earned: (summary?.budgets?.length || 0) >= 3
        },
        {
            id: '3',
            title: 'Expense Tracker',
            description: 'Log 10 expenses',
            icon: 'zap',
            earned: false,
            progress: summary?.recentTransactions?.length || 0,
            total: 10
        }
    ];

    const categorySpending = calculateCategorySpending();
    const insights = generateInsights();
    const categoryChartData = categorySpending.slice(0, 6).map(c => ({
        name: c.category,
        value: c.amount,
        color: c.color
    }));

    if (loading) {
        return (
            <div className="flex h-[50vh] items-center justify-center">
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
        <div className="space-y-8">
            {/* Header with Greeting */}
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-white mb-1 flex items-center gap-2">
                        {getGreeting()}, {user?.name || 'there'}!
                        <Sparkles className="w-5 h-5 text-yellow-500" />
                    </h1>
                    <p className="text-gray-400">
                        {isEmptyAccount
                            ? "Welcome to your financial dashboard. Let's get started!"
                            : "Here's what's happening with your money."
                        }
                    </p>
                </div>
                {!isEmptyAccount && (
                    <Link
                        to="/expenses"
                        className="inline-flex items-center gap-2 px-4 py-2.5 bg-gradient-to-r from-primary to-blue-600 text-white rounded-xl hover:opacity-90 transition-opacity font-medium shadow-lg shadow-primary/25"
                    >
                        <Plus size={18} />
                        Add Expense
                    </Link>
                )}
            </div>

            {/* Welcome Banner for New Users */}
            {isEmptyAccount && (
                <div className="relative overflow-hidden bg-gradient-to-r from-primary/10 via-accent/10 to-secondary/10 border border-slate-800 rounded-2xl p-8">
                    <div className="absolute top-0 right-0 w-64 h-64 bg-primary/5 rounded-full blur-3xl" />
                    <div className="absolute bottom-0 left-0 w-48 h-48 bg-accent/5 rounded-full blur-3xl" />
                    <div className="relative z-10">
                        <div className="flex items-center gap-3 mb-4">
                            <div className="w-12 h-12 rounded-xl bg-gradient-to-tr from-primary to-accent flex items-center justify-center">
                                <Sparkles className="w-6 h-6 text-white" />
                            </div>
                            <div>
                                <h2 className="text-xl font-bold text-white">Welcome to Expense Tracker!</h2>
                                <p className="text-gray-400 text-sm">Your journey to financial clarity starts here</p>
                            </div>
                        </div>
                        <p className="text-gray-300 mb-6 max-w-2xl">
                            Take control of your finances by tracking expenses, setting budgets, and achieving your savings goals.
                            Start by adding your first expense or setting up a budget for the month.
                        </p>
                        <div className="flex flex-wrap gap-3">
                            <Link
                                to="/expenses"
                                className="inline-flex items-center gap-2 px-5 py-2.5 bg-primary text-white rounded-xl hover:bg-blue-600 transition-colors font-medium"
                            >
                                <Receipt size={18} />
                                Add First Expense
                            </Link>
                            <Link
                                to="/budgets"
                                className="inline-flex items-center gap-2 px-5 py-2.5 bg-white/10 text-white rounded-xl hover:bg-white/20 transition-colors font-medium"
                            >
                                <Target size={18} />
                                Set a Budget
                            </Link>
                        </div>
                    </div>
                </div>
            )}

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                <StatCard
                    title="This Month's Expenses"
                    value={isEmptyAccount ? '₦0.00' : `₦${summary?.totalExpenses?.toFixed(2) || '0.00'}`}
                    trend={isEmptyAccount ? null : 'down'}
                    icon={TrendingDown}
                    isEmpty={isEmptyAccount}
                />
                <StatCard
                    title="This Month's Savings"
                    value={isEmptyAccount ? '₦0.00' : `₦${summary?.totalSavings?.toFixed(2) || '0.00'}`}
                    trend={isEmptyAccount ? null : 'up'}
                    icon={TrendingUp}
                    isEmpty={isEmptyAccount}
                />
                <StatCard
                    title="All-Time Expenses"
                    value={isEmptyAccount ? '₦0.00' : `₦${summary?.allTimeExpenses?.toFixed(2) || '0.00'}`}
                    trend={isEmptyAccount ? null : 'down'}
                    icon={DollarSign}
                    isEmpty={isEmptyAccount}
                />
                <StatCard
                    title="All-Time Savings"
                    value={isEmptyAccount ? '₦0.00' : `₦${summary?.allTimeSavings?.toFixed(2) || '0.00'}`}
                    trend={isEmptyAccount ? null : ((summary?.allTimeSavings || 0) >= 0 ? 'up' : 'down')}
                    icon={BarChart3}
                    isEmpty={isEmptyAccount}
                />
            </div>

            {/* Financial Health Score */}
            <FinancialHealthScoreWidget />

            {/* Smart Insights */}
            {!isEmptyAccount && insights.length > 0 && (
                <SmartInsights insights={insights} />
            )}

            {/* Content Area */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Chart */}
                <div className="lg:col-span-2 bg-surface border border-slate-800 rounded-xl p-6 h-[400px]">
                    {isEmptyAccount ? (
                        <EmptyChartPlaceholder />
                    ) : (
                        <SpendingChart data={summary?.dailyTrend || []} />
                    )}
                </div>

                {/* Recent Transactions or Quick Actions */}
                <div className="bg-surface border border-slate-800 rounded-xl p-6 h-[400px]">
                    {isEmptyAccount ? (
                        <div className="h-full flex flex-col">
                            <h3 className="font-semibold text-white mb-4">Quick Actions</h3>
                            <div className="flex-1 flex flex-col justify-center gap-4">
                                <QuickActionCard
                                    icon={Receipt}
                                    title="Track Expenses"
                                    description="Log your daily spending"
                                    to="/expenses"
                                    gradient="from-blue-500 to-blue-600"
                                />
                                <QuickActionCard
                                    icon={PiggyBank}
                                    title="Set Goals"
                                    description="Save for what matters"
                                    to="/goals"
                                    gradient="from-emerald-500 to-emerald-600"
                                />
                            </div>
                        </div>
                    ) : (
                        <>
                            <h3 className="font-semibold text-white mb-4">Recent Transactions</h3>
                            {summary?.recentTransactions?.length === 0 ? (
                                <div className="h-full flex items-center justify-center text-gray-500">No recent transactions</div>
                            ) : (
                                <div className="space-y-4 overflow-y-auto h-[320px] pr-2 custom-scrollbar">
                                    {summary?.recentTransactions?.map((t: any) => (
                                        <div key={t.id} className="flex justify-between items-center p-3 hover:bg-white/5 rounded-lg transition-colors border border-transparent hover:border-slate-800">
                                            <div className="flex items-center gap-3">
                                                <div className="p-2 bg-slate-900 rounded-md text-gray-400">
                                                    <TrendingDown size={16} />
                                                </div>
                                                <div>
                                                    <p className="text-white font-medium text-sm">{t.description || 'Expense'}</p>
                                                    <p className="text-xs text-gray-500">
                                                        {t.dateOfExpense && new Date(t.dateOfExpense).getFullYear() > 1
                                                            ? new Date(t.dateOfExpense).toLocaleDateString()
                                                            : 'No date'}
                                                    </p>
                                                </div>
                                            </div>
                                            <span className="text-rose-500 font-semibold text-sm">-₦{t.amount.toFixed(2)}</span>
                                        </div>
                                    ))}
                                </div>
                            )}
                        </>
                    )}
                </div>
            </div>

            {/* Category Breakdown & Top Spending */}
            {!isEmptyAccount && categorySpending.length > 0 && (
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                    <CategoryBreakdownChart data={categoryChartData} />
                    <TopSpendingWidget categories={categorySpending} />
                </div>
            )}

            {/* Month Comparison &  Achievements */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                {!isEmptyAccount && monthComparison && (
                    <MonthComparison
                        currentMonth={monthComparison.currentMonth}
                        lastMonth={monthComparison.lastMonth}
                    />
                )}

                {achievements.filter(a => a.earned || a.progress).length > 0 && (
                    <AchievementBadges achievements={achievements} />
                )}
            </div>

            {/* Budget Overview Section */}
            {!isEmptyAccount && summary?.budgets && summary.budgets.length > 0 && (
                <div className="bg-surface border border-slate-800 rounded-xl p-6">
                    <div className="flex justify-between items-center mb-6">
                        <div>
                            <h3 className="font-semibold text-white text-lg">Budget Overview</h3>
                            <p className="text-gray-400 text-sm">Your spending against budgets this month</p>
                        </div>
                        <Link
                            to="/budgets"
                            className="text-primary hover:text-blue-600 text-sm font-medium inline-flex items-center gap-1"
                        >
                            View All <ArrowRight size={14} />
                        </Link>
                    </div>
                    <div className="space-y-4">
                        {summary.budgets.map((budget: any) => {
                            const percentage = Math.min(100, (budget.spentAmount / budget.budgetedAmount) * 100);
                            const remaining = budget.budgetedAmount - budget.spentAmount;
                            const isOverBudget = percentage > 100;
                            const isNearLimit = percentage > 80 && !isOverBudget;

                            return (
                                <div key={budget.id} className="space-y-2">
                                    <div className="flex justify-between items-center">
                                        <div className="flex items-center gap-2">
                                            <span className="text-white font-medium">{budget.category}</span>
                                            {isOverBudget && (
                                                <span className="text-xs bg-rose-500/10 text-rose-500 px-2 py-0.5 rounded">Over Budget</span>
                                            )}
                                            {isNearLimit && (
                                                <span className="text-xs bg-amber-500/10 text-amber-500 px-2 py-0.5 rounded">Near Limit</span>
                                            )}
                                        </div>
                                        <span className="text-gray-400 text-sm">
                                            ₦{budget.spentAmount.toFixed(2)} / ₦{budget.budgetedAmount.toFixed(2)}
                                        </span>
                                    </div>
                                    <div className="h-2 bg-slate-900 rounded-full overflow-hidden">
                                        <div
                                            className={`h-full transition-all ${isOverBudget ? 'bg-rose-500' :
                                                isNearLimit ? 'bg-amber-500' :
                                                    'bg-gradient-to-r from-primary to-blue-600'
                                                }`}
                                            style={{ width: `${Math.min(percentage, 100)}%` }}
                                        />
                                    </div>
                                    <div className="flex justify-between text-xs">
                                        <span className={isOverBudget ? 'text-rose-500' : 'text-gray-500'}>
                                            {percentage.toFixed(0)}% used
                                        </span>
                                        <span className={remaining < 0 ? 'text-rose-500' : 'text-emerald-500'}>
                                            {remaining >= 0 ? `₦${remaining.toFixed(2)} remaining` : `₦${Math.abs(remaining).toFixed(2)} over`}
                                        </span>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            )}

            {/* Saving Goals Overview Section */}
            {!isEmptyAccount && summary?.savingGoals && summary.savingGoals.length > 0 && (
                <div className="bg-surface border border-slate-800 rounded-xl p-6">
                    <div className="flex justify-between items-center mb-6">
                        <div>
                            <h3 className="font-semibold text-white text-lg">Saving Goals Progress</h3>
                            <p className="text-gray-400 text-sm">Track your progress towards financial goals</p>
                        </div>
                        <Link
                            to="/goals"
                            className="text-primary hover:text-blue-600 text-sm font-medium inline-flex items-center gap-1"
                        >
                            View All <ArrowRight size={14} />
                        </Link>
                    </div>
                    <div className="space-y-4">
                        {summary.savingGoals.map((goal: any) => {
                            const percentage = Math.min(100, (goal.currentAmount / goal.targetAmount) * 100);
                            const isComplete = percentage >= 100;
                            const daysLeft = goal.deadline ? Math.ceil((new Date(goal.deadline).getTime() - Date.now()) / (1000 * 60 * 60 * 24)) : 0;

                            return (
                                <div key={goal.title} className="space-y-2">
                                    <div className="flex justify-between items-center">
                                        <div className="flex items-center gap-2">
                                            <span className="text-white font-medium">{goal.title}</span>
                                            {isComplete && (
                                                <span className="text-xs bg-emerald-500/10 text-emerald-500 px-2 py-0.5 rounded">🎉 Achieved!</span>
                                            )}
                                            {!isComplete && daysLeft > 0 && daysLeft <= 30 && (
                                                <span className="text-xs bg-amber-500/10 text-amber-500 px-2 py-0.5 rounded">{daysLeft} days left</span>
                                            )}
                                        </div>
                                        <span className="text-gray-400 text-sm">
                                            ₦{goal.currentAmount.toFixed(2)} / ₦{goal.targetAmount.toFixed(2)}
                                        </span>
                                    </div>
                                    <div className="h-2 bg-slate-900 rounded-full overflow-hidden">
                                        <div
                                            className={`h-full transition-all ${isComplete ? 'bg-emerald-500' :
                                                'bg-gradient-to-r from-emerald-500 to-teal-400'
                                                }`}
                                            style={{ width: `${Math.min(percentage, 100)}%` }}
                                        />
                                    </div>
                                    <div className="flex justify-between text-xs">
                                        <span className={isComplete ? 'text-emerald-400' : 'text-gray-500'}>
                                            {percentage.toFixed(0)}% complete
                                        </span>
                                        <span className="text-gray-500">
                                            ₦{(goal.targetAmount - goal.currentAmount).toFixed(2)} to go
                                        </span>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            )}

            {/* Quick Start Guide for Empty Account */}
            {isEmptyAccount && (
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <div className="bg-surface border border-slate-800 rounded-xl p-6 hover:border-slate-700 transition-colors">
                        <div className="w-10 h-10 rounded-lg bg-blue-500/10 text-blue-500 flex items-center justify-center mb-4 text-lg font-bold">
                            1
                        </div>
                        <h4 className="text-white font-semibold mb-2">Add Your Expenses</h4>
                        <p className="text-gray-500 text-sm">Start logging your daily expenses to get insights into your spending habits.</p>
                    </div>
                    <div className="bg-surface border border-slate-800 rounded-xl p-6 hover:border-slate-700 transition-colors">
                        <div className="w-10 h-10 rounded-lg bg-emerald-500/10 text-emerald-500 flex items-center justify-center mb-4 text-lg font-bold">
                            2
                        </div>
                        <h4 className="text-white font-semibold mb-2">Set Your Budgets</h4>
                        <p className="text-gray-500 text-sm">Create monthly budgets for different categories to control your spending.</p>
                    </div>
                    <div className="bg-surface border border-slate-800 rounded-xl p-6 hover:border-slate-700 transition-colors">
                        <div className="w-10 h-10 rounded-lg bg-violet-500/10 text-violet-500 flex items-center justify-center mb-4 text-lg font-bold">
                            3
                        </div>
                        <h4 className="text-white font-semibold mb-2">Achieve Your Goals</h4>
                        <p className="text-gray-500 text-sm">Set savings goals and track your progress towards financial freedom.</p>
                    </div>
                </div>
            )}
        </div>
    );
};
