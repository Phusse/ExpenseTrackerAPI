import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    User,
    Bell,
    Globe,
    Shield,
    LogOut,
    ChevronRight,
    Smartphone,
    Info,
    Lock,
    Trash2,
    Download,
    X,
    Check,
    Loader2
} from 'lucide-react';
import { authService } from '../services/authService';
import { userService, type Currency } from '../services/userService';
import { useSettings } from '../context/SettingsContext';
import { useToast } from '../context/ToastContext';
import { Button } from '../components/Button';
import { Input } from '../components/Input';
import { ConfirmModal } from '../components/ConfirmModal';

export const Settings = () => {
    const navigate = useNavigate();
    const toast = useToast();
    const { settings, updateSetting } = useSettings();

    // State
    const [currencies, setCurrencies] = useState<Currency[]>([]);
    const [showLogoutConfirm, setShowLogoutConfirm] = useState(false);
    const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

    // Modal states
    const [showProfileModal, setShowProfileModal] = useState(false);
    const [showPasswordModal, setShowPasswordModal] = useState(false);
    const [showCurrencyModal, setShowCurrencyModal] = useState(false);

    // Form states
    const [profileForm, setProfileForm] = useState({ name: '', email: '' });
    const [passwordForm, setPasswordForm] = useState({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
    const [deletePassword, setDeletePassword] = useState('');
    const [securityQuestion, setSecurityQuestion] = useState<{ questionId: number; question: string } | null>(null);
    const [securityAnswer, setSecurityAnswer] = useState('');
    const [saving, setSaving] = useState(false);

    const user = authService.getCurrentUser();

    useEffect(() => {
        fetchCurrencies();
    }, []);

    const fetchCurrencies = async () => {
        try {
            const data = await userService.getCurrencies();
            setCurrencies(data);
        } catch (error) {
            console.error('Failed to fetch currencies', error);
        }
    };

    const handleLogout = () => {
        authService.logout();
        toast.success('Logged out', 'You have been signed out successfully.');
        navigate('/login');
    };

    const handleUpdateProfile = async () => {
        setSaving(true);
        try {
            const result = await userService.updateProfile({
                name: profileForm.name || undefined,
                email: profileForm.email || undefined
            });
            if (result.success) {
                toast.success('Profile Updated', result.message || 'Your profile has been updated.');
                setShowProfileModal(false);
                // Update local storage
                const currentUser = authService.getCurrentUser();
                if (currentUser) {
                    localStorage.setItem('user', JSON.stringify({
                        ...currentUser,
                        name: profileForm.name || currentUser.name,
                        email: profileForm.email || currentUser.email
                    }));
                }
            } else {
                toast.error('Error', result.message || 'Failed to update profile.');
            }
        } catch (error: any) {
            toast.error('Error', error.response?.data?.message || 'Failed to update profile.');
        } finally {
            setSaving(false);
        }
    };

    const handleChangePassword = async () => {
        if (passwordForm.newPassword !== passwordForm.confirmNewPassword) {
            toast.error('Error', 'New passwords do not match.');
            return;
        }
        setSaving(true);
        try {
            const result = await userService.changePassword(passwordForm);
            if (result.success) {
                toast.success('Password Changed', result.message || 'Your password has been changed.');
                setShowPasswordModal(false);
                setPasswordForm({ currentPassword: '', newPassword: '', confirmNewPassword: '' });
            } else {
                toast.error('Error', result.message || 'Failed to change password.');
            }
        } catch (error: any) {
            toast.error('Error', error.response?.data?.message || 'Failed to change password.');
        } finally {
            setSaving(false);
        }
    };

    const handleToggleNotifications = async (value: boolean) => {
        try {
            await updateSetting('notificationsEnabled', value);
        } catch (error) {
            toast.error('Error', 'Failed to update setting.');
        }
    };

    const handleChangeCurrency = async (currency: string) => {
        try {
            await updateSetting('currency', currency);
            setShowCurrencyModal(false);
            toast.success('Currency Updated', `Currency changed to ${currency}.`);
        } catch (error) {
            toast.error('Error', 'Failed to update currency.');
        }
    };

    const handleExportData = async () => {
        try {
            const blob = await userService.exportData();
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `expense-tracker-export-${new Date().toISOString().split('T')[0]}.json`;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            window.URL.revokeObjectURL(url);
            toast.success('Data Exported', 'Your data has been downloaded.');
        } catch (error) {
            toast.error('Error', 'Failed to export data.');
        }
    };

    const handleInitiateDelete = async () => {
        setSaving(true);
        try {
            // Check for security questions
            const questions = await authService.getMySecurityQuestions();
            if (questions && questions.length > 0) {
                // Pick random question
                const randomIndex = Math.floor(Math.random() * questions.length);
                setSecurityQuestion(questions[randomIndex]);
            } else {
                setSecurityQuestion(null);
            }
            setShowDeleteConfirm(true);
        } catch (error) {
            // proceed without security question if failed (or legacy user)
            setSecurityQuestion(null);
            setShowDeleteConfirm(true);
        } finally {
            setSaving(false);
        }
    };

    const handleDeleteAccount = async () => {
        if (!deletePassword) {
            toast.error('Error', 'Please enter your password to confirm.');
            return;
        }

        if (securityQuestion && !securityAnswer) {
            toast.error('Error', 'Please answer the security question.');
            return;
        }

        setSaving(true);
        try {
            const result = await userService.deleteAccount(
                deletePassword,
                securityQuestion?.questionId,
                securityAnswer
            );

            if (result.success) {
                toast.success('Account Deleted', 'Your account has been permanently deleted.');
                authService.logout();
                navigate('/login');
            } else {
                toast.error('Error', result.message || 'Failed to delete account.');
            }
        } catch (error: any) {
            toast.error('Error', error.response?.data?.message || 'Failed to delete account.');
        } finally {
            setSaving(false);
        }
    };

    const SettingItem = ({
        icon: Icon,
        label,
        value,
        onClick,
        danger = false,
        toggle = false,
        isOn = false,
        onToggle
    }: {
        icon: any;
        label: string;
        value?: string;
        onClick?: () => void;
        danger?: boolean;
        toggle?: boolean;
        isOn?: boolean;
        onToggle?: (val: boolean) => void;
    }) => (
        <button
            onClick={toggle ? () => onToggle?.(!isOn) : onClick}
            className={`w-full flex items-center gap-4 p-4 rounded-xl transition-colors ${danger ? 'hover:bg-rose-500/10 active:bg-rose-500/20' : 'hover:bg-white/5 active:bg-white/10'}`}
        >
            <div className={`w-10 h-10 rounded-xl flex items-center justify-center ${danger ? 'bg-rose-500/10' : 'bg-slate-800'}`}>
                <Icon className={`w-5 h-5 ${danger ? 'text-rose-400' : 'text-gray-400'}`} />
            </div>
            <div className="flex-1 text-left">
                <p className={`font-medium ${danger ? 'text-rose-400' : 'text-white'}`}>{label}</p>
                {value && <p className="text-sm text-gray-500 mt-0.5">{value}</p>}
            </div>
            {toggle ? (
                <div
                    className={`w-12 h-7 rounded-full p-1 transition-colors ${isOn ? 'bg-primary' : 'bg-slate-700'}`}
                >
                    <div
                        className={`w-5 h-5 rounded-full bg-white transition-transform ${isOn ? 'translate-x-5' : 'translate-x-0'}`}
                    />
                </div>
            ) : (
                <ChevronRight className="w-5 h-5 text-gray-600" />
            )}
        </button>
    );

    if (!settings) {
        return (
            <div className="flex items-center justify-center py-20">
                <Loader2 className="w-8 h-8 animate-spin text-primary" />
            </div>
        );
    }

    return (
        <div className="min-h-screen pb-20 md:pb-8">
            {/* Header */}
            <div className="mb-6">
                <h1 className="text-2xl font-bold text-white">Settings</h1>
                <p className="text-gray-400 text-sm mt-1">Manage your account and preferences</p>
            </div>

            {/* Profile Section */}
            <div className="glass-card p-6 mb-6">
                <div className="flex items-center gap-4">
                    <div className="w-16 h-16 rounded-2xl bg-gradient-to-br from-primary to-indigo-600 flex items-center justify-center">
                        <span className="text-2xl font-bold text-white">
                            {user?.name?.charAt(0)?.toUpperCase() || 'U'}
                        </span>
                    </div>
                    <div className="flex-1 min-w-0">
                        <h2 className="text-lg font-bold text-white truncate">{user?.name || 'User'}</h2>
                        <p className="text-sm text-gray-400 truncate">{user?.email || 'user@example.com'}</p>
                    </div>
                </div>
            </div>

            {/* Account Settings */}
            <div className="glass-card mb-4 overflow-hidden">
                <div className="px-4 pt-4 pb-2">
                    <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wider">Account</h3>
                </div>
                <SettingItem
                    icon={User}
                    label="Edit Profile"
                    value="Update your name or email"
                    onClick={() => {
                        setProfileForm({ name: user?.name || '', email: user?.email || '' });
                        setShowProfileModal(true);
                    }}
                />
                <SettingItem
                    icon={Lock}
                    label="Change Password"
                    value="Update your password"
                    onClick={() => setShowPasswordModal(true)}
                />
            </div>

            {/* Preferences */}
            <div className="glass-card mb-4 overflow-hidden">
                <div className="px-4 pt-4 pb-2">
                    <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wider">Preferences</h3>
                </div>
                <SettingItem
                    icon={Bell}
                    label="Notifications"
                    toggle
                    isOn={settings.notificationsEnabled}
                    onToggle={(val) => handleToggleNotifications(val)}
                />
                <SettingItem
                    icon={Globe}
                    label="Currency"
                    value={currencies.find(c => c.code === settings.currency)?.name || settings.currency}
                    onClick={() => setShowCurrencyModal(true)}
                />
            </div>

            {/* Data & Privacy */}
            <div className="glass-card mb-4 overflow-hidden">
                <div className="px-4 pt-4 pb-2">
                    <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wider">Data & Privacy</h3>
                </div>
                <SettingItem
                    icon={Download}
                    label="Export Data"
                    value="Download your financial data as JSON"
                    onClick={handleExportData}
                />
                <SettingItem
                    icon={Shield}
                    label="Privacy Policy"
                    onClick={() => toast.info('Coming soon', 'Privacy policy will be available soon.')}
                />
            </div>

            {/* About */}
            <div className="glass-card mb-4 overflow-hidden">
                <div className="px-4 pt-4 pb-2">
                    <h3 className="text-xs font-semibold text-gray-500 uppercase tracking-wider">About</h3>
                </div>
                <SettingItem
                    icon={Smartphone}
                    label="App Version"
                    value="1.0.0"
                    onClick={() => { }}
                />
                <SettingItem
                    icon={Info}
                    label="Help & Support"
                    onClick={() => toast.info('Coming soon', 'Help center will be available soon.')}
                />
            </div>

            {/* Danger Zone */}
            <div className="glass-card overflow-hidden">
                <div className="px-4 pt-4 pb-2">
                    <h3 className="text-xs font-semibold text-rose-400 uppercase tracking-wider">Danger Zone</h3>
                </div>
                <SettingItem
                    icon={LogOut}
                    label="Log Out"
                    danger
                    onClick={() => setShowLogoutConfirm(true)}
                />
                <SettingItem
                    icon={Trash2}
                    label="Delete Account"
                    value="Permanently delete your account and data"
                    danger
                    onClick={handleInitiateDelete}
                />
            </div>

            {/* Logout Confirmation */}
            <ConfirmModal
                isOpen={showLogoutConfirm}
                title="Log Out"
                message="Are you sure you want to log out?"
                confirmText="Log Out"
                onConfirm={handleLogout}
                onClose={() => setShowLogoutConfirm(false)}
            />

            {/* Edit Profile Modal */}
            {showProfileModal && (
                <>
                    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={() => setShowProfileModal(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md max-h-[90vh] overflow-auto animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <div className="flex items-center justify-between mb-6">
                                    <h2 className="text-xl font-bold text-white">Edit Profile</h2>
                                    <button onClick={() => setShowProfileModal(false)} className="p-2 text-gray-400 hover:text-white">
                                        <X className="w-5 h-5" />
                                    </button>
                                </div>
                                <div className="space-y-4">
                                    <Input
                                        label="Name"
                                        value={profileForm.name}
                                        onChange={(e) => setProfileForm({ ...profileForm, name: e.target.value })}
                                        placeholder="Your name"
                                    />
                                    <Input
                                        label="Email"
                                        type="email"
                                        value={profileForm.email}
                                        onChange={(e) => setProfileForm({ ...profileForm, email: e.target.value })}
                                        placeholder="your@email.com"
                                    />
                                    <Button onClick={handleUpdateProfile} loading={saving} className="w-full">
                                        Save Changes
                                    </Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </>
            )}

            {/* Change Password Modal */}
            {showPasswordModal && (
                <>
                    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={() => setShowPasswordModal(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md max-h-[90vh] overflow-auto animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <div className="flex items-center justify-between mb-6">
                                    <h2 className="text-xl font-bold text-white">Change Password</h2>
                                    <button onClick={() => setShowPasswordModal(false)} className="p-2 text-gray-400 hover:text-white">
                                        <X className="w-5 h-5" />
                                    </button>
                                </div>
                                <div className="space-y-4">
                                    <Input
                                        label="Current Password"
                                        type="password"
                                        value={passwordForm.currentPassword}
                                        onChange={(e) => setPasswordForm({ ...passwordForm, currentPassword: e.target.value })}
                                        placeholder="••••••••"
                                    />
                                    <Input
                                        label="New Password"
                                        type="password"
                                        value={passwordForm.newPassword}
                                        onChange={(e) => setPasswordForm({ ...passwordForm, newPassword: e.target.value })}
                                        placeholder="••••••••"
                                    />
                                    <Input
                                        label="Confirm New Password"
                                        type="password"
                                        value={passwordForm.confirmNewPassword}
                                        onChange={(e) => setPasswordForm({ ...passwordForm, confirmNewPassword: e.target.value })}
                                        placeholder="••••••••"
                                    />
                                    <Button onClick={handleChangePassword} loading={saving} className="w-full">
                                        Change Password
                                    </Button>
                                </div>
                            </div>
                        </div>
                    </div>
                </>
            )}

            {/* Currency Selection Modal */}
            {showCurrencyModal && (
                <>
                    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={() => setShowCurrencyModal(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md max-h-[90vh] overflow-auto animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <div className="flex items-center justify-between mb-6">
                                    <h2 className="text-xl font-bold text-white">Select Currency</h2>
                                    <button onClick={() => setShowCurrencyModal(false)} className="p-2 text-gray-400 hover:text-white">
                                        <X className="w-5 h-5" />
                                    </button>
                                </div>
                                <div className="space-y-2">
                                    {currencies.map((currency) => (
                                        <button
                                            key={currency.code}
                                            onClick={() => handleChangeCurrency(currency.code)}
                                            className={`w-full flex items-center justify-between p-4 rounded-xl transition-colors ${settings.currency === currency.code
                                                ? 'bg-primary/20 border border-primary/30'
                                                : 'bg-white/5 hover:bg-white/10'
                                                }`}
                                        >
                                            <span className="text-white font-medium">{currency.name}</span>
                                            {settings.currency === currency.code && (
                                                <Check className="w-5 h-5 text-primary" />
                                            )}
                                        </button>
                                    ))}
                                </div>
                            </div>
                        </div>
                    </div>
                </>
            )}

            {/* Delete Account Modal */}
            {showDeleteConfirm && (
                <>
                    <div className="fixed inset-0 z-50 bg-black/60 backdrop-blur-sm" onClick={() => setShowDeleteConfirm(false)} />
                    <div className="fixed inset-x-0 bottom-0 z-50 md:inset-0 md:flex md:items-center md:justify-center p-0 md:p-4">
                        <div className="bg-surface-solid rounded-t-3xl md:rounded-2xl w-full md:max-w-md max-h-[90vh] overflow-auto animate-slide-up md:animate-fade-in">
                            <div className="bottom-sheet-handle md:hidden" />
                            <div className="p-6">
                                <div className="flex items-center justify-between mb-4">
                                    <h2 className="text-xl font-bold text-rose-400">Delete Account</h2>
                                    <button onClick={() => setShowDeleteConfirm(false)} className="p-2 text-gray-400 hover:text-white">
                                        <X className="w-5 h-5" />
                                    </button>
                                </div>
                                <div className="mb-6 p-4 bg-rose-500/10 border border-rose-500/20 rounded-xl">
                                    <p className="text-rose-400 text-sm">
                                        <strong>Warning:</strong> This action is permanent and cannot be undone. All your data including expenses, budgets, goals, and settings will be permanently deleted.
                                    </p>
                                </div>
                                <div className="space-y-4">
                                    {/* Security Question Section */}
                                    {securityQuestion && (
                                        <div className="space-y-2">
                                            <p className="text-sm text-gray-400">Security Verification</p>
                                            <div className="p-3 bg-slate-800 rounded-xl border border-slate-700">
                                                <p className="text-white text-sm">{securityQuestion.question}</p>
                                            </div>
                                            <Input
                                                label="Answer"
                                                value={securityAnswer}
                                                onChange={(e) => setSecurityAnswer(e.target.value)}
                                                placeholder="Enter your security answer"
                                            />
                                        </div>
                                    )}

                                    <Input
                                        label="Enter your password to confirm"
                                        type="password"
                                        value={deletePassword}
                                        onChange={(e) => setDeletePassword(e.target.value)}
                                        placeholder="••••••••"
                                    />
                                    <div className="flex gap-3">
                                        <Button
                                            variant="secondary"
                                            onClick={() => setShowDeleteConfirm(false)}
                                            className="flex-1"
                                        >
                                            Cancel
                                        </Button>
                                        <Button
                                            onClick={handleDeleteAccount}
                                            loading={saving}
                                            className="flex-1 !bg-rose-500 hover:!bg-rose-600"
                                        >
                                            Delete Account
                                        </Button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </>
            )}
        </div>
    );
};
