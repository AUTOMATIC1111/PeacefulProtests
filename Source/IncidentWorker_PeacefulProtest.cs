using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PeacefulProtests
{
    public class IncidentWorker_PeacefulProtest : IncidentWorker_Raid
    {
        protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
        {
            return base.FactionCanBeGroupSource(f, map, desperate) && f.HostileTo(Faction.OfPlayer) && (desperate || GenDate.DaysPassed >= f.def.earliestRaidDays);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!base.TryExecuteWorker(parms))
            {
                return false;
            }

            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaidsEnemy++;
            return true;
        }

        protected override bool TryResolveRaidFaction(IncidentParms parms)
        {
            FactionDef def = DefDatabase<FactionDef>.GetNamed("PeacefulProtesters");
            parms.faction =  Find.FactionManager.FirstFactionOfDef(def);
            if (parms.faction == null)
            {
                parms.faction = FactionGenerator.NewGeneratedFaction(def);
                parms.faction.SetRelationDirect(Faction.OfPlayer, FactionRelationKind.Hostile, false);
                parms.faction.TryGenerateNewLeader();
                if (parms.faction.leader != null)
                {
                    parms.faction.leader.Name = PeacefulNames.Generate(parms.faction.leader);
                }
                Find.FactionManager.Add(parms.faction);
            }

            return true;
        }

        protected override void ResolveRaidPoints(IncidentParms parms)
        {
            if (parms.points <= 0f)
            {
                parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);
            }
        }

        public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed("PeacefulProtest");
        }

		protected override string GetLetterLabel(IncidentParms parms)
		{
			return parms.raidStrategy.letterLabelEnemy + ": " + parms.faction.Name;
		}

        protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            string text = string.Format(parms.raidArrivalMode.textFriendly, parms.faction.def.pawnsPlural, parms.faction.Name.ApplyTag(parms.faction)).CapitalizeFirst();
            text += "\n\n";
            text += parms.raidStrategy.arrivalTextFriendly;
            return text;
        }

        protected override LetterDef GetLetterDef()
        {
            return LetterDefOf.PositiveEvent;
        }

        protected override string GetRelatedPawnsInfoLetterText(IncidentParms parms)
        {
            return "LetterRelatedPawnsRaidFriendly".Translate(Faction.OfPlayer.def.pawnsPlural, parms.faction.def.pawnsPlural);
        }

        protected override void GenerateRaidLoot(IncidentParms parms, float raidLootPoints, List<Pawn> pawns)
        {
            if (parms.faction.def.raidLootMaker == null || !pawns.Any())
            {
                return;
            }
            raidLootPoints *= Find.Storyteller.difficultyValues.EffectiveRaidLootPointsFactor;
            float num = parms.faction.def.raidLootValueFromPointsCurve.Evaluate(raidLootPoints);
            if (parms.raidStrategy != null)
            {
                num *= parms.raidStrategy.raidLootValueFactor;
            }
            ThingSetMakerParams parms2 = default;
            parms2.totalMarketValueRange = new FloatRange?(new FloatRange(num, num));
            parms2.makingFaction = parms.faction;
            List<Thing> loot = parms.faction.def.raidLootMaker.root.Generate(parms2);
            new ReparationsDistributor(parms, pawns, loot).DistributeLoot();
        }
    }
}
