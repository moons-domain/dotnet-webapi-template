using JetBrains.Annotations;
using NCrontab;

namespace $rootnamespace$;

[UsedImplicitly]
internal sealed class $safeitemname$ : CronScheduleService
{
    public $safeitemname$(IServiceProvider serviceProvider) : base(serviceProvider)
	{
}

    public override Task PerformServiceFunctionAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override CrontabSchedule GetSchedule()
    {
        throw new NotImplementedException();
    }

}