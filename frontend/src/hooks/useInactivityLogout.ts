import { useEffect, useRef, useCallback, useState } from 'react';
import { authService } from '../services/authService';

const INACTIVITY_TIMEOUT = 15 * 60 * 1000; // 15 minutes in milliseconds
const COUNTDOWN_DURATION = 5; // 5 seconds countdown

export const useInactivityLogout = () => {
    const [showWarning, setShowWarning] = useState(false);
    const [countdown, setCountdown] = useState(COUNTDOWN_DURATION);
    const inactivityTimeoutRef = useRef<number | null>(null);
    const countdownIntervalRef = useRef<number | null>(null);

    const logout = useCallback(() => {
        clearAllTimers();
        authService.logout();
    }, []);

    const clearAllTimers = useCallback(() => {
        if (inactivityTimeoutRef.current) {
            clearTimeout(inactivityTimeoutRef.current);
            inactivityTimeoutRef.current = null;
        }
        if (countdownIntervalRef.current) {
            clearInterval(countdownIntervalRef.current);
            countdownIntervalRef.current = null;
        }
    }, []);

    const startCountdown = useCallback(() => {
        setShowWarning(true);
        setCountdown(COUNTDOWN_DURATION);

        let timeLeft = COUNTDOWN_DURATION;

        countdownIntervalRef.current = window.setInterval(() => {
            timeLeft -= 1;
            setCountdown(timeLeft);

            if (timeLeft <= 0) {
                logout();
            }
        }, 1000);
    }, [logout]);

    const resetTimer = useCallback(() => {
        setShowWarning(false);
        setCountdown(COUNTDOWN_DURATION);
        clearAllTimers();

        // Start inactivity timer
        inactivityTimeoutRef.current = window.setTimeout(() => {
            startCountdown();
        }, INACTIVITY_TIMEOUT);
    }, [clearAllTimers, startCountdown]);

    const stayLoggedIn = useCallback(() => {
        resetTimer();
    }, [resetTimer]);

    useEffect(() => {
        // Activity events to track
        const events = ['mousedown', 'mousemove', 'keydown', 'scroll', 'touchstart', 'click'];

        // Start the timer initially
        resetTimer();

        // Reset timer on any activity (only if warning is not showing)
        const handleActivity = () => {
            if (!showWarning) {
                resetTimer();
            }
        };

        events.forEach(event => {
            document.addEventListener(event, handleActivity, { passive: true });
        });

        // Cleanup on unmount
        return () => {
            clearAllTimers();
            events.forEach(event => {
                document.removeEventListener(event, handleActivity);
            });
        };
    }, [resetTimer, clearAllTimers, showWarning]);

    return { showWarning, countdown, stayLoggedIn, logout };
};
