import { forwardRef, type InputHTMLAttributes, type ReactNode } from 'react';
import { clsx } from 'clsx';

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
    label?: string;
    error?: string;
    icon?: ReactNode;
    rightIcon?: ReactNode;
}

export const Input = forwardRef<HTMLInputElement, InputProps>(({
    label,
    error,
    icon,
    rightIcon,
    className,
    ...props
}, ref) => {
    return (
        <div className="w-full">
            {label && (
                <label className="block text-sm font-medium text-gray-300 mb-2">
                    {label}
                </label>
            )}
            <div className="relative">
                {icon && (
                    <div className="absolute left-4 top-1/2 -translate-y-1/2 text-gray-500">
                        {icon}
                    </div>
                )}
                <input
                    ref={ref}
                    className={clsx(
                        "input-glass",
                        // Mobile-optimized: 48px height, 16px font (prevents iOS zoom)
                        "h-12 text-base",
                        // Desktop: slightly smaller
                        "md:h-11 md:text-sm",
                        icon && "pl-12",
                        rightIcon && "pr-12",
                        error && "border-danger focus:border-danger focus:ring-danger/20",
                        className
                    )}
                    {...props}
                />
                {rightIcon && (
                    <div className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-500">
                        {rightIcon}
                    </div>
                )}
            </div>
            {error && (
                <p className="mt-2 text-sm text-danger">{error}</p>
            )}
        </div>
    );
});

Input.displayName = 'Input';
