using RimWorld;
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