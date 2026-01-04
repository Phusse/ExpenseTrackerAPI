import { Outlet } from 'react-router-dom';
import { TopNavbar } from '../components/TopNavbar';
import { SessionTimeoutModal } from '../components/SessionTimeoutModal';
import { useInactivityLogout } from '../hooks/useInactivityLogout';

export const Layout = () => {
    const { showWarning, countdown, stayLoggedIn, logout } = useInactivityLogout();

    return (
        <div className="min-h-screen bg-background text-text-main flex flex-col">
            <TopNavbar />
            <main className="flex-1 relative w-full overflow-y-auto">
                <div className="container mx-auto px-6 py-8 lg:px-12">
                    <Outlet />
                </div>
            </main>

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
