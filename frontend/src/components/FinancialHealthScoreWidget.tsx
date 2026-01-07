import { useState, useEffect } from 'react';
import { analyticsService, type FinancialHealthScore } from '../services/analyticsService';

interface Props {
    onClick?: (score: FinancialHealthScore) => void;
}

export const FinancialHealthScoreWidget = ({ onClick }: Props) => {
    const [score, setScore] = useState<FinancialHealthScore | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchScore = async () => {
            try {
                const data = await analyticsService.getHealthScore();
                setScore(data);
            } catch (error) {
                console.error('Failed to fetch health score', error);
            } finally {
                setLoading(false);
            }
        };
        fetchScore();
    }, []);

    if (loading) {
        return (
            <div className="glass-card p-4 animate-pulse">
                <div className="h-20 bg-slate-800 rounded-xl" />
            </div>
        );
    }

    if (!score) return null;

    // Use the actual backend property: totalScore
    const displayScore = score.totalScore || 0;

    const getScoreColor = (s: number) => {
        if (s >= 80) return 'text-emerald-400';
        if (s >= 60) return 'text-amber-400';
        return 'text-rose-400';
    };

    const getScoreGradientColor = (s: number) => {
        if (s >= 80) return '#10b981';
        if (s >= 60) return '#f59e0b';
        return '#ef4444';
    };

    return (
        <div
            className={`glass-card p-4 md:p-6 transition-transform active:scale-[0.98] ${onClick ? 'cursor-pointer hover:bg-white/5' : ''}`}
            onClick={() => onClick?.(score)}
        >
            <div className="flex items-center gap-4">
                {/* Score Circle */}
                <div className="relative w-16 h-16 md:w-20 md:h-20 flex-shrink-0">
                    <svg className="w-full h-full transform -rotate-90">
                        <circle
                            cx="50%"
                            cy="50%"
                            r="45%"
                            stroke="currentColor"
                            strokeWidth="6"
                            fill="none"
                            className="text-slate-800"
                        />
                        <circle
                            cx="50%"
                            cy="50%"
                            r="45%"
                            stroke={getScoreGradientColor(displayScore)}
                            strokeWidth="6"
                            fill="none"
                            strokeLinecap="round"
                            strokeDasharray={`${displayScore * 2.83} 283`}
                        />
                    </svg>
                    <div className="absolute inset-0 flex items-center justify-center">
                        <span className={`text-lg md:text-xl font-bold ${getScoreColor(displayScore)}`}>
                            {displayScore}
                        </span>
                    </div>
                </div>

                {/* Details */}
                <div className="flex-1 min-w-0">
                    <h3 className="font-bold text-white mb-1">Financial Health</h3>
                    <p className="text-sm text-gray-400 mb-1">{score.rating || 'Calculating...'}</p>
                    <p className="text-xs text-gray-500 capitalize">Trend: {score.trend || 'stable'}</p>
                    {score.recommendations && score.recommendations.length > 0 && (
                        <p className="text-xs text-primary mt-1 truncate">{score.recommendations[0]}</p>
                    )}
                </div>
            </div>
        </div>
    );
};
