
/****** Object:  StoredProcedure [dbo].[sp_SearchSchedules]    Script Date: 8/30/2023 10:55:48 PM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.sp_SearchSportsSchedules'))
   exec sp_executesql N'CREATE PROCEDURE [dbo].[sp_SearchSportsSchedules] AS BEGIN SET NOCOUNT ON; END'
GO

ALTER PROCEDURE [dbo].[sp_SearchSportsSchedules](
	  @channels			nvarchar(max) = NULL
	, @startDate		datetime = NULL
	, @endDate			datetime = NULL
	, @sportsCategoryId	int = NULL
	, @searchWords		nvarchar(max) = NULL
	, @searchPhrase		nvarchar(max) = NULL
	, @unwantedWords	nvarchar(max) = NULL
	, @isLive			bit = NULL
) AS
BEGIN
	
	DECLARE @sql nvarchar(max) = N''
	DECLARE @cr nvarchar(max) = N'
'
	DECLARE @tb nvarchar(max) = N'					'
	DECLARE @or nvarchar(2) = N'OR'
	DECLARE @and nvarchar(3) = N'AND'
	DECLARE @base nvarchar(max) = N''
	SET @sql = @sql + N'
				DECLARE @Schedules TABLE(
					[Id] [uniqueidentifier] NOT NULL,
					[AfnId] [int] NOT NULL,
					[ChannelId] [int] NOT NULL,
					[SportName] [nvarchar](max) NULL,
					[IsTapeDelayed] [bit] NOT NULL,
					[IsLive] [bit] NOT NULL,
					[AirDateUTC] [datetime2](7) NOT NULL,
					[SportsNetworkId] [int] NULL,
					[SportsCategoryId] [int] NULL,
					[CreatedOnUTC] [datetime2](7) NOT NULL
				)

				INSERT INTO @Schedules
				SELECT 
					  [s].[Id]
					, [s].[AfnId]
					, [s].[ChannelId]
					, [s].[SportName]
					, [s].[IsTapeDelayed]
					, [s].[IsLive]
					, [s].[AirDateUTC]
					, [s].[SportsNetworkId]
					, [s].[SportsCategoryId]
					, [s].[CreatedOnUTC]
				FROM 
					[dbo].[SportsSchedules] [s]
				WHERE 
						[s].[Id] IS NOT NULL
					-- Make sure AirDateUTC is current
					AND [s].[AirDateUTC] >= DATEADD(hour, CAST(-12.0E0 AS int), GETUTCDATE()) ' + @cr

	IF @startDate IS NOT NULL
	BEGIN
		SET @sql = @sql + @tb + '-- Check Start Date' + @cr
		SET @sql = @sql + @tb + 'AND ([s].[AirDateUTC] >= ''' + FORMAT(@startDate, 'MM/dd/yyyy') + ''') ' + @cr
	END
	IF @endDate IS NOT NULL
	BEGIN
		SET @sql = @sql + @tb + '-- Check End Date' + @cr
		SET @sql = @sql + @tb + 'AND ([s].[AirDateUTC] <= ''' + FORMAT(@endDate, 'MM/dd/yyyy') + ''') ' + @cr
	END
	IF @isLive IS NOT NULL AND @isLive = 1
	BEGIN
		SET @sql = @sql + @tb + '-- Check IsLive' + @cr
		SET @sql = @sql + @tb + 'AND ([s].[IsLive] = 1) ' + @cr
	END
	IF @channels IS NOT NULL AND LEN(RTRIM(@channels)) > 0
	BEGIN
		SET @sql = @sql + @tb + '-- Check Channels' + @cr
		SET @sql = @sql + @tb + 'AND ([s].[ChannelId] IN (' + @channels + ')) ' + @cr
	END
	IF @sportsCategoryId IS NOT NULL
	BEGIN
		SET @sql = @sql + @tb + '-- Check SportsCategoryId' + @cr
		SET @sql = @sql + @tb + 'AND ([s].[SportsCategoryId] = ' + CONVERT(nvarchar(10), @sportsCategoryId) + @cr
	END

	-- SEARCH WORDS
	DECLARE @searchWordsSqlSportName nvarchar(max) = ''
	IF @searchWords IS NOT NULL AND LEN(TRIM(@searchWords)) > 0
	BEGIN
		SET @searchWordsSqlSportName = @searchWordsSqlSportName + @tb + '-- Check SportName' + @cr 
		SELECT @searchWordsSqlSportName = @searchWordsSqlSportName + @tb + '	OR ([s].[Id] IN (SELECT [s1].[Id] FROM [dbo].[SportsSchedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[SportName]) > 0)))' + @cr FROM STRING_SPLIT(@searchWords, '|')
	END

	-- SEARCH PHRASES
	DECLARE @searchPhraseSqlSportName nvarchar(max) = ''
	IF @searchPhrase IS NOT NULL AND LEN(RTRIM(@searchPhrase)) > 0
	BEGIN
		SET @searchPhraseSqlSportName = @searchPhraseSqlSportName + @tb + '-- Check SportName' + @cr 
		SET @searchPhraseSqlSportName = @searchPhraseSqlSportName + @tb + '	OR ((''' + REPLACE(@searchPhrase, '', '''') + ''' LIKE N'''') OR CHARINDEX(''' + REPLACE(REPLACE(@searchPhrase, '', ''''), '"', '') + ''', [s].[SportName]) > 0)' + @cr 
	END

	-- EXCLUDE UNWANTED WORDS
	DECLARE @unwantedWordsSqlSportName nvarchar(max) = ''
	IF @unwantedWords IS NOT NULL AND LEN(TRIM(@unwantedWords)) > 0
	BEGIN
		SET @unwantedWordsSqlSportName = @unwantedWordsSqlSportName + @tb + 'AND (1=1 ' + @cr
		SET @unwantedWordsSqlSportName = @unwantedWordsSqlSportName + @tb + '-- Check SportName' + @cr 
		SELECT @unwantedWordsSqlSportName = @unwantedWordsSqlSportName + @tb + '	AND ([s].[Id] NOT IN (SELECT [s1].[Id] FROM [dbo].[SportsSchedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[SportName]) > 0)))' + @cr FROM STRING_SPLIT(@unwantedWords, '|')
		SET @unwantedWordsSqlSportName = @unwantedWordsSqlSportName + @tb + ')' + @cr
	END

	SET @sql = @sql + @tb + '-- Search ALL' + @cr
	IF @searchWords IS NOT NULL AND LEN(TRIM(@searchWords)) > 0
	BEGIN
		SET @sql = @sql + @tb + '-- Search Words' + @cr
		SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
		SET @sql = @sql + @searchWordsSqlSportName
		SET @sql = @sql + @tb + ')' + @cr
	END

	IF @searchPhrase IS NOT NULL AND LEN(RTRIM(@searchPhrase)) > 0
	BEGIN
		SET @sql = @sql + @tb + '-- Search Phrase' + @cr
		SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
		SET @sql = @sql + @searchPhraseSqlSportName
		SET @sql = @sql + @tb + ')' + @cr
	END

	IF @unwantedWords IS NOT NULL AND LEN(TRIM(@unwantedWords)) > 0
	BEGIN
		SET @sql = @sql + @tb + '-- Unwanted Words' + @cr
		SET @sql = @sql + @unwantedWordsSqlSportName
	END

	SET @sql = @sql + @cr
	SET @sql = @sql + N'SELECT COUNT(*) AS ''TotalCount'' FROM @Schedules' + @cr
	SET @sql = @sql + @cr
	SET @sql = @sql + N'SELECT * FROM @Schedules' + @cr
	SET @sql = @sql + N'	ORDER BY [AirDateUTC] ASC' + @cr

	exec LongPrint @sql

	exec sp_executesql @sql
END
