import React from 'react';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';

function cn(...inputs: ClassValue[]) {
    return twMerge(clsx(inputs));
}

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
    label?: string;
    error?: string;
    icon?: React.ReactNode;
}

export const Input = React.forwardRef<HTMLInputElement, InputProps>(
    ({ className, label, error, icon, ...props }, ref) => {
        return (
            <div className="w-full">
                {label && (
                    <label className="block text-sm font-medium text-gray-400 mb-1.5">
                        {label}
                    </label>
                )}
                <div className="relative">
                    <input
                        ref={ref}
                        className={cn(
                            'w-full bg-slate-800/50 border border-slate-700 rounded-lg px-4 py-2.5 text-white placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-primary/50 focus:border-primary transition-all duration-200',
                            icon && 'pl-10',
                            error && 'border-danger focus:ring-danger/50 focus:border-danger',
                            className
                        )}
                        {...props}
                    />
                    {icon && (
                        <div className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500">
                            {icon}
                        </div>
                    )}
                </div>
                {error && (
                    <p className="mt-1 text-sm text-danger">{error}</p>
                )}
            </div>
        );
    }
);

Input.displayName = 'Input';
