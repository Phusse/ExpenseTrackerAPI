import { LayoutDashboard, Wallet, PiggyBank, Receipt, LogOut, Menu, X, User } from 'lucide-react';
import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { clsx } from 'clsx';
import { authService } from '../services/authService';

export const TopNavbar = () => {
    const location = useLocation();
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
    const [isProfileOpen, setIsProfileOpen] = useState(false);

    const navigation = [
        { name: 'Dashboard', href: '/', icon: LayoutDashboard },
        { name: 'Expenses', href: '/expenses', icon: Receipt },
        { name: 'Budgets', href: '/budgets', icon: Wallet },
        { name: 'Saving Goals', href: '/goals', icon: PiggyBank },
    ];

    return (
        <header className="sticky top-0 z-50 w-full">
            {/* Main Navbar */}
            <nav className="bg-slate-900/95 backdrop-blur-xl border-b border-slate-800/80 shadow-lg shadow-slate-950/20">
                <div className="container mx-auto px-6 lg:px-12">
                    <div className="flex items-center justify-between h-16">
                        {/* Logo */}
                        <Link to="/" className="flex items-center space-x-2 group">
                            <div className="w-9 h-9 rounded-xl bg-gradient-to-br from-primary via-accent to-primary flex items-center justify-center shadow-lg shadow-primary/25 group-hover:shadow-primary/40 transition-shadow">
                                <Wallet className="w-5 h-5 text-white" />
                            </div>
                            <span className="text-xl font-bold bg-gradient-to-r from-white via-primary to-accent bg-clip-text text-transparent">
                                Expensify
                            </span>
                        </Link>

                        {/* Desktop Navigation */}
                        <div className="hidden md:flex items-center space-x-1">
                            {navigation.map((item) => {
                                const isActive = location.pathname === item.href;
                                const Icon = item.icon;
                                return (
                                    <Link
                                        key={item.name}
                                        to={item.href}
                                        className={clsx(
                                            "relative flex items-center px-4 py-2 rounded-xl transition-all duration-300 group",
                                            isActive
                                                ? "text-primary"
                                                : "text-gray-400 hover:text-white"
                                        )}
                                    >
                                        {/* Background glow for active */}
                                        {isActive && (
                                            <div className="absolute inset-0 bg-primary/10 rounded-xl border border-primary/20" />
                                        )}

                                        {/* Hover effect */}
                                        <div className={clsx(
                                            "absolute inset-0 rounded-xl opacity-0 group-hover:opacity-100 transition-opacity duration-300",
                                            !isActive && "bg-slate-800/50"
                                        )} />

                                        <Icon className={clsx(
                                            "relative mr-2 h-4 w-4 transition-colors",
                                            isActive ? "text-primary" : "text-gray-500 group-hover:text-white"
                                        )} />
                                        <span className="relative font-medium text-sm">{item.name}</span>

                                        {/* Active indicator dot */}
                                        {isActive && (
                                            <div className="absolute -bottom-3 left-1/2 -translate-x-1/2 w-1 h-1 rounded-full bg-primary shadow-[0_0_8px_rgba(59,130,246,0.8)]" />
                                        )}
                                    </Link>
                                );
                            })}
                        </div>

                        {/* Right Section - Profile & Mobile Menu */}
                        <div className="flex items-center space-x-3">
                            {/* Profile Dropdown */}
                            <div className="relative">
                                <button
                                    onClick={() => setIsProfileOpen(!isProfileOpen)}
                                    className="hidden md:flex items-center space-x-2 px-3 py-2 rounded-xl bg-slate-800/50 border border-slate-700/50 hover:bg-slate-800 hover:border-slate-600 transition-all duration-300 group"
                                >
                                    <div className="w-7 h-7 rounded-lg bg-gradient-to-br from-primary/20 to-accent/20 flex items-center justify-center border border-primary/20">
                                        <User className="w-4 h-4 text-primary" />
                                    </div>
                                    <span className="text-sm text-gray-300 group-hover:text-white transition-colors">Account</span>
                                </button>

                                {/* Profile Dropdown Menu */}
                                {isProfileOpen && (
                                    <>
                                        <div
                                            className="fixed inset-0 z-40"
                                            onClick={() => setIsProfileOpen(false)}
                                        />
                                        <div className="absolute right-0 mt-2 w-48 py-2 bg-slate-800 border border-slate-700 rounded-xl shadow-xl shadow-slate-950/50 z-50 animate-in fade-in slide-in-from-top-2 duration-200">
                                            <button
                                                onClick={() => {
                                                    authService.logout();
                                                    setIsProfileOpen(false);
                                                }}
                                                className="flex items-center w-full px-4 py-2.5 text-gray-300 hover:text-danger hover:bg-danger/10 transition-colors"
                                            >
                                                <LogOut className="mr-3 h-4 w-4" />
                                                <span className="text-sm font-medium">Sign Out</span>
                                            </button>
                                        </div>
                                    </>
                                )}
                            </div>

                            {/* Mobile Menu Button */}
                            <button
                                onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                                className="md:hidden p-2.5 bg-slate-800/50 rounded-xl text-gray-400 hover:text-white border border-slate-700/50 hover:border-slate-600 transition-all duration-300"
                            >
                                {isMobileMenuOpen ? <X size={20} /> : <Menu size={20} />}
                            </button>
                        </div>
                    </div>
                </div>

                {/* Mobile Navigation Menu */}
                <div className={clsx(
                    "md:hidden overflow-hidden transition-all duration-300 ease-in-out",
                    isMobileMenuOpen ? "max-h-96 border-t border-slate-800/80" : "max-h-0"
                )}>
                    <div className="container mx-auto px-4 py-4 space-y-1">
                        {navigation.map((item) => {
                            const isActive = location.pathname === item.href;
                            const Icon = item.icon;
                            return (
                                <Link
                                    key={item.name}
                                    to={item.href}
                                    onClick={() => setIsMobileMenuOpen(false)}
                                    className={clsx(
                                        "flex items-center px-4 py-3 rounded-xl transition-all duration-200",
                                        isActive
                                            ? "bg-primary/10 text-primary border border-primary/20"
                                            : "text-gray-400 hover:text-white hover:bg-slate-800/50"
                                    )}
                                >
                                    <Icon className={clsx("mr-3 h-5 w-5", isActive ? "text-primary" : "text-gray-500")} />
                                    <span className="font-medium">{item.name}</span>
                                    {isActive && (
                                        <div className="ml-auto w-1.5 h-1.5 rounded-full bg-primary shadow-[0_0_8px_rgba(59,130,246,0.5)]" />
                                    )}
                                </Link>
                            );
                        })}

                        {/* Mobile Sign Out */}
                        <button
                            onClick={() => {
                                authService.logout();
                                setIsMobileMenuOpen(false);
                            }}
                            className="flex items-center w-full px-4 py-3 text-gray-400 hover:text-danger hover:bg-danger/10 rounded-xl transition-colors mt-2 border-t border-slate-800 pt-4"
                        >
                            <LogOut className="mr-3 h-5 w-5" />
                            <span className="font-medium">Sign Out</span>
                        </button>
                    </div>
                </div>
            </nav>
        </header>
    );
};
