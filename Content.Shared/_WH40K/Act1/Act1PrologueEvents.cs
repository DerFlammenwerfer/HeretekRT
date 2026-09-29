using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._WH40K.Act1;

/// <summary>
/// Server-owned stage of the first-act prologue. Dialogue and cutscene systems advance it later.
/// </summary>
public enum Act1ProloguePhase : byte
{
    Intro,
    Exploration,
    Dialogue,
    Vision,
    Fracture,
    Transition,
}

/// <summary>
/// Role of an authored Act I map marker.
/// </summary>
public enum Act1PrologueMarkerRole : byte
{
    PlayerSpawn,
    EchoAnchor,
    VisionOne,
    VisionTwo,
    VisionThree,
    VisionFour,
    VisionFive,
    VisionSix,
    VisionSeven,
    VisionEight,
    FootfallArrival,
}

/// <summary>
/// A client-only cue layered above the world. It never changes gameplay state.
/// </summary>
public enum Act1ProloguePresentationCue : byte
{
    Intro,
    FadeToBlack,
    CancelTransition,
    ShatterToBlack,
    FadeFromBlack,
}

/// <summary>
/// Timing shared by the server trigger and the client presentation.
/// </summary>
public static class Act1ProloguePresentationTiming
{
    public static readonly TimeSpan IntroHoldDuration = TimeSpan.FromSeconds(1.2);
    public static readonly TimeSpan IntroFadeDuration = TimeSpan.FromSeconds(2.8);
    public static readonly TimeSpan IntroTotalDuration = IntroHoldDuration + IntroFadeDuration;
    public static readonly TimeSpan FadeToBlackDuration = TimeSpan.FromSeconds(1.6);
    public static readonly TimeSpan TransitionTimeout = TimeSpan.FromSeconds(15);
    public static readonly TimeSpan FractureBuildDuration = TimeSpan.FromSeconds(4.2);
    public static readonly TimeSpan FractureMaxHoldDuration = TimeSpan.FromMilliseconds(450);
    public static readonly TimeSpan ShatterToBlackDuration = TimeSpan.FromMilliseconds(500);
    public static readonly TimeSpan FadeFromBlackDuration = TimeSpan.FromSeconds(1.8);

    public static float GetFractureProgress(TimeSpan elapsed)
    {
        var seconds = Math.Clamp((float) elapsed.TotalSeconds, 0f, (float) FractureBuildDuration.TotalSeconds);
        return seconds switch
        {
            < 0.35f => Lerp(0.03f, 0.20f, seconds / 0.35f),
            < 1.15f => Lerp(0.20f, 0.64f, (seconds - 0.35f) / 0.80f),
            < 2.65f => Lerp(0.64f, 0.90f, (seconds - 1.15f) / 1.50f),
            _ => Lerp(0.90f, 1f, (seconds - 2.65f) / 1.55f),
        };
    }

    private static float Lerp(float start, float end, float amount) =>
        start + (end - start) * Math.Clamp(amount, 0f, 1f);

    public static TimeSpan GetDuration(Act1ProloguePresentationCue cue) => cue switch
    {
        Act1ProloguePresentationCue.Intro => IntroTotalDuration,
        Act1ProloguePresentationCue.FadeToBlack => FadeToBlackDuration,
        Act1ProloguePresentationCue.ShatterToBlack => ShatterToBlackDuration,
        Act1ProloguePresentationCue.FadeFromBlack => FadeFromBlackDuration,
        _ => TimeSpan.Zero,
    };
}

/// <summary>
/// Sent only to the participant of a private prologue instance.
/// </summary>
[Serializable, NetSerializable]
public sealed class Act1ProloguePresentationEvent(Act1ProloguePresentationCue cue) : EntityEventArgs
{
    public Act1ProloguePresentationCue Cue { get; } = cue;
}

/// <summary>
/// Server-timed fracture state. It affects presentation only, while all transfer and progression remain server-owned.
/// </summary>
[Serializable, NetSerializable]
public sealed class Act1PrologueFractureEvent(float degree, bool blackout) : EntityEventArgs
{
    public float Degree { get; } = degree;
    public bool Blackout { get; } = blackout;
}
