using RSBot.Core;
using RSBot.Core.Client.ReferenceObjects;
using RSBot.Core.Components;
using RSBot.Core.Objects;
using RSBot.Skills.Components;
using System.Linq;
using System.Threading;

namespace RSBot.Skills
{
    public class SkillsManager
    {
        private bool _isUpdatingMastery = false;

        public void UpdateMastery(byte level, RefSkillMastery record,decimal gap = 0)
        {
            if (_isUpdatingMastery) return;
            while (level + gap < Game.Player.Level)
            {
                _isUpdatingMastery = true;
                var nextMasteryLevel = Game.ReferenceManager.GetRefLevel((byte)(level + 1));

                if (nextMasteryLevel.Exp_M > Game.Player.SkillPoints)
                {
                    Log.Debug(
                        $"Auto. upping mastery cancelled due to insufficient skill points. Required: {nextMasteryLevel.Exp_M}"
                    );

                    break;
                }
                Log.Notify($"Auto. train mastery [{record.Name} to lv. {nextMasteryLevel}");
                LearnMasteryHandler.LearnMastery(record.ID);
                level += 1;
                Thread.Sleep(500);
            }
            _isUpdatingMastery = false;
        }
        /// <summary>
        ///     Applies the attack skills.
        /// </summary>
        public static void ApplyAttackSkills()
        {
            foreach (var collection in SkillManager.Skills.Values)
                collection.Clear();

            for (var i = 0; i < 10; i++)
            {
                var skillIds = PlayerConfig.GetArray<uint>("RSBot.Skills.Attacks_" + i);

                foreach (var skillId in skillIds)
                {
                    var skillInfo = Game.Player.Skills.GetSkillInfoById(skillId);
                    if (skillInfo == null)
                        continue;

                    switch (i)
                    {
                        case 1:
                            SkillManager.Skills[MonsterRarity.Champion].Add(skillInfo);
                            continue;
                        case 2:
                            SkillManager.Skills[MonsterRarity.Giant].Add(skillInfo);
                            continue;
                        case 3:
                            SkillManager.Skills[MonsterRarity.GeneralParty].Add(skillInfo);
                            continue;
                        case 4:
                            SkillManager.Skills[MonsterRarity.ChampionParty].Add(skillInfo);
                            continue;
                        case 5:
                            SkillManager.Skills[MonsterRarity.GiantParty].Add(skillInfo);
                            continue;
                        case 6:
                            SkillManager.Skills[MonsterRarity.Elite].Add(skillInfo);
                            continue;
                        case 7:
                            SkillManager.Skills[MonsterRarity.EliteStrong].Add(skillInfo);
                            continue;
                        case 8:
                            SkillManager.Skills[MonsterRarity.Unique].Add(skillInfo);
                            continue;
                        case 9:
                            SkillManager.Skills[MonsterRarity.Event].Add(skillInfo);
                            continue;
                        default:
                            SkillManager.Skills[MonsterRarity.General].Add(skillInfo);
                            continue;
                    }
                }
            }
        }
        /// <summary>
        ///     Applies the buff skills.
        /// </summary>
        public static void ApplyBuffSkills()
        {
            SkillManager.Buffs.Clear();

            Game.Player.TryGetAbilitySkills(out var abilitySkills);

            foreach (var buffId in PlayerConfig.GetArray<uint>("RSBot.Skills.Buffs"))
            {
                var skillInfo = Game.Player.Skills.GetSkillInfoById(buffId);
                if (skillInfo == null)
                {
                    skillInfo = abilitySkills.FirstOrDefault(p => p.Id == buffId);
                    if (skillInfo == null)
                        continue;
                }

                SkillManager.Buffs.Add(skillInfo);
            }
        }
    }
}
