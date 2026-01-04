import { useEffect, useState } from 'react';
import { Trophy, TrendingUp, TrendingDown, Minus, Loader2 } from 'lucide-react';
import { analyticsService, type FinancialHealthScore } from '../services/analyticsService';

export const FinancialHealthScoreWidget = () => {
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
            <div className="bg-surface border border-slate-800 rounded-xl p-6 flex items-center justify-center h-[280px]">
                <Loader2 className="w-8 h-8 animate-spin text-primary" />
            </div>
        );
    }

    if (!score) return null;

    const getScoreColor = (totalScore: number) => {
        if (totalScore >= 70) return 'text-emerald-400';
        if (totalScore >= 50) return 'text-blue-400';
        if (totalScore >= 30) return 'text-amber-400';
        return 'text-rose-400';
    };

    const getScoreGradient = (totalScore: number) => {
        if (totalScore >= 70) return 'from-emerald-500 to-teal-500';
        if (totalScore >= 50) return 'from-blue-500 to-cyan-500';
        if (totalScore >= 30) return 'from-amber-500 to-orange-500';
        return 'from-rose-500 to-red-500';
    };

    const getRatingIcon = (rating: string) => {
        if (rating === "Excellent") return Trophy;
        if (score.trend === "improving") return TrendingUp;
        if (score.trend === "declining") return TrendingDown;
        return Minus;
    };

    const circumference = 2 * Math.PI * 70;
    const strokeDashoffset = circumference - (score.totalScore / 100) * circumference;

    const Icon = getRatingIcon(score.rating);

    return (
        <div className="bg-surface border border-slate-800 rounded-xl p-6">
            <h3 className="font-semibold text-white mb-6 text-lg">Financial Health Score</h3>

            <div className="flex items-center gap-8">
                {/* Circular Progress */}
                <div className="relative w-[160px] h-[160px]">
                    <svg className="w-full h-full transform -rotate-90">
                        {/* Background circle */}
                        <circle
                            cx="80"
                            cy="80"
                            r="70"
                            stroke="#1e293b"
                            strokeWidth="12"
                            fill="none"
                        />
                        {/* Progress circle */}
                        <circle
                            cx="80"
                            cy="80"
                            r="70"
                            stroke="url(#scoreGradient)"
                            strokeWidth="12"
                            fill="none"
                            strokeDasharray={circumference}
                            strokeDashoffset={strokeDashoffset}
                            strokeLinecap="round"
                            className="transition-all duration-1000"
                        />
                        <defs>
                            <linearGradient id="scoreGradient" x1="0%" y1="0%" x2="100%" y2="0%">
                                <stop offset="0%" className={getScoreGradient(score.totalScore).split(' ')[0].replace('from-', '')} />
                                <stop offset="100%" className={getScoreGradient(score.totalScore).split(' ')[1].replace('to-', '')} />
                            </linearGradient>
                        </defs>
                    </svg>

                    {/* Score text */}
                    <div className="absolute inset-0 flex flex-col items-center justify-center">
                        <span className={`text-4xl font-bold ${getScoreColor(score.totalScore)}`}>
                            {score.totalScore}
                        </span>
                        <span className="text-xs text-gray-500 mt-1">out of 100</span>
                    </div>
                </div>

                {/* Score breakdown */}
                <div className="flex-1 space-y-3">
                    <div className="flex items-center justify-between">
                        <div className="flex items-center gap-2">
                            <Icon className="w-5 h-5 text-primary" />
                            <span className={`text-lg font-semibold ${getScoreColor(score.totalScore)}`}>
                                {score.rating}
                            </span>
                        </div>
                        <span className="text-xs text-gray-500 uppercase tracking-wide">Rating</span>
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <ScoreItem label="Savings" score={score.savingsScore} max={30} />
                        <ScoreItem label="Budget" score={score.budgetScore} max={25} />
                        <ScoreItem label="Goals" score={score.goalScore} max={20} />
                        <ScoreItem label="Trend" score={score.trendScore} max={15} />
                    </div>

                    {score.recommendations.length > 0 && (
                        <div className="mt-4 p-3 bg-slate-900/50 rounded-lg border border-slate-800">
                            <p className="text-xs text-gray-400 mb-2 font-medium">Top Tip:</p>
                            <p className="text-sm text-gray-300">{score.recommendations[0]}</p>
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

const ScoreItem = ({ label, score, max }: { label: string; score: number; max: number }) => {
    const percentage = (score / max) * 100;

    return (
        <div>
            <div className="flex items-center justify-between mb-1">
                <span className="text-xs text-gray-400">{label}</span>
                <span className="text-xs text-white font-medium">{score}/{max}</span>
            </div>
            <div className="h-1.5 bg-slate-900 rounded-full overflow-hidden">
                <div
                    className="h-full bg-gradient-to-r from-primary to-accent transition-all"
                    style={{ width: `${percentage}%` }}
                />
            </div>
        </div>
    );
};
