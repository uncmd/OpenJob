using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;
using Volo.Abp.Guids;
using Volo.Abp.Linq;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Settings;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace OpenJob.Server.Actors;

public abstract class ActorBase : ActorBase<Guid>
{

}

public abstract class ActorBase<TPrimaryKey> : Grain, IActor<TPrimaryKey>
{
    public IAbpLazyServiceProvider LazyServiceProvider { get; private set; }

    protected IClock Clock => LazyServiceProvider.LazyGetRequiredService<IClock>();

    protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();

    protected IGrainFactory ActorFactory => LazyServiceProvider.LazyGetRequiredService<IGrainFactory>();

    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>((provider) => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance);

    protected IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();

    protected IAsyncQueryableExecuter AsyncExecuter => LazyServiceProvider.LazyGetRequiredService<IAsyncQueryableExecuter>();

    protected Type ObjectMapperContext { get; set; }
    protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetService<IObjectMapper>(provider =>
        ObjectMapperContext == null
            ? provider.GetRequiredService<IObjectMapper>()
            : (IObjectMapper)provider.GetRequiredService(typeof(IObjectMapper<>).MakeGenericType(ObjectMapperContext)));

    protected ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();

    protected IDataFilter DataFilter => LazyServiceProvider.LazyGetRequiredService<IDataFilter>();

    protected ICurrentUser CurrentUser => LazyServiceProvider.LazyGetRequiredService<ICurrentUser>();

    protected ISettingProvider SettingProvider => LazyServiceProvider.LazyGetRequiredService<ISettingProvider>();

    protected IAuthorizationService AuthorizationService => LazyServiceProvider.LazyGetRequiredService<IAuthorizationService>();

    protected IFeatureChecker FeatureChecker => LazyServiceProvider.LazyGetRequiredService<IFeatureChecker>();

    protected IStringLocalizerFactory StringLocalizerFactory => LazyServiceProvider.LazyGetRequiredService<IStringLocalizerFactory>();

    protected IStringLocalizer L
    {
        get
        {
            if (_localizer == null)
            {
                _localizer = CreateLocalizer();
            }

            return _localizer;
        }
    }
    private IStringLocalizer _localizer;

    protected Type LocalizationResource
    {
        get => _localizationResource;
        set
        {
            _localizationResource = value;
            _localizer = null;
        }
    }
    private Type _localizationResource = typeof(DefaultResource);

    protected IUnitOfWork CurrentUnitOfWork => UnitOfWorkManager?.Current;

    public TPrimaryKey ActorId { get; private set; }

    protected Type ActorType => GetType();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var type = typeof(TPrimaryKey);
        if (type == typeof(long) && this.GetPrimaryKeyLong() is TPrimaryKey longKey)
        {
            ActorId = longKey;
        }
        else if (type == typeof(string) && this.GetPrimaryKeyString() is TPrimaryKey stringKey)
        {
            ActorId = stringKey;
        }
        else if (type == typeof(Guid) && this.GetPrimaryKey() is TPrimaryKey guidKey)
        {
            ActorId = guidKey;
        }
        else
        {
            throw new ArgumentOutOfRangeException(typeof(TPrimaryKey).FullName);
        }

        LazyServiceProvider = ServiceProvider.GetRequiredService<IAbpLazyServiceProvider>();

        return base.OnActivateAsync(cancellationToken);
    }

    protected virtual IStringLocalizer CreateLocalizer()
    {
        if (LocalizationResource != null)
        {
            return StringLocalizerFactory.Create(LocalizationResource);
        }

        var localizer = StringLocalizerFactory.CreateDefaultOrNull();
        if (localizer == null)
        {
            throw new AbpException($"Set {nameof(LocalizationResource)} or define the default localization resource type (by configuring the {nameof(AbpLocalizationOptions)}.{nameof(AbpLocalizationOptions.DefaultResourceType)}) to be able to use the {nameof(L)} object!");
        }

        return localizer;
    }
}
