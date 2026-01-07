import { useEffect, useState } from 'react';
import { Plus, Target, Trash2, Loader2, Sparkles, Trophy } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { ConfirmModal } from '../components/ConfirmModal';
import { goalService } from '../services/goalService';
import { useToast } from '../context/ToastContext';
import { useSettings } from '../context/SettingsContext';
import type { SavingGoal, CreateSavingGoalDto } from '../types';

const EmptyState = ({ onAddClick }: { onAddClick: () => void }) => (
    <div className="py-12 flex flex-col items-center justify-center text-center px-4">
        <div className="relative mb-6">
            <div className="w-20 h-20 rounded-full bg-gradient-to-tr from-emerald-500/20 to-teal-500/20 flex items-center justify-center ring-1 ring-white/10">
                <Target className="w-10 h-10 text-emerald-400" />
            </div>
            <div className="absolute -top-1 -right-1 w-7 h-7 rounded-full bg-emerald-500 flex items-center justify-center">
                <Sparkles className="w-3.5 h-3.5 text-white" />
            </div>
        </div>
        <h3 className="text-xl font-bold text-white mb-2">Start Your Savings Journey</h3>
        <p className="text-gray-400 text-sm max-w-xs mb-6">
            Set meaningful goals and track your progress towards financial freedom.
        </p>
        <Button onClick={onAddClick} className="bg-gradient-to-r from-emerald-500 to-teal-500">
            <Plus className="w-5 h-5 mr-2" />
            Create First Goal
        </Button>
    </div>
);

export const Goals = () => {
    const toast = useToast();
    const { formatCurrency } = useSettings();
    const [goals, setGoals] = useState<SavingGoal[]>([]);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [deleteConfirm, setDeleteConfirm] = useState<{ isOpen: boolean; goalId: string | null }>({
        isOpen: false,
        goalId: null
    });

    const [newGoal, setNewGoal] = useState<CreateSavingGoalDto>({
        title: '',
        targetAmount: 0,
        deadline: new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().split('T')[0]
    });

    useEffect(() => {
        fetchGoals();
    }, []);

    const fetchGoals = async () => {
        try {
            setLoading(true);
            const data = await goalService.getAll();
            setGoals(data);
        } catch (error) {
            console.error('Failed to fetch goals', error);
        } finally {
            setLoading(false);
        }
    };

    const handleCreate = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            await goalService.create(newGoal);
            setIsModalOpen(false);
            fetchGoals();
            toast.success('Goal Created', `"${newGoal.title}" has been set.`);
            setNewGoal({
                title: '',
                targetAmount: 0,
                deadline: new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().split('T')[0]
            });
        } catch (error) {
            // Error handled
        }
    };

    const handleDelete = (id: string) => {
        setDeleteConfirm({ isOpen: true, goalId: id });
    };

    const confirmDelete = async () => {
        if (deleteConfirm.goalId) {
            try {
                await goalService.delete(deleteConfirm.goalId);
                setGoals(goals.filter(g => g.id !== deleteConfirm.goalId));
                toast.success('Goal Deleted', 'The saving goal has been removed.');
            } catch (error) {
                // Error handled
            }
        }
    };

    const totalSaved = goals.reduce((sum, g) => sum + g.currentAmount, 0);
    const totalTarget = goals.reduce((sum, g) => sum + g.targetAmount, 0);

    return (
        <div className="space-y-4 md:space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-xl md:text-2xl font-bold text-white">Saving Goals</h1>
                    <p className="text-sm text-gray-400 hidden md:block">Track your financial dreams</p>
                </div>
                <Button onClick={() => setIsModalOpen(true)} className="!w-auto bg-gradient-to-r from-emerald-500 to-teal-500">
                    <Plus className="w-5 h-5 md:mr-2" />
                    <span className="hidden md:inline">Add Goal</span>
                </Button>
            </div>

            {/* Total Summary */}
            {goals.length > 0 && (
                <div className="glass-card p-4 flex items-center justify-between">
                    <div>
                        <p className="text-xs text-gray-400">Total Progress</p>
                        <div className="flex items-baseline gap-2">
                            <span className="text-lg font-bold text-emerald-400">{formatCurrency(totalSaved)}</span>
                            <span className="text-gray-500">/</span>
                            <span className="text-gray-400">{formatCurrency(totalTarget)}</span>
                        </div>
                    </div>
                    <div className="w-10 h-10 rounded-xl bg-emerald-500/10 flex items-center justify-center">
                        <Target className="w-5 h-5 text-emerald-400" />
                    </div>
                </div>
            )}

            {/* Goals List */}
            {loading ? (
                <div className="flex justify-center py-12">
                    <Loader2 className="w-8 h-8 animate-spin text-primary" />
                </div>
            ) : goals.length === 0 ? (
                <EmptyState onAddClick={() => setIsModalOpen(true)} />
            ) : (
                <div className="space-y-3">
                    {goals.map((goal) => {
                        const percentage = Math.min((goal.currentAmount / goal.targetAmount) * 100, 100);
                        const isComplete = percentage >= 100;
                        const daysLeft = goal.deadline ? Math.ceil((new Date(goal.deadline).getTime() - Date.now()) / (1000 * 60 * 60 * 24)) : 0;

                        return (
                            <div
                                key={goal.id}
                                className={`glass-card p-4 ${isComplete ? 'border-emerald-500/30' : ''}`}
                            >
                                <div className="flex items-start justify-between mb-3">
                                    <div className="flex items-center gap-3">
                                        <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${isComplete ? 'bg-emerald-500/10 text-emerald-400' : 'bg-white/5 text-emerald-400'}`}>
                                            {isComplete ? <Trophy className="w-5 h-5" /> : <Target className="w-5 h-5" />}
                                        </div>
                                        <div>
                                            <h3 className="font-bold text-white">{goal.title}</h3>
                                            <p className="text-xs text-gray-500">
                                                {isComplete ? '🎉 Achieved!' : daysLeft > 0 ? `${daysLeft} days left` : 'Past deadline'}
                                            </p>
                                        </div>
                                    </div>
                                    <button
                                        onClick={() => handleDelete(goal.id)}
                                        className="p-2 text-gray-500 hover:text-rose-500 active:scale-95"
                                    >
                                        <Trash2 className="w-4 h-4" />
                                    </button>
                                </div>

                                <div className="flex justify-between items-end mb-2">
                                    <span className="text-xl font-bold text-white">
                                        {formatCurrency(goal.currentAmount)}
                                    </span>
                                    <span className="text-gray-400 text-sm">
                                        of {formatCurrency(goal.targetAmount)}
                                    </span>
                                </div>

                                <div className="h-2 bg-slate-800 rounded-full overflow-hidden mb-2">
                                    <div
                                        className={`h-full transition-all ${isComplete ? 'bg-emerald-500' : 'bg-gradient-to-r from-emerald-500 to-teal-400'}`}
                                        style={{ width: `${percentage}%` }}
                                    />
                                </div>

                                <div className="flex justify-between text-xs">
                                    <span className={isComplete ? 'text-emerald-400' : 'text-teal-400'}>
                                        {percentage.toFixed(0)}% complete
                                    </span>
                                    <span className="text-gray-500">
                                        {formatCurrency(goal.targetAmount - goal.currentAmount)} to go
                                    </span>
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}

            {/* Create Goal Modal */}
            {isModalOpen && (
                <>
                    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={() => setIsModalOpen(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <div className="flex items-center gap-3 mb-6">
                                    <div className="p-3 bg-gradient-to-tr from-emerald-500 to-teal-500 rounded-xl">
                                        <Target className="w-5 h-5 text-white" />
                                    </div>
                                    <div>
                                        <h2 className="text-lg font-bold text-white">Create Goal</h2>
                                        <p className="text-xs text-gray-400">What are you saving for?</p>
                                    </div>
                                </div>
                                <form onSubmit={handleCreate} className="space-y-4">
                                    <Input
                                        label="Goal Title"
                                        placeholder="e.g. New Car, Vacation"
                                        required
                                        value={newGoal.title}
                                        onChange={(e) => setNewGoal({ ...newGoal, title: e.target.value })}
                                    />
                                    <Input
                                        label="Target Amount (₦)"
                                        type="number"
                                        required
                                        placeholder="e.g. 500000"
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
                                        <Button type="button" variant="ghost" onClick={() => setIsModalOpen(false)}>
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
                onClose={() => setDeleteConfirm({ isOpen: false, goalId: null })}
                onConfirm={confirmDelete}
                title="Delete Goal?"
                message="All progress will be lost. This cannot be undone."
                confirmText="Delete"
                cancelText="Cancel"
                variant="danger"
            />

            {/* FAB */}
            <button onClick={() => setIsModalOpen(true)} className="fab md:hidden bg-gradient-to-br from-emerald-500 to-teal-500">
                <Plus className="w-6 h-6 text-white" />
            </button>
        </div>
    );
};
