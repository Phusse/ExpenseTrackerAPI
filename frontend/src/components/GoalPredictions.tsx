import { Target, CheckCircle, AlertCircle, TrendingUp } from 'lucide-react';
import { type GoalPrediction } from '../services/analyticsService';

interface GoalPredictionsProps {
    predictions: GoalPrediction[];
}

export const GoalPredictions = ({ predictions }: GoalPredictionsProps) => {
    if (!predictions || predictions.length === 0) return null;

    const getStatusColor = (status: string) => {
        if (status === 'on-track') return 'text-emerald-400';
        if (status === 'ahead') return 'text-blue-400';
        return 'text-amber-400';
    };

    const getStatusIcon = (status: string) => {
        if (status === 'on-track') return CheckCircle;
        if (status === 'ahead') return TrendingUp;
        return AlertCircle;
    };

    return (
        <div className="bg-surface border border-slate-800 rounded-xl p-6">
            <h3 className="font-semibold text-white mb-4 flex items-center gap-2">
                <Target className="w-5 h-5 text-primary" />
                Goal Predictions
            </h3>

            <div className="space-y-4">
                {predictions.map((pred, index) => {
                    const Icon = getStatusIcon(pred.status);
                    const progress = (pred.currentAmount / pred.targetAmount) * 100;

                    return (
                        <div key={index} className="bg-slate-900/50 rounded-lg p-4 border border-slate-800">
                            <div className="flex items-start justify-between mb-3">
                                <div>
                                    <h4 className="text-white font-medium">{pred.goalTitle}</h4>
                                    <p className="text-sm text-gray-400 mt-1">{pred.message}</p>
                                </div>
                                <Icon className={`w-5 h-5 ${getStatusColor(pred.status)}`} />
                            </div>

                            <div className="space-y-2">
                                <div className="flex items-center justify-between text-xs">
                                    <span className="text-gray-400">Progress</span>
                                    <span className="text-white font-medium">
                                        ₦{pred.currentAmount.toLocaleString()} / ₦{pred.targetAmount.toLocaleString()}
                                    </span>
                                </div>
                                <div className="h-2 bg-slate-800 rounded-full overflow-hidden">
                                    <div
                                        className={`h-full ${pred.status === 'on-track' ? 'bg-emerald-500' : 'bg-amber-500'}`}
                                        style={{ width: `${Math.min(progress, 100)}%` }}
                                    />
                                </div>
                                {pred.monthlyContributionNeeded > 0 && (
                                    <p className="text-xs text-gray-500 mt-2">
                                        Avg contribution: ₦{pred.monthlyContributionNeeded.toFixed(0)}/month
                                    </p>
                                )}
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
};
