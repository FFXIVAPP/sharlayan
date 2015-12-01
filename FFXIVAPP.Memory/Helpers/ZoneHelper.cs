// FFXIVAPP.Memory
// FFXIVAPP & Related Plugins/Modules
// Copyright © 2007 - 2015 Ryan Wilson - All Rights Reserved
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace FFXIVAPP.Memory.Helpers
{
    public static class ZoneHelper
    {
        private static IList<MapInfo> _mapInfoList;

        private static IEnumerable<MapInfo> MapInfoList
        {
            get { return _mapInfoList ?? (_mapInfoList = GenerateMapList()); }
        }

        public static MapInfo GetMapInfo(uint index)
        {
            var mapInfo = MapInfoList.FirstOrDefault(m => m.Index == index);
            return mapInfo ?? new MapInfo(false, index);
        }

        private static IList<MapInfo> GenerateMapList()
        {
            var mapList = new List<MapInfo>
            {
                new MapInfo(false, 128)
                {
                    Chinese = "利姆萨·罗敏萨上层甲上层甲板",
                    English = "Limsa Lominsa Upper Decks",
                    French = "Le Tillac",
                    German = "Obere Decks",
                    Japanese = "リムサ・ロミンサ：上甲板層"
                },
                new MapInfo(false, 129)
                {
                    Chinese = "利姆萨·罗敏萨下层甲上层甲板",
                    English = "Limsa Lominsa Lower Decks",
                    French = "L'Entrepont",
                    German = "Untere Decks",
                    Japanese = "リムサ・ロミンサ：下甲板層"
                },
                new MapInfo(false, 130)
                {
                    Chinese = "乌尔达哈现世回廊",
                    English = "Ul'dah - Steps of Nald",
                    French = "Ul'dah - faubourg de Nald",
                    German = "Nald-Kreuzgang",
                    Japanese = "ウルダハ：ナル回廊"
                },
                new MapInfo(false, 131)
                {
                    Chinese = "乌尔达哈来生回廊",
                    English = "Ul'dah - Steps of Thal",
                    French = "Ul'dah - faubourg de Thal",
                    German = "Thal-Kreuzgang",
                    Japanese = "ウルダハ：ザル回廊"
                },
                new MapInfo(false, 132)
                {
                    Chinese = "格里达尼亚新街",
                    English = "New Gridania",
                    French = "Nouvelle Gridania",
                    German = "Neu-Gridania",
                    Japanese = "グリダニア：新市街"
                },
                new MapInfo(false, 133)
                {
                    Chinese = "格里达尼亚旧街",
                    English = "Old Gridania",
                    French = "Vieille Gridania",
                    German = "Alt-Gridania",
                    Japanese = "グリダニア：旧市街"
                },
                new MapInfo(false, 134)
                {
                    Chinese = "中拉诺西亚",
                    English = "Middle La Noscea",
                    French = "Noscea centrale",
                    German = "Zentrales La Noscea",
                    Japanese = "中央ラノシア"
                },
                new MapInfo(false, 135)
                {
                    Chinese = "拉诺西亚低地",
                    English = "Lower La Noscea",
                    French = "Basse-Noscea",
                    German = "Unteres La Noscea",
                    Japanese = "低地ラノシア"
                },
                new MapInfo(false, 137)
                {
                    Chinese = "东拉诺西亚",
                    English = "Eastern La Noscea",
                    French = "Noscea orientale",
                    German = "Östliches La Noscea",
                    Japanese = "東ラノシア"
                },
                new MapInfo(false, 138)
                {
                    Chinese = "西拉诺西亚",
                    English = "Western La Noscea",
                    French = "Noscea occidentale",
                    German = "Westliches La Noscea",
                    Japanese = "西ラノシア"
                },
                new MapInfo(false, 139)
                {
                    Chinese = "拉诺西亚高地",
                    English = "Upper La Noscea",
                    French = "Haute-Noscea",
                    German = "Oberes La Noscea",
                    Japanese = "高地ラノシア"
                },
                new MapInfo(false, 140)
                {
                    Chinese = "西萨纳兰",
                    English = "Western Thanalan",
                    French = "Thanalan occidental",
                    German = "Westliches Thanalan",
                    Japanese = "西ザナラーン"
                },
                new MapInfo(false, 141)
                {
                    Chinese = "中萨纳兰",
                    English = "Central Thanalan",
                    French = "Thanalan central",
                    German = "Zentrales Thanalan",
                    Japanese = "中央ザナラーン"
                },
                new MapInfo(false, 145)
                {
                    Chinese = "东萨纳兰",
                    English = "Eastern Thanalan",
                    French = "Thanalan oriental",
                    German = "Östliches Thanalan",
                    Japanese = "東ザナラーン"
                },
                new MapInfo(false, 146)
                {
                    Chinese = "南萨纳兰",
                    English = "Southern Thanalan",
                    French = "Thanalan méridional",
                    German = "Südliches Thanalan",
                    Japanese = "南ザナラーン"
                },
                new MapInfo(false, 147)
                {
                    Chinese = "北萨纳兰",
                    English = "Northern Thanalan",
                    French = "Thanalan septentrional",
                    German = "Nördliches Thanalan",
                    Japanese = "北ザナラーン"
                },
                new MapInfo(false, 148)
                {
                    Chinese = "黑衣森林中央林区",
                    English = "Central Shroud",
                    French = "Forêt centrale",
                    German = "Tiefer Wald",
                    Japanese = "黒衣森：中央森林"
                },
                new MapInfo(false, 152)
                {
                    Chinese = "黑衣森林东部林区",
                    English = "East Shroud",
                    French = "Forêt de l'est",
                    German = "Ostwald",
                    Japanese = "黒衣森：東部森林"
                },
                new MapInfo(false, 153)
                {
                    Chinese = "黑衣森林南部林区",
                    English = "South Shroud",
                    French = "Forêt du sud",
                    German = "Südwald",
                    Japanese = "黒衣森：南部森林"
                },
                new MapInfo(false, 154)
                {
                    Chinese = "黑衣森林北部林区",
                    English = "North Shroud",
                    French = "Forêt du nord",
                    German = "Nordwald",
                    Japanese = "黒衣森：北部森林"
                },
                new MapInfo(false, 155)
                {
                    Chinese = "库尔扎斯中央高地",
                    English = "Coerthas Central Highlands",
                    French = "Hautes terres du Coerthas central",
                    German = "Zentrales Hochland von Coerthas",
                    Japanese = "クルザス中央高地"
                },
                new MapInfo(false, 156)
                {
                    Chinese = "摩杜纳",
                    English = "Mor Dhona",
                    French = "Mor Dhona",
                    German = "Mor Dhona",
                    Japanese = "モードゥナ"
                },
                new MapInfo(true, 157)
                {
                    Chinese = "沙斯塔夏溶洞",
                    English = "Sastasha",
                    French = "Sastasha",
                    German = "Sastasha-Höhle",
                    Japanese = "サスタシャ浸食洞"
                },
                new MapInfo(true, 158)
                {
                    Chinese = "布雷福洛克斯野营地",
                    English = "Brayflox's Longstop",
                    French = "Bivouac de Brayflox",
                    German = "Brüllvolx' Langrast",
                    Japanese = "ブレイフロクスの野営地"
                },
                new MapInfo(true, 159)
                {
                    Chinese = "放浪神古神殿",
                    English = "The Wanderer's Palace",
                    French = "Palais du Vagabond",
                    German = "Palast des Wanderers",
                    Japanese = "ワンダラーパレス"
                },
                new MapInfo(true, 160)
                {
                    Chinese = "天狼星灯塔",
                    English = "Pharos Sirius",
                    French = "Phare de Sirius",
                    German = "Pharos Sirius",
                    Japanese = "シリウス大灯台"
                },
                new MapInfo(true, 161)
                {
                    Chinese = "铜铃铜山",
                    English = "Copperbell Mines",
                    French = "Mines de Clochecuivre",
                    German = "Kupferglocken-Mine",
                    Japanese = "カッパーベル銅山"
                },
                new MapInfo(true, 162)
                {
                    Chinese = "日影地修炼所",
                    English = "Halatali",
                    French = "Halatali",
                    German = "Halatali",
                    Japanese = "ハラタリ修練所"
                },
                new MapInfo(true, 163)
                {
                    Chinese = "喀恩淹没圣堂",
                    English = "The Sunken Temple of Qarn",
                    French = "Temple enseveli de Qarn",
                    German = "Versunkener Tempel von Qarn",
                    Japanese = "カルン埋没寺院 "
                },
                new MapInfo(true, 164)
                {
                    Chinese = "塔姆·塔拉墓园",
                    English = "The Tam-Tara Deepcroft",
                    French = "Hypogée de Tam-Tara",
                    German = "Totenacker Tam-Tara",
                    Japanese = "タムタラの墓所"
                },
                new MapInfo(true, 166)
                {
                    Chinese = "静语庄园",
                    English = "Haukke Manor",
                    French = "Manoir des Haukke",
                    German = "Haukke-Herrenhaus",
                    Japanese = "ハウケタ御用邸"
                },
                new MapInfo(true, 167)
                {
                    Chinese = "无限城古堡",
                    English = "Amdapor Keep",
                    French = "Château d'Amdapor",
                    German = "Ruinen von Amdapor",
                    Japanese = "古城アムダプール"
                },
                new MapInfo(true, 168)
                {
                    Chinese = "石卫塔",
                    English = "Stone Vigil",
                    French = "Vigile de pierre",
                    German = "Steinerne Wacht",
                    Japanese = "ストーンヴィジル"
                },
                new MapInfo(true, 169)
                {
                    Chinese = "托托·拉克千狱",
                    English = "The Thousand Maws of Toto-Rak",
                    French = "Mille Gueules de Toto-Rak",
                    German = "Tausend Löcher von Toto-Rak",
                    Japanese = "トトラクの千獄"
                },
                new MapInfo(true, 170)
                {
                    Chinese = "樵鸣洞",
                    English = "Cutter's Cry",
                    French = "Gouffre hurlant",
                    German = "Sägerschrei",
                    Japanese = "カッターズクライ"
                },
                new MapInfo(true, 171)
                {
                    Chinese = "泽梅尔要塞",
                    English = "Dzemael Darkhold",
                    French = "Forteresse de Dzemael",
                    German = "Feste Dzemael",
                    Japanese = "ゼーメル要塞"
                },
                new MapInfo(true, 172)
                {
                    Chinese = "黄金谷",
                    English = "Aurum Vale",
                    French = "Val d'aurum",
                    German = "Goldklamm",
                    Japanese = "オーラムヴェイル"
                },
                new MapInfo(true, 174)
                {
                    Chinese = "古代人迷宫",
                    English = "Labyrinth of the Ancients",
                    French = "Dédale antique",
                    German = "Labyrinth der Alten",
                    Japanese = "古代の民の迷宮"
                },
                new MapInfo(true, 175)
                {
                    Chinese = "狼狱水上竞技场",
                    English = "The Wolves' Den",
                    French = "L'Antre des loups",
                    German = "Die Wolfshöhle",
                    Japanese = "ウルヴズジェイル"
                },
                new MapInfo(false, 177)
                {
                    Chinese = "后桅旅店",
                    English = "Mizzenmast Inn",
                    French = "Auberge de l'Artimon",
                    German = "Gasthaus Gaffelschoner",
                    Japanese = "宿屋「ミズンマスト」"
                },
                new MapInfo(false, 178)
                {
                    Chinese = "沙钟旅亭",
                    English = "The Hourglass",
                    French = "Le Sablier",
                    German = "Die Sanduhr",
                    Japanese = "宿屋「砂時計亭」"
                },
                new MapInfo(false, 179)
                {
                    Chinese = "栖木旅馆",
                    English = "The Roost",
                    French = "Le Perchoir",
                    German = "Der Traumbaum",
                    Japanese = "旅館「とまり木」"
                },
                new MapInfo(false, 180)
                {
                    Chinese = "拉诺西亚外地",
                    English = "Outer La Noscea",
                    French = "Noscea extérieure",
                    German = "Äußeres La Noscea",
                    Japanese = "外地ラノシア"
                },
                new MapInfo(false, 198)
                {
                    Chinese = "提督室",
                    English = "Command Room",
                    French = "Salle de l'Amiral",
                    German = "Admiralsbrücke",
                    Japanese = "アドミラルブリッジ：提督室"
                },
                new MapInfo(false, 199)
                {
                    Chinese = "利姆萨·罗敏萨会议室",
                    English = "リムサ・ロミンサ会議部屋",
                    French = "リムサ・ロミンサ会議部屋",
                    German = "Besprechungszimmer",
                    Japanese = "リムサ・ロミンサ会議部屋"
                },
                new MapInfo(false, 200)
                {
                    Chinese = "リムサ・ロミンサ演説部屋",
                    English = "リムサ・ロミンサ演説部屋",
                    French = "リムサ・ロミンサ演説部屋",
                    German = "Verkündungszimmer",
                    Japanese = "リムサ・ロミンサ演説部屋"
                },
                new MapInfo(true, 202)
                {
                    Chinese = "炎帝陵",
                    English = "Bowl of Embers",
                    French = "Cratère des tisons",
                    German = "Das Grab der Lohe",
                    Japanese = "炎帝祭跡"
                },
                new MapInfo(false, 204)
                {
                    Chinese = "神勇队司令室",
                    English = "Seat of the First Bow",
                    French = "Salle de commandement du Carquois",
                    German = "Kommandozimmer von Nophicas Schar ",
                    Japanese = "神勇隊司令室"
                },
                new MapInfo(false, 205)
                {
                    Chinese = "Lotus Stand",
                    English = "Lotus Stand",
                    French = "Chaire du lotus",
                    German = "Wasserrosentisch",
                    Japanese = "不語仙の座卓"
                },
                new MapInfo(true, 206)
                {
                    Chinese = "奥・哥摩罗火口神殿",
                    English = "The Navel",
                    French = "Le Nombril",
                    German = "Der Nabel",
                    Japanese = "オ・ゴモロ火口神殿"
                },
                new MapInfo(true, 207)
                {
                    Chinese = "Thornmarch",
                    English = "Thornmarch",
                    French = "Lisière de ronces",
                    German = "Dornmarsch",
                    Japanese = "茨の園"
                },
                new MapInfo(true, 208)
                {
                    Chinese = "呼啸眼石塔群",
                    English = "The Howling Eye",
                    French = "Hurlœil",
                    German = "Das Tosende Auge",
                    Japanese = "ハウリングアイ石塔群"
                },
                new MapInfo(false, 210)
                {
                    Chinese = "Heart of the Sworn",
                    English = "Heart of the Sworn",
                    French = "Hall d'argent",
                    German = "Hauptquartier der Palastwache",
                    Japanese = "銀冑団総長室"
                },
                new MapInfo(false, 211)
                {
                    Chinese = "The Fragrant Chamber",
                    English = "The Fragrant Chamber",
                    French = "Chambre de l'encens",
                    German = "Die Weihrauchkammer",
                    Japanese = "香煙の間"
                },
                new MapInfo(false, 212)
                {
                    Chinese = "沙之家",
                    English = "The Waking Sands",
                    French = "Refuge des sables",
                    German = "Sonnenwind",
                    Japanese = "砂の家"
                },
                new MapInfo(true, 217)
                {
                    Chinese = "Castrum Meridianum",
                    English = "Castrum Meridianum",
                    French = "Castrum Meridianum",
                    German = "Castrum Meridianum",
                    Japanese = "カストルム・メリディアヌム"
                },
                new MapInfo(true, 224)
                {
                    Chinese = "Praetorium",
                    English = "Praetorium",
                    French = "Praetorium",
                    German = "Praetorium",
                    Japanese = "魔導城プラエトリウム"
                },
                new MapInfo(false, 241)
                {
                    Chinese = "Upper Aetheroacoustic Exploratory Site",
                    English = "Upper Aetheroacoustic Exploratory Site",
                    French = "Site impérial d'exploration supérieur",
                    German = "Obere ätheroakustische Grabung",
                    Japanese = "メテオ探査坑浅部"
                },
                new MapInfo(false, 242)
                {
                    Chinese = "Lower Aetheroacoustic Exploratory Site",
                    English = "Lower Aetheroacoustic Exploratory Site",
                    French = "Site impérial d'exploration inférieur",
                    German = "Untere ätheroakustische Grabung",
                    Japanese = "メテオ探査坑深部"
                },
                new MapInfo(false, 243)
                {
                    Chinese = "The Ragnarok",
                    English = "The Ragnarok",
                    French = "Le Ragnarok",
                    German = "Die Ragnarök",
                    Japanese = "ラグナロク級拘束艦"
                },
                new MapInfo(false, 244)
                {
                    Chinese = "Ragnarok Drive Cylinder",
                    English = "Ragnarok Drive Cylinder",
                    French = "Cylindre propulseur du Ragnarok",
                    German = "Antriebszylinder der Ragnarök",
                    Japanese = "稼働隔壁"
                },
                new MapInfo(false, 245)
                {
                    Chinese = "Ragnarok Central Core",
                    English = "Ragnarok Central Core",
                    French = "Noyau central du Ragnarok",
                    German = "Kernsektor der Ragnarök",
                    Japanese = "中枢区画"
                },
                new MapInfo(false, 250)
                {
                    Chinese = "狼狱停船场",
                    English = "Wolves' Den Pier",
                    French = "Jetée de l'Antre des loups",
                    German = "Wolfshöhlen-Pier",
                    Japanese = "ウルヴズジェイル係船場"
                },
                new MapInfo(false, 282)
                {
                    Chinese = "海雾村私人小屋",
                    English = "Private Cottage - Mist",
                    French = "Maisonnette - Brumée",
                    German = "Privathütte (Dorf des Nebels)",
                    Japanese = "ミスト・ヴィレッジ：コテージ"
                },
                new MapInfo(false, 283)
                {
                    Chinese = "海雾村私人公馆",
                    English = "Private House - Mist",
                    French = "Pavillon - Brumée",
                    German = "Privathaus (Dorf des Nebels)",
                    Japanese = "ミスト・ヴィレッジ：ハウス"
                },
                new MapInfo(false, 284)
                {
                    Chinese = "海雾村私人别墅",
                    English = "Private Mansion - Mist",
                    French = "Villa - Brumée",
                    German = "Privatresidenz (Dorf des Nebels)",
                    Japanese = "ミスト・ヴィレッジ：レジデンス"
                },
                new MapInfo(true, 331)
                {
                    Chinese = "呼啸眼石塔群",
                    English = "The Howling Eye",
                    French = "Hurlœil",
                    German = "Das Tosende Auge",
                    Japanese = "ハウリングアイ外縁"
                },
                new MapInfo(false, 339)
                {
                    Chinese = "海雾村",
                    English = "Mist",
                    French = "Brumée",
                    German = "Dorf des Nebels",
                    Japanese = "ミスト・ヴィレッジ"
                },
                new MapInfo(false, 340)
                {
                    Chinese = "薰衣草苗圃",
                    English = "Lavender Beds",
                    French = "Lavandière",
                    German = "Lavendelbeete",
                    Japanese = "ラベンダーベッド"
                },
                new MapInfo(false, 341)
                {
                    Chinese = "高脚孤丘",
                    English = "The Goblet",
                    French = "La Coupe",
                    German = "Kelchkuppe",
                    Japanese = "ゴブレットビュート"
                },
                new MapInfo(false, 342)
                {
                    Chinese = "薰衣草苗圃私人小屋",
                    English = "Private Cottage - Lavender Beds",
                    French = "Maisonnette - Lavandière",
                    German = "Privathütte (Lavendelbeete)",
                    Japanese = "ラベンダーベッド：コテージ"
                },
                new MapInfo(false, 343)
                {
                    Chinese = "薰衣草苗圃私人公馆",
                    English = "Private House - Lavender Beds",
                    French = "Pavillon - Lavandière",
                    German = "Privathaus (Lavendelbeete)",
                    Japanese = "ラベンダーベッド：ハウス"
                },
                new MapInfo(false, 344)
                {
                    Chinese = "薰衣草苗圃私人别墅",
                    English = "Private Mansion - Lavender Beds",
                    French = "Villa - Lavandière",
                    German = "Privatresidenz (Lavendelbeete)",
                    Japanese = "ラベンダーベッド：レジデンス"
                },
                new MapInfo(false, 345)
                {
                    Chinese = "高脚孤丘私人小屋",
                    English = "Private Cottage - The Goblet",
                    French = "Maisonnette - la Coupe",
                    German = "Privathütte (Kelchkuppe)",
                    Japanese = "ゴブレットビュート：コテージ"
                },
                new MapInfo(false, 346)
                {
                    Chinese = "高脚孤丘私人公馆",
                    English = "Private House - The Goblet",
                    French = "Pavillon - la Coupe",
                    German = "Privathaus (Kelchkuppe)",
                    Japanese = "ゴブレットビュート：ハウス"
                },
                new MapInfo(false, 347)
                {
                    Chinese = "高脚孤丘私人别墅",
                    English = "Private Mansion - The Goblet",
                    French = "Villa - la Coupe",
                    German = "Privatresidenz (Kelchkuppe)",
                    Japanese = "ゴブレットビュート：レジデンス"
                },
                new MapInfo(false, 348)
                {
                    Chinese = "究极神兵",
                    English = "Porta Decumana",
                    French = "Porta decumana",
                    German = "Porta Decumana",
                    Japanese = "ポルタ・デクマーナ"
                },
                new MapInfo(true, 349)
                {
                    Chinese = "铜铃铜山（骚乱坑道）",
                    English = "Copperbell Mines (Hard)",
                    French = "Mines de Clochecuivre (brutal)",
                    German = "Kupferglocken-Mine (schwer)",
                    Japanese = "カッパーベル銅山（騒乱坑道）"
                },
                new MapInfo(true, 350)
                {
                    Chinese = "静语庄园（恶灵府邸）",
                    English = "Haukke Manor (Hard)",
                    French = "Manoir des Haukke (brutal)",
                    German = "Haukke-Herrenhaus (schwer)",
                    Japanese = "ハウケタ御用邸（妖異屋敷）"
                },
                new MapInfo(false, 351)
                {
                    Chinese = "石之家",
                    English = "The Rising Stones",
                    French = "Refuge des roches",
                    German = "Sonnenstein",
                    Japanese = "石の家"
                }
            };

            return mapList;
        }

        public class MapInfo
        {
            private string _korean;
            private string _chinese;
            private string _english;
            private string _french;
            private string _german;
            private string _japanese;

            /// <summary>
            /// </summary>
            /// <param name="isDunegonInstance"></param>
            /// <param name="index"></param>
            /// <param name="english"></param>
            /// <param name="french"></param>
            /// <param name="japanese"></param>
            /// <param name="german"></param>
            /// <param name="chinese"></param>
            public MapInfo(bool isDunegonInstance, uint index = 0, string english = null, string french = null, string german = null, string japanese = null, string chinese = null, string korean = null)
            {
                Index = index;
                IsDungeonInstance = isDunegonInstance;
                English = english;
                French = french;
                Japanese = japanese;
                German = german;
                Chinese = chinese;
                Korean = korean;
            }

            public uint Index { get; set; }
            public bool IsDungeonInstance { get; set; }

            public string English
            {
                get { return _english ?? String.Format("Unknown_{0}", Index); }
                set { _english = value; }
            }

            public string French
            {
                get { return _french ?? String.Format("Unknown_{0}", Index); }
                set { _french = value; }
            }

            public string Japanese
            {
                get { return _japanese ?? String.Format("Unknown_{0}", Index); }
                set { _japanese = value; }
            }

            public string German
            {
                get { return _german ?? String.Format("Unknown_{0}", Index); }
                set { _german = value; }
            }

            public string Chinese
            {
                get { return _chinese ?? String.Format("Unknown_{0}", Index); }
                set { _chinese = value; }
            }

            public string Korean
            {
                get { return _korean ?? String.Format("Unknown_{0}", Index); }
                set { _korean = value; }
            }
        }
    }
}
