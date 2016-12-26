﻿using System;
using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace ARAMDetFull.Champions
{
    class MissFortune : Champion
    {
        private Spell.Skillshot Q1;
        private float RCastTime = 0;

        public MissFortune()
        {
            //DeathWalker.AfterAttack += afterAttack;
            Orbwalker.OnPostAttack += afterAttack;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Infinity_Edge),
                    new ConditionalItem(ItemId.Berserkers_Greaves),
                    new ConditionalItem(ItemId.Phantom_Dancer),
                    new ConditionalItem(ItemId.Youmuus_Ghostblade),
                    new ConditionalItem(ItemId.Last_Whisper),
                    new ConditionalItem(ItemId.The_Bloodthirster),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Pickaxe,ItemId.Boots_of_Speed
                }
            };
        }

        private void afterAttack(AttackableUnit unit, EventArgs args)
        {
            if (!unit.IsMe)
                return;

            if (ObjectManager.Player.Spellbook.IsChanneling || Game.Time - RCastTime < 0.2)
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                return;
            }

            if (!(unit is AIHeroClient))
                return;
            var t = unit as AIHeroClient;

            if (Q.IsReady() && t.IsValidTarget(Q.Range))
            {
                Q.Cast(t);
            }
            if (W.IsReady())
            {
                W.Cast();
            }
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady() || target == null)
                return;
            if (!Q.IsReady(4500) && player.Mana > 200)
                W.Cast();
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (target == null)
                return;
        }

        public override void useSpells()
        {
            if (ObjectManager.Player.Spellbook.IsChanneling)
                return;

            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            LogicR();
        }


        public override void setUpSpells()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 720);
            Q1 = new Spell.Skillshot(SpellSlot.Q, 1100, SkillShotType.Linear, 250, 2000, 50);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Skillshot(SpellSlot.E, 1000, SkillShotType.Circular)
            {
                Width = 350
            };
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Cone)
            {
                Width = (int)Math.PI / 180 * 35
            };
            /*Q = new Spell(SpellSlot.Q, 655f);
            Q1 = new Spell(SpellSlot.Q, 1100f);
            W = new Spell(SpellSlot.W,550);
            E = new Spell(SpellSlot.E, 800f);
            R = new Spell(SpellSlot.R, 1350f);

            Q1.SetSkillshot(0.25f, 50f, 2000f, true, SkillshotType.SkillshotLine);
            Q.SetTargetted(0.25f, 1400f);
            E.SetSkillshot(0.5f, 200f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 200f, 2000f, false, SkillshotType.SkillshotCircle);*/

        }

        private void LogicR()
        {
            var t = ARAMTargetSelector.getBestTarget(R.Range);

            if (t.IsValidTarget(R.Range))
            {
                var rDmg = R.GetDamage(t) + (W.GetDamage(t) * 10);

                if (player.CountEnemiesInRange(700) == 0 && t.CountAlliesInRange(400) == 0)
                {
                    var tDis = player.Distance(t.ServerPosition);
                    if (rDmg * 7 > t.Health && tDis < 800)
                    {
                        R.Cast(t);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 6 > t.Health && tDis < 900)
                    {
                        R.Cast(t);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 5 > t.Health && tDis < 1000)
                    {
                        R.Cast(t);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 4 > t.Health && tDis < 1100)
                    {
                        R.Cast(t);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg * 3 > t.Health && tDis < 1200)
                    {
                        R.Cast(t);
                        RCastTime = Game.Time;
                    }
                    else if (rDmg > t.Health && tDis < 1300)
                    {
                        R.Cast(t);
                        RCastTime = Game.Time;
                    }
                    return;
                }
                else if (rDmg * 8 > t.Health && t.CountEnemiesInRange(300) > 2 && player.CountEnemiesInRange(700) == 0)
                {
                    R.Cast(t);
                    RCastTime = Game.Time;
                    return;
                }
                else if (rDmg * 8 > t.Health && player.CountEnemiesInRange(600) == 0)
                {
                    R.Cast(t);
                    RCastTime = Game.Time;
                    return;
                }
            }

        }
    }
}