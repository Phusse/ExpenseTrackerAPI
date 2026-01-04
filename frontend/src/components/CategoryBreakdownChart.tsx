import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip, Legend } from 'recharts';

interface CategoryData {
    name: string;
    value: number;
    color: string;
}

interface CategoryBreakdownChartProps {
    data: CategoryData[];
}

const COLORS = {
    'Food': '#3b82f6',
    'Transport': '#8b5cf6',
    'Health': '#ec4899',
    'Entertainment': '#f59e0b',
    'Utilities': '#10b981',
    'Education': '#06b6d4',
    'Savings': '#14b8a6',
    'Investments': '#6366f1',
    'Miscellaneous': '#64748b',
};

export const CategoryBreakdownChart = ({ data }: CategoryBreakdownChartProps) => {
    if (!data || data.length === 0) {
        return (
            <div className="bg-surface border border-slate-800 rounded-xl p-6 h-[400px] flex items-center justify-center">
                <p className="text-gray-500">No category data available</p>
            </div>
        );
    }

    const chartData = data.map(item => ({
        ...item,
        color: COLORS[item.name as keyof typeof COLORS] || '#64748b'
    }));

    const CustomTooltip = ({ active, payload }: any) => {
        if (active && payload && payload.length) {
            return (
                <div className="bg-slate-900 border border-slate-700 rounded-lg p-3 shadow-lg">
                    <p className="text-white font-medium">{payload[0].name}</p>
                    <p className="text-primary font-semibold">₦{payload[0].value.toLocaleString()}</p>
                    <p className="text-gray-400 text-xs">
                        {((payload[0].value / data.reduce((sum, item) => sum + item.value, 0)) * 100).toFixed(1)}%
                    </p>
                </div>
            );
        }
        return null;
    };

    return (
        <div className="bg-surface border border-slate-800 rounded-xl p-6">
            <h3 className="font-semibold text-white mb-4">Spending by Category</h3>
            <ResponsiveContainer width="100%" height={320}>
                <PieChart>
                    <Pie
                        data={chartData}
                        cx="50%"
                        cy="50%"
                        innerRadius={60}
                        outerRadius={100}
                        paddingAngle={2}
                        dataKey="value"
                    >
                        {chartData.map((entry, index) => (
                            <Cell key={`cell-${index}`} fill={entry.color} stroke="#0f172a" strokeWidth={2} />
                        ))}
                    </Pie>
                    <Tooltip content={<CustomTooltip />} />
                </PieChart>
            </ResponsiveContainer>

            {/* Legend */}
            <div className="grid grid-cols-2 gap-2 mt-4">
                {chartData.slice(0, 6).map((entry) => (
                    <div key={entry.name} className="flex items-center gap-2">
                        <div className="w-3 h-3 rounded-full" style={{ backgroundColor: entry.color }} />
                        <span className="text-xs text-gray-400">{entry.name}</span>
                        <span className="text-xs text-white ml-auto">₦{entry.value.toLocaleString()}</span>
                    </div>
                ))}
            </div>
        </div>
    );
};
