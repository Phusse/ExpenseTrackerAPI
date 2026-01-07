import { AlertTriangle, Trash2 } from 'lucide-react';
import { Button } from './Button';

interface ConfirmModalProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => void;
    title: string;
    message: string;
    confirmText?: string;
    cancelText?: string;
    variant?: 'danger' | 'warning' | 'default';
}

export const ConfirmModal = ({
    isOpen,
    onClose,
    onConfirm,
    title,
    message,
    confirmText = 'Confirm',
    cancelText = 'Cancel',
    variant = 'default'
}: ConfirmModalProps) => {
    if (!isOpen) return null;

    const handleConfirm = () => {
        onConfirm();
        onClose();
    };

    const iconColors = {
        danger: 'bg-danger/10 text-danger',
        warning: 'bg-warning/10 text-warning',
        default: 'bg-primary/10 text-primary'
    };

    return (
        <>
            {/* Overlay */}
            <div
                className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm animate-fade-in"
                onClick={onClose}
            />

            {/* Modal - Bottom sheet on mobile, centered on desktop */}
            <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md animate-slide-up md:animate-fade-in">
                    {/* Drag handle - mobile only */}
                    <div className="bottom-sheet-handle md:hidden" />

                    <div className="p-6 pb-8 md:pb-6">
                        {/* Icon */}
                        <div className="flex justify-center mb-4">
                            <div className={`w-14 h-14 rounded-full ${iconColors[variant]} flex items-center justify-center`}>
                                {variant === 'danger' ? (
                                    <Trash2 className="w-7 h-7" />
                                ) : (
                                    <AlertTriangle className="w-7 h-7" />
                                )}
                            </div>
                        </div>

                        {/* Content */}
                        <h3 className="text-xl font-bold text-white text-center mb-2">{title}</h3>
                        <p className="text-gray-400 text-center text-sm mb-6">{message}</p>

                        {/* Actions - Stacked on mobile, side by side on desktop */}
                        <div className="flex flex-col-reverse gap-3 md:flex-row md:gap-3">
                            <Button
                                variant="ghost"
                                onClick={onClose}
                                className="md:flex-1"
                            >
                                {cancelText}
                            </Button>
                            <Button
                                variant={variant === 'danger' ? 'danger' : 'primary'}
                                onClick={handleConfirm}
                                className="md:flex-1"
                            >
                                {confirmText}
                            </Button>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};
