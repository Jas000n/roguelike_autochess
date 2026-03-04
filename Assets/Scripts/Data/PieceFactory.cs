using System.Collections.Generic;
using UnityEngine;
using DragonChessLegends.Core;

namespace DragonChessLegends.Data
{
    /// <summary>
    /// 棋子工厂 - 创建具有预设属性的棋子
    /// </summary>
    public static class PieceFactory
    {
        /// <summary>
        /// 创建棋子
        /// </summary>
        public static ChessPieceData CreatePiece(PieceType type, PieceSubType subType, int starLevel = 1)
        {
            ChessPieceData piece = new ChessPieceData
            {
                type = type,
                subType = subType,
                starLevel = starLevel
            };

            // 设置基础属性和名称
            switch (type)
            {
                case PieceType.General:
                    CreateGeneral(piece, subType);
                    break;
                case PieceType.Chariot:
                    CreateChariot(piece, subType);
                    break;
                case PieceType.Horse:
                    CreateHorse(piece, subType);
                    break;
                case PieceType.Cannon:
                    CreateCannon(piece, subType);
                    break;
                case PieceType.Elephant:
                    CreateElephant(piece, subType);
                    break;
                case PieceType.Guard:
                    CreateGuard(piece, subType);
                    break;
                case PieceType.Soldier:
                    CreateSoldier(piece, subType);
                    break;
            }

            // 应用星级加成
            ApplyStarBonus(piece);

            piece.currentHp = piece.maxHp;
            return piece;
        }

        private static void CreateGeneral(ChessPieceData piece, PieceSubType subType)
        {
            switch (subType)
            {
                case PieceSubType.Fire:
                    piece.pieceName = "火焰君主";
                    piece.description = "攻击附带灼烧";
                    piece.maxHp = 150;
                    piece.attack = 25;
                    piece.defense = 10;
                    piece.magicResist = 10;
                    piece.speed = 8;
                    piece.critRate = 0.1f;
                    break;
                case PieceSubType.Ice:
                    piece.pieceName = "冰霜王者";
                    piece.description = "普攻减速敌人";
                    piece.maxHp = 140;
                    piece.attack = 20;
                    piece.defense = 12;
                    piece.magicResist = 15;
                    piece.speed = 9;
                    piece.critRate = 0.08f;
                    break;
                case PieceSubType.Holy:
                    piece.pieceName = "圣殿骑士";
                    piece.description = "周围友军减伤";
                    piece.maxHp = 180;
                    piece.attack = 18;
                    piece.defense = 20;
                    piece.magicResist = 15;
                    piece.speed = 7;
                    piece.critRate = 0.05f;
                    piece.damageReduce = 0.1f;
                    break;
            }
        }

        private static void CreateChariot(ChessPieceData piece, PieceSubType subType)
        {
            switch (subType)
            {
                case PieceSubType.Iron:
                    piece.pieceName = "泰坦战车";
                    piece.description = "高护甲，冲锋后获得护盾";
                    piece.maxHp = 200;
                    piece.attack = 30;
                    piece.defense = 25;
                    piece.magicResist = 10;
                    piece.speed = 6;
                    piece.critRate = 0.1f;
                    break;
                case PieceSubType.Shadow:
                    piece.pieceName = "暗影战车";
                    piece.description = "隐身入场，优先攻击";
                    piece.maxHp = 120;
                    piece.attack = 35;
                    piece.defense = 8;
                    piece.magicResist = 8;
                    piece.speed = 12;
                    piece.critRate = 0.2f;
                    break;
                case PieceSubType.Thunder:
                    piece.pieceName = "雷鸣战车";
                    piece.description = "攻击附带闪电链";
                    piece.maxHp = 130;
                    piece.attack = 28;
                    piece.defense = 10;
                    piece.magicResist = 20;
                    piece.speed = 10;
                    piece.critRate = 0.15f;
                    break;
            }
        }

        private static void CreateHorse(ChessPieceData piece, PieceSubType subType)
        {
            switch (subType)
            {
                case PieceSubType.IronHorse:
                    piece.pieceName = "铁甲战马";
                    piece.description = "高生命，侧翼攻击+伤害";
                    piece.maxHp = 140;
                    piece.attack = 22;
                    piece.defense = 15;
                    piece.magicResist = 10;
                    piece.speed = 15;
                    piece.critRate = 0.1f;
                    piece.damageBonus = 0.15f;
                    break;
                case PieceSubType.Nightmare:
                    piece.pieceName = "梦魇骑士";
                    piece.description = "恐惧敌人，击杀回血";
                    piece.maxHp = 110;
                    piece.attack = 28;
                    piece.defense = 8;
                    piece.magicResist = 8;
                    piece.speed = 18;
                    piece.critRate = 0.2f;
                    piece.lifesteal = 0.15f;
                    break;
                case PieceSubType.Unicorn:
                    piece.pieceName = "独角兽";
                    piece.description = "周围友军+闪避";
                    piece.maxHp = 100;
                    piece.attack = 18;
                    piece.defense = 12;
                    piece.magicResist = 15;
                    piece.speed = 16;
                    piece.critRate = 0.1f;
                    piece.dodge = 0.2f;
                    break;
            }
        }

        private static void CreateCannon(ChessPieceData piece, PieceSubType subType)
        {
            switch (subType)
            {
                case PieceSubType.FireCannon:
                    piece.pieceName = "火炮手";
                    piece.description = "高伤害，穿透攻击";
                    piece.maxHp = 90;
                    piece.attack = 35;
                    piece.defense = 5;
                    piece.magicResist = 10;
                    piece.speed = 8;
                    piece.critRate = 0.15f;
                    break;
                case PieceSubType.Stone:
                    piece.pieceName = "投石车";
                    piece.description = "范围伤害，弹射";
                    piece.maxHp = 100;
                    piece.attack = 25;
                    piece.defense = 8;
                    piece.magicResist = 8;
                    piece.speed = 6;
                    piece.critRate = 0.1f;
                    break;
                case PieceSubType.Magic:
                    piece.pieceName = "魔导炮";
                    piece.description = "魔法伤害，削弱抗性";
                    piece.maxHp = 80;
                    piece.attack = 30;
                    piece.defense = 5;
                    piece.magicResist = 25;
                    piece.speed = 10;
                    piece.critRate = 0.12f;
                    break;
            }
        }

        private static void CreateElephant(ChessPieceData piece, PieceSubType subType)
        {
            switch (subType)
            {
                case PieceSubType.Forest:
                    piece.pieceName = "森林守护者";
                    piece.description = "受击反伤，召唤小怪";
                    piece.maxHp = 180;
                    piece.attack = 15;
                    piece.defense = 20;
                    piece.magicResist = 15;
                    piece.speed = 5;
                    piece.critRate = 0.05f;
                    break;
                case PieceSubType.Rock:
                    piece.pieceName = "岩石巨像";
                    piece.description = "高护甲，免疫暴击";
                    piece.maxHp = 220;
                    piece.attack = 12;
                    piece.defense = 30;
                    piece.magicResist = 20;
                    piece.speed = 4;
                    piece.critRate = 0f;
                    break;
                case PieceSubType.Cloud:
                    piece.pieceName = "云翼天使";
                    piece.description = "友军+魔抗，治疗";
                    piece.maxHp = 120;
                    piece.attack = 15;
                    piece.defense = 10;
                    piece.magicResist = 25;
                    piece.speed = 12;
                    piece.critRate = 0.08f;
                    break;
            }
        }

        private static void CreateGuard(ChessPieceData piece, PieceSubType subType)
        {
            switch (subType)
            {
                case PieceSubType.Dark:
                    piece.pieceName = "暗影刺客";
                    piece.description = "背后攻击必暴击";
                    piece.maxHp = 80;
                    piece.attack = 30;
                    piece.defense = 5;
                    piece.magicResist = 5;
                    piece.speed = 20;
                    piece.critRate = 0.3f;
                    piece.critDamage = 2f;
                    break;
                case PieceSubType.HolyGuard:
                    piece.pieceName = "圣光守卫";
                    piece.description = "保护队友，嘲讽";
                    piece.maxHp = 150;
                    piece.attack = 15;
                    piece.defense = 25;
                    piece.magicResist = 15;
                    piece.speed = 8;
                    piece.critRate = 0.05f;
                    break;
                case PieceSubType.Blood:
                    piece.pieceName = "血纹狂战士";
                    piece.description = "血量越低攻击越高";
                    piece.maxHp = 130;
                    piece.attack = 25;
                    piece.defense = 10;
                    piece.magicResist = 8;
                    piece.speed = 14;
                    piece.critRate = 0.15f;
                    piece.damageBonus = 0.5f;
                    break;
            }
        }

        private static void CreateSoldier(ChessPieceData piece, PieceSubType subType)
        {
            switch (subType)
            {
                case PieceSubType.Sword:
                    piece.pieceName = "剑士";
                    piece.description = "廉价劳动力，连击";
                    piece.maxHp = 80;
                    piece.attack = 15;
                    piece.defense = 5;
                    piece.magicResist = 5;
                    piece.speed = 12;
                    piece.critRate = 0.1f;
                    break;
                case PieceSubType.Bow:
                    piece.pieceName = "弓手";
                    piece.description = "远程攻击，射程+1";
                    piece.maxHp = 60;
                    piece.attack = 18;
                    piece.defense = 3;
                    piece.magicResist = 5;
                    piece.speed = 10;
                    piece.critRate = 0.12f;
                    break;
                case PieceSubType.Scout:
                    piece.pieceName = "斥候";
                    piece.description = "侦查敌人，+全队移速";
                    piece.maxHp = 70;
                    piece.attack = 12;
                    piece.defense = 4;
                    piece.magicResist = 8;
                    piece.speed = 18;
                    piece.critRate = 0.15f;
                    piece.dodge = 0.1f;
                    break;
            }
        }

        private static void ApplyStarBonus(ChessPieceData piece)
        {
            // 星级加成
            float hpBonus = 1f + (piece.starLevel - 1) * 0.5f;
            float atkBonus = 1f + (piece.starLevel - 1) * 0.3f;
            float defBonus = 1f + (piece.starLevel - 1) * 0.3f;

            piece.maxHp = Mathf.RoundToInt(piece.maxHp * hpBonus);
            piece.attack = Mathf.RoundToInt(piece.attack * atkBonus);
            piece.defense = Mathf.RoundToInt(piece.defense * defBonus);
        }

        /// <summary>
        /// 获取棋子显示名称
        /// </summary>
        public static string GetPieceDisplayName(PieceType type, PieceSubType subType)
        {
            var piece = CreatePiece(type, subType);
            return piece.pieceName;
        }
    }
}
