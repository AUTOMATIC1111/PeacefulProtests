using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace PeacefulProtests
{
    public class LordJob_PeacefulProtest : LordJob
    {
        public override bool GuiltyOnDowned
        {
            get
            {
                return true;
            }
        }

        public LordJob_PeacefulProtest()
        {
        }

        public LordJob_PeacefulProtest(SpawnedPawnParams parms)
        {
            protestingFaction = parms.spawnerThing.Faction;
        }

        public LordJob_PeacefulProtest(Faction faction)
        {
            protestingFaction = faction;
        }

        public override StateGraph CreateGraph()
        {
            // adopted from LordJob_AssaultColony
            // thanks G.J.S. for edits!

            // XXX figure out how to prevent protesters from taking stuff

            StateGraph stateGraph = new StateGraph();

            LordToil lordToilProtest = new LordToil_AssaultColony(false);
            stateGraph.AddToil(lordToilProtest);

            LordToil_ExitMap lordToil_ExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, false, true);
            lordToil_ExitMap.useAvoidGrid = true;
            stateGraph.AddToil(lordToil_ExitMap);

            Transition transition3 = new Transition(lordToilProtest, lordToil_ExitMap, false, true);
            transition3.AddTrigger(new Trigger_TicksPassed(ProtestDuration.RandomInRange));
            transition3.AddPreAction(new TransitionAction_Message("MessagePeacefulProtestersGivenUpLeaving".Translate(protestingFaction.def.pawnsPlural.CapitalizeFirst(), protestingFaction.Name), null, 1f));
            stateGraph.AddTransition(transition3, false);

            Transition transition4 = new Transition(lordToilProtest, lordToil_ExitMap, false, true);
            transition4.AddTrigger(new Trigger_FractionColonyDamageTaken(desiredColonyProtestFraction.RandomInRange, 200f));
            transition4.AddPreAction(new TransitionAction_Message("MessagePeacefulProtestersSatisfiedLeaving".Translate(protestingFaction.def.pawnsPlural.CapitalizeFirst(), protestingFaction.Name), null, 1f));
            stateGraph.AddTransition(transition4, false);

            LordToil startingToil = stateGraph.AttachSubgraph(new LordJob_Kidnap().CreateGraph()).StartingToil;
            Transition transition5 = new Transition(lordToilProtest, startingToil, false, true);
            transition5.AddPreAction(new TransitionAction_Message("MessagePeacefulProtestersMedics".Translate(protestingFaction.def.pawnsPlural.CapitalizeFirst(), protestingFaction.Name), null, 1f));
            transition5.AddTrigger(new Trigger_KidnapVictimPresent());
            stateGraph.AddTransition(transition5, false);

            LordToil startingToil2 = stateGraph.AttachSubgraph(new LordJob_Steal().CreateGraph()).StartingToil;
            Transition transition6 = new Transition(lordToilProtest, startingToil2, false, true);
            transition6.AddPreAction(new TransitionAction_Message("MessagePeacefulProtestersReparations".Translate(protestingFaction.def.pawnsPlural.CapitalizeFirst(), protestingFaction.Name), null, 1f));
            transition6.AddTrigger(new Trigger_HighValueThingsAround());
            stateGraph.AddTransition(transition6, false);

            return stateGraph;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref protestingFaction, "protestingFaction", false);
        }

        private Faction protestingFaction;
        private static readonly IntRange ProtestDuration = new IntRange(26000, 38000);
        private static readonly FloatRange desiredColonyProtestFraction = new FloatRange(0.25f, 0.35f);
    }
}
