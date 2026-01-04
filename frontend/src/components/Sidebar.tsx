import { LayoutDashboard, Wallet, PiggyBank, Receipt, LogOut, Menu, X } from 'lucide-react';
import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { clsx } from 'clsx';
import { authService } from '../services/authService';

export const Sidebar = () => {
    const location = useLocation();
    const [isOpen, setIsOpen] = useState(false);

    const navigation = [
        { name: 'Dashboard', href: '/', icon: LayoutDashboard },
        { name: 'Expenses', href: '/expenses', icon: Receipt },
        { name: 'Budgets', href: '/budgets', icon: Wallet },
        { name: 'Saving Goals', href: '/goals', icon: PiggyBank },
    ];

    const toggleSidebar = () => setIsOpen(!isOpen);

    return (
        <>
            {/* Mobile Menu Button */}
            <button
                onClick={toggleSidebar}
                className="lg:hidden fixed top-4 left-4 z-50 p-2 bg-slate-800 rounded-lg text-white border border-slate-700"
            >
                {isOpen ? <X size={24} /> : <Menu size={24} />}
            </button>

            {/* Overlay */}
            {isOpen && (
                <div
                    className="fixed inset-0 bg-black/50 backdrop-blur-sm z-40 lg:hidden"
                    onClick={() => setIsOpen(false)}
                />
            )}

            {/* Sidebar */}
            <aside className={clsx(
                "fixed inset-y-0 left-0 z-40 w-64 bg-slate-900 border-r border-slate-800 transition-transform duration-300 lg:translate-x-0 lg:static lg:block",
                isOpen ? "translate-x-0" : "-translate-x-full"
            )}>
                <div className="flex flex-col h-full bg-slate-900">
                    {/* Logo */}
                    <div className="h-16 flex items-center px-6 border-b border-slate-800">
                        <span className="text-xl font-bold bg-gradient-to-r from-primary to-accent bg-clip-text text-transparent">
                            Expensify
                        </span>
                    </div>

                    {/* Nav Items */}
                    <nav className="flex-1 px-4 py-8 space-y-2">
                        {navigation.map((item) => {
                            const isActive = location.pathname === item.href;
                            const Icon = item.icon;
                            return (
                                <Link
                                    key={item.name}
                                    to={item.href}
                                    onClick={() => setIsOpen(false)}
                                    className={clsx(
                                        "flex items-center px-4 py-3 rounded-lg transition-all duration-200 group",
                                        isActive
                                            ? "bg-primary/10 text-primary"
                                            : "text-gray-400 hover:text-white hover:bg-slate-800"
                                    )}
                                >
                                    <Icon className={clsx("mr-3 h-5 w-5", isActive ? "text-primary" : "text-gray-500 group-hover:text-white")} />
                                    <span className="font-medium">{item.name}</span>
                                    {isActive && (
                                        <div className="ml-auto w-1.5 h-1.5 rounded-full bg-primary shadow-[0_0_8px_rgba(59,130,246,0.5)]" />
                                    )}
                                </Link>
                            );
                        })}
                    </nav>

                    {/* User Profile / Logout */}
                    <div className="p-4 border-t border-slate-800">
                        <button
                            onClick={() => authService.logout()}
                            className="flex items-center w-full px-4 py-3 text-gray-400 hover:text-danger hover:bg-danger/10 rounded-lg transition-colors"
                        >
                            <LogOut className="mr-3 h-5 w-5" />
                            <span className="font-medium">Sign Out</span>
                        </button>
                    </div>
                </div>
            </aside>
        </>
    );
};
