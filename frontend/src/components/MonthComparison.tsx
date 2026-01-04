import { TrendingUp, TrendingDown, Minus } from 'lucide-react';

interface MonthComparisonProps {
    currentMonth: {
        expenses: number;
        savings: number;
        budgetUsed: number;
    };
    lastMonth: {
        expenses: number;
        savings: number;
        budgetUsed: number;
    };
}

export const MonthComparison = ({ currentMonth, lastMonth }: MonthComparisonProps) => {
    const expenseChange = ((currentMonth.expenses - lastMonth.expenses) / lastMonth.expenses) * 100;
    const savingsChange = ((currentMonth.savings - lastMonth.savings) / lastMonth.savings) * 100;
    const budgetChange = currentMonth.budgetUsed - lastMonth.budgetUsed;

    const getChangeIcon = (change: number) => {
        if (change > 5) return <TrendingUp className="w-4 h-4" />;
        if (change < -5) return <TrendingDown className="w-4 h-4" />;
        return <Minus className="w-4 h-4" />;
    };

    const getChangeColor = (change: number, inverse: boolean = false) => {
        const isPositive = inverse ? change < 0 : change > 0;
        if (Math.abs(change) < 5) return 'text-gray-400';
        return isPositive ? 'text-emerald-400' : 'text-rose-400';
    };

    return (
        <div className="bg-surface border border-slate-800 rounded-xl p-6">
            <h3 className="font-semibold text-white mb-4">This Month vs Last Month</h3>

            <div className="space-y-4">
                {/* Expenses */}
                <div className="flex items-center justify-between p-3 bg-slate-900/50 rounded-lg border border-slate-800">
                    <div>
                        <p className="text-sm text-gray-400 mb-1">Expenses</p>
                        <p className="text-lg font-semibold text-white">₦{currentMonth.expenses.toLocaleString()}</p>
                    </div>
                    <div className={`flex items-center gap-2 px-3 py-1.5 rounded-full text-sm font-medium ${expenseChange > 0 ? 'bg-rose-500/10 text-rose-400' : expenseChange < 0 ? 'bg-emerald-500/10 text-emerald-400' : 'bg-gray-500/10 text-gray-400'
                        }`}>
                        {getChangeIcon(expenseChange)}
                        <span>{Math.abs(expenseChange).toFixed(1)}%</span>
                    </div>
                </div>

                {/* Savings */}
                <div className="flex items-center justify-between p-3 bg-slate-900/50 rounded-lg border border-slate-800">
                    <div>
                        <p className="text-sm text-gray-400 mb-1">Savings</p>
                        <p className="text-lg font-semibold text-white">₦{currentMonth.savings.toLocaleString()}</p>
                    </div>
                    <div className={`flex items-center gap-2 px-3 py-1.5 rounded-full text-sm font-medium ${savingsChange > 0 ? 'bg-emerald-500/10 text-emerald-400' : savingsChange < 0 ? 'bg-rose-500/10 text-rose-400' : 'bg-gray-500/10 text-gray-400'
                        }`}>
                        {getChangeIcon(savingsChange)}
                        <span>{Math.abs(savingsChange).toFixed(1)}%</span>
                    </div>
                </div>

                {/* Budget Usage */}
                <div className="flex items-center justify-between p-3 bg-slate-900/50 rounded-lg border border-slate-800">
                    <div>
                        <p className="text-sm text-gray-400 mb-1">Budget Used</p>
                        <p className="text-lg font-semibold text-white">{currentMonth.budgetUsed}%</p>
                    </div>
                    <div className={`flex items-center gap-2 px-3 py-1.5 rounded-full text-sm font-medium ${budgetChange > 0 ? 'bg-rose-500/10 text-rose-400' : budgetChange < 0 ? 'bg-emerald-500/10 text-emerald-400' : 'bg-gray-500/10 text-gray-400'
                        }`}>
                        {getChangeIcon(budgetChange)}
                        <span>{Math.abs(budgetChange).toFixed(0)}pts</span>
                    </div>
                </div>
            </div>
        </div>
    );
};
