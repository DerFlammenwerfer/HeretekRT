using System;
using System.Numerics;
using Content.Shared._WH40K.Act1;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Client._WH40K.Act1.UI;

/// <summary>
/// Non-interactive full-screen black fade used before the player is shown the white room.
/// </summary>
public sealed class Act1ProloguePresentationControl : Control
{
    private readonly Font _titleFont;
    private readonly string _title;
    private Act1ProloguePresentationCue _cue;
    private TimeSpan _elapsed;

    public Act1ProloguePresentationControl()
    {
        var resources = IoCManager.Resolve<IResourceCache>();
        _titleFont = new VectorFont(
            resources.GetResource<FontResource>("/Fonts/_WH40K/CormorantSC/CormorantSC-Regular.ttf"),
            52);
        _title = Loc.GetString("heretek-act1-presentation-title");

        MouseFilter = MouseFilterMode.Ignore;
        RectClipContent = true;
    }

    public TimeSpan Duration => Act1ProloguePresentationTiming.GetDuration(_cue);

    public void Start(Act1ProloguePresentationCue cue)
    {
        _cue = cue;
        _elapsed = TimeSpan.Zero;
        Visible = true;
    }

    public void SetElapsed(TimeSpan elapsed)
    {
        _elapsed = elapsed < TimeSpan.Zero ? TimeSpan.Zero : elapsed;
    }

    protected override void Draw(DrawingHandleScreen handle)
    {
        base.Draw(handle);

        if (PixelSize.X <= 0f || PixelSize.Y <= 0f)
            return;

        switch (_cue)
        {
            case Act1ProloguePresentationCue.Intro:
                DrawIntro(handle);
                break;
            case Act1ProloguePresentationCue.FadeToBlack:
                DrawFadeToBlack(handle);
                break;
            case Act1ProloguePresentationCue.FadeFromBlack:
                DrawFadeFromBlack(handle);
                break;
        }
    }

    private void DrawIntro(DrawingHandleScreen handle)
    {
        var hold = Act1ProloguePresentationTiming.IntroHoldDuration;
        var fade = Act1ProloguePresentationTiming.IntroFadeDuration;
        var fadeProgress = Math.Clamp((float) ((_elapsed - hold) / fade), 0f, 1f);
        var blackOpacity = 1f - SmoothStep(fadeProgress);
        handle.DrawRect(PixelSizeBox, Color.Black.WithAlpha(blackOpacity));

        var titleFadeIn = Math.Clamp((float) (_elapsed.TotalSeconds / 0.45f), 0f, 1f);
        var titleOpacity = SmoothStep(titleFadeIn) * blackOpacity;
        if (titleOpacity <= 0f)
            return;

        var textSize = handle.GetDimensions(_titleFont, _title, UIScale);
        var position = (PixelSize - textSize) / 2f;
        handle.DrawString(_titleFont, position, _title, UIScale, Color.White.WithAlpha(titleOpacity));
    }

    private void DrawFadeToBlack(DrawingHandleScreen handle)
    {
        var duration = Act1ProloguePresentationTiming.FadeToBlackDuration;
        var progress = Math.Clamp((float) (_elapsed / duration), 0f, 1f);
        handle.DrawRect(PixelSizeBox, Color.Black.WithAlpha(SmoothStep(progress)));
    }

    private void DrawFadeFromBlack(DrawingHandleScreen handle)
    {
        var duration = Act1ProloguePresentationTiming.FadeFromBlackDuration;
        var progress = Math.Clamp((float) (_elapsed / duration), 0f, 1f);
        handle.DrawRect(PixelSizeBox, Color.Black.WithAlpha(1f - SmoothStep(progress)));
    }

    private static float SmoothStep(float value)
    {
        return value * value * (3f - 2f * value);
    }
}
