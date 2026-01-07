import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { userService, type UserSettings } from '../services/userService';
import { authService } from '../services/authService';

interface SettingsContextType {
    settings: UserSettings | null;
    loading: boolean;
    updateSetting: <K extends keyof UserSettings>(key: K, value: UserSettings[K]) => Promise<void>;
    refreshSettings: () => Promise<void>;
    formatCurrency: (amount: number) => string;
}

const defaultSettings: UserSettings = {
    notificationsEnabled: true,
    darkModeEnabled: true,
    currency: 'NGN',
    locale: 'en-US',
    budgetReminderDay: 1,
    weeklyReportEnabled: true
};

const currencySymbols: Record<string, string> = {
    NGN: '₦',
    USD: '$',
    EUR: '€',
    GBP: '£',
    GHS: '₵',
    KES: 'KSh',
    ZAR: 'R'
};

const SettingsContext = createContext<SettingsContextType | undefined>(undefined);

export const useSettings = () => {
    const context = useContext(SettingsContext);
    if (!context) {
        throw new Error('useSettings must be used within a SettingsProvider');
    }
    return context;
};

export const SettingsProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [settings, setSettings] = useState<UserSettings | null>(null);
    const [loading, setLoading] = useState(true);

    const fetchSettings = useCallback(async () => {
        if (!authService.isAuthenticated()) {
            setSettings(defaultSettings);
            setLoading(false);
            return;
        }

        try {
            const data = await userService.getSettings();
            setSettings(data);
            // Apply dark mode
            applyTheme(data.darkModeEnabled);
        } catch (error) {
            console.error('Failed to fetch settings', error);
            setSettings(defaultSettings);
        } finally {
            setLoading(false);
        }
    }, []);

    const applyTheme = (isDark: boolean) => {
        const root = document.documentElement;
        if (isDark) {
            root.classList.add('dark');
            root.classList.remove('light');
        } else {
            root.classList.add('light');
            root.classList.remove('dark');
        }
    };

    const updateSetting = async <K extends keyof UserSettings>(key: K, value: UserSettings[K]) => {
        try {
            const result = await userService.updateSettings({ [key]: value });
            if (result.success && result.data) {
                setSettings(result.data);

                // Apply theme immediately if dark mode changed
                if (key === 'darkModeEnabled') {
                    applyTheme(value as boolean);
                }
            }
        } catch (error) {
            console.error('Failed to update setting', error);
            throw error;
        }
    };

    const formatCurrency = useCallback((amount: number): string => {
        const currency = settings?.currency || 'NGN';
        const symbol = currencySymbols[currency] || currency;

        // Format number with commas
        const formatted = new Intl.NumberFormat('en-US', {
            minimumFractionDigits: 0,
            maximumFractionDigits: 2
        }).format(amount);

        return `${symbol}${formatted}`;
    }, [settings?.currency]);

    useEffect(() => {
        fetchSettings();
    }, [fetchSettings]);

    // Re-fetch when auth state changes
    useEffect(() => {
        const handleStorageChange = (e: StorageEvent) => {
            if (e.key === 'token') {
                fetchSettings();
            }
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, [fetchSettings]);

    return (
        <SettingsContext.Provider value={{
            settings,
            loading,
            updateSetting,
            refreshSettings: fetchSettings,
            formatCurrency
        }}>
            {children}
        </SettingsContext.Provider>
    );
};
