using ScreenSystem.Page;

public class CharacterColorChangePageBuilder : PageBuilderBase<CharacterColorChangePageLifecycle, CharacterColorChangePageView>
{
    public CharacterColorChangePageBuilder(bool playAnimation = true, bool stack = true) : base(playAnimation, stack) { }
} 