import { TrendingUp } from 'lucide-react';

interface CategorySpending {
    category: string;
    amount: number;
    percentage: number;
    color: string;
}

interface TopSpendingWidgetProps {
    categories: CategorySpending[];
}

export const TopSpendingWidget = ({ categories }: TopSpendingWidgetProps) => {
    if (!categories || categories.length === 0) {
        return (
            <div className="bg-surface border border-slate-800 rounded-xl p-6">
                <h3 className="font-semibold text-white mb-4">Top Spending Categories</h3>
                <p className="text-gray-500 text-sm">No spending data available</p>
            </div>
        );
    }

    const topCategories = categories.slice(0, 5);

    return (
        <div className="bg-surface border border-slate-800 rounded-xl p-6">
            <div className="flex items-center justify-between mb-4">
                <h3 className="font-semibold text-white">Top Spending Categories</h3>
                <TrendingUp className="w-5 h-5 text-primary" />
            </div>

            <div className="space-y-4">
                {topCategories.map((cat, index) => (
                    <div key={cat.category} className="space-y-2">
                        <div className="flex items-center justify-between">
                            <div className="flex items-center gap-3">
                                <span className="text-lg font-bold text-gray-600">#{index + 1}</span>
                                <span className="text-white font-medium">{cat.category}</span>
                            </div>
                            <div className="text-right">
                                <p className="text-white font-semibold">₦{cat.amount.toLocaleString()}</p>
                                <p className="text-xs text-gray-500">{cat.percentage}%</p>
                            </div>
                        </div>
                        <div className="h-2 bg-slate-900 rounded-full overflow-hidden">
                            <div
                                className="h-full rounded-full transition-all"
                                style={{
                                    width: `${cat.percentage}%`,
                                    backgroundColor: cat.color
                                }}
                            />
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};
