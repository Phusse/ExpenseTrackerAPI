import { useEffect, useState } from 'react';
import { Plus, Search, Filter, Calendar, Trash2, Edit2, Loader2, Receipt, TrendingDown, ArrowRight, Sparkles, Target, FileText, BarChart3, Lightbulb, CheckCircle } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { ConfirmModal } from '../components/ConfirmModal';
import { expenseService } from '../services/expenseService';
import { goalService } from '../services/goalService';
import type { Expense, CreateExpenseDto, SavingGoal, CreateSavingGoalDto } from '../types';

const EmptyState = ({ onAddClick }: { onAddClick: () => void }) => (
    <div className="py-16 flex flex-col items-center justify-center">
        <div className="relative mb-6">
            <div className="w-20 h-20 rounded-full bg-gradient-to-tr from-blue-500/20 to-violet-500/20 flex items-center justify-center">
                <Receipt className="w-10 h-10 text-blue-400" />
            </div>
            <div className="absolute -top-1 -right-1 w-6 h-6 rounded-full bg-primary flex items-center justify-center">
                <Sparkles className="w-3 h-3 text-white" />
            </div>
        </div>
        <h3 className="text-xl font-semibold text-white mb-2">No Expenses Yet</h3>
        <p className="text-gray-500 text-center max-w-sm mb-6">
            Start tracking your spending to gain insights into where your money goes. It only takes a few seconds!
        </p>
        <Button onClick={onAddClick} className="gap-2">
            <Plus className="w-5 h-5" />
            Add Your First Expense
        </Button>

        {/* Tips Section */}
        <div className="mt-10 grid grid-cols-1 md:grid-cols-3 gap-4 w-full max-w-2xl">
            <div className="bg-slate-900/50 rounded-lg p-4 text-center">
                <div className="flex items-center justify-center mb-2"><FileText className="w-6 h-6 text-blue-400" /></div>
                <p className="text-sm text-gray-400">Log daily purchases</p>
            </div>
            <div className="bg-slate-900/50 rounded-lg p-4 text-center">
                <div className="flex items-center justify-center mb-2"><BarChart3 className="w-6 h-6 text-emerald-400" /></div>
                <p className="text-sm text-gray-400">Categorize expenses</p>
            </div>
            <div className="bg-slate-900/50 rounded-lg p-4 text-center">
                <div className="flex items-center justify-center mb-2"><Lightbulb className="w-6 h-6 text-amber-400" /></div>
                <p className="text-sm text-gray-400">Spot spending patterns</p>
            </div>
        </div>
    </div>
);

export const Expenses = () => {
    const [expenses, setExpenses] = useState<Expense[]>([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [savingGoals, setSavingGoals] = useState<SavingGoal[]>([]);
    const [isCreatingGoal, setIsCreatingGoal] = useState(false);
    const [deleteConfirm, setDeleteConfirm] = useState<{ isOpen: boolean; expenseId: string | null }>({
        isOpen: false,
        expenseId: null
    });
    const [newGoal, setNewGoal] = useState<CreateSavingGoalDto>({
        title: '',
        targetAmount: 0,
        deadline: new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().split('T')[0]
    });

    const [newExpense, setNewExpense] = useState<CreateExpenseDto>({
        amount: 0,
        category: 0,
        dateOfExpense: new Date().toISOString().split('T')[0],
        paymentMethod: 0,
        description: ''
    });

    const paymentMethods = [
        { id: 0, name: 'Cash' },
        { id: 1, name: 'Card' },
        { id: 2, name: 'Bank Transfer' },
        { id: 3, name: 'Mobile Payment' },
        { id: 4, name: 'POS' },
        { id: 5, name: 'Online Gateway' },
        { id: 6, name: 'Other' },
    ];

    const categories = expenseService.getCategories();

    useEffect(() => {
        fetchExpenses();
        fetchGoals();
    }, []);

    const fetchExpenses = async () => {
        try {
            setLoading(true);
            const data = await expenseService.getAll();
            setExpenses(data);
        } catch (error) {
            console.error('Failed to fetch expenses', error);
        } finally {
            setLoading(false);
        }
    };

    const fetchGoals = async () => {
        try {
            const data = await goalService.getAll();
            setSavingGoals(data);
        } catch (error) {
            console.error('Failed to fetch goals', error);
        }
    };

    const handleDelete = async (id: string) => {
        setDeleteConfirm({ isOpen: true, expenseId: id });
    };

    const confirmDelete = async () => {
        if (deleteConfirm.expenseId) {
            await expenseService.delete(deleteConfirm.expenseId);
            setExpenses(expenses.filter(e => e.id !== deleteConfirm.expenseId));
        }
    };

    const handleCreateGoal = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            const created = await goalService.create(newGoal);
            await fetchGoals();
            setNewExpense({ ...newExpense, savingGoalId: created.id });
            setIsCreatingGoal(false);
            setNewGoal({
                title: '',
                targetAmount: 0,
                deadline: new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().split('T')[0]
            });
        } catch (error) {
            console.error('Failed to create goal', error);
        }
    };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await expenseService.create(newExpense);
            setIsModalOpen(false);
            fetchExpenses();
            // Refresh goals if linked
            if (newExpense.savingGoalId) {
                fetchGoals();
            }
            setNewExpense({
                amount: 0,
                category: 0,
                dateOfExpense: new Date().toISOString().split('T')[0],
                paymentMethod: 0,
                description: ''
            });
        } catch (error) {
            console.error('Failed to create expense', error);
        }
    };

    const totalExpenses = expenses.reduce((sum, e) => sum + e.amount, 0);

    return (
        <div className="space-y-6">
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-white mb-2">Expenses</h1>
                    <p className="text-gray-400">Manage and track your daily spending.</p>
                </div>
                <div className="flex items-center gap-4">
                    {expenses.length > 0 && (
                        <div className="bg-surface border border-slate-800 rounded-lg px-4 py-2">
                            <span className="text-gray-400 text-sm">Total:</span>
                            <span className="text-white font-semibold ml-2">₦{totalExpenses.toFixed(2)}</span>
                        </div>
                    )}
                    <Button onClick={() => setIsModalOpen(true)}>
                        <Plus className="w-5 h-5 mr-2" />
                        Add Expense
                    </Button>
                </div>
            </div>

            {/* Filters Bar - Only show when there are expenses */}
            {expenses.length > 0 && (
                <div className="bg-surface border border-slate-800 rounded-xl p-4 flex flex-col md:flex-row gap-4 items-center">
                    <div className="relative flex-1 w-full">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 w-4 h-4" />
                        <input
                            type="text"
                            placeholder="Search expenses..."
                            className="w-full bg-slate-900 border border-slate-700 rounded-lg pl-10 pr-4 py-2 text-white focus:outline-none focus:border-primary"
                        />
                    </div>
                    <Button variant="outline" className="w-full md:w-auto">
                        <Filter className="w-4 h-4 mr-2" />
                        Filter
                    </Button>
                    <Button variant="outline" className="w-full md:w-auto">
                        <Calendar className="w-4 h-4 mr-2" />
                        Date Range
                    </Button>
                </div>
            )}

            {/* Expenses Table */}
            <div className="bg-surface border border-slate-800 rounded-xl overflow-hidden shadow-xl">
                {loading ? (
                    <div className="p-12 flex justify-center">
                        <Loader2 className="w-8 h-8 animate-spin text-primary" />
                    </div>
                ) : expenses.length === 0 ? (
                    <EmptyState onAddClick={() => setIsModalOpen(true)} />
                ) : (
                    <table className="w-full text-left">
                        <thead className="bg-slate-900 border-b border-slate-800">
                            <tr>
                                <th className="p-4 font-medium text-gray-400 text-sm">Description</th>
                                <th className="p-4 font-medium text-gray-400 text-sm">Category</th>
                                <th className="p-4 font-medium text-gray-400 text-sm">Payment Method</th>
                                <th className="p-4 font-medium text-gray-400 text-sm">Date</th>
                                <th className="p-4 font-medium text-gray-400 text-sm text-right">Amount</th>
                                <th className="p-4 font-medium text-gray-400 text-sm text-right">Actions</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-800">
                            {expenses.map((expense) => (
                                <tr key={expense.id} className="hover:bg-white/5 transition-colors group">
                                    <td className="p-4">
                                        <div className="flex items-center gap-3">
                                            <div className="p-2 bg-slate-900 rounded-lg text-gray-400">
                                                <TrendingDown size={16} />
                                            </div>
                                            <span className="font-medium text-white">{expense.description || 'No description'}</span>
                                        </div>
                                    </td>
                                    <td className="p-4">
                                        <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-500/10 text-blue-400 border border-blue-500/20">
                                            {expense.category || 'Unknown'}
                                        </span>
                                    </td>
                                    <td className="p-4">
                                        <span className="text-gray-300 text-sm">
                                            {expense.paymentMethod || 'Unknown'}
                                        </span>
                                    </td>

                                    <td className="p-4 text-gray-400 text-sm">
                                        {expense.dateOfExpense && new Date(expense.dateOfExpense).getFullYear() > 1
                                            ? new Date(expense.dateOfExpense).toLocaleDateString()
                                            : 'Not set'}
                                    </td>
                                    <td className="p-4 text-right font-semibold text-rose-400">
                                        -₦{expense.amount.toFixed(2)}
                                    </td>
                                    <td className="p-4 text-right">
                                        <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                                            <button className="p-1.5 hover:bg-slate-700 rounded-lg text-gray-400 hover:text-white transition-colors">
                                                <Edit2 className="w-4 h-4" />
                                            </button>
                                            <button
                                                onClick={() => handleDelete(expense.id)}
                                                className="p-1.5 hover:bg-rose-500/10 rounded-lg text-gray-400 hover:text-rose-500 transition-colors"
                                            >
                                                <Trash2 className="w-4 h-4" />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>

            {/* Add Expense Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
                    <div className="bg-surface border border-slate-700 rounded-2xl w-full max-w-lg shadow-2xl p-6 relative animate-in fade-in zoom-in-95 duration-200">
                        <h2 className="text-xl font-bold text-white mb-6">Add New Expense</h2>
                        <form onSubmit={handleCreate} className="space-y-4">
                            <Input
                                label="Description"
                                placeholder="e.g., Grocery Shopping"
                                value={newExpense.description}
                                onChange={(e) => setNewExpense({ ...newExpense, description: e.target.value })}
                            />

                            <div className="grid grid-cols-2 gap-4">
                                <Input
                                    label="Amount"
                                    type="number"
                                    placeholder="0.00"
                                    step="0.01"
                                    required
                                    value={newExpense.amount}
                                    onChange={(e) => setNewExpense({ ...newExpense, amount: parseFloat(e.target.value) })}
                                />
                                <div>
                                    <label className="block text-sm font-medium text-gray-400 mb-1.5">Payment Method</label>
                                    <select
                                        className="w-full bg-slate-800/50 border border-slate-700 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all duration-200"
                                        value={newExpense.paymentMethod}
                                        onChange={(e) => setNewExpense({ ...newExpense, paymentMethod: parseInt(e.target.value) })}
                                    >
                                        {paymentMethods.map(p => (
                                            <option key={p.id} value={p.id}>{p.name}</option>
                                        ))}
                                    </select>
                                </div>
                            </div>

                            <div className="grid grid-cols-2 gap-4">
                                <div>
                                    <label className="block text-sm font-medium text-gray-400 mb-1.5">Category</label>
                                    <select
                                        className="w-full bg-slate-800/50 border border-slate-700 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all duration-200"
                                        value={newExpense.category}
                                        onChange={(e) => setNewExpense({ ...newExpense, category: parseInt(e.target.value) })}
                                    >
                                        {categories.map(c => (
                                            <option key={c.id} value={c.id}>{c.name}</option>
                                        ))}
                                    </select>
                                </div>
                                <Input
                                    label="Date"
                                    type="date"
                                    required
                                    value={newExpense.dateOfExpense ? newExpense.dateOfExpense.split('T')[0] : ''}
                                    onChange={(e) => setNewExpense({ ...newExpense, dateOfExpense: e.target.value })}
                                />
                            </div>

                            {/* Saving Goal Selection - Only show when Savings (6) or Investments (7) category is selected */}
                            {(newExpense.category === 6 || newExpense.category === 7) && (
                                <div>
                                    <label className="block text-sm font-medium text-gray-400 mb-1.5">Link to Saving Goal (Optional)</label>
                                    <div className="flex gap-2">
                                        <select
                                            className="flex-1 bg-slate-800/50 border border-slate-700 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all duration-200"
                                            value={newExpense.savingGoalId || ''}
                                            onChange={(e) => setNewExpense({ ...newExpense, savingGoalId: e.target.value || undefined })}
                                        >
                                            <option value="">No goal (General savings)</option>
                                            {savingGoals.map(goal => (
                                                <option key={goal.id} value={goal.id}>
                                                    {goal.title} (₦{goal.currentAmount.toLocaleString()} / ₦{goal.targetAmount.toLocaleString()})
                                                </option>
                                            ))}
                                        </select>
                                        <Button
                                            type="button"
                                            variant="outline"
                                            onClick={() => setIsCreatingGoal(true)}
                                            className="whitespace-nowrap"
                                        >
                                            <Target className="w-4 h-4 mr-1" />
                                            New Goal
                                        </Button>
                                    </div>
                                    {newExpense.savingGoalId && (
                                        <p className="text-xs text-emerald-400 mt-1.5 flex items-center gap-1">
                                            <CheckCircle size={14} />
                                            This expense will contribute to your selected goal
                                        </p>
                                    )}
                                </div>
                            )}

                            <div className="flex justify-end gap-3 mt-8">
                                <Button type="button" variant="ghost" onClick={() => setIsModalOpen(false)}>Cancel</Button>
                                <Button type="submit">Create Expense</Button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Quick Create Goal Modal */}
            {isCreatingGoal && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
                    <div className="bg-surface border border-slate-700 rounded-2xl w-full max-w-md shadow-2xl p-6 animate-in fade-in zoom-in-95 duration-200">
                        <div className="flex items-center gap-3 mb-6">
                            <div className="p-3 bg-gradient-to-tr from-emerald-500 to-teal-500 rounded-xl">
                                <Target className="w-6 h-6 text-white" />
                            </div>
                            <div>
                                <h2 className="text-xl font-bold text-white">Quick Create Goal</h2>
                                <p className="text-sm text-gray-400">This will be linked to your expense</p>
                            </div>
                        </div>
                        <form onSubmit={handleCreateGoal} className="space-y-4">
                            <Input
                                label="Goal Title"
                                placeholder="e.g. New Car, Dream Vacation"
                                required
                                value={newGoal.title}
                                onChange={(e) => setNewGoal({ ...newGoal, title: e.target.value })}
                            />

                            <Input
                                label="Target Amount"
                                type="number"
                                required
                                placeholder="e.g. 5000"
                                value={newGoal.targetAmount}
                                onChange={(e) => setNewGoal({ ...newGoal, targetAmount: parseFloat(e.target.value) })}
                            />

                            <Input
                                label="Deadline (Optional)"
                                type="date"
                                value={newGoal.deadline?.split('T')[0] || ''}
                                onChange={(e) => setNewGoal({ ...newGoal, deadline: e.target.value })}
                            />

                            <div className="flex justify-end gap-3 mt-6">
                                <Button type="button" variant="ghost" onClick={() => setIsCreatingGoal(false)}>Cancel</Button>
                                <Button type="submit" className="bg-gradient-to-r from-emerald-500 to-teal-500 hover:from-emerald-600 hover:to-teal-600">
                                    Create & Link Goal
                                </Button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Delete Confirmation Modal */}
            <ConfirmModal
                isOpen={deleteConfirm.isOpen}
                onClose={() => setDeleteConfirm({ isOpen: false, expenseId: null })}
                onConfirm={confirmDelete}
                title="Delete Expense?"
                message="Are you sure you want to delete this expense? This action cannot be undone."
                confirmText="Delete"
                cancelText="Cancel"
                variant="danger"
            />
        </div>
    );
};
