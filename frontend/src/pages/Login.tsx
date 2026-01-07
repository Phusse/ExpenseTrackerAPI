import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Lock, ArrowRight, Wallet } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { authService } from '../services/authService';
import { useToast } from '../context/ToastContext';

export const Login = () => {
    const navigate = useNavigate();
    const toast = useToast();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const [formData, setFormData] = useState({
        email: '',
        password: ''
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setError('');

        try {
            const result = await authService.login(formData.email, formData.password);

            if (result.success) {
                toast.success('Welcome back!', 'You have successfully signed in.');
                navigate('/');
            } else {
                const errorMessage = result.message || 'Login failed. Please try again.';
                setError(errorMessage);
            }
        } catch (err: any) {
            let errorMessage = 'An unexpected error occurred.';

            if (err.response?.data?.message) {
                errorMessage = err.response.data.message;
            } else if (err.apiError?.message) {
                errorMessage = err.apiError.message;
            } else if (err.code === 'ERR_NETWORK' || err.message === 'Network Error') {
                errorMessage = 'Unable to connect to the server.';
                toast.error('Connection Error', errorMessage);
            } else if (err.message) {
                errorMessage = err.message;
            }

            setError(errorMessage);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex flex-col justify-center bg-background px-4 py-8">
            {/* Background */}
            <div className="fixed top-[-20%] left-[-20%] w-[60%] h-[60%] rounded-full bg-primary/10 blur-[100px] pointer-events-none" />
            <div className="fixed bottom-[-20%] right-[-20%] w-[60%] h-[60%] rounded-full bg-accent/10 blur-[100px] pointer-events-none" />

            <div className="w-full max-w-md mx-auto relative z-10">
                {/* Logo */}
                <div className="text-center mb-8">
                    <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-gradient-to-tr from-primary to-accent mb-4">
                        <Wallet className="w-7 h-7 text-white" />
                    </div>
                    <h1 className="text-2xl font-bold text-white">Welcome Back</h1>
                    <p className="text-gray-400 text-sm mt-1">Sign in to manage your finances</p>
                </div>

                {/* Form */}
                <div className="glass-card p-6 md:p-8">
                    <form onSubmit={handleSubmit} className="space-y-5">
                        <Input
                            label="Email Address"
                            type="email"
                            required
                            placeholder="you@example.com"
                            icon={<Mail className="w-5 h-5" />}
                            value={formData.email}
                            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                        />

                        <Input
                            label="Password"
                            type="password"
                            required
                            placeholder="••••••••"
                            icon={<Lock className="w-5 h-5" />}
                            value={formData.password}
                            onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                        />

                        {error && (
                            <div className="p-3 bg-danger/10 border border-danger/20 rounded-xl text-danger text-sm text-center">
                                {error}
                            </div>
                        )}

                        <Button
                            type="submit"
                            loading={loading}
                            className="w-full"
                        >
                            {!loading && (
                                <>
                                    Sign In
                                    <ArrowRight className="w-4 h-4 ml-2" />
                                </>
                            )}
                        </Button>
                    </form>

                    {/* Forgot Password */}
                    <div className="mt-4 text-center">
                        <Link to="/forgot-password" className="text-sm text-gray-400 hover:text-primary">
                            Forgot your password?
                        </Link>
                    </div>
                </div>

                {/* Footer */}
                <p className="mt-6 text-center text-sm text-gray-400">
                    Don't have an account?{' '}
                    <Link to="/signup" className="text-primary hover:text-blue-400 font-medium">
                        Create account
                    </Link>
                </p>
            </div>
        </div>
    );
};
