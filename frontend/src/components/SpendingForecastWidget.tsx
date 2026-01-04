import { TrendingUp, Calendar, DollarSign } from 'lucide-react';
import { type SpendingForecast } from '../services/analyticsService';

interface SpendingForecastWidgetProps {
    forecast: SpendingForecast;
}

export const SpendingForecastWidget = ({ forecast }: SpendingForecastWidgetProps) => {
    if (!forecast) return null;

    const projectionAccuracy = forecast.daysElapsed > 0 ? (forecast.daysElapsed / (forecast.daysElapsed + forecast.daysRemaining)) * 100 : 0;

    return (
        <div className="bg-surface border border-slate-800 rounded-xl p-6">
            <h3 className="font-semibold text-white mb-6 flex items-center gap-2">
                <TrendingUp className="w-5 h-5 text-primary" />
                Month-End Forecast
            </h3>

            <div className="grid grid-cols-3 gap-4 mb-6">
                <div className="bg-slate-900/50 rounded-lg p-4 border border-slate-800">
                    <div className="flex items-center gap-2 mb-2">
                        <DollarSign className="w-4 h-4 text-blue-400" />
                        <span className="text-xs text-gray-400">Current</span>
                    </div>
                    <p className="text-xl font-bold text-white">₦{forecast.currentSpending.toLocaleString()}</p>
                </div>

                <div className="bg-slate-900/50 rounded-lg p-4 border border-slate-800">
                    <div className="flex items-center gap-2 mb-2">
                        <TrendingUp className="w-4 h-4 text-amber-400" />
                        <span className="text-xs text-gray-400">Projected</span>
                    </div>
                    <p className="text-xl font-bold text-amber-400">₦{forecast.projectedMonthEnd.toLocaleString()}</p>
                </div>

                <div className="bg-slate-900/50 rounded-lg p-4 border border-slate-800">
                    <div className="flex items-center gap-2 mb-2">
                        <Calendar className="w-4 h-4 text-emerald-400" />
                        <span className="text-xs text-gray-400">Days Left</span>
                    </div>
                    <p className="text-xl font-bold text-white">{forecast.daysRemaining}</p>
                </div>
            </div>

            <div className="space-y-3">
                <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-400">Daily average</span>
                    <span className="text-white font-medium">₦{forecast.dailyAverage.toFixed(0)}</span>
                </div>
                <div className="flex items-center justify-between text-sm">
                    <span className="text-gray-400">Projected additional</span>
                    <span className="text-white font-medium">₦{forecast.projectedAdditionalSpending.toFixed(0)}</span>
                </div>

                <div className="mt-4 pt-4 border-t border-slate-800">
                    <div className="flex items-center justify-between mb-2">
                        <span className="text-xs text-gray-500">Forecast confidence</span>
                        <span className="text-xs text-gray-400">{projectionAccuracy.toFixed(0)}%</span>
                    </div>
                    <div className="h-1.5 bg-slate-900 rounded-full overflow-hidden">
                        <div
                            className="h-full bg-gradient-to-r from-primary to-accent"
                            style={{ width: `${projectionAccuracy}%` }}
                        />
                    </div>
                    <p className="text-xs text-gray-500 mt-2">
                        Based on {forecast.daysElapsed} days of spending data
                    </p>
                </div>
            </div>
        </div>
    );
};
