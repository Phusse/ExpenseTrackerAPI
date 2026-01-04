import { AlertTriangle, X } from 'lucide-react';
import { Button } from './Button';

interface ConfirmModalProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => void;
    title: string;
    message: string;
    confirmText?: string;
    cancelText?: string;
    variant?: 'danger' | 'warning' | 'info';
}

export const ConfirmModal = ({
    isOpen,
    onClose,
    onConfirm,
    title,
    message,
    confirmText = 'Confirm',
    cancelText = 'Cancel',
    variant = 'danger'
}: ConfirmModalProps) => {
    if (!isOpen) return null;

    const variantStyles = {
        danger: {
            iconBg: 'bg-rose-500/10',
            iconColor: 'text-rose-500',
            buttonBg: 'bg-rose-500 hover:bg-rose-600'
        },
        warning: {
            iconBg: 'bg-amber-500/10',
            iconColor: 'text-amber-500',
            buttonBg: 'bg-amber-500 hover:bg-amber-600'
        },
        info: {
            iconBg: 'bg-blue-500/10',
            iconColor: 'text-blue-500',
            buttonBg: 'bg-primary hover:bg-blue-600'
        }
    };

    const styles = variantStyles[variant];

    const handleConfirm = () => {
        onConfirm();
        onClose();
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4">
            <div className="bg-surface border border-slate-700 rounded-2xl w-full max-w-md shadow-2xl p-6 animate-in fade-in zoom-in-95 duration-200">
                <div className="flex items-start gap-4 mb-6">
                    <div className={`p-3 rounded-xl ${styles.iconBg}`}>
                        <AlertTriangle className={`w-6 h-6 ${styles.iconColor}`} />
                    </div>
                    <div className="flex-1">
                        <h2 className="text-xl font-bold text-white mb-2">{title}</h2>
                        <p className="text-gray-400 text-sm">{message}</p>
                    </div>
                    <button
                        onClick={onClose}
                        className="text-gray-500 hover:text-white transition-colors"
                    >
                        <X size={20} />
                    </button>
                </div>
                <div className="flex justify-end gap-3">
                    <Button
                        type="button"
                        variant="ghost"
                        onClick={onClose}
                    >
                        {cancelText}
                    </Button>
                    <Button
                        type="button"
                        onClick={handleConfirm}
                        className={styles.buttonBg}
                    >
                        {confirmText}
                    </Button>
                </div>
            </div>
        </div>
    );
};
