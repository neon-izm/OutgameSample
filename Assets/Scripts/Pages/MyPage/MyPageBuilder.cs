using ScreenSystem.Page;

public class MyPageBuilder : PageBuilderBase<MyPageLifecycle, MyPageView>
{
    public MyPageBuilder(bool playAnimation = true, bool stack = true) : base(playAnimation, stack) { }
} 