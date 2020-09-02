using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PeacefulProtests
{
    class PawnsArrivalModeWorker_PeacefulProtest : PawnsArrivalModeWorker_EdgeWalkIn
    {
        public override void Arrive(List<Pawn> pawns, IncidentParms parms)
        {
            Map map = (Map)parms.target;
            foreach (Pawn pawn in pawns)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(parms.spawnCenter, map, 8, null);

                pawn.Name = PeacefulNames.Generate(pawn);
                pawn.story.melanin = Rand.Range(0.85f, 1.0f);

                // the hediff is added to prevent work while the pawn is protesting
                Hediff hediff = HediffMaker.MakeHediff(DefDatabase<HediffDef>.GetNamed("PeacefulProtester"), pawn, null);
                if (hediff != null) pawn.health.AddHediff(hediff);
                
                // boost pawn's skills
                Teach(pawn);

                GenSpawn.Spawn(pawn, loc, map, parms.spawnRotation, WipeMode.Vanish, false);
            }
        }

        public void Teach(Pawn pawn)
        {
            foreach(SkillDef skillDef in DefDatabase<SkillDef>.AllDefsListForReading)
            {
                SkillRecord skill = pawn.skills.GetSkill(skillDef);

                if (skill.def == SkillDefOf.Melee || skill.def == SkillDefOf.Shooting)
                {
                    // set either combat skill to be guaranteed to be passion
                    skill.passion = Rand.Range(0.0f, 1.0f) > 0.2f ? Passion.Major : Passion.Minor;
                }
                else
                {
                    // for other skills, give a passion boost: Major at skill level 5 and Minor and level 2
                    skill.Level = (int)Rand.Value;
                    if (skill.Level > 5) skill.passion = Passion.Major;
                    else if (skill.Level > 2) skill.passion = Passion.Minor;
                    else skill.passion = Passion.None;
                }
            }
        }
    }


}
