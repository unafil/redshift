using Content.Server.Fluids.EntitySystems;
using Content.Server.Forensics;
using Content.Shared._Redshift.Sex.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
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

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GenitalComponent, ComponentStartup>(OnCompInit);
    }

    private void OnCompInit(Entity<GenitalComponent> ent, ref ComponentStartup args)
    {
        if (!_solutionContainer.EnsureSolution(ent.Owner, ent.Comp.SolutionName, out var solution, ent.Comp.MaxVolume))
            return;

        solution.AddReagent(ent.Comp.ReagentId, ent.Comp.MaxVolume - solution.Volume);
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
