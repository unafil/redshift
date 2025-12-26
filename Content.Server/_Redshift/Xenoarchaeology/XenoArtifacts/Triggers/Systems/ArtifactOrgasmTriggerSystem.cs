using Content.Server._Redshift.Sex;
using Content.Server._Redshift.Xenoarchaeology.XenoArtifacts.Triggers.Components;
using Content.Shared.Xenoarchaeology.Artifact.Components;
using Content.Shared.Xenoarchaeology.Artifact.XAT;

namespace Content.Server._Redshift.Xenoarchaeology.XenoArtifacts.Triggers.Systems;

public sealed class ArtifactOrgasmTriggerSystem : BaseXATSystem<ArtifactOrgasmTriggerComponent>
{
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    private EntityQuery<XenoArtifactComponent> _xenoArtifactQuery;

    public override void Initialize()
    {
        base.Initialize();

        _xenoArtifactQuery = GetEntityQuery<XenoArtifactComponent>();

        SubscribeLocalEvent<OrgasmEvent>(OnOrgasm);
    }

    private void OnOrgasm(OrgasmEvent args)
    {
        var query = EntityQueryEnumerator<ArtifactOrgasmTriggerComponent, XenoArtifactNodeComponent>();
        while (query.MoveNext(out var uid, out var comp, out var node))
        {
            if (node.Attached == null)
                continue;

            var artifact = _xenoArtifactQuery.Get(GetEntity(node.Attached.Value));

            if (!CanTrigger(artifact, (uid, node)))
                continue;

            var artifactPos = _transform.GetMapCoordinates(artifact);
            var genitalPos = _transform.GetMapCoordinates(args.Uid);

            var distance = (artifactPos.Position - genitalPos.Position);

            if (distance.LengthSquared() < 5)
            {
                Trigger(artifact, (uid, comp, node));
            }
        }
    }
}
