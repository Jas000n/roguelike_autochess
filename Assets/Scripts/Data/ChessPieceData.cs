using System.Collections.Generic;
using UnityEngine;

namespace DragonChessLegends.Core
{
    /// <summary>
    /// 棋子类型（大类）
    /// </summary>
    public enum PieceType
    {
        General,   // 帅
        Chariot,   // 车
        Horse,     // 马
        Cannon,    // 炮
        Elephant,  // 象
        Guard,     // 士
        Soldier    // 兵
    }

    /// <summary>
    /// 子类型（元素/风格）
    /// </summary>
    public enum PieceSubType
    {
        // 帅
        Fire,      // 火焰君主
        Ice,       // 冰霜王者  
        Holy,      // 圣殿骑士
        
        // 车
        Iron,      // 泰坦战车
        Shadow,    // 暗影战车
        Thunder,   // 雷鸣战车
        
        // 马
        IronHorse, // 铁甲战马
        Nightmare, // 梦魇骑士
        Unicorn,   // 独角兽
        
        // 炮
        FireCannon,    // 火炮手
        Stone,         // 投石车
        Magic,         // 魔导炮
        
        // 象
        Forest,    // 森林守护者
        Rock,      // 岩石巨像
        Cloud,     // 云翼天使
        
        // 士
        Dark,      // 暗影刺客
        HolyGuard, // 圣光守卫
        Blood,     // 血纹狂战士
        
        // 兵
        Sword,     // 剑士
        Bow,       // 弓手
        Scout      // 斥候
    }

    /// <summary>
    /// 棋子数据
    /// </summary>
    [System.Serializable]
    public class ChessPieceData
    {
        public string id;
        public PieceType type;
        public PieceSubType subType;
        public int starLevel;     // 1-3星
        public int level;         // 等级
        
        // 基础属性
        public float maxHp;
        public float currentHp;
        public float attack;
        public float defense;
        public float magicResist;
        public float speed;
        public float critRate;
        public float critDamage;
        
        // 特殊属性
        public float lifesteal;      // 吸血
        public float dodge;          // 闪避
        public float damageBonus;    // 增伤
        public float damageReduce;   // 减伤
        
        // 位置
        public int x;
        public int y;
        
        public string pieceName;
        public string description;

        public ChessPieceData()
        {
            id = System.Guid.NewGuid().ToString();
            starLevel = 1;
            level = 1;
            maxHp = 100;
            currentHp = 100;
            attack = 10;
            defense = 5;
            magicResist = 5;
            speed = 10;
            critRate = 0.05f;
            critDamage = 1.5f;
        }

        public ChessPieceData Clone()
        {
            return new ChessPieceData
            {
                id = System.Guid.NewGuid().ToString(),
                type = this.type,
                subType = this.subType,
                starLevel = this.starLevel,
                level = this.level,
                maxHp = this.maxHp,
                currentHp = this.maxHp,
                attack = this.attack,
                defense = this.defense,
                magicResist = this.magicResist,
                speed = this.speed,
                critRate = this.critRate,
                critDamage = this.critDamage,
                lifesteal = this.lifesteal,
                dodge = this.dodge,
                damageBonus = this.damageBonus,
                damageReduce = this.damageReduce,
                x = -1,
                y = -1,
                pieceName = this.pieceName,
                description = this.description
            };
        }

        public void Upgrade()
        {
            if (starLevel >= 3) return;
            starLevel++;
            maxHp *= 1.5f;
            currentHp = maxHp;
            attack *= 1.3f;
            defense *= 1.3f;
            magicResist *= 1.3f;
            speed *= 1.1f;
        }
    }
}
