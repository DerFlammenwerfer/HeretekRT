using Robust.Shared.GameStates;

namespace Content.Shared._WH40K.Act1;

/// <summary>
/// Marks a player currently inside their private Act I instance. Its presence drives client HUD suppression.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class Act1PrologueComponent : Component
{
    [DataField, AutoNetworkedField]
    public Act1ProloguePhase Phase = Act1ProloguePhase.Intro;
}

/// <summary>
/// Blocks Act I interactions without blocking locomotion. The server remains authoritative.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState, Access(typeof(SharedAct1PrologueControlLockSystem))]
public sealed partial class Act1PrologueControlLockComponent : Component
{
    /// <summary>
    /// Exploration leaves movement alone; the vision tableau locks it without creating a second control component.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool BlockMovement;
}

/// <summary>
/// Authored point used by the Act I instance loader and later cutscene systems.
/// </summary>
[RegisterComponent]
public sealed partial class Act1PrologueMarkerComponent : Component
{
    [DataField(required: true)]
    public Act1PrologueMarkerRole Role;
}
