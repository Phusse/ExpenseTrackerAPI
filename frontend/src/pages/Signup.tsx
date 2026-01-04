import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Lock, User, ArrowRight, Wallet } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { authService } from '../services/authService';

export const Signup = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: ''
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        setLoading(true);
        setError('');

        try {
            await authService.register(formData.username, formData.email, formData.password);
            // Automatically login after register or redirect to login
            navigate('/login');
        } catch (err: any) {
            setError(err.response?.data?.message || 'Registration failed');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-background p-4 relative overflow-hidden">
            {/* Background decoration */}
            <div className="absolute top-[-10%] right-[-10%] w-[500px] h-[500px] rounded-full bg-secondary/10 blur-[100px]" />
            <div className="absolute bottom-[-10%] left-[-10%] w-[500px] h-[500px] rounded-full bg-primary/10 blur-[100px]" />

            <div className="w-full max-w-md relative z-10 bg-surface/50 backdrop-blur-xl border border-white/5 rounded-2xl p-8 shadow-2xl">
                <div className="text-center mb-8">
                    <div className="inline-flex items-center justify-center w-12 h-12 rounded-xl bg-gradient-to-tr from-secondary to-primary mb-4">
                        <Wallet className="w-6 h-6 text-white" />
                    </div>
                    <h1 className="text-2xl font-bold text-white mb-2">Create Account</h1>
                    <p className="text-gray-400">Start your financial journey today</p>
                </div>

                <form onSubmit={handleSubmit} className="space-y-6">
                    <Input
                        label="Full Name"
                        type="text"
                        required
                        placeholder="John Doe"
                        icon={<User className="w-5 h-5" />}
                        value={formData.username}
                        onChange={(e) => setFormData({ ...formData, username: e.target.value })}
                    />

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

                    <Input
                        label="Confirm Password"
                        type="password"
                        required
                        placeholder="••••••••"
                        icon={<Lock className="w-5 h-5" />}
                        value={formData.confirmPassword}
                        onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                    />

                    {error && (
                        <div className="p-3 bg-danger/10 border border-danger/20 rounded-lg text-danger text-sm text-center">
                            {error}
                        </div>
                    )}

                    <Button
                        type="submit"
                        className="w-full relative group overflow-hidden"
                        disabled={loading}
                    >
                        <span className="relative z-10 flex items-center justify-center">
                            {loading ? 'Creating Account...' : 'Create Account'}
                            {!loading && <ArrowRight className="w-4 h-4 ml-2 group-hover:translate-x-1 transition-transform" />}
                        </span>
                        <div className="absolute inset-0 bg-gradient-to-r from-emerald-600 to-teal-600 opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
                    </Button>
                </form>

                <p className="mt-8 text-center text-sm text-gray-400">
                    Already have an account?{' '}
                    <Link to="/login" className="text-primary hover:text-blue-400 font-medium transition-colors">
                        Sign In
                    </Link>
                </p>
            </div>
        </div>
    );
};
