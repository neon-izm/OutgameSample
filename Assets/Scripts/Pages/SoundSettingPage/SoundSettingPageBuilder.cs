using ScreenSystem.Page;

public class SoundSettingPageBuilder : PageBuilderBase<SoundSettingPageLifecycle, SoundSettingPageView>
{
    public SoundSettingPageBuilder(bool playAnimation = true, bool stack = true) : base(playAnimation, stack) { }
}