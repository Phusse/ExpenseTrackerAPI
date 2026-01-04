import { useEffect, useState } from 'react';
import { Plus, Trash2, Wallet, Target, TrendingUp, Loader2, Sparkles, PieChart } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { ConfirmModal } from '../components/ConfirmModal';
import { budgetService } from '../services/budgetService';
import { expenseService } from '../services/expenseService';
import type { Budget, CreateBudgetDto } from '../types';

const EmptyState = ({ onAddClick }: { onAddClick: () => void }) => (
    <div className="col-span-full py-16 flex flex-col items-center justify-center">
        <div className="relative mb-6">
            <div className="w-20 h-20 rounded-full bg-gradient-to-tr from-violet-500/20 to-pink-500/20 flex items-center justify-center">
                <Wallet className="w-10 h-10 text-violet-400" />
            </div>
            <div className="absolute -top-1 -right-1 w-6 h-6 rounded-full bg-violet-500 flex items-center justify-center">
                <Sparkles className="w-3 h-3 text-white" />
            </div>
        </div>
        <h3 className="text-xl font-semibold text-white mb-2">No Budgets Set</h3>
        <p className="text-gray-500 text-center max-w-sm mb-6">
            Create budgets to set spending limits and stay on track with your financial goals.
        </p>
        <Button onClick={onAddClick} className="gap-2">
            <Plus className="w-5 h-5" />
            Create Your First Budget
        </Button>

        {/* Benefits Section */}
        <div className="mt-10 grid grid-cols-1 md:grid-cols-3 gap-4 w-full max-w-2xl">
            <div className="bg-slate-900/50 rounded-lg p-4 text-center border border-slate-800">
                <div className="w-10 h-10 rounded-full bg-blue-500/10 text-blue-400 flex items-center justify-center mx-auto mb-3">
                    <Target size={20} />
                </div>
                <h4 className="text-white font-medium text-sm mb-1">Set Limits</h4>
                <p className="text-xs text-gray-500">Control spending by category</p>
            </div>
            <div className="bg-slate-900/50 rounded-lg p-4 text-center border border-slate-800">
                <div className="w-10 h-10 rounded-full bg-emerald-500/10 text-emerald-400 flex items-center justify-center mx-auto mb-3">
                    <TrendingUp size={20} />
                </div>
                <h4 className="text-white font-medium text-sm mb-1">Track Progress</h4>
                <p className="text-xs text-gray-500">See how you're doing</p>
            </div>
            <div className="bg-slate-900/50 rounded-lg p-4 text-center border border-slate-800">
                <div className="w-10 h-10 rounded-full bg-violet-500/10 text-violet-400 flex items-center justify-center mx-auto mb-3">
                    <PieChart size={20} />
                </div>
                <h4 className="text-white font-medium text-sm mb-1">Stay Aware</h4>
                <p className="text-xs text-gray-500">Get spending insights</p>
            </div>
        </div>
    </div>
);

export const Budgets = () => {
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
        period: new Date().toISOString().split('T')[0] // Backend expects period as DateOnly
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
        } catch (error) {
            console.error('Failed to create budget', error);
        }
    };

    const handleDelete = async (id: string) => {
        setDeleteConfirm({ isOpen: true, budgetId: id });
    };

    const confirmDelete = async () => {
        if (deleteConfirm.budgetId) {
            await budgetService.delete(deleteConfirm.budgetId);
            setBudgets(budgets.filter(b => b.id !== deleteConfirm.budgetId));
        }
    };

    const totalBudget = budgets.reduce((sum, b) => sum + b.limit, 0);

    return (
        <div className="space-y-6">
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-white mb-2">Budgets</h1>
                    <p className="text-gray-400">Set limits and track your spending goals.</p>
                </div>
                <div className="flex items-center gap-4">
                    {budgets.length > 0 && (
                        <div className="bg-surface border border-slate-800 rounded-lg px-4 py-2">
                            <span className="text-gray-400 text-sm">Total Budget:</span>
                            <span className="text-white font-semibold ml-2">₦{totalBudget.toFixed(2)}</span>
                        </div>
                    )}
                    <Button onClick={() => setIsModalOpen(true)}>
                        <Plus className="w-5 h-5 mr-2" />
                        Create Budget
                    </Button>
                </div>
            </div>

            {loading ? (
                <div className="flex justify-center py-12">
                    <Loader2 className="w-8 h-8 animate-spin text-primary" />
                </div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {budgets.map((budget) => {
                        const spent = budget.spent || 0;
                        const percentage = Math.min(100, (spent / budget.limit) * 100);
                        const categoryName = budget.category || 'General';
                        const isOverBudget = percentage >= 100;
                        const isWarning = percentage >= 80 && percentage < 100;

                        return (
                            <div key={budget.id} className="bg-surface border border-slate-800 rounded-xl p-6 hover:border-slate-700 transition-all group">
                                <div className="flex justify-between items-start mb-4">
                                    <div className="flex items-center gap-3">
                                        <div className={`p-3 rounded-lg ${isOverBudget ? 'bg-rose-500/10 text-rose-500' : isWarning ? 'bg-amber-500/10 text-amber-500' : 'bg-blue-500/10 text-blue-500'}`}>
                                            <Wallet size={24} />
                                        </div>
                                        <div>
                                            <h3 className="font-semibold text-white">{categoryName}</h3>
                                            <p className="text-sm text-gray-400">Monthly Budget</p>
                                        </div>
                                    </div>
                                    <button onClick={() => handleDelete(budget.id)} className="text-gray-500 hover:text-rose-500 transition-colors opacity-0 group-hover:opacity-100">
                                        <Trash2 size={18} />
                                    </button>
                                </div>

                                <div className="mb-2 flex justify-between text-sm font-medium">
                                    <span className={isOverBudget ? 'text-rose-400' : 'text-gray-300'}>₦{spent.toFixed(2)} spent</span>
                                    <span className="text-gray-400">of ₦{budget.limit.toFixed(2)}</span>
                                </div>

                                <div className="h-2.5 bg-slate-900 rounded-full overflow-hidden">
                                    <div
                                        className={`h-full rounded-full transition-all ${isOverBudget ? 'bg-rose-500' : isWarning ? 'bg-amber-500' : 'bg-primary'}`}
                                        style={{ width: `${Math.min(percentage, 100)}%` }}
                                    />
                                </div>

                                <div className="mt-2 flex justify-between text-xs">
                                    <span className={`font-medium ${isOverBudget ? 'text-rose-400' : isWarning ? 'text-amber-400' : 'text-gray-500'}`}>
                                        {percentage.toFixed(0)}% used
                                    </span>
                                    <span className="text-gray-500">
                                        ₦{Math.max(0, budget.limit - spent).toFixed(2)} remaining
                                    </span>
                                </div>
                            </div>
                        );
                    })}

                    {budgets.length === 0 && <EmptyState onAddClick={() => setIsModalOpen(true)} />}
                </div>
            )}

            {/* Create Budget Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
                    <div className="bg-surface border border-slate-700 rounded-2xl w-full max-w-md shadow-2xl p-6 animate-in fade-in zoom-in-95 duration-200">
                        <h2 className="text-xl font-bold text-white mb-6">Create New Budget</h2>
                        <form onSubmit={handleCreate} className="space-y-4">
                            <div>
                                <label className="block text-sm font-medium text-gray-400 mb-1.5">Category</label>
                                <select
                                    className="w-full bg-slate-800/50 border border-slate-700 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all duration-200"
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
                                value={newBudget.limit}
                                onChange={(e) => setNewBudget({ ...newBudget, limit: parseFloat(e.target.value) })}
                            />

                            <Input
                                label="Budget Period (Month)"
                                type="month"
                                required
                                value={newBudget.period.slice(0, 7)}
                                onChange={(e) => setNewBudget({ ...newBudget, period: e.target.value + '-01' })}
                            />

                            <div className="flex justify-end gap-3 mt-6">
                                <Button type="button" variant="ghost" onClick={() => setIsModalOpen(false)}>Cancel</Button>
                                <Button type="submit">Create Budget</Button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Delete Confirmation Modal */}
            <ConfirmModal
                isOpen={deleteConfirm.isOpen}
                onClose={() => setDeleteConfirm({ isOpen: false, budgetId: null })}
                onConfirm={confirmDelete}
                title="Delete Budget?"
                message="Are you sure you want to delete this budget? This action cannot be undone."
                confirmText="Delete"
                cancelText="Cancel"
                variant="danger"
            />
        </div>
    );
};
