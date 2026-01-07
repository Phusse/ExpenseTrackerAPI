import { AreaChart, Area, XAxis, YAxis, Tooltip, ResponsiveContainer } from 'recharts';

interface DailySpending {
    date?: string;
    Date?: string;
    amount?: number;
    totalSpent?: number;
    TotalSpent?: number;
}

interface SpendingChartProps {
    data: DailySpending[];
    formatCurrency?: (amount: number) => string;
}

export const SpendingChart = ({ data, formatCurrency }: SpendingChartProps) => {
    if (!data || data.length === 0) {
        return (
            <div className="h-full flex items-center justify-center text-gray-500">
                No spending data available
            </div>
        );
    }

    const formatAmount = (amount: number) => {
        if (formatCurrency) return formatCurrency(amount);
        return `₦${amount.toLocaleString()}`;
    };

    const formatAxisAmount = (value: number) => {
        if (formatCurrency) {
            const formatted = formatCurrency(value);
            // Shorten for axis (e.g., "$1.5k" instead of "$1,500")
            if (value >= 1000) {
                const symbol = formatted.charAt(0);
                return `${symbol}${(value / 1000).toFixed(0)}k`;
            }
            return formatted;
        }
        return `₦${(value / 1000).toFixed(0)}k`;
    };

    // Normalize data - handle both frontend (date, amount) and backend (Date, TotalSpent) formats
    const chartData = data.map(item => {
        // Get date from any available property
        const dateValue = item.date || item.Date || '';
        // Get amount from any available property
        const amountValue = item.amount ?? item.totalSpent ?? item.TotalSpent ?? 0;

        // Format date for display
        let formattedDate = dateValue;
        try {
            if (dateValue) {
                const dateObj = new Date(dateValue);
                if (!isNaN(dateObj.getTime())) {
                    formattedDate = dateObj.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
                }
            }
        } catch {
            // Keep original if parsing fails
        }

        return {
            date: formattedDate,
            amount: amountValue
        };
    });

    return (
        <div className="h-full">
            <h3 className="font-bold text-white mb-4">Spending Trend</h3>
            <div className="h-[calc(100%-40px)]">
                <ResponsiveContainer width="100%" height="100%">
                    <AreaChart data={chartData} margin={{ top: 10, right: 10, left: 0, bottom: 0 }}>
                        <defs>
                            <linearGradient id="spendingGradient" x1="0" y1="0" x2="0" y2="1">
                                <stop offset="5%" stopColor="#6366f1" stopOpacity={0.3} />
                                <stop offset="95%" stopColor="#6366f1" stopOpacity={0} />
                            </linearGradient>
                        </defs>
                        <XAxis
                            dataKey="date"
                            axisLine={false}
                            tickLine={false}
                            tick={{ fill: '#64748b', fontSize: 11 }}
                            interval="preserveStartEnd"
                        />
                        <YAxis
                            axisLine={false}
                            tickLine={false}
                            tick={{ fill: '#64748b', fontSize: 11 }}
                            tickFormatter={formatAxisAmount}
                            width={50}
                        />
                        <Tooltip
                            contentStyle={{
                                backgroundColor: '#1e293b',
                                border: '1px solid rgba(255,255,255,0.1)',
                                borderRadius: '12px',
                                padding: '8px 12px'
                            }}
                            labelStyle={{ color: '#94a3b8', fontSize: 12 }}
                            formatter={(value: number | string | undefined) => [formatAmount(Number(value || 0)), 'Spent']}
                        />
                        <Area
                            type="monotone"
                            dataKey="amount"
                            stroke="#6366f1"
                            strokeWidth={2}
                            fill="url(#spendingGradient)"
                        />
                    </AreaChart>
                </ResponsiveContainer>
            </div>
        </div>
    );
};
