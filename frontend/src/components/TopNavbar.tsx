import { LayoutDashboard, Wallet, PiggyBank, Receipt, LogOut, User, DollarSign, Settings } from 'lucide-react';
import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { clsx } from 'clsx';
import { authService } from '../services/authService';

export const TopNavbar = () => {
    const location = useLocation();
    const [isProfileOpen, setIsProfileOpen] = useState(false);

    const navigation = [
        { name: 'Dashboard', href: '/', icon: LayoutDashboard },
        { name: 'Expenses', href: '/expenses', icon: Receipt },
        { name: 'Income', href: '/income', icon: DollarSign },
        { name: 'Budgets', href: '/budgets', icon: Wallet },
        { name: 'Saving Goals', href: '/goals', icon: PiggyBank },
    ];

    return (
        <header className="desktop-nav">
            {/* Desktop Navbar */}
            <nav className="glass-card-elevated border-b border-white/5 rounded-none !overflow-visible">
                <div className="container mx-auto px-6 lg:px-12">
                    <div className="flex items-center justify-between h-16 lg:h-20">
                        {/* Logo */}
                        <Link to="/" className="flex items-center space-x-3 group">
                            <div className="relative w-10 h-10">
                                <div className="absolute inset-0 bg-primary rounded-xl blur-lg opacity-40 group-hover:opacity-60 transition-opacity" />
                                <div className="relative w-full h-full rounded-xl bg-gradient-to-br from-primary via-indigo-500 to-purple-600 flex items-center justify-center shadow-lg group-hover:scale-105 transition-transform duration-300">
                                    <Wallet className="w-5 h-5 text-white" />
                                </div>
                            </div>
                            <span className="text-xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-white via-primary-light to-accent-light">
                                Expensify
                            </span>
                        </Link>

                        {/* Desktop Navigation */}
                        <div className="flex items-center p-1.5 rounded-2xl bg-slate-950/30 border border-white/5 backdrop-blur-md">
                            {navigation.map((item) => {
                                const isActive = location.pathname === item.href;
                                const Icon = item.icon;
                                return (
                                    <Link
                                        key={item.name}
                                        to={item.href}
                                        className={clsx(
                                            "relative flex items-center px-4 lg:px-5 py-2.5 rounded-xl transition-all duration-300",
                                            isActive
                                                ? "text-white shadow-lg"
                                                : "text-gray-400 hover:text-white hover:bg-white/5"
                                        )}
                                    >
                                        {isActive && (
                                            <div className="absolute inset-0 rounded-xl bg-gradient-to-r from-primary to-indigo-600 opacity-100 shadow-[0_2px_20px_rgba(99,102,241,0.4)]" />
                                        )}
                                        <Icon className={clsx(
                                            "relative mr-2 h-4 w-4 transition-transform duration-300",
                                            isActive ? "scale-110" : ""
                                        )} />
                                        <span className="relative font-medium text-sm">{item.name}</span>
                                    </Link>
                                );
                            })}
                        </div>

                        {/* Profile Dropdown */}
                        <div className="relative">
                            <button
                                onClick={() => setIsProfileOpen(!isProfileOpen)}
                                className="flex items-center gap-3 p-1.5 rounded-full hover:bg-white/5 transition-colors border border-transparent hover:border-white/10"
                            >
                                <div className="w-10 h-10 rounded-full bg-gradient-to-br from-slate-700 to-slate-900 border border-white/10 flex items-center justify-center shadow-inner relative overflow-hidden group">
                                    <User className="w-5 h-5 text-gray-300 group-hover:text-white transition-colors" />
                                    <div className="absolute inset-0 bg-gradient-to-tr from-transparent via-white/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
                                </div>
                            </button>

                            {/* Dropdown Menu */}
                            {isProfileOpen && (
                                <>
                                    <div
                                        className="fixed inset-0 z-30"
                                        onClick={() => setIsProfileOpen(false)}
                                    />
                                    <div className="absolute right-0 mt-3 w-64 glass-card-elevated border border-white/10 shadow-[0_10px_40px_rgba(0,0,0,0.5)] transform transition-all duration-200 z-40 p-2">
                                        <div className="p-4 border-b border-white/5 mb-2">
                                            <p className="text-sm font-medium text-gray-400">Signed in as</p>
                                            <p className="text-white font-semibold truncate mt-0.5">{authService.getCurrentUser()?.email}</p>
                                        </div>
                                        <Link
                                            to="/settings"
                                            onClick={() => setIsProfileOpen(false)}
                                            className="w-full flex items-center gap-3 px-4 py-3 text-sm text-gray-400 hover:text-white hover:bg-white/5 rounded-xl transition-all group"
                                        >
                                            <Settings className="w-4 h-4 text-gray-500 group-hover:text-primary transition-colors" />
                                            Settings
                                        </Link>
                                        <button
                                            onClick={() => {
                                                authService.logout();
                                                setIsProfileOpen(false);
                                            }}
                                            className="w-full flex items-center gap-3 px-4 py-3 text-sm text-gray-400 hover:text-white hover:bg-white/5 rounded-xl transition-all group"
                                        >
                                            <LogOut className="w-4 h-4 text-gray-500 group-hover:text-rose-500 transition-colors" />
                                            Sign out
                                        </button>
                                    </div>
                                </>
                            )}
                        </div>
                    </div>
                </div>
            </nav>
        </header>
    );
};
