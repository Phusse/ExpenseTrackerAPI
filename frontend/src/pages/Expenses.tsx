import { useEffect, useState } from 'react';
import { Plus, Search, Filter, Calendar, Trash2, Loader2, Receipt, TrendingDown, Sparkles, Target, Clock, ChevronDown } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { ConfirmModal } from '../components/ConfirmModal';
import { expenseService } from '../services/expenseService';
import { goalService } from '../services/goalService';
import { useToast } from '../context/ToastContext';
import { useSettings } from '../context/SettingsContext';
import type { Expense, CreateExpenseDto, SavingGoal, CreateSavingGoalDto } from '../types';

const EmptyState = ({ onAddClick }: { onAddClick: () => void }) => (
    <div className="py-12 flex flex-col items-center justify-center text-center px-4">
        <div className="relative mb-6">
            <div className="w-20 h-20 rounded-full bg-gradient-to-tr from-blue-500/20 to-violet-500/20 flex items-center justify-center ring-1 ring-white/10">
                <Receipt className="w-10 h-10 text-blue-400" />
            </div>
            <div className="absolute -top-1 -right-1 w-7 h-7 rounded-full bg-primary flex items-center justify-center">
                <Sparkles className="w-3.5 h-3.5 text-white" />
            </div>
        </div>
        <h3 className="text-xl font-bold text-white mb-2">No Expenses Yet</h3>
        <p className="text-gray-400 text-sm max-w-xs mb-6">
            Start tracking your spending to understand where your money goes.
        </p>
        <Button onClick={onAddClick}>
            <Plus className="w-5 h-5 mr-2" />
            Add First Expense
        </Button>
    </div>
);

export const Expenses = () => {
    const toast = useToast();
    const { formatCurrency } = useSettings();
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
        { id: 3, name: 'Mobile' },
        { id: 4, name: 'POS' },
        { id: 5, name: 'Online' },
        { id: 6, name: 'Other' },
    ];

    const categories = expenseService.getCategories();

    const [searchTerm, setSearchTerm] = useState('');
    const [categoryFilter, setCategoryFilter] = useState<number | 'all'>('all');
    const [dateFilter, setDateFilter] = useState<'all' | 'week' | 'month' | 'year'>('all');
    const [showFilters, setShowFilters] = useState(false);

    const getCategoryName = (val: string | number) => {
        if (val === undefined || val === null) return 'Unknown';
        if (typeof val === 'number' || !isNaN(Number(val))) {
            const match = categories.find(c => c.id === Number(val));
            if (match) return match.name;
        }
        return String(val);
    };

    const getPaymentMethodName = (val: string | number) => {
        if (val === undefined || val === null) return 'Unknown';
        if (typeof val === 'number' || !isNaN(Number(val))) {
            const match = paymentMethods.find(p => p.id === Number(val));
            if (match) return match.name;
        }
        return String(val);
    };

    const filteredExpenses = expenses.filter(expense => {
        const matchesSearch =
            expense.description?.toLowerCase().includes(searchTerm.toLowerCase()) ||
            getCategoryName(expense.category).toLowerCase().includes(searchTerm.toLowerCase()) ||
            expense.amount.toString().includes(searchTerm);

        if (!matchesSearch) return false;

        if (categoryFilter !== 'all') {
            const filterName = categories.find(c => c.id === categoryFilter)?.name;
            const expName = getCategoryName(expense.category);
            if (expName !== filterName) return false;
        }

        if (dateFilter !== 'all') {
            const date = new Date(expense.dateOfExpense);
            const now = new Date();
            const diffTime = Math.abs(now.getTime() - date.getTime());
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

            if (dateFilter === 'week' && diffDays > 7) return false;
            if (dateFilter === 'month' && diffDays > 30) return false;
            if (dateFilter === 'year' && diffDays > 365) return false;
        }

        return true;
    });

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
            try {
                await expenseService.delete(deleteConfirm.expenseId);
                setExpenses(expenses.filter(e => e.id !== deleteConfirm.expenseId));
                toast.success('Expense Deleted', 'The expense has been removed.');
            } catch (error) {
                // Error handled by API
            }
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
            toast.success('Goal Created', `"${created.title}" has been created.`);
        } catch (error) {
            // Error handled
        }
    };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await expenseService.create(newExpense);
            setIsModalOpen(false);
            fetchExpenses();
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
            toast.success('Expense Added', `${formatCurrency(newExpense.amount)} expense recorded.`);
        } catch (error) {
            // Error handled
        }
    };

    const totalAmount = expenses.reduce((sum, expense) => sum + expense.amount, 0);
    const currentMonth = new Date().getMonth();
    const currentYear = new Date().getFullYear();
    const todayDate = new Date().toISOString().split('T')[0];

    const totalMonth = expenses
        .filter(e => {
            const d = new Date(e.dateOfExpense);
            return d.getMonth() === currentMonth && d.getFullYear() === currentYear;
        })
        .reduce((sum, e) => sum + e.amount, 0);

    const totalToday = expenses
        .filter(e => {
            const d = new Date(e.dateOfExpense);
            const expenseDate = d.toISOString().split('T')[0];
            return expenseDate === todayDate;
        })
        .reduce((sum, e) => sum + e.amount, 0);

    return (
        <div className="space-y-4 md:space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-xl md:text-2xl font-bold text-white">Expenses</h1>
                    <p className="text-sm text-gray-400 hidden md:block">Manage your spending</p>
                </div>
                <Button onClick={() => setIsModalOpen(true)} className="!w-auto">
                    <Plus className="w-5 h-5 md:mr-2" />
                    <span className="hidden md:inline">Add Expense</span>
                </Button>
            </div>

            {/* Summary Cards - Horizontal scroll on mobile */}
            <div className="flex gap-3 overflow-x-auto pb-2 -mx-4 px-4 md:grid md:grid-cols-3 md:mx-0 md:px-0 md:overflow-visible">
                <div className="glass-card p-4 min-w-[140px] md:min-w-0 flex-shrink-0 md:flex-shrink">
                    <div className="flex items-center gap-2 mb-2">
                        <div className="w-8 h-8 rounded-lg bg-rose-500/10 flex items-center justify-center">
                            <TrendingDown className="w-4 h-4 text-rose-400" />
                        </div>
                        <span className="text-xs text-gray-400">Total</span>
                    </div>
                    <p className="text-lg font-bold text-white">{formatCurrency(totalAmount)}</p>
                </div>
                <div className="glass-card p-4 min-w-[140px] md:min-w-0 flex-shrink-0 md:flex-shrink">
                    <div className="flex items-center gap-2 mb-2">
                        <div className="w-8 h-8 rounded-lg bg-blue-500/10 flex items-center justify-center">
                            <Calendar className="w-4 h-4 text-blue-400" />
                        </div>
                        <span className="text-xs text-gray-400">This Month</span>
                    </div>
                    <p className="text-lg font-bold text-white">{formatCurrency(totalMonth)}</p>
                </div>
                <div className="glass-card p-4 min-w-[140px] md:min-w-0 flex-shrink-0 md:flex-shrink">
                    <div className="flex items-center gap-2 mb-2">
                        <div className="w-8 h-8 rounded-lg bg-emerald-500/10 flex items-center justify-center">
                            <Clock className="w-4 h-4 text-emerald-400" />
                        </div>
                        <span className="text-xs text-gray-400">Today</span>
                    </div>
                    <p className="text-lg font-bold text-white">{formatCurrency(totalToday)}</p>
                </div>
            </div>

            {/* Search and Filters */}
            {expenses.length > 0 && (
                <div className="space-y-3">
                    <div className="relative">
                        <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 w-5 h-5" />
                        <input
                            type="text"
                            placeholder="Search expenses..."
                            className="input-glass pl-12"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                    <button
                        onClick={() => setShowFilters(!showFilters)}
                        className="flex items-center gap-2 text-sm text-gray-400"
                    >
                        <Filter className="w-4 h-4" />
                        Filters
                        <ChevronDown className={`w-4 h-4 transition-transform ${showFilters ? 'rotate-180' : ''}`} />
                    </button>
                    {showFilters && (
                        <div className="flex gap-2 overflow-x-auto pb-2">
                            <select
                                className="input-glass !w-auto text-sm"
                                value={categoryFilter === 'all' ? '' : categoryFilter}
                                onChange={(e) => setCategoryFilter(e.target.value ? parseInt(e.target.value) : 'all')}
                            >
                                <option value="">All Categories</option>
                                {categories.map(c => (
                                    <option key={c.id} value={c.id}>{c.name}</option>
                                ))}
                            </select>
                            <select
                                className="input-glass !w-auto text-sm"
                                value={dateFilter}
                                onChange={(e) => setDateFilter(e.target.value as any)}
                            >
                                <option value="all">All Time</option>
                                <option value="week">Last 7 Days</option>
                                <option value="month">Last 30 Days</option>
                                <option value="year">Last Year</option>
                            </select>
                        </div>
                    )}
                </div>
            )}

            {/* Expense List */}
            {loading ? (
                <div className="flex justify-center py-12">
                    <Loader2 className="w-8 h-8 animate-spin text-primary" />
                </div>
            ) : expenses.length === 0 ? (
                <EmptyState onAddClick={() => setIsModalOpen(true)} />
            ) : filteredExpenses.length === 0 ? (
                <div className="text-center py-12 text-gray-400">
                    No expenses match your search.
                </div>
            ) : (
                <div className="space-y-2">
                    {filteredExpenses.map((expense) => (
                        <div
                            key={expense.id}
                            className="glass-card p-4 active:scale-[0.99] transition-transform"
                        >
                            {/* Top row: Icon, Description, Amount */}
                            <div className="flex items-start gap-3">
                                <div className="w-10 h-10 rounded-xl bg-slate-800 flex items-center justify-center flex-shrink-0">
                                    <TrendingDown className="w-5 h-5 text-rose-400" />
                                </div>
                                <div className="flex-1 min-w-0">
                                    <div className="flex items-start justify-between gap-2">
                                        <p className="text-sm font-medium text-white truncate flex-1">
                                            {expense.description || 'Expense'}
                                        </p>
                                        <p className="text-rose-400 font-bold text-sm whitespace-nowrap">
                                            -{formatCurrency(expense.amount)}
                                        </p>
                                    </div>
                                    {/* Bottom row: Category, Date, Payment, Delete */}
                                    <div className="flex items-center justify-between mt-2">
                                        <div className="flex items-center gap-2 flex-wrap">
                                            <span className="text-xs px-2 py-0.5 rounded bg-blue-500/10 text-blue-400">
                                                {getCategoryName(expense.category)}
                                            </span>
                                            <span className="text-xs text-gray-500">
                                                {expense.dateOfExpense
                                                    ? new Date(expense.dateOfExpense).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })
                                                    : 'No date'}
                                            </span>
                                            <span className="text-xs text-gray-600">
                                                {getPaymentMethodName(expense.paymentMethod)}
                                            </span>
                                        </div>
                                        <button
                                            onClick={() => handleDelete(expense.id)}
                                            className="p-1.5 text-gray-500 hover:text-rose-500 active:scale-95 flex-shrink-0"
                                        >
                                            <Trash2 className="w-4 h-4" />
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

            )}

            {/* Add Expense Modal - Bottom sheet on mobile */}
            {isModalOpen && (
                <>
                    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={() => setIsModalOpen(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-lg max-h-[90vh] overflow-auto animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <h2 className="text-xl font-bold text-white mb-6">Add Expense</h2>
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
                                            value={newExpense.amount || ''}
                                            onChange={(e) => setNewExpense({ ...newExpense, amount: parseFloat(e.target.value) || 0 })}
                                        />
                                        <div>
                                            <label className="block text-sm font-medium text-gray-300 mb-2">Payment</label>
                                            <select
                                                className="input-glass"
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
                                            <label className="block text-sm font-medium text-gray-300 mb-2">Category</label>
                                            <select
                                                className="input-glass"
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
                                            value={newExpense.dateOfExpense?.split('T')[0] || ''}
                                            onChange={(e) => setNewExpense({ ...newExpense, dateOfExpense: e.target.value })}
                                        />
                                    </div>

                                    {(newExpense.category === 6 || newExpense.category === 7) && (
                                        <div>
                                            <label className="block text-sm font-medium text-gray-400 mb-2">Link to Goal</label>
                                            <div className="flex gap-2">
                                                <select
                                                    className="input-glass flex-1"
                                                    value={newExpense.savingGoalId || ''}
                                                    onChange={(e) => setNewExpense({ ...newExpense, savingGoalId: e.target.value || undefined })}
                                                >
                                                    <option value="">No goal</option>
                                                    {savingGoals.map(goal => (
                                                        <option key={goal.id} value={goal.id}>{goal.title}</option>
                                                    ))}
                                                </select>
                                                <Button
                                                    type="button"
                                                    variant="outline"
                                                    onClick={() => setIsCreatingGoal(true)}
                                                    className="!w-auto"
                                                >
                                                    <Target className="w-4 h-4" />
                                                </Button>
                                            </div>
                                        </div>
                                    )}

                                    <div className="flex flex-col-reverse gap-3 pt-4 md:flex-row">
                                        <Button type="button" variant="ghost" onClick={() => setIsModalOpen(false)}>
                                            Cancel
                                        </Button>
                                        <Button type="submit">
                                            Add Expense
                                        </Button>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                </>
            )}

            {/* Quick Create Goal Modal */}
            {isCreatingGoal && (
                <>
                    <div className="fixed inset-0 z-[60] bg-black/60 backdrop-blur-sm" onClick={() => setIsCreatingGoal(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-[60] md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <div className="flex items-center gap-3 mb-6">
                                    <div className="p-3 bg-gradient-to-tr from-emerald-500 to-teal-500 rounded-xl">
                                        <Target className="w-5 h-5 text-white" />
                                    </div>
                                    <div>
                                        <h2 className="text-lg font-bold text-white">Quick Create Goal</h2>
                                        <p className="text-xs text-gray-400">Link to your expense</p>
                                    </div>
                                </div>
                                <form onSubmit={handleCreateGoal} className="space-y-4">
                                    <Input
                                        label="Goal Title"
                                        placeholder="e.g. New Car"
                                        required
                                        value={newGoal.title}
                                        onChange={(e) => setNewGoal({ ...newGoal, title: e.target.value })}
                                    />
                                    <Input
                                        label="Target Amount"
                                        type="number"
                                        required
                                        placeholder="e.g. 5000"
                                        value={newGoal.targetAmount || ''}
                                        onChange={(e) => setNewGoal({ ...newGoal, targetAmount: parseFloat(e.target.value) || 0 })}
                                    />
                                    <Input
                                        label="Deadline"
                                        type="date"
                                        value={newGoal.deadline?.split('T')[0] || ''}
                                        onChange={(e) => setNewGoal({ ...newGoal, deadline: e.target.value })}
                                    />
                                    <div className="flex flex-col-reverse gap-3 pt-4 md:flex-row">
                                        <Button type="button" variant="ghost" onClick={() => setIsCreatingGoal(false)}>
                                            Cancel
                                        </Button>
                                        <Button type="submit" className="bg-gradient-to-r from-emerald-500 to-teal-500">
                                            Create Goal
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
                onClose={() => setDeleteConfirm({ isOpen: false, expenseId: null })}
                onConfirm={confirmDelete}
                title="Delete Expense?"
                message="This action cannot be undone."
                confirmText="Delete"
                cancelText="Cancel"
                variant="danger"
            />

            {/* FAB - Mobile only */}
            <button
                onClick={() => setIsModalOpen(true)}
                className="fab md:hidden"
            >
                <Plus className="w-6 h-6 text-white" />
            </button>
        </div>
    );
};
