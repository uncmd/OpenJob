using PowerScheduler.Entities;
using PowerScheduler.Entities.Orleans;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace PowerScheduler.Data;

public class PowerSchedulerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    protected IGuidGenerator GuidGenerator { get; }
    protected ICurrentTenant CurrentTenant { get; }
    protected IRepository<OrleansQuery> OrleansQueryRepository { get; }
    protected IRepository<SchedulerJob, Guid> JobRepository { get; }
    protected IRepository<SchedulerTask, Guid> TaskRepository { get; }

    public PowerSchedulerDataSeedContributor(
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IRepository<OrleansQuery> orleansQueryRepository,
        IRepository<SchedulerJob, Guid> jobRepository,
        IRepository<SchedulerTask, Guid> taskRepository)
    {
        GuidGenerator = guidGenerator;
        CurrentTenant = currentTenant;
        OrleansQueryRepository = orleansQueryRepository;
        JobRepository = jobRepository;
        TaskRepository = taskRepository;
    }

    public Task SeedAsync(DataSeedContext context)
    {
        return SeedAsync(context.TenantId);
    }

    [UnitOfWork]
    protected async Task SeedAsync(Guid? tenantId = null)
    {
        using (CurrentTenant.Change(tenantId))
        {
            await SeedClustering();
            await SeedReminders();
            await SeedPersistence();
        }
    }

    protected async Task SeedClustering()
    {
        // updateIAmAlivetime
        var updateIAmAlivetime = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == updateIAmAlivetimeKey);

        if (updateIAmAlivetime != null)
        {
            return;
        }

        updateIAmAlivetime = new OrleansQuery(
            updateIAmAlivetimeKey,
            updateIAmAlivetimeText);

        await OrleansQueryRepository.InsertAsync(updateIAmAlivetime);

        // insertMembershipVersion
        var insertMembershipVersion = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == insertMembershipVersionKey);
        if (insertMembershipVersion == null)
        {
            insertMembershipVersion = new OrleansQuery(
                insertMembershipVersionKey,
                insertMembershipVersionText);

            await OrleansQueryRepository.InsertAsync(insertMembershipVersion);
        }

        // insertMembership
        var insertMembership = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == insertMembershipKey);
        if (insertMembership == null)
        {
            insertMembership = new OrleansQuery(
                insertMembershipKey,
                insertMembershipText);

            await OrleansQueryRepository.InsertAsync(insertMembership);
        }

        // updateMembership
        var updateMembership = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == updateMembershipKey);
        if (updateMembership == null)
        {
            updateMembership = new OrleansQuery(
                updateMembershipKey,
                updateMembershipText);

            await OrleansQueryRepository.InsertAsync(updateMembership);
        }

        // gatewaysQuery
        var gatewaysQuery = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == gatewaysQueryKey);
        if (gatewaysQuery == null)
        {
            gatewaysQuery = new OrleansQuery(
                gatewaysQueryKey,
                gatewaysQueryText);

            await OrleansQueryRepository.InsertAsync(gatewaysQuery);
        }

        // membershipReadRow
        var membershipReadRow = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == membershipReadRowKey);
        if (membershipReadRow == null)
        {
            membershipReadRow = new OrleansQuery(
                membershipReadRowKey,
                membershipReadRowText);

            await OrleansQueryRepository.InsertAsync(membershipReadRow);
        }

        // membershipReadAll
        var membershipReadAll = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == membershipReadAllKey);
        if (membershipReadAll == null)
        {
            membershipReadAll = new OrleansQuery(
                membershipReadAllKey,
                membershipReadAllText);

            await OrleansQueryRepository.InsertAsync(membershipReadAll);
        }

        // deleteMembershipTableEntries
        var deleteMembershipTableEntries = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == deleteMembershipTableEntriesKey);
        if (deleteMembershipTableEntries == null)
        {
            deleteMembershipTableEntries = new OrleansQuery(
                deleteMembershipTableEntriesKey,
                deleteMembershipTableEntriesText);

            await OrleansQueryRepository.InsertAsync(deleteMembershipTableEntries);
        }
    }

    protected async Task SeedReminders()
    {
        // upsertReminderRow
        var upsertReminderRow = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == upsertReminderRowKey);
        if (upsertReminderRow == null)
        {
            upsertReminderRow = new OrleansQuery(
                upsertReminderRowKey,
                upsertReminderRowText);

            await OrleansQueryRepository.InsertAsync(upsertReminderRow);
        }

        // readReminderRows
        var readReminderRows = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == readReminderRowsKey);
        if (readReminderRows == null)
        {
            readReminderRows = new OrleansQuery(
                readReminderRowsKey,
                readReminderRowsText);

            await OrleansQueryRepository.InsertAsync(readReminderRows);
        }

        // readReminderRow
        var readReminderRow = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == readReminderRowKey);
        if (readReminderRow == null)
        {
            readReminderRow = new OrleansQuery(
                readReminderRowKey,
                readReminderRowText);

            await OrleansQueryRepository.InsertAsync(readReminderRow);
        }

        // readRangeRows1
        var readRangeRows1 = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == readRangeRows1Key);
        if (readRangeRows1 == null)
        {
            readRangeRows1 = new OrleansQuery(
                readRangeRows1Key,
                readRangeRows1Text);

            await OrleansQueryRepository.InsertAsync(readRangeRows1);
        }

        // readRangeRows2
        var readRangeRows2 = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == readRangeRows2Key);
        if (readRangeRows2 == null)
        {
            readRangeRows2 = new OrleansQuery(
                readRangeRows2Key,
                readRangeRows2Text);

            await OrleansQueryRepository.InsertAsync(readRangeRows2);
        }

        // deleteReminderRow
        var deleteReminderRow = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == deleteReminderRowKey);
        if (deleteReminderRow == null)
        {
            deleteReminderRow = new OrleansQuery(
                deleteReminderRowKey,
                deleteReminderRowText);

            await OrleansQueryRepository.InsertAsync(deleteReminderRow);
        }

        // deleteReminderRows
        var deleteReminderRows = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == deleteReminderRowsKey);
        if (deleteReminderRows == null)
        {
            deleteReminderRows = new OrleansQuery(
                deleteReminderRowsKey,
                deleteReminderRowsText);

            await OrleansQueryRepository.InsertAsync(deleteReminderRows);
        }
    }

    protected async Task SeedPersistence()
    {
        // writeToStorage
        var writeToStorage = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == writeToStorageKey);
        if (writeToStorage == null)
        {
            writeToStorage = new OrleansQuery(
                writeToStorageKey,
                writeToStorageText);

            await OrleansQueryRepository.InsertAsync(writeToStorage);
        }

        // clearStorage
        var clearStorage = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == clearStorageKey);
        if (clearStorage == null)
        {
            clearStorage = new OrleansQuery(
                clearStorageKey,
                clearStorageText);

            await OrleansQueryRepository.InsertAsync(clearStorage);
        }

        // readFromStorage
        var readFromStorage = await OrleansQueryRepository
            .FirstOrDefaultAsync(p => p.QueryKey == readFromStorageKey);
        if (readFromStorage == null)
        {
            readFromStorage = new OrleansQuery(
                readFromStorageKey,
                readFromStorageText);

            await OrleansQueryRepository.InsertAsync(readFromStorage);
        }
    }

    // Clustering
    const string updateIAmAlivetimeKey = "UpdateIAmAlivetimeKey";
    const string updateIAmAlivetimeText = @"
    -- This is expected to never fail by Orleans, so return value
	-- is not needed nor is it checked.
	SET NOCOUNT ON;
	UPDATE OrleansMembershipTable
	SET
		IAmAliveTime = @IAmAliveTime
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Address = @Address AND @Address IS NOT NULL
		AND Port = @Port AND @Port IS NOT NULL
		AND Generation = @Generation AND @Generation IS NOT NULL;";

    const string insertMembershipVersionKey = "InsertMembershipVersionKey";
    const string insertMembershipVersionText = @"
    SET NOCOUNT ON;
	INSERT INTO OrleansMembershipVersionTable
	(
		DeploymentId
	)
	SELECT @DeploymentId
	WHERE NOT EXISTS
	(
		SELECT 1
		FROM
			OrleansMembershipVersionTable WITH(HOLDLOCK, XLOCK, ROWLOCK)
		WHERE
			DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
	);

	SELECT @@ROWCOUNT;";

    const string insertMembershipKey = "InsertMembershipKey";
    const string insertMembershipText = @"
	SET XACT_ABORT, NOCOUNT ON;
	DECLARE @ROWCOUNT AS INT;
	BEGIN TRANSACTION;
	INSERT INTO OrleansMembershipTable
	(
		DeploymentId,
		Address,
		Port,
		Generation,
		SiloName,
		HostName,
		Status,
		ProxyPort,
		StartTime,
		IAmAliveTime
	)
	SELECT
		@DeploymentId,
		@Address,
		@Port,
		@Generation,
		@SiloName,
		@HostName,
		@Status,
		@ProxyPort,
		@StartTime,
		@IAmAliveTime
	WHERE NOT EXISTS
	(
		SELECT 1
		FROM
			OrleansMembershipTable WITH(HOLDLOCK, XLOCK, ROWLOCK)
		WHERE
			DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
			AND Address = @Address AND @Address IS NOT NULL
			AND Port = @Port AND @Port IS NOT NULL
			AND Generation = @Generation AND @Generation IS NOT NULL
	);

	UPDATE OrleansMembershipVersionTable
	SET
		Timestamp = GETUTCDATE(),
		Version = Version + 1
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Version = @Version AND @Version IS NOT NULL
		AND @@ROWCOUNT > 0;

	SET @ROWCOUNT = @@ROWCOUNT;

	IF @ROWCOUNT = 0
		ROLLBACK TRANSACTION
	ELSE
		COMMIT TRANSACTION
	SELECT @ROWCOUNT;";

    const string updateMembershipKey = "UpdateMembershipKey";
    const string updateMembershipText = @"
	SET XACT_ABORT, NOCOUNT ON;
	BEGIN TRANSACTION;

	UPDATE OrleansMembershipVersionTable
	SET
		Timestamp = GETUTCDATE(),
		Version = Version + 1
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Version = @Version AND @Version IS NOT NULL;

	UPDATE OrleansMembershipTable
	SET
		Status = @Status,
		SuspectTimes = @SuspectTimes,
		IAmAliveTime = @IAmAliveTime
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Address = @Address AND @Address IS NOT NULL
		AND Port = @Port AND @Port IS NOT NULL
		AND Generation = @Generation AND @Generation IS NOT NULL
		AND @@ROWCOUNT > 0;

	SELECT @@ROWCOUNT;
	COMMIT TRANSACTION;";

    const string gatewaysQueryKey = "GatewaysQueryKey";
    const string gatewaysQueryText = @"
	SELECT
		Address,
		ProxyPort,
		Generation
	FROM
		OrleansMembershipTable
	WHERE
		DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL
		AND Status = @Status AND @Status IS NOT NULL
		AND ProxyPort > 0;";

    const string membershipReadRowKey = "MembershipReadRowKey";
    const string membershipReadRowText = @"
	SELECT
		v.DeploymentId,
		m.Address,
		m.Port,
		m.Generation,
		m.SiloName,
		m.HostName,
		m.Status,
		m.ProxyPort,
		m.SuspectTimes,
		m.StartTime,
		m.IAmAliveTime,
		v.Version
	FROM
		OrleansMembershipVersionTable v
		-- This ensures the version table will returned even if there is no matching membership row.
		LEFT OUTER JOIN OrleansMembershipTable m ON v.DeploymentId = m.DeploymentId
		AND Address = @Address AND @Address IS NOT NULL
		AND Port = @Port AND @Port IS NOT NULL
		AND Generation = @Generation AND @Generation IS NOT NULL
	WHERE
		v.DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;";

    const string membershipReadAllKey = "MembershipReadAllKey";
    const string membershipReadAllText = @"
	SELECT
		v.DeploymentId,
		m.Address,
		m.Port,
		m.Generation,
		m.SiloName,
		m.HostName,
		m.Status,
		m.ProxyPort,
		m.SuspectTimes,
		m.StartTime,
		m.IAmAliveTime,
		v.Version
	FROM
		OrleansMembershipVersionTable v LEFT OUTER JOIN OrleansMembershipTable m
		ON v.DeploymentId = m.DeploymentId
	WHERE
		v.DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;";

    const string deleteMembershipTableEntriesKey = "DeleteMembershipTableEntriesKey";
    const string deleteMembershipTableEntriesText = @"
	DELETE FROM OrleansMembershipTable
	WHERE DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;
	DELETE FROM OrleansMembershipVersionTable
	WHERE DeploymentId = @DeploymentId AND @DeploymentId IS NOT NULL;";

    // Reminders
    const string upsertReminderRowKey = "UpsertReminderRowKey";
    const string upsertReminderRowText = @"
	DECLARE @Version AS INT = 0;
	SET XACT_ABORT, NOCOUNT ON;
	BEGIN TRANSACTION;
	UPDATE OrleansRemindersTable WITH(UPDLOCK, ROWLOCK, HOLDLOCK)
	SET
		StartTime = @StartTime,
		Period = @Period,
		GrainHash = @GrainHash,
		@Version = Version = Version + 1
	WHERE
		ServiceId = @ServiceId AND @ServiceId IS NOT NULL
		AND GrainId = @GrainId AND @GrainId IS NOT NULL
		AND ReminderName = @ReminderName AND @ReminderName IS NOT NULL;

	INSERT INTO OrleansRemindersTable
	(
		ServiceId,
		GrainId,
		ReminderName,
		StartTime,
		Period,
		GrainHash,
		Version
	)
	SELECT
		@ServiceId,
		@GrainId,
		@ReminderName,
		@StartTime,
		@Period,
		@GrainHash,
		0
	WHERE
		@@ROWCOUNT=0;
	SELECT @Version AS Version;
	COMMIT TRANSACTION;";

    const string readReminderRowsKey = "ReadReminderRowsKey";
    const string readReminderRowsText = @"
	SELECT
		GrainId,
		ReminderName,
		StartTime,
		Period,
		Version
	FROM OrleansRemindersTable
	WHERE
		ServiceId = @ServiceId AND @ServiceId IS NOT NULL
		AND GrainId = @GrainId AND @GrainId IS NOT NULL;";

    const string readReminderRowKey = "ReadReminderRowKey";
    const string readReminderRowText = @"
	SELECT
		GrainId,
		ReminderName,
		StartTime,
		Period,
		Version
	FROM OrleansRemindersTable
	WHERE
		ServiceId = @ServiceId AND @ServiceId IS NOT NULL
		AND GrainId = @GrainId AND @GrainId IS NOT NULL
		AND ReminderName = @ReminderName AND @ReminderName IS NOT NULL;";

    const string readRangeRows1Key = "ReadRangeRows1Key";
    const string readRangeRows1Text = @"
	SELECT
		GrainId,
		ReminderName,
		StartTime,
		Period,
		Version
	FROM OrleansRemindersTable
	WHERE
		ServiceId = @ServiceId AND @ServiceId IS NOT NULL
		AND GrainHash > @BeginHash AND @BeginHash IS NOT NULL
		AND GrainHash <= @EndHash AND @EndHash IS NOT NULL;";

    const string readRangeRows2Key = "ReadRangeRows2Key";
    const string readRangeRows2Text = @"
	SELECT
		GrainId,
		ReminderName,
		StartTime,
		Period,
		Version
	FROM OrleansRemindersTable
	WHERE
		ServiceId = @ServiceId AND @ServiceId IS NOT NULL
		AND ((GrainHash > @BeginHash AND @BeginHash IS NOT NULL)
		OR (GrainHash <= @EndHash AND @EndHash IS NOT NULL));";

    const string deleteReminderRowKey = "DeleteReminderRowKey";
    const string deleteReminderRowText = @"
	DELETE FROM OrleansRemindersTable
	WHERE
		ServiceId = @ServiceId AND @ServiceId IS NOT NULL
		AND GrainId = @GrainId AND @GrainId IS NOT NULL
		AND ReminderName = @ReminderName AND @ReminderName IS NOT NULL
		AND Version = @Version AND @Version IS NOT NULL;
	SELECT @@ROWCOUNT;";

    const string deleteReminderRowsKey = "DeleteReminderRowsKey";
    const string deleteReminderRowsText = @"
	DELETE FROM OrleansRemindersTable
	WHERE
		ServiceId = @ServiceId AND @ServiceId IS NOT NULL;";

    // Persistence
    const string writeToStorageKey = "WriteToStorageKey";
    const string writeToStorageText = @"
    -- When Orleans is running in normal, non-split state, there will
    -- be only one grain with the given ID and type combination only. This
    -- grain saves states mostly serially if Orleans guarantees are upheld. Even
    -- if not, the updates should work correctly due to version number.
    --
    -- In split brain situations there can be a situation where there are two or more
    -- grains with the given ID and type combination. When they try to INSERT
    -- concurrently, the table needs to be locked pessimistically before one of
    -- the grains gets @GrainStateVersion = 1 in return and the other grains will fail
    -- to update storage. The following arrangement is made to reduce locking in normal operation.
    --
    -- If the version number explicitly returned is still the same, Orleans interprets it so the update did not succeed
    -- and throws an InconsistentStateException.
    --
    -- See further information at https://docs.microsoft.com/dotnet/orleans/grains/grain-persistence.
    BEGIN TRANSACTION;
    SET XACT_ABORT, NOCOUNT ON;

    DECLARE @NewGrainStateVersion AS INT = @GrainStateVersion;


    -- If the @GrainStateVersion is not zero, this branch assumes it exists in this database.
    -- The NULL value is supplied by Orleans when the state is new.
    IF @GrainStateVersion IS NOT NULL
    BEGIN
        UPDATE OrleansStorage
        SET
            PayloadBinary = @PayloadBinary,
            PayloadJson = @PayloadJson,
            PayloadXml = @PayloadXml,
            ModifiedOn = GETUTCDATE(),
            Version = Version + 1,
            @NewGrainStateVersion = Version + 1,
            @GrainStateVersion = Version + 1
        WHERE
            GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
            AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
            AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
            AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
            AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
            AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
            AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
            AND Version IS NOT NULL AND Version = @GrainStateVersion AND @GrainStateVersion IS NOT NULL
            OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));
    END

    -- The grain state has not been read. The following locks rather pessimistically
    -- to ensure only one INSERT succeeds.
    IF @GrainStateVersion IS NULL
    BEGIN
        INSERT INTO OrleansStorage
        (
            GrainIdHash,
            GrainIdN0,
            GrainIdN1,
            GrainTypeHash,
            GrainTypeString,
            GrainIdExtensionString,
            ServiceId,
            PayloadBinary,
            PayloadJson,
            PayloadXml,
            ModifiedOn,
            Version
        )
        SELECT
            @GrainIdHash,
            @GrainIdN0,
            @GrainIdN1,
            @GrainTypeHash,
            @GrainTypeString,
            @GrainIdExtensionString,
            @ServiceId,
            @PayloadBinary,
            @PayloadJson,
            @PayloadXml,
            GETUTCDATE(),
            1
         WHERE NOT EXISTS
         (
            -- There should not be any version of this grain state.
            SELECT 1
            FROM OrleansStorage WITH(XLOCK, ROWLOCK, HOLDLOCK, INDEX(IX_OrleansStorage))
            WHERE
                GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
                AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
                AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
                AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
                AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
                AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
                AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
         ) OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));

        IF @@ROWCOUNT > 0
        BEGIN
            SET @NewGrainStateVersion = 1;
        END
    END

    SELECT @NewGrainStateVersion AS NewGrainStateVersion;
    COMMIT TRANSACTION;";

    const string clearStorageKey = "ClearStorageKey";
    const string clearStorageText = @"
    BEGIN TRANSACTION;
    SET XACT_ABORT, NOCOUNT ON;
    DECLARE @NewGrainStateVersion AS INT = @GrainStateVersion;
    UPDATE OrleansStorage
    SET
        PayloadBinary = NULL,
        PayloadJson = NULL,
        PayloadXml = NULL,
        ModifiedOn = GETUTCDATE(),
        Version = Version + 1,
        @NewGrainStateVersion = Version + 1
    WHERE
        GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
        AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
        AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
        AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
        AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
        AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
        AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        AND Version IS NOT NULL AND Version = @GrainStateVersion AND @GrainStateVersion IS NOT NULL
        OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));

    SELECT @NewGrainStateVersion;
    COMMIT TRANSACTION;";

    const string readFromStorageKey = "ReadFromStorageKey";
    const string readFromStorageText = @"
    -- The application code will deserialize the relevant result. Not that the query optimizer
    -- estimates the result of rows based on its knowledge on the index. It does not know there
    -- will be only one row returned. Forcing the optimizer to process the first found row quickly
    -- creates an estimate for a one-row result and makes a difference on multi-million row tables.
    -- Also the optimizer is instructed to always use the same plan via index using the OPTIMIZE
    -- FOR UNKNOWN flags. These hints are only available in SQL Server 2008 and later. They
    -- should guarantee the execution time is robustly basically the same from query-to-query.
    SELECT
        PayloadBinary,
        PayloadXml,
        PayloadJson,
        Version
    FROM
        OrleansStorage
    WHERE
        GrainIdHash = @GrainIdHash AND @GrainIdHash IS NOT NULL
        AND GrainTypeHash = @GrainTypeHash AND @GrainTypeHash IS NOT NULL
        AND (GrainIdN0 = @GrainIdN0 OR @GrainIdN0 IS NULL)
        AND (GrainIdN1 = @GrainIdN1 OR @GrainIdN1 IS NULL)
        AND (GrainTypeString = @GrainTypeString OR @GrainTypeString IS NULL)
        AND ((@GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString IS NOT NULL AND GrainIdExtensionString = @GrainIdExtensionString) OR @GrainIdExtensionString IS NULL AND GrainIdExtensionString IS NULL)
        AND ServiceId = @ServiceId AND @ServiceId IS NOT NULL
        OPTION(FAST 1, OPTIMIZE FOR(@GrainIdHash UNKNOWN, @GrainTypeHash UNKNOWN));";
}
