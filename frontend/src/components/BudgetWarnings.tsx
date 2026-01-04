import { AlertTriangle, AlertCircle, Info } from 'lucide-react';
import { type BudgetWarning } from '../services/analyticsService';

interface BudgetWarningsProps {
    warnings: BudgetWarning[];
}

export const BudgetWarnings = ({ warnings }: BudgetWarningsProps) => {
    if (!warnings || warnings.length === 0) return null;

    const getIcon = (severity: string) => {
        if (severity === 'critical') return AlertTriangle;
        if (severity === 'warning') return AlertCircle;
        return Info;
    };

    const getStyles = (severity: string) => {
        if (severity === 'critical') {
            return {
                bg: 'bg-rose-500/10',
                border: 'border-rose-500/20',
                text: 'text-rose-400',
                icon: 'text-rose-500'
            };
        }
        if (severity === 'warning') {
            return {
                bg: 'bg-amber-500/10',
                border: 'border-amber-500/20',
                text: 'text-amber-400',
                icon: 'text-amber-500'
            };
        }
        return {
            bg: 'bg-blue-500/10',
            border: 'border-blue-500/20',
            text: 'text-blue-400',
            icon: 'text-blue-500'
        };
    };

    return (
        <div className="space-y-3">
            {warnings.map((warning, index) => {
                const Icon = getIcon(warning.severity);
                const styles = getStyles(warning.severity);

                return (
                    <div
                        key={index}
                        className={`${styles.bg} border ${styles.border} rounded-lg p-4 flex items-start gap-3`}
                    >
                        <Icon className={`w-5 h-5 ${styles.icon} flex-shrink-0 mt-0.5`} />
                        <div className="flex-1">
                            <p className={`text-sm font-medium ${styles.text} mb-1`}>
                                {warning.message}
                            </p>
                            <div className="flex items-center gap-4 text-xs text-gray-400">
                                <span>Current: ₦{warning.currentSpending.toLocaleString()}</span>
                                <span>Projected: ₦{warning.projectedTotal.toLocaleString()}</span>
                                <span>Limit: ₦{warning.budgetLimit.toLocaleString()}</span>
                            </div>
                        </div>
                    </div>
                );
            })}
        </div>
    );
};
