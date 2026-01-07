// TopSpendingWidget

interface TopSpendingProps {
    categories: {
        category: string;
        amount: number;
        percentage: number;
        color: string;
    }[];
    formatCurrency?: (amount: number) => string;
}

export const TopSpendingWidget = ({ categories, formatCurrency }: TopSpendingProps) => {
    if (categories.length === 0) return null;

    const topCategories = categories.slice(0, 5);

    const formatAmount = (amount: number) => {
        if (formatCurrency) return formatCurrency(amount);
        return `₦${amount.toLocaleString()}`;
    };

    return (
        <div className="glass-card p-4 md:p-6">
            <h3 className="font-bold text-white mb-4">Top Spending</h3>
            <div className="space-y-3">
                {topCategories.map((cat, index) => (
                    <div key={cat.category} className="flex items-center gap-3">
                        <div
                            className="w-8 h-8 rounded-lg flex items-center justify-center text-xs font-bold text-white"
                            style={{ backgroundColor: cat.color }}
                        >
                            {index + 1}
                        </div>
                        <div className="flex-1 min-w-0">
                            <div className="flex justify-between items-center mb-1">
                                <span className="text-sm font-medium text-white truncate">{cat.category}</span>
                                <span className="text-sm text-gray-400 ml-2">{formatAmount(cat.amount)}</span>
                            </div>
                            <div className="h-1.5 bg-slate-800 rounded-full overflow-hidden">
                                <div
                                    className="h-full rounded-full transition-all"
                                    style={{ width: `${cat.percentage}%`, backgroundColor: cat.color }}
                                />
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};
