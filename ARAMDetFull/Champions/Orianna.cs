﻿using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SharpDX;
using static EloBuddy.SDK.Spell;

namespace ARAMDetFull.Champions
{
    class Orianna : Champion
    {
        public Orianna()
        {
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Interrupter.OnInterruptableSpell += Interrupter_OnPossibleToInterrupt;

            ARAMSimulator.champBuild = new Build
            {
                coreItems = new List<ConditionalItem>
                {
                    new ConditionalItem(ItemId.Athenes_Unholy_Grail),
                    new ConditionalItem(ItemId.Sorcerers_Shoes),
                    new ConditionalItem(ItemId.Rabadons_Deathcap),
                    new ConditionalItem(ItemId.Zhonyas_Hourglass),
                    new ConditionalItem(ItemId.Void_Staff),
                    new ConditionalItem(ItemId.Banshees_Veil),
                },
                startingItems = new List<ItemId>
                {
                    ItemId.Chalice_of_Harmony,ItemId.Boots_of_Speed
                }
            };
        }

        private void Interrupter_OnPossibleToInterrupt(Obj_AI_Base unit, Interrupter.InterruptableSpellEventArgs args)
        {
            if (args.DangerLevel <= DangerLevel.Medium)
            {
                return;
            }

            if (E.IsReady())
            {
                Q.Cast(unit);
                if (BallManager.BallPosition.Distance(unit.ServerPosition, true) < R.Range * R.Range)
                {
                    R.Cast(player.ServerPosition);
                }
            }
        }


        private Dictionary<string, string> InitiatorsList = new Dictionary<string, string>
        {
            {"aatroxq", "Aatrox"},
            {"akalishadowdance", "Akali"},
            {"headbutt", "Alistar"},
            {"bandagetoss", "Amumu"},
            {"dianateleport", "Diana"},
            {"elisespidereinitial", "Elise"},
            {"crowstorm", "FiddleSticks"},
            {"fioraq", "Fiora"},
            {"gragase", "Gragas"},
            {"hecarimult", "Hecarim"},
            {"ireliagatotsu", "Irelia"},
            {"jarvanivdragonstrike", "JarvanIV"},
            {"jaxleapstrike", "Jax"},
            {"riftwalk", "Kassadin"}, // prob outdated
            {"katarinae", "Katarina"},
            {"kennenlightningrush", "Kennen"},
            {"khazixe", "KhaZix"},
            {"khazixelong", "KhaZix"},
            {"blindmonkqtwo", "LeeSin"},
            {"leonazenithblademissle", "Leona"},
            {"lissandrae", "Lissandra"},
            {"ufslash", "Malphite"},
            {"maokaiunstablegrowth", "Maokai"},
            {"monkeykingnimbus", "MonkeyKing"},
            {"monkeykingspintowin", "MonkeyKing"},
            {"summonerflash", "MonkeyKing"},
            {"nocturneparanoia", "Nocturne"},
            {"olafragnarok", "Olaf"},
            {"poppyheroiccharge", "Poppy"},
            {"renektonsliceanddice", "Renekton"},
            {"rengarr", "Rengar"},
            {"reksaieburrowed", "RekSai"},
            {"sejuaniarcticassault", "Sejuani"},
            {"shenshadowdash", "Shen"},
            {"shyvanatransformcast", "Shyvana"},
            {"shyvanatransformleap", "Shyvana"},
            {"sionr", "Sion"},
            {"taloncutthroat", "Talon"},
            {"threshqleap", "Thresh"},
            {"slashcast", "Tryndamere"},
            {"udyrbearstance", "Udyr"},
            {"urgotswap2", "Urgot"},
            {"viq", "Vi"},
            {"vir", "Vi"},
            {"volibearq", "Volibear"},
            {"infiniteduress", "Warwick"},
            {"yasuorknockupcombow", "Yasuo"},
            {"zace", "Zac"}
        };

        public override void useQ(Obj_AI_Base target)
        {
            if (!Q.IsReady() || target == null)
                return;
            if (safeGap(target))
                Q.Cast(target);
        }

        public override void useW(Obj_AI_Base target)
        {
            if (!W.IsReady())
                return;
        }

        public override void useE(Obj_AI_Base target)
        {
            if (!E.IsReady() || target == null)
                return;
            E.Cast(target);
        }


        public override void useR(Obj_AI_Base target)
        {
            if (!R.IsReady() || target == null)
                return;
            if (target.IsValidTarget(R.Range))
            {
                R.CastIfWillHit(target, 2);
            }
        }

        public override void useSpells()
        {
            var tar = ARAMTargetSelector.getBestTarget(Q.Range + 350);
            if (tar == null)
            {
                Farm(false);
            }
            else
            {
                Combo();
            }
        }

        public override void setUpSpells()
        {
            //Create the spells
            Q = new Spell.Skillshot(SpellSlot.Q, 825, SkillShotType.Circular, 0, 1400, 130);
            W = new Spell.Skillshot(SpellSlot.W, 245, SkillShotType.Circular, 250, int.MaxValue, 240);
            E = new Spell.Skillshot(SpellSlot.E, 1095, SkillShotType.Linear, 250, 1700, 80);
            R = new Spell.Skillshot(SpellSlot.R, 380, SkillShotType.Circular, 600, int.MaxValue, 375);

            //Q.SetSkillshot(0f, 130f, 1400f, false, SkillshotType.SkillshotCircle);
            //W.SetSkillshot(0.25f, 240f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            //E.SetSkillshot(0.25f, 80f, 1700f, true, SkillshotType.SkillshotLine);
            //R.SetSkillshot(0.6f, 375f, float.MaxValue, false, SkillshotType.SkillshotCircle);
        }


        void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!(sender is AIHeroClient))
            {
                return;
            }

            var spellName = args.SData.Name.ToLower();
            if (!InitiatorsList.ContainsKey(spellName))
            {
                return;
            }


            if (!E.IsReady())
            {
                return;
            }

            if (sender.IsAlly && player.Distance(sender, true) < E.Range * E.Range)
            {
                E.CastOnUnit(sender);
            }
        }

        void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            //TODO fix GetHits
            /*
            if (args.Slot == SpellSlot.R && GetHits().Item1 == 0)
            {
                args.Process = false;
            }*/
        }


        private void Farm(bool laneClear)
        {
            //TODO fix ranged minions
            var qw = Q.Range + W.Width();
            var allMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, ObjectManager.Player.ServerPosition, qw);
            var rangedMinions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, ObjectManager.Player.ServerPosition, qw);

            var useQi = 2;
            var useWi = 2;
            var useEi = 2;

            var useQ = (laneClear && (useQi == 1 || useQi == 2)) || (!laneClear && (useQi == 0 || useQi == 2));
            var useW = (laneClear && (useWi == 1 || useWi == 2)) || (!laneClear && (useWi == 0 || useWi == 2));
            var useE = (laneClear && (useEi == 1 || useEi == 2)) || (!laneClear && (useEi == 0 || useEi == 2));

            if (useQ && Q.IsReady())
            {
                if (useW)
                {
                    var qLocation = Q.GetCircularFarmLocation(allMinions, W.Width());
                    //var qLocation = Q.GetCircularFarmLocation(allMinions, W.Range);
                    var q2Location = Q.GetCircularFarmLocation(rangedMinions, W.Width());
                    var bestLocation = (qLocation.HitNumber > q2Location.HitNumber + 1) ? qLocation : q2Location;

                    if (bestLocation.HitNumber > 0)
                    {
                        Q.Cast(bestLocation.CastPosition);
                        return;
                    }
                }
                else
                {
                    var qspeed = 1400;
                    foreach (var minion in allMinions.Where(m => !Player.Instance.IsInAutoAttackRange(m)))
                    {
                        if (Prediction.Health.GetPrediction(minion, Math.Max((int)(minion.ServerPosition.Distance(BallManager.BallPosition) / qspeed * 1000) - 100, 0)) < 50)
                        {
                            Q.Cast(minion.ServerPosition);
                            return;
                        }
                    }
                }
            }

            if (useW && W.IsReady())
            {
                var n = 0;
                var d = 0;
                foreach (var m in allMinions)
                {
                    if (m.Distance(BallManager.BallPosition) <= W.Range)
                    {
                        n++;
                        if (W.GetDamage(m) > m.Health)
                        {
                            d++;
                        }
                    }
                }
                if (n >= 3 || d >= 2)
                {
                    W.Cast(player.ServerPosition);
                    return;
                }
            }

            if (useE && E.IsReady())
            {/*
                if (W.AmmoQuantity(allMinions, player.ServerPosition) >= 3)
                {
                    E.CastOnUnit(player);
                    return;
                }*/
            }
        }

        public Tuple<int, Vector3> GetBestQLocation(AIHeroClient mainTarget)
        {
            var points = new List<Vector2>();
            var qPrediction = Q.GetPrediction(mainTarget);
            if (qPrediction.HitChance < HitChance.High)
            {
                return new Tuple<int, Vector3>(1, Vector3.Zero);
            }
            points.Add(qPrediction.UnitPosition.To2D());

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(Q.Range + R.Range)))
            {
                points.Add(Q.GetPrediction(enemy).UnitPosition.To2D());
            }

            for (int j = 0; j < 5; j++)
            {
                var mecResult = MEC.GetMec(points);

                if (mecResult.Radius < R.Range && points.Count >= 3 && R.IsReady())
                {
                    return new Tuple<int, Vector3>(3, mecResult.Center.To3D());
                }

                if (mecResult.Radius < W.Range && points.Count >= 2 && W.IsReady())
                {
                    return new Tuple<int, Vector3>(2, mecResult.Center.To3D());
                }

                if (points.Count == 1)
                {
                    return new Tuple<int, Vector3>(1, mecResult.Center.To3D());
                }

                if (mecResult.Radius < (Q.Width + 50) && points.Count == 2)
                {
                    return new Tuple<int, Vector3>(2, mecResult.Center.To3D());
                }

                float maxdist = -1;
                var maxdistindex = 1;
                for (var i = 1; i < points.Count; i++)
                {
                    var distance = Vector2.DistanceSquared(points[i], points[0]);
                    if (distance > maxdist || maxdist.CompareTo(-1) == 0)
                    {
                        maxdistindex = i;
                        maxdist = distance;
                    }
                }
                points.RemoveAt(maxdistindex);
            }

            return new Tuple<int, Vector3>(1, points[0].To3D());
        }

        void Combo()
        {
            var target = ARAMTargetSelector.getBestTarget(Q.Range + Q.Width);

            if (target == null)
            {
                return;
            }

            var minRTargets = 2 + 1;

            if (W.IsReady())
            {
                CastW(1);
            }

            if (Utility.CountEnemiesInRange((int)(Q.Range + R.Width)) <= 1)
            {
                if (GetComboDamage(target) > target.Health && R.IsReady())
                {
                    CastR(minRTargets);
                }

                if (Q.IsReady())
                {
                    CastQ(target);
                }

                if (true)
                {
                    foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(E.Range, false) && h.IsAlly && !h.IsMe))
                    {
                        if (ally.Position.CountEnemiesInRange(300) >= 1)
                        {
                            E.CastOnUnit(ally, true);
                        }

                        CastE(ally, 1);
                    }

                    CastE(player, 1);
                }
            }
            else
            {
                if (R.IsReady())
                {
                    if (BallManager.BallPosition.CountEnemiesInRange(800) > 1)
                    {
                        var rCheck = GetHits(R);
                        var pk = 0;
                        var k = 0;
                        if (rCheck.Item1 >= 2)
                        {
                            foreach (var hero in rCheck.Item2)
                            {
                                if ((hero.Health - GetComboDamage(hero)) < 0.4 * hero.MaxHealth || GetComboDamage(hero) >= 0.4 * hero.MaxHealth)
                                {
                                    pk++;
                                }

                                if ((hero.Health - GetComboDamage(hero)) < 0)
                                {
                                    k++;
                                }
                            }

                            if (rCheck.Item1 >= BallManager.BallPosition.CountEnemiesInRange(800) || pk >= 2 ||
                                k >= 1)
                            {
                                if (rCheck.Item1 >= minRTargets)
                                {
                                    R.Cast(player.ServerPosition, true);
                                }
                            }
                        }
                    }
                    else if (GetComboDamage(target) > target.Health)
                    {
                        CastR(minRTargets);
                    }
                }

                if (Q.IsReady())
                {
                    var qLoc = GetBestQLocation(target);
                    if (qLoc.Item1 > 1)
                    {
                        Q.Cast(qLoc.Item2, true);
                    }
                    else
                    {
                        CastQ(target);
                    }
                }

                if (E.IsReady())
                {
                    if (BallManager.BallPosition.CountEnemiesInRange(800) <= 2)
                    {
                        CastE(player, 1);
                    }
                    else
                    {
                        CastE(player, 2);
                    }

                    foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(E.Range, false) && h.IsAlly))
                    {
                        if (ally.Position.CountEnemiesInRange(300) >= 2)
                        {
                            E.CastOnUnit(ally, true);
                        }
                    }
                }
            }
        }

        public bool CastQ(AIHeroClient target)
        {
            var qPrediction = Q.GetPrediction(target);

            if (qPrediction.Hitchance < HitChance.High)
            {
                return false;
            }

            if (E.IsReady())
            {
                var directTravelTime = BallManager.BallPosition.Distance(qPrediction.CastPosition) / Q.Speed;
                var bestEQTravelTime = float.MaxValue;

                AIHeroClient eqTarget = null;

                foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.Team == player.Team && h.IsValidTarget(E.Range, false)))
                {
                    var t = BallManager.BallPosition.Distance(ally.ServerPosition) / E.Speed + ally.Distance(qPrediction.CastPosition) / Q.Speed;
                    if (t < bestEQTravelTime)
                    {
                        eqTarget = ally;
                        bestEQTravelTime = t;
                    }
                }

                if (eqTarget != null && bestEQTravelTime < directTravelTime * 1.3f && (BallManager.BallPosition.Distance(eqTarget.ServerPosition, true) > 10000))
                {
                    E.CastOnUnit(eqTarget, true);
                    return true;
                }
            }
            Q.Cast(qPrediction.CastPosition, true);
            return true;
        }

        public bool CastW(int minTargets)
        {
            var hits = GetHits(W);
            if (hits.Item1 >= minTargets)
            {
                W.Cast(player.ServerPosition, true);
                return true;
            }
            return false;
        }

        public bool CastE(AIHeroClient target, int minTargets)
        {
            if (GetEHits(target.ServerPosition).Item1 >= minTargets)
            {
                E.CastOnUnit(target, true);
                return true;
            }
            return false;
        }

        public bool CastR(int minTargets)
        {
            if (GetHits(R).Item1 >= minTargets)
            {
                R.Cast(player.ServerPosition, true);
                return true;
            }
            return false;
        }

        public Tuple<int, List<AIHeroClient>> GetEHits(Vector3 to)
        {
            var hits = new List<AIHeroClient>();
            var oldERange = E.Range;
            E.Range = 10000; //avoid the range check
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(2000)))
            {
                if (E.WillHit(enemy, to))
                {
                    hits.Add(enemy);
                }
            }
            E.Range = oldERange;
            return new Tuple<int, List<AIHeroClient>>(hits.Count, hits);
        }

        public int GetNumberOfMinionsHitByE(AIHeroClient target)
        {
            var minions = MinionManager.GetMinions(BallManager.BallPosition, 2000);
            return E.CountHits(minions, target.ServerPosition);
        }

        public float GetComboDamage(AIHeroClient target)
        {
            var result = 0f;
            if (Q.IsReady())
            {
                result += 2 * Q.GetDamage(target);
            }

            if (W.IsReady())
            {
                result += W.GetDamage(target);
            }

            if (R.IsReady())
            {
                result += R.GetDamage(target);
            }

            result += 2 * (float)player.GetAutoAttackDamage(target);

            return result;
        }

        public Tuple<int, List<AIHeroClient>> GetHits(SpellBase spell)
        {
            var hits = new List<AIHeroClient>();
            var range = spell.Range * spell.Range;
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget() && BallManager.BallPosition.Distance(h.ServerPosition, true) < range))
            {
                if (spell.WillHit(enemy, BallManager.BallPosition) && BallManager.BallPosition.Distance(enemy.ServerPosition, true) < spell.Width() * spell.Width())
                {
                    hits.Add(enemy);
                }
            }
            return new Tuple<int, List<AIHeroClient>>(hits.Count, hits);
        }

    }

    public static class BallManager
    {
        public static Vector3 BallPosition { get; private set; }
        private static int _sTick = 0;

        static BallManager()
        {
            Game.OnUpdate += Game_OnGameUpdate;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            BallPosition = ObjectManager.Player.Position;
        }

        static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.SData.Name)
                {
                    case "OrianaIzunaCommand":
                    Utility.DelayAction.Add((int)(BallPosition.Distance(args.End) / 1.2 - 70 - Game.Ping), () => BallPosition = args.End);
                    BallPosition = Vector3.Zero;
                    _sTick = Environment.TickCount;
                    break;

                    case "OrianaRedactCommand":
                    BallPosition = Vector3.Zero;
                    _sTick = Environment.TickCount;
                    break;
                }
            }
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Environment.TickCount - _sTick > 300 && ObjectManager.Player.HasBuff("OrianaGhostSelf"))
            {
                BallPosition = ObjectManager.Player.Position;
            }

            foreach (var ally in ObjectManager.Get<AIHeroClient>().Where(h => h.IsAlly && !h.IsMe))
            {
                if (ally.HasBuff("OrianaGhost"))
                {
                    BallPosition = ally.Position;
                }
            }
        }
    }

}