--declare @searchWords nvarchar(max) = 'superman|lois'
--declare @channels nvarchar(max) = '1,3'
--declare @startDate datetime = '9/1/2023'
--declare @endDate datetime = '9/30/2023'
--declare @rating nvarchar(max) = 'TV-14'
--declare @searchField nvarchar(max) = 'TITLE'
--declare @searchPhrase nvarchar(max) = NULL
--declare @unwantedWords nvarchar(max) = 'family|guy'

/****** Object:  StoredProcedure [dbo].[sp_SearchSchedules]    Script Date: 8/30/2023 10:55:48 PM ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.sp_SearchSchedules'))
   exec sp_executesql N'CREATE PROCEDURE [dbo].[sp_SearchSchedules] AS BEGIN SET NOCOUNT ON; END'
GO

ALTER PROCEDURE [dbo].[sp_SearchSchedules](
	  @searchWords		nvarchar(max) = NULL
	, @channels			nvarchar(max) = NULL
	, @startDate		datetime = NULL
	, @endDate			datetime = NULL
	, @rating			nvarchar(max) = NULL
	, @searchField		nvarchar(max) = NULL
	, @searchPhrase		nvarchar(max) = NULL
	, @unwantedWords	nvarchar(max) = NULL
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
					[Title] [nvarchar](max) NULL,
					[Description] [nvarchar](max) NULL,
					[Category] [nvarchar](max) NULL,
					[AirDateUTC] [datetime2](7) NOT NULL,
					[EpisodeTitle] [nvarchar](max) NULL,
					[Duration] [int] NOT NULL,
					[Genre] [nvarchar](max) NULL,
					[Rating] [nvarchar](max) NULL,
					[Year] [int] NULL,
					[IsPremiere] [bit] NOT NULL,
					[CreatedOnUTC] [datetime2](7) NOT NULL
				)

				INSERT INTO @Schedules
				SELECT 
					  [s].[Id]
					, [s].[AfnId]
					, [s].[ChannelId]
					, [s].[Title]
					, [s].[Description]
					, [s].[Category]
					, [s].[AirDateUTC]
					, [s].[EpisodeTitle]
					, [s].[Duration]
					, [s].[Genre]
					, [s].[Rating]
					, [s].[Year]
					, [s].[IsPremiere]
					, [s].[CreatedOnUTC]
				FROM 
					[dbo].[Schedules] [s]
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
	IF @rating IS NOT NULL AND @rating <> 'ANY'
	BEGIN
		SET @sql = @sql + @tb + '-- Check Rating' + @cr
		SET @sql = @sql + @tb + 'AND ([s].[Rating] IN (SELECT [s1].[Rating] FROM [dbo].[Schedules] [s1] WHERE ((''' + @rating + ''' LIKE N'''') OR CHARINDEX(''' + @rating + ''', [s1].[Rating]) > 0))) ' + @cr
	END
	IF @channels IS NOT NULL AND LEN(RTRIM(@channels)) > 0
	BEGIN
		SET @sql = @sql + @tb + '-- Check Channels' + @cr
		SET @sql = @sql + @tb + 'AND ([s].[ChannelId] IN (' + @channels + ')) ' + @cr
	END

	-- SEARCH WORDS
	DECLARE @searchWordsSqlTitle nvarchar(max) = ''
	DECLARE @searchWordsSqlDescription nvarchar(max) = ''
	DECLARE @searchWordsSqlEpisodeTitle nvarchar(max) = ''
	DECLARE @searchWordsSqlGenre nvarchar(max) = ''
	IF @searchWords IS NOT NULL AND LEN(TRIM(@searchWords)) > 0
	BEGIN
		SET @searchWordsSqlTitle = @searchWordsSqlTitle + @tb + '-- Check Title' + @cr 
		SELECT @searchWordsSqlTitle = @searchWordsSqlTitle + @tb + '	OR ([s].[Id] IN (SELECT [s1].[Id] FROM [dbo].[Schedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[Title]) > 0)))' + @cr FROM STRING_SPLIT(@searchWords, '|')

		SET @searchWordsSqlDescription = @searchWordsSqlDescription + @tb + '-- Check Description' + @cr 
		SELECT @searchWordsSqlDescription = @searchWordsSqlDescription + @tb + '	OR ([s].[Id] IN (SELECT [s1].[Id] FROM [dbo].[Schedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[Description]) > 0)))' + @cr FROM STRING_SPLIT(@searchWords, '|')

		SET @searchWordsSqlEpisodeTitle = @searchWordsSqlEpisodeTitle + @tb + '-- Check Episode Title' + @cr 
		SELECT @searchWordsSqlEpisodeTitle = @searchWordsSqlEpisodeTitle + @tb + '	OR ([s].[Id] IN (SELECT [s1].[Id] FROM [dbo].[Schedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[EpisodeTitle]) > 0)))' + @cr FROM STRING_SPLIT(@searchWords, '|')

		SET @searchWordsSqlGenre = @searchWordsSqlGenre + @tb + '-- Check Genre' + @cr 
		SELECT @searchWordsSqlGenre = @searchWordsSqlGenre + @tb + '	OR ([s].[Id] IN (SELECT [s1].[Id] FROM [dbo].[Schedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[Genre]) > 0)))' + @cr FROM STRING_SPLIT(@searchWords, '|')
	END

	-- SEARCH PHRASES
	DECLARE @searchPhraseSqlTitle nvarchar(max) = ''
	DECLARE @searchPhraseSqlDescription nvarchar(max) = ''
	DECLARE @searchPhraseSqlEpisodeTitle nvarchar(max) = ''
	DECLARE @searchPhraseSqlGenre nvarchar(max) = ''
	IF @searchPhrase IS NOT NULL AND LEN(RTRIM(@searchPhrase)) > 0
	BEGIN
		SET @searchPhraseSqlTitle = @searchPhraseSqlTitle + @tb + '-- Check Title' + @cr 
		SET @searchPhraseSqlTitle = @searchPhraseSqlTitle + @tb + '	OR ((''' + REPLACE(@searchPhrase, '', '''') + ''' LIKE N'''') OR CHARINDEX(''' + REPLACE(REPLACE(@searchPhrase, '', ''''), '"', '') + ''', [s].[Title]) > 0)' + @cr 

		SET @searchPhraseSqlDescription = @searchPhraseSqlDescription + @tb + '-- Check Description' + @cr 
		SET @searchPhraseSqlDescription = @searchPhraseSqlDescription + @tb + '	OR ((''' + REPLACE(@searchPhrase, '', '''') + ''' LIKE N'''') OR CHARINDEX(''' + REPLACE(REPLACE(@searchPhrase, '', ''''), '"', '') + ''', [s].[Description]) > 0)' + @cr 

		SET @searchPhraseSqlEpisodeTitle = @searchPhraseSqlEpisodeTitle + @tb + '-- Check Episode Title' + @cr 
		SET @searchPhraseSqlEpisodeTitle = @searchPhraseSqlEpisodeTitle + @tb + '	OR ((''' + REPLACE(@searchPhrase, '', '''') + ''' LIKE N'''') OR CHARINDEX(''' + REPLACE(REPLACE(@searchPhrase, '', ''''), '"', '')+ ''', [s].[EpisodeTitle]) > 0)' + @cr 

		SET @searchPhraseSqlGenre = @searchPhraseSqlGenre + @tb + '-- Check Genre' + @cr 
		SET @searchPhraseSqlGenre = @searchPhraseSqlGenre + @tb + '	OR ((''' + REPLACE(@searchPhrase, '', '''') + ''' LIKE N'''') OR CHARINDEX(''' + REPLACE(REPLACE(@searchPhrase, '', ''''), '"', '') + ''', [s].[Genre]) > 0)' + @cr 
	END

	-- EXCLUDE UNWANTED WORDS
	DECLARE @unwantedWordsSqlTitle nvarchar(max) = ''
	DECLARE @unwantedWordsSqlDescription nvarchar(max) = ''
	DECLARE @unwantedWordsSqlEpisodeTitle nvarchar(max) = ''
	DECLARE @unwantedWordsSqlGenre nvarchar(max) = ''
	IF @unwantedWords IS NOT NULL AND LEN(TRIM(@unwantedWords)) > 0
	BEGIN
		SET @unwantedWordsSqlTitle = @unwantedWordsSqlTitle + @tb + 'AND (1=1 ' + @cr
		SET @unwantedWordsSqlTitle = @unwantedWordsSqlTitle + @tb + '-- Check Title' + @cr 
		SELECT @unwantedWordsSqlTitle = @unwantedWordsSqlTitle + @tb + '	AND ([s].[Id] NOT IN (SELECT [s1].[Id] FROM [dbo].[Schedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[Title]) > 0)))' + @cr FROM STRING_SPLIT(@unwantedWords, '|')
		SET @unwantedWordsSqlTitle = @unwantedWordsSqlTitle + @tb + ')' + @cr

		SET @unwantedWordsSqlDescription = @unwantedWordsSqlDescription + @tb + 'AND (1=1 ' + @cr
		SET @unwantedWordsSqlDescription = @unwantedWordsSqlDescription + @tb + '-- Check Description' + @cr 
		SELECT @unwantedWordsSqlDescription = @unwantedWordsSqlDescription + @tb + '	AND ([s].[Id] NOT IN (SELECT [s1].[Id] FROM [dbo].[Schedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[Description]) > 0)))' + @cr FROM STRING_SPLIT(@unwantedWords, '|')
		SET @unwantedWordsSqlDescription = @unwantedWordsSqlDescription + @tb + ')' + @cr

		SET @unwantedWordsSqlEpisodeTitle = @unwantedWordsSqlEpisodeTitle + @tb + 'AND (1=1 ' + @cr
		SET @unwantedWordsSqlEpisodeTitle = @unwantedWordsSqlEpisodeTitle + @tb + '-- Check Episode Title' + @cr 
		SELECT @unwantedWordsSqlEpisodeTitle = @unwantedWordsSqlEpisodeTitle + @tb + '	AND ([s].[Id] NOT IN (SELECT [s1].[Id] FROM [dbo].[Schedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[EpisodeTitle]) > 0)))' + @cr FROM STRING_SPLIT(@unwantedWords, '|')
		SET @unwantedWordsSqlEpisodeTitle = @unwantedWordsSqlEpisodeTitle + @tb + ')' + @cr

		SET @unwantedWordsSqlGenre = @unwantedWordsSqlGenre + @tb + '-- Check Genre' + @cr 
		SET @unwantedWordsSqlGenre = @unwantedWordsSqlGenre + @tb + 'AND (1=1 ' + @cr
		SELECT @unwantedWordsSqlGenre = @unwantedWordsSqlGenre + @tb + '	AND ([s].[Id] NOT IN (SELECT [s1].[Id] FROM [dbo].[Schedules] [s1] WHERE ((''' + value + ''' LIKE N'''') OR CHARINDEX(''' + value + ''', [s1].[Genre]) > 0)))' + @cr FROM STRING_SPLIT(@unwantedWords, '|')
		SET @unwantedWordsSqlGenre = @unwantedWordsSqlGenre + @tb + ')' + @cr
	END

	IF (@searchField IS NULL) OR (@searchField = '') OR @searchField = 'ALL'
	BEGIN
		SET @sql = @sql + @tb + '-- Search ALL' + @cr
		IF @searchWords IS NOT NULL AND LEN(TRIM(@searchWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Words' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchWordsSqlTitle
			SET @sql = @sql + @searchWordsSqlDescription
			SET @sql = @sql + @searchWordsSqlEpisodeTitle
			SET @sql = @sql + @searchWordsSqlGenre
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @searchPhrase IS NOT NULL AND LEN(RTRIM(@searchPhrase)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Phrase' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchPhraseSqlTitle
			SET @sql = @sql + @searchPhraseSqlDescription
			SET @sql = @sql + @searchPhraseSqlEpisodeTitle
			SET @sql = @sql + @searchPhraseSqlGenre
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @unwantedWords IS NOT NULL AND LEN(TRIM(@unwantedWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Unwanted Words' + @cr
			SET @sql = @sql + @unwantedWordsSqlTitle
			SET @sql = @sql + @unwantedWordsSqlDescription
			SET @sql = @sql + @unwantedWordsSqlEpisodeTitle
			SET @sql = @sql + @unwantedWordsSqlGenre
		END
	END

	IF @searchField = 'TITLE'
	BEGIN
		SET @sql = @sql + @tb + '-- Search ALL' + @cr
		IF @searchWords IS NOT NULL AND LEN(TRIM(@searchWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Words' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchWordsSqlTitle
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @searchPhrase IS NOT NULL AND LEN(RTRIM(@searchPhrase)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Phrase' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchPhraseSqlTitle
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @unwantedWords IS NOT NULL AND LEN(TRIM(@unwantedWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Unwanted Words' + @cr
			SET @sql = @sql + @unwantedWordsSqlTitle
		END
	END

	IF @searchField = 'DESCRIPTION'
	BEGIN
		SET @sql = @sql + @tb + '-- Search DESCRIPTION' + @cr
		IF @searchWords IS NOT NULL AND LEN(TRIM(@searchWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Words' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchWordsSqlDescription
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @searchPhrase IS NOT NULL AND LEN(RTRIM(@searchPhrase)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Phrase' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchPhraseSqlDescription
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @unwantedWords IS NOT NULL AND LEN(TRIM(@unwantedWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Unwanted Words' + @cr
			SET @sql = @sql + @unwantedWordsSqlDescription
		END
	END

	IF @searchField = 'EPISODE_TITLE'
	BEGIN
		SET @sql = @sql + @tb + '-- Search EPISODE_TITLE' + @cr
		IF @searchWords IS NOT NULL AND LEN(TRIM(@searchWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Words' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchWordsSqlEpisodeTitle
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @searchPhrase IS NOT NULL AND LEN(RTRIM(@searchPhrase)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Phrase' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchPhraseSqlEpisodeTitle
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @unwantedWords IS NOT NULL AND LEN(TRIM(@unwantedWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Unwanted Words' + @cr
			SET @sql = @sql + @unwantedWordsSqlEpisodeTitle
		END
	END

	IF @searchField = 'GENRE'
	BEGIN
		SET @sql = @sql + @tb + '-- Search GENRE' + @cr
		IF @searchWords IS NOT NULL AND LEN(TRIM(@searchWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Words' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchWordsSqlGenre
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @searchPhrase IS NOT NULL AND LEN(RTRIM(@searchPhrase)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Search Phrase' + @cr
			SET @sql = @sql + @tb + 'AND (1=0 ' + @cr
			SET @sql = @sql + @searchPhraseSqlGenre
			SET @sql = @sql + @tb + ')' + @cr
		END

		IF @unwantedWords IS NOT NULL AND LEN(TRIM(@unwantedWords)) > 0
		BEGIN
			SET @sql = @sql + @tb + '-- Unwanted Words' + @cr
			SET @sql = @sql + @unwantedWordsSqlGenre
		END
	END

	SET @sql = @sql + @cr
	SET @sql = @sql + N'SELECT COUNT(*) AS ''TotalCount'' FROM @Schedules' + @cr
	SET @sql = @sql + @cr
	SET @sql = @sql + N'SELECT * FROM @Schedules' + @cr
	SET @sql = @sql + N'	ORDER BY [AirDateUTC] ASC' + @cr

	exec LongPrint @sql

	exec sp_executesql @sql
END
