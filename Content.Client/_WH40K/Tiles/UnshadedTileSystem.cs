using Robust.Client.Graphics;

namespace Content.Client._WH40K.Tiles;

public sealed partial class UnshadedTileSystem : EntitySystem
{
    [Dependency] private IOverlayManager _overlayManager = default!;

    public override void Initialize()
    {
        base.Initialize();
        _overlayManager.AddOverlay(new UnshadedTileOverlay(EntityManager));
    }

    public override void Shutdown()
    {
        base.Shutdown();
        _overlayManager.RemoveOverlay<UnshadedTileOverlay>();
    }
}
