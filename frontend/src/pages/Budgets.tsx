import { useEffect, useState } from 'react';
import { Plus, Trash2, Wallet, Loader2, Sparkles } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { ConfirmModal } from '../components/ConfirmModal';
import { budgetService } from '../services/budgetService';
import { expenseService } from '../services/expenseService';
import { useToast } from '../context/ToastContext';
import { useSettings } from '../context/SettingsContext';
import type { Budget, CreateBudgetDto } from '../types';

const EmptyState = ({ onAddClick }: { onAddClick: () => void }) => (
    <div className="py-12 flex flex-col items-center justify-center text-center px-4">
        <div className="relative mb-6">
            <div className="w-20 h-20 rounded-full bg-gradient-to-tr from-violet-500/20 to-pink-500/20 flex items-center justify-center ring-1 ring-white/10">
                <Wallet className="w-10 h-10 text-violet-400" />
            </div>
            <div className="absolute -top-1 -right-1 w-7 h-7 rounded-full bg-violet-500 flex items-center justify-center">
                <Sparkles className="w-3.5 h-3.5 text-white" />
            </div>
        </div>
        <h3 className="text-xl font-bold text-white mb-2">No Budgets Set</h3>
        <p className="text-gray-400 text-sm max-w-xs mb-6">
            Create budgets to control your spending and reach your financial goals.
        </p>
        <Button onClick={onAddClick} className="bg-gradient-to-r from-violet-600 to-indigo-600">
            <Plus className="w-5 h-5 mr-2" />
            Create First Budget
        </Button>
    </div>
);

export const Budgets = () => {
    const toast = useToast();
    const { formatCurrency } = useSettings();
    const [budgets, setBudgets] = useState<Budget[]>([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [deleteConfirm, setDeleteConfirm] = useState<{ isOpen: boolean; budgetId: string | null }>({
        isOpen: false,
        budgetId: null
    });

    const [newBudget, setNewBudget] = useState<CreateBudgetDto>({
        limit: 0,
        category: 0,
        period: new Date().toISOString().split('T')[0]
    });

    const categories = expenseService.getCategories();

    useEffect(() => {
        fetchBudgets();
    }, []);

    const fetchBudgets = async () => {
        try {
            setLoading(true);
            const data = await budgetService.getAll();
            setBudgets(data);
        } catch (error) {
            console.error('Failed to fetch budgets', error);
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await budgetService.create(newBudget);
            setIsModalOpen(false);
            fetchBudgets();
            const categoryName = categories.find(c => c.id === newBudget.category)?.name || 'Category';
            toast.success('Budget Created', `${formatCurrency(newBudget.limit)} budget set for ${categoryName}.`);
            setNewBudget({ limit: 0, category: 0, period: new Date().toISOString().split('T')[0] });
        } catch (error) {
            // Error handled
        }
    };

    const handleDelete = async (id: string) => {
        setDeleteConfirm({ isOpen: true, budgetId: id });
    };

    const confirmDelete = async () => {
        if (deleteConfirm.budgetId) {
            try {
                await budgetService.delete(deleteConfirm.budgetId);
                setBudgets(budgets.filter(b => b.id !== deleteConfirm.budgetId));
                toast.success('Budget Deleted', 'The budget has been removed.');
            } catch (error) {
                // Error handled
            }
        }
    };

    const totalBudget = budgets.reduce((sum, b) => sum + b.limit, 0);

    return (
        <div className="space-y-4 md:space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-xl md:text-2xl font-bold text-white">Budgets</h1>
                    <p className="text-sm text-gray-400 hidden md:block">Control your spending</p>
                </div>
                <Button onClick={() => setIsModalOpen(true)} className="!w-auto bg-gradient-to-r from-violet-600 to-indigo-600">
                    <Plus className="w-5 h-5 md:mr-2" />
                    <span className="hidden md:inline">Create Budget</span>
                </Button>
            </div>

            {/* Total Summary */}
            {budgets.length > 0 && (
                <div className="glass-card p-4 flex items-center justify-between">
                    <div>
                        <p className="text-xs text-gray-400">Total Budget</p>
                        <p className="text-lg font-bold text-white">{formatCurrency(totalBudget)}</p>
                    </div>
                    <div className="w-10 h-10 rounded-xl bg-violet-500/10 flex items-center justify-center">
                        <Wallet className="w-5 h-5 text-violet-400" />
                    </div>
                </div>
            )}

            {/* Budget List */}
            {loading ? (
                <div className="flex justify-center py-12">
                    <Loader2 className="w-8 h-8 animate-spin text-primary" />
                </div>
            ) : budgets.length === 0 ? (
                <EmptyState onAddClick={() => setIsModalOpen(true)} />
            ) : (
                <div className="space-y-3">
                    {budgets.map((budget) => {
                        const spent = budget.spent || 0;
                        const percentage = Math.min((spent / budget.limit) * 100, 100);
                        const isOver = spent > budget.limit;
                        const isWarning = percentage >= 80 && !isOver;
                        const remaining = budget.limit - spent;

                        return (
                            <div
                                key={budget.id}
                                className={`glass-card p-4 ${isOver ? 'border-rose-500/30' : ''}`}
                            >
                                <div className="flex items-start justify-between mb-3">
                                    <div className="flex items-center gap-3">
                                        <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${isOver ? 'bg-rose-500/10 text-rose-400' : isWarning ? 'bg-amber-500/10 text-amber-400' : 'bg-violet-500/10 text-violet-400'}`}>
                                            <Wallet className="w-5 h-5" />
                                        </div>
                                        <div>
                                            <h3 className="font-bold text-white">{budget.category}</h3>
                                            <p className="text-xs text-gray-500">Monthly Budget</p>
                                        </div>
                                    </div>
                                    <button
                                        onClick={() => handleDelete(budget.id)}
                                        className="p-2 text-gray-500 hover:text-rose-500 active:scale-95"
                                    >
                                        <Trash2 className="w-4 h-4" />
                                    </button>
                                </div>

                                {isOver && (
                                    <div className="mb-3 px-3 py-2 bg-rose-500/10 border border-rose-500/20 rounded-lg">
                                        <p className="text-rose-400 text-xs font-bold">
                                            Over by {formatCurrency(Math.abs(remaining))}
                                        </p>
                                    </div>
                                )}

                                <div className="flex justify-between items-end mb-2">
                                    <span className={`text-xl font-bold ${isOver ? 'text-rose-400' : 'text-white'}`}>
                                        {formatCurrency(spent)}
                                    </span>
                                    <span className="text-gray-400 text-sm">
                                        of {formatCurrency(budget.limit)}
                                    </span>
                                </div>

                                <div className="h-2 bg-slate-800 rounded-full overflow-hidden mb-2">
                                    <div
                                        className={`h-full transition-all ${isOver ? 'bg-rose-500' : isWarning ? 'bg-amber-500' : 'bg-gradient-to-r from-violet-500 to-fuchsia-500'}`}
                                        style={{ width: `${percentage}%` }}
                                    />
                                </div>

                                <div className="flex justify-between text-xs">
                                    <span className={isOver ? 'text-rose-400' : isWarning ? 'text-amber-400' : 'text-violet-400'}>
                                        {percentage.toFixed(0)}% used
                                    </span>
                                    <span className={remaining < 0 ? 'text-rose-400' : 'text-gray-400'}>
                                        {remaining >= 0 ? `${formatCurrency(remaining)} left` : 'Exceeded'}
                                    </span>
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}

            {/* Create Budget Modal */}
            {isModalOpen && (
                <>
                    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={() => setIsModalOpen(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <h2 className="text-xl font-bold text-white mb-6">Create Budget</h2>
                                <form onSubmit={handleCreate} className="space-y-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-300 mb-2">Category</label>
                                        <select
                                            className="input-glass"
                                            value={newBudget.category}
                                            onChange={(e) => setNewBudget({ ...newBudget, category: parseInt(e.target.value) })}
                                        >
                                            {categories.map(c => (
                                                <option key={c.id} value={c.id}>{c.name}</option>
                                            ))}
                                        </select>
                                    </div>
                                    <Input
                                        label="Budget Limit"
                                        type="number"
                                        required
                                        placeholder="e.g. 50000"
                                        value={newBudget.limit || ''}
                                        onChange={(e) => setNewBudget({ ...newBudget, limit: parseFloat(e.target.value) || 0 })}
                                    />
                                    <Input
                                        label="Budget Period"
                                        type="month"
                                        required
                                        value={newBudget.period.slice(0, 7)}
                                        onChange={(e) => setNewBudget({ ...newBudget, period: e.target.value + '-01' })}
                                    />
                                    <div className="flex flex-col-reverse gap-3 pt-4 md:flex-row">
                                        <Button type="button" variant="ghost" onClick={() => setIsModalOpen(false)}>
                                            Cancel
                                        </Button>
                                        <Button type="submit" className="bg-gradient-to-r from-violet-600 to-indigo-600">
                                            Create Budget
                                        </Button>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                </>
            )}

            {/* Delete Confirmation */}
            <ConfirmModal
                isOpen={deleteConfirm.isOpen}
                onClose={() => setDeleteConfirm({ isOpen: false, budgetId: null })}
                onConfirm={confirmDelete}
                title="Delete Budget?"
                message="This action cannot be undone."
                confirmText="Delete"
                cancelText="Cancel"
                variant="danger"
            />

            {/* FAB */}
            <button onClick={() => setIsModalOpen(true)} className="fab md:hidden bg-gradient-to-br from-violet-600 to-indigo-600">
                <Plus className="w-6 h-6 text-white" />
            </button>
        </div>
    );
};
