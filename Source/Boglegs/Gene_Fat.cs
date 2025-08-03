using Verse;

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
}
