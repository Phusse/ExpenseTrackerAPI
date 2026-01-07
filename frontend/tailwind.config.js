/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    // Mobile-first breakpoints (Tailwind default but explicitly stated)
    screens: {
      'sm': '640px',   // Small tablets
      'md': '768px',   // Tablets
      'lg': '1024px',  // Laptops
      'xl': '1280px',  // Desktops
      '2xl': '1536px', // Large screens
    },
    extend: {
      colors: {
        // Premium dark palette with depth
        background: {
          DEFAULT: '#0a0e1a',
          deep: '#050810',
          elevated: '#0f1424',
        },
        surface: {
          DEFAULT: 'rgba(15, 23, 42, 0.6)',
          solid: '#111827',
          glass: 'rgba(17, 24, 39, 0.4)',
          hover: 'rgba(30, 41, 59, 0.5)',
        },
        // Rich accent colors
        primary: {
          DEFAULT: '#6366f1',
          light: '#818cf8',
          dark: '#4f46e5',
          glow: 'rgba(99, 102, 241, 0.4)',
        },
        secondary: {
          DEFAULT: '#10b981',
          light: '#34d399',
          dark: '#059669',
          glow: 'rgba(16, 185, 129, 0.4)',
        },
        accent: {
          DEFAULT: '#f472b6',
          light: '#f9a8d4',
          dark: '#db2777',
          glow: 'rgba(244, 114, 182, 0.4)',
        },
        warning: {
          DEFAULT: '#f59e0b',
          light: '#fbbf24',
          dark: '#d97706',
          glow: 'rgba(245, 158, 11, 0.4)',
        },
        danger: {
          DEFAULT: '#ef4444',
          light: '#f87171',
          dark: '#dc2626',
          glow: 'rgba(239, 68, 68, 0.4)',
        },
        text: {
          main: '#f8fafc',
          secondary: '#cbd5e1',
          muted: '#64748b',
        },
        border: {
          DEFAULT: 'rgba(148, 163, 184, 0.1)',
          light: 'rgba(148, 163, 184, 0.15)',
          glow: 'rgba(99, 102, 241, 0.3)',
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
      // Mobile-first spacing (touch-friendly)
      spacing: {
        'safe-top': 'env(safe-area-inset-top)',
        'safe-bottom': 'env(safe-area-inset-bottom)',
        'safe-left': 'env(safe-area-inset-left)',
        'safe-right': 'env(safe-area-inset-right)',
        'touch': '44px',  // Minimum touch target
        'touch-lg': '48px',
        'nav-height': '56px',
        'bottom-nav': '64px',
      },
      minHeight: {
        'touch': '44px',
        'touch-lg': '48px',
      },
      minWidth: {
        'touch': '44px',
        'touch-lg': '48px',
      },
      boxShadow: {
        'glass': '0 8px 32px rgba(0, 0, 0, 0.3), inset 0 1px 0 rgba(255, 255, 255, 0.05)',
        'glass-sm': '0 4px 16px rgba(0, 0, 0, 0.2), inset 0 1px 0 rgba(255, 255, 255, 0.03)',
        'glow-primary': '0 0 20px rgba(99, 102, 241, 0.3)',
        'glow-secondary': '0 0 20px rgba(16, 185, 129, 0.3)',
        'glow-accent': '0 0 20px rgba(244, 114, 182, 0.3)',
        'glow-warning': '0 0 20px rgba(245, 158, 11, 0.3)',
        'glow-danger': '0 0 20px rgba(239, 68, 68, 0.3)',
        'card': '0 4px 24px rgba(0, 0, 0, 0.4)',
        'elevated': '0 12px 40px rgba(0, 0, 0, 0.5)',
        'bottom-nav': '0 -4px 20px rgba(0, 0, 0, 0.3)',
      },
      backdropBlur: {
        'glass': '16px',
        'heavy': '24px',
      },
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
        'gradient-mesh': 'radial-gradient(at 40% 20%, rgba(99, 102, 241, 0.08) 0px, transparent 50%), radial-gradient(at 80% 0%, rgba(16, 185, 129, 0.06) 0px, transparent 50%), radial-gradient(at 0% 50%, rgba(244, 114, 182, 0.05) 0px, transparent 50%)',
        'gradient-shine': 'linear-gradient(135deg, rgba(255,255,255,0.1) 0%, rgba(255,255,255,0) 50%, rgba(255,255,255,0.05) 100%)',
      },
      animation: {
        'shimmer': 'shimmer 2s linear infinite',
        'float': 'float 6s ease-in-out infinite',
        'pulse-glow': 'pulse-glow 2s ease-in-out infinite',
        'slide-up': 'slide-up 0.3s ease-out',
        'slide-down': 'slide-down 0.3s ease-out',
        'fade-in': 'fade-in 0.2s ease-out',
      },
      keyframes: {
        shimmer: {
          '0%': { transform: 'translateX(-100%)' },
          '100%': { transform: 'translateX(100%)' },
        },
        float: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(-10px)' },
        },
        'pulse-glow': {
          '0%, 100%': { opacity: '1' },
          '50%': { opacity: '0.6' },
        },
        'slide-up': {
          '0%': { transform: 'translateY(100%)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
        'slide-down': {
          '0%': { transform: 'translateY(-100%)', opacity: '0' },
          '100%': { transform: 'translateY(0)', opacity: '1' },
        },
        'fade-in': {
          '0%': { opacity: '0' },
          '100%': { opacity: '1' },
        },
      },
      borderRadius: {
        '2xl': '1rem',
        '3xl': '1.5rem',
      },
    },
  },
  plugins: [],
}
