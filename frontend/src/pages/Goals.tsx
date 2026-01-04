import { useEffect, useState } from 'react';
import { Plus, Target, Trash2, Loader2, Sparkles, Trophy, Rocket, PiggyBank, Calendar, Palmtree, Car, Home, Wallet } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { ConfirmModal } from '../components/ConfirmModal';
import { goalService } from '../services/goalService';
import type { SavingGoal, CreateSavingGoalDto } from '../types';

const EmptyState = ({ onAddClick }: { onAddClick: () => void }) => (
    <div className="col-span-full py-16 flex flex-col items-center justify-center">
        <div className="relative mb-6">
            <div className="w-24 h-24 rounded-full bg-gradient-to-tr from-emerald-500/20 to-teal-500/20 flex items-center justify-center">
                <Target className="w-12 h-12 text-emerald-400" />
            </div>
            <div className="absolute -top-2 -right-2 w-8 h-8 rounded-full bg-gradient-to-r from-emerald-500 to-teal-500 flex items-center justify-center shadow-lg">
                <Sparkles className="w-4 h-4 text-white" />
            </div>
        </div>
        <h3 className="text-2xl font-bold text-white mb-2">Start Your Savings Journey</h3>
        <p className="text-gray-400 text-center max-w-md mb-8">
            Set meaningful goals and watch your dreams come to life. Whether it's a vacation, a new gadget, or an emergency fund – every goal matters.
        </p>
        <Button onClick={onAddClick} className="gap-2 bg-gradient-to-r from-emerald-500 to-teal-500 hover:from-emerald-600 hover:to-teal-600">
            <Plus className="w-5 h-5" />
            Create Your First Goal
        </Button>

        {/* Inspiration Section */}
        <div className="mt-12 w-full max-w-3xl">
            <h4 className="text-center text-gray-500 text-sm font-medium mb-6">POPULAR SAVING GOALS</h4>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div className="bg-slate-900/50 rounded-xl p-4 text-center border border-slate-800 hover:border-slate-700 transition-colors cursor-pointer group" onClick={onAddClick}>
                    <div className="flex items-center justify-center mb-2"><Palmtree className="w-8 h-8 text-blue-400 group-hover:text-blue-300 transition-colors" /></div>
                    <p className="text-sm text-gray-400 group-hover:text-white transition-colors">Vacation</p>
                </div>
                <div className="bg-slate-900/50 rounded-xl p-4 text-center border border-slate-800 hover:border-slate-700 transition-colors cursor-pointer group" onClick={onAddClick}>
                    <div className="flex items-center justify-center mb-2"><Car className="w-8 h-8 text-violet-400 group-hover:text-violet-300 transition-colors" /></div>
                    <p className="text-sm text-gray-400 group-hover:text-white transition-colors">New Car</p>
                </div>
                <div className="bg-slate-900/50 rounded-xl p-4 text-center border border-slate-800 hover:border-slate-700 transition-colors cursor-pointer group" onClick={onAddClick}>
                    <div className="flex items-center justify-center mb-2"><Home className="w-8 h-8 text-emerald-400 group-hover:text-emerald-300 transition-colors" /></div>
                    <p className="text-sm text-gray-400 group-hover:text-white transition-colors">Home</p>
                </div>
                <div className="bg-slate-900/50 rounded-xl p-4 text-center border border-slate-800 hover:border-slate-700 transition-colors cursor-pointer group" onClick={onAddClick}>
                    <div className="flex items-center justify-center mb-2"><Wallet className="w-8 h-8 text-amber-400 group-hover:text-amber-300 transition-colors" /></div>
                    <p className="text-sm text-gray-400 group-hover:text-white transition-colors">Emergency Fund</p>
                </div>
            </div>
        </div>

        {/* Benefits */}
        <div className="mt-10 grid grid-cols-1 md:grid-cols-3 gap-6 w-full max-w-2xl">
            <div className="flex items-center gap-3 p-4 bg-slate-900/30 rounded-lg">
                <div className="p-2 bg-emerald-500/10 rounded-lg text-emerald-400">
                    <Trophy size={20} />
                </div>
                <div>
                    <p className="text-white text-sm font-medium">Track Progress</p>
                    <p className="text-xs text-gray-500">Visual progress bars</p>
                </div>
            </div>
            <div className="flex items-center gap-3 p-4 bg-slate-900/30 rounded-lg">
                <div className="p-2 bg-blue-500/10 rounded-lg text-blue-400">
                    <Calendar size={20} />
                </div>
                <div>
                    <p className="text-white text-sm font-medium">Set Deadlines</p>
                    <p className="text-xs text-gray-500">Stay motivated</p>
                </div>
            </div>
            <div className="flex items-center gap-3 p-4 bg-slate-900/30 rounded-lg">
                <div className="p-2 bg-violet-500/10 rounded-lg text-violet-400">
                    <Rocket size={20} />
                </div>
                <div>
                    <p className="text-white text-sm font-medium">Achieve Dreams</p>
                    <p className="text-xs text-gray-500">Celebrate success</p>
                </div>
            </div>
        </div>
    </div>
);

export const Goals = () => {
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
            setNewGoal({
                title: '',
                targetAmount: 0,
                deadline: new Date(new Date().setFullYear(new Date().getFullYear() + 1)).toISOString().split('T')[0]
            });
        } catch (error) {
            console.error('Failed to create goal', error);
        }
    };

    const handleDelete = async (id: string) => {
        setDeleteConfirm({ isOpen: true, goalId: id });
    };

    const confirmDelete = async () => {
        if (deleteConfirm.goalId) {
            await goalService.delete(deleteConfirm.goalId);
            setGoals(goals.filter(g => g.id !== deleteConfirm.goalId));
        }
    };

    const totalSaved = goals.reduce((sum, g) => sum + g.currentAmount, 0);
    const totalTarget = goals.reduce((sum, g) => sum + g.targetAmount, 0);

    return (
        <div className="space-y-6">
            <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-white mb-2">Saving Goals</h1>
                    <p className="text-gray-400">Track your progress towards your financial dreams.</p>
                </div>
                <div className="flex items-center gap-4">
                    {goals.length > 0 && (
                        <div className="bg-surface border border-slate-800 rounded-lg px-4 py-2">
                            <span className="text-gray-400 text-sm">Saved:</span>
                            <span className="text-emerald-400 font-semibold ml-2">₦{totalSaved.toLocaleString()}</span>
                            <span className="text-gray-500 mx-1">/</span>
                            <span className="text-gray-400">₦{totalTarget.toLocaleString()}</span>
                        </div>
                    )}
                    <Button onClick={() => setIsModalOpen(true)}>
                        <Plus className="w-5 h-5 mr-2" />
                        Add Goal
                    </Button>
                </div>
            </div>

            {loading ? (
                <div className="flex justify-center py-12">
                    <Loader2 className="w-8 h-8 animate-spin text-primary" />
                </div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {goals.map((goal) => {
                        const percentage = Math.min(100, (goal.currentAmount / goal.targetAmount) * 100);
                        const isComplete = percentage >= 100;
                        const daysLeft = goal.deadline ? Math.ceil((new Date(goal.deadline).getTime() - Date.now()) / (1000 * 60 * 60 * 24)) : 0;

                        return (
                            <div key={goal.id} className={`bg-surface border rounded-xl p-6 transition-all group ${isComplete ? 'border-emerald-500/50 bg-emerald-500/5' : 'border-slate-800 hover:border-slate-700'}`}>
                                <div className="flex justify-between items-start mb-6">
                                    <div className="flex items-center gap-3">
                                        <div className={`p-3 rounded-full border ${isComplete ? 'bg-emerald-500/20 border-emerald-500/30 text-emerald-400' : 'bg-white/5 border-white/10 text-emerald-400'}`}>
                                            {isComplete ? <Trophy size={24} /> : <Target size={24} />}
                                        </div>
                                        <div>
                                            <h3 className="font-semibold text-white text-lg">{goal.title}</h3>
                                            <p className="text-sm text-gray-400">
                                                {isComplete ? 'Goal achieved!' : daysLeft > 0 ? `${daysLeft} days left` : 'Past deadline'}
                                            </p>
                                        </div>
                                    </div>
                                    <button onClick={() => handleDelete(goal.id)} className="text-gray-500 hover:text-rose-500 transition-colors opacity-0 group-hover:opacity-100">
                                        <Trash2 size={18} />
                                    </button>
                                </div>

                                <div className="flex items-end justify-between mb-2">
                                    <span className="text-3xl font-bold text-white">₦{goal.currentAmount.toLocaleString()}</span>
                                    <span className="text-sm text-gray-400 mb-1">of ₦{goal.targetAmount.toLocaleString()}</span>
                                </div>

                                <div className="h-3 bg-slate-900 rounded-full overflow-hidden mb-4">
                                    <div
                                        className={`h-full rounded-full transition-all ${isComplete ? 'bg-emerald-500' : 'bg-gradient-to-r from-emerald-500 to-teal-400'}`}
                                        style={{ width: `${percentage}%` }}
                                    />
                                </div>

                                <div className="flex justify-between text-xs text-gray-500 font-medium mb-4">
                                    <span className={isComplete ? 'text-emerald-400' : ''}>{percentage.toFixed(0)}% Complete</span>
                                    <span>₦{(goal.targetAmount - goal.currentAmount).toLocaleString()} to go</span>
                                </div>
                            </div>
                        );
                    })}

                    {goals.length === 0 && <EmptyState onAddClick={() => setIsModalOpen(true)} />}
                </div>
            )}

            {/* Create Goal Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
                    <div className="bg-surface border border-slate-700 rounded-2xl w-full max-w-md shadow-2xl p-6 animate-in fade-in zoom-in-95 duration-200">
                        <div className="flex items-center gap-3 mb-6">
                            <div className="p-3 bg-gradient-to-tr from-emerald-500 to-teal-500 rounded-xl">
                                <Target className="w-6 h-6 text-white" />
                            </div>
                            <div>
                                <h2 className="text-xl font-bold text-white">Create Saving Goal</h2>
                                <p className="text-sm text-gray-400">What are you saving for?</p>
                            </div>
                        </div>
                        <form onSubmit={handleCreate} className="space-y-4">
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
                                label="Deadline"
                                type="date"
                                value={newGoal.deadline?.split('T')[0] || ''}
                                onChange={(e) => setNewGoal({ ...newGoal, deadline: e.target.value })}
                            />

                            <div className="flex justify-end gap-3 mt-6">
                                <Button type="button" variant="ghost" onClick={() => setIsModalOpen(false)}>Cancel</Button>
                                <Button type="submit" className="bg-gradient-to-r from-emerald-500 to-teal-500 hover:from-emerald-600 hover:to-teal-600">
                                    Set Goal
                                </Button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Delete Confirmation Modal */}
            <ConfirmModal
                isOpen={deleteConfirm.isOpen}
                onClose={() => setDeleteConfirm({ isOpen: false, goalId: null })}
                onConfirm={confirmDelete}
                title="Delete Saving Goal?"
                message="Are you sure you want to delete this saving goal? This action cannot be undone and all progress will be lost."
                confirmText="Delete"
                cancelText="Cancel"
                variant="danger"
            />
        </div>
    );
};
