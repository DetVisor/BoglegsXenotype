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
        private HediffCompProperties_Fat Props => props as HediffCompProperties_Fat;

        public float storedFat;

        public override string CompTipStringExtra => Math.Round(storedFat * 100).ToString() + " nutrition stored";

        private Need_Food cachedNeed;
        private Need_Food need
        {
            get
            {
                if (cachedNeed == null)
                {
                    cachedNeed = parent.pawn.needs.TryGetNeed<Need_Food>();
                }

                return cachedNeed;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            Log.Message(need.CurLevel + "");

            if (need.CurLevel > Props.startThreshold)
            {
                need.CurLevel -= Props.conversionSpeed * Props.conversionRate;
                storedFat += Props.conversionSpeed;
            }
            else if (need.CurLevel < Props.startThreshold && storedFat > 0)
            {
                need.CurLevel += Math.Min(storedFat, Props.starvationSpeed);
                storedFat -= Math.Min(storedFat, Props.starvationSpeed);
            }
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

        public HediffCompProperties_Fat()
        {
            compClass = typeof(HediffComp_Fat);
        }
    }
}
