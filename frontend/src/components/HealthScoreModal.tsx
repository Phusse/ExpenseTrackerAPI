import { X, TrendingUp, TrendingDown, Award, Lightbulb } from 'lucide-react';
import type { FinancialHealthScore } from '../services/analyticsService';

interface HealthScoreModalProps {
    isOpen: boolean;
    onClose: () => void;
    score: FinancialHealthScore | null;
}

export const HealthScoreModal = ({ isOpen, onClose, score }: HealthScoreModalProps) => {
    if (!isOpen || !score) return null;

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
        <>
            <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={onClose} />
            <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-xl max-h-[90vh] overflow-auto animate-slide-up md:animate-fade-in relative scrollbar-hide">
                    {/* Header */}
                    <div className="sticky top-0 z-10 bg-surface-solid/95 backdrop-blur-md p-6 border-b border-white/5 flex items-center justify-between">
                        <h2 className="text-xl font-bold text-white flex items-center gap-2">
                            Financial Health
                            <Award className="w-5 h-5 text-yellow-500" />
                        </h2>
                        <button onClick={onClose} className="p-2 text-gray-400 hover:text-white rounded-full hover:bg-white/5 transition-colors">
                            <X className="w-5 h-5" />
                        </button>
                    </div>

                    <div className="p-6 space-y-8">
                        {/* Score Circle */}
                        <div className="flex flex-col items-center justify-center py-4">
                            <div className="relative w-40 h-40 flex-shrink-0 mb-4">
                                <svg className="w-full h-full transform -rotate-90">
                                    <circle
                                        cx="50%"
                                        cy="50%"
                                        r="45%"
                                        stroke="currentColor"
                                        strokeWidth="8"
                                        fill="none"
                                        className="text-slate-800"
                                    />
                                    <circle
                                        cx="50%"
                                        cy="50%"
                                        r="45%"
                                        stroke={getScoreGradientColor(displayScore)}
                                        strokeWidth="8"
                                        fill="none"
                                        strokeLinecap="round"
                                        strokeDasharray={`${displayScore * 2.83} 283`}
                                    />
                                </svg>
                                <div className="absolute inset-0 flex flex-col items-center justify-center">
                                    <span className={`text-4xl font-bold ${getScoreColor(displayScore)}`}>
                                        {displayScore}
                                    </span>
                                    <span className="text-sm text-gray-400 font-medium uppercase tracking-wider mt-1">
                                        / 100
                                    </span>
                                </div>
                            </div>
                            <div className="text-center">
                                <h3 className={`text-2xl font-bold mb-1 ${getScoreColor(displayScore)}`}>
                                    {score.rating || 'Analyzing...'}
                                </h3>
                                <p className="text-gray-400 text-sm max-w-sm mx-auto">
                                    Your financial health is based on your savings rate, budget adherence, and spending habits.
                                </p>
                            </div>
                        </div>

                        {/* Breakdown Metrics */}
                        <div>
                            <h3 className="text-sm font-semibold text-gray-400 uppercase tracking-wider mb-4">Score Breakdown</h3>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div className="glass-card p-4">
                                    <div className="flex items-center justify-between mb-2">
                                        <span className="text-gray-300">Savings Rate</span>
                                        <TrendingUp className="w-4 h-4 text-emerald-400" />
                                    </div>
                                    <p className="text-sm text-gray-500">Target: &gt;20%</p>
                                    <div className="flex items-center gap-2 mt-2">
                                        <div className="flex-1 h-2 bg-slate-700 rounded-full overflow-hidden">
                                            <div
                                                className="h-full bg-emerald-400 rounded-full"
                                                style={{ width: `${Math.min(score.savingsScore, 100)}%` }}
                                            />
                                        </div>
                                        <span className="text-white font-medium">{score.savingsScore}/100</span>
                                    </div>
                                </div>
                                <div className="glass-card p-4">
                                    <div className="flex items-center justify-between mb-2">
                                        <span className="text-gray-300">Budget Adherence</span>
                                        <TrendingDown className="w-4 h-4 text-primary" />
                                    </div>
                                    <p className="text-sm text-gray-500">Spending vs Limits</p>
                                    <div className="flex items-center gap-2 mt-2">
                                        <div className="flex-1 h-2 bg-slate-700 rounded-full overflow-hidden">
                                            <div
                                                className="h-full bg-primary rounded-full"
                                                style={{ width: `${Math.min(score.budgetScore, 100)}%` }}
                                            />
                                        </div>
                                        <span className="text-white font-medium">{score.budgetScore}/100</span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        {/* Recommendations */}
                        {score.recommendations && score.recommendations.length > 0 && (
                            <div>
                                <h3 className="text-sm font-semibold text-gray-400 uppercase tracking-wider mb-4">Tips to Improve</h3>
                                <div className="space-y-3">
                                    {score.recommendations.map((rec, index) => (
                                        <div key={index} className="flex gap-4 p-4 rounded-xl bg-gradient-to-br from-primary/10 to-indigo-500/10 border border-primary/20">
                                            <div className="w-10 h-10 rounded-full bg-primary/20 flex items-center justify-center flex-shrink-0">
                                                <Lightbulb className="w-5 h-5 text-primary" />
                                            </div>
                                            <div>
                                                <h4 className="font-semibold text-white mb-1">Recommendation #{index + 1}</h4>
                                                <p className="text-sm text-gray-300 leading-relaxed">{rec}</p>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};
