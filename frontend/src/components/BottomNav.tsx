import { LayoutDashboard, Wallet, PiggyBank, Receipt, DollarSign } from 'lucide-react';
import { Link, useLocation } from 'react-router-dom';
import { clsx } from 'clsx';

export const BottomNav = () => {
    const location = useLocation();

    const navigation = [
        { name: 'Home', href: '/', icon: LayoutDashboard },
        { name: 'Expenses', href: '/expenses', icon: Receipt },
        { name: 'Income', href: '/income', icon: DollarSign },
        { name: 'Budgets', href: '/budgets', icon: Wallet },
        { name: 'Goals', href: '/goals', icon: PiggyBank },
    ];

    return (
        <nav className="bottom-nav md:hidden">
            {navigation.map((item) => {
                const isActive = location.pathname === item.href;
                const Icon = item.icon;
                return (
                    <Link
                        key={item.name}
                        to={item.href}
                        className={clsx(
                            "bottom-nav-item",
                            isActive && "active"
                        )}
                    >
                        <Icon className={clsx(
                            "w-5 h-5 transition-transform",
                            isActive && "scale-110"
                        )} />
                        <span>{item.name}</span>
                    </Link>
                );
            })}
        </nav>
    );
};
