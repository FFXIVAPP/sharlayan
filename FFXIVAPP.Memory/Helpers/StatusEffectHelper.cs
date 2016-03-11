// FFXIVAPP.Memory ~ StatusEffectHelper.cs
// 
// Copyright © 2007 - 2016 Ryan Wilson - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Concurrent;
using System.Linq;
using FFXIVAPP.Memory.Models;

namespace FFXIVAPP.Memory.Helpers
{
    public static class StatusEffectHelper
    {
        private static ConcurrentDictionary<short, StatusItem> _statusEffects;

        private static ConcurrentDictionary<short, StatusItem> StatusEffects
        {
            get { return _statusEffects ?? (_statusEffects = new ConcurrentDictionary<short, StatusItem>()); }
            set
            {
                if (_statusEffects == null)
                {
                    _statusEffects = new ConcurrentDictionary<short, StatusItem>();
                }
                _statusEffects = value;
            }
        }

        public static StatusItem StatusInfo(short id)
        {
            lock (StatusEffects)
            {
                if (!StatusEffects.Any())
                {
                    Generate();
                }
                if (StatusEffects.ContainsKey(id))
                {
                    return StatusEffects[id];
                }
                return new StatusItem
                {
                    Name = new StatusLocalization
                    {
                        Chinese = "???",
                        English = "???",
                        French = "???",
                        German = "???",
                        Japanese = "???",
                        Korean = "???"
                    },
                    CompanyAction = false
                };
            }
        }

        private static void Generate()
        {
            StatusEffects.TryAdd(1, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "石化",
                    English = "Petrification",
                    French = "Pétrification",
                    German = "Stein",
                    Japanese = "石化",
                    Korean = "석화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(2, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "昏迷",
                    English = "Stun",
                    French = "Étourdissement",
                    German = "Betäubung",
                    Japanese = "スタン",
                    Korean = "기절"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(3, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "睡眠",
                    English = "Sleep",
                    French = "Sommeil",
                    German = "Schlaf",
                    Japanese = "睡眠",
                    Korean = "수면"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(4, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "眩晕",
                    English = "Daze",
                    French = "Évanouissement",
                    German = "Benommenheit",
                    Japanese = "気絶",
                    Korean = "혼절"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(5, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Amnesia",
                    English = "Amnesia",
                    French = "Amnésie",
                    German = "Amnesie",
                    Japanese = "アビリティ不可",
                    Korean = "능력 사용불가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(6, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Pacification",
                    English = "Pacification",
                    French = "Pacification",
                    German = "Pacem",
                    Japanese = "ＷＳ不可",
                    Korean = "무기 기술 사용불가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(7, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "沉默",
                    English = "Silence",
                    French = "Silence",
                    German = "Stumm",
                    Japanese = "沈黙",
                    Korean = "침묵"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(8, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "加速",
                    English = "Haste",
                    French = "Hâte",
                    German = "Hast",
                    Japanese = "ヘイスト",
                    Korean = "헤이스트"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(9, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "减速",
                    English = "Slow",
                    French = "Lenteur",
                    German = "Gemach",
                    Japanese = "スロウ",
                    Korean = "둔화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(10, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "减速",
                    English = "Slow",
                    French = "Lenteur",
                    German = "Gemach",
                    Japanese = "拘束装置：スロウ",
                    Korean = "구속 장치: 둔화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(11, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "混乱",
                    English = "Confused",
                    French = "Confusion",
                    German = "Konfus",
                    Japanese = "混乱",
                    Korean = "혼란"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(12, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Levitation",
                    English = "Levitation",
                    French = "Lévitation",
                    German = "Levitation",
                    Japanese = "レビテト",
                    Korean = "레비테트"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(13, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "束缚",
                    English = "Bind",
                    French = "Entrave",
                    German = "Fessel",
                    Japanese = "バインド",
                    Korean = "속박"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(14, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "加重",
                    English = "Heavy",
                    French = "Pesanteur",
                    German = "Gewicht",
                    Japanese = "ヘヴィ",
                    Korean = "과중력"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(15, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "失明",
                    English = "Blind",
                    French = "Cécité",
                    German = "Blind",
                    Japanese = "暗闇",
                    Korean = "암흑"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(17, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "麻痹",
                    English = "Paralysis",
                    French = "Paralysie",
                    German = "Paralyse",
                    Japanese = "麻痺",
                    Korean = "마비"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(18, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "中毒",
                    English = "Poison",
                    French = "Poison",
                    German = "Gift",
                    Japanese = "毒",
                    Korean = "독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(19, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "猛毒",
                    English = "Pollen",
                    French = "Poison violent",
                    German = "Giftpollen",
                    Japanese = "猛毒",
                    Korean = "맹독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(20, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "TP Bleed",
                    English = "TP Bleed",
                    French = "Saignée de PT",
                    German = "TP-Verlust",
                    Japanese = "ＴＰ継続ダメージ",
                    Korean = "TP 지속 피해"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(21, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "最大体力增加",
                    English = "HP Boost",
                    French = "Bonus de PV",
                    German = "LP-Bonus",
                    Japanese = "最大ＨＰアップ",
                    Korean = "최대 HP 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(22, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "最大体力减少",
                    English = "HP Penalty",
                    French = "Malus de PV",
                    German = "LP-Malus",
                    Japanese = "最大ＨＰダウン",
                    Korean = "최대 HP 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(23, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "最大魔力增加",
                    English = "MP Boost",
                    French = "Bonus de PM",
                    German = "MP-Bonus",
                    Japanese = "最大ＭＰアップ",
                    Korean = "최대 MP 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(24, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "最大魔力减少",
                    English = "MP Penalty",
                    French = "Malus de PM",
                    German = "MP-Malus",
                    Japanese = "最大ＭＰダウン",
                    Korean = "최대 MP 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(25, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "物理攻击力提高",
                    English = "Attack Up",
                    French = "Bonus d'attaque",
                    German = "Attacke-Bonus",
                    Japanese = "物理攻撃力アップ",
                    Korean = "물리 공격력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(26, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "物理攻击力降低",
                    English = "Attack Down",
                    French = "Malus d'attaque",
                    German = "Attacke-Malus",
                    Japanese = "物理攻撃力ダウン",
                    Korean = "물리 공격력 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(27, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "命中力增加",
                    English = "Accuracy Up",
                    French = "Bonus de précision",
                    German = "Präzisions-Bonus",
                    Japanese = "命中率アップ",
                    Korean = "명중률 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(28, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "命中力下降",
                    English = "Accuracy Down",
                    French = "Malus de précision",
                    German = "Präzisions-Malus",
                    Japanese = "命中率ダウン",
                    Korean = "명중률 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(29, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "物理防御力提高",
                    English = "Defense Up",
                    French = "Bonus de défense",
                    German = "Verteidigungs-Bonus",
                    Japanese = "物理防御力アップ",
                    Korean = "물리 방어력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(30, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "物理防御力下降",
                    English = "Defense Down",
                    French = "Malus de défense",
                    German = "Verteidigungs-Malus",
                    Japanese = "物理防御力ダウン",
                    Korean = "물리 방어력 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(31, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "回避力提高",
                    English = "Evasion Up",
                    French = "Bonus d'esquive",
                    German = "Ausweich-Bonus",
                    Japanese = "回避力アップ",
                    Korean = "회피력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(32, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "回避力降低",
                    English = "Evasion Down",
                    French = "Malus d'esquive",
                    German = "Ausweich-Malus",
                    Japanese = "回避力ダウン",
                    Korean = "회피력 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(33, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔法攻击力增加",
                    English = "Attack Magic Potency Up",
                    French = "Bonus de puissance magique",
                    German = "Offensivmagie-Bonus",
                    Japanese = "魔法攻撃力アップ",
                    Korean = "마법 공격력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(34, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔法攻击力降低",
                    English = "Attack Magic Potency Down",
                    French = "Malus de puissance magique",
                    German = "Offensivmagie-Malus",
                    Japanese = "魔法攻撃力ダウン",
                    Korean = "마법 공격력 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(35, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔法恢复力增加",
                    English = "Healing Potency Up",
                    French = "Bonus de magie curative",
                    German = "Heilmagie-Bonus",
                    Japanese = "魔法回復力アップ",
                    Korean = "마법 회복력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(36, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Healing Potency Down",
                    English = "Healing Potency Down",
                    French = "Malus de magie curative",
                    German = "Heilmagie-Malus",
                    Japanese = "魔法回復力ダウン",
                    Korean = "마법 회복력 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(37, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔法防御力上升",
                    English = "Magic Defense Up",
                    French = "Bonus de défense magique",
                    German = "Magieabwehr-Bonus",
                    Japanese = "魔法防御力アップ",
                    Korean = "마법 방어력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(38, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔法防御力下降",
                    English = "Magic Defense Down",
                    French = "Malus de défense magique",
                    German = "Magieabwehr-Malus",
                    Japanese = "魔法防御力ダウン",
                    Korean = "마법 방어력 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(39, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "眩晕抗性",
                    English = "Stun Resistance",
                    French = "Résistance à Étourdissement",
                    German = "Betäubungsresistenz",
                    Japanese = "スタン無効",
                    Korean = "기절 무효"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(40, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "沉默抗性",
                    English = "Silence Resistance",
                    French = "Résistance à Silence",
                    German = "Stumm-Resistenz",
                    Japanese = "沈黙無効",
                    Korean = "침묵 무효"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(41, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "制作设备",
                    English = "Crafting Facility",
                    French = "Installation d'artisanat",
                    German = "Werkstattstimmung",
                    Japanese = "製作設備",
                    Korean = "제작설비"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(42, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "超越之力",
                    English = "The Echo",
                    French = "L'Écho",
                    German = "Kraft des Transzendierens",
                    Japanese = "超える力",
                    Korean = "초월하는 힘"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(43, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "衰弱",
                    English = "Weakness",
                    French = "Affaiblissement",
                    German = "Schwäche",
                    Japanese = "衰弱",
                    Korean = "쇠약"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(44, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Brink of Death",
                    English = "Brink of Death",
                    French = "Mourant",
                    German = "Sterbenselend",
                    Japanese = "衰弱［強］",
                    Korean = "쇠약[강]"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(45, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Crafter's Grace",
                    English = "Crafter's Grace",
                    French = "Grâce de l'artisan",
                    German = "Sternstunde der Handwerker",
                    Japanese = "経験値アップ（クラフター専用）",
                    Korean = "경험치 증가(제작자 전용)"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(46, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Gatherer's Grace",
                    English = "Gatherer's Grace",
                    French = "Grâce du récolteur",
                    German = "Sternstunde der Sammler",
                    Japanese = "経験値アップ（ギャザラー専用）",
                    Korean = "경험치 증가(채집가 전용)"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(47, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stealth",
                    English = "Stealth",
                    French = "Furtivité",
                    German = "Coeurl-Pfoten",
                    Japanese = "ステルス",
                    Korean = "은신"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(48, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "用食",
                    English = "Well Fed",
                    French = "Repu",
                    German = "Gut gesättigt",
                    Japanese = "食事",
                    Korean = "식사"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(49, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "强化药",
                    English = "Medicated",
                    French = "Médicamenté",
                    German = "Stärkung",
                    Japanese = "強化薬",
                    Korean = "강화약"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(50, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sprint",
                    English = "Sprint",
                    French = "Sprint",
                    German = "Sprint",
                    Japanese = "スプリント",
                    Korean = "전력 질주"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(51, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "力量下降",
                    English = "Strength Down",
                    French = "Malus de force",
                    German = "Stärke-Malus",
                    Japanese = "ＳＴＲダウン",
                    Korean = "힘 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(52, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vitality Down",
                    English = "Vitality Down",
                    French = "Malus de vitalité",
                    German = "Konstitutions-Malus",
                    Japanese = "ＶＩＴダウン",
                    Korean = "활력 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(53, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Damage Up",
                    English = "Physical Damage Up",
                    French = "Bonus de dégâts physiques",
                    German = "Schadenswert +",
                    Japanese = "物理ダメージ上昇",
                    Korean = "물리 공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(54, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Damage Down",
                    English = "Physical Damage Down",
                    French = "Malus de dégâts physiques",
                    German = "Schadenswert -",
                    Japanese = "物理ダメージ低下",
                    Korean = "물리 공격 피해량 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(55, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Vulnerability Down",
                    English = "Physical Vulnerability Down",
                    French = "Vulnérabilité physique diminuée",
                    German = "Verringerte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ軽減",
                    Korean = "물리 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(56, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Vulnerability Up",
                    English = "Physical Vulnerability Up",
                    French = "Vulnérabilité physique augmentée",
                    German = "Erhöhte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ増加",
                    Korean = "물리 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(57, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔法伤害上升",
                    English = "Magic Damage Up",
                    French = "Bonus de dégâts magiques",
                    German = "Magieschaden +",
                    Japanese = "魔法ダメージ上昇",
                    Korean = "마법 공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(58, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔法伤害下降",
                    English = "Magic Damage Down",
                    French = "Malus de dégâts magiques",
                    German = "Magieschaden -",
                    Japanese = "魔法ダメージ低下",
                    Korean = "마법 공격 피해량 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(59, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Magic Vulnerability Down",
                    English = "Magic Vulnerability Down",
                    French = "Vulnérabilité magique diminuée",
                    German = "Verringerte Magie-Verwundbarkeit",
                    Japanese = "被魔法ダメージ軽減",
                    Korean = "마법 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(60, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Magic Vulnerability Up",
                    English = "Magic Vulnerability Up",
                    French = "Vulnérabilité magique augmentée",
                    German = "Erhöhte Magie-Verwundbarkeit",
                    Japanese = "被魔法ダメージ増加",
                    Korean = "마법 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(61, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(62, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Down",
                    English = "Damage Down",
                    French = "Malus de dégâts",
                    German = "Schaden -",
                    Japanese = "ダメージ低下",
                    Korean = "공격 피해량 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(63, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vulnerability Down",
                    English = "Vulnerability Down",
                    French = "Vulnérabilité diminuée",
                    German = "Verringerte Verwundbarkeit",
                    Japanese = "被ダメージ低下",
                    Korean = "받는 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(64, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "受到的伤害提升",
                    English = "Vulnerability Up",
                    French = "Vulnérabilité augmentée",
                    German = "Erhöhte Verwundbarkeit",
                    Japanese = "被ダメージ上昇",
                    Korean = "받는 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(65, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Critical Skill",
                    English = "Critical Skill",
                    French = "Maîtrise critique",
                    German = "Kritisches Potenzial",
                    Japanese = "ウェポンスキル強化：クリティカル",
                    Korean = "무기 기술 강화: 극대화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(66, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "恐怖",
                    English = "Terror",
                    French = "Terreur",
                    German = "Terror",
                    Japanese = "恐怖",
                    Korean = "공포"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(67, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Leaden",
                    English = "Leaden",
                    French = "Plombé",
                    German = "Bleischwere",
                    Japanese = "ヘヴィ[強]",
                    Korean = "과중력[강]"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(68, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Drainstrikes",
                    English = "Drainstrikes",
                    French = "Coups drainants",
                    German = "Auszehren",
                    Japanese = "オートアタック強化：ＨＰ吸収",
                    Korean = "자동 공격 강화: HP 흡수"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(69, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aspirstrikes",
                    English = "Aspirstrikes",
                    French = "Coups aspirants",
                    German = "Auslaugen",
                    Japanese = "オートアタック強化：ＴＰ吸収",
                    Korean = "자동 공격 강화: TP 흡수"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(70, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stunstrikes",
                    English = "Stunstrikes",
                    French = "Coups étourdissants",
                    German = "Ausschalten",
                    Japanese = "オートアタック強化：スタン",
                    Korean = "자동 공격 강화: 기절"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(71, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rampart",
                    English = "Rampart",
                    French = "Rempart",
                    German = "Schutzwall",
                    Japanese = "ランパート",
                    Korean = "철벽 방어"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(72, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Convalescence",
                    English = "Convalescence",
                    French = "Convalescence",
                    German = "Konvaleszenz",
                    Japanese = "コンバレセンス",
                    Korean = "재활"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(73, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Awareness",
                    English = "Awareness",
                    French = "Diligence",
                    German = "Achtsamkeit",
                    Japanese = "アウェアネス",
                    Korean = "경각심"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(74, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sentinel",
                    English = "Sentinel",
                    French = "Sentinelle",
                    German = "Sentinel",
                    Japanese = "センチネル",
                    Korean = "경계"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(75, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "钢之意志",
                    English = "Tempered Will",
                    French = "Volonté d'acier",
                    German = "Eherner Wille",
                    Japanese = "鋼の意志",
                    Korean = "강철의 의지"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(76, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fight or Flight",
                    English = "Fight or Flight",
                    French = "Combat acharné",
                    German = "Verwegenheit",
                    Japanese = "ファイト・オア・フライト",
                    Korean = "임전무퇴"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(77, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bulwark",
                    English = "Bulwark",
                    French = "Forteresse",
                    German = "Bollwerk",
                    Japanese = "ブルワーク",
                    Korean = "방패 각성"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(78, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "忠义之剑",
                    English = "Sword Oath",
                    French = "Serment de l'épée",
                    German = "Schwert-Eid",
                    Japanese = "忠義の剣",
                    Korean = "충의의 검"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(79, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "忠义之盾",
                    English = "Shield Oath",
                    French = "Serment du bouclier",
                    German = "Schild-Eid",
                    Japanese = "忠義の盾",
                    Korean = "충의의 방패"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(80, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Cover",
                    English = "Cover",
                    French = "Couverture",
                    German = "Deckung",
                    Japanese = "かばう",
                    Korean = "감싸기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(81, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Covered",
                    English = "Covered",
                    French = "Couvert",
                    German = "Gedeckt",
                    Japanese = "かばう［被］",
                    Korean = "감싸기 대상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(82, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Hallowed Ground",
                    English = "Hallowed Ground",
                    French = "Invincible",
                    German = "Heiliger Boden",
                    Japanese = "インビンシブル",
                    Korean = "천하무적"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(83, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Foresight",
                    English = "Foresight",
                    French = "Aguet",
                    German = "Vorahnung",
                    Japanese = "フォーサイト",
                    Korean = "예지력"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(84, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bloodbath",
                    English = "Bloodbath",
                    French = "Bain de sang",
                    German = "Blutbad",
                    Japanese = "ブラッドバス",
                    Korean = "피의 갈증"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(85, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Maim",
                    English = "Maim",
                    French = "Mutilation",
                    German = "Verstümmelung",
                    Japanese = "メイム",
                    Korean = "관절 파괴"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(86, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Berserk",
                    English = "Berserk",
                    French = "Berserk",
                    German = "Tollwut",
                    Japanese = "バーサク",
                    Korean = "광폭화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(87, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thrill of Battle",
                    English = "Thrill of Battle",
                    French = "Frisson de la bataille",
                    German = "Kampfrausch",
                    Japanese = "スリル・オブ・バトル",
                    Korean = "전투의 짜릿함"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(88, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Holmgang",
                    English = "Holmgang",
                    French = "Holmgang",
                    German = "Holmgang",
                    Japanese = "ホルムギャング",
                    Korean = "일대일 결투"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(89, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vengeance",
                    English = "Vengeance",
                    French = "Représailles",
                    German = "Rache",
                    Japanese = "ヴェンジェンス",
                    Korean = "보복"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(90, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Storm's Eye",
                    English = "Storm's Eye",
                    French = "Œil de la tempête",
                    German = "Sturmbrecher",
                    Japanese = "シュトルムブレハ",
                    Korean = "태풍의 눈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(91, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Defiance",
                    English = "Defiance",
                    French = "Défi",
                    German = "Verteidiger",
                    Japanese = "ディフェンダー",
                    Korean = "수비 태세"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(92, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Unchained",
                    English = "Unchained",
                    French = "Affranchissement",
                    German = "Entfesselt",
                    Japanese = "アンチェインド",
                    Korean = "힘의 해방"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(93, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Wrath",
                    English = "Wrath",
                    French = "Rage",
                    German = "Zorn",
                    Japanese = "ラース",
                    Korean = "격노"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(94, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Wrath II",
                    English = "Wrath II",
                    French = "Rage II",
                    German = "Zorn II",
                    Japanese = "ラースII",
                    Korean = "격노 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(95, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Wrath III",
                    English = "Wrath III",
                    French = "Rage III",
                    German = "Zorn III",
                    Japanese = "ラースIII",
                    Korean = "격노 3"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(96, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Wrath IV",
                    English = "Wrath IV",
                    French = "Rage IV",
                    German = "Zorn IV",
                    Japanese = "ラースIV",
                    Korean = "격노 4"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(97, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Infuriated",
                    English = "Infuriated",
                    French = "Rage V",
                    German = "Zorn V",
                    Japanese = "ラースV",
                    Korean = "격노 5"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(98, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "双龙脚",
                    English = "Dragon Kick",
                    French = "Tacle du dragon",
                    German = "Drachentritt",
                    Japanese = "双竜脚",
                    Korean = "쌍룡각"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(99, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "飘羽步",
                    English = "Featherfoot",
                    French = "Pieds légers",
                    German = "Leichtfuß",
                    Japanese = "フェザーステップ",
                    Korean = "새털 걸음"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(100, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "发劲",
                    English = "Internal Release",
                    French = "Relâchement intérieur",
                    German = "Innere Gelöstheit",
                    Japanese = "発勁",
                    Korean = "발경"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(101, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "双掌打",
                    English = "Twin Snakes",
                    French = "Serpents jumeaux",
                    German = "Doppelviper",
                    Japanese = "双掌打",
                    Korean = "쌍장타"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(102, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "真言",
                    English = "Mantra",
                    French = "Mantra",
                    German = "Mantra",
                    Japanese = "マントラ",
                    Korean = "만트라"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(103, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "红莲体势",
                    English = "Fists of Fire",
                    French = "Poings de feu",
                    German = "Sengende Aura",
                    Japanese = "紅蓮の構え",
                    Korean = "홍련 태세"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(104, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "金刚体势",
                    English = "Fists of Earth",
                    French = "Poings de terre",
                    German = "Steinerne Aura",
                    Japanese = "金剛の構え",
                    Korean = "금강 태세"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(105, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "疾风体势",
                    English = "Fists of Wind",
                    French = "Poings de vent",
                    German = "Beflügelnde Aura",
                    Japanese = "疾風の構え",
                    Korean = "질풍 태세"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(106, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "秘孔拳",
                    English = "Touch of Death",
                    French = "Toucher mortel",
                    German = "Hauch des Todes",
                    Japanese = "秘孔拳",
                    Korean = "혈도 찌르기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(107, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔猿身形",
                    English = "Opo-opo Form",
                    French = "Posture de l'opo-opo",
                    German = "Opo-Opo-Form",
                    Japanese = "壱の型：魔猿",
                    Korean = "원숭이 품새"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(108, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Raptor Form",
                    English = "Raptor Form",
                    French = "Posture du raptor",
                    German = "Raptor-Form",
                    Japanese = "弐の型：走竜",
                    Korean = "용 품새"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(109, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Coeurl Form",
                    English = "Coeurl Form",
                    French = "Posture du coeurl",
                    German = "Coeurl-Form",
                    Japanese = "参の型：猛虎",
                    Korean = "호랑이 품새"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(110, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Perfect Balance",
                    English = "Perfect Balance",
                    French = "Équilibre parfait",
                    German = "Improvisation",
                    Japanese = "踏鳴",
                    Korean = "진각"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(111, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "疾风迅雷",
                    English = "Greased Lightning",
                    French = "Vitesse de l'éclair",
                    German = "Geölter Blitz",
                    Japanese = "疾風迅雷",
                    Korean = "질풍번개"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(112, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "疾风迅雷 II",
                    English = "Greased Lightning II",
                    French = "Vitesse de l'éclair II",
                    German = "Geölter Blitz II",
                    Japanese = "疾風迅雷II",
                    Korean = "질풍번개 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(113, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "疾风迅雷 III",
                    English = "Greased Lightning III",
                    French = "Vitesse de l'éclair III",
                    German = "Geölter Blitz III",
                    Japanese = "疾風迅雷III",
                    Korean = "질풍번개 3"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(114, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Keen Flurry",
                    English = "Keen Flurry",
                    French = "Volée défensive",
                    German = "Auge des Sturms",
                    Japanese = "キーンフラーリ",
                    Korean = "날카로운 돌풍"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(115, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Heavy Thrust",
                    English = "Heavy Thrust",
                    French = "Percée puissante",
                    German = "Gewaltiger Stoß",
                    Japanese = "ヘヴィスラスト",
                    Korean = "겹찌르기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(116, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Life Surge",
                    English = "Life Surge",
                    French = "Souffle de vie",
                    German = "Vitalwallung",
                    Japanese = "ライフサージ",
                    Korean = "생명력 쇄도"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(117, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "舍身",
                    English = "Blood for Blood",
                    French = "Du sang pour du sang",
                    German = "Zahn um Zahn",
                    Japanese = "捨身",
                    Korean = "필사의 각오"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(118, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "樱花怒放",
                    English = "Chaos Thrust",
                    French = "Percée chaotique",
                    German = "Chaotischer Tjost",
                    Japanese = "桜華狂咲",
                    Korean = "꽃잎 폭풍"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(119, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "二段突刺",
                    English = "Phlebotomize",
                    French = "Double percée",
                    German = "Phlebotomie",
                    Japanese = "二段突き",
                    Korean = "이단 찌르기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(120, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Power Surge",
                    English = "Power Surge",
                    French = "Souffle de puissance",
                    German = "Drachenklaue",
                    Japanese = "竜槍",
                    Korean = "용의 창"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(121, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Disembowel",
                    English = "Disembowel",
                    French = "Éventration",
                    German = "Drachengriff",
                    Japanese = "ディセムボウル",
                    Korean = "몸통 가르기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(122, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Straighter Shot",
                    English = "Straighter Shot",
                    French = "Tir à l'arc surpuissant",
                    German = "Direkter Schuss +",
                    Japanese = "ストレートショット効果アップ",
                    Korean = "직선 사격 효과 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(123, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "鹰眼",
                    English = "Hawk's Eye",
                    French = "Œil de faucon",
                    German = "Falkenauge",
                    Japanese = "ホークアイ",
                    Korean = "매의 눈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(124, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "毒咬箭",
                    English = "Venomous Bite",
                    French = "Morsure venimeuse",
                    German = "Infizierte Wunde",
                    Japanese = "ベノムバイト",
                    Korean = "독화살"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(125, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "猛者强击",
                    English = "Raging Strikes",
                    French = "Tir furieux",
                    German = "Wütende Attacke",
                    Japanese = "猛者の撃",
                    Korean = "용맹한 사격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(126, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Vulnerability Up",
                    English = "Physical Vulnerability Up",
                    French = "Vulnérabilité physique augmentée",
                    German = "Erhöhte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ増加",
                    Korean = "물리 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(127, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "静者强击",
                    English = "Quelling Strikes",
                    French = "Frappe silencieuse",
                    German = "Heimliche Attacke",
                    Japanese = "静者の撃",
                    Korean = "고요한 사격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(128, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "纷乱箭",
                    English = "Barrage",
                    French = "Rafale de coups",
                    German = "Sperrfeuer",
                    Japanese = "乱れ撃ち",
                    Korean = "다중 사격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(129, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "风蚀箭",
                    English = "Windbite",
                    French = "Morsure du vent",
                    German = "Beißender Wind",
                    Japanese = "ウィンドバイト",
                    Korean = "바람 화살"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(130, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "直线射击",
                    English = "Straight Shot",
                    French = "Tir droit",
                    German = "Direkter Schuss",
                    Japanese = "ストレートショット",
                    Korean = "직선 사격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(131, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Downpour of Death",
                    English = "Downpour of Death",
                    French = "Déluge mortel",
                    German = "Tödlicher Regen +",
                    Japanese = "レイン・オブ・デス効果アップ",
                    Korean = "죽음의 화살비 효과 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(132, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Quicker Nock",
                    English = "Quicker Nock",
                    French = "Salve fulgurante améliorée",
                    German = "Pfeilsalve +",
                    Japanese = "クイックノック効果アップ",
                    Korean = "재빠른 활시위 효과 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(133, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Swiftsong",
                    English = "Swiftsong",
                    French = "Chant rapide",
                    German = "Schmissiger Song",
                    Japanese = "スウィフトソング",
                    Korean = "질주의 노래"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(134, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Swiftsong",
                    English = "Swiftsong",
                    French = "Chant rapide",
                    German = "Schmissiger Song",
                    Japanese = "スウィフトソング：効果",
                    Korean = "질주의 노래"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(135, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mage's Ballad",
                    English = "Mage's Ballad",
                    French = "Ballade du mage",
                    German = "Ballade des Weisen",
                    Japanese = "賢人のバラード",
                    Korean = "현자의 담시곡"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(136, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mage's Ballad",
                    English = "Mage's Ballad",
                    French = "Ballade du mage",
                    German = "Ballade des Weisen",
                    Japanese = "賢人のバラード：効果",
                    Korean = "현자의 담시곡"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(137, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Army's Paeon",
                    English = "Army's Paeon",
                    French = "Péan martial",
                    German = "Hymne der Krieger",
                    Japanese = "軍神のパイオン",
                    Korean = "군신의 찬가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(138, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Army's Paeon",
                    English = "Army's Paeon",
                    French = "Péan martial",
                    German = "Hymne der Krieger",
                    Japanese = "軍神のパイオン：効果",
                    Korean = "군신의 찬가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(139, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Foe Requiem",
                    English = "Foe Requiem",
                    French = "Requiem ennemi",
                    German = "Requiem der Feinde",
                    Japanese = "魔人のレクイエム",
                    Korean = "마인의 진혼곡"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(140, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Foe Requiem",
                    English = "Foe Requiem",
                    French = "Requiem ennemi",
                    German = "Requiem der Feinde",
                    Japanese = "魔人のレクイエム：効果",
                    Korean = "마인의 진혼곡"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(141, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Battle Voice",
                    English = "Battle Voice",
                    French = "Voix de combat",
                    German = "Ode an die Seele",
                    Japanese = "バトルボイス",
                    Korean = "전장의 노래"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(142, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Chameleon",
                    English = "Chameleon",
                    French = "Caméléon",
                    German = "Chamäleon",
                    Japanese = "カメレオン",
                    Korean = "카멜레온"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(143, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "疾风",
                    English = "Aero",
                    French = "Vent",
                    German = "Wind",
                    Japanese = "エアロ",
                    Korean = "에어로"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(144, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "烈风",
                    English = "Aero II",
                    French = "Extra Vent",
                    German = "Windra",
                    Japanese = "エアロラ",
                    Korean = "에어로라"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(145, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "站姿",
                    English = "Cleric Stance",
                    French = "Prestance du prêtre",
                    German = "Bußprediger",
                    Japanese = "クルセードスタンス",
                    Korean = "성전 태세"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(146, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Protect",
                    English = "Protect",
                    French = "Bouclier",
                    German = "Protes",
                    Japanese = "プロテス",
                    Korean = "프로테스"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(147, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Protect",
                    English = "Protect",
                    French = "Bouclier",
                    German = "Protes",
                    Japanese = "プロテス",
                    Korean = "프로테스"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(148, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Raise",
                    English = "Raise",
                    French = "Vie",
                    German = "Wiederbeleben",
                    Japanese = "蘇生",
                    Korean = "부활"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(149, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stun",
                    English = "Stun",
                    French = "Étourdissement",
                    German = "Betäubung",
                    Japanese = "スタン",
                    Korean = "기절"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(150, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Medica II",
                    English = "Medica II",
                    French = "Extra Médica",
                    German = "Resedra",
                    Japanese = "メディカラ",
                    Korean = "메디카라"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(151, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stoneskin",
                    English = "Stoneskin",
                    French = "Cuirasse",
                    German = "Steinhaut",
                    Japanese = "ストンスキン",
                    Korean = "스톤스킨"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(152, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stoneskin (Physical)",
                    English = "Stoneskin (Physical)",
                    French = "Cuirasse (physique)",
                    German = "Steinhaut (physisch)",
                    Japanese = "ストンスキン（物理攻撃）",
                    Korean = "스톤스킨(물리 공격)"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(153, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stoneskin (Magical)",
                    English = "Stoneskin (Magical)",
                    French = "Cuirasse (magique)",
                    German = "Steinhaut (magisch)",
                    Japanese = "ストンスキン（魔法攻撃）",
                    Korean = "스톤스킨(마법 공격)"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(154, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Shroud of Saints",
                    English = "Shroud of Saints",
                    French = "Voile des saints",
                    German = "Fispelstimme",
                    Japanese = "女神の加護",
                    Korean = "여신의 가호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(155, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Freecure",
                    English = "Freecure",
                    French = "Extra Soin amélioré",
                    German = "Vitra +",
                    Japanese = "ケアルラ効果アップ",
                    Korean = "케알라 효과 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(156, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Overcure",
                    English = "Overcure",
                    French = "Méga Soin amélioré",
                    German = "Vitaga +",
                    Japanese = "ケアルガ効果アップ",
                    Korean = "케알가 효과 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(157, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Presence of Mind",
                    English = "Presence of Mind",
                    French = "Présence d'esprit",
                    German = "Geistesgegenwart",
                    Japanese = "神速魔",
                    Korean = "쾌속의 마법"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(158, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Regen",
                    English = "Regen",
                    French = "Récup",
                    German = "Regena",
                    Japanese = "リジェネ",
                    Korean = "리제네"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(159, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Divine Seal",
                    English = "Divine Seal",
                    French = "Sceau divin",
                    German = "Göttliches Siegel",
                    Japanese = "ディヴァインシール",
                    Korean = "신성한 문장"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(160, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Surecast",
                    English = "Surecast",
                    French = "Stoïcisme",
                    German = "Unbeirrbarkeit",
                    Japanese = "堅実魔",
                    Korean = "견고한 마법"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(161, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "闪雷",
                    English = "Thunder",
                    French = "Foudre",
                    German = "Blitz",
                    Japanese = "サンダー",
                    Korean = "선더"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(162, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "震雷",
                    English = "Thunder II",
                    French = "Extra Foudre",
                    German = "Blitzra",
                    Japanese = "サンダラ",
                    Korean = "선더라"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(163, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "暴雷",
                    English = "Thunder III",
                    French = "Méga Foudre",
                    German = "Blitzga",
                    Japanese = "サンダガ",
                    Korean = "선더가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(164, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thundercloud",
                    English = "Thundercloud",
                    French = "Nuage d'orage",
                    German = "Blitz +",
                    Japanese = "サンダー系魔法効果アップ",
                    Korean = "선더 계열 마법 효과 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(165, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Firestarter",
                    English = "Firestarter",
                    French = "Pyromane",
                    German = "Feuga +",
                    Japanese = "ファイガ効果アップ",
                    Korean = "파이가 효과 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(166, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Succor",
                    English = "Succor",
                    French = "Dogme de survie",
                    German = "Kurieren +",
                    Japanese = "士気高揚の策効果アップ",
                    Korean = "사기고양책 효과 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(167, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Swiftcast",
                    English = "Swiftcast",
                    French = "Magie prompte",
                    German = "Spontaneität",
                    Japanese = "迅速魔",
                    Korean = "신속한 마법"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(168, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Manaward",
                    English = "Manaward",
                    French = "Barrière de mana",
                    German = "Mana-Schild",
                    Japanese = "マバリア",
                    Korean = "마배리어"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(169, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Manawall",
                    English = "Manawall",
                    French = "Mur de mana",
                    German = "Mana-Wand",
                    Japanese = "ウォール",
                    Korean = "마법 장벽"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(170, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Apocatastasis",
                    English = "Apocatastasis",
                    French = "Apocatastase",
                    German = "Apokatastasis",
                    Japanese = "アポカタスタシス",
                    Korean = "구원"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(171, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Ekpyrosis",
                    English = "Ekpyrosis",
                    French = "Ekpyrosis",
                    German = "Ekpyrosis",
                    Japanese = "アポカタスタシス不可",
                    Korean = "구원 불가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(172, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Infirmity",
                    English = "Infirmity",
                    French = "Infirmité",
                    German = "Gebrechlichkeit",
                    Japanese = "虚弱",
                    Korean = "허약"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(173, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Astral Fire",
                    English = "Astral Fire",
                    French = "Feu astral",
                    German = "Lichtfeuer",
                    Japanese = "アストラルファイア",
                    Korean = "천상의 화염"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(174, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Astral Fire II",
                    English = "Astral Fire II",
                    French = "Feu astral II",
                    German = "Lichtfeuer II",
                    Japanese = "アストラルファイアII",
                    Korean = "천상의 화염 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(175, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Astral Fire III",
                    English = "Astral Fire III",
                    French = "Feu astral III",
                    German = "Lichtfeuer III",
                    Japanese = "アストラルファイアIII",
                    Korean = "천상의 화염 3"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(176, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Umbral Ice",
                    English = "Umbral Ice",
                    French = "Glace ombrale",
                    German = "Schatteneis",
                    Japanese = "アンブラルブリザード",
                    Korean = "저승의 냉기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(177, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Umbral Ice II",
                    English = "Umbral Ice II",
                    French = "Glace ombrale II",
                    German = "Schatteneis II",
                    Japanese = "アンブラルブリザードII",
                    Korean = "저승의 냉기 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(178, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Umbral Ice III",
                    English = "Umbral Ice III",
                    French = "Glace ombrale III",
                    German = "Schatteneis III",
                    Japanese = "アンブラルブリザードIII",
                    Korean = "저승의 냉기 3"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(179, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "毒菌",
                    English = "Bio",
                    French = "Bactérie",
                    German = "Bio",
                    Japanese = "バイオ",
                    Korean = "바이오"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(180, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "瘴气",
                    English = "Miasma",
                    French = "Miasmes",
                    German = "Miasma",
                    Japanese = "ミアズマ",
                    Korean = "미아즈마"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(181, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "病弱",
                    English = "Disease",
                    French = "Maladie",
                    German = "Krankheit",
                    Japanese = "病気",
                    Korean = "질병"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(182, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "病毒",
                    English = "Virus",
                    French = "Virus",
                    German = "Virus",
                    Japanese = "ウイルス",
                    Korean = "바이러스"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(183, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fever",
                    English = "Fever",
                    French = "Virus de l'esprit",
                    German = "Geistesvirus",
                    Japanese = "マインドウイルス",
                    Korean = "정신 바이러스"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(184, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sustain",
                    English = "Sustain",
                    French = "Transfusion",
                    German = "Erhaltung",
                    Japanese = "サステイン",
                    Korean = "독려"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(185, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Eye for an Eye",
                    English = "Eye for an Eye",
                    French = "Œil pour œil",
                    German = "Auge um Auge",
                    Japanese = "アイ・フォー・アイ",
                    Korean = "눈에는 눈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(186, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Eye for an Eye",
                    English = "Eye for an Eye",
                    French = "Garde-corps",
                    German = "Auge um Auge",
                    Japanese = "アイ・フォー・アイ：効果",
                    Korean = "눈에는 눈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(187, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rouse",
                    English = "Rouse",
                    French = "Stimulation",
                    German = "Aufmuntern",
                    Japanese = "ラウズ",
                    Korean = "각성"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(188, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "瘴疠",
                    English = "Miasma II",
                    French = "Extra Miasmes",
                    German = "Miasra",
                    Japanese = "ミアズラ",
                    Korean = "미아즈라"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(189, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "猛毒菌",
                    English = "Bio II",
                    French = "Extra Bactérie",
                    German = "Biora",
                    Japanese = "バイオラ",
                    Korean = "바이오라"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(190, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Shadow Flare",
                    English = "Shadow Flare",
                    French = "Éruption ténébreuse",
                    German = "Schattenflamme",
                    Japanese = "シャドウフレア",
                    Korean = "섀도우 플레어"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(191, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Malady",
                    French = "Infection",
                    German = "Pestilenz",
                    Japanese = "瘴気"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(192, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "鼓舞",
                    English = "Spur",
                    French = "Encouragement",
                    German = "Ansporn",
                    Japanese = "スパー",
                    Korean = "자극"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(193, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "减速",
                    English = "Slow",
                    French = "Lenteur",
                    German = "Gemach",
                    Japanese = "スロウ",
                    Korean = "둔화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(194, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Shield Wall",
                    English = "Shield Wall",
                    French = "Mur protecteur",
                    German = "Schutzschild",
                    Japanese = "シールドウォール",
                    Korean = "방패의 벽"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(195, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mighty Guard",
                    English = "Mighty Guard",
                    French = "Garde puissante",
                    German = "Totalabwehr",
                    Japanese = "マイティガード",
                    Korean = "강력 방어"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(196, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Last Bastion",
                    English = "Last Bastion",
                    French = "Dernier bastion",
                    German = "Letzte Bastion",
                    Japanese = "ラストバスティオン",
                    Korean = "철벽 요새"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(197, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Blaze Spikes",
                    English = "Blaze Spikes",
                    French = "Pointes de feu",
                    German = "Feuerstachel",
                    Japanese = "ブレイズスパイク",
                    Korean = "화염 보호막"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(198, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Ice Spikes",
                    English = "Ice Spikes",
                    French = "Pointes de glace",
                    German = "Eisstachel",
                    Japanese = "アイススパイク",
                    Korean = "얼음 보호막"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(199, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Shock Spikes",
                    English = "Shock Spikes",
                    French = "Pointes de foudre",
                    German = "Schockstachel",
                    Japanese = "ショックスパイク",
                    Korean = "번개 보호막"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(200, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Vulnerability Up",
                    English = "Physical Vulnerability Up",
                    French = "Vulnérabilité physique augmentée",
                    German = "Erhöhte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ増加",
                    Korean = "물리 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(201, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stun",
                    English = "Stun",
                    French = "Étourdissement",
                    German = "Betäubung",
                    Japanese = "スタン",
                    Korean = "기절"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(202, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vulnerability Up",
                    English = "Vulnerability Up",
                    French = "Vulnérabilité augmentée",
                    German = "Erhöhte Verwundbarkeit",
                    Japanese = "被ダメージ上昇",
                    Korean = "받는 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(203, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Boost",
                    English = "Boost",
                    French = "Accumulation",
                    German = "Akkumulation",
                    Japanese = "力溜め",
                    Korean = "힘 모으기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(204, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Enfire",
                    English = "Enfire",
                    French = "EndoFeu",
                    German = "Runenwaffe: Feuer",
                    Japanese = "魔法剣・火",
                    Korean = "마법검: 불"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(205, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Enblizzard",
                    English = "Enblizzard",
                    French = "EndoGlace",
                    German = "Runenwaffe: Eis",
                    Japanese = "魔法剣・氷",
                    Korean = "마법검: 얼음"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(206, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Enaero",
                    English = "Enaero",
                    French = "EndoVent",
                    German = "Runenwaffe: Wind",
                    Japanese = "魔法剣・風",
                    Korean = "마법검: 바람"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(207, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Enstone",
                    English = "Enstone",
                    French = "EndoPierre",
                    German = "Runenwaffe: Erde",
                    Japanese = "魔法剣・土",
                    Korean = "마법검: 땅"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(208, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Enthunder",
                    English = "Enthunder",
                    French = "EndoFoudre",
                    German = "Runenwaffe: Blitz",
                    Japanese = "魔法剣・雷",
                    Korean = "마법검: 번개"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(209, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Enwater",
                    English = "Enwater",
                    French = "EndoEau",
                    German = "Runenwaffe: Wasser",
                    Japanese = "魔法剣・水",
                    Korean = "마법검: 물"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(210, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Doom",
                    English = "Doom",
                    French = "Glas",
                    German = "Todesurteil",
                    Japanese = "死の宣告",
                    Korean = "죽음의 선고"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(211, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sharpened Knife",
                    English = "Sharpened Knife",
                    French = "Couteau aiguisé",
                    German = "Gewetztes Messer",
                    Japanese = "研がれた包丁",
                    Korean = "잘 갈린 식칼"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(212, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "True Sight",
                    English = "True Sight",
                    French = "Vision véritable",
                    German = "Wahre Gestalt",
                    Japanese = "見破り",
                    Korean = "간파"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(213, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "怀柔状态",
                    English = "Pacification",
                    French = "Tranquillisation",
                    German = "Besänftigung",
                    Japanese = "懐柔状態",
                    Korean = "회유 상태"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(214, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Agitation",
                    English = "Agitation",
                    French = "Énervement",
                    German = "Aufstachelung",
                    Japanese = "懐柔失敗",
                    Korean = "회유 실패"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(215, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Down",
                    English = "Damage Down",
                    French = "Malus de dégâts",
                    German = "Schaden -",
                    Japanese = "ダメージ低下",
                    Korean = "공격 피해량 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(216, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "麻痹",
                    English = "Paralysis",
                    French = "Paralysie",
                    German = "Paralyse",
                    Japanese = "麻痺",
                    Korean = "마비"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(217, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "三角测量",
                    English = "Triangulate",
                    French = "Forestier",
                    German = "Geodäsie",
                    Japanese = "トライアングレート",
                    Korean = "원예가의 눈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(218, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "采集获得率提升",
                    English = "Gathering Rate Up",
                    French = "Récolte améliorée",
                    German = "Sammelrate erhöht",
                    Japanese = "採集獲得率アップ",
                    Korean = "채집 획득률 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(219, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "采集获得数增加",
                    English = "Gathering Yield Up",
                    French = "Récolte abondante",
                    German = "Sammelgewinn erhöht",
                    Japanese = "採集獲得数アップ",
                    Korean = "채집 획득 수 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(220, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Gathering Fortune Up",
                    English = "Gathering Fortune Up",
                    French = "Récolte de qualité",
                    German = "Sammelglück erhöht",
                    Japanese = "採集HQ獲得率アップ",
                    Korean = "채집 HQ 획득률 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(221, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Truth of Forests",
                    English = "Truth of Forests",
                    French = "Science des végétaux",
                    German = "Flurenthüllung",
                    Japanese = "トゥルー・オブ・フォレスト",
                    Korean = "숲의 탐구자"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(222, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Truth of Mountains",
                    English = "Truth of Mountains",
                    French = "Science des minéraux",
                    German = "Tellurische Enthüllung",
                    Japanese = "トゥルー・オブ・ミネラル",
                    Korean = "광산의 탐구자"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(223, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Byregot's Ward",
                    English = "Byregot's Ward",
                    French = "Grâce de Byregot",
                    German = "Byregots Segen",
                    Japanese = "ビエルゴの加護",
                    Korean = "비레고의 가호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(224, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "诺菲卡的加护",
                    English = "Nophica's Ward",
                    French = "Grâce de Nophica",
                    German = "Nophicas Segen",
                    Japanese = "ノフィカの加護",
                    Korean = "노피카의 가호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(225, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Prospect",
                    English = "Prospect",
                    French = "Prospecteur",
                    German = "Prospektion",
                    Japanese = "プロスペクト",
                    Korean = "광부의 눈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(226, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "加速",
                    English = "Haste",
                    French = "Hâte",
                    German = "Hast",
                    Japanese = "ヘイスト",
                    Korean = "헤이스트"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(227, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Seduced",
                    English = "Seduced",
                    French = "Séduction",
                    German = "Versuchung",
                    Japanese = "誘惑",
                    Korean = "유혹"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(228, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Menphina's Ward",
                    English = "Menphina's Ward",
                    French = "Grâce de Menphina",
                    German = "Menphinas Segen",
                    Japanese = "メネフィナの加護",
                    Korean = "메느피나의 가호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(229, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Nald'thal's Ward",
                    English = "Nald'thal's Ward",
                    French = "Grâce de Nald'thal",
                    German = "Nald'thals Segen",
                    Japanese = "ナルザルの加護",
                    Korean = "날달의 가호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(230, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Llymlaen's Ward",
                    English = "Llymlaen's Ward",
                    French = "Grâce de Llymlaen",
                    German = "Llymlaens Segen",
                    Japanese = "リムレーンの加護",
                    Korean = "리믈렌의 가호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(231, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thaliak's Ward",
                    English = "Thaliak's Ward",
                    French = "Grâce de Thaliak",
                    German = "Thaliaks Segen",
                    Japanese = "サリャクの加護",
                    Korean = "살리아크의 가호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(232, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Preparation",
                    English = "Preparation",
                    French = "Préparation",
                    German = "Vorausplanung",
                    Japanese = "プレパレーション",
                    Korean = "통찰"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(233, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Arbor Call",
                    English = "Arbor Call",
                    French = "Dendrologie",
                    German = "Ruf des Waldes",
                    Japanese = "アーバーコール",
                    Korean = "숲의 부름"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(234, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Lay of the Land",
                    English = "Lay of the Land",
                    French = "Géologie",
                    German = "Bodenbefund",
                    Japanese = "ランドサーベイ",
                    Korean = "바위의 부름"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(235, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Windburn",
                    English = "Windburn",
                    French = "Brûlure du vent",
                    German = "Beißender Wind",
                    Japanese = "裂傷",
                    Korean = "열상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(236, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "陆行鸟猛啄",
                    English = "Choco Beak",
                    French = "Choco-bec",
                    German = "Chocobo-Schnabel",
                    Japanese = "チョコビーク",
                    Korean = "초코 쪼기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(237, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "陆行鸟再生",
                    English = "Choco Regen",
                    French = "Choco-récup",
                    German = "Chocobo-Regena",
                    Japanese = "チョコリジェネ",
                    Korean = "초코 리제네"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(238, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Choco Surge",
                    English = "Choco Surge",
                    French = "Choco-ardeur",
                    German = "Chocobo-Quelle",
                    Japanese = "チョコサージ",
                    Korean = "초코 쇄도"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(239, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "The Echo",
                    English = "The Echo",
                    French = "L'Écho",
                    German = "Kraft des Transzendierens",
                    Japanese = "超える力",
                    Korean = "초월하는 힘"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(240, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Heavy",
                    English = "Heavy",
                    French = "Pesanteur",
                    German = "Gewicht",
                    Japanese = "ヘヴィ",
                    Korean = "과중력"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(241, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Blessing of Light",
                    English = "Blessing of Light",
                    French = "Bénédiction de la Lumière",
                    German = "Gnade des Lichts",
                    Japanese = "光の加護",
                    Korean = "빛의 가호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(242, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Arbor Call II",
                    English = "Arbor Call II",
                    French = "Dendrologie II",
                    German = "Ruf des Waldes II",
                    Japanese = "アーバーコールII",
                    Korean = "숲의 부름 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(243, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Lay of the Land II",
                    English = "Lay of the Land II",
                    French = "Géologie II",
                    German = "Bodenbefund II",
                    Japanese = "ランドサーベイII",
                    Korean = "바위의 부름 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(244, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "碎骨打",
                    English = "Fracture",
                    French = "Fracture",
                    German = "Knochenbrecher",
                    Japanese = "フラクチャー",
                    Korean = "골절"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(245, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sanction",
                    English = "Sanction",
                    French = "Sanction",
                    German = "Ermächtigung",
                    Japanese = "サンクション",
                    Korean = "강화 승인"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(246, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "破碎拳",
                    English = "Demolish",
                    French = "Démolition",
                    German = "Demolieren",
                    Japanese = "破砕拳",
                    Korean = "파쇄권"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(247, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rain of Death",
                    English = "Rain of Death",
                    French = "Pluie mortelle",
                    German = "Tödlicher Regen",
                    Japanese = "レイン・オブ・デス",
                    Korean = "죽음의 화살비"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(248, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "厄运流转",
                    English = "Circle of Scorn",
                    French = "Cercle du destin",
                    German = "Kreis der Verachtung",
                    Japanese = "サークル・オブ・ドゥーム",
                    Korean = "파멸의 진"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(249, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Flaming Arrow",
                    English = "Flaming Arrow",
                    French = "Flèche enflammée",
                    German = "Flammenpfeil",
                    Japanese = "フレイミングアロー",
                    Korean = "불타는 화살"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(250, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Burns",
                    English = "Burns",
                    French = "Brûlure",
                    German = "Brandwunde",
                    Japanese = "火傷",
                    Korean = "화상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(251, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Inner Quiet",
                    English = "Inner Quiet",
                    French = "Calme intérieur",
                    German = "Innere Ruhe",
                    Japanese = "インナークワイエット",
                    Korean = "정신 집중"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(252, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Waste Not",
                    English = "Waste Not",
                    French = "Parcimonie",
                    German = "Nachhaltigkeit",
                    Japanese = "倹約",
                    Korean = "근검절약"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(253, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Steady Hand",
                    English = "Steady Hand",
                    French = "Main sûre",
                    German = "Ruhige Hand",
                    Japanese = "ステディハンド",
                    Korean = "안정된 솜씨"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(254, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Great Strides",
                    English = "Great Strides",
                    French = "Grands progrès",
                    German = "Große Schritte",
                    Japanese = "グレートストライド",
                    Korean = "장족의 발전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(255, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Ingenuity",
                    English = "Ingenuity",
                    French = "Ingéniosité",
                    German = "Einfallsreichtum",
                    Japanese = "工面算段",
                    Korean = "독창적 발상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(256, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "加工 II",
                    English = "Ingenuity II",
                    French = "Ingéniosité II",
                    German = "Einfallsreichtum II",
                    Japanese = "工面算段II",
                    Korean = "독창적 발상 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(257, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "俭约 II",
                    English = "Waste Not II",
                    French = "Parcimonie II",
                    German = "Nachhaltigkeit II",
                    Japanese = "倹約II",
                    Korean = "근검절약 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(258, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Manipulation",
                    English = "Manipulation",
                    French = "Manipulation",
                    German = "Manipulation",
                    Japanese = "マニピュレーション",
                    Korean = "교묘한 손놀림"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(259, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "新颖",
                    English = "Innovation",
                    French = "Innovation",
                    German = "Innovation",
                    Japanese = "イノベーション",
                    Korean = "혁신"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(260, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Reclaim",
                    English = "Reclaim",
                    French = "Récupération",
                    German = "Reklamation",
                    Japanese = "リクレイム",
                    Korean = "재료 환원"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(261, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Comfort Zone",
                    English = "Comfort Zone",
                    French = "Zone de confort",
                    German = "Komfortzone",
                    Japanese = "コンファートゾーン",
                    Korean = "작업 요령"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(262, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Steady Hand II",
                    English = "Steady Hand II",
                    French = "Main sûre II",
                    German = "Ruhige Hand II",
                    Japanese = "ステディハンドII",
                    Korean = "안정된 솜씨 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(263, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(264, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Flesh Wound",
                    English = "Flesh Wound",
                    French = "Blessure physique",
                    German = "Fleischwunde",
                    Japanese = "切傷",
                    Korean = "절상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(265, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stab Wound",
                    English = "Stab Wound",
                    French = "Perforation",
                    German = "Stichwunde",
                    Japanese = "刺傷",
                    Korean = "자상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(266, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Concussion",
                    English = "Concussion",
                    French = "Concussion",
                    German = "Prellung",
                    Japanese = "打撲傷",
                    Korean = "타박상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(267, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Burns",
                    English = "Burns",
                    French = "Brûlure",
                    German = "Brandwunde",
                    Japanese = "火傷",
                    Korean = "화상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(268, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Frostbite",
                    English = "Frostbite",
                    French = "Gelure",
                    German = "Erfrierung",
                    Japanese = "凍傷",
                    Korean = "동상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(269, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Windburn",
                    English = "Windburn",
                    French = "Brûlure du vent",
                    German = "Beißender Wind",
                    Japanese = "裂傷",
                    Korean = "열상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(270, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sludge",
                    English = "Sludge",
                    French = "Emboué",
                    German = "Schlamm",
                    Japanese = "汚泥",
                    Korean = "진흙탕"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(271, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Electrocution",
                    English = "Electrocution",
                    French = "Électrocution",
                    German = "Stromschlag",
                    Japanese = "感電",
                    Korean = "감전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(272, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Dropsy",
                    English = "Dropsy",
                    French = "Œdème",
                    German = "Wassersucht",
                    Japanese = "水毒",
                    Korean = "물독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(273, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bleeding",
                    English = "Bleeding",
                    French = "Saignant",
                    German = "Blutung",
                    Japanese = "ペイン",
                    Korean = "고통"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(274, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Recuperation",
                    English = "Recuperation",
                    French = "Rétablissement",
                    German = "Segnung",
                    Japanese = "治癒",
                    Korean = "치유"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(275, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Poison +1",
                    English = "Poison +1",
                    French = "Poison",
                    German = "Gift +1",
                    Japanese = "毒",
                    Korean = "독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(276, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Voice of Valor",
                    English = "Voice of Valor",
                    French = "Voix de la valeur",
                    German = "Lob des Kämpen",
                    Japanese = "勇戦の誉れ：効果",
                    Korean = "용기의 영예"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(277, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Voice of Fortitude",
                    English = "Voice of Fortitude",
                    French = "Voix de la ténacité",
                    German = "Stimme der Stärke",
                    Japanese = "堅忍の誉れ：効果",
                    Korean = "인내의 영예"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(278, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Relentless March",
                    English = "Relentless March",
                    French = "Marche implacable",
                    German = "無敵の進撃マーチ：効果",
                    Japanese = "無敵の進撃マーチ：効果",
                    Korean = "무적의 진격 행진곡"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(279, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rehabilitation",
                    English = "Rehabilitation",
                    French = "Recouvrement",
                    German = "Rehabilitation",
                    Japanese = "徐々にＨＰ回復",
                    Korean = "서서히 HP 회복"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(280, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bind",
                    English = "Bind",
                    French = "Entrave",
                    German = "Fessel",
                    Japanese = "バインド",
                    Korean = "속박"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(281, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Damage Down",
                    English = "Physical Damage Down",
                    French = "Malus de dégâts physiques",
                    German = "Schadenswert -",
                    Japanese = "物理ダメージ低下",
                    Korean = "물리 공격 피해량 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(282, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mana Modulation",
                    English = "Mana Modulation",
                    French = "Anormalité magique",
                    German = "Magieschaden -",
                    Japanese = "魔力変調",
                    Korean = "마력 변조"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(283, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "水毒",
                    English = "Dropsy",
                    French = "Œdème",
                    German = "Wassersucht",
                    Japanese = "水毒",
                    Korean = "물독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(284, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "火伤",
                    English = "Burns",
                    French = "Brûlure",
                    German = "Brandwunde",
                    Japanese = "火傷",
                    Korean = "화상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(285, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "冻伤",
                    English = "Frostbite",
                    French = "Gelure",
                    German = "Erfrierung",
                    Japanese = "凍傷",
                    Korean = "동상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(286, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "裂伤",
                    English = "Windburn",
                    French = "Brûlure du vent",
                    German = "Beißender Wind",
                    Japanese = "裂傷",
                    Korean = "열상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(287, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "污泥",
                    English = "Sludge",
                    French = "Emboué",
                    German = "Schlamm",
                    Japanese = "汚泥",
                    Korean = "진흙탕"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(288, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "感电",
                    English = "Electrocution",
                    French = "Électrocution",
                    German = "Stromschlag",
                    Japanese = "感電",
                    Korean = "감전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(289, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Dropsy",
                    English = "Dropsy",
                    French = "Œdème",
                    German = "Wassersucht",
                    Japanese = "水毒",
                    Korean = "물독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(290, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(291, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "百烈拳",
                    English = "Hundred Fists",
                    French = "Cent poings",
                    German = "100 Fäuste",
                    Japanese = "百烈拳",
                    Korean = "백렬권"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(292, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fetters",
                    English = "Fetters",
                    French = "Attache",
                    German = "Granitgefängnis",
                    Japanese = "拘束",
                    Korean = "구속"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(293, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "技能速度提升",
                    English = "Skill Speed Up",
                    French = "Bonus de vivacité",
                    German = "Schnelligkeit +",
                    Japanese = "スキルスピード上昇",
                    Korean = "기술 시전 속도 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(294, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Spell Speed Up",
                    English = "Spell Speed Up",
                    French = "Bonus de célérité",
                    German = "Zaubertempo +",
                    Japanese = "スペルスピード上昇",
                    Korean = "마법 시전 속도 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(295, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Goldbile",
                    English = "Goldbile",
                    French = "Eau bilieuse",
                    German = "Goldlunge",
                    Japanese = "黄毒沼",
                    Korean = "금빛 독늪"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(296, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Hysteria",
                    English = "Hysteria",
                    French = "Hystérie",
                    German = "Panik",
                    Japanese = "恐慌",
                    Korean = "공황"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(297, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Galvanize",
                    English = "Galvanize",
                    French = "Traité du réconfort",
                    German = "Adloquium",
                    Japanese = "鼓舞",
                    Korean = "격려"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(298, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sacred Soil",
                    English = "Sacred Soil",
                    French = "Dogme de survie",
                    German = "Geweihte Erde",
                    Japanese = "野戦治療の陣",
                    Korean = "야전치유진"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(299, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sacred Soil",
                    English = "Sacred Soil",
                    French = "Dogme de survie",
                    German = "Geweihte Erde",
                    Japanese = "野戦治療の陣：効果",
                    Korean = "야전치유진"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(300, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(301, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Critical Strikes",
                    English = "Critical Strikes",
                    French = "Coups critiques",
                    German = "Kritische Attacke",
                    Japanese = "クリティカル攻撃",
                    Korean = "극대화 공격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(302, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Gold Lung",
                    English = "Gold Lung",
                    French = "Poumons bilieux",
                    German = "Galle",
                    Japanese = "黄毒",
                    Korean = "금빛 독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(303, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Burrs",
                    English = "Burrs",
                    French = "Bardanes",
                    German = "Klettenpilz",
                    Japanese = "粘菌",
                    Korean = "점균"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(304, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aetherflow",
                    English = "Aetherflow",
                    French = "Flux d'éther",
                    German = "Ätherfluss",
                    Japanese = "エーテルフロー",
                    Korean = "에테르 순환"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(305, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "The Dragon's Curse",
                    English = "The Dragon's Curse",
                    French = "Malédiction du dragon",
                    German = "Bann des Drachen",
                    Japanese = "竜の呪縛",
                    Korean = "용의 저주"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(306, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Inner Dragon",
                    English = "Inner Dragon",
                    French = "Dragon intérieur",
                    German = "Kraft des Drachen",
                    Japanese = "竜の力",
                    Korean = "용의 힘"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(307, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Voice of Valor",
                    English = "Voice of Valor",
                    French = "Voix de la valeur",
                    German = "Lob des Kämpen",
                    Japanese = "勇戦の誉れ",
                    Korean = "용기의 영예"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(308, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Voice of Fortitude",
                    English = "Voice of Fortitude",
                    French = "Voix de la ténacité",
                    German = "Stimme der Stärke",
                    Japanese = "堅忍の誉れ",
                    Korean = "인내의 영예"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(309, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Relentless March",
                    English = "Relentless March",
                    French = "Marche implacable",
                    German = "Marsch ohne Rücksicht",
                    Japanese = "無敵の進撃マーチ（仮）",
                    Korean = "무적의 진격 행진곡(가칭)"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(310, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "突风",
                    English = "Curl",
                    French = "Pelotonnement",
                    German = "Einrollen",
                    Japanese = "かたまり",
                    Korean = "웅크리기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(311, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Earthen Ward",
                    English = "Earthen Ward",
                    French = "Barrière terrestre",
                    German = "Erdengewahrsam",
                    Japanese = "大地の守り",
                    Korean = "대지의 수호"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(312, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Razed Earth",
                    English = "Razed Earth",
                    French = "Fureur tellurique",
                    German = "Gaias Zorn",
                    Japanese = "大地の怒り",
                    Korean = "대지의 분노"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(313, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Radiant Shield",
                    English = "Radiant Shield",
                    French = "Bouclier radiant",
                    German = "Glühender Schild",
                    Japanese = "光輝の盾",
                    Korean = "광휘의 방패"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(314, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Inferno",
                    English = "Inferno",
                    French = "Flammes de l'enfer",
                    German = "Inferno",
                    Japanese = "地獄の火炎",
                    Korean = "지옥의 화염"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(315, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Whispering Dawn",
                    English = "Whispering Dawn",
                    French = "Murmure de l'aurore",
                    German = "Erhebendes Flüstern",
                    Japanese = "光の囁き",
                    Korean = "빛의 속삭임"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(316, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fey Covenant",
                    English = "Fey Covenant",
                    French = "Alliance féerique",
                    German = "Feenverheißung",
                    Japanese = "フェイコヴナント",
                    Korean = "요정의 서약"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(317, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fey Illumination",
                    English = "Fey Illumination",
                    French = "Illumination féerique",
                    German = "Illumination",
                    Japanese = "フェイイルミネーション",
                    Korean = "요정의 광휘"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(318, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fey Glow",
                    English = "Fey Glow",
                    French = "Lueur féerique",
                    German = "Sprühender Glanz",
                    Japanese = "フェイグロウ",
                    Korean = "요정의 달빛"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(319, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fey Light",
                    English = "Fey Light",
                    French = "Lumière féerique",
                    German = "Feenlicht",
                    Japanese = "フェイライト",
                    Korean = "요정의 햇빛"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(320, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "流血",
                    English = "Bleeding",
                    French = "Saignant",
                    German = "Blutung",
                    Japanese = "ペイン",
                    Korean = "고통"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(321, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Gungnir",
                    English = "Gungnir",
                    French = "Gungnir",
                    German = "Gugnir",
                    Japanese = "グングニルの槍",
                    Korean = "투창 궁니르"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(322, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "水晶纱帐",
                    English = "Crystal Veil",
                    French = "Voile cristallin",
                    German = "Kristallschleier",
                    Japanese = "クリスタルヴェール",
                    Korean = "크리스탈 장막"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(323, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "免疫低下",
                    English = "Reduced Immunity",
                    French = "Immunité réduite",
                    German = "Schwache Immunabwehr",
                    Japanese = "免疫低下",
                    Korean = "면역력 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(324, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Greenwrath",
                    English = "Greenwrath",
                    French = "Ire de la forêt",
                    German = "Sintmal",
                    Japanese = "森の悲憤",
                    Korean = "숲의 분노"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(325, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "无敌",
                    English = "Invincibility",
                    French = "Invulnérable",
                    German = "Unverwundbar",
                    Japanese = "無敵",
                    Korean = "무적"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(326, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Lightning Charge",
                    English = "Lightning Charge",
                    French = "Charge électrique",
                    German = "Statische Ladung",
                    Japanese = "帯電",
                    Korean = "전류 충전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(327, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Ice Charge",
                    English = "Ice Charge",
                    French = "Charge glacée",
                    German = "Eisige Ladung",
                    Japanese = "帯氷",
                    Korean = "얼음 충전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(328, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Heart of the Mountain",
                    English = "Heart of the Mountain",
                    French = "Cœur de la montagne",
                    German = "Herz des Felsgotts",
                    Japanese = "岩神の心石",
                    Korean = "바위신의 심장석"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(329, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Modification",
                    English = "Modification",
                    French = "Récupération robotique",
                    German = "Fortifikationsprogramm 1",
                    Japanese = "自己強化プログラム",
                    Korean = "자기 강화 프로그램"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(330, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Haste",
                    English = "Haste",
                    French = "Hâte",
                    German = "Hast",
                    Japanese = "ヘイスト",
                    Korean = "헤이스트"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(331, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "受到的魔法伤害下降",
                    English = "Magic Vulnerability Down",
                    French = "Vulnérabilité magique diminuée",
                    German = "Verringerte Magie-Verwundbarkeit",
                    Japanese = "被魔法ダメージ軽減",
                    Korean = "마법 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(332, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(333, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Allagan Rot",
                    English = "Allagan Rot",
                    French = "Pourriture allagoise",
                    German = "Allagische Fäulnis",
                    Japanese = "アラガンロット",
                    Korean = "알라그 부패"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(334, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Allagan Immunity",
                    English = "Allagan Immunity",
                    French = "Anticorps allagois",
                    German = "Allagische Immunität",
                    Japanese = "アラガンロット抗体",
                    Korean = "알라그 부패 항체"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(335, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Firestream",
                    English = "Firestream",
                    French = "Courants de feu",
                    German = "Feuerstrahlen",
                    Japanese = "ファイアストリーム",
                    Korean = "화염 기류"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(336, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sequence AB1",
                    English = "Sequence AB1",
                    French = "Séquence AB1",
                    German = "Sequenz AB1",
                    Japanese = "対打撃プログラム",
                    Korean = "타격 대비 프로그램"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(337, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sequence AP1",
                    English = "Sequence AP1",
                    French = "Séquence AP1",
                    German = "Sequenz AP1",
                    Japanese = "対突撃プログラム",
                    Korean = "찌르기 대비 프로그램"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(338, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sequence AS1",
                    English = "Sequence AS1",
                    French = "Séquence AS1",
                    German = "Sequenz AS1",
                    Japanese = "対斬撃プログラム",
                    Korean = "베기 대비 프로그램"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(339, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "流血",
                    English = "Bleeding",
                    French = "Saignant",
                    German = "Blutung",
                    Japanese = "ペイン",
                    Korean = "고통"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(340, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "物理壁垒",
                    English = "Physical Field",
                    French = "Champ physique",
                    German = "Physisches Feld",
                    Japanese = "対物理障壁",
                    Korean = "물리 공격 장벽"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(341, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aetherial Field",
                    English = "Aetherial Field",
                    French = "Champ éthéré",
                    German = "Magisches Feld",
                    Japanese = "対魔法障壁",
                    Korean = "마법 공격 장벽"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(342, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Repelling Spray",
                    English = "Repelling Spray",
                    French = "Réplique",
                    German = "Reflektorschild",
                    Japanese = "応射",
                    Korean = "대응"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(343, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bleeding",
                    English = "Bleeding",
                    French = "Saignant",
                    German = "Blutung",
                    Japanese = "ペイン",
                    Korean = "고통"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(344, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Neurolink",
                    English = "Neurolink",
                    French = "Neurolien",
                    German = "Neurolink",
                    Japanese = "拘束装置",
                    Korean = "구속 장치"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(345, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Recharge",
                    English = "Recharge",
                    French = "Recharge",
                    German = "Aufladung",
                    Japanese = "魔力供給",
                    Korean = "마력 공급"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(346, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Waxen Flesh",
                    English = "Waxen Flesh",
                    French = "Chair fondue",
                    German = "Wächserne Haut",
                    Japanese = "帯炎",
                    Korean = "불보라"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(347, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Pox",
                    English = "Pox",
                    French = "Vérole",
                    German = "Pocken",
                    Japanese = "ポックス",
                    Korean = "두창"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(348, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Disseminate",
                    English = "Disseminate",
                    French = "Dissémination",
                    German = "Aussäen",
                    Japanese = "ディスセミネイト",
                    Korean = "유포"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(349, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Steel Scales",
                    English = "Steel Scales",
                    French = "Écailles d'acier",
                    German = "Stahlschuppen",
                    Japanese = "スチールスケール",
                    Korean = "강철 비늘"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(350, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vulnerability Down",
                    English = "Vulnerability Down",
                    French = "Vulnérabilité diminuée",
                    German = "Verringerte Verwundbarkeit",
                    Japanese = "被ダメージ低下",
                    Korean = "받는 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(351, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rancor",
                    English = "Rancor",
                    French = "Rancune",
                    German = "Groll",
                    Japanese = "怨み",
                    Korean = "원한"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(352, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Spjot",
                    English = "Spjot",
                    French = "Spjot",
                    German = "Gugnirs Zauber",
                    Japanese = "グングニルの魔力",
                    Korean = "투창 궁니르의 마력"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(353, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Brave New World",
                    English = "Brave New World",
                    French = "Un nouveau monde",
                    German = "Startbonus",
                    Japanese = "カンパニーアクション：ビギナーボーナス",
                    Korean = "부대 혜택: 초보자 지원"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(354, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Live off the Land",
                    English = "Live off the Land",
                    French = "Vivre de la terre",
                    German = "Sammelgeschick-Bonus",
                    Japanese = "カンパニーアクション：獲得力アップ",
                    Korean = "부대 혜택: 획득력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(355, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "What You See",
                    English = "What You See",
                    French = "Avoir le coup d'œil",
                    German = "Wahrnehmungsbonus",
                    Japanese = "カンパニーアクション：識質力アップ",
                    Korean = "부대 혜택: 감별력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(356, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Eat from the Hand",
                    English = "Eat from the Hand",
                    French = "La main qui nourrit",
                    German = "Kunstfertigkeits-Bonus",
                    Japanese = "カンパニーアクション：作業精度アップ",
                    Korean = "부대 혜택: 작업 숙련도 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(357, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "In Control",
                    English = "In Control",
                    French = "Passer maître",
                    German = "Kontrolle-Bonus",
                    Japanese = "カンパニーアクション：加工精度アップ",
                    Korean = "부대 혜택: 가공 숙련도 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(358, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Snowman",
                    English = "Snowman",
                    French = "Bonhomme de neige",
                    German = "Schneemann",
                    Japanese = "雪だるま",
                    Korean = "눈사람"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(359, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fey Fire",
                    English = "Fey Fire",
                    French = "Feu follet",
                    German = "Feenfeuer",
                    Japanese = "妖炎",
                    Korean = "요괴의 화염"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(360, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Meat and Mead",
                    English = "Meat and Mead",
                    French = "À boire et à manger",
                    German = "Längere Nahrungseffekte",
                    Japanese = "カンパニーアクション：食事効果時間延長",
                    Korean = "부대 혜택: 식사 지속시간 연장"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(361, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "That Which Binds Us",
                    English = "That Which Binds Us",
                    French = "Union parfaite",
                    German = "Bindungsbonus",
                    Japanese = "カンパニーアクション：錬精度上昇量アップ",
                    Korean = "부대 혜택: 결속도 상승치 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(362, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Proper Care",
                    English = "Proper Care",
                    French = "Protections protégées",
                    German = "Verminderter Verschleiß",
                    Japanese = "カンパニーアクション：装備品劣化低減",
                    Korean = "부대 혜택: 장비 소모 절감"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(363, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Back on Your Feet",
                    English = "Back on Your Feet",
                    French = "Prompt rétablissement",
                    German = "Verkürzte Schwäche",
                    Japanese = "カンパニーアクション：衰弱時間短縮",
                    Korean = "부대 혜택: 쇠약 시간 단축"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(364, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Reduced Rates",
                    English = "Reduced Rates",
                    French = "Prix d'ami",
                    German = "Vergünstigter Teleport",
                    Japanese = "カンパニーアクション：テレポ割引",
                    Korean = "부대 혜택: 텔레포 할인"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(365, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "The Heat of Battle",
                    English = "The Heat of Battle",
                    French = "Feu du combat",
                    German = "Kampfroutine-Bonus",
                    Japanese = "カンパニーアクション：討伐経験値アップ",
                    Korean = "부대 혜택: 전투 경험치 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(366, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "A Man's Best Friend",
                    English = "A Man's Best Friend",
                    French = "Meilleur ami de l'homme",
                    German = "Mitstreiterroutine-Bonus",
                    Japanese = "カンパニーアクション：バディ経験値アップ",
                    Korean = "부대 혜택: 버디 경험치 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(367, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Earth and Water",
                    English = "Earth and Water",
                    French = "Terre et eau",
                    German = "Sammelroutine-Bonus",
                    Japanese = "カンパニーアクション：採集経験値アップ",
                    Korean = "부대 혜택: 채집 경험치 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(368, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Helping Hand",
                    English = "Helping Hand",
                    French = "Être en bonnes mains",
                    German = "Syntheseroutine-Bonus",
                    Japanese = "カンパニーアクション：製作経験値アップ",
                    Korean = "부대 혜택: 제작 경험치 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(369, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Viscous Aetheroplasm",
                    English = "Viscous Aetheroplasm",
                    French = "Éthéroplasma",
                    German = "Ätheroplasma",
                    Japanese = "吸着爆雷",
                    Korean = "흡착 폭뢰"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(370, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Siren Song",
                    English = "Siren Song",
                    French = "Chant de la sirène",
                    German = "Sirenengesang",
                    Japanese = "セイレーンの歌声",
                    Korean = "세이렌의 노랫소리"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(371, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Zombification",
                    English = "Zombification",
                    French = "Zombi",
                    German = "Zombie",
                    Japanese = "ゾンビー",
                    Korean = "좀비"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(372, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Brood Rage",
                    English = "Brood Rage",
                    French = "Colère maternelle",
                    German = "Brutrage",
                    Japanese = "母鳥の怒り",
                    Korean = "어미새의 분노"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(373, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Blight",
                    English = "Blight",
                    French = "Insoignabilité",
                    German = "Unheilbar",
                    Japanese = "被回復無効",
                    Korean = "회복 무효"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(374, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Corrupted Crystal",
                    English = "Corrupted Crystal",
                    French = "Cristaux corrompus",
                    German = "Denaturierter Kristall",
                    Japanese = "偏属性クリスタル",
                    Korean = "편속성 크리스탈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(375, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Suppuration",
                    English = "Suppuration",
                    French = "Morsure du feu",
                    German = "Verbrennung",
                    Japanese = "熱傷",
                    Korean = "열화상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(376, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Searing Wind",
                    English = "Searing Wind",
                    French = "Fournaise",
                    German = "Gluthitze",
                    Japanese = "灼熱",
                    Korean = "작열"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(377, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "炎狱之锁",
                    English = "Infernal Fetters",
                    French = "Chaînes infernales",
                    German = "Infernofesseln",
                    Japanese = "炎獄の鎖",
                    Korean = "염옥의 사슬"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(378, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Death Throes",
                    English = "Death Throes",
                    French = "Affres de la mort",
                    German = "Agonales Klammern",
                    Japanese = "道連れ",
                    Korean = "물귀신 작전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(379, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thermal Low",
                    English = "Thermal Low",
                    French = "Basse pression",
                    German = "Tiefdruck",
                    Japanese = "低気圧",
                    Korean = "저기압"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(380, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thermal High",
                    English = "Thermal High",
                    French = "Haute pression",
                    German = "Hochdruck",
                    Japanese = "高気圧",
                    Korean = "고기압"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(381, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Testudo",
                    English = "Testudo",
                    French = "Testudo",
                    German = "Testudo",
                    Japanese = "テストゥド",
                    Korean = "귀갑 방패"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(384, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thrill of War",
                    English = "Thrill of War",
                    French = "Frisson de la guerre",
                    German = "Schlachtrausch",
                    Japanese = "スリル・オブ・ウォー",
                    Korean = "전쟁의 짜릿함"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(385, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Full Swing",
                    English = "Full Swing",
                    French = "Plein élan",
                    German = "Voller Schwinger",
                    Japanese = "フルスイング",
                    Korean = "전력 강타"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(386, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Somersault",
                    English = "Somersault",
                    French = "Saut périlleux",
                    German = "Salto",
                    Japanese = "サマーソルト",
                    Korean = "공중제비"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(387, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fetter Ward",
                    English = "Fetter Ward",
                    French = "Émancipation",
                    German = "Obhut",
                    Japanese = "フェターウォード",
                    Korean = "구속 방지"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(388, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Impulse Rush",
                    English = "Impulse Rush",
                    French = "Impulsion subite",
                    German = "Impuls-Ansturm",
                    Japanese = "インパルスラッシュ",
                    Korean = "연쇄 돌격"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(389, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Skewer",
                    English = "Skewer",
                    French = "Embrochement",
                    German = "Spieß",
                    Japanese = "スキュアー",
                    Korean = "꼬챙이 꿰기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(390, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sacred Prism",
                    English = "Sacred Prism",
                    French = "Prisme sacré",
                    German = "Barmherzigkeit",
                    Japanese = "女神の慈悲",
                    Korean = "여신의 자비"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(391, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Phantom Dart",
                    English = "Phantom Dart",
                    French = "Projectile fantôme",
                    German = "Phantompfeil",
                    Japanese = "ファントムダート",
                    Korean = "유령 쐐기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(392, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Misty Veil",
                    English = "Misty Veil",
                    French = "Voile de brume",
                    German = "Nebelschleier",
                    Japanese = "ミスティヴェール",
                    Korean = "안개의 장막"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(393, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Wither",
                    English = "Wither",
                    French = "Flétrissure",
                    German = "Entkräften",
                    Japanese = "ウィザー",
                    Korean = "쇠퇴"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(394, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Focalization",
                    English = "Focalization",
                    French = "Traité de la focalisation",
                    German = "Lege Artis",
                    Japanese = "精神統一の策",
                    Korean = "정신통일책"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(395, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aetheric Burst",
                    English = "Aetheric Burst",
                    French = "Explosion éthérée",
                    German = "Ätherschub",
                    Japanese = "エーテルバースト",
                    Korean = "에테르 분출"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(396, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Equanimity",
                    English = "Equanimity",
                    French = "Équanimité",
                    German = "Gleichmut",
                    Japanese = "専心",
                    Korean = "전념"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(397, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Attunement",
                    English = "Attunement",
                    French = "Harmonie",
                    German = "Einstimmung",
                    Japanese = "調和",
                    Korean = "조화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(398, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Astral Realignment",
                    English = "Astral Realignment",
                    French = "Alignement astral",
                    German = "Astralkörper",
                    Japanese = "アストラル体",
                    Korean = "영체화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(399, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Corporeal Return",
                    English = "Corporeal Return",
                    French = "Retour corporel",
                    German = "Wiederkunft",
                    Japanese = "生還",
                    Korean = "생환"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(400, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Charge",
                    English = "Charge",
                    French = "Charge électrique",
                    German = "Laden",
                    Japanese = "蓄電",
                    Korean = "축전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(401, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Seized",
                    English = "Seized",
                    French = "Étreinte mortelle",
                    German = "Umklammert",
                    Japanese = "捕獲",
                    Korean = "포획"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(402, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thrown for a Loop",
                    English = "Thrown for a Loop",
                    French = "Acharnement aveugle",
                    German = "Blinde Wut",
                    Japanese = "有頂天",
                    Korean = "노기 충천"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(403, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(404, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Transporting",
                    English = "Transporting",
                    French = "Chargé",
                    German = "Transport",
                    Japanese = "運搬",
                    Korean = "운반"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(405, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bewildered",
                    English = "Bewildered",
                    French = "Égarement",
                    German = "Bezaubert",
                    Japanese = "幻惑",
                    Korean = "현혹"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(406, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vulnerability Down",
                    English = "Vulnerability Down",
                    French = "Vulnérabilité diminuée",
                    German = "Verringerte Verwundbarkeit",
                    Japanese = "被ダメージ低下",
                    Korean = "받는 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(407, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Dust Poisoning",
                    English = "Dust Poisoning",
                    French = "Empoisonnement cristallin",
                    German = "Staubvergiftung",
                    Japanese = "粉塵中毒",
                    Korean = "분진 중독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(408, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Storm's Path",
                    English = "Storm's Path",
                    French = "Couperet de justice",
                    German = "Sturmkeil",
                    Japanese = "シュトルムヴィント",
                    Korean = "폭풍 쐐기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(409, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Holmgang",
                    English = "Holmgang",
                    French = "Holmgang",
                    German = "Holmgang",
                    Japanese = "ホルムギャング",
                    Korean = "일대일 결투"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(410, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Antibody",
                    English = "Antibody",
                    French = "Réaction antivirale",
                    German = "Antikörper",
                    Japanese = "ウイルス抗体",
                    Korean = "바이러스 항체"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(411, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Inner Beast",
                    English = "Inner Beast",
                    French = "Bête intérieure",
                    German = "Tier in dir",
                    Japanese = "原初の魂",
                    Korean = "원초의 혼"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(412, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Hover",
                    English = "Hover",
                    French = "Élévation",
                    German = "Schweben",
                    Japanese = "滞空",
                    Korean = "체공"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(413, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mark Up",
                    English = "Mark Up",
                    French = "Marque des vainqueurs",
                    German = "Wolfsmarken-Bonus",
                    Japanese = "カンパニーアクション：対人戦績アップ",
                    Korean = "부대 혜택: 명예 점수"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(414, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Seal Sweetener",
                    English = "Seal Sweetener",
                    French = "Solde accrue",
                    German = "Staatstaler-Bonus",
                    Japanese = "カンパニーアクション：軍票アップ",
                    Korean = "부대 혜택: 군표 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(415, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Regain",
                    English = "Regain",
                    French = "Regain",
                    German = "Erholen",
                    Japanese = "ＴＰ継続回復",
                    Korean = "TP 지속 회복"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(416, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Transparent",
                    English = "Transparent",
                    French = "Transparence",
                    German = "Transparenz",
                    Japanese = "透明",
                    Korean = "투명"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(417, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Protect",
                    English = "Protect",
                    French = "Bouclier",
                    German = "Protes",
                    Japanese = "プロテス",
                    Korean = "프로테스"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(418, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Vulnerability Down",
                    English = "Physical Vulnerability Down",
                    French = "Vulnérabilité physique diminuée",
                    German = "Verringerte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ軽減",
                    Korean = "물리 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(419, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Misty Veil",
                    English = "Misty Veil",
                    French = "Voile de brume",
                    German = "Nebelschleier",
                    Japanese = "ミスティヴェール",
                    Korean = "안개의 장막"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(420, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Prey",
                    English = "Prey",
                    French = "Marquage",
                    German = "Markiert",
                    Japanese = "マーキング",
                    Korean = "표식"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(421, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Devoured",
                    English = "Devoured",
                    French = "Dévorement",
                    German = "Halbverschlungen",
                    Japanese = "捕食",
                    Korean = "포식"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(422, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Healing Magic Down",
                    English = "Healing Magic Down",
                    French = "Malus de soin",
                    German = "Heilmagie -",
                    Japanese = "回復魔法効果低下",
                    Korean = "회복마법 효과 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(423, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Nightmare",
                    English = "Nightmare",
                    French = "Cauchemar",
                    German = "Albtraum",
                    Japanese = "悪夢",
                    Korean = "악몽"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(424, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Diabolic Curse",
                    English = "Diabolic Curse",
                    French = "Maléfice du néant",
                    German = "Diabolischer Fluch",
                    Japanese = "ヴォイドの呪詛",
                    Korean = "보이드의 저주"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(425, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Eerie Air",
                    English = "Eerie Air",
                    French = "Présence du néant",
                    German = "Diabolische Aura",
                    Japanese = "ヴォイドの妖気",
                    Korean = "보이드의 요기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(426, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Noctoshield",
                    English = "Noctoshield",
                    French = "Nocto-bouclier",
                    German = "Nachtschild",
                    Japanese = "ノクトシールド",
                    Korean = "밤의 방패"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(427, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Slow+",
                    English = "Slow+",
                    French = "Lenteur +",
                    German = "Gemach +",
                    Japanese = "スロウ＋",
                    Korean = "둔화+"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(428, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Haste+",
                    English = "Haste+",
                    French = "Hâte +",
                    German = "Hast +",
                    Japanese = "ヘイスト＋",
                    Korean = "헤이스트+"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(429, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Scale Flakes",
                    English = "Scale Flakes",
                    French = "Poussière érosive",
                    German = "Erosionsstaub",
                    Japanese = "妖鱗粉",
                    Korean = "환각 가루"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(430, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Misery",
                    English = "Misery",
                    French = "Désolation",
                    German = "Kummer",
                    Japanese = "悲嘆",
                    Korean = "비탄"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(431, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Water Resistance Down",
                    English = "Water Resistance Down",
                    French = "Résistance à l'eau diminuée",
                    German = "Wasserresistenz -",
                    Japanese = "水属性耐性低下",
                    Korean = "물속성 저항 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(432, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Briny Mirror",
                    English = "Briny Mirror",
                    French = "Reflets d'eau libérés",
                    German = "Wassermembran",
                    Japanese = "水鏡飛散",
                    Korean = "수경 확산"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(433, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Briny Veil",
                    English = "Briny Veil",
                    French = "Reflet d'eau",
                    German = "Wasserspiegelung",
                    Japanese = "水鏡",
                    Korean = "수경"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(434, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Absolute Bind",
                    English = "Absolute Bind",
                    French = "Étreinte impénétrable",
                    German = "Absoluter Bann",
                    Japanese = "完全呪縛",
                    Korean = "완전한 속박"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(435, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Demon Eye",
                    English = "Demon Eye",
                    French = "Œil diabolique",
                    German = "Dämonenauge",
                    Japanese = "悪魔の瞳",
                    Korean = "악마의 눈동자"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(436, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Briar",
                    English = "Briar",
                    French = "Ronces sauvages",
                    German = "Dorngestrüpp",
                    Japanese = "野茨",
                    Korean = "가시밭"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(437, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stone Curse",
                    English = "Stone Curse",
                    French = "Piège de pierre",
                    German = "Steinfluch",
                    Japanese = "石化の呪い",
                    Korean = "석화의 저주"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(438, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Minimum",
                    English = "Minimum",
                    French = "Mini",
                    German = "Wicht",
                    Japanese = "ミニマム",
                    Korean = "미니멈"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(439, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Toad",
                    English = "Toad",
                    French = "Crapaud",
                    German = "Frosch",
                    Japanese = "トード",
                    Korean = "두꺼비"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(440, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Minimum",
                    English = "Minimum",
                    French = "Mini",
                    German = "Wicht",
                    Japanese = "ミニマム",
                    Korean = "미니멈"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(441, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Toad",
                    English = "Toad",
                    French = "Crapaud",
                    German = "Frosch",
                    Japanese = "トード",
                    Korean = "두꺼비"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(442, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Slow",
                    English = "Slow",
                    French = "Lenteur",
                    German = "Gemach",
                    Japanese = "スロウ",
                    Korean = "둔화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(443, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(444, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vulnerability Up",
                    English = "Vulnerability Up",
                    French = "Vulnérabilité augmentée",
                    German = "Erhöhte Verwundbarkeit",
                    Japanese = "被ダメージ上昇",
                    Korean = "받는 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(445, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thorny Vine",
                    English = "Thorny Vine",
                    French = "Sarment de ronces",
                    German = "Dornenranken",
                    Japanese = "茨の蔓",
                    Korean = "가시덩굴"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(446, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Honey-glazed",
                    English = "Honey-glazed",
                    French = "Mielleux",
                    German = "Honigsüß",
                    Japanese = "蜂蜜",
                    Korean = "벌꿀"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(447, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Potent Acid",
                    English = "Potent Acid",
                    French = "Acide corrosif",
                    German = "Konzentrierte Säure",
                    Japanese = "強酸",
                    Korean = "강산"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(448, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Swarmed",
                    English = "Swarmed",
                    French = "Essaim",
                    German = "Umschwärmt",
                    Japanese = "スウォーム",
                    Korean = "벌레 떼"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(449, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Stung",
                    English = "Stung",
                    French = "Dard",
                    German = "Gestochen",
                    Japanese = "蜂刺症",
                    Korean = "벌 중독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(450, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Petrification Resistance",
                    English = "Petrification Resistance",
                    French = "Résistance à Pétrification",
                    German = "Steinresistenz",
                    Japanese = "石化無効",
                    Korean = "석화 무효"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(451, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Cursed Voice",
                    English = "Cursed Voice",
                    French = "Voix du maléfice",
                    German = "Stimme der Verwünschung",
                    Japanese = "呪詛の声",
                    Korean = "저주의 목소리"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(452, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Cursed Shriek",
                    English = "Cursed Shriek",
                    French = "Cri du maléfice",
                    German = "Schrei der Verwünschung",
                    Japanese = "呪詛の叫声",
                    Korean = "저주의 외침"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(453, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Allagan Venom",
                    English = "Allagan Venom",
                    French = "Venin allagois",
                    German = "Allagisches Gift",
                    Japanese = "アラガンポイズン",
                    Korean = "알라그 뱀독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(454, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Allagan Field",
                    English = "Allagan Field",
                    French = "Champ allagois",
                    German = "Allagisches Feld",
                    Japanese = "アラガンフィールド",
                    Korean = "알라그 필드"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(455, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Languishing",
                    English = "Languishing",
                    French = "Agonie vitale",
                    German = "Ermattung",
                    Japanese = "生気減退",
                    Korean = "생기 감퇴"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(456, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Maximum HP Down",
                    English = "Maximum HP Down",
                    French = "PV maximum réduits",
                    German = "LP-Malus",
                    Japanese = "最大ＨＰダウン",
                    Korean = "최대 HP 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(457, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bind+",
                    English = "Bind+",
                    French = "Entrave +",
                    German = "Fessel +",
                    Japanese = "バインド＋",
                    Korean = "속박+"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(458, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Raven Blight",
                    English = "Raven Blight",
                    French = "Bile de rapace",
                    German = "Pestschwinge",
                    Japanese = "凶鳥毒気",
                    Korean = "흉조의 독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(459, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Normal Stance",
                    English = "Normal Stance",
                    French = "Posture normale",
                    German = "Normales Verhalten",
                    Japanese = "ノーマルスタンス",
                    Korean = "일반 태세"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(460, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aggressive Stance",
                    English = "Aggressive Stance",
                    French = "Posture d'attaque",
                    German = "Aggressives Verhalten",
                    Japanese = "アタッカースタンス",
                    Korean = "공격 태세"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(461, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Subversive Stance",
                    English = "Subversive Stance",
                    French = "Posture de diversion",
                    German = "Hemmendes Verhalten",
                    Japanese = "ジャマースタンス",
                    Korean = "방해 태세"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(462, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Garrote Twist",
                    English = "Garrote Twist",
                    French = "Sangle accélérée",
                    German = "Leicht fixierbar",
                    Japanese = "拘束加速",
                    Korean = "구속 가속"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(463, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Garrote",
                    English = "Garrote",
                    French = "Attache",
                    German = "Fixierungsfessel",
                    Japanese = "拘束",
                    Korean = "구속"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(464, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Firescorched",
                    English = "Firescorched",
                    French = "Corne-de-feu",
                    German = "Feuerhorn",
                    Japanese = "ファイアホーン",
                    Korean = "화염뿔"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(465, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Icebitten",
                    English = "Icebitten",
                    French = "Griffe-de-glace",
                    German = "Eisklaue",
                    Japanese = "アイスクロウ",
                    Korean = "얼음발톱"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(466, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thunderstruck",
                    English = "Thunderstruck",
                    French = "Aile-de-foudre",
                    German = "Donnerschwinge",
                    Japanese = "サンダーウィング",
                    Korean = "번개날개"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(467, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Briny Veil",
                    English = "Briny Veil",
                    French = "Reflet d'eau",
                    German = "Wasserspiegelung",
                    Japanese = "水鏡",
                    Korean = "수경"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(468, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Voidbound",
                    English = "Voidbound",
                    French = "Transfert du néant",
                    German = "Nichtsgebunden",
                    Japanese = "異界の狭間",
                    Korean = "이계의 틈새"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(469, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "High and Mighty",
                    English = "High and Mighty",
                    French = "Monarchie absolue",
                    German = "Absoluter Herrscher",
                    Japanese = "極王",
                    Korean = "극왕"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(470, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Pombination",
                    English = "Pombination",
                    French = "Mogtimisation",
                    German = "Pombination",
                    Japanese = "モグビネーション",
                    Korean = "모그비네이션"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(471, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Moglight Resistance Down",
                    English = "Moglight Resistance Down",
                    French = "Nyctophobie mog",
                    German = "Mogryschatten-Aversion",
                    Japanese = "モグ闇過敏症",
                    Korean = "모그어둠 과민증"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(472, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mogdark Resistance Down",
                    English = "Mogdark Resistance Down",
                    French = "Photophobie mog",
                    German = "Moglicht-Empfindlichkeit",
                    Japanese = "モグ光過敏症",
                    Korean = "모그빛 과민증"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(473, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bemoggled",
                    English = "Bemoggled",
                    French = "Tournis mog",
                    German = "Tohuwabohu-Wahn",
                    Japanese = "モグルグル",
                    Korean = "모글빙글"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(474, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Royal Rouse",
                    English = "Royal Rouse",
                    French = "Mogtivation",
                    German = "Mogul-Fanfare",
                    Japanese = "闘魂",
                    Korean = "투혼"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(475, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Slippery Prey",
                    English = "Slippery Prey",
                    French = "Marquage impossible",
                    German = "Unmarkierbar",
                    Japanese = "マーキング対象外",
                    Korean = "표식 대상 제외"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(476, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Gloam",
                    English = "Gloam",
                    French = "Voile ombreux",
                    German = "Dämmerlicht",
                    Japanese = "薄闇",
                    Korean = "희미한 어둠"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(477, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mantle of the Whorl",
                    English = "Mantle of the Whorl",
                    French = "Manteau du Déchaîneur",
                    German = "Wogenmantel",
                    Japanese = "水神のマント",
                    Korean = "수신의 망토"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(478, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Veil of the Whorl",
                    English = "Veil of the Whorl",
                    French = "Voile du Déchaîneur",
                    German = "Wogenschleier",
                    Japanese = "水神のヴェール",
                    Korean = "수신의 장막"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(479, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rehabilitation",
                    English = "Rehabilitation",
                    French = "Recouvrement",
                    German = "Rehabilitation",
                    Japanese = "徐々にＨＰ回復",
                    Korean = "서서히 HP 회복"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(480, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Haste+",
                    English = "Haste+",
                    French = "Hâte +",
                    German = "Hast +",
                    Japanese = "ヘイスト＋",
                    Korean = "헤이스트+"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(481, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sprint",
                    English = "Sprint",
                    French = "Sprint",
                    German = "Sprint",
                    Japanese = "スプリント",
                    Korean = "전력 질주"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(482, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Paralysis",
                    English = "Paralysis",
                    French = "Paralysie",
                    German = "Paralyse",
                    Japanese = "麻痺",
                    Korean = "마비"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(483, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "HP Boost",
                    English = "HP Boost",
                    French = "Bonus de PV",
                    German = "LP-Bonus",
                    Japanese = "最大ＨＰアップ",
                    Korean = "최대 HP 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(484, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Ink",
                    English = "Ink",
                    French = "Sépia venimeux",
                    German = "Toxische Tinte",
                    Japanese = "毒墨",
                    Korean = "먹물독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(485, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Dropsy",
                    English = "Dropsy",
                    French = "Œdème",
                    German = "Wassersucht",
                    Japanese = "水毒",
                    Korean = "물독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(486, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Watery Grave",
                    English = "Watery Grave",
                    French = "Geôle aqueuse",
                    German = "Wasserkäfig",
                    Japanese = "水牢",
                    Korean = "수중 감옥"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(487, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Deep Freeze",
                    English = "Deep Freeze",
                    French = "Congélation",
                    German = "Tiefkühlung",
                    Japanese = "氷結",
                    Korean = "빙결"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(488, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Shade Shift",
                    English = "Shade Shift",
                    French = "Décalage d'ombre",
                    German = "Superkniff",
                    Japanese = "残影",
                    Korean = "잔영"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(489, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Kiss of the Wasp",
                    English = "Kiss of the Wasp",
                    French = "Baiser de guêpe",
                    German = "Wespenkuss",
                    Japanese = "蜂毒",
                    Korean = "벌독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(490, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Kiss of the Viper",
                    English = "Kiss of the Viper",
                    French = "Baiser de vipère",
                    German = "Vipernkuss",
                    Japanese = "蛇毒",
                    Korean = "뱀독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(491, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Dancing Edge",
                    English = "Dancing Edge",
                    French = "Lame dansante",
                    German = "Tanzende Schneide",
                    Japanese = "舞踏刃",
                    Korean = "춤추는 칼날"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(492, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mutilation",
                    English = "Mutilation",
                    French = "Attaque mutilante",
                    German = "Verstümmeln",
                    Japanese = "無双旋",
                    Korean = "무쌍베기"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(494, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Shadow Fang",
                    English = "Shadow Fang",
                    French = "Croc d'ombre",
                    German = "Schattenfang",
                    Japanese = "一閃",
                    Korean = "일섬"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(495, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Goad",
                    English = "Goad",
                    French = "Aiguillonnement",
                    German = "Dampf",
                    Japanese = "叱咤",
                    Korean = "질타"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(496, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mudra",
                    English = "Mudra",
                    French = "Mudrâ",
                    German = "Mudra",
                    Japanese = "印",
                    Korean = "인"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(497, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Kassatsu",
                    English = "Kassatsu",
                    French = "Kassatsu",
                    German = "Kassatsu",
                    Japanese = "活殺自在",
                    Korean = "생사여탈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(500, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Huton",
                    English = "Huton",
                    French = "Fûton",
                    German = "Huton",
                    Japanese = "風遁の術",
                    Korean = "풍둔술"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(501, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Doton",
                    English = "Doton",
                    French = "Doton",
                    German = "Doton",
                    Japanese = "土遁の術",
                    Korean = "토둔술"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(502, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Doton Heavy",
                    English = "Doton Heavy",
                    French = "Pesanteur",
                    German = "Gewicht",
                    Japanese = "ヘヴィ",
                    Korean = "과중력"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(503, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Burns",
                    French = "Brûlure",
                    German = "Brandwunde",
                    Japanese = "火傷"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(504, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fetters",
                    English = "Fetters",
                    French = "Attache",
                    German = "Gefesselt",
                    Japanese = "拘束",
                    Korean = "구속"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(505, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(506, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vertigo",
                    English = "Vertigo",
                    French = "Vertige",
                    German = "Schwindel",
                    Japanese = "目眩",
                    Korean = "현기증"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(507, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Suiton",
                    English = "Suiton",
                    French = "Suiton",
                    German = "Suiton",
                    Japanese = "水遁の術",
                    Korean = "수둔술"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(508, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Shadow Fang",
                    English = "Shadow Fang",
                    French = "Croc d'ombre",
                    German = "Schattenfang",
                    Japanese = "影牙",
                    Korean = "그림자 송곳니"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(509, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aetherochemical Bomb",
                    English = "Aetherochemical Bomb",
                    French = "Magismobombe",
                    German = "Ätherochemischer Sprengkörper",
                    Japanese = "魔爆弾",
                    Korean = "마폭탄"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(510, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fetters",
                    English = "Fetters",
                    French = "Attache",
                    German = "Gefesselt",
                    Japanese = "拘束",
                    Korean = "구속"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(511, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fire Toad",
                    English = "Fire Toad",
                    French = "Pyrocrapaud",
                    German = "Knallfrosch",
                    Japanese = "ファイアトード",
                    Korean = "불두꺼비"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(512, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Electroconductivity",
                    English = "Electroconductivity",
                    French = "Électroconductivité",
                    German = "Elektrokonduktivität",
                    Japanese = "導電",
                    Korean = "전기 전도"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(513, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Static Condensation",
                    English = "Static Condensation",
                    French = "Charge électrostatique",
                    German = "Statische Ladung",
                    Japanese = "蓄電",
                    Korean = "축전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(514, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Causality",
                    English = "Causality",
                    French = "Causalité",
                    German = "Kausalität",
                    Japanese = "因果",
                    Korean = "인과응보"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(515, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thunderclap",
                    English = "Thunderclap",
                    French = "Roulement de tonnerre",
                    German = "Rollender Donner",
                    Japanese = "雷鼓",
                    Korean = "뇌신의 북소리"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(516, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Chaos",
                    English = "Chaos",
                    French = "Chaos",
                    German = "Chaos",
                    Japanese = "混沌",
                    Korean = "혼돈"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(517, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Surge Protection",
                    English = "Surge Protection",
                    French = "Parafoudre",
                    German = "Überspannungsschutz",
                    Japanese = "避雷",
                    Korean = "피뢰침"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(518, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Reflect",
                    English = "Reflect",
                    French = "Miroir",
                    German = "Reflektion",
                    Japanese = "リフレク",
                    Korean = "리플렉트"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(519, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Counter",
                    English = "Counter",
                    French = "Riposte",
                    German = "Konter",
                    Japanese = "カウンター",
                    Korean = "반격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(520, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fire Resistance Up",
                    English = "Fire Resistance Up",
                    French = "Résistance au feu accrue",
                    German = "Feuerresistenz +",
                    Japanese = "火属性耐性向上",
                    Korean = "불속성 저항 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(521, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Water Resistance Up",
                    English = "Water Resistance Up",
                    French = "Résistance à l'eau accrue",
                    German = "Wasserresistenz +",
                    Japanese = "水属性耐性向上",
                    Korean = "물속성 저항 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(522, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Wind Resistance Up",
                    English = "Wind Resistance Up",
                    French = "Résistance au vent accrue",
                    German = "Windresistenz +",
                    Japanese = "風属性耐性向上",
                    Korean = "바람속성 저항 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(523, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Lightning Resistance Up",
                    English = "Lightning Resistance Up",
                    French = "Résistance à la foudre accrue",
                    German = "Blitzresistenz +",
                    Japanese = "雷属性耐性向上",
                    Korean = "번개속성 저항 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(524, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Earth Resistance Up",
                    English = "Earth Resistance Up",
                    French = "Résistance à la terre accrue",
                    German = "Erdresistenz +",
                    Japanese = "土属性耐性向上",
                    Korean = "땅속성 저항 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(525, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Ice Resistance Up",
                    English = "Ice Resistance Up",
                    French = "Résistance à la glace accrue",
                    German = "Eisresistenz +",
                    Japanese = "氷属性耐性向上",
                    Korean = "얼음속성 저항 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(526, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Frost Blade",
                    English = "Frost Blade",
                    French = "Lame glaciale",
                    German = "Frostklinge",
                    Japanese = "凍てつく剣",
                    Korean = "얼어붙은 검"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(527, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Frost Brand",
                    English = "Frost Brand",
                    French = "Bâton glacial",
                    German = "Froststab",
                    Japanese = "凍てつく杖",
                    Korean = "얼어붙은 지팡이"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(528, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Frost Bow",
                    English = "Frost Bow",
                    French = "Arc glacial",
                    German = "Frostbogen",
                    Japanese = "凍てつく弓",
                    Korean = "얼어붙은 활"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(529, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Invincibility",
                    English = "Invincibility",
                    French = "Invulnérable",
                    German = "Unverwundbar",
                    Japanese = "無敵",
                    Korean = "무적"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(530, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Burns",
                    English = "Burns",
                    French = "Brûlure",
                    German = "Brandwunde",
                    Japanese = "火傷",
                    Korean = "화상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(531, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Dropsy",
                    English = "Dropsy",
                    French = "Œdème",
                    German = "Wassersucht",
                    Japanese = "水毒",
                    Korean = "물독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(532, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Windburn",
                    English = "Windburn",
                    French = "Brûlure du vent",
                    German = "Beißender Wind",
                    Japanese = "裂傷",
                    Korean = "열상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(533, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Electrocution",
                    English = "Electrocution",
                    French = "Électrocution",
                    German = "Stromschlag",
                    Japanese = "感電",
                    Korean = "감전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(534, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Sludge",
                    English = "Sludge",
                    French = "Emboué",
                    German = "Schlamm",
                    Japanese = "汚泥",
                    Korean = "진흙탕"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(535, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Frostbite",
                    English = "Frostbite",
                    French = "Gelure",
                    German = "Erfrierung",
                    Japanese = "凍傷",
                    Korean = "동상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(536, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion EXP Up",
                    English = "Companion EXP Up",
                    French = "Compagnon d'expérience",
                    German = "Sternstunde der Mitstreiter",
                    Japanese = "バディ強化：経験値アップ",
                    Korean = "버디 강화: 경험치 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(537, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion EXP Up II",
                    English = "Companion EXP Up II",
                    French = "Compagnon d'expérience II",
                    German = "Sternstunde der Mitstreiter II",
                    Japanese = "バディ強化：経験値アップII",
                    Korean = "버디 강화: 경험치 증가 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(538, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion Attack Up",
                    English = "Companion Attack Up",
                    French = "Compagnon d'attaque",
                    German = "Mitstreiter-Attackebonus",
                    Japanese = "バディ強化：攻撃力アップ",
                    Korean = "버디 강화: 공격력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(539, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion Attack Up II",
                    English = "Companion Attack Up II",
                    French = "Compagnon d'attaque II",
                    German = "Mitstreiter-Attackebonus II",
                    Japanese = "バディ強化：攻撃力アップII",
                    Korean = "버디 강화: 공격력 상승 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(540, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion Healing Potency Up",
                    English = "Companion Healing Potency Up",
                    French = "Compagnon attentionné",
                    German = "Mitstreiter-Heilmagiebonus",
                    Japanese = "バディ強化：魔法回復力アップ",
                    Korean = "버디 강화: 마법 회복력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(541, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion Healing Potency Up II",
                    English = "Companion Healing Potency Up II",
                    French = "Compagnon attentioné II",
                    German = "Mitstreiter-Heilmagiebonus II",
                    Japanese = "バディ強化：魔法回復力アップII",
                    Korean = "버디 강화: 마법 회복력 상승 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(542, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion Maximum HP Up",
                    English = "Companion Maximum HP Up",
                    French = "Compagnon gaillard",
                    German = "Mitstreiter-LP-Bonus",
                    Japanese = "バディ強化：最大ＨＰアップ",
                    Korean = "버디 강화: 최대 HP 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(543, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion Maximum HP Up II",
                    English = "Companion Maximum HP Up II",
                    French = "Compagnon gaillard II",
                    German = "Mitstreiter-LP-Bonus II",
                    Japanese = "バディ強化：最大ＨＰアップII",
                    Korean = "버디 강화: 최대 HP 증가 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(544, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion Enmity Up",
                    English = "Companion Enmity Up",
                    French = "Compagnon boutefeu",
                    German = "Provokativer Mitstreiter",
                    Japanese = "バディ強化：敵視アップ",
                    Korean = "버디 강화: 적개심 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(545, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Companion Enmity Up II",
                    English = "Companion Enmity Up II",
                    French = "Compagnon boutefeu II",
                    German = "Provokativer Mitstreiter II",
                    Japanese = "バディ強化：敵視アップII",
                    Korean = "버디 강화: 적개심 상승 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(546, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Enervation",
                    English = "Enervation",
                    French = "Dans les choux",
                    German = "Schöner Salat",
                    Japanese = "攻防低下",
                    Korean = "공방 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(547, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Facility Access: Production",
                    English = "Facility Access: Production",
                    French = "Installation: production",
                    German = "Arbeitsstätte: Herstellung",
                    Japanese = "製作施設：部材工作",
                    Korean = "제작시설: 부재제작"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(548, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Facility Access: Finishing",
                    English = "Facility Access: Finishing",
                    French = "Installation: finition",
                    German = "Arbeitsstätte: Veredelung",
                    Japanese = "製作施設：精密工作",
                    Korean = "제작시설: 정밀제작"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(549, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Facility Access: Detailing",
                    English = "Facility Access: Detailing",
                    French = "Installation: minutie",
                    German = "Arbeitsstätte: Feinarbeit",
                    Japanese = "製作施設：難関工作",
                    Korean = "제작시설: 고급제작"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(550, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Facility Access: Production II",
                    English = "Facility Access: Production II",
                    French = "Installation: production II",
                    German = "Arbeitsstätte: Herstellung II",
                    Japanese = "製作施設：部材工作II",
                    Korean = "제작시설: 부재제작 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(551, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Facility Access: Specialization",
                    English = "Facility Access: Specialization",
                    French = "Installation: spécialité",
                    German = "Arbeitsstätte: Spezialisierung",
                    Japanese = "製作施設：専門工作",
                    Korean = "제작시설: 전문제작"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(552, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Facility Access: Specialization II",
                    English = "Facility Access: Specialization II",
                    French = "Installation: spécialité II",
                    German = "Arbeitsstätte: Spezialisierung II",
                    Japanese = "製作施設：専門工作II",
                    Korean = "제작시설: 전문제작 2"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(553, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Facility Access: Detailing II",
                    English = "Facility Access: Detailing II",
                    French = "Installation: minutie II",
                    German = "Arbeitsstätte: Feinarbeit II",
                    Japanese = "製作施設：難関工作II",
                    Korean = "제작시설: 고급제작 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(554, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Facility Access: Finishing II",
                    English = "Facility Access: Finishing II",
                    French = "Installation: finition II",
                    German = "Arbeitsstätte: Veredelung II",
                    Japanese = "製作施設：精密工作II",
                    Korean = "제작시설: 정밀제작 2"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(555, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Repelling Spray",
                    English = "Repelling Spray",
                    French = "Réplique",
                    German = "Reflektorschild",
                    Japanese = "応射",
                    Korean = "대응"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(556, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Repelling Spray",
                    English = "Repelling Spray",
                    French = "Réplique",
                    German = "Reflektorschild",
                    Japanese = "応射",
                    Korean = "대응"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(557, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Repelling Spray",
                    English = "Repelling Spray",
                    French = "Réplique",
                    German = "Reflektorschild",
                    Japanese = "応射",
                    Korean = "대응"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(558, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Repelling Spray",
                    English = "Repelling Spray",
                    French = "Réplique",
                    German = "Reflektorschild",
                    Japanese = "応射",
                    Korean = "대응"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(559, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Poison",
                    English = "Poison",
                    French = "Poison",
                    German = "Gift",
                    Japanese = "毒",
                    Korean = "독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(560, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Poison",
                    English = "Poison",
                    French = "Poison",
                    German = "Gift",
                    Japanese = "毒",
                    Korean = "독"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(561, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Slow",
                    English = "Slow",
                    French = "Lenteur",
                    German = "Gemach",
                    Japanese = "スロウ",
                    Korean = "둔화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(562, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Prey",
                    English = "Prey",
                    French = "Marquage",
                    German = "Markiert",
                    Japanese = "マーキング",
                    Korean = "표식"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(563, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vulnerability Up",
                    English = "Vulnerability Up",
                    French = "Vulnérabilité augmentée",
                    German = "Erhöhte Verwundbarkeit",
                    Japanese = "被ダメージ上昇",
                    Korean = "받는 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(564, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bind",
                    English = "Bind",
                    French = "Entrave",
                    German = "Fessel",
                    Japanese = "バインド",
                    Korean = "속박"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(565, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Transfiguration",
                    English = "Transfiguration",
                    French = "Transformation",
                    German = "Verwandlung",
                    Japanese = "変身",
                    Korean = "변신"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(566, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Damage Up",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(567, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Shifting Sands",
                    English = "Shifting Sands",
                    French = "Sable mouvant",
                    German = "Treibsand",
                    Japanese = "流砂",
                    Korean = "모래지옥"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(568, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fisher's Intuition",
                    English = "Fisher's Intuition",
                    French = "Instinct du pêcheur",
                    German = "Petri Heil",
                    Japanese = "漁師の直感",
                    Korean = "어부의 직감"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(569, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Slime",
                    English = "Slime",
                    French = "Mucus",
                    German = "Schleim",
                    Japanese = "粘液",
                    Korean = "점액"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(570, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "In the Line of Fire",
                    English = "In the Line of Fire",
                    French = "Dans la ligne de tir",
                    German = "In der Schusslinie",
                    Japanese = "エイム",
                    Korean = "겨냥"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(571, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Blind",
                    English = "Blind",
                    French = "Cécité",
                    German = "Blind",
                    Japanese = "暗闇",
                    Korean = "암흑"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(572, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Slashing Resistance Down",
                    English = "Slashing Resistance Down",
                    French = "Résistance au tranchant réduite",
                    German = "Hiebresistenz -",
                    Japanese = "斬属性耐性低下",
                    Korean = "베기 저항 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(573, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Blunt Resistance Down",
                    English = "Blunt Resistance Down",
                    French = "Résistance au contondant réduite",
                    German = "Schlagresistenz -",
                    Japanese = "打属性耐性低下",
                    Korean = "타격 저항 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(574, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Erratic Blaster",
                    English = "Erratic Blaster",
                    French = "Électrochoc imprévisible",
                    German = "Erratischer Puls",
                    Japanese = "エラティックブラスター",
                    Korean = "불안정한 블래스터"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(575, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Static Charge",
                    English = "Static Charge",
                    French = "Charge statique",
                    German = "Statische Ladung",
                    Japanese = "帯電",
                    Korean = "전류 충전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(576, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Lightning Resistance Down",
                    English = "Lightning Resistance Down",
                    French = "Résistance à la foudre réduite",
                    German = "Blitzresistenz -",
                    Japanese = "雷属性耐性低下",
                    Korean = "번개속성 저항 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(577, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Barofield",
                    English = "Barofield",
                    French = "Barotraumatisme",
                    German = "Baro-Feld",
                    Japanese = "バロフィールド",
                    Korean = "압력 필드"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(578, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "In the Headlights",
                    English = "In the Headlights",
                    French = "À portée de tête",
                    German = "Hauptkopf",
                    Japanese = "メインヘッド耐性低下",
                    Korean = "가운뎃머리 저항 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(579, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Critical Strikes",
                    English = "Critical Strikes",
                    French = "Coups critiques",
                    German = "Kritische Attacke",
                    Japanese = "クリティカル攻撃",
                    Korean = "극대화 공격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(580, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aetherochemical Nanospores α",
                    English = "Aetherochemical Nanospores α",
                    French = "Magismoparticules α",
                    German = "Nanosporen α",
                    Japanese = "魔科学粒子α",
                    Korean = "마과학 입자 α"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(581, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aetherochemical Nanospores β",
                    English = "Aetherochemical Nanospores β",
                    French = "Magismoparticules β",
                    German = "Nanosporen β",
                    Japanese = "魔科学粒子β",
                    Korean = "마과학 입자 β"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(582, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Magic Vulnerability Down",
                    English = "Magic Vulnerability Down",
                    French = "Vulnérabilité magique diminuée",
                    German = "Verringerte Magie-Verwundbarkeit",
                    Japanese = "被魔法ダメージ軽減",
                    Korean = "마법 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(583, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Vulnerability Down",
                    English = "Physical Vulnerability Down",
                    French = "Vulnérabilité physique diminuée",
                    German = "Verringerte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ軽減",
                    Korean = "물리 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(584, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Energy Field",
                    English = "Energy Field",
                    French = "Champ défensif",
                    German = "Abwehrfeld",
                    Japanese = "防御フィールド",
                    Korean = "방어 필드"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(585, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Energy Field Down",
                    English = "Energy Field Down",
                    French = "Anti-champ défensif",
                    German = "Anti-Abwehrfeld",
                    Japanese = "対防御フィールド",
                    Korean = "방어 필드 무효"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(586, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "HP Boost",
                    English = "HP Boost",
                    French = "Bonus de PV",
                    German = "LP-Bonus",
                    Japanese = "最大ＨＰアップ",
                    Korean = "HP 최대치 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(587, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Forked Lightning",
                    English = "Forked Lightning",
                    French = "Éclair ramifié",
                    German = "Gabelblitz",
                    Japanese = "フォークライトニング",
                    Korean = "갈래 번개"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(588, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Revelation Resistance Down",
                    English = "Revelation Resistance Down",
                    French = "Résistance à Révélation réduite",
                    German = "Offenbarungs-Resistenz -",
                    Japanese = "リヴァレーション耐性低下",
                    Korean = "계시 저항 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(589, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Chain of Purgatory",
                    English = "Chain of Purgatory",
                    French = "Souffle du purgatoire",
                    German = "Kette der Purgation",
                    Japanese = "誘爆",
                    Korean = "유폭"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(590, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Arm of Purgatory",
                    English = "Arm of Purgatory",
                    French = "Bras du purgatoire",
                    German = "Arm der Purgation",
                    Japanese = "延焼",
                    Korean = "연소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(591, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bluefire",
                    English = "Bluefire",
                    French = "Flamme bleue",
                    German = "Blaufeuer",
                    Japanese = "青碧の炎",
                    Korean = "청벽의 불꽃"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(592, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Ring of Fire",
                    English = "Ring of Fire",
                    French = "Vortex de feu",
                    German = "Flammenwand",
                    Japanese = "炎渦",
                    Korean = "불꽃의 소용돌이"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(593, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rise of the Phoenix",
                    English = "Rise of the Phoenix",
                    French = "Oiseau de feu",
                    German = "Feuervogel",
                    Japanese = "不死鳥",
                    Korean = "불사조"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(594, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Harvest",
                    English = "Harvest",
                    French = "Buveur d'âme",
                    German = "Seelensog",
                    Japanese = "吸魂",
                    Korean = "영혼 흡수"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(595, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Cloak of Death",
                    English = "Cloak of Death",
                    French = "Remous de la vie",
                    German = "Sog der Verzehrung",
                    Japanese = "霊泉禍",
                    Korean = "영검의 소용돌이"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(596, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Suffocated Will",
                    English = "Suffocated Will",
                    French = "Aura du Dragon-dieu",
                    German = "Drachenopfer",
                    Japanese = "龍圧",
                    Korean = "용의 위압"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(597, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Flare Dampening",
                    English = "Flare Dampening",
                    French = "ParaTéraBrasier",
                    German = "Neurolink",
                    Japanese = "拘束装置",
                    Korean = "구속 장치"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(598, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "(仮)物理シールド(ストンスキン)",
                    English = "(仮)物理シールド(ストンスキン)",
                    French = "(仮)物理シールド(ストンスキン)",
                    German = "(仮)物理シールド(ストンスキン)",
                    Japanese = "(仮)物理シールド(ストンスキン)",
                    Korean = "(仮)物理シールド(ストンスキン)"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(599, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "(仮)魔法シールド(ストンスキン)",
                    English = "(仮)魔法シールド(ストンスキン)",
                    French = "(仮)魔法シールド(ストンスキン)",
                    German = "(仮)魔法シールド(ストンスキン)",
                    Japanese = "(仮)魔法シールド(ストンスキン)",
                    Korean = "(仮)魔法シールド(ストンスキン)"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(600, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "魔法受伤减轻",
                    English = "Magic Vulnerability Down",
                    French = "Vulnérabilité magique diminuée",
                    German = "Verringerte Magie-Verwundbarkeit",
                    Japanese = "被魔法ダメージ軽減",
                    Korean = "마법 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(601, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Vulnerability Down",
                    English = "Physical Vulnerability Down",
                    French = "Vulnérabilité physique diminuée",
                    German = "Verringerte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ軽減",
                    Korean = "물리 피해 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(602, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Curse of the Mummy",
                    English = "Curse of the Mummy",
                    French = "Malédiction d'Azeyma",
                    German = "Azeymas Fluch",
                    Japanese = "アーゼマの呪い",
                    Korean = "아제마의 저주"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(603, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Mummification",
                    English = "Mummification",
                    French = "Pion d'Azeyma",
                    German = "Azeymas Jünger",
                    Japanese = "アーゼマの使徒",
                    Korean = "아제마의 사도"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(604, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Thin Ice",
                    English = "Thin Ice",
                    French = "Verglas",
                    German = "Glatteis",
                    Japanese = "氷床",
                    Korean = "얼음 바닥"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(605, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Frostbite",
                    English = "Frostbite",
                    French = "Gelure",
                    German = "Erfrierung",
                    Japanese = "凍傷",
                    Korean = "동상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(606, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Frozen",
                    English = "Frozen",
                    French = "Glaciation",
                    German = "Überfroren",
                    Japanese = "凍結",
                    Korean = "동결"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(607, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Snowball",
                    English = "Snowball",
                    French = "Boule de neige",
                    German = "Schneeball",
                    Japanese = "雪玉",
                    Korean = "눈덩이"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(608, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Death Throes",
                    English = "Death Throes",
                    French = "Affres de la mort",
                    German = "Agonales Klammern",
                    Japanese = "道連れ",
                    Korean = "물귀신 작전"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(609, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Seized",
                    English = "Seized",
                    French = "Étreinte mortelle",
                    German = "Umschlungen",
                    Japanese = "捕獲",
                    Korean = "포획"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(610, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Petrification",
                    English = "Petrification",
                    French = "Pétrification",
                    German = "Stein",
                    Japanese = "石化",
                    Korean = "석화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(611, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Invigoration",
                    English = "Invigoration",
                    French = "Bonne humeur",
                    German = "Schwippdischwapp",
                    Japanese = "気分上々",
                    Korean = "기분 최고"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(612, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Wet Plate",
                    English = "Wet Plate",
                    French = "Plein d'eau",
                    German = "Vollgelaufen",
                    Japanese = "うるおい",
                    Korean = "촉촉함"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(613, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Imp",
                    English = "Imp",
                    French = "Kappa",
                    German = "Flusskobold",
                    Japanese = "カッパ",
                    Korean = "물요정"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(614, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Hidden",
                    English = "Hidden",
                    French = "Dissimulation",
                    German = "Versteckt",
                    Japanese = "かくれる",
                    Korean = "은신술"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(615, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Hidden",
                    English = "Hidden",
                    French = "Dissimulation",
                    German = "Versteckt",
                    Japanese = "かくれる",
                    Korean = "은신술"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(616, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Invisible",
                    English = "Invisible",
                    French = "Invisible",
                    German = "Unsichtbar",
                    Japanese = "インビジブル",
                    Korean = "투명화"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(617, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Irradiated",
                    English = "Irradiated",
                    French = "Irradiation",
                    German = "Erstrahlend",
                    Japanese = "帯光",
                    Korean = "광채"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(618, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Area of Influence Up",
                    English = "Area of Influence Up",
                    French = "Aire d'effet augmentée",
                    German = "Erweiterter Radius",
                    Japanese = "アクション効果範囲拡大",
                    Korean = "기술 범위 확대"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(619, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Burns",
                    English = "Burns",
                    French = "Brûlure",
                    German = "Brandwunde",
                    Japanese = "火傷",
                    Korean = "화상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(620, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Pacification",
                    English = "Pacification",
                    French = "Pacification",
                    German = "Pacem",
                    Japanese = "ＷＳ不可",
                    Korean = "무기 기술 사용불가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(621, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fire Resistance Down",
                    English = "Fire Resistance Down",
                    French = "Résistance au feu diminuée",
                    German = "Feuerresistenz -",
                    Japanese = "火属性耐性低下",
                    Korean = "불속성 저항 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(622, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rotting Lungs",
                    English = "Rotting Lungs",
                    French = "Gaz putride",
                    German = "Verrottende Lunge",
                    Japanese = "ロットガス",
                    Korean = "부패 가스"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(623, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Disease",
                    English = "Disease",
                    French = "Maladie",
                    German = "Krankheit",
                    Japanese = "病気",
                    Korean = "질병"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(624, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Flesh Wound",
                    English = "Flesh Wound",
                    French = "Blessure physique",
                    German = "Fleischwunde",
                    Japanese = "切傷",
                    Korean = "절상"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(625, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Down for the Count",
                    English = "Down for the Count",
                    French = "Au tapis",
                    German = "Am Boden",
                    Japanese = "ノックダウン",
                    Korean = "넉다운"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(626, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Out of the Action",
                    English = "Out of the Action",
                    French = "Actions bloquées",
                    German = "Außer Gefecht",
                    Japanese = "アクション実行不可",
                    Korean = "기술 실행 불가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(627, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Regen",
                    English = "Regen",
                    French = "Récup",
                    German = "Regena",
                    Japanese = "リジェネ",
                    Korean = "리제네"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(628, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Medica II",
                    English = "Medica II",
                    French = "Extra Médica",
                    German = "Resedra",
                    Japanese = "メディカラ",
                    Korean = "메디카라"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(629, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Febrile",
                    English = "Febrile",
                    French = "Infirmité",
                    German = "Fiebrig",
                    Japanese = "虚弱",
                    Korean = "허약"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(630, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Heavy",
                    French = "Pesanteur",
                    German = "Gewicht",
                    Japanese = "ヘヴィ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(631, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Tireless",
                    French = "Infatigable",
                    German = "Beflügelt",
                    Japanese = "体力消耗無効"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(632, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Frenzied",
                    French = "Fébrilité",
                    German = "Raserei",
                    Japanese = "興奮"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(636, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Brand of the Sullen",
                    English = "Brand of the Sullen",
                    French = "Marque de la désolation",
                    German = "Mal des Leids",
                    Japanese = "悲嘆の烙印",
                    Korean = "비탄의 낙인"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(637, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Brand of the Ireful",
                    English = "Brand of the Ireful",
                    French = "Marque de la fureur",
                    German = "Mal des Zorns",
                    Japanese = "憤怒の烙印",
                    Korean = "분노의 낙인"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(638, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Vulnerability Up",
                    English = "Vulnerability Up",
                    French = "Vulnérabilité augmentée",
                    German = "Erhöhte Verwundbarkeit",
                    Japanese = "被ダメージ上昇",
                    Korean = "받는 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(639, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Pyretic",
                    English = "Pyretic",
                    French = "Pyromanie",
                    German = "Pyretisch",
                    Japanese = "ヒート",
                    Korean = "열병"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(640, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Poison Resistance Up",
                    English = "Poison Resistance Up",
                    French = "Résistance au poison accrue",
                    German = "Giftresistenz +",
                    Japanese = "毒耐性向上",
                    Korean = "독 저항 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(641, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Choco Reflect",
                    French = "Chocoboomerang",
                    German = "Chocobo-Reflektion",
                    Japanese = "弱体効果反射"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(642, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bleeding",
                    English = "Bleeding",
                    French = "Saignant",
                    German = "Blutung",
                    Japanese = "ペイン",
                    Korean = "고통"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(643, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Bleeding",
                    English = "Bleeding",
                    French = "Saignant",
                    German = "Blutung",
                    Japanese = "ペイン",
                    Korean = "고통"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(644, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Chicken",
                    English = "Chicken",
                    French = "Poulet",
                    German = "Huhn",
                    Japanese = "ニワトリ",
                    Korean = "닭"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(645, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Digesting",
                    English = "Digesting",
                    French = "Digestion",
                    German = "Verdauung",
                    Japanese = "消化中",
                    Korean = "소화 중"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(646, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Abandonment",
                    English = "Abandonment",
                    French = "Isolement",
                    German = "Verlassen",
                    Japanese = "孤独感",
                    Korean = "고독감"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(647, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Atrophy",
                    English = "Atrophy",
                    French = "Épuisement",
                    German = "Atrophie",
                    Japanese = "フィジカルダウン",
                    Korean = "능력치 저하"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(648, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Rehabilitation",
                    English = "Rehabilitation",
                    French = "Récup",
                    German = "Rehabilitation",
                    Japanese = "徐々にＨＰ回復",
                    Korean = "서서히 HP 회복"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(649, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Attack Up",
                    English = "Attack Up",
                    French = "Bonus d'attaque",
                    German = "Attacke-Bonus",
                    Japanese = "物理攻撃力アップ",
                    Korean = "물리 공격력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(650, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Attack Magic Potency Up",
                    English = "Attack Magic Potency Up",
                    French = "Bonus de puissance magique",
                    German = "Offensivmagie-Bonus",
                    Japanese = "魔法攻撃力アップ",
                    Korean = "마법 공격력 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(651, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Haste",
                    English = "Haste",
                    French = "Hâte",
                    German = "Hast",
                    Japanese = "ヘイスト",
                    Korean = "헤이스트"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(652, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "HP & MP Boost",
                    English = "HP & MP Boost",
                    French = "Bonus de PV et PM",
                    German = "LP-/MP-Bonus",
                    Japanese = "最大ＨＰアップ＆最大ＭＰアップ",
                    Korean = "최대 HP 및 MP 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(653, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Battle High",
                    English = "Battle High",
                    French = "Ivresse du combat",
                    German = "Euphorie",
                    Japanese = "戦意高揚",
                    Korean = "투쟁심 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(654, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Battle Fever",
                    English = "Battle Fever",
                    French = "Fièvre du combat",
                    German = "Raserei",
                    Japanese = "戦意高揚［強］",
                    Korean = "투쟁심 폭발"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(655, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Aegis Boon",
                    English = "Aegis Boon",
                    French = "Égide",
                    German = "Ägidensegen",
                    Japanese = "イージスブーン",
                    Korean = "이지스의 은혜"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(656, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Invincibility",
                    English = "Invincibility",
                    French = "Invulnérable",
                    German = "Unverwundbar",
                    Japanese = "無敵",
                    Korean = "무적"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(657, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Physical Vulnerability Up",
                    English = "Physical Vulnerability Up",
                    French = "Vulnérabilité physique augmentée",
                    German = "Erhöhte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ増加",
                    Korean = "물리 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(658, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Magic Vulnerability Up",
                    English = "Magic Vulnerability Up",
                    French = "Vulnérabilité magique augmentée",
                    German = "Erhöhte Magie-Verwundbarkeit",
                    Japanese = "被魔法ダメージ増加",
                    Korean = "마법 피해 증가"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(659, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Blight",
                    English = "Blight",
                    French = "Supplice",
                    German = "Pesthauch",
                    Japanese = "クラウダ",
                    Korean = "독안개"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(660, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Extend",
                    English = "Extend",
                    French = "Prolongation",
                    German = "Zeitdehnung",
                    Japanese = "エテンド",
                    Korean = "시간 연장"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(661, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Double",
                    English = "Double",
                    French = "Double",
                    German = "Doppel",
                    Japanese = "ダブル",
                    Korean = "이중 공격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(662, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Triple",
                    English = "Triple",
                    French = "Triple",
                    German = "Tripel",
                    Japanese = "トリプル",
                    Korean = "삼중 공격"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(664, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Prey",
                    English = "Prey",
                    French = "Marquage",
                    German = "Markiert",
                    Japanese = "マーキング",
                    Korean = "표식"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(665, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Slippery Prey",
                    English = "Slippery Prey",
                    French = "Marquage impossible",
                    German = "Unmarkierbar",
                    Japanese = "マーキング対象外",
                    Korean = "표식 대상 제외"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(666, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "感電",
                    English = "Electrocution",
                    French = "Électrocution",
                    German = "Stromschlag",
                    Japanese = "感電",
                    Korean = "感電"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(667, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fetters",
                    English = "Fetters",
                    French = "Attache",
                    German = "Gefesselt",
                    Japanese = "拘束",
                    Korean = "구속"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(668, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fetters",
                    English = "Fetters",
                    French = "Attache",
                    German = "Gefesselt",
                    Japanese = "拘束",
                    Korean = "구속"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(669, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Movement Speed Up",
                    English = "Movement Speed Up",
                    French = "Vitesse de déplacement accrue",
                    German = "Geschwindigkeit +",
                    Japanese = "移動速度上昇",
                    Korean = "이동 속도 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(670, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "Fire Resistance Down",
                    English = "Fire Resistance Down",
                    French = "Résistance au feu diminuée",
                    German = "Feuerresistenz -",
                    Japanese = "火属性耐性低下",
                    Korean = "불속성 저항 감소"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(671, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "无敌",
                    English = "Invincibility",
                    French = "Invulnérable",
                    German = "Unverwundbar",
                    Japanese = "無敵",
                    Korean = "무적"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(672, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Chinese = "伤害上升",
                    English = "Damage Up",
                    French = "Bonus de dégâts",
                    German = "Schaden +",
                    Japanese = "ダメージ上昇",
                    Korean = "공격 피해량 상승"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(676, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Concealed",
                    French = "Camouflage",
                    German = "Verborgen",
                    Japanese = "潜伏"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(677, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Concentrated Poison",
                    French = "Poison concentré",
                    German = "Starkes Gift",
                    Japanese = "劇毒"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(678, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Tailwind",
                    French = "Vent arrière",
                    German = "Rückenwind",
                    Japanese = "帯風"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(679, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Windwall",
                    French = "Mur de vent",
                    German = "Windmauer",
                    Japanese = "風壁"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(680, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Directional Parry",
                    French = "Parade directionnelle",
                    German = "Gerichtete Parade",
                    Japanese = "受け流し"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(681, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Offensive Optimization",
                    French = "Optimisation offensive",
                    German = "Offensivhaltung",
                    Japanese = "攻撃形態"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(682, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Defensive Optimization",
                    French = "Optimisation défensive",
                    German = "Defensivhaltung",
                    Japanese = "防御形態"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(683, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Blessing of Earth",
                    French = "Bénédiction de la terre",
                    German = "Segen der Erde",
                    Japanese = "土の加護"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(684, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Blessing of Fire",
                    French = "Bénédiction du feu",
                    German = "Segen des Feuers",
                    Japanese = "火の加護"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(685, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Wind Resistance Down",
                    French = "Résistance au vent diminuée",
                    German = "Windresistenz -",
                    Japanese = "風属性耐性低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(686, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Poison",
                    French = "Poison",
                    German = "Gift",
                    Japanese = "毒"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(688, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Hypercharge",
                    French = "Hypercharge",
                    German = "Hyperladung",
                    Japanese = "ハイパーチャージ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(689, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Mana Capacitor",
                    French = "Condensateur d'éther",
                    German = "Mana-Kondensator",
                    Japanese = "マナキャパシター"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(690, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Muscle Activator",
                    French = "Activateur de muscle",
                    German = "Muskelaktivator",
                    Japanese = "マッスルアクティベーター"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(695, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Physical Vulnerability Up",
                    French = "Vulnérabilité physique augmentée",
                    German = "Erhöhte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ増加"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(696, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Damage Down",
                    French = "Malus de dégâts",
                    German = "Schaden -",
                    Japanese = "ダメージ低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(697, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Healing Magic Down",
                    French = "Malus de soin",
                    German = "Heilmagie -",
                    Japanese = "回復魔法効果低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(701, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Battle Efficiency Down",
                    French = "Efficacité de combat diminuée",
                    German = "Verringerte Kampffähigkeit",
                    Japanese = "戦闘能力低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(702, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Bloated",
                    French = "Ballonnement",
                    German = "Zyklonische Separation",
                    Japanese = "ガス吸引"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(703, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Draconian Gaze",
                    French = "Regard draconique",
                    German = "Siegel des Auges",
                    Japanese = "竜眼の封印"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(704, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Draconian Light",
                    French = "Lumière draconique",
                    German = "Schutz des Auges",
                    Japanese = "竜眼の加護"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(705, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Transfiguration",
                    French = "Transformation",
                    German = "Verwandlung",
                    Japanese = "変身"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(714, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Vulnerability Up",
                    French = "Vulnérabilité augmentée",
                    German = "Erhöhte Verwundbarkeit",
                    Japanese = "被ダメージ上昇"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(715, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Staggered",
                    French = "Chancellement",
                    German = "Schwankend",
                    Japanese = "よろめき"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(716, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Turbulence",
                    French = "Turbulence",
                    German = "Turbulenzen",
                    Japanese = "乱気流"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(717, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Will of the Wind",
                    French = "Domination du vent",
                    German = "Herrschaft des Windes",
                    Japanese = "風の支配"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(718, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Will of the Water",
                    French = "Domination de l'eau",
                    German = "Herrschaft des Wassers",
                    Japanese = "水の支配"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(719, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Whaleback",
                    French = "Attaque directe",
                    German = "In Schusslinie",
                    Japanese = "直接攻撃"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(720, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Slashing Resistance Up",
                    French = "Résistance au tranchant accrue",
                    German = "Hiebresistenz +",
                    Japanese = "斬属性耐性向上"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(721, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Piercing Resistance Up",
                    French = "Résistance au perforant accrue",
                    German = "Stichresistenz +",
                    Japanese = "突属性耐性向上"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(722, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Blunt Resistance Up",
                    French = "Résistance au contondant accrue",
                    German = "Schlagresistenz +",
                    Japanese = "打属性耐性向上"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(723, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Aetherochemical Bomb",
                    French = "Magismobombe",
                    German = "Ätherochemischer Sprengkörper",
                    Japanese = "魔爆弾"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(725, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Goring Blade",
                    French = "Lame étripante",
                    German = "Ausweiden",
                    Japanese = "ゴアブレード"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(726, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Divine Veil",
                    French = "Voile divin",
                    German = "Heiliger Quell",
                    Japanese = "ディヴァインヴェール"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(727, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Divine Veil",
                    French = "Voile divin",
                    German = "Heiliger Quell",
                    Japanese = "ディヴァインヴェール［バリア］"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(728, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Sheltron",
                    French = "Schiltron",
                    German = "Schiltron",
                    Japanese = "シェルトロン"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(729, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Deliverance",
                    French = "Délivrance",
                    German = "Erlöser",
                    Japanese = "デストロイヤー"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(730, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Abandon",
                    French = "Abandonnement",
                    German = "Abkehr",
                    Japanese = "アバンドン"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(731, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Abandon II",
                    French = "Abandonnement II",
                    German = "Abkehr II",
                    Japanese = "アバンドンII"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(732, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Abandon III",
                    French = "Abandonnement III",
                    German = "Abkehr III",
                    Japanese = "アバンドンIII"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(733, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Abandon IV",
                    French = "Abandonnement IV",
                    German = "Abkehr IV",
                    Japanese = "アバンドンIV"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(734, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Uncontrollable",
                    French = "Abandonnement V",
                    German = "Abkehr V",
                    Japanese = "アバンドンV"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(735, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Raw Intuition",
                    French = "Intuition pure",
                    German = "Ur-Instinkt",
                    Japanese = "原初の直感"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(736, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Blood of the Dragon",
                    French = "Sang du dragon",
                    German = "Drachenherz",
                    Japanese = "蒼の竜血"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(737, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Ley Lines",
                    French = "Manalignements",
                    German = "Ley-Linien",
                    Japanese = "黒魔紋"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(738, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Circle of Power",
                    French = "Manalignements",
                    German = "Ley-Linien",
                    Japanese = "黒魔紋：効果"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(739, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Asylum",
                    French = "Asile",
                    German = "Refugium",
                    Japanese = "アサイラム"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(740, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Shadowskin",
                    French = "Peau d'ombre",
                    German = "Schattenhaut",
                    Japanese = "シャドウスキン"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(741, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Scourge",
                    French = "Fatalité",
                    German = "Geißel",
                    Japanese = "スカージ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(742, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Blood Weapon",
                    French = "Arme de sang",
                    German = "Blutwaffe",
                    Japanese = "ブラッドウェポン"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(743, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Grit",
                    French = "Férocité",
                    German = "Zähigkeit",
                    Japanese = "グリットスタンス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(744, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Dark Dance",
                    French = "Danse ténébreuse",
                    German = "Dunkler Tanz",
                    Japanese = "ダークダンス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(745, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Blood Price",
                    French = "Prix du sang",
                    German = "Blutzoll",
                    Japanese = "ブラッドプライス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(746, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Dark Mind",
                    French = "Esprit ténébreux",
                    German = "Dunkler Geist",
                    Japanese = "ダークマインド"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(747, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Shadow Wall",
                    French = "Mur d'ombre",
                    German = "Schattenwand",
                    Japanese = "シャドウウォール"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(748, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Delirium",
                    French = "Délirium",
                    German = "Delirium",
                    Japanese = "デリリアムブレード"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(749, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Salted Earth",
                    French = "Terre salée",
                    German = "Salzige Erde",
                    Japanese = "ソルトアース"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(750, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Another Victim",
                    French = "Autre victime",
                    German = "Einziger Überlebender",
                    Japanese = "ソウルサバイバー"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(751, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Darkside",
                    French = "Ténèbres intérieures",
                    German = "Dunkle Seite",
                    Japanese = "暗黒"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(752, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Dark Arts",
                    French = "Arts ténébreux",
                    German = "Dunkle Künste",
                    Japanese = "ダークアーツ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(753, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Reprisal",
                    French = "Rétorsion",
                    German = "Reflexion",
                    Japanese = "リプライザル"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(754, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Gathering Rate Up (Limited)",
                    French = "Récolte améliorée (limitée)",
                    German = "Sammelrate erhöht (Limit)",
                    Japanese = "採集獲得率アップ：限定"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(755, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Gathering Fortune Up (Limited)",
                    French = "Récolte de qualité (limitée)",
                    German = "Sammelglück erhöht (Limit)",
                    Japanese = "採集HQ獲得率アップ：限定"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(756, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Gathering Yield Up (Limited)",
                    French = "Récolte abondante (limitée)",
                    German = "Sammelgewinn erhöht (Limit)",
                    Japanese = "採集獲得数アップ：限定"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(757, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Discerning Eye",
                    French = "Œil perspicace",
                    German = "Kennerblick",
                    Japanese = "審美眼"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(758, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Utmost Caution",
                    French = "Attention maximale",
                    German = "Äußerste Sorgfalt",
                    Japanese = "コーション"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(759, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Deep Breath",
                    French = "Respiration profonde",
                    German = "Tiefes Durchatmen",
                    Japanese = "ディープブレス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(760, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Single Mind",
                    French = "Esprit résolu",
                    German = "Fünf Sinne",
                    Japanese = "シングルマインド"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(761, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Snagging",
                    French = "Casaquage",
                    German = "Reißen",
                    Japanese = "引掛釣り"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(762, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Fish Eyes",
                    French = "Yeux de poisson",
                    German = "Fischaugen",
                    Japanese = "フィッシュアイ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(763, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Chum",
                    French = "Amorçage",
                    German = "Streuköder",
                    Japanese = "撒き餌"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(764, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Inefficient Hooking",
                    French = "Chances de ferrage réduites",
                    German = "Schlechter Anschlag",
                    Japanese = "フッキング成功率低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(765, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Catch and Release",
                    French = "Pêche et remise à l'eau",
                    German = "Fangen und Freilassen",
                    Japanese = "最小サイズ向上"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(769, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Burning Chains",
                    French = "Chaînes brûlantes",
                    German = "Brennende Ketten",
                    Japanese = "炎の鎖"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(770, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Fetters",
                    French = "Attache",
                    German = "Gefesselt",
                    Japanese = "拘束"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(783, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Down for the Count",
                    French = "Au tapis",
                    German = "Am Boden",
                    Japanese = "ノックダウン"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(784, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Voidblood",
                    French = "Sang du néant",
                    German = "Nichtsblut",
                    Japanese = "妖異の血"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(785, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Nymian Plague",
                    French = "Lèpre du tomberry",
                    German = "Nym-Pest",
                    Japanese = "トンベリ病"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(786, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Battle Litany",
                    French = "Litanie combattante",
                    German = "Litanei der Schlacht",
                    Japanese = "バトルリタニー"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(787, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Silhouette",
                    French = "Silhouette",
                    German = "Schattenwandler",
                    Japanese = "影渡［被］"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(788, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Shadewalker",
                    French = "Manipulateur d'ombre",
                    German = "Schattenwandler",
                    Japanese = "影渡"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(789, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Smoke Screen",
                    French = "Écran de fumée",
                    German = "Rauchschwaden",
                    Japanese = "煙玉"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(790, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Duality",
                    French = "Dualité",
                    German = "Dualität",
                    Japanese = "一双"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(791, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Dissipation",
                    French = "Dissipation",
                    German = "Dissipation",
                    Japanese = "転化"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(792, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Emergency Tactics",
                    French = "Stratagème de l'urgence",
                    German = "Apotropaion",
                    Japanese = "応急戦術"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(793, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "First Chakra",
                    French = "Premier chakra",
                    German = "Erstes Schatten-Chakra",
                    Japanese = "闘気"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(794, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Second Chakra",
                    French = "Deuxième chakra",
                    German = "Zweites Schatten-Chakra",
                    Japanese = "闘気II"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(795, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Third Chakra",
                    French = "Troisième chakra",
                    German = "Drittes Schatten-Chakra",
                    Japanese = "闘気III"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(796, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Fourth Chakra",
                    French = "Quatrième chakra",
                    German = "Viertes Schatten-Chakra",
                    Japanese = "闘気IV"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(797, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Fifth Chakra",
                    French = "Cinquième chakra",
                    German = "Fünftes Schatten-Chakra",
                    Japanese = "闘気V"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(798, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Aero III",
                    French = "Méga Vent",
                    German = "Windga",
                    Japanese = "エアロガ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(799, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Fey Wind",
                    French = "Vent féerique",
                    German = "Feenwind",
                    Japanese = "フェイウィンド"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(800, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Fetters",
                    French = "Attache",
                    German = "Gefesselt",
                    Japanese = "拘束"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(801, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Poison",
                    French = "Poison",
                    German = "Gift",
                    Japanese = "毒"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(802, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Sharper Fang and Claw",
                    French = "Croc-et-griffe",
                    German = "Fang und Klaue",
                    Japanese = "竜牙竜爪効果アップ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(803, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Enhanced Wheeling Thrust",
                    French = "Percée tournante",
                    German = "Fächerstoß",
                    Japanese = "竜尾大車輪効果アップ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(804, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Vulnerability Down",
                    French = "Vulnérabilité diminuée",
                    German = "Verringerte Verwundbarkeit",
                    Japanese = "被ダメージ低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(805, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Collector's Glove",
                    French = "Gant de collectionneur",
                    German = "Sammlergespür",
                    Japanese = "蒐集品採集"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(806, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Vulnerability Up",
                    French = "Vulnérabilité augmentée",
                    German = "Erhöhte Verwundbarkeit",
                    Japanese = "被ダメージ上昇"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(807, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Aethertrail Attunement",
                    French = "Éther de Bahamut",
                    German = "Bahamut-Äther",
                    Japanese = "バハムートエーテル"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(808, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Dreadwyrm Trance",
                    French = "Transe-Bahamut",
                    German = "Bahamut-Trance",
                    Japanese = "トランス・バハムート"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(809, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Slime",
                    French = "Mucus",
                    German = "Schleim",
                    Japanese = "粘液"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(810, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Living Dead",
                    French = "Mort-vivant",
                    German = "Totenerweckung",
                    Japanese = "リビングデッド"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(811, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Walking Dead",
                    French = "Marcheur des limbes",
                    German = "Erweckter",
                    Japanese = "ウォーキングデッド"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(812, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Magic Vulnerability Down",
                    French = "Vulnérabilité magique diminuée",
                    German = "Verringerte Magie-Verwundbarkeit",
                    Japanese = "被魔法ダメージ軽減"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(813, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Dark Dance",
                    French = "Danse ténébreuse",
                    German = "Dunkler Tanz",
                    Japanese = "ダークダンス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(814, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Enhanced Unleash",
                    French = "Déchaînement déchaîné",
                    German = "Entfesselung",
                    Japanese = "アンリーシュ効果アップ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(815, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Enhanced Benefic II",
                    French = "Bienfaisance II bénéfique",
                    German = "Harmonie",
                    Japanese = "ベネフィラ効果アップ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(816, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Enhanced Royal Road",
                    French = "Voie royale améliorée",
                    German = "Königsweg der Stärke",
                    Japanese = "ロイヤルロード：効果量増加"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(817, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Expanded Royal Road",
                    French = "Voie royale élargie",
                    German = "Königsweg der Umsicht",
                    Japanese = "ロイヤルロード：効果範囲化"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(818, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Extended Royal Road",
                    French = "Voie royale doublée",
                    German = "Königsweg der Geduld",
                    Japanese = "ロイヤルロード：効果時間増加"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(826, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Card Drawn",
                    French = "Tirage",
                    German = "Gezogene Karte",
                    Japanese = "ドロー"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(827, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Royal Road",
                    French = "Voie royale",
                    German = "Königsweg",
                    Japanese = "ロイヤルロード"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(828, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Card Held",
                    French = "Ajout",
                    German = "Abgelegte Karte",
                    Japanese = "キープ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(829, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "The Balance",
                    French = "La Balance",
                    German = "Kraft der Waage",
                    Japanese = "アーゼマの均衡"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(830, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "The Bole",
                    French = "Le Tronc",
                    German = "Kraft der Eiche",
                    Japanese = "世界樹の幹"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(831, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "The Arrow",
                    French = "La Flèche",
                    German = "Kraft des Pfeils",
                    Japanese = "オシュオンの矢"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(832, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "The Spear",
                    French = "L'Épieu",
                    German = "Kraft des Speers",
                    Japanese = "ハルオーネの槍"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(833, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "The Ewer",
                    French = "L'Aiguière",
                    German = "Kraft des Krugs",
                    Japanese = "サリャクの水瓶"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(834, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "The Spire",
                    French = "La Tour",
                    German = "Kraft des Turms",
                    Japanese = "ビエルゴの塔"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(835, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Aspected Benefic",
                    French = "Bienfaisance aspect",
                    German = "Harmonischer Orbis",
                    Japanese = "アスペクト・ベネフィク"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(836, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Aspected Helios",
                    French = "Hélios aspect",
                    German = "Aspektierter Helios",
                    Japanese = "アスペクト・ヘリオス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(837, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Nocturnal Field",
                    French = "Champ nocturne",
                    German = "Nocturnales Feld",
                    Japanese = "ノクターナルフィールド"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(838, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Combust",
                    French = "Conjonction supérieure",
                    German = "Konjunktion",
                    Japanese = "コンバス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(839, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Diurnal Sect",
                    French = "Thème diurne",
                    German = "Diurnal",
                    Japanese = "ダイアーナルセクト"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(840, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Nocturnal Sect",
                    French = "Thème nocturne",
                    German = "Nocturnal",
                    Japanese = "ノクターナルセクト"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(841, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Lightspeed",
                    French = "Vitesse de la lumière",
                    German = "Lichtgeschwindigkeit",
                    Japanese = "ライトスピード"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(842, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Luminiferous Aether",
                    French = "Éther luminifère",
                    German = "Lichtäther",
                    Japanese = "ルミナスエーテル"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(843, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Combust II",
                    French = "Conjonction supérieure II",
                    German = "Konjunktion II",
                    Japanese = "コンバラ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(844, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Disabled",
                    French = "Invalidation",
                    German = "Inakt",
                    Japanese = "ドンアク"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(845, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Synastry",
                    French = "Synastrie",
                    German = "Synastrie",
                    Japanese = "シナストリー"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(846, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Synastry",
                    French = "Synastrie",
                    German = "Synastrie",
                    Japanese = "シナストリー［被］"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(847, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Collective Unconscious",
                    French = "Inconscient collectif",
                    German = "Numinosum",
                    Japanese = "運命の輪"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(848, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Collective Unconscious",
                    French = "Inconscient collectif",
                    German = "Numinosum",
                    Japanese = "運命の輪"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(849, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Collective Unconscious",
                    French = "Inconscient collectif",
                    German = "Numinosum",
                    Japanese = "運命の輪：効果"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(850, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Gathering Fortune Up",
                    French = "Récolte de qualité",
                    German = "Sammelglück erhöht",
                    Japanese = "採集HQ獲得率アップ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(851, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Reassembled",
                    French = "Réassemblage",
                    German = "Justiert",
                    Japanese = "整備"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(852, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Promotion",
                    French = "Sous-promotion",
                    German = "Umwandlung",
                    Japanese = "プロモーション"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(853, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Rapid Fire",
                    French = "Feu rapide",
                    German = "Schnellfeuer",
                    Japanese = "ラピッドファイア"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(854, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Lead Shot",
                    French = "Tir de plombs",
                    German = "Parabelschuss",
                    Japanese = "レッドショット"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(855, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Hot Shot",
                    French = "Tir déchaîné",
                    German = "Explosivgeschoss",
                    Japanese = "ホットショット"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(856, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Enhanced Slug Shot",
                    French = "Tir de balle amélioré",
                    German = "Verbessertes Flintenlaufgeschoss",
                    Japanese = "スラッグショット効果アップ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(857, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Cleaner Shot",
                    French = "Tir net amélioré",
                    German = "Verbesserter Sauberer Schuss",
                    Japanese = "クリーンショット効果アップ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(858, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Gauss Barrel",
                    French = "Canon Gauss",
                    German = "Gauß-Laufaufsatz",
                    Japanese = "ガウスバレル"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(859, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Rent Mind",
                    French = "Brise-esprit",
                    German = "Zerrüttet",
                    Japanese = "マインドブレイク"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(860, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Dismantled",
                    French = "Brise-arme",
                    German = "Zerlegt",
                    Japanese = "ウェポンブレイク"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(861, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Wildfire",
                    French = "Flambée",
                    German = "Wildfeuer",
                    Japanese = "ワイルドファイア"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(862, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Ammunition Loaded",
                    French = "Munitions spéciales",
                    German = "Spezialprojektil geladen",
                    Japanese = "特殊弾"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(863, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Land Waker",
                    French = "Terre vierge",
                    German = "Erdbrecher",
                    Japanese = "原初の大地"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(864, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Dark Force",
                    French = "Force des ténèbres",
                    German = "Dunkle Macht",
                    Japanese = "ダークフォース"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(865, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "The Wanderer's Minuet",
                    French = "Menuet du Vagabond",
                    German = "Menuett des Wanderers",
                    Japanese = "旅神のメヌエット"
                },
                CompanyAction = true
            });
            StatusEffects.TryAdd(866, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "The Warden's Paean",
                    French = "Péan du Contemplateur",
                    German = "Päan des Hüters",
                    Japanese = "時神のピーアン"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(867, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Sharpcast",
                    French = "Dynamisation",
                    German = "Augmentierung",
                    Japanese = "激成魔"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(868, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Enochian",
                    French = "Énochien",
                    German = "Henochisch",
                    Japanese = "エノキアン"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(869, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Carnal Chill",
                    French = "Froideur charnelle",
                    German = "Greifbares Grauen",
                    Japanese = "カーナルチル"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(870, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Push Back",
                    French = "Repoussée",
                    German = "Abstoßung",
                    Japanese = "プッシュバック"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(871, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Name of the Wind",
                    French = "Nom de vent",
                    German = "Name des Winds",
                    Japanese = "アート・オブ・ウィンド"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(872, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Name of Fire",
                    French = "Nom de feu",
                    German = "Name des Feuers",
                    Japanese = "アート・オブ・ファイア"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(873, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Name of Ice",
                    French = "Nom de glace",
                    German = "Name des Eises",
                    Japanese = "アート・オブ・アイス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(874, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Name of Earth",
                    French = "Nom de terre",
                    German = "Name der Erde",
                    Japanese = "アート・オブ・アース"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(875, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Name of Lightning",
                    French = "Nom de foudre",
                    German = "Name des Blitzes",
                    Japanese = "アート・オブ・ライトニング"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(876, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Name of Water",
                    French = "Nom d'eau",
                    German = "Name des Wassers",
                    Japanese = "アート・オブ・ウォーター"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(877, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Nameless",
                    French = "Sans nom",
                    German = "Namenlos",
                    Japanese = "アート不可"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(878, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Maker's Mark",
                    French = "Marque du fabricant",
                    German = "Kunst des Kundigen",
                    Japanese = "堅実の心得"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(879, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Crafter's Soul",
                    French = "Âme d'artisan",
                    German = "Seele des Handwerkers",
                    Japanese = "職人の魂"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(880, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Whistle",
                    French = "Siffler en travaillant",
                    German = "Trällern",
                    Japanese = "仕事唄"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(881, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Traders' Favor (Coerthas Western Highlands)",
                    French = "Talisman des Marchands (Coerthas occidental)",
                    German = "Nald'thals Gunst (Westliches Hochland von Coerthas)",
                    Japanese = "ナルザルの護符：クルザス西部高地"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(882, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Traders' Favor (Dravanian Hinterlands)",
                    French = "Talisman des Marchands (Avant-pays dravanien)",
                    German = "Nald'thals Gunst (Dravanisches Hinterland)",
                    Japanese = "ナルザルの護符：高地ドラヴァニア"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(883, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Traders' Favor (Dravanian Forelands)",
                    French = "Talisman des Marchands (Arrière-pays dravanien)",
                    German = "Nald'thals Gunst (Dravanisches Vorland)",
                    Japanese = "ナルザルの護符：低地ドラヴァニア"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(884, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Traders' Favor (The Churning Mists)",
                    French = "Talisman des Marchands (Cieux de Dravania)",
                    German = "Nald'thals Gunst (Wallende Nebel)",
                    Japanese = "ナルザルの護符：ドラヴァニア雲海"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(885, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Traders' Favor (The Sea of Clouds)",
                    French = "Talisman des Marchands (Cieux d'Abalathia)",
                    German = "Nald'thals Gunst (Abalathisches Wolkenmeer)",
                    Japanese = "ナルザルの護符：アバラシア雲海"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(886, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Traders' Favor (Azys Lla)",
                    French = "Talisman des Marchands (Azys Lla)",
                    German = "Nald'thals Gunst (Azys Lla)",
                    Japanese = "ナルザルの護符：魔大陸アジス・ラー"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(887, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Matron's Favor (Coerthas Western Highlands)",
                    French = "Talisman de la Mère (Coerthas occidental)",
                    German = "Nophicas Gunst (Westliches Hochland von Coerthas)",
                    Japanese = "ノフィカの護符：クルザス西部高地"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(888, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Matron's Favor (Dravanian Hinterlands)",
                    French = "Talisman de la Mère (Avant-pays dravanien)",
                    German = "Nophicas Gunst (Dravanisches Hinterland)",
                    Japanese = "ノフィカの護符：高地ドラヴァニア"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(889, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Matron's Favor (Dravanian Forelands)",
                    French = "Talisman de la Mère (Arrière-pays dravanien)",
                    German = "Nophicas Gunst (Dravanisches Vorland)",
                    Japanese = "ノフィカの護符：低地ドラヴァニア"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(890, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Matron's Favor (The Churning Mists)",
                    French = "Talisman de la Mère (Cieux de Dravania)",
                    German = "Nophicas Gunst (Wallende Nebel)",
                    Japanese = "ノフィカの護符：ドラヴァニア雲海"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(891, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Matron's Favor (The Sea of Clouds)",
                    French = "Talisman de la Mère (Cieux d'Abalathia)",
                    German = "Nophicas Gunst (Abalathisches Wolkenmeer)",
                    Japanese = "ノフィカの護符：アバラシア雲海"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(892, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Matron's Favor (Azys Lla)",
                    French = "Talisman de la Mère (Azys Lla)",
                    German = "Nophicas Gunst (Azys Lla)",
                    Japanese = "ノフィカの護符：魔大陸アジス・ラー"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(893, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Vulnerability Up",
                    French = "Vulnérabilité augmentée",
                    German = "Erhöhte Verwundbarkeit",
                    Japanese = "被ダメージ上昇"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(894, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Wind Resistance Down",
                    French = "Résistance au vent diminuée",
                    German = "Windresistenz -",
                    Japanese = "風属性耐性低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(895, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Invincibility",
                    French = "Invulnérable",
                    German = "Unverwundbar",
                    Japanese = "無敵"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(896, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Last Ditch",
                    French = "Ultime effort",
                    German = "Letzter Graben",
                    Japanese = "ラストディッチ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(897, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Regen",
                    French = "○削除予定",
                    German = "○削除予定",
                    Japanese = "○削除予定"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(898, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Lightning Resistance Down",
                    French = "Résistance à la foudre réduite",
                    German = "Blitzresistenz -",
                    Japanese = "雷属性耐性低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(899, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Physical Vulnerability Down",
                    French = "Vulnérabilité physique diminuée",
                    German = "Verringerte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ軽減"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(900, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Temporal Displacement",
                    French = "Arrêt du temps",
                    German = "Zeitriss",
                    Japanese = "時間停止"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(901, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Fetters",
                    French = "Attache",
                    German = "Gefesselt",
                    Japanese = "拘束"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(902, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Jackpot",
                    French = "Gros lot",
                    German = "MGP-Bonus",
                    Japanese = "カンパニーアクション：MGPアップ"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(903, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Collectable Synthesis",
                    French = "Synthèse collectionnable",
                    German = "Sammlerstück-Synthese",
                    Japanese = "蒐集品製作"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(904, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Prey",
                    French = "Marquage",
                    German = "Markiert",
                    Japanese = "マーキング"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(905, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Thin Ice",
                    French = "Glaciation",
                    German = "Überfroren",
                    Japanese = "凍結"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(906, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Darkness",
                    French = "Pouvoir des ténèbres",
                    German = "Dunkelheit",
                    Japanese = "暗黒の力"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(907, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Arcanum Blessing",
                    French = "Bénédiction des arcanes",
                    German = "Kraft der Arkana",
                    Japanese = "アルカナの祝福"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(908, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Aethertrail Attunement",
                    French = "Éther de Bahamut",
                    German = "Bahamut-Äther",
                    Japanese = "バハムートエーテル"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(909, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Refresh",
                    French = "Recharge",
                    German = "MP-Regeneration",
                    Japanese = "MP持続回復"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(910, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Doom",
                    French = "Glas",
                    German = "Todesurteil",
                    Japanese = "死の宣告"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(911, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Thin Ice",
                    French = "Verglas",
                    German = "Glatteis",
                    Japanese = "氷床"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(913, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Balance Drawn",
                    French = "Tirage: la Balance",
                    German = "Waage gezogen",
                    Japanese = "ドロー：アーゼマの均衡"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(914, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Bole Drawn",
                    French = "Tirage: le Tronc",
                    German = "Eiche gezogen",
                    Japanese = "ドロー：世界樹の幹"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(915, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Arrow Drawn",
                    French = "Tirage: la Flèche",
                    German = "Pfeil gezogen",
                    Japanese = "ドロー：オシュオンの矢"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(916, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Spear Drawn",
                    French = "Tirage: l'Épieu",
                    German = "Speer gezogen",
                    Japanese = "ドロー：ハルオーネの槍"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(917, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Ewer Drawn",
                    French = "Tirage: l'Aiguière",
                    German = "Krug gezogen",
                    Japanese = "ドロー：サリャクの水瓶"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(918, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Spire Drawn",
                    French = "Tirage: la Tour",
                    German = "Turm gezogen",
                    Japanese = "ドロー：ビエルゴの塔"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(919, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Heightened Visibility",
                    French = "Décèlement",
                    German = "Durchblick",
                    Japanese = "看破"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(920, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Balance Held",
                    French = "Ajout: la Balance",
                    German = "Waage abgelegt",
                    Japanese = "キープ：アーゼマの均衡"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(921, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Bole Held",
                    French = "Ajout: le Tronc",
                    German = "Eiche abgelegt",
                    Japanese = "キープ：世界樹の幹"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(922, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Arrow Held",
                    French = "Ajout: la Flèche",
                    German = "Pfeil abgelegt",
                    Japanese = "キープ：オシュオンの矢"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(923, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Spear Held",
                    French = "Ajout: l'Épieu",
                    German = "Speer abgelegt",
                    Japanese = "キープ：ハルオーネの槍"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(924, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Ewer Held",
                    French = "Ajout: l'Aiguière",
                    German = "Krug abgelegt",
                    Japanese = "キープ：サリャクの水瓶"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(925, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Spire Held",
                    French = "Ajout: la Tour",
                    German = "Turm abgelegt",
                    Japanese = "キープ：ビエルゴの塔"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(926, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Sleep",
                    French = "Sommeil",
                    German = "Schlaf",
                    Japanese = "睡眠"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(927, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Nectar",
                    French = "Nectar",
                    German = "Blütenhonig",
                    Japanese = "花蜜"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(928, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Black Menace",
                    French = "Menace fulgurante",
                    German = "Dunkle Drohung",
                    Japanese = "ブラックメナス"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(929, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Vulnerability Down",
                    French = "Vulnérabilité diminuée",
                    German = "Verringerte Verwundbarkeit",
                    Japanese = "被ダメージ低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(930, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Fetters",
                    French = "Attache",
                    German = "Gefesselt",
                    Japanese = "拘束"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(931, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Blunt Resistance Down",
                    French = "Résistance au contondant réduite",
                    German = "Schlagresistenz -",
                    Japanese = "打属性耐性低下"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(934, new StatusItem
            {
                Name = new StatusLocalization
                {
                    English = "Physical Vulnerability Up",
                    French = "Vulnérabilité physique augmentée",
                    German = "Erhöhte physische Verwundbarkeit",
                    Japanese = "被物理ダメージ増加"
                },
                CompanyAction = false
            });

            // Korean version exclusive exp and echo buff when you're on PC bang ( https://imgur.com/a/thmzB )
            StatusEffects.TryAdd(935, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Korean = "PC방 혜택"
                },
                CompanyAction = false
            });
            StatusEffects.TryAdd(936, new StatusItem
            {
                Name = new StatusLocalization
                {
                    Korean = "PC방 초월하는 힘"
                },
                CompanyAction = false
            });
        }
    }
}
