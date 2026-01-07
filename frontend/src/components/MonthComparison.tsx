import { TrendingUp, TrendingDown } from 'lucide-react';

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
    formatCurrency?: (amount: number) => string;
}

export const MonthComparison = ({ currentMonth, lastMonth, formatCurrency }: MonthComparisonProps) => {
    const expenseChange = lastMonth.expenses > 0
        ? ((currentMonth.expenses - lastMonth.expenses) / lastMonth.expenses * 100).toFixed(0)
        : '0';

    const savingsChange = lastMonth.savings > 0
        ? ((currentMonth.savings - lastMonth.savings) / lastMonth.savings * 100).toFixed(0)
        : '0';

    const isExpenseUp = Number(expenseChange) > 0;
    const isSavingsUp = Number(savingsChange) > 0;

    const formatAmount = (amount: number) => {
        if (formatCurrency) return formatCurrency(amount);
        return `₦${amount.toLocaleString()}`;
    };

    return (
        <div className="glass-card p-4 md:p-6">
            <h3 className="font-bold text-white mb-4">Month vs Month</h3>
            <div className="space-y-4">
                {/* Expenses Compare */}
                <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                        <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${isExpenseUp ? 'bg-rose-500/10 text-rose-400' : 'bg-emerald-500/10 text-emerald-400'}`}>
                            {isExpenseUp ? <TrendingUp className="w-5 h-5" /> : <TrendingDown className="w-5 h-5" />}
                        </div>
                        <div>
                            <p className="text-sm font-medium text-white">Expenses</p>
                            <p className="text-xs text-gray-500">{formatAmount(currentMonth.expenses)}</p>
                        </div>
                    </div>
                    <div className={`text-sm font-bold ${isExpenseUp ? 'text-rose-400' : 'text-emerald-400'}`}>
                        {isExpenseUp ? '+' : ''}{expenseChange}%
                    </div>
                </div>

                {/* Savings Compare */}
                <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                        <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${isSavingsUp ? 'bg-emerald-500/10 text-emerald-400' : 'bg-rose-500/10 text-rose-400'}`}>
                            {isSavingsUp ? <TrendingUp className="w-5 h-5" /> : <TrendingDown className="w-5 h-5" />}
                        </div>
                        <div>
                            <p className="text-sm font-medium text-white">Savings</p>
                            <p className="text-xs text-gray-500">{formatAmount(currentMonth.savings)}</p>
                        </div>
                    </div>
                    <div className={`text-sm font-bold ${isSavingsUp ? 'text-emerald-400' : 'text-rose-400'}`}>
                        {isSavingsUp ? '+' : ''}{savingsChange}%
                    </div>
                </div>

                {/* Budget Usage */}
                <div className="pt-2 border-t border-white/5">
                    <div className="flex justify-between items-center mb-2">
                        <span className="text-sm text-gray-400">Budget Used</span>
                        <span className="text-sm font-bold text-white">{currentMonth.budgetUsed}%</span>
                    </div>
                    <div className="h-2 bg-slate-800 rounded-full overflow-hidden">
                        <div
                            className={`h-full transition-all ${currentMonth.budgetUsed > 90 ? 'bg-rose-500' : currentMonth.budgetUsed > 70 ? 'bg-amber-500' : 'bg-gradient-to-r from-primary to-indigo-500'}`}
                            style={{ width: `${Math.min(currentMonth.budgetUsed, 100)}%` }}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
};
