import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Lock, User, ArrowRight, Wallet, ChevronLeft, Shield } from 'lucide-react';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { authService, type SecurityQuestionItem, type SecurityQuestionAnswer } from '../services/authService';
import { useToast } from '../context/ToastContext';

export const Signup = () => {
    const navigate = useNavigate();
    const toast = useToast();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [step, setStep] = useState(1); // 1 = Basic info, 2 = Security questions
    const [availableQuestions, setAvailableQuestions] = useState<SecurityQuestionItem[]>([]);

    const [formData, setFormData] = useState({
        name: '',
        email: '',
        password: '',
        confirmPassword: ''
    });

    const [securityAnswers, setSecurityAnswers] = useState<{ questionId: number; answer: string }[]>([
        { questionId: 0, answer: '' },
        { questionId: 0, answer: '' },
        { questionId: 0, answer: '' }
    ]);

    useEffect(() => {
        loadSecurityQuestions();
    }, []);

    const loadSecurityQuestions = async () => {
        try {
            const questions = await authService.getSecurityQuestions();
            setAvailableQuestions(questions);
            // Set default selections
            if (questions.length >= 3) {
                setSecurityAnswers([
                    { questionId: questions[0].id, answer: '' },
                    { questionId: questions[1].id, answer: '' },
                    { questionId: questions[2].id, answer: '' }
                ]);
            }
        } catch (err) {
            console.error('Failed to load security questions', err);
        }
    };

    const handleStep1Submit = (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        if (formData.password !== formData.confirmPassword) {
            setError('Passwords do not match');
            return;
        }

        if (formData.password.length < 6) {
            setError('Password must be at least 6 characters');
            return;
        }

        setStep(2);
    };

    const handleStep2Submit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');

        // Validate security answers
        const questionIds = securityAnswers.map(a => a.questionId);
        if (new Set(questionIds).size !== 3) {
            setError('Please select 3 different security questions');
            return;
        }

        for (const answer of securityAnswers) {
            if (!answer.answer || answer.answer.trim().length < 2) {
                setError('Each security answer must be at least 2 characters');
                return;
            }
        }

        setLoading(true);

        try {
            const result = await authService.registerWithSecurityQuestions(
                formData.name,
                formData.email,
                formData.password,
                securityAnswers as SecurityQuestionAnswer[]
            );

            if (result.success) {
                toast.success('Account Created!', 'Please sign in to continue.');
                navigate('/login');
            } else {
                setError(result.message || 'Registration failed. Please try again.');
            }
        } catch (err: any) {
            let errorMessage = 'An unexpected error occurred.';

            if (err.response?.data?.message) {
                errorMessage = err.response.data.message;
            } else if (err.apiError?.message) {
                errorMessage = err.apiError.message;
            } else if (err.code === 'ERR_NETWORK') {
                errorMessage = 'Unable to connect to the server.';
            } else if (err.message) {
                errorMessage = err.message;
            }

            setError(errorMessage);
        } finally {
            setLoading(false);
        }
    };

    const updateSecurityAnswer = (index: number, field: 'questionId' | 'answer', value: number | string) => {
        setSecurityAnswers(prev => {
            const updated = [...prev];
            if (field === 'questionId') {
                updated[index] = { ...updated[index], questionId: value as number };
            } else {
                updated[index] = { ...updated[index], answer: value as string };
            }
            return updated;
        });
    };

    const getAvailableQuestionsForIndex = (index: number) => {
        const selectedIds = securityAnswers.map((a, i) => i !== index ? a.questionId : 0);
        return availableQuestions.filter(q => !selectedIds.includes(q.id) || q.id === securityAnswers[index].questionId);
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
                        {step === 1 ? <Wallet className="w-7 h-7 text-white" /> : <Shield className="w-7 h-7 text-white" />}
                    </div>
                    <h1 className="text-2xl font-bold text-white">
                        {step === 1 ? 'Create Account' : 'Security Questions'}
                    </h1>
                    <p className="text-gray-400 text-sm mt-1">
                        {step === 1 ? 'Start your financial journey' : 'Set up account recovery'}
                    </p>
                </div>

                {/* Progress */}
                <div className="flex gap-2 mb-6">
                    <div className={`h-1 flex-1 rounded-full ${step >= 1 ? 'bg-primary' : 'bg-slate-700'}`} />
                    <div className={`h-1 flex-1 rounded-full ${step >= 2 ? 'bg-primary' : 'bg-slate-700'}`} />
                </div>

                {/* Form */}
                <div className="glass-card p-6 md:p-8">
                    {step === 1 ? (
                        <form onSubmit={handleStep1Submit} className="space-y-4">
                            <Input
                                label="Full Name"
                                type="text"
                                required
                                placeholder="John Doe"
                                icon={<User className="w-5 h-5" />}
                                value={formData.name}
                                onChange={(e) => setFormData({ ...formData, name: e.target.value })}
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
                                <div className="p-3 bg-danger/10 border border-danger/20 rounded-xl text-danger text-sm text-center">
                                    {error}
                                </div>
                            )}

                            <Button type="submit" className="w-full">
                                Continue
                                <ArrowRight className="w-4 h-4 ml-2" />
                            </Button>
                        </form>
                    ) : (
                        <form onSubmit={handleStep2Submit} className="space-y-4">
                            <p className="text-sm text-gray-400 mb-4">
                                Choose 3 security questions. These will be used to recover your account if you forget your password.
                            </p>

                            {[0, 1, 2].map((index) => (
                                <div key={index} className="space-y-2">
                                    <label className="block text-sm font-medium text-gray-300">
                                        Question {index + 1}
                                    </label>
                                    <select
                                        value={securityAnswers[index].questionId}
                                        onChange={(e) => updateSecurityAnswer(index, 'questionId', parseInt(e.target.value))}
                                        className="w-full px-4 py-3 bg-slate-800/50 border border-slate-700 rounded-xl text-white focus:outline-none focus:border-primary"
                                        required
                                    >
                                        <option value={0} disabled>Select a question...</option>
                                        {getAvailableQuestionsForIndex(index).map((q) => (
                                            <option key={q.id} value={q.id}>{q.question}</option>
                                        ))}
                                    </select>
                                    <Input
                                        placeholder="Your answer"
                                        value={securityAnswers[index].answer}
                                        onChange={(e) => updateSecurityAnswer(index, 'answer', e.target.value)}
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
                                <Button
                                    type="submit"
                                    loading={loading}
                                    className="flex-1"
                                >
                                    {!loading && (
                                        <>
                                            Create Account
                                            <ArrowRight className="w-4 h-4 ml-2" />
                                        </>
                                    )}
                                </Button>
                            </div>
                        </form>
                    )}
                </div>

                {/* Footer */}
                <p className="mt-6 text-center text-sm text-gray-400">
                    Already have an account?{' '}
                    <Link to="/login" className="text-primary hover:text-blue-400 font-medium">
                        Sign in
                    </Link>
                </p>
            </div>
        </div>
    );
};
