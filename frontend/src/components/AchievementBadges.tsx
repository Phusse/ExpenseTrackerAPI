import { Trophy, Star, Zap, Award, TrendingUp, Target, PiggyBank, Wallet, BadgeCheck, Medal, Crown, Flame, Gift, Heart, Shield } from 'lucide-react';
import type { Achievement } from '../services/analyticsService';

interface AchievementBadgesProps {
    achievements: Achievement[];
}

// Map icon names from backend to Lucide React icons
const iconMap: Record<string, React.ReactNode> = {
    'zap': <Zap className="w-6 h-6" />,
    'trophy': <Trophy className="w-6 h-6" />,
    'award': <Award className="w-6 h-6" />,
    'star': <Star className="w-6 h-6" />,
    'trending': <TrendingUp className="w-6 h-6" />,
    'target': <Target className="w-6 h-6" />,
    'piggy': <PiggyBank className="w-6 h-6" />,
    'wallet': <Wallet className="w-6 h-6" />,
    'badge': <BadgeCheck className="w-6 h-6" />,
    'medal': <Medal className="w-6 h-6" />,
    'crown': <Crown className="w-6 h-6" />,
    'flame': <Flame className="w-6 h-6" />,
    'gift': <Gift className="w-6 h-6" />,
    'heart': <Heart className="w-6 h-6" />,
    'shield': <Shield className="w-6 h-6" />,
};

const getIcon = (iconName: string) => {
    // Check if it's in our icon map
    const lowerName = iconName?.toLowerCase() || '';
    if (iconMap[lowerName]) {
        return iconMap[lowerName];
    }
    // Check if it contains key words
    for (const key of Object.keys(iconMap)) {
        if (lowerName.includes(key)) {
            return iconMap[key];
        }
    }
    // Default fallback
    return <Star className="w-6 h-6" />;
};

export const AchievementBadges = ({ achievements }: AchievementBadgesProps) => {
    const earnedAchievements = achievements.filter(a => a.earned);
    const inProgressAchievements = achievements.filter(a => !a.earned && a.progress && a.progress > 0);

    if (earnedAchievements.length === 0 && inProgressAchievements.length === 0) {
        return null;
    }

    return (
        <div className="glass-card p-4 md:p-6">
            <h3 className="font-bold text-white mb-4 flex items-center gap-2">
                <Trophy className="w-5 h-5 text-amber-400" />
                Achievements
            </h3>

            {/* Earned */}
            {earnedAchievements.length > 0 && (
                <div className="flex gap-2 overflow-x-auto pb-2 mb-3">
                    {earnedAchievements.map((achievement) => (
                        <div
                            key={achievement.id}
                            className="flex-shrink-0 w-16 h-16 rounded-xl bg-gradient-to-br from-amber-500/20 to-yellow-500/20 border border-amber-500/20 flex flex-col items-center justify-center text-amber-400"
                            title={achievement.title}
                        >
                            {getIcon(achievement.icon)}
                            <span className="text-[10px] text-amber-400 mt-1 truncate max-w-full px-1">
                                {achievement.title.split(' ')[0]}
                            </span>
                        </div>
                    ))}
                </div>
            )}

            {/* In Progress */}
            {inProgressAchievements.length > 0 && (
                <div className="space-y-2">
                    {inProgressAchievements.slice(0, 2).map((achievement) => (
                        <div key={achievement.id} className="flex items-center gap-3">
                            <div className="w-10 h-10 rounded-xl bg-slate-800 flex items-center justify-center text-gray-500">
                                {getIcon(achievement.icon)}
                            </div>
                            <div className="flex-1 min-w-0">
                                <p className="text-xs font-medium text-white truncate">{achievement.title}</p>
                                <div className="flex items-center gap-2 mt-1">
                                    <div className="flex-1 h-1.5 bg-slate-800 rounded-full overflow-hidden">
                                        <div
                                            className="h-full bg-primary rounded-full"
                                            style={{ width: `${achievement.progress || 0}%` }}
                                        />
                                    </div>
                                    <span className="text-[10px] text-gray-500">{Math.round(achievement.progress || 0)}%</span>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};
