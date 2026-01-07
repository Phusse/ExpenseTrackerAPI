import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from 'recharts';

interface CategoryData {
    name: string;
    value: number;
    color: string;
    [key: string]: string | number;
}

interface CategoryBreakdownChartProps {
    data: CategoryData[];
    formatCurrency?: (amount: number) => string;
}

export const CategoryBreakdownChart = ({ data, formatCurrency }: CategoryBreakdownChartProps) => {
    if (data.length === 0) return null;

    const total = data.reduce((sum, item) => sum + item.value, 0);

    const formatAmount = (amount: number) => {
        if (formatCurrency) return formatCurrency(amount);
        return `₦${amount.toLocaleString()}`;
    };

    return (
        <div className="glass-card p-4 md:p-6">
            <h3 className="font-bold text-white mb-4">Category Breakdown</h3>
            <div className="flex flex-col md:flex-row items-center gap-4">
                {/* Chart */}
                <div className="w-32 h-32 md:w-40 md:h-40">
                    <ResponsiveContainer width="100%" height="100%">
                        <PieChart>
                            <Pie
                                data={data}
                                cx="50%"
                                cy="50%"
                                innerRadius={30}
                                outerRadius={50}
                                dataKey="value"
                                stroke="none"
                            >
                                {data.map((entry, index) => (
                                    <Cell key={`cell-${index}`} fill={entry.color} />
                                ))}
                            </Pie>
                            <Tooltip
                                contentStyle={{
                                    backgroundColor: '#1e293b',
                                    border: '1px solid rgba(255,255,255,0.1)',
                                    borderRadius: '8px',
                                    fontSize: '12px'
                                }}
                                formatter={(value: number | string | undefined) => [formatAmount(Number(value || 0)), '']}
                            />
                        </PieChart>
                    </ResponsiveContainer>
                </div>

                {/* Legend */}
                <div className="flex-1 w-full">
                    <div className="grid grid-cols-2 gap-2">
                        {data.slice(0, 6).map((item) => (
                            <div key={item.name} className="flex items-center gap-2">
                                <div
                                    className="w-3 h-3 rounded-full flex-shrink-0"
                                    style={{ backgroundColor: item.color }}
                                />
                                <div className="min-w-0">
                                    <p className="text-xs text-white truncate">{item.name}</p>
                                    <p className="text-xs text-gray-500">
                                        {((item.value / total) * 100).toFixed(0)}%
                                    </p>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
};
