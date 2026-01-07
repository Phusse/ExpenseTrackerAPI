import { Outlet, Link } from 'react-router-dom';
import { Settings } from 'lucide-react';
import { TopNavbar } from '../components/TopNavbar';
import { BottomNav } from '../components/BottomNav';
import { SessionTimeoutModal } from '../components/SessionTimeoutModal';
import { useInactivityLogout } from '../hooks/useInactivityLogout';
import { useApiErrorHandler } from '../hooks/useApiErrorHandler';

export const Layout = () => {
    const { showWarning, countdown, stayLoggedIn, logout } = useInactivityLogout();

    // Connect API errors to toast notifications
    useApiErrorHandler();

    return (
        <div className="min-h-screen bg-background text-text-main flex flex-col">
            {/* Desktop Navigation - Hidden on mobile */}
            <TopNavbar />

            {/* Mobile Header - Shown only on mobile */}
            <header className="mobile-header md:hidden">
                <div className="flex items-center gap-2">
                    <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-primary via-indigo-500 to-purple-600 flex items-center justify-center">
                        <svg className="w-4 h-4 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                        </svg>
                    </div>
                    <span className="text-base font-bold text-white">Expensify</span>
                </div>
                <Link
                    to="/settings"
                    className="w-10 h-10 rounded-xl flex items-center justify-center text-gray-400 hover:text-white hover:bg-white/10 active:scale-95 transition-all"
                >
                    <Settings className="w-5 h-5" />
                </Link>
            </header>

            {/* Main Content - with padding for bottom nav on mobile */}
            <main className="flex-1 relative w-full overflow-y-auto pb-20 md:pb-0">
                <div className="px-4 py-4 md:container md:mx-auto md:px-6 md:py-6 lg:px-12 lg:py-8">
                    <Outlet />
                </div>
            </main>

            {/* Mobile Bottom Navigation */}
            <BottomNav />

            {/* Session Timeout Warning Modal */}
            <SessionTimeoutModal
                isOpen={showWarning}
                countdown={countdown}
                onStayLoggedIn={stayLoggedIn}
                onLogout={logout}
            />
        </div>
    );
};
