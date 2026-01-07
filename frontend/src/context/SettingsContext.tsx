import React, { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { userService, type UserSettings } from '../services/userService';
import { authService } from '../services/authService';

interface SettingsContextType {
    settings: UserSettings | null;
    loading: boolean;
    updateSetting: <K extends keyof UserSettings>(key: K, value: UserSettings[K]) => Promise<void>;
    refreshSettings: () => Promise<void>;
    formatCurrency: (amount: number) => string;
    convertAmount: (amount: number) => number;
    exchangeRates: Record<string, number> | null;
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
    ZAR: 'R',
    CAD: 'C$',
    AUD: 'A$',
    JPY: '¥',
    CNY: '¥',
    INR: '₹'
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
    const [exchangeRates, setExchangeRates] = useState<Record<string, number> | null>(null);

    const fetchExchangeRates = useCallback(async () => {
        try {
            const data = await userService.getExchangeRates('NGN');
            setExchangeRates(data.rates);
        } catch (error) {
            console.error('Failed to fetch exchange rates', error);
            // Fallback rates
            setExchangeRates({
                NGN: 1,
                USD: 0.00063,
                EUR: 0.00058,
                GBP: 0.00050,
                CAD: 0.00084,
                AUD: 0.00096
            });
        }
    }, []);

    const fetchSettings = useCallback(async () => {
        if (!authService.isAuthenticated()) {
            setSettings(defaultSettings);
            setLoading(false);
            return;
        }

        try {
            const data = await userService.getSettings();
            setSettings(data);
        } catch (error) {
            console.error('Failed to fetch settings', error);
            setSettings(defaultSettings);
        } finally {
            setLoading(false);
        }
    }, []);

    const updateSetting = async <K extends keyof UserSettings>(key: K, value: UserSettings[K]) => {
        try {
            const result = await userService.updateSettings({ [key]: value });
            if (result.success && result.data) {
                setSettings(result.data);
            }
        } catch (error) {
            console.error('Failed to update setting', error);
            throw error;
        }
    };

    // Convert amount from NGN to selected currency
    const convertAmount = useCallback((amount: number): number => {
        const currency = settings?.currency || 'NGN';
        if (currency === 'NGN' || !exchangeRates) {
            return amount;
        }
        const rate = exchangeRates[currency];
        if (!rate) return amount;
        return Math.round(amount * rate * 100) / 100;
    }, [settings?.currency, exchangeRates]);

    // Format amount with currency symbol
    const formatCurrency = useCallback((amount: number): string => {
        const currency = settings?.currency || 'NGN';
        const symbol = currencySymbols[currency] || currency;

        // Convert the amount if not NGN
        const convertedAmount = convertAmount(amount);

        // Format number with commas
        const formatted = new Intl.NumberFormat('en-US', {
            minimumFractionDigits: currency === 'NGN' ? 0 : 2,
            maximumFractionDigits: 2
        }).format(convertedAmount);

        return `${symbol}${formatted}`;
    }, [settings?.currency, convertAmount]);

    useEffect(() => {
        fetchSettings();
        fetchExchangeRates();
    }, [fetchSettings, fetchExchangeRates]);

    // Re-fetch when auth state changes
    useEffect(() => {
        const handleStorageChange = (e: StorageEvent) => {
            if (e.key === 'token') {
                fetchSettings();
                fetchExchangeRates();
            }
        };
        window.addEventListener('storage', handleStorageChange);
        return () => window.removeEventListener('storage', handleStorageChange);
    }, [fetchSettings, fetchExchangeRates]);

    return (
        <SettingsContext.Provider value={{
            settings,
            loading,
            updateSetting,
            refreshSettings: fetchSettings,
            formatCurrency,
            convertAmount,
            exchangeRates
        }}>
            {children}
        </SettingsContext.Provider>
    );
};
