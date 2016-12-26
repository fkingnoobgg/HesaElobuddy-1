﻿using System.Collections.Generic;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace ARAMDetFull.Champions
{
    class Maokai : Champion
    {
        public Maokai()
        {
            //Interrupter.OnInterruptableSpell += OnPossibleToInterrupt;
            //Gapcloser.OnGapcloser += OnEnemyGapcloser;
            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Rod_of_Ages),
                    new ConditionalItem(ItemId.Mercurys_Treads, ItemId.Ninja_Tabi, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Spirit_Visage, ItemId.Frozen_Heart, ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Abyssal_Scepter),
                    new ConditionalItem(ItemId.Banner_of_Command,ItemId.Locket_of_the_Iron_Solari,ItemCondition.ENEMY_AP),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Catalyst_of_Aeons,ItemId.Boots_of_Speed
                }
            };
        }

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady())
                return;
            Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
            if (safeGap(target))
                W.CastOnUnit(target);
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady())
                return;
            E.Cast(target);
        }

        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady())
                return;
            if (R.IsKillable(target)/*(CanKill(target, SpellSlot.R, GetRDmg(target))*/ || Player.Instance.CountEnemiesInRange((int)R.Range - 100) > 2)
                R.Cast();
            if (Player.Instance.ManaPercent >= 30)
            {
                if (!player.HasBuff("MaokaiDrain")) R.Cast();
            }
            else if (player.HasBuff("MaokaiDrain")) R.Cast();


        }
        /*private double GetRDmg(Obj_AI_Base Target)
        {
            return player.CalculateDamageOnUnit(Target, DamageType.Magical, new double[] { 100, 150, 200 }[R.Level - 1] + 0.5 * player.FlatMagicDamageMod + R.Instance.Ammo+ R.Instance.Ammo);
        }*/

        public override void setUpSpells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 1000, int.MaxValue, 85, DamageType.Magical)
            { AllowedCollisionCount = -1 };
            W = new Spell.Skillshot(SpellSlot.W, 450, SkillShotType.Circular, 250, int.MaxValue, 250)
            { AllowedCollisionCount = -1 };
            E = new Spell.Targeted(SpellSlot.E, 650, DamageType.Magical);
            R = new Spell.Targeted(SpellSlot.R, 700, DamageType.Magical);
            /*
            Q = new Spell(SpellSlot.Q, 630);
            W = new Spell(SpellSlot.W, 525);
            E = new Spell(SpellSlot.E, 1115);
            R = new Spell(SpellSlot.R, 478);
            Q.SetSkillshot(0.3333f, 110, 1100, false, SkillshotType.SkillshotLine);
            W.SetTargetted(0.5f, 1000);
            E.SetSkillshot(0.25f, 225, 1750, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.25f, 478, float.MaxValue, false, SkillshotType.SkillshotCircle);*/
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range);
            if (tar != null) useQ(tar);
            tar = ARAMTargetSelector.getBestTarget(W.Range);
            if (tar != null) useW(tar);
            tar = ARAMTargetSelector.getBestTarget(E.Range);
            if (tar != null) useE(tar);
            tar = ARAMTargetSelector.getBestTarget(R.Range);
            if (tar != null) useR(tar);
        }
        //TODO disabled for now.
        /*private void OnEnemyGapcloser(Obj_AI_Base sender, Gapcloser.GapcloserEventArgs args)
        {
            if (player.IsDead || !Q.CanCast(gapcloser.Sender)) return;
            if (player.Distance3D(gapcloser.Sender) <= 100) Q.Cast(gapcloser.Sender.Position);
        }

        private void OnPossibleToInterrupt(Obj_AI_Base unit, Interrupter.InterruptableSpellEventArgs args)
        {
            if ( player.IsDead || !Q.IsReady()) return;
            if (player.Distance3D(unit) > 100 && W.CanCast(unit) && player.Mana >= Q.Instance.ManaCost + W.Instance.ManaCost)
            {
                W.CastOnUnit(unit);
                return;
            }
            if (Q.IsInRange(unit) && player.Distance3D(unit) <= 100) Q.Cast(unit.Position);
        }
        public static bool CanKill(Obj_AI_Base Target, Spell Skill, double Health, double SubDmg)
        {
            return Skill.GetHealthPrediction(Target) - Health + 5 <= SubDmg;
        }

        public static bool CanKill(Obj_AI_Base Target, Spell Skill, double SubDmg)
        {
            return CanKill(Target, Skill, 0, SubDmg);
        }

        public static bool CanKill(Obj_AI_Base Target, Spell Skill, int Stage = 0, double SubDmg = 0)
        {
            return Skill.GetHealthPrediction(Target) + 5 <= (SubDmg > 0 ? SubDmg : Skill.GetDamage(Target, Stage));
        }*/
    }
}