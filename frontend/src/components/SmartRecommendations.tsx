import { Lightbulb, TrendingUp, Target, DollarSign } from 'lucide-react';
import { type Recommendation } from '../services/analyticsService';

interface SmartRecommendationsProps {
    recommendations: Recommendation[];
}

export const SmartRecommendations = ({ recommendations }: SmartRecommendationsProps) => {
    if (!recommendations || recommendations.length === 0) return null;

    const getIcon = (type: string) => {
        if (type === 'budget') return DollarSign;
        if (type === 'goal') return Target;
        if (type === 'savings') return TrendingUp;
        return Lightbulb;
    };

    const getPriorityStyles = (priority: string) => {
        if (priority === 'high') {
            return {
                bg: 'bg-rose-500/10',
                border: 'border-rose-500/20',
                text: 'text-rose-400'
            };
        }
        if (priority === 'medium') {
            return {
                bg: 'bg-amber-500/10',
                border: 'border-amber-500/20',
                text: 'text-amber-400'
            };
        }
        return {
            bg: 'bg-blue-500/10',
            border: 'border-blue-500/20',
            text: 'text-blue-400'
        };
    };

    return (
        <div className="bg-surface border border-slate-800 rounded-xl p-6">
            <h3 className="font-semibold text-white mb-4 flex items-center gap-2">
                <Lightbulb className="w-5 h-5 text-yellow-500" />
                Smart Recommendations
            </h3>

            <div className="space-y-3">
                {recommendations.map((rec, index) => {
                    const Icon = getIcon(rec.type);
                    const styles = getPriorityStyles(rec.priority);

                    return (
                        <div
                            key={index}
                            className={`${styles.bg} border ${styles.border} rounded-lg p-4 flex items-start gap-3`}
                        >
                            <div className={`p-2 rounded-lg ${styles.bg}`}>
                                <Icon className={`w-4 h-4 ${styles.text}`} />
                            </div>
                            <div className="flex-1">
                                <p className={`text-sm font-medium ${styles.text}`}>
                                    {rec.message}
                                </p>
                                {rec.suggestedAmount && (
                                    <p className="text-xs text-gray-500 mt-1">
                                        Suggested: ₦{rec.suggestedAmount.toLocaleString()}
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
