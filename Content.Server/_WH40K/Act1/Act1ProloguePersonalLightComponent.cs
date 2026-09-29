using Robust.Shared.GameObjects;

namespace Content.Server._WH40K.Act1;

/// <summary>
/// Marks the temporary light installed on a player while they are inside Act I.
/// </summary>
[RegisterComponent, Access(typeof(Act1PrologueSystem))]
public sealed partial class Act1ProloguePersonalLightComponent : Component
{
}
