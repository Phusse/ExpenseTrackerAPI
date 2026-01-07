import { Sparkles, AlertTriangle, Lightbulb, Target, PiggyBank } from 'lucide-react';
import type { PredictiveInsights } from '../services/analyticsService';

interface AIInsightsWidgetProps {
    insights: PredictiveInsights;
    formatCurrency?: (amount: number) => string;
}

export const AIInsightsWidget = ({ insights, formatCurrency }: AIInsightsWidgetProps) => {
    if (!insights) return null;

    const formatAmount = (amount: number) => {
        if (formatCurrency) return formatCurrency(amount);
        return `₦${amount.toLocaleString()}`;
    };

    // Check if there's any data to display
    const hasBudgetWarnings = insights.budgetWarnings && insights.budgetWarnings.length > 0;
    const hasGoalPredictions = insights.goalPredictions && insights.goalPredictions.length > 0;
    const hasRecommendations = insights.recommendations && insights.recommendations.length > 0;
    const hasSavingsOpportunities = insights.savingsOpportunities && insights.savingsOpportunities.length > 0;

    if (!hasBudgetWarnings && !hasGoalPredictions && !hasRecommendations && !hasSavingsOpportunities) {
        return (
            <div className="glass-card p-4 md:p-6">
                <div className="flex items-center gap-2 mb-4">
                    <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-violet-500 to-purple-600 flex items-center justify-center">
                        <Sparkles className="w-4 h-4 text-white" />
                    </div>
                    <h3 className="font-bold text-white">AI Insights</h3>
                </div>
                <p className="text-sm text-gray-400">Add more expenses and budgets to get personalized insights.</p>
            </div>
        );
    }

    const getSeverityIcon = (severity: string) => {
        switch (severity) {
            case 'critical': return <AlertTriangle className="w-4 h-4 text-rose-400" />;
            case 'warning': return <AlertTriangle className="w-4 h-4 text-amber-400" />;
            default: return <Lightbulb className="w-4 h-4 text-primary" />;
        }
    };

    return (
        <div className="glass-card p-4 md:p-6">
            <div className="flex items-center gap-2 mb-4">
                <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-violet-500 to-purple-600 flex items-center justify-center">
                    <Sparkles className="w-4 h-4 text-white" />
                </div>
                <h3 className="font-bold text-white">AI Insights</h3>
            </div>

            {/* Budget Warnings */}
            {hasBudgetWarnings && (
                <div className="space-y-2 mb-4">
                    {insights.budgetWarnings.slice(0, 2).map((warning, index) => (
                        <div
                            key={index}
                            className={`flex items-start gap-3 p-2 rounded-lg ${warning.severity === 'critical' ? 'bg-rose-500/10' : 'bg-amber-500/10'}`}
                        >
                            <div className="mt-0.5">
                                {getSeverityIcon(warning.severity)}
                            </div>
                            <div className="flex-1 min-w-0">
                                <p className="text-sm text-gray-300">{warning.message}</p>
                                <p className="text-xs text-gray-500 mt-0.5">{warning.category}</p>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {/* Goal Predictions */}
            {hasGoalPredictions && (
                <div className="space-y-2 mb-4">
                    {insights.goalPredictions.slice(0, 2).map((goal, index) => (
                        <div
                            key={index}
                            className="flex items-start gap-3 p-2 rounded-lg bg-emerald-500/10"
                        >
                            <div className="mt-0.5">
                                <Target className="w-4 h-4 text-emerald-400" />
                            </div>
                            <div className="flex-1 min-w-0">
                                <p className="text-sm text-gray-300">{goal.message}</p>
                                <p className="text-xs text-gray-500 mt-0.5">{goal.goalTitle} - {goal.status}</p>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {/* Savings Opportunities */}
            {hasSavingsOpportunities && (
                <div className="space-y-2 mb-4">
                    {insights.savingsOpportunities.slice(0, 2).map((opp, index) => (
                        <div
                            key={index}
                            className="flex items-start gap-3 p-2 rounded-lg bg-blue-500/10"
                        >
                            <div className="mt-0.5">
                                <PiggyBank className="w-4 h-4 text-blue-400" />
                            </div>
                            <div className="flex-1 min-w-0">
                                <p className="text-sm text-gray-300">{opp.message}</p>
                                <p className="text-xs text-emerald-400 mt-0.5">
                                    Save {formatAmount(opp.potentialMonthlySavings || 0)}/month
                                </p>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {/* Recommendations */}
            {hasRecommendations && (
                <div className="pt-3 border-t border-white/5">
                    <p className="text-xs text-gray-400 mb-2">Recommendations</p>
                    <ul className="space-y-1">
                        {insights.recommendations.slice(0, 2).map((rec, index) => (
                            <li key={index} className="text-sm text-gray-300 flex items-start gap-2">
                                <span className="text-primary">•</span>
                                <span>{rec.message}</span>
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
};
