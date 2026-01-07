import { useState, useEffect } from 'react';
import { Plus, Trash2, Edit2, DollarSign, TrendingUp, Calendar, RefreshCw, Search, Briefcase, Monitor, Building2, BarChart2, Home, Gift, Wallet, FileText } from 'lucide-react';
import {
    incomeService,
    type Income,
    type CreateIncomeRequest,
    IncomeSource,
    RecurrenceFrequency,
    getIncomeSourceColor
} from '../services/incomeService';
import { useToast } from '../context/ToastContext';
import { useSettings } from '../context/SettingsContext';
import { ConfirmModal } from '../components/ConfirmModal';
import { Button } from '../components/Button';
import { Input } from '../components/Input';

const IncomeSourceIcon = ({ source, size = 20 }: { source: IncomeSource; size?: number }) => {
    const iconProps = { size, className: 'text-white' };
    switch (source) {
        case IncomeSource.Salary: return <Briefcase {...iconProps} />;
        case IncomeSource.Freelance: return <Monitor {...iconProps} />;
        case IncomeSource.Business: return <Building2 {...iconProps} />;
        case IncomeSource.Investments: return <BarChart2 {...iconProps} />;
        case IncomeSource.Rental: return <Home {...iconProps} />;
        case IncomeSource.Gift: return <Gift {...iconProps} />;
        case IncomeSource.Refund: return <Wallet {...iconProps} />;
        case IncomeSource.Other: return <FileText {...iconProps} />;
        default: return <DollarSign {...iconProps} />;
    }
};

const EmptyState = ({ onAddClick }: { onAddClick: () => void }) => (
    <div className="flex flex-col items-center justify-center py-12 px-4 text-center">
        <div className="w-20 h-20 rounded-2xl bg-gradient-to-br from-emerald-500/20 to-green-500/20 flex items-center justify-center mb-6">
            <DollarSign className="w-10 h-10 text-emerald-400" />
        </div>
        <h3 className="text-xl font-semibold text-white mb-2">No income recorded</h3>
        <p className="text-gray-400 text-sm max-w-xs mb-6">
            Start tracking your income to get a complete picture of your finances.
        </p>
        <Button onClick={onAddClick}>
            <Plus size={20} className="mr-2" />
            Add Your First Income
        </Button>
    </div>
);

export default function Income() {
    const [incomes, setIncomes] = useState<Income[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingIncome, setEditingIncome] = useState<Income | null>(null);
    const [deleteConfirm, setDeleteConfirm] = useState<string | null>(null);
    const [searchQuery, setSearchQuery] = useState('');
    const [filterSource, setFilterSource] = useState<IncomeSource | ''>('');
    const toast = useToast();
    const { formatCurrency } = useSettings();

    const [formData, setFormData] = useState<CreateIncomeRequest>({
        source: IncomeSource.Salary,
        amount: 0,
        dateReceived: new Date().toISOString().split('T')[0],
        description: '',
        isRecurring: false,
        frequency: undefined
    });

    const fetchIncomes = async () => {
        setIsLoading(true);
        try {
            const data = await incomeService.getAll();
            setIncomes(data);
        } catch {
            // Error handled
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchIncomes();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        try {
            if (editingIncome) {
                await incomeService.update(editingIncome.id, { id: editingIncome.id, ...formData });
                toast.success('Income Updated', 'Income record updated successfully');
            } else {
                await incomeService.create(formData);
                toast.success('Income Added', 'New income record created');
            }
            setIsModalOpen(false);
            setEditingIncome(null);
            resetForm();
            fetchIncomes();
        } catch {
            // Error handled
        }
    };

    const handleDelete = async (id: string) => {
        try {
            await incomeService.delete(id);
            toast.success('Income Deleted', 'Income record deleted');
            setDeleteConfirm(null);
            fetchIncomes();
        } catch {
            // Error handled
        }
    };

    const handleEdit = (income: Income) => {
        setEditingIncome(income);
        setFormData({
            source: income.source,
            amount: income.amount,
            dateReceived: income.dateReceived.split('T')[0],
            description: income.description || '',
            isRecurring: income.isRecurring,
            frequency: income.frequency
        });
        setIsModalOpen(true);
    };

    const resetForm = () => {
        setFormData({
            source: IncomeSource.Salary,
            amount: 0,
            dateReceived: new Date().toISOString().split('T')[0],
            description: '',
            isRecurring: false,
            frequency: undefined
        });
    };

    const openNewModal = () => {
        setEditingIncome(null);
        resetForm();
        setIsModalOpen(true);
    };

    const filteredIncomes = incomes.filter(income => {
        const matchesSearch = !searchQuery ||
            income.description?.toLowerCase().includes(searchQuery.toLowerCase()) ||
            income.sourceName.toLowerCase().includes(searchQuery.toLowerCase());
        const matchesSource = !filterSource || income.source === filterSource;
        return matchesSearch && matchesSource;
    });

    const totalIncome = filteredIncomes.reduce((sum, i) => sum + i.amount, 0);

    return (
        <div className="space-y-4 md:space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-xl md:text-2xl font-bold text-white">Income</h1>
                    <p className="text-sm text-gray-400 hidden md:block">Track your revenue streams</p>
                </div>
                <Button onClick={openNewModal} className="!w-auto bg-gradient-to-r from-emerald-500 to-green-600">
                    <Plus size={20} className="md:mr-2" />
                    <span className="hidden md:inline">Add Income</span>
                </Button>
            </div>

            {/* Summary Card */}
            <div className="glass-card p-4 md:p-6">
                <div className="flex items-center justify-between">
                    <div>
                        <p className="text-emerald-400 text-xs md:text-sm font-medium uppercase">Total Income</p>
                        <p className="text-2xl md:text-3xl font-bold text-white mt-1">
                            {formatCurrency(totalIncome)}
                        </p>
                        <p className="text-gray-400 text-xs mt-1">
                            {filteredIncomes.length} transaction{filteredIncomes.length !== 1 ? 's' : ''}
                        </p>
                    </div>
                    <div className="w-12 h-12 rounded-xl bg-gradient-to-br from-emerald-500/20 to-teal-500/20 flex items-center justify-center">
                        <TrendingUp className="w-6 h-6 text-emerald-400" />
                    </div>
                </div>
            </div>

            {/* Filters */}
            <div className="flex gap-3">
                <div className="relative flex-1">
                    <Search className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500 w-5 h-5" />
                    <input
                        type="text"
                        placeholder="Search income..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="input-glass pl-12"
                    />
                </div>
                <select
                    value={filterSource}
                    onChange={(e) => setFilterSource(e.target.value as IncomeSource | '')}
                    className="input-glass !w-auto"
                >
                    <option value="">All Sources</option>
                    {Object.values(IncomeSource).map(source => (
                        <option key={source} value={source}>{source}</option>
                    ))}
                </select>
            </div>

            {/* Income List */}
            {isLoading ? (
                <div className="flex justify-center py-12">
                    <div className="w-8 h-8 border-4 border-emerald-500/30 border-t-emerald-500 rounded-full animate-spin" />
                </div>
            ) : incomes.length === 0 ? (
                <EmptyState onAddClick={openNewModal} />
            ) : (
                <div className="space-y-2">
                    {filteredIncomes.map(income => (
                        <div
                            key={income.id}
                            className="glass-card p-4 flex items-center gap-3 active:scale-[0.99] transition-transform"
                        >
                            <div
                                className="w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0"
                                style={{ backgroundColor: `${getIncomeSourceColor(income.source)}20`, border: `1px solid ${getIncomeSourceColor(income.source)}30` }}
                            >
                                <div style={{ color: getIncomeSourceColor(income.source) }}>
                                    <IncomeSourceIcon source={income.source} size={20} />
                                </div>
                            </div>
                            <div className="flex-1 min-w-0">
                                <div className="flex items-center gap-2 mb-0.5">
                                    <span className="font-medium text-white text-sm">{income.sourceName}</span>
                                    {income.isRecurring && (
                                        <span className="text-[10px] bg-blue-500/10 text-blue-400 px-1.5 py-0.5 rounded flex items-center gap-0.5">
                                            <RefreshCw size={8} />
                                            {income.frequencyName}
                                        </span>
                                    )}
                                </div>
                                {income.description && (
                                    <p className="text-gray-400 text-xs truncate">{income.description}</p>
                                )}
                                <p className="text-gray-500 text-xs mt-0.5 flex items-center gap-1">
                                    <Calendar size={10} />
                                    {new Date(income.dateReceived).toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}
                                </p>
                            </div>
                            <span className="text-emerald-400 font-bold text-sm flex-shrink-0">
                                +{formatCurrency(income.amount)}
                            </span>
                            <div className="flex items-center gap-1">
                                <button
                                    onClick={() => handleEdit(income)}
                                    className="p-2 text-gray-400 hover:text-white active:scale-95"
                                >
                                    <Edit2 size={16} />
                                </button>
                                <button
                                    onClick={() => setDeleteConfirm(income.id)}
                                    className="p-2 text-gray-400 hover:text-rose-500 active:scale-95"
                                >
                                    <Trash2 size={16} />
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {/* Add/Edit Modal */}
            {isModalOpen && (
                <>
                    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={() => setIsModalOpen(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md max-h-[90vh] overflow-auto animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <h2 className="text-xl font-bold text-white mb-6">
                                    {editingIncome ? 'Edit Income' : 'Add Income'}
                                </h2>
                                <form onSubmit={handleSubmit} className="space-y-4">
                                    <div>
                                        <label className="block text-sm font-medium text-gray-300 mb-2">Source</label>
                                        <select
                                            value={formData.source}
                                            onChange={(e) => setFormData({ ...formData, source: e.target.value as IncomeSource })}
                                            className="input-glass"
                                            required
                                        >
                                            {Object.values(IncomeSource).map(source => (
                                                <option key={source} value={source}>{source}</option>
                                            ))}
                                        </select>
                                    </div>
                                    <Input
                                        label="Amount (₦)"
                                        type="number"
                                        value={formData.amount || ''}
                                        onChange={(e) => setFormData({ ...formData, amount: parseFloat(e.target.value) || 0 })}
                                        placeholder="0.00"
                                        required
                                    />
                                    <Input
                                        label="Date Received"
                                        type="date"
                                        value={formData.dateReceived}
                                        onChange={(e) => setFormData({ ...formData, dateReceived: e.target.value })}
                                        required
                                    />
                                    <Input
                                        label="Description (Optional)"
                                        value={formData.description}
                                        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                                        placeholder="e.g., January salary"
                                    />
                                    <div className="flex items-center gap-3 p-3 rounded-xl bg-white/5">
                                        <input
                                            type="checkbox"
                                            id="isRecurring"
                                            checked={formData.isRecurring}
                                            onChange={(e) => setFormData({
                                                ...formData,
                                                isRecurring: e.target.checked,
                                                frequency: e.target.checked ? RecurrenceFrequency.Monthly : undefined
                                            })}
                                            className="w-5 h-5 rounded"
                                        />
                                        <label htmlFor="isRecurring" className="text-sm text-gray-300">
                                            Recurring income
                                        </label>
                                    </div>
                                    {formData.isRecurring && (
                                        <div>
                                            <label className="block text-sm font-medium text-gray-300 mb-2">Frequency</label>
                                            <select
                                                value={formData.frequency || ''}
                                                onChange={(e) => setFormData({ ...formData, frequency: e.target.value as RecurrenceFrequency })}
                                                className="input-glass"
                                            >
                                                {Object.values(RecurrenceFrequency).map(freq => (
                                                    <option key={freq} value={freq}>{freq}</option>
                                                ))}
                                            </select>
                                        </div>
                                    )}
                                    <div className="flex flex-col-reverse gap-3 pt-4 md:flex-row">
                                        <Button type="button" variant="ghost" onClick={() => setIsModalOpen(false)}>
                                            Cancel
                                        </Button>
                                        <Button type="submit" className="bg-gradient-to-r from-emerald-500 to-green-600">
                                            {editingIncome ? 'Update' : 'Add Income'}
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
                isOpen={!!deleteConfirm}
                onClose={() => setDeleteConfirm(null)}
                onConfirm={() => deleteConfirm && handleDelete(deleteConfirm)}
                title="Delete Income"
                message="This action cannot be undone."
                confirmText="Delete"
                variant="danger"
            />

            {/* FAB */}
            <button onClick={openNewModal} className="fab md:hidden bg-gradient-to-br from-emerald-500 to-green-600">
                <Plus className="w-6 h-6 text-white" />
            </button>
        </div>
    );
}
