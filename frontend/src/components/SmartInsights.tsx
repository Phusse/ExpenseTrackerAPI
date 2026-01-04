import { AlertCircle, TrendingDown, TrendingUp, Target, Sparkles } from 'lucide-react';

interface Insight {
    type: 'success' | 'warning' | 'info' | 'achievement';
    message: string;
    icon?: any;
}

interface SmartInsightsProps {
    insights: Insight[];
}

export const SmartInsights = ({ insights }: SmartInsightsProps) => {
    if (!insights || insights.length === 0) {
        return null;
    }

    const getInsightStyle = (type: string) => {
        switch (type) {
            case 'success':
                return {
                    bg: 'bg-emerald-500/10',
                    border: 'border-emerald-500/20',
                    text: 'text-emerald-400',
                    icon: TrendingDown
                };
            case 'warning':
                return {
                    bg: 'bg-amber-500/10',
                    border: 'border-amber-500/20',
                    text: 'text-amber-400',
                    icon: AlertCircle
                };
            case 'achievement':
                return {
                    bg: 'bg-purple-500/10',
                    border: 'border-purple-500/20',
                    text: 'text-purple-400',
                    icon: Sparkles
                };
            default:
                return {
                    bg: 'bg-blue-500/10',
                    border: 'border-blue-500/20',
                    text: 'text-blue-400',
                    icon: Target
                };
        }
    };

    // Duplicate insights for seamless loop
    const duplicatedInsights = [...insights, ...insights];

    return (
        <div className="relative overflow-hidden py-1">
            <div className="flex gap-4 animate-marquee hover:pause-marquee">
                {duplicatedInsights.map((insight, index) => {
                    const style = getInsightStyle(insight.type);
                    const Icon = style.icon;

                    return (
                        <div
                            key={index}
                            className={`${style.bg} border ${style.border} rounded-xl p-4 flex items-center gap-3 flex-shrink-0 min-w-[350px]`}
                        >
                            <div className={`p-2 rounded-lg ${style.bg}`}>
                                <Icon className={`w-5 h-5 ${style.text}`} />
                            </div>
                            <p className={`text-sm font-medium ${style.text} whitespace-nowrap`}>
                                {insight.message}
                            </p>
                        </div>
                    );
                })}
            </div>

            {/* Add custom styles */}
            <style>{`
                @keyframes marquee {
                    0% {
                        transform: translateX(0);
                    }
                    100% {
                        transform: translateX(-50%);
                    }
                }
                
                .animate-marquee {
                    animation: marquee 30s linear infinite;
                }
                
                .pause-marquee {
                    animation-play-state: paused;
                }
            `}</style>
        </div>
    );
};
