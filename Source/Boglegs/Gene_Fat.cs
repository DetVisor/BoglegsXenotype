using Mono.Unix.Native;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

[DefOf]
public static class Bogleg_DefOf
{
    public static HediffDef Bogleg_FatHediff;

    static Bogleg_DefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(Bogleg_DefOf));
    }
}

namespace Boglegs
{
    public class Gene_Fat : Gene
    {
        public override void PostAdd()
        {
            base.PostAdd();
            pawn.health.AddHediff(Bogleg_DefOf.Bogleg_FatHediff);
        }

        public override void PostRemove()
        {
            base.PostRemove();
            pawn.health.RemoveHediff(pawn.health.hediffSet.GetFirstHediffOfDef(Bogleg_DefOf.Bogleg_FatHediff));
        }
    }

    public class HediffComp_Fat : HediffComp
    {
        public HediffCompProperties_Fat Props => props as HediffCompProperties_Fat;

        public float storedFat;

        public override string CompTipStringExtra => "Boglegs.StoredNutrition".Translate(Math.Round(storedFat, 2));

        public Need_Food cachedNeed;
        public Need_Food need
        {
            get
            {
                if (cachedNeed == null || cachedNeed.pawn == null)
                {
                    cachedNeed = parent.pawn.needs.TryGetNeed<Need_Food>();
                }

                return cachedNeed;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (need.CurLevel > Props.startThreshold && storedFat < Props.maxFat)
            {
                need.CurLevel -= Math.Min(Props.conversionSpeed, Props.maxFat - storedFat) * Props.conversionRate;
                storedFat += Math.Min(Props.conversionSpeed, Props.maxFat - storedFat);
            }
            else if (need.CurLevel < Props.starvationThreshold && storedFat > 0)
            {
                need.CurLevel += Math.Min(storedFat, Props.starvationSpeed);
                storedFat -= Math.Min(storedFat, Props.starvationSpeed);
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref storedFat, "storedFat");
        }
    }

    public class HediffCompProperties_Fat : HediffCompProperties
    {
        // Amount of nutrition past which fat is accumulated
        public float startThreshold;
        // Conversion rate between fat and nutrition when accumulating (when using fat, conversion rate is always 1)
        // Higher number will mean that more nutrition is converted into less fat
        public float conversionRate;
        // Amount of nutrition below which fat will be spent
        public float starvationThreshold;
        // Rate at which fat is accumulated, fat units per tick
        public float conversionSpeed;
        // Rate at which fat is spent, fat units per tick
        public float starvationSpeed;
        // Maximum amount of nutrition that can be stored
        public float maxFat;

        // Curve for slowdown, 1 = 100% movespeed
        public SimpleCurve slowdownCurve;
        // Curve for blunt protection, 1 = 100% blunt protection
        public SimpleCurve bluntProtectionCurve;
        // Curve for additional meat amount
        public SimpleCurve meatCurve;

        public HediffCompProperties_Fat()
        {
            compClass = typeof(HediffComp_Fat);
        }
    }

    public class HediffFat : HediffWithComps
    {
        private HediffComp_Fat cachedComp;
        private HediffComp_Fat Comp
        {
            get
            {
                if (cachedComp == null)
                {
                    cachedComp = this.TryGetComp<HediffComp_Fat>();
                }

                return cachedComp;
            }
        }

        private float cachedFat = -1f;
        private HediffStage cachedStage;

        public override HediffStage CurStage
        {
            get
            {
                if (cachedFat == Comp.storedFat)
                {
                    return cachedStage;
                }

                cachedFat = Comp.storedFat;
                cachedStage = new HediffStage();
                cachedStage.capMods = new List<PawnCapacityModifier>()
                {
                    new PawnCapacityModifier() { capacity = PawnCapacityDefOf.Moving, offset = Comp.Props.slowdownCurve.Evaluate(cachedFat) }
                };
                cachedStage.statOffsets = new List<StatModifier>()
                {
                    new StatModifier() { stat = StatDefOf.ArmorRating_Blunt, value = Comp.Props.bluntProtectionCurve.Evaluate(cachedFat) },
                    new StatModifier() { stat = StatDefOf.MeatAmount, value = Comp.Props.meatCurve.Evaluate(cachedFat) }
                };

                return cachedStage;
            }
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);
            Comp.cachedNeed = null;
        }
    }
}
