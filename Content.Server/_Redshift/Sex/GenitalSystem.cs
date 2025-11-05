using Content.Server.Fluids.EntitySystems;
using Content.Server.Forensics;
using Content.Shared._Redshift.Sex.Components;
using Content.Shared._Redshift.Undies;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Inventory;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Redshift.Sex;

public sealed class GenitalSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainer = default!; // deprecated???
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly PuddleSystem _puddle = default!;
    [Dependency] private readonly ForensicsSystem _forensics = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GenitalComponent, ComponentStartup>(OnCompInit);
        SubscribeLocalEvent<GenitalComponent, ExaminedEvent>(OnExamine);
    }

    private void OnCompInit(Entity<GenitalComponent> ent, ref ComponentStartup args)
    {
        if (!_solutionContainer.EnsureSolution(ent.Owner, ent.Comp.SolutionName, out var solution, ent.Comp.MaxVolume))
            return;

        solution.AddReagent(ent.Comp.ReagentId, ent.Comp.MaxVolume - solution.Volume);
    }

    private void OnExamine(Entity<GenitalComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.ExamineText == null || !Loc.HasString(ent.Comp.ExamineText))
            return;


        // i will be attempting to handle most logic inside the fluent text itself, because i enjoy masochism
        // due to this, we will simply pass a fuckton of arguments into GetString

        // underwear goidacode PLEASE cache this shit somewhere instead of doing this massive check 24/7
        // ffs im stealing this from ArousalSystem and it's ugly there too
        var hasUnderwear = false;
        if (TryComp<HumanoidAppearanceComponent>(ent, out var humApp))
        {
            if(humApp.MarkingSet.Markings.TryGetValue(MarkingCategories.UndergarmentBottom, out var markings))
            {
                foreach (var marking in markings)
                {
                    if (humApp.HiddenMarkings.Contains(marking.MarkingId)) // has underwear but it's disabled
                        continue;

                    hasUnderwear = true;
                }
            }
        }

        int ars = 0;
        var pastThreshold = false;
        if (TryComp<ArousalComponent>(ent, out var arousal))
        {
            ars = (int)arousal.CurrentArousal / 25;
            pastThreshold = arousal.CurrentArousal > ent.Comp.ExamineArousalThreshold;
        }

        // we differentiate solely between skirts and Everything Else via checking clothing BlockUndies values
        // this is, frankly, horrible
        var uniformType = "suit";
        if (_inventory.TryGetSlotEntity(ent, "jumpsuit", out var jumpSuit))
        {
            if (TryComp<BlockUndiesComponent>(jumpSuit, out var block) && !block.BlockedLayers.Contains(HumanoidVisualLayers.UndergarmentBottom))
                uniformType = "skirt";
        }
        else
        {
            uniformType = "none";
        }

        // :fear:
        args.PushMarkup(Loc.GetString(ent.Comp.ExamineText, ("hasUnderwear", hasUnderwear), ("arousal", ars), ("uniformType", uniformType), ("ent", ent), ("pastThreshold", pastThreshold)), -1);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var queryGenital = EntityQueryEnumerator<GenitalComponent>();
        var now = _timing.CurTime;

        while (queryGenital.MoveNext(out var uid, out var genital))
        {
            if (now < genital.NextUpdateTime)
                continue;

            if (!_solutionContainer.ResolveSolution(uid, genital.SolutionName, ref genital.Solution))
                continue;

            genital.NextUpdateTime += genital.UpdateRate;

            _solutionContainer.TryAddReagent(genital.Solution.Value, genital.ReagentId, genital.QuantityPerUpdate, out _);
        }
    }

    /// <summary>
    /// Returns False when empty.
    /// </summary>
    public bool Emit(Entity<GenitalComponent> ent)
    {
        if (!_solutionContainer.TryGetSolution(ent.Owner, ent.Comp.SolutionName, out var sol, out var solState))
            return false;

        var emissionSolution = new Solution();
        FixedPoint2 emissionAmount = Math.Clamp((float)solState.Volume / 2, 5, 30);
        var removedReagentSolution = _solutionContainer.SplitSolution(sol.Value, emissionAmount);
        emissionSolution.AddSolution(removedReagentSolution, _prototypes);

        // TrySpillAt fails randomly, not entirely sure why. not vital so i don't particularly care enough to fix it
        // todo (for future gamers): try to fix it
        if (_puddle.TrySpillAt(ent, emissionSolution, out var puddle, true))
        {
            _forensics.TransferDna(puddle, ent.Owner, false); // detective work gonna go crazy
        }

        return sol.Value.Comp.Solution.Volume > 0; // if empty, signal to ArousalSystem we're done
    }
}
