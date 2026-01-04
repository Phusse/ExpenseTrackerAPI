import { Trophy, Star, Target, Award, Zap, TrendingUp } from 'lucide-react';

interface Achievement {
    id: string;
    title: string;
    description: string;
    icon: string;
    earned: boolean;
    progress?: number;
    total?: number;
}

interface AchievementBadgesProps {
    achievements: Achievement[];
}

const ACHIEVEMENT_ICONS: { [key: string]: any } = {
    trophy: Trophy,
    star: Star,
    target: Target,
    award: Award,
    zap: Zap,
    trending: TrendingUp
};

export const AchievementBadges = ({ achievements }: AchievementBadgesProps) => {
    if (!achievements || achievements.length === 0) {
        return null;
    }

    const earnedAchievements = achievements.filter(a => a.earned);
    const inProgressAchievements = achievements.filter(a => !a.earned && a.progress);

    return (
        <div className="bg-surface border border-slate-800 rounded-xl p-6">
            <div className="flex items-center justify-between mb-4">
                <h3 className="font-semibold text-white">Achievements</h3>
                <span className="text-sm text-gray-400">{earnedAchievements.length} earned</span>
            </div>

            {/* Earned Badges */}
            {earnedAchievements.length > 0 && (
                <div className="mb-6">
                    <p className="text-xs text-gray-500 uppercase mb-3">Earned</p>
                    <div className="flex flex-wrap gap-3">
                        {earnedAchievements.slice(0, 6).map((achievement) => {
                            const Icon = ACHIEVEMENT_ICONS[achievement.icon] || Trophy;
                            return (
                                <div
                                    key={achievement.id}
                                    className="group relative"
                                    title={achievement.description}
                                >
                                    <div className="w-16 h-16 rounded-xl bg-gradient-to-br from-amber-500 to-orange-500 flex items-center justify-center border-2 border-amber-400 shadow-lg shadow-amber-500/20">
                                        <Icon className="w-8 h-8 text-white" />
                                    </div>
                                    <div className="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 px-3 py-2 bg-slate-900 border border-slate-700 rounded-lg opacity-0 group-hover:opacity-100 transition-opacity pointer-events-none whitespace-nowrap z-10">
                                        <p className="text-xs font-medium text-white">{achievement.title}</p>
                                        <p className="text-xs text-gray-400">{achievement.description}</p>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            )}

            {/* In Progress */}
            {inProgressAchievements.length > 0 && (
                <div>
                    <p className="text-xs text-gray-500 uppercase mb-3">In Progress</p>
                    <div className="space-y-3">
                        {inProgressAchievements.slice(0, 2).map((achievement) => {
                            const Icon = ACHIEVEMENT_ICONS[achievement.icon] || Target;
                            const progress = ((achievement.progress || 0) / (achievement.total || 1)) * 100;

                            return (
                                <div key={achievement.id} className="bg-slate-900/50 rounded-lg p-3 border border-slate-800">
                                    <div className="flex items-center gap-3 mb-2">
                                        <div className="w-10 h-10 rounded-lg bg-slate-800 flex items-center justify-center">
                                            <Icon className="w-5 h-5 text-gray-400" />
                                        </div>
                                        <div className="flex-1">
                                            <p className="text-sm font-medium text-white">{achievement.title}</p>
                                            <p className="text-xs text-gray-500">{achievement.progress}/{achievement.total}</p>
                                        </div>
                                    </div>
                                    <div className="h-1.5 bg-slate-800 rounded-full overflow-hidden">
                                        <div
                                            className="h-full bg-gradient-to-r from-blue-500 to-purple-500 rounded-full transition-all"
                                            style={{ width: `${progress}%` }}
                                        />
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            )}
        </div>
    );
};
