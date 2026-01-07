import { forwardRef, type ButtonHTMLAttributes, type ReactNode } from 'react';
import { clsx } from 'clsx';
import { Loader2 } from 'lucide-react';

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
    variant?: 'primary' | 'secondary' | 'outline' | 'ghost' | 'danger';
    size?: 'sm' | 'md' | 'lg';
    loading?: boolean;
    icon?: ReactNode;
    fullWidth?: boolean;
    children: ReactNode;
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(({
    variant = 'primary',
    size = 'md',
    loading = false,
    icon,
    fullWidth = false,
    className,
    disabled,
    children,
    ...props
}, ref) => {
    const baseStyles = clsx(
        "relative inline-flex items-center justify-center font-semibold",
        "transition-all duration-200 rounded-xl",
        "active:scale-[0.97]",
        // Touch-friendly sizing (44px+ height)
        "min-h-touch",
        // Full width on mobile by default, auto on desktop
        fullWidth ? "w-full" : "w-full md:w-auto",
        disabled && "opacity-50 cursor-not-allowed"
    );

    const variants = {
        primary: clsx(
            "bg-gradient-to-r from-primary to-indigo-600 text-white",
            "shadow-lg shadow-primary/25",
            "md:hover:shadow-xl md:hover:shadow-primary/30",
            "md:hover:-translate-y-0.5"
        ),
        secondary: clsx(
            "bg-gradient-to-r from-secondary to-teal-600 text-white",
            "shadow-lg shadow-secondary/25",
            "md:hover:shadow-xl md:hover:shadow-secondary/30"
        ),
        outline: clsx(
            "bg-transparent border border-white/10 text-white",
            "md:hover:bg-white/5 md:hover:border-white/20"
        ),
        ghost: clsx(
            "bg-transparent text-gray-400",
            "md:hover:text-white md:hover:bg-white/5"
        ),
        danger: clsx(
            "bg-gradient-to-r from-danger to-red-600 text-white",
            "shadow-lg shadow-danger/25",
            "md:hover:shadow-xl md:hover:shadow-danger/30"
        ),
    };

    const sizes = {
        sm: "px-4 py-2 text-sm min-h-[40px]",
        md: "px-5 py-3 text-sm",
        lg: "px-6 py-4 text-base",
    };

    return (
        <button
            ref={ref}
            className={clsx(baseStyles, variants[variant], sizes[size], className)}
            disabled={disabled || loading}
            {...props}
        >
            {loading ? (
                <Loader2 className="w-5 h-5 animate-spin" />
            ) : (
                <>
                    {icon && <span className="mr-2">{icon}</span>}
                    {children}
                </>
            )}
        </button>
    );
});

Button.displayName = 'Button';
