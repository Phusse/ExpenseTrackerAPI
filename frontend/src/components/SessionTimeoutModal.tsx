import { Clock, LogOut, RefreshCw } from 'lucide-react';
import { Button } from './Button';

interface SessionTimeoutModalProps {
    isOpen: boolean;
    countdown: number;
    onStayLoggedIn: () => void;
    onLogout: () => void;
}

export const SessionTimeoutModal = ({
    isOpen,
    countdown,
    onStayLoggedIn,
    onLogout
}: SessionTimeoutModalProps) => {
    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
            <div className="bg-surface border border-slate-700 rounded-2xl w-full max-w-md shadow-2xl p-6 animate-in fade-in zoom-in-95 duration-200">
                {/* Header with animated icon */}
                <div className="flex flex-col items-center text-center mb-6">
                    <div className="relative mb-4">
                        <div className="absolute inset-0 bg-amber-500/20 rounded-full animate-ping" />
                        <div className="relative p-4 bg-amber-500/10 rounded-full">
                            <Clock className="w-8 h-8 text-amber-500" />
                        </div>
                    </div>
                    <h2 className="text-xl font-bold text-white mb-2">
                        Session Expiring Soon
                    </h2>
                    <p className="text-gray-400 text-sm">
                        You've been inactive for a while. For your security, you'll be logged out automatically.
                    </p>
                </div>

                {/* Countdown Timer */}
                <div className="flex justify-center mb-6">
                    <div className="relative">
                        <svg className="w-24 h-24 transform -rotate-90">
                            <circle
                                cx="48"
                                cy="48"
                                r="40"
                                stroke="currentColor"
                                strokeWidth="6"
                                fill="transparent"
                                className="text-slate-700"
                            />
                            <circle
                                cx="48"
                                cy="48"
                                r="40"
                                stroke="currentColor"
                                strokeWidth="6"
                                fill="transparent"
                                strokeDasharray={251.2}
                                strokeDashoffset={251.2 - (countdown / 5) * 251.2}
                                className="text-amber-500 transition-all duration-1000 ease-linear"
                                strokeLinecap="round"
                            />
                        </svg>
                        <div className="absolute inset-0 flex items-center justify-center">
                            <span className="text-3xl font-bold text-white">{countdown}</span>
                        </div>
                    </div>
                </div>

                {/* Info text */}
                <p className="text-center text-sm text-gray-500 mb-6">
                    Logging out in <span className="text-amber-500 font-semibold">{countdown}</span> second{countdown !== 1 ? 's' : ''}...
                </p>

                {/* Action Buttons */}
                <div className="flex flex-col sm:flex-row gap-3">
                    <Button
                        type="button"
                        variant="ghost"
                        onClick={onLogout}
                        className="flex-1 flex items-center justify-center gap-2"
                    >
                        <LogOut size={18} />
                        Logout Now
                    </Button>
                    <Button
                        type="button"
                        onClick={onStayLoggedIn}
                        className="flex-1 bg-primary hover:bg-blue-600 flex items-center justify-center gap-2"
                    >
                        <RefreshCw size={18} />
                        Stay Logged In
                    </Button>
                </div>
            </div>
        </div>
    );
};
