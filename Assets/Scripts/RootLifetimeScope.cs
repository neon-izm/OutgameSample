using MessagePipe;
using NewDemo.Core.Scripts.Domain.Character.Model;
using NewDemo.Core.Scripts.Domain.Settings.Model;
using NewDemo.Core.Scripts.Infrastructure.Settings;
using NewDemo.Core.Scripts.UseCase.Settings;
using ScreenSystem.Page;
using ScreenSystem.VContainerExtension;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class RootLifetimeScope : LifetimeScope
{
    [SerializeField] UnityScreenNavigator.Runtime.Core.Page.PageContainer _container;
    [SerializeField] UnityScreenNavigator.Runtime.Core.Modal.ModalContainer _modalContainer;

    protected override void Configure(IContainerBuilder builder)
    {

        builder.RegisterPageSystem(_container);
        builder.RegisterModalSystem(_modalContainer);
        // このようにVContainerのライフサイクルに依存しないクラスはRegisterEntryPointではなく、Registerで登録する
        builder.Register<IHttpClient>(_ => new HttpClient(), Lifetime.Singleton);
        // 層別に登録順序を明確化
        ConfigureDomainLayer(builder);           // コア層
        ConfigureInfrastructureLayer(builder);  // Repository層
        ConfigureUseCaseLayer(builder);         // ユースケース層
        //あちこちのViewで参照されるServiceやUseCaseはここで登録する
        var options = builder.RegisterMessagePipe();
        
        // note: RegisterEntryPointはIStartableなどを実装しているクラスを登録するときに使用する.
        // AsSelf()はRegisterEntryPointで登録したクラスの実体化を即時に行うために使用する.
        builder.RegisterEntryPoint<PageRoutingService>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<GuidCounterService>(Lifetime.Singleton).AsSelf();

        builder.RegisterMessageBroker<MessagePipeCounterMessage>(options);
        builder.RegisterEntryPoint<DemoEntryPoint>().AsSelf();
    }

    /// <summary>
    /// Domain層の依存関係を設定
    /// 注意: この層のクラスは他の層から直接参照してはいけません
    /// </summary>
    private void ConfigureDomainLayer(IContainerBuilder builder)
    {
        // ドメインモデルをSingletonとして登録
        // ※ UseCase層からのみアクセス可能とする
        builder.Register<Character>(Lifetime.Singleton);
        builder.Register<DemoAudioSettings>(Lifetime.Singleton);
    }
    /// <summary>
    /// Infrastructure層の依存関係を設定
    /// 注意: この層のクラスは他の層から直接参照してはいけません
    /// </summary>
    private void ConfigureInfrastructureLayer(IContainerBuilder builder)
    {
        // Repository実装をSingletonとして登録
        // ※ UseCase層からのみアクセス可能とする
        builder.Register<UserSettingsRepository>(Lifetime.Singleton);
    }

    /// <summary>
    /// UseCase層の依存関係を設定
    /// この層がPresentation層からアクセスできる唯一の層です
    /// </summary>
    private void ConfigureUseCaseLayer(IContainerBuilder builder)
    {
        // ユーザー設定管理UseCase
        // Presentation層はこのUseCaseを通じてのみビジネスロジックにアクセス
        builder.Register<UserSettingsUseCase>(Lifetime.Singleton);
        
        // 将来の拡張例
        // builder.Register<GameplayUseCase>(Lifetime.Singleton);
        // builder.Register<BattleUseCase>(Lifetime.Singleton);
    }
    /// <summary>
    /// アプリの初期化の例
    /// </summary>
    private class DemoEntryPoint : IStartable
    {
        private readonly PageEventPublisher _publisher;

        public DemoEntryPoint(PageEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Start()
        {
            _publisher.SendPushEvent(new FirstPageBuilder());
        }
    }
}