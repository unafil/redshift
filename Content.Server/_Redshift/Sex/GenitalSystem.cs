using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Shared._Redshift.Sex.Components;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.Timing;

namespace Content.Server._Redshift.Sex;

/// <summary>
/// This handles...
/// </summary>
public sealed class GenitalSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solutionContainer = default!; // deprecated???

    /// <inheritdoc/>
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
            if (now < genital.NextUpdateTime || genital.Solution == null) // evil ass null check (shared solutioncontainer refactor wacky)
                continue;

            genital.NextUpdateTime += genital.UpdateRate;

            _solutionContainer.TryAddReagent(genital.Solution.Value, genital.ReagentId, genital.QuantityPerUpdate, out _);
        }
    }
}
