using Content.Server.Fluids.EntitySystems;
using Content.Server.Forensics;
using Content.Shared._Redshift.Sex.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Jittering;
using Content.Shared.Stunnable;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._Redshift.Sex;

/// <summary>
/// This handles...
/// </summary>
public sealed class GenitalSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainer = default!; // deprecated???
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly PuddleSystem _puddle = default!;
    [Dependency] private readonly ForensicsSystem _forensics = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GenitalComponent, ComponentStartup>(OnCompInit);
    }

    private void OnCompInit(Entity<GenitalComponent> ent, ref ComponentStartup args)
    {
        if (!_solutionContainer.EnsureSolution(ent.Owner, ent.Comp.SolutionName, out var solution, ent.Comp.MaxVolume))
        {
            Log.Info("Failed to ensure solution!");
            return;
        }

        solution.AddReagent(ent.Comp.ReagentId, ent.Comp.MaxVolume - solution.Volume);

        //_solutionContainer.ResolveSolution(ent.Owner, ent.Comp.SolutionName, ref ent.Comp.Solution); // i. think this is how this works
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var queryGenital = EntityQueryEnumerator<GenitalComponent>();
        var now = _timing.CurTime;

        while (queryGenital.MoveNext(out var uid, out var genital))
        {
            if (now < genital.NextUpdateTime) // evil ass null check (shared solutioncontainer refactor wacky)
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
        if (!_solutionContainer.TryGetSolution(ent.Owner, ent.Comp.SolutionName, out var sol, out _))
        {
            Log.Info("failed trygetsolution");
            return false;
        }

        /*
        if (ent.Comp.Solution == null)
        {
            Log.Info("Emit solution null!");
            return false;
        }
        */

        var emissionSolution = new Solution();
        FixedPoint2 emissionAmount = Math.Clamp((float)sol.Value.Comp.Solution.Volume / 2, 5, 30); // what the fuck
        //FixedPoint2 emissionAmount = 5;
        Log.Info("sol before split: " + sol.Value.Comp.Solution.Volume);
        var removedReagentSolution = _solutionContainer.SplitSolution(sol.Value, emissionAmount);
        //var removedReagentSolution = sol.Value.Comp.Solution.SplitSolution(emissionAmount);
        Log.Info("sol after split: " + sol.Value.Comp.Solution.Volume);
        emissionSolution.AddSolution(removedReagentSolution, _prototypes);
        //_solutionContainer.TryAddSolution(emissionSolution, removedReagentSolution);

        //_solutionContainer.RemoveAllSolution(sol.Value); // FUCK YOU

        Log.Info("Emission solution volume: " + emissionSolution.Volume);

        if (_puddle.TrySpillAt(ent, emissionSolution, out var puddle))
        {
            _forensics.TransferDna(puddle, ent, false); // detective work gonna go crazy
        }
        else
        {
            Log.Info("Failed to spill!");
        }

        Log.Info("sol solution volume (yo balls): " + sol.Value.Comp.Solution.Volume);

        if(sol.Value.Comp.Solution.Volume <= 0) // empty, signal to ArousalSystem we're done
            return false;

        return true;
    }
}
