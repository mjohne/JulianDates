namespace Planetoid_DB.Helpers;

/// <summary>Provides calculations and conversions for Julian dates and related date and time systems.</summary>
/// <remarks>Supports Julian Date, Modified Julian Date, Reduced Julian Date, and other related formats, including conversions to and from civil calendar dates and Unix time. Chronological date calculations use the local time zone.</remarks>
internal static class JulianDates
{
	#region consonants

	/// <summary>The Julian date corresponding to the OLE Automation date epoch.</summary>
	/// <remarks>Used as the base coefficient for converting OLE Automation dates to astronomical Julian dates.</remarks>
	private const double DoubleJulianDateCoefficient = 2415018.5;

	/// <summary>The number of days to add to a Modified Julian Date to obtain a Julian Date.</summary>
	/// <remarks>Represents the offset between Julian Date and Modified Julian Date (MJD = JD - 2400000.5).</remarks>
	private const double DoubleModifiedJulianDateCoefficient = 2400000.5;

	/// <summary>Coefficient used to calculate the reduced Julian Date from a Julian Date.</summary>
	/// <remarks>Represents the offset for Reduced Julian Date (RJD = JD - 2400000).</remarks>
	private const double DoubleReducedJulianDateCoefficient = 2400000;

	/// <summary>Offset used to convert a truncated Julian date to a Julian date.</summary>
	/// <remarks>Represents the offset for Truncated Julian Date introduced by NASA.</remarks>
	private const double DoubleTruncatedJulianDateCoefficient = 2440000.5;

	/// <summary>Julian Date offset used to calculate the Dublin Julian Date.</summary>
	/// <remarks>Corresponds to the epoch of 1899-12-31 12:00 UTC used in Dublin Julian Date calculations.</remarks>
	private const double DoubleDublinJulianDateCoefficient = 2415020;

	/// <summary>Julian date of the CNES reference epoch (JD 2,433,282.5).</summary>
	/// <remarks>Corresponds to the reference epoch used by the Centre National d'Études Spatiales (1950-01-01 00:00:00 UTC).</remarks>
	private const double DoubleCnesJulianDateCoefficient = 2433282.5;

	/// <summary>The Julian date offset used for CCSDS date conversions.</summary>
	/// <remarks>Corresponds to the epoch defined by the Consultative Committee for Space Data Systems (1958-01-01 00:00:00 UTC).</remarks>
	private const double DoubleCcsdsJulianDateCoefficient = 2436204.5;

	/// <summary>Julian date coefficient used by the double-precision LOP calculation.</summary>
	/// <remarks>Used for Lunar Orbiter / LOP reference epoch date calculations.</remarks>
	private const double DoubleLopJulianDateCoefficient = 2448622.5;

	/// <summary>Julian Date value corresponding to 2000-01-01 00:00.</summary>
	/// <remarks>Represents the J2000 epoch reference date.</remarks>
	private const double DoubleMillenniumJulianDateCoefficient = 2451544.5;

	/// <summary>The Julian date offset used to calculate Lilian dates.</summary>
	/// <remarks>Lilian day 1 corresponds to October 15, 1582.</remarks>
	private const double DoubleLilianDateCoefficient = 2299159.5;

	/// <summary>The offset used to convert a Rata Die day number to a Julian Date.</summary>
	/// <remarks>Rata Die day 1 corresponds to January 1, AD 1 in the proleptic Gregorian calendar.</remarks>
	private const double DoubleRataDieCoefficient = 1721424.5;

	/// <summary>Coefficient used to calculate the double-precision Mars Sol Date.</summary>
	/// <remarks>Used as the baseline offset for calculating Mars Sol Date (MSD).</remarks>
	private const double DoubleMarsSolDateCoefficient = 2405522;

	/// <summary>The ratio of Mars's rotation period to Earth's.</summary>
	/// <remarks>Approximately 1.02749, accounting for the longer length of a Martian sol compared to an Earth day.</remarks>
	private const double DoubleRatioRotationAxisEarthMars = 1.02749;

	/// <summary>Julian date corresponding to the Unix epoch (1970-01-01 00:00:00 UTC).</summary>
	/// <remarks>Used for converting between Julian Date and Unix time in seconds.</remarks>
	private const double DoubleUnixTimeCoefficient = 2440587.5;

	/// <summary>The number of seconds in a day.</summary>
	/// <remarks>Standard constant representing 24 hours * 60 minutes * 60 seconds.</remarks>
	private const int SecondsOfDay = 86400;

	/// <summary>The Julian date corresponding to the GPS epoch (1980-01-06 00:00:00 UTC).</summary>
	/// <remarks>Used as the reference baseline for calculating GPS Week Numbers and Time of Week (TOW).</remarks>
	private const double DoubleGpsEpochJulianDateCoefficient = 2444244.5;

	/// <summary>Julian date corresponding to the J2000.0 astronomical epoch.</summary>
	/// <remarks>Represents 2000-01-01 12:00:00 TT (Terrestrial Time), the standard astronomical reference epoch.</remarks>
	private const double DoubleJ2000EpochJulianDateCoefficient = 2451545.0;

	/// <summary>Julian date corresponding to the B1900.0 Besselian epoch.</summary>
	/// <remarks>Represents 1900-00-00 12:00:00 UT, the historical reference epoch for stellar coordinates.</remarks>
	private const double DoubleB1900EpochJulianDateCoefficient = 2415020.31352;

	/// <summary>The length of a tropical year in days used for Besselian epoch conversions.</summary>
	/// <remarks>Standard astronomical constant for fractional year calculation in the Besselian system.</remarks>
	private const double DoubleTropicalYearDays = 365.242198781;

	/// <summary>Speed of light in astronomical units per day (AU/day).</summary>
	/// <remarks>Used for computing light travel time corrections in Heliocentric and Barycentric Julian Dates.</remarks>
	private const double DoubleSpeedOfLightAuPerDay = 173.144632684;

	#endregion

	#region JD Calculators

	/// <summary>Calculates the current Julian Date using the current system time in UTC. The calculation is based on converting the current UTC time to an OLE Automation Date (OADate) and then adding the Julian Date coefficient to obtain the astronomical Julian Date.</summary>
	/// <remarks>Uses system UTC time at execution time.</remarks>
	/// <returns>Julian Date for the current UTC time</returns>
	public static double CalculateJulianDate()
	{
		return DateTime.Now.ToUniversalTime().ToOADate() + DoubleJulianDateCoefficient;
	}

	/// <summary>Calculates the Julian Date for a given date and time. The calculation converts the provided DateTime to an OLE Automation Date (OADate) and adds the Julian Date coefficient to obtain the astronomical Julian Date.</summary>
	/// <remarks>The provided date parameter should specify timezone information appropriately.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Julian Date corresponding to the specified date and time</returns>
	public static double CalculateJulianDate(DateTime date)
	{
		return date.ToOADate() + DoubleJulianDateCoefficient;
	}

	/// <summary>Calculates the current Modified Julian Date (MJD) using the current system time in UTC. The result is obtained by subtracting the Modified Julian Date coefficient from the current Julian Date.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Modified Julian Date for the current UTC time</returns>
	public static double CalculateModifiedJulianDate()
	{
		return CalculateJulianDate() - DoubleModifiedJulianDateCoefficient;
	}

	/// <summary>Calculates the Modified Julian Date (MJD) for a given date and time. The result is obtained by subtracting the Modified Julian Date coefficient from the Julian Date of the specified date.</summary>
	/// <remarks>Converts input date to JD before computing MJD.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Modified Julian Date corresponding to the specified date and time</returns>
	public static double CalculateModifiedJulianDate(DateTime date)
	{
		return CalculateJulianDate(date: date) - DoubleModifiedJulianDateCoefficient;
	}

	/// <summary>Calculates the Modified Julian Date (MJD) from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Modified Julian Date</returns>
	public static double CalculateModifiedJulianDate(double julianDate)
	{
		return julianDate - DoubleModifiedJulianDateCoefficient;
	}

	/// <summary>Calculates the current Reduced Julian Date (RJD) using the current system time in UTC. The result is obtained by subtracting the Reduced Julian Date coefficient from the current Julian Date.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Reduced Julian Date for the current UTC time</returns>
	public static double CalculateReducedJulianDate()
	{
		return CalculateJulianDate() - DoubleReducedJulianDateCoefficient;
	}

	/// <summary>Calculates the Reduced Julian Date (RJD) for a given date and time. The result is obtained by subtracting the Reduced Julian Date coefficient from the Julian Date of the specified date.</summary>
	/// <remarks>Converts input date to JD before computing RJD.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Reduced Julian Date corresponding to the specified date and time</returns>
	public static double CalculateReducedJulianDate(DateTime date)
	{
		return CalculateJulianDate(date: date) - DoubleReducedJulianDateCoefficient;
	}

	/// <summary>Calculates the Reduced Julian Date (RJD) from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Reduced Julian Date</returns>
	public static double CalculateReducedJulianDate(double julianDate)
	{
		return julianDate - DoubleReducedJulianDateCoefficient;
	}

	/// <summary>Calculates the current Truncated Julian Date (TJD) using the current system time in UTC. The result is the floored value of the current Julian Date minus the Truncated Julian Date coefficient.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Truncated Julian Date for the current UTC time</returns>
	public static double CalculateTruncatedJulianDate()
	{
		return Math.Floor(d: CalculateJulianDate() - DoubleTruncatedJulianDateCoefficient);
	}

	/// <summary>Calculates the Truncated Julian Date (TJD) for a given date and time. The result is the floored value of the Julian Date of the specified date minus the Truncated Julian Date coefficient.</summary>
	/// <remarks>Converts input date to JD before computing TJD.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Truncated Julian Date corresponding to the specified date and time</returns>
	public static double CalculateTruncatedJulianDate(DateTime date)
	{
		return Math.Floor(d: CalculateJulianDate(date: date) - DoubleTruncatedJulianDateCoefficient);
	}

	/// <summary>Calculates the Truncated Julian Date (TJD) from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Truncated Julian Date</returns>
	public static double CalculateTruncatedJulianDate(double julianDate)
	{
		return Math.Floor(d: julianDate - DoubleTruncatedJulianDateCoefficient);
	}

	/// <summary>Calculates the current Dublin Julian Date (DJD) using the current system time in UTC. The result is obtained by subtracting the Dublin Julian Date coefficient from the current Julian Date.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Dublin Julian Date for the current UTC time</returns>
	public static double CalculateDublinJulianDate()
	{
		return CalculateJulianDate() - DoubleDublinJulianDateCoefficient;
	}

	/// <summary>Calculates the Dublin Julian Date (DJD) for a given date and time. The result is obtained by subtracting the Dublin Julian Date coefficient from the Julian Date of the specified date.</summary>
	/// <remarks>Converts input date to JD before computing DJD.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Dublin Julian Date corresponding to the specified date and time</returns>
	public static double CalculateDublinJulianDate(DateTime date)
	{
		return CalculateJulianDate(date: date) - DoubleDublinJulianDateCoefficient;
	}

	/// <summary>Calculates the Dublin Julian Date (DJD) from a given Julian Date. The result is obtained by subtracting the Dublin Julian Date coefficient from the specified Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Dublin Julian Date</returns>
	public static double CalculateDublinJulianDate(double julianDate)
	{
		return julianDate - DoubleDublinJulianDateCoefficient;
	}

	/// <summary>Calculates the current CNES Julian Date using the current system time in UTC. The result is obtained by subtracting the CNES Julian Date coefficient from the current Julian Date.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>CNES Julian Date for the current UTC time</returns>
	public static double CalculateCnesJulianDate()
	{
		return CalculateJulianDate() - DoubleCnesJulianDateCoefficient;
	}

	/// <summary>Calculates the CNES Julian Date for a given date and time. The result is obtained by subtracting the CNES Julian Date coefficient from the Julian Date of the specified date.</summary>
	/// <remarks>Converts input date to JD before computing CNES Julian Date.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>CNES Julian Date corresponding to the specified date and time</returns>
	public static double CalculateCnesJulianDate(DateTime date)
	{
		return CalculateJulianDate(date: date) - DoubleCnesJulianDateCoefficient;
	}

	/// <summary>Calculates the CNES Julian Date from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>CNES Julian Date</returns>
	public static double CalculateCnesJulianDate(double julianDate)
	{
		return julianDate - DoubleCnesJulianDateCoefficient;
	}

	/// <summary>Calculates the current CCSDS Julian Date using the current system time in UTC. The result is obtained by subtracting the CCSDS Julian Date coefficient from the current Julian Date.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>CCSDS Julian Date for the current UTC time</returns>
	public static double CalculateCcsdsJulianDate()
	{
		return CalculateJulianDate() - DoubleCcsdsJulianDateCoefficient;
	}

	/// <summary>Calculates the CCSDS Julian Date for a given date and time. The result is obtained by subtracting the CCSDS Julian Date coefficient from the Julian Date of the specified date.</summary>
	/// <remarks>Converts input date to JD before computing CCSDS Julian Date.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>CCSDS Julian Date corresponding to the specified date and time</returns>
	public static double CalculateCcsdsJulianDate(DateTime date)
	{
		return CalculateJulianDate(date: date) - DoubleCcsdsJulianDateCoefficient;
	}

	/// <summary>Calculates the CCSDS Julian Date from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>CCSDS Julian Date</returns>
	public static double CalculateCcsdsJulianDate(double julianDate)
	{
		return julianDate - DoubleCcsdsJulianDateCoefficient;
	}

	/// <summary>Calculates the current LOP Julian Date using the current system time in UTC. The result is obtained by subtracting the LOP Julian Date coefficient from the current Julian Date.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>LOP Julian Date for the current UTC time</returns>
	public static double CalculateLopJulianDate()
	{
		return CalculateJulianDate() - DoubleLopJulianDateCoefficient;
	}

	/// <summary>Calculates the LOP Julian Date for a given date and time. The result is obtained by subtracting the LOP Julian Date coefficient from the Julian Date of the specified date.</summary>
	/// <remarks>Converts input date to JD before computing LOP Julian Date.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>LOP Julian Date corresponding to the specified date and time</returns>
	public static double CalculateLopJulianDate(DateTime date)
	{
		return CalculateJulianDate(date: date) - DoubleLopJulianDateCoefficient;
	}

	/// <summary>Calculates the LOP Julian Date from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>LOP Julian Date</returns>
	public static double CalculateLopJulianDate(double julianDate)
	{
		return julianDate - DoubleLopJulianDateCoefficient;
	}

	/// <summary>Calculates the current Millennium Julian Date using the current system time in UTC. The result is obtained by subtracting the Millennium Julian Date coefficient from the current Julian Date.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Millennium Julian Date for the current UTC time</returns>
	public static double CalculateMillenniumJulianDate()
	{
		return CalculateJulianDate() - DoubleMillenniumJulianDateCoefficient;
	}

	/// <summary>Calculates the Millennium Julian Date for a given date and time. The result is obtained by subtracting the Millennium Julian Date coefficient from the Julian Date of the specified date.</summary>
	/// <remarks>Converts input date to JD before computing Millennium Julian Date.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Millennium Julian Date corresponding to the specified date and time</returns>
	public static double CalculateMillenniumJulianDate(DateTime date)
	{
		return CalculateJulianDate(date: date) - DoubleMillenniumJulianDateCoefficient;
	}

	/// <summary>Calculates the Millennium Julian Date from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Millennium Julian Date</returns>
	public static double CalculateMillenniumJulianDate(double julianDate)
	{
		return julianDate - DoubleMillenniumJulianDateCoefficient;
	}

	/// <summary>Calculates the current Chronological Julian Date using the current system time in the local time zone. The result is the Julian Date plus 0.5 and the local daylight saving time offset (in days).</summary>
	/// <remarks>Uses local timezone daylight saving rules for the offset.</remarks>
	/// <returns>Chronological Julian Date for the current local time</returns>
	public static double CalculateChronologicalJulianDate()
	{
		TimeZoneInfo zone = TimeZoneInfo.Local;
		TimeSpan time = zone.GetAdjustmentRules().FirstOrDefault()?.DaylightDelta ?? TimeSpan.Zero;
		return CalculateJulianDate() + 0.5 + (time.TotalHours / 24.0);
	}

	/// <summary>Calculates the Chronological Julian Date for a given date and time in the local time zone. The result is the Julian Date of the specified date plus 0.5 and the local daylight saving time offset (in days).</summary>
	/// <remarks>Uses local timezone daylight saving rules for the offset.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Chronological Julian Date corresponding to the specified date and time</returns>
	public static double CalculateChronologicalJulianDate(DateTime date)
	{
		TimeZoneInfo zone = TimeZoneInfo.Local;
		TimeSpan time = zone.GetAdjustmentRules().FirstOrDefault()?.DaylightDelta ?? TimeSpan.Zero;
		return CalculateJulianDate(date: date) + 0.5 + (time.TotalHours / 24.0);
	}

	/// <summary>Calculates the Chronological Julian Date from a given Julian Date in the local time zone. The result is the specified Julian Date plus 0.5 and the local daylight saving time offset (in days).</summary>
	/// <remarks>Uses local timezone daylight saving rules for the offset.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Chronological Julian Date</returns>
	public static double CalculateChronologicalJulianDate(double julianDate)
	{
		TimeZoneInfo zone = TimeZoneInfo.Local;
		TimeSpan time = zone.GetAdjustmentRules().FirstOrDefault()?.DaylightDelta ?? TimeSpan.Zero;
		return julianDate + 0.5 + (time.TotalHours / 24.0);
	}

	/// <summary>Calculates the current Chronological Modified Julian Date using the current system time in the local time zone. The result is the Modified Julian Date plus 0.5 and the local daylight saving time offset (in days).</summary>
	/// <remarks>Uses local timezone daylight saving rules for the offset.</remarks>
	/// <returns>Chronological Modified Julian Date for the current local time</returns>
	public static double CalculateChronologicalModifiedJulianDate()
	{
		TimeZoneInfo zone = TimeZoneInfo.Local;
		TimeSpan time = zone.GetAdjustmentRules().FirstOrDefault()?.DaylightDelta ?? TimeSpan.Zero;
		return CalculateJulianDate() - DoubleModifiedJulianDateCoefficient + 0.5 + (time.TotalHours / 24.0);
	}

	/// <summary>Calculates the Chronological Modified Julian Date for a given date and time in the local time zone. The result is the Modified Julian Date of the specified date plus 0.5 and the local daylight saving time offset (in days).</summary>
	/// <remarks>Uses local timezone daylight saving rules for the offset.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Chronological Modified Julian Date corresponding to the specified date and time</returns>
	public static double CalculateChronologicalModifiedJulianDate(DateTime date)
	{
		TimeZoneInfo zone = TimeZoneInfo.Local;
		TimeSpan time = zone.GetAdjustmentRules().FirstOrDefault()?.DaylightDelta ?? TimeSpan.Zero;
		return CalculateJulianDate(date: date) - DoubleModifiedJulianDateCoefficient + 0.5 + (time.TotalHours / 24.0);
	}

	/// <summary>Calculates the Chronological Modified Julian Date from a given Julian Date in the local time zone. The result is the specified Julian Date minus the Modified Julian Date coefficient, plus 0.5 and the local daylight saving time offset (in days).</summary>
	/// <remarks>Uses local timezone daylight saving rules for the offset.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Chronological Modified Julian Date</returns>
	public static double CalculateChronologicalModifiedJulianDate(double julianDate)
	{
		TimeZoneInfo zone = TimeZoneInfo.Local;
		TimeSpan time = zone.GetAdjustmentRules().FirstOrDefault()?.DaylightDelta ?? TimeSpan.Zero;
		return julianDate - DoubleModifiedJulianDateCoefficient + 0.5 + (time.TotalHours / 24.0);
	}

	/// <summary>Calculates the current Lilian Date using the current system time in UTC. The result is the floored value of the current Julian Date minus the Lilian Date coefficient.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Lilian Date for the current UTC time</returns>
	public static double CalculateLilianDate()
	{
		return Math.Floor(d: CalculateJulianDate() - DoubleLilianDateCoefficient);
	}

	/// <summary>Calculates the Lilian Date for a given date and time. The result is the floored value of the Julian Date of the specified date minus the Lilian Date coefficient.</summary>
	/// <remarks>Converts input date to JD before computing Lilian Date.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Lilian Date corresponding to the specified date and time</returns>
	public static double CalculateLilianDate(DateTime date)
	{
		return Math.Floor(d: CalculateJulianDate(date: date) - DoubleLilianDateCoefficient);
	}

	/// <summary>Calculates the Lilian Date from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Lilian Date</returns>
	public static double CalculateLilianDate(double julianDate)
	{
		return Math.Floor(d: julianDate - DoubleLilianDateCoefficient);
	}

	/// <summary>Calculates the current Rata Die using the current system time in UTC. The result is the floored value of the current Julian Date minus the Rata Die coefficient.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Rata Die for the current UTC time</returns>
	public static double CalculateRataDie()
	{
		return Math.Floor(d: CalculateJulianDate() - DoubleRataDieCoefficient);
	}

	/// <summary>Calculates the Rata Die for a given date and time. The result is the floored value of the Julian Date of the specified date minus the Rata Die coefficient.</summary>
	/// <remarks>Converts input date to JD before computing Rata Die.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Rata Die corresponding to the specified date and time</returns>
	public static double CalculateRataDie(DateTime date)
	{
		return Math.Floor(d: CalculateJulianDate(date: date) - DoubleRataDieCoefficient);
	}

	/// <summary>Calculates the Rata Die from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Rata Die</returns>
	public static double CalculateRataDie(double julianDate)
	{
		return Math.Floor(d: julianDate - DoubleRataDieCoefficient);
	}

	/// <summary>Calculates the current Mars Sol Date using the current system time in UTC. The result is obtained by subtracting the Mars Sol Date coefficient from the current Julian Date and dividing by the Earth-Mars rotation ratio.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Mars Sol Date for the current UTC time</returns>
	public static double CalculateMarsSolDate()
	{
		return (CalculateJulianDate() - DoubleMarsSolDateCoefficient) / DoubleRatioRotationAxisEarthMars;
	}

	/// <summary>Calculates the Mars Sol Date for a given date and time. The result is obtained by subtracting the Mars Sol Date coefficient from the Julian Date of the specified date and dividing by the Earth-Mars rotation ratio.</summary>
	/// <remarks>Converts input date to JD before computing Mars Sol Date.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Mars Sol Date corresponding to the specified date and time</returns>
	public static double CalculateMarsSolDate(DateTime date)
	{
		return (CalculateJulianDate(date: date) - DoubleMarsSolDateCoefficient) / DoubleRatioRotationAxisEarthMars;
	}

	/// <summary>Calculates the Mars Sol Date from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Mars Sol Date</returns>
	public static double CalculateMarsSolDate(double julianDate)
	{
		return (julianDate - DoubleMarsSolDateCoefficient) / DoubleRatioRotationAxisEarthMars;
	}

	/// <summary>
	/// Calculates the current Unix Time (seconds since 1970-01-01) using the current system time in UTC. The result is obtained by subtracting the Unix Time coefficient from the current Julian Date and multiplying by the number of seconds in a day.</summary>
	/// <remarks>Calculated relative to current system UTC time.</remarks>
	/// <returns>Unix Time for the current UTC time</returns>
	public static double CalculateUnixTime()
	{
		return (CalculateJulianDate() - DoubleUnixTimeCoefficient) * SecondsOfDay;
	}

	/// <summary>Calculates the Unix Time (seconds since 1970-01-01) for a given date and time. The result is obtained by subtracting the Unix Time coefficient from the Julian Date of the specified date and multiplying by the number of seconds in a day.</summary>
	/// <remarks>Converts input date to JD before computing Unix time.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Unix Time corresponding to the specified date and time</returns>
	public static double CalculateUnixTime(DateTime date)
	{
		return (CalculateJulianDate(date: date) - DoubleUnixTimeCoefficient) * SecondsOfDay;
	}

	/// <summary>Calculates the Unix Time (seconds since 1970-01-01) from a given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Unix Time</returns>
	public static double CalculateUnixTime(double julianDate)
	{
		return (julianDate - DoubleUnixTimeCoefficient) * SecondsOfDay;
	}

	/// <summary>Calculates the Julian Day Number (JDN) from a given Julian Date.</summary>
	/// <remarks>JDN represents the integer day number without fractional time, starting at noon UTC.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Integer Julian Day Number</returns>
	public static long CalculateJulianDayNumber(double julianDate)
	{
		return (long)Math.Floor(d: julianDate + 0.5);
	}

	/// <summary>Calculates the NASA Truncated Julian Date (NASA-TJD) with modulo 10000 wrap-around.</summary>
	/// <remarks>Defined by NASA as JD - 2440000.5 mod 10000, commonly used in satellite telemetry.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Truncated Julian Date modulo 10000</returns>
	public static double CalculateNasaTruncatedJulianDate(double julianDate)
	{
		return (julianDate - 2440000.5) % 10000.0;
	}

	/// <summary>Calculates the GPS Week Number from a given Julian Date.</summary>
	/// <remarks>Counts the number of full 7-day weeks elapsed since the GPS epoch (January 6, 1980).</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>GPS Week Number</returns>
	public static int CalculateGpsWeek(double julianDate)
	{
		return (int)Math.Floor(d: (julianDate - DoubleGpsEpochJulianDateCoefficient) / 7.0);
	}

	/// <summary>Calculates the GPS Time of Week (TOW) in seconds from a given Julian Date.</summary>
	/// <remarks>Represents seconds elapsed within the current GPS week (0 to 604,799 seconds).</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Time of Week in seconds</returns>
	public static double CalculateGpsTimeOfWeek(double julianDate)
	{
		double daysSinceEpoch = julianDate - DoubleGpsEpochJulianDateCoefficient;
		double daysInWeek = daysSinceEpoch - (Math.Floor(d: daysSinceEpoch / 7.0) * 7.0);
		return daysInWeek * 86400.0;
	}

	/// <summary>Calculates the Julian Epoch (e.g., J2000.0 format) from a given Julian Date.</summary>
	/// <remarks>Expresses time as fractional astronomical years of exactly 365.25 days relative to J2000.0.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Julian Epoch year (e.g., 2024.5)</returns>
	public static double CalculateJulianEpoch(double julianDate)
	{
		return 2000.0 + ((julianDate - DoubleJ2000EpochJulianDateCoefficient) / 365.25);
	}

	/// <summary>Calculates the Besselian Epoch (e.g., B1950.0 format) from a given Julian Date.</summary>
	/// <remarks>Expresses time in tropical years relative to the B1900.0 epoch.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Besselian Epoch year</returns>
	public static double CalculateBesselianEpoch(double julianDate)
	{
		return 1900.0 + ((julianDate - DoubleB1900EpochJulianDateCoefficient) / DoubleTropicalYearDays);
	}

	/// <summary>Calculates the Heliocentric Julian Date (HJD) by applying a geometric light-travel time correction.</summary>
	/// <remarks>Corrects light arrival time from the Earth to the Sun's center. Vector components (earthVectorX/Y/Z) and unit direction (targetUnitX/Y/Z) must be in Astronomical Units (AU).</remarks>
	/// <param name="julianDate">Geocentric Julian Date at observation time</param>
	/// <param name="earthVectorX">Heliocentric X coordinate of Earth (AU)</param>
	/// <param name="earthVectorY">Heliocentric Y coordinate of Earth (AU)</param>
	/// <param name="earthVectorZ">Heliocentric Z coordinate of Earth (AU)</param>
	/// <param name="targetUnitX">Unit vector X pointing towards the target object</param>
	/// <param name="targetUnitY">Unit vector Y pointing towards the target object</param>
	/// <param name="targetUnitZ">Unit vector Z pointing towards the target object</param>
	/// <returns>Heliocentric Julian Date (HJD)</returns>
	public static double CalculateHeliocentricJulianDate(double julianDate, double earthVectorX, double earthVectorY, double earthVectorZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		// Scalar dot product of Earth position vector and target direction vector
		double dotProduct = (earthVectorX * targetUnitX) + (earthVectorY * targetUnitY) + (earthVectorZ * targetUnitZ);
		// Light travel time delay in days
		double timeCorrectionDays = dotProduct / DoubleSpeedOfLightAuPerDay;
		return julianDate + timeCorrectionDays;
	}

	#endregion

	#region JD Converters

	/// <summary>Converts a Julian Date to a civil calendar date and time (DateTime).</summary>
	/// <remarks>Implements Fliegel-Van Flandern / Meeus algorithm for Gregorian and Julian calendar conversions.</remarks>
	/// <param name="julianDate">The Julian Date to convert.</param>
	/// <returns>DateTime representing the corresponding civil calendar date and time.</returns>
	public static DateTime ConvertJulianDateToCivilCalendar(double julianDate)
	{
		// Add 0.5 to the Julian date to align with the start of the civil day
		double j = julianDate + 0.5;
		// Separate the integer and fractional parts
		int z = (int)j;
		double f = j - z;
		// Gregorian calendar correction
		int a;
		if (z < 2299161)
		{
			// Julian calendar
			a = z;
		}
		else
		{
			// Gregorian calendar
			int alpha = (int)((z - 1867216.25) / 36524.25);
			a = z + 1 + alpha - (alpha / 4);
		}
		// Calculate intermediate values for year, month, and day
		int b = a + 1524;
		int c = (int)((b - 122.1) / 365.25);
		int d = (int)(365.25 * c);
		int e = (int)((b - d) / 30.6001);
		// Calculate day, month, and year
		int day = b - d - (int)(30.6001 * e);
		int month = (e < 14) ? e - 1 : e - 13;
		int year = (month > 2) ? c - 4716 : c - 4715;
		// Efficiently calculate the time part (hour, minute, second) from the fractional day
		double totalSeconds = f * 86400.0;
		int hour = (int)(totalSeconds / 3600);
		int minute = (int)(totalSeconds % 3600 / 60);
		int second = (int)(totalSeconds % 60);
		// Construct and return the DateTime object
		return new DateTime(year: year, month: month, day: day, hour: hour, minute: minute, second: second);
	}

	/// <summary>Converts a Julian Date to the Modified Julian Date (MJD). The result is obtained by subtracting the Modified Julian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Modified Julian Date</returns>
	public static double ConvertJulianDateToModifiedJulianDate(double julianDate)
	{
		return julianDate - DoubleModifiedJulianDateCoefficient;
	}

	/// <summary>Converts a Modified Julian Date (MJD) to the Julian Date. The result is obtained by adding the Modified Julian Date coefficient to the given Modified Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Modified Julian Date.</remarks>
	/// <param name="julianModifiedDate">Modified Julian Date</param>
	/// <returns>Julian Date</returns>
	public static double ConvertModifiedJulianDateToJulianDate(double julianModifiedDate)
	{
		return julianModifiedDate + DoubleModifiedJulianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the Reduced Julian Date (RJD). The result is obtained by subtracting the Reduced Julian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Reduced Julian Date</returns>
	public static double ConvertJulianDateToReducedJulianDate(double julianDate)
	{
		return julianDate - DoubleReducedJulianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the Truncated Julian Date (TJD). The result is obtained by subtracting the Truncated Julian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Truncated Julian Date</returns>
	public static double ConvertJulianDateToTruncatedJulianDate(double julianDate)
	{
		return julianDate - DoubleTruncatedJulianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the Dublin Julian Date (DJD). The result is obtained by subtracting the Dublin Julian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Dublin Julian Date</returns>
	public static double ConvertJulianDateToDublinJulianDate(double julianDate)
	{
		return julianDate - DoubleDublinJulianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the CNES Julian Date. The result is obtained by subtracting the CNES Julian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>CNES Julian Date</returns>
	public static double ConvertJulianDateToCnesJulianDate(double julianDate)
	{
		return julianDate - DoubleCnesJulianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the CCSDS Julian Date. The result is obtained by subtracting the CCSDS Julian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>CCSDS Julian Date</returns>
	public static double ConvertJulianDateToCcsdsJulianDate(double julianDate)
	{
		return julianDate - DoubleCcsdsJulianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the LOP Julian Date. The result is obtained by subtracting the LOP Julian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>LOP Julian Date</returns>
	public static double ConvertJulianDateToLopJulianDate(double julianDate)
	{
		return julianDate - DoubleLopJulianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the Millennium Julian Date. The result is obtained by subtracting the Millennium Julian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Millennium Julian Date</returns>
	public static double ConvertJulianDateToMillenniumJulianDate(double julianDate)
	{
		return julianDate - DoubleMillenniumJulianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the Chronological Julian Date. The result is calculated using the CalculateChronologicalJulianDate method.</summary>
	/// <remarks>Uses local timezone daylight saving rules for the conversion.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Chronological Julian Date</returns>
	public static double ConvertJulianDateToChronologicalJulianDate(double julianDate)
	{
		return CalculateChronologicalJulianDate(julianDate: julianDate);
	}

	/// <summary>Converts a Julian Date to the Chronological Modified Julian Date. The result is calculated using the CalculateChronologicalModifiedJulianDate method.</summary>
	/// <remarks>Uses local timezone daylight saving rules for the conversion.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Chronological Modified Julian Date</returns>
	public static double ConvertJulianDateToChronologicalModifiedJulianDate(double julianDate)
	{
		return CalculateChronologicalModifiedJulianDate(julianDate: julianDate);
	}

	/// <summary>Converts a Julian Date to the Lilian Date. The result is obtained by subtracting the Lilian Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Lilian Date</returns>
	public static double ConvertJulianDateToLilianDate(double julianDate)
	{
		return julianDate - DoubleLilianDateCoefficient;
	}

	/// <summary>Converts a Julian Date to the Rata Die. The result is obtained by subtracting the Rata Die coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Rata Die</returns>
	public static double ConvertJulianDateToRataDie(double julianDate)
	{
		return julianDate - DoubleRataDieCoefficient;
	}

	/// <summary>Converts a Julian Date to the Mars Sol Date. The result is obtained by subtracting the Mars Sol Date coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Mars Sol Date</returns>
	public static double ConvertJulianDateToMarsSolDate(double julianDate)
	{
		return julianDate - DoubleMarsSolDateCoefficient;
	}

	/// <summary>
	/// Converts a Julian Date to Unix Time (days since 1970-01-01). The result is obtained by subtracting the Unix Time coefficient from the given Julian Date.</summary>
	/// <remarks>Direct arithmetic conversion from Julian Date.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Unix Time</returns>
	public static double ConvertJulianDateToUnixTime(double julianDate)
	{
		return julianDate - DoubleUnixTimeCoefficient;
	}

	/// <summary>Converts a Julian Epoch year back to a Julian Date.</summary>
	/// <remarks>Inverse operation of CalculateJulianEpoch.</remarks>
	/// <param name="julianEpoch">Julian Epoch year (e.g., 2000.0)</param>
	/// <returns>Julian Date</returns>
	public static double ConvertJulianEpochToJulianDate(double julianEpoch)
	{
		return DoubleJ2000EpochJulianDateCoefficient + ((julianEpoch - 2000.0) * 365.25);
	}

	#endregion
}
