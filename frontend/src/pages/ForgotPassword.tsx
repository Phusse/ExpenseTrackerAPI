import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Lock, ArrowRight, Wallet, ChevronLeft, Shield, Check } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { authService, type SecurityQuestionAnswer, type UserSecurityQuestion } from '../services/authService';

export const ForgotPassword = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [step, setStep] = useState(1); // 1 = Email, 2 = Security Questions, 3 = New Password, 4 = Success

    const [email, setEmail] = useState('');
    const [questions, setQuestions] = useState<UserSecurityQuestion[]>([]);
    const [answers, setAnswers] = useState<SecurityQuestionAnswer[]>([]);
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');

    const handleEmailSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const result = await authService.forgotPassword(email);

            if (result.success && result.data) {
                setQuestions(result.data.questions);
                setAnswers(result.data.questions.map(q => ({ questionId: q.questionId, answer: '' })));
                setStep(2);
            } else {
                setError(result.message || 'No account found with this email.');
            }
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to find account.');
        } finally {
            setLoading(false);
        }
    };

    const handleAnswersSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        // Validate all answers are filled
        for (const answer of answers) {
            if (!answer.answer || answer.answer.trim().length < 2) {
                setError('Please provide all answers (at least 2 characters each).');
                return;
            }
        }

        setStep(3);
    };

    const handleResetSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        if (newPassword !== confirmPassword) {
            setError('Passwords do not match.');
            return;
        }

        if (newPassword.length < 6) {
            setError('Password must be at least 6 characters.');
            return;
        }

        setLoading(true);

        try {
            const result = await authService.resetPassword(email, answers, newPassword, confirmPassword);

            if (result.success) {
                setStep(4);
            } else {
                setError(result.message || 'Failed to reset password.');
            }
        } catch (err: any) {
            setError(err.response?.data?.message || 'Failed to reset password. Please check your answers.');
        } finally {
            setLoading(false);
        }
    };

    const updateAnswer = (questionId: number, answer: string) => {
        setAnswers(prev => prev.map(a => a.questionId === questionId ? { ...a, answer } : a));
    };

    return (
        <div className="min-h-screen flex flex-col justify-center bg-background px-4 py-8">
            {/* Background */}
            <div className="fixed top-[-20%] left-[-20%] w-[60%] h-[60%] rounded-full bg-primary/10 blur-[100px] pointer-events-none" />
            <div className="fixed bottom-[-20%] right-[-20%] w-[60%] h-[60%] rounded-full bg-secondary/10 blur-[100px] pointer-events-none" />

            <div className="w-full max-w-md mx-auto relative z-10">
                {/* Logo */}
                <div className="text-center mb-8">
                    <div className="inline-flex items-center justify-center w-14 h-14 rounded-2xl bg-gradient-to-tr from-primary to-secondary mb-4">
                        {step === 4 ? (
                            <Check className="w-7 h-7 text-white" />
                        ) : step >= 2 ? (
                            <Shield className="w-7 h-7 text-white" />
                        ) : (
                            <Wallet className="w-7 h-7 text-white" />
                        )}
                    </div>
                    <h1 className="text-2xl font-bold text-white">
                        {step === 1 && 'Forgot Password'}
                        {step === 2 && 'Security Questions'}
                        {step === 3 && 'New Password'}
                        {step === 4 && 'Password Reset!'}
                    </h1>
                    <p className="text-gray-400 text-sm mt-1">
                        {step === 1 && 'Enter your email to recover your account'}
                        {step === 2 && 'Answer your security questions'}
                        {step === 3 && 'Create a new password'}
                        {step === 4 && 'You can now sign in with your new password'}
                    </p>
                </div>

                {/* Progress */}
                {step < 4 && (
                    <div className="flex gap-2 mb-6">
                        <div className={`h-1 flex-1 rounded-full ${step >= 1 ? 'bg-primary' : 'bg-slate-700'}`} />
                        <div className={`h-1 flex-1 rounded-full ${step >= 2 ? 'bg-primary' : 'bg-slate-700'}`} />
                        <div className={`h-1 flex-1 rounded-full ${step >= 3 ? 'bg-primary' : 'bg-slate-700'}`} />
                    </div>
                )}

                {/* Form */}
                <div className="glass-card p-6 md:p-8">
                    {step === 1 && (
                        <form onSubmit={handleEmailSubmit} className="space-y-4">
                            <Input
                                label="Email Address"
                                type="email"
                                required
                                placeholder="you@example.com"
                                icon={<Mail className="w-5 h-5" />}
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                            />

                            {error && (
                                <div className="p-3 bg-danger/10 border border-danger/20 rounded-xl text-danger text-sm text-center">
                                    {error}
                                </div>
                            )}

                            <Button type="submit" loading={loading} className="w-full">
                                {!loading && (
                                    <>
                                        Continue
                                        <ArrowRight className="w-4 h-4 ml-2" />
                                    </>
                                )}
                            </Button>
                        </form>
                    )}

                    {step === 2 && (
                        <form onSubmit={handleAnswersSubmit} className="space-y-4">
                            {questions.map((q, index) => (
                                <div key={q.questionId}>
                                    <label className="block text-sm font-medium text-gray-300 mb-2">
                                        {index + 1}. {q.question}
                                    </label>
                                    <Input
                                        placeholder="Your answer"
                                        value={answers.find(a => a.questionId === q.questionId)?.answer || ''}
                                        onChange={(e) => updateAnswer(q.questionId, e.target.value)}
                                        required
                                    />
                                </div>
                            ))}

                            {error && (
                                <div className="p-3 bg-danger/10 border border-danger/20 rounded-xl text-danger text-sm text-center">
                                    {error}
                                </div>
                            )}

                            <div className="flex gap-3">
                                <Button
                                    type="button"
                                    variant="secondary"
                                    onClick={() => setStep(1)}
                                    className="flex-1"
                                >
                                    <ChevronLeft className="w-4 h-4 mr-1" />
                                    Back
                                </Button>
                                <Button type="submit" className="flex-1">
                                    Continue
                                    <ArrowRight className="w-4 h-4 ml-2" />
                                </Button>
                            </div>
                        </form>
                    )}

                    {step === 3 && (
                        <form onSubmit={handleResetSubmit} className="space-y-4">
                            <Input
                                label="New Password"
                                type="password"
                                required
                                placeholder="••••••••"
                                icon={<Lock className="w-5 h-5" />}
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                            />

                            <Input
                                label="Confirm Password"
                                type="password"
                                required
                                placeholder="••••••••"
                                icon={<Lock className="w-5 h-5" />}
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                            />

                            {error && (
                                <div className="p-3 bg-danger/10 border border-danger/20 rounded-xl text-danger text-sm text-center">
                                    {error}
                                </div>
                            )}

                            <div className="flex gap-3">
                                <Button
                                    type="button"
                                    variant="secondary"
                                    onClick={() => setStep(2)}
                                    className="flex-1"
                                >
                                    <ChevronLeft className="w-4 h-4 mr-1" />
                                    Back
                                </Button>
                                <Button type="submit" loading={loading} className="flex-1">
                                    {!loading && 'Reset Password'}
                                </Button>
                            </div>
                        </form>
                    )}

                    {step === 4 && (
                        <div className="text-center space-y-4">
                            <div className="w-16 h-16 rounded-full bg-secondary/20 flex items-center justify-center mx-auto">
                                <Check className="w-8 h-8 text-secondary" />
                            </div>
                            <p className="text-gray-400">
                                Your password has been reset successfully.
                            </p>
                            <Button onClick={() => navigate('/login')} className="w-full">
                                Sign In
                                <ArrowRight className="w-4 h-4 ml-2" />
                            </Button>
                        </div>
                    )}
                </div>

                {/* Footer */}
                <p className="mt-6 text-center text-sm text-gray-400">
                    Remember your password?{' '}
                    <Link to="/login" className="text-primary hover:text-blue-400 font-medium">
                        Sign in
                    </Link>
                </p>
            </div>
        </div>
    );
};
