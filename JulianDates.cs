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
	private const double DoubleMarsSolDateCoefficient = 2405522.0028779;

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

	/// <summary>The Julian Date reference epoch used by the IAU-defined TDB/TCB transformation.</summary>
	/// <remarks>IAU 2006 reference epoch T0 = JD 2443144.5003725.</remarks>
	private const double DoubleTcbReferenceEpoch = 2443144.5003725;

	/// <summary>The defining rate constant for the TCG-to-TT transformation.</summary>
	/// <remarks>IAU defining constant LG.</remarks>
	private const double DoubleLg = 6.969290134e-10;

	/// <summary>The defining rate constant for the TCB-to-TDB transformation.</summary>
	/// <remarks>IAU defining constant LB.</remarks>
	private const double DoubleLb = 1.550519768e-8;

	/// <summary>The TDB minus TCB offset at the IAU reference epoch, in seconds.</summary>
	private const double DoubleTdbAtT0Seconds = -6.55e-5;

	/// <summary>The Julian Date epoch of the CCSDS day-segmented time code.</summary>
	/// <remarks>CCSDS epoch: 1958-01-01 00:00:00 TAI.</remarks>
	private const double DoubleCcsdsEpoch = 2436204.5;

	/// <summary>The Julian Date epoch of the J2000 day-count origin.</summary>
	private const double DoubleJ2000DateEpoch = 2451545.0;

	/// <summary>The conventional GPS-to-TAI offset, in seconds.</summary>
	private const double DoubleGpsTaiOffsetSeconds = 19.0;

	/// <summary>The conventional TT-to-TAI offset, in seconds.</summary>
	private const double DoubleTtTaiOffsetSeconds = 32.184;

	/// <summary>The reference period of a Martian sol in Earth days.</summary>
	private const double DoubleMarsSolLengthDays = 1.0274912517;

	/// <summary>The Mars Sol Date reference epoch in Julian Date.</summary>
	/// <remarks>MSD 0 is referenced to JD 2405522.0028779 in the conventional Mars time definition.</remarks>
	private const double DoubleMarsSolEpoch = 2405522.0028779;

	#endregion

	#region JD Calculators

	/// <summary>Calculates the current Julian Date using the current system time in UTC. The calculation is based on converting the current UTC time to an OLE Automation Date (OADate) and then adding the Julian Date coefficient to obtain the astronomical Julian Date.</summary>
	/// <remarks>Uses system UTC time at execution time.</remarks>
	/// <returns>Julian Date for the current UTC time</returns>
	public static double CalculateJulianDate()
	{
		return DateTime.UtcNow.ToOADate() + DoubleJulianDateCoefficient;
	}

	/// <summary>Calculates the Julian Date for a given date and time. The calculation converts the provided DateTime to an OLE Automation Date (OADate) and adds the Julian Date coefficient to obtain the astronomical Julian Date.</summary>
	/// <remarks>UTC values are used unchanged; local values are converted to UTC. Unspecified values are interpreted as local time by the .NET runtime.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Julian Date corresponding to the specified date and time</returns>
	public static double CalculateJulianDate(DateTime date)
	{
		DateTime utcDate = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
		return utcDate.ToOADate() + DoubleJulianDateCoefficient;
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

	/// <summary>Calculates the current Mars Sol Date using the current UTC time and the current TAI-UTC offset.</summary>
	/// <remarks>The current TAI-UTC offset is 37 seconds. The resulting TT value also includes the fixed TT-TAI offset of 32.184 seconds.</remarks>
	/// <returns>Current Mars Sol Date.</returns>
	public static double CalculateMarsSolDate()
	{
		return CalculateMarsSolDate(julianDateUtc: CalculateJulianDate(), taiMinusUtcSeconds: 37.0);
	}

	/// <summary>Calculates Mars Sol Date for a civil date, treating the input as UTC and applying the current TAI-UTC offset.</summary>
	/// <remarks>For historical dates, use the overload that accepts the TAI-UTC offset valid at the requested instant.</remarks>
	/// <param name="date">UTC date and time.</param>
	/// <returns>Mars Sol Date.</returns>
	public static double CalculateMarsSolDate(DateTime date)
	{
		return CalculateMarsSolDate(julianDateUtc: CalculateJulianDate(date: date), taiMinusUtcSeconds: 37.0);
	}

	/// <summary>Calculates Mars Sol Date from a TT Julian Date.</summary>
	/// <remarks>The conventional expression is (JD(TT) - 2405522.0028779) / 1.0274912517.</remarks>
	/// <param name="julianDateTt">Terrestrial Time Julian Date.</param>
	/// <returns>Mars Sol Date.</returns>
	public static double CalculateMarsSolDate(double julianDateTt)
	{
		return (julianDateTt - DoubleMarsSolDateCoefficient) / DoubleMarsSolLengthDays;
	}

	/// <summary>Calculates Mars Sol Date from a UTC Julian Date and the applicable TAI-UTC offset.</summary>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds, valid at the specified date.</param>
	/// <returns>Mars Sol Date.</returns>
	public static double CalculateMarsSolDate(double julianDateUtc, double taiMinusUtcSeconds)
	{
		double julianDateTt = ConvertUtcToTt(julianDateUtc, taiMinusUtcSeconds);
		return CalculateMarsSolDate(julianDateTt: julianDateTt);
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

	/// <summary>Returns a geocentric Julian Date (GJD) for an already geocentric Julian Date value.</summary>
	/// <remarks>GJD identifies the origin of the time coordinate; it is not a separate epoch offset from JD.</remarks>
	/// <param name="geocentricJulianDate">Geocentric Julian Date.</param>
	/// <returns>The unchanged geocentric Julian Date.</returns>
	public static double CalculateGeocentricJulianDate(double geocentricJulianDate)
	{
		return geocentricJulianDate;
	}

	/// <summary>Calculates the J2000 date, expressed as days from JD 2451545.0.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>J2000 date in days.</returns>
	public static double CalculateJ2000Date(double julianDate)
	{
		return julianDate - DoubleJ2000DateEpoch;
	}

	/// <summary>Calculates the Modified Julian Date 2000 (MJD2000).</summary>
	/// <remarks>MJD2000 is used here as a J2000-based day count: JD - 2451545.0. The name is not universally standardized.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>J2000-based day count.</returns>
	public static double CalculateModifiedJulianDate2000(double julianDate)
	{
		return julianDate - DoubleJ2000DateEpoch;
	}

	/// <summary>Calculates a normalized Julian Date using a caller-defined reference epoch.</summary>
	/// <remarks>NJD has no single universally accepted epoch. This method uses the explicitly supplied reference epoch.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <param name="referenceEpoch">Reference epoch expressed as a Julian Date.</param>
	/// <returns>Normalized Julian Date.</returns>
	public static double CalculateNormalizedJulianDate(double julianDate, double referenceEpoch)
	{
		return CalculateOffsetJulianDate(julianDate: julianDate, referenceEpoch: referenceEpoch);
	}

	/// <summary>Calculates the fractional part of a Julian Date.</summary>
	/// <remarks>The returned value is normalized to the range [0, 1), using the Julian Date noon boundary.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Fractional day.</returns>
	public static double CalculateJulianDayFraction(double julianDate)
	{
		return julianDate - Math.Floor(d: julianDate);
	}

	/// <summary>Calculates an offset Julian Date using a caller-defined reference epoch.</summary>
	/// <remarks>This generic form can represent application-specific EJD, OJD, or NJD conventions. Those abbreviations do not have one universally accepted epoch.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <param name="referenceEpoch">Reference epoch expressed as a Julian Date.</param>
	/// <returns>Number of days since the reference epoch.</returns>
	public static double CalculateOffsetJulianDate(double julianDate, double referenceEpoch)
	{
		return julianDate - referenceEpoch;
	}

	/// <summary>Calculates a configurable extended or normalized Julian Date.</summary>
	/// <remarks>Because EJD and NJD are used with differing conventions, the reference epoch must be supplied explicitly.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <param name="referenceEpoch">Reference epoch expressed as a Julian Date.</param>
	/// <returns>Offset Julian Date.</returns>
	public static double CalculateExtendedJulianDate(double julianDate, double referenceEpoch)
	{
		return CalculateOffsetJulianDate(julianDate: julianDate, referenceEpoch: referenceEpoch);
	}

	/// <summary>Calculates the Julian Century from J2000.0.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Julian centuries of 36525 days.</returns>
	public static double CalculateJulianCentury(double julianDate)
	{
		return (julianDate - DoubleJ2000EpochJulianDateCoefficient) / 36525.0;
	}

	/// <summary>Calculates the Julian Millennium from J2000.0.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Julian millennia of 365250 days.</returns>
	public static double CalculateJulianMillennium(double julianDate)
	{
		return (julianDate - DoubleJ2000EpochJulianDateCoefficient) / 365250.0;
	}

	/// <summary>Calculates the decimal year from a Julian Date.</summary>
	/// <remarks>The calculation uses the proleptic Gregorian calendar and the actual length of the corresponding calendar year.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Decimal Gregorian year.</returns>
	public static double CalculateDecimalYear(double julianDate)
	{
		DateTime date = ConvertJulianDateToCivilCalendar(julianDate: julianDate);
		DateTime start = new(year: date.Year, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
		DateTime end = start.AddYears(value: 1);
		double dayOfYear = (date - start).TotalDays;
		return date.Year + (dayOfYear / (end - start).TotalDays);
	}

	/// <summary>Calculates the fractional Gregorian year from a Julian Date.</summary>
	/// <remarks>This is an alias for the decimal year representation.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Fractional Gregorian year.</returns>
	public static double CalculateFractionalYear(double julianDate)
	{
		return CalculateDecimalYear(julianDate: julianDate);
	}

	/// <summary>Calculates the day of year, including the fractional time of day, from a Julian Date.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>One-based fractional day of year.</returns>
	public static double CalculateDayOfYear(double julianDate)
	{
		DateTime date = ConvertJulianDateToCivilCalendar(julianDate: julianDate);
		DateTime start = new(year: date.Year, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
		return (date - start).TotalDays + 1.0;
	}

	/// <summary>Calculates the Barycentric Julian Date (BJD) using a geometric light-time correction.</summary>
	/// <remarks>The observer position relative to the solar-system barycenter and the target unit direction must use the same inertial frame and AU units. This geometric correction does not include Shapiro delay or other relativistic terms.</remarks>
	/// <param name="julianDate">Input Julian Date on the explicitly selected time scale.</param>
	/// <param name="observerBarycentricX">Observer barycentric X coordinate in AU.</param>
	/// <param name="observerBarycentricY">Observer barycentric Y coordinate in AU.</param>
	/// <param name="observerBarycentricZ">Observer barycentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>Geometrically corrected BJD on the same time scale as the input.</returns>
	public static double CalculateBarycentricJulianDate(double julianDate, double observerBarycentricX, double observerBarycentricY, double observerBarycentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		double dotProduct = (observerBarycentricX * targetUnitX) + (observerBarycentricY * targetUnitY) + (observerBarycentricZ * targetUnitZ);
		return julianDate + (dotProduct / DoubleSpeedOfLightAuPerDay);
	}

	/// <summary>Calculates BJD(UTC) using a geometric barycentric light-time correction.</summary>
	/// <remarks>The result remains on the UTC scale; this method does not convert UTC to TDB.</remarks>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="observerBarycentricX">Observer barycentric X coordinate in AU.</param>
	/// <param name="observerBarycentricY">Observer barycentric Y coordinate in AU.</param>
	/// <param name="observerBarycentricZ">Observer barycentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>BJD(UTC).</returns>
	public static double CalculateBarycentricJulianDateUtc(double julianDateUtc, double observerBarycentricX, double observerBarycentricY, double observerBarycentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateBarycentricJulianDate(julianDate: julianDateUtc, observerBarycentricX: observerBarycentricX, observerBarycentricY: observerBarycentricY, observerBarycentricZ: observerBarycentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates BJD(TT) using a geometric barycentric light-time correction.</summary>
	/// <remarks>The input must already be expressed in TT. The geometric correction does not include Shapiro delay.</remarks>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <param name="observerBarycentricX">Observer barycentric X coordinate in AU.</param>
	/// <param name="observerBarycentricY">Observer barycentric Y coordinate in AU.</param>
	/// <param name="observerBarycentricZ">Observer barycentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>BJD(TT).</returns>
	public static double CalculateBarycentricJulianDateTt(double julianDateTt, double observerBarycentricX, double observerBarycentricY, double observerBarycentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateBarycentricJulianDate(julianDate: julianDateTt, observerBarycentricX: observerBarycentricX, observerBarycentricY: observerBarycentricY, observerBarycentricZ: observerBarycentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates BJD(TDB) using a geometric barycentric light-time correction.</summary>
	/// <remarks>The input must already be expressed in TDB. The geometric correction does not include Shapiro delay.</remarks>
	/// <param name="julianDateTdb">TDB Julian Date.</param>
	/// <param name="observerBarycentricX">Observer barycentric X coordinate in AU.</param>
	/// <param name="observerBarycentricY">Observer barycentric Y coordinate in AU.</param>
	/// <param name="observerBarycentricZ">Observer barycentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>BJD(TDB).</returns>
	public static double CalculateBarycentricJulianDateTdb(double julianDateTdb, double observerBarycentricX, double observerBarycentricY, double observerBarycentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateBarycentricJulianDate(julianDate: julianDateTdb, observerBarycentricX: observerBarycentricX, observerBarycentricY: observerBarycentricY, observerBarycentricZ: observerBarycentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates HJD(UTC) using a geometric heliocentric light-time correction.</summary>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="earthHeliocentricX">Earth heliocentric X coordinate in AU.</param>
	/// <param name="earthHeliocentricY">Earth heliocentric Y coordinate in AU.</param>
	/// <param name="earthHeliocentricZ">Earth heliocentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>HJD(UTC).</returns>
	public static double CalculateHeliocentricJulianDateUtc(double julianDateUtc, double earthHeliocentricX, double earthHeliocentricY, double earthHeliocentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateHeliocentricJulianDate(julianDate: julianDateUtc, earthVectorX: earthHeliocentricX, earthVectorY: earthHeliocentricY, earthVectorZ: earthHeliocentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates HJD(TT) using a geometric heliocentric light-time correction.</summary>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <param name="earthHeliocentricX">Earth heliocentric X coordinate in AU.</param>
	/// <param name="earthHeliocentricY">Earth heliocentric Y coordinate in AU.</param>
	/// <param name="earthHeliocentricZ">Earth heliocentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>HJD(TT).</returns>
	public static double CalculateHeliocentricJulianDateTt(double julianDateTt, double earthHeliocentricX, double earthHeliocentricY, double earthHeliocentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateHeliocentricJulianDate(julianDate: julianDateTt, earthVectorX: earthHeliocentricX, earthVectorY: earthHeliocentricY, earthVectorZ: earthHeliocentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates HJD(TDB) using a geometric heliocentric light-time correction.</summary>
	/// <param name="julianDateTdb">TDB Julian Date.</param>
	/// <param name="earthHeliocentricX">Earth heliocentric X coordinate in AU.</param>
	/// <param name="earthHeliocentricY">Earth heliocentric Y coordinate in AU.</param>
	/// <param name="earthHeliocentricZ">Earth heliocentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>HJD(TDB).</returns>
	public static double CalculateHeliocentricJulianDateTdb(double julianDateTdb, double earthHeliocentricX, double earthHeliocentricY, double earthHeliocentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateHeliocentricJulianDate(julianDate: julianDateTdb, earthVectorX: earthHeliocentricX, earthVectorY: earthHeliocentricY, earthVectorZ: earthHeliocentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates a mission-specific planetary sol number.</summary>
	/// <remarks>Mission sol numbering is not globally standardized. Supply the mission's documented epoch and the adopted sol length.</remarks>
	/// <param name="julianDate">Input Julian Date.</param>
	/// <param name="missionEpochJulianDate">Julian Date of mission sol zero.</param>
	/// <param name="solLengthDays">Length of one local planetary sol in Earth days.</param>
	/// <returns>Zero-based fractional mission sol number.</returns>
	public static double CalculatePlanetarySolDate(double julianDate, double missionEpochJulianDate, double solLengthDays)
	{
		return (julianDate - missionEpochJulianDate) / solLengthDays;
	}

	/// <summary>Calculates a zero-based mission sol number from a planetary sol date.</summary>
	/// <param name="planetarySolDate">Fractional planetary sol date.</param>
	/// <returns>Integer sol number.</returns>
	public static long CalculatePlanetarySolNumber(double planetarySolDate)
	{
		return (long)Math.Floor(d: planetarySolDate);
	}

	/// <summary>Calculates a mission-specific Mars Sol Number (MSN).</summary>
	/// <remarks>MSN is mission-dependent and must not be confused with the continuous Mars Sol Date (MSD).</remarks>
	/// <param name="julianDate">Input Julian Date.</param>
	/// <param name="missionEpochJulianDate">Julian Date of mission sol zero.</param>
	/// <returns>Zero-based integer mission sol number.</returns>
	public static long CalculateMarsSolNumber(double julianDate, double missionEpochJulianDate)
	{
		return (long)Math.Floor(d: (julianDate - missionEpochJulianDate) / DoubleMarsSolLengthDays);
	}

	/// <summary>Calculates Mars Coordinated Time (MTC) in decimal Martian hours.</summary>
	/// <param name="marsSolDate">Mars Sol Date.</param>
	/// <returns>Time of day in the range [0, 24) Martian hours.</returns>
	public static double CalculateMarsCoordinatedTime(double marsSolDate)
	{
		double fraction = marsSolDate - Math.Floor(d: marsSolDate);
		return fraction * 24.0;
	}

	/// <summary>Calculates areocentric solar longitude (Ls) from the Martian true anomaly.</summary>
	/// <remarks>For the conventional orbital-element definition, Ls is the sum of true anomaly and longitude of perihelion, normalized to [0, 360) degrees.</remarks>
	/// <param name="trueAnomalyDegrees">Mars true anomaly in degrees.</param>
	/// <param name="longitudeOfPerihelionDegrees">Mars longitude of perihelion in degrees, in the same reference frame.</param>
	/// <returns>Areocentric solar longitude in degrees.</returns>
	public static double CalculateAreocentricSolarLongitude(double trueAnomalyDegrees, double longitudeOfPerihelionDegrees)
	{
		double solarLongitude = trueAnomalyDegrees + longitudeOfPerihelionDegrees;
		return ((solarLongitude % 360.0) + 360.0) % 360.0;
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

	/// <summary>Converts a TT Julian Date to Mars Sol Date using the conventional Martian sol length.</summary>
	/// <remarks>The input is interpreted as TT. Use the UTC overload of CalculateMarsSolDate when a leap-second offset is available.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Mars Sol Date</returns>
	public static double ConvertJulianDateToMarsSolDate(double julianDate)
	{
		return (julianDate - DoubleMarsSolDateCoefficient) / DoubleMarsSolLengthDays;
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

	/// <summary>Converts Mars Sol Date to a TT Julian Date.</summary>
	/// <param name="marsSolDate">Mars Sol Date.</param>
	/// <returns>TT Julian Date.</returns>
	public static double ConvertMarsSolDateToJulianDate(double marsSolDate)
	{
		return DoubleMarsSolDateCoefficient + (marsSolDate * DoubleMarsSolLengthDays);
	}

	/// <summary>Converts a normalized Julian Date to Julian Date.</summary>
	/// <param name="normalizedJulianDate">Normalized Julian Date.</param>
	/// <param name="referenceEpoch">Reference epoch expressed as a Julian Date.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertNormalizedJulianDateToJulianDate(double normalizedJulianDate, double referenceEpoch)
	{
		return ConvertOffsetJulianDateToJulianDate(offsetJulianDate: normalizedJulianDate, referenceEpoch: referenceEpoch);
	}

	/// <summary>Converts a Julian Date to the J2000 date day count.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Days from JD 2451545.0.</returns>
	public static double ConvertJulianDateToJ2000Date(double julianDate)
	{
		return CalculateJ2000Date(julianDate: julianDate);
	}

	/// <summary>Converts a Julian Date to MJD2000.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>J2000-based day count.</returns>
	public static double ConvertJulianDateToModifiedJulianDate2000(double julianDate)
	{
		return CalculateModifiedJulianDate2000(julianDate: julianDate);
	}

	/// <summary>Converts a J2000 date day count to Julian Date.</summary>
	/// <param name="j2000Date">Days from JD 2451545.0.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertJ2000DateToJulianDate(double j2000Date)
	{
		return j2000Date + DoubleJ2000DateEpoch;
	}

	/// <summary>Converts MJD2000 to Julian Date.</summary>
	/// <param name="mjd2000">J2000-based day count.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertModifiedJulianDate2000ToJulianDate(double mjd2000)
	{
		return mjd2000 + DoubleJ2000DateEpoch;
	}

	/// <summary>Converts an offset Julian Date to Julian Date.</summary>
	/// <param name="offsetJulianDate">Days since the reference epoch.</param>
	/// <param name="referenceEpoch">Reference epoch as Julian Date.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertOffsetJulianDateToJulianDate(double offsetJulianDate, double referenceEpoch)
	{
		return offsetJulianDate + referenceEpoch;
	}

	/// <summary>Converts a Julian Century to Julian Date.</summary>
	/// <param name="julianCentury">Julian centuries from J2000.0.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertJulianCenturyToJulianDate(double julianCentury)
	{
		return DoubleJ2000EpochJulianDateCoefficient + (julianCentury * 36525.0);
	}

	/// <summary>Converts a Julian Millennium to Julian Date.</summary>
	/// <param name="julianMillennium">Julian millennia from J2000.0.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertJulianMillenniumToJulianDate(double julianMillennium)
	{
		return DoubleJ2000EpochJulianDateCoefficient + (julianMillennium * 365250.0);
	}

	/// <summary>Converts a decimal Gregorian year to Julian Date.</summary>
	/// <param name="decimalYear">Decimal Gregorian year.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertDecimalYearToJulianDate(double decimalYear)
	{
		int year = (int)Math.Floor(d: decimalYear);
		DateTime start = new(year: year, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
		DateTime end = start.AddYears(value: 1);
		return CalculateJulianDate(date: start) + ((decimalYear - year) * (end - start).TotalDays);
	}

	/// <summary>Converts a Julian Date to the number of elapsed seconds since a specified epoch.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <param name="epochJulianDate">Reference epoch as Julian Date.</param>
	/// <returns>Elapsed seconds.</returns>
	public static double ConvertJulianDateToElapsedSeconds(double julianDate, double epochJulianDate)
	{
		return (julianDate - epochJulianDate) * SecondsOfDay;
	}

	/// <summary>Returns a geocentric Julian Date (GJD) for an already geocentric Julian Date value.</summary>
	/// <param name="julianDate">Geocentric Julian Date.</param>
	/// <returns>Geocentric Julian Date.</returns>
	public static double ConvertJulianDateToGeocentricJulianDate(double julianDate)
	{
		return julianDate;
	}

	/// <summary>Converts UTC Julian Date to TAI Julian Date using the supplied TAI-UTC offset.</summary>
	/// <remarks>The caller must provide the leap-second offset valid at the represented UTC instant. DateTime alone cannot represent the UTC leap-second label 23:59:60.</remarks>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds.</param>
	/// <returns>TAI Julian Date.</returns>
	public static double ConvertUtcToTai(double julianDateUtc, double taiMinusUtcSeconds)
	{
		return julianDateUtc + (taiMinusUtcSeconds / SecondsOfDay);
	}

	/// <summary>Converts TAI Julian Date to UTC Julian Date using the supplied TAI-UTC offset.</summary>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds.</param>
	/// <returns>UTC Julian Date.</returns>
	public static double ConvertTaiToUtc(double julianDateTai, double taiMinusUtcSeconds)
	{
		return julianDateTai - (taiMinusUtcSeconds / SecondsOfDay);
	}

	/// <summary>Converts TAI Julian Date to Terrestrial Time (TT).</summary>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <returns>TT Julian Date.</returns>
	public static double ConvertTaiToTt(double julianDateTai)
	{
		return julianDateTai + (DoubleTtTaiOffsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts Terrestrial Time (TT) Julian Date to TAI.</summary>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <returns>TAI Julian Date.</returns>
	public static double ConvertTtToTai(double julianDateTt)
	{
		return julianDateTt - (DoubleTtTaiOffsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts UTC Julian Date to TT using the supplied TAI-UTC offset.</summary>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds.</param>
	/// <returns>TT Julian Date.</returns>
	public static double ConvertUtcToTt(double julianDateUtc, double taiMinusUtcSeconds)
	{
		return ConvertTaiToTt(ConvertUtcToTai(julianDateUtc, taiMinusUtcSeconds));
	}

	/// <summary>Converts TT Julian Date to UTC using the supplied TAI-UTC offset.</summary>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds.</param>
	/// <returns>UTC Julian Date.</returns>
	public static double ConvertTtToUtc(double julianDateTt, double taiMinusUtcSeconds)
	{
		return ConvertTaiToUtc(ConvertTtToTai(julianDateTt), taiMinusUtcSeconds);
	}

	/// <summary>Converts UTC Julian Date to UT1 using DUT1.</summary>
	/// <remarks>DUT1 is UT1 minus UTC and must be obtained from Earth orientation data for the requested date.</remarks>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="dut1Seconds">UT1 minus UTC, in seconds.</param>
	/// <returns>UT1 Julian Date.</returns>
	public static double ConvertUtcToUt1(double julianDateUtc, double dut1Seconds)
	{
		return julianDateUtc + (dut1Seconds / SecondsOfDay);
	}

	/// <summary>Converts UT1 Julian Date to UTC using DUT1.</summary>
	/// <param name="julianDateUt1">UT1 Julian Date.</param>
	/// <param name="dut1Seconds">UT1 minus UTC, in seconds.</param>
	/// <returns>UTC Julian Date.</returns>
	public static double ConvertUt1ToUtc(double julianDateUt1, double dut1Seconds)
	{
		return julianDateUt1 - (dut1Seconds / SecondsOfDay);
	}

	/// <summary>Approximates UT2 from UT1 using the conventional seasonal correction.</summary>
	/// <remarks>UT2 is a historical smoothed form of UT1. This low-order periodic approximation is not a substitute for official historical UT2 data.</remarks>
	/// <param name="julianDateUt1">UT1 Julian Date.</param>
	/// <returns>Approximate UT2 Julian Date.</returns>
	public static double ConvertUt1ToUt2(double julianDateUt1)
	{
		DateTime date = ConvertJulianDateToCivilCalendar(julianDateUt1);
		double fractionalYear = (date.DayOfYear - 1 + date.TimeOfDay.TotalDays) / (DateTime.IsLeapYear(date.Year) ? 366.0 : 365.0);
		double angle = 2.0 * Math.PI * fractionalYear;
		double correctionSeconds = (0.022 * Math.Sin(angle)) - (0.012 * Math.Cos(angle)) - (0.006 * Math.Sin(2.0 * angle)) + (0.007 * Math.Cos(2.0 * angle));
		return julianDateUt1 + (correctionSeconds / SecondsOfDay);
	}

	/// <summary>Converts TT Julian Date to TCG using the IAU linear transformation.</summary>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <returns>TCG Julian Date.</returns>
	public static double ConvertTtToTcg(double julianDateTt)
	{
		double referenceMjd = 43144.0;
		double elapsedDays = julianDateTt - DoubleModifiedJulianDateCoefficient - referenceMjd;
		return julianDateTt + (DoubleLg * elapsedDays);
	}

	/// <summary>Converts TCG Julian Date to TT using the IAU linear transformation.</summary>
	/// <param name="julianDateTcg">TCG Julian Date.</param>
	/// <returns>TT Julian Date.</returns>
	public static double ConvertTcgToTt(double julianDateTcg)
	{
		double referenceMjd = 43144.0;
		double elapsedDays = julianDateTcg - DoubleModifiedJulianDateCoefficient - referenceMjd;
		return julianDateTcg - (DoubleLg * elapsedDays / (1.0 + DoubleLg));
	}

	/// <summary>Approximates TDB from TT using the leading periodic terms of the solar-system time transformation.</summary>
	/// <remarks>This compact approximation has millisecond-level accuracy at best. Precision ephemeris work should use an IAU SOFA/ERFA implementation or a full Fairhead-Bretagnon model.</remarks>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <returns>Approximate TDB Julian Date.</returns>
	public static double ConvertTtToTdb(double julianDateTt)
	{
		double t = (julianDateTt - DoubleJ2000EpochJulianDateCoefficient) / 36525.0;
		double meanAnomaly = (357.5277233 + (35999.05034 * t)) * (Math.PI / 180.0);
		double correctionSeconds = (0.001657 * Math.Sin(meanAnomaly)) + (0.000022 * Math.Sin(2.0 * meanAnomaly));
		return julianDateTt + (correctionSeconds / SecondsOfDay);
	}

	/// <summary>Approximates TT from TDB by iteratively reversing the leading periodic correction.</summary>
	/// <param name="julianDateTdb">TDB Julian Date.</param>
	/// <returns>Approximate TT Julian Date.</returns>
	public static double ConvertTdbToTt(double julianDateTdb)
	{
		double result = julianDateTdb;
		for (int i = 0; i < 3; i++)
		{
			result = julianDateTdb - (ConvertTtToTdb(result) - result);
		}

		return result;
	}

	/// <summary>Converts TCB Julian Date to TDB using the IAU 2006 linear transformation.</summary>
	/// <param name="julianDateTcb">TCB Julian Date.</param>
	/// <returns>TDB Julian Date.</returns>
	public static double ConvertTcbToTdb(double julianDateTcb)
	{
		double offsetSeconds = (-DoubleLb * (julianDateTcb - DoubleTcbReferenceEpoch) * SecondsOfDay) + DoubleTdbAtT0Seconds;
		return julianDateTcb + (offsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts TDB Julian Date to TCB using the inverse IAU 2006 linear transformation.</summary>
	/// <param name="julianDateTdb">TDB Julian Date.</param>
	/// <returns>TCB Julian Date.</returns>
	public static double ConvertTdbToTcb(double julianDateTdb)
	{
		double numerator = julianDateTdb - DoubleTcbReferenceEpoch - (DoubleTdbAtT0Seconds / SecondsOfDay);
		return DoubleTcbReferenceEpoch + (numerator / (1.0 - DoubleLb));
	}

	/// <summary>Converts GPS Julian Date to TAI Julian Date.</summary>
	/// <param name="julianDateGps">GPS Julian Date.</param>
	/// <returns>TAI Julian Date.</returns>
	public static double ConvertGpsToTai(double julianDateGps)
	{
		return julianDateGps + (DoubleGpsTaiOffsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts TAI Julian Date to GPS Julian Date.</summary>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <returns>GPS Julian Date.</returns>
	public static double ConvertTaiToGps(double julianDateTai)
	{
		return julianDateTai - (DoubleGpsTaiOffsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts a Julian Date to CCSDS Day Segmented Time (CDS) components.</summary>
	/// <remarks>The returned day count is relative to the CCSDS epoch. The sub-day component is expressed in milliseconds; higher-resolution CCSDS variants may use finer fields.</remarks>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <returns>A tuple containing elapsed days and milliseconds within the day.</returns>
	public static (long Days, uint Milliseconds) ConvertJulianDateToCcsdsDaySegmentedTime(double julianDateTai)
	{
		double elapsedDays = julianDateTai - DoubleCcsdsEpoch;
		long days = (long)Math.Floor(elapsedDays);
		uint milliseconds = (uint)Math.Floor((elapsedDays - days) * SecondsOfDay * 1000.0);
		return (Days: days, Milliseconds: milliseconds);
	}

	/// <summary>Converts a Julian Date to CCSDS Unsegmented Time Code (CUC) components.</summary>
	/// <remarks>The result is a numeric coarse/fine pair. The actual byte encoding depends on the mission-selected CUC epoch and field widths.</remarks>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <param name="epochJulianDate">Mission-defined CUC epoch as a Julian Date.</param>
	/// <returns>Elapsed whole seconds and fractional second.</returns>
	public static (long CoarseSeconds, double FineSeconds) ConvertJulianDateToCcsdsUnsegmentedTime(double julianDateTai, double epochJulianDate)
	{
		double elapsedSeconds = (julianDateTai - epochJulianDate) * SecondsOfDay;
		long coarseSeconds = (long)Math.Floor(d: elapsedSeconds);
		return (CoarseSeconds: coarseSeconds, FineSeconds: elapsedSeconds - coarseSeconds);
	}

	#endregion
}
namespace JulianDates;

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
	private const double DoubleMarsSolDateCoefficient = 2405522.0028779;

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

	/// <summary>The Julian Date reference epoch used by the IAU-defined TDB/TCB transformation.</summary>
	/// <remarks>IAU 2006 reference epoch T0 = JD 2443144.5003725.</remarks>
	private const double DoubleTcbReferenceEpoch = 2443144.5003725;

	/// <summary>The defining rate constant for the TCG-to-TT transformation.</summary>
	/// <remarks>IAU defining constant LG.</remarks>
	private const double DoubleLg = 6.969290134e-10;

	/// <summary>The defining rate constant for the TCB-to-TDB transformation.</summary>
	/// <remarks>IAU defining constant LB.</remarks>
	private const double DoubleLb = 1.550519768e-8;

	/// <summary>The TDB minus TCB offset at the IAU reference epoch, in seconds.</summary>
	private const double DoubleTdbAtT0Seconds = -6.55e-5;

	/// <summary>The Julian Date epoch of the CCSDS day-segmented time code.</summary>
	/// <remarks>CCSDS epoch: 1958-01-01 00:00:00 TAI.</remarks>
	private const double DoubleCcsdsEpoch = 2436204.5;

	/// <summary>The Julian Date epoch of the J2000 day-count origin.</summary>
	private const double DoubleJ2000DateEpoch = 2451545.0;

	/// <summary>The conventional GPS-to-TAI offset, in seconds.</summary>
	private const double DoubleGpsTaiOffsetSeconds = 19.0;

	/// <summary>The conventional TT-to-TAI offset, in seconds.</summary>
	private const double DoubleTtTaiOffsetSeconds = 32.184;

	/// <summary>The reference period of a Martian sol in Earth days.</summary>
	private const double DoubleMarsSolLengthDays = 1.0274912517;

	/// <summary>The Mars Sol Date reference epoch in Julian Date.</summary>
	/// <remarks>MSD 0 is referenced to JD 2405522.0028779 in the conventional Mars time definition.</remarks>
	private const double DoubleMarsSolEpoch = 2405522.0028779;

	#endregion

	#region JD Calculators

	/// <summary>Calculates the current Julian Date using the current system time in UTC. The calculation is based on converting the current UTC time to an OLE Automation Date (OADate) and then adding the Julian Date coefficient to obtain the astronomical Julian Date.</summary>
	/// <remarks>Uses system UTC time at execution time.</remarks>
	/// <returns>Julian Date for the current UTC time</returns>
	public static double CalculateJulianDate()
	{
		return DateTime.UtcNow.ToOADate() + DoubleJulianDateCoefficient;
	}

	/// <summary>Calculates the Julian Date for a given date and time. The calculation converts the provided DateTime to an OLE Automation Date (OADate) and adds the Julian Date coefficient to obtain the astronomical Julian Date.</summary>
	/// <remarks>UTC values are used unchanged; local values are converted to UTC. Unspecified values are interpreted as local time by the .NET runtime.</remarks>
	/// <param name="date">The date and time to convert</param>
	/// <returns>Julian Date corresponding to the specified date and time</returns>
	public static double CalculateJulianDate(DateTime date)
	{
		DateTime utcDate = date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
		return utcDate.ToOADate() + DoubleJulianDateCoefficient;
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

	/// <summary>Calculates the current Mars Sol Date using the current UTC time and the current TAI-UTC offset.</summary>
	/// <remarks>The current TAI-UTC offset is 37 seconds. The resulting TT value also includes the fixed TT-TAI offset of 32.184 seconds.</remarks>
	/// <returns>Current Mars Sol Date.</returns>
	public static double CalculateMarsSolDate()
	{
		return CalculateMarsSolDate(julianDateUtc: CalculateJulianDate(), taiMinusUtcSeconds: 37.0);
	}

	/// <summary>Calculates Mars Sol Date for a civil date, treating the input as UTC and applying the current TAI-UTC offset.</summary>
	/// <remarks>For historical dates, use the overload that accepts the TAI-UTC offset valid at the requested instant.</remarks>
	/// <param name="date">UTC date and time.</param>
	/// <returns>Mars Sol Date.</returns>
	public static double CalculateMarsSolDate(DateTime date)
	{
		return CalculateMarsSolDate(julianDateUtc: CalculateJulianDate(date: date), taiMinusUtcSeconds: 37.0);
	}

	/// <summary>Calculates Mars Sol Date from a TT Julian Date.</summary>
	/// <remarks>The conventional expression is (JD(TT) - 2405522.0028779) / 1.0274912517.</remarks>
	/// <param name="julianDateTt">Terrestrial Time Julian Date.</param>
	/// <returns>Mars Sol Date.</returns>
	public static double CalculateMarsSolDate(double julianDateTt)
	{
		return (julianDateTt - DoubleMarsSolDateCoefficient) / DoubleMarsSolLengthDays;
	}

	/// <summary>Calculates Mars Sol Date from a UTC Julian Date and the applicable TAI-UTC offset.</summary>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds, valid at the specified date.</param>
	/// <returns>Mars Sol Date.</returns>
	public static double CalculateMarsSolDate(double julianDateUtc, double taiMinusUtcSeconds)
	{
		double julianDateTt = ConvertUtcToTt(julianDateUtc, taiMinusUtcSeconds);
		return CalculateMarsSolDate(julianDateTt);
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

	#region Additional Date Formats and Time Scales

	/// <summary>Returns a geocentric Julian Date (GJD) for an already geocentric Julian Date value.</summary>
	/// <remarks>GJD identifies the origin of the time coordinate; it is not a separate epoch offset from JD.</remarks>
	/// <param name="geocentricJulianDate">Geocentric Julian Date.</param>
	/// <returns>The unchanged geocentric Julian Date.</returns>
	public static double CalculateGeocentricJulianDate(double geocentricJulianDate)
	{
		return geocentricJulianDate;
	}

	/// <summary>Returns a geocentric Julian Date (GJD) for an already geocentric Julian Date value.</summary>
	/// <param name="julianDate">Geocentric Julian Date.</param>
	/// <returns>Geocentric Julian Date.</returns>
	public static double ConvertJulianDateToGeocentricJulianDate(double julianDate)
	{
		return julianDate;
	}

	/// <summary>Calculates the J2000 date, expressed as days from JD 2451545.0.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>J2000 date in days.</returns>
	public static double CalculateJ2000Date(double julianDate)
	{
		return julianDate - DoubleJ2000DateEpoch;
	}

	/// <summary>Calculates the Modified Julian Date 2000 (MJD2000).</summary>
	/// <remarks>MJD2000 is used here as a J2000-based day count: JD - 2451545.0. The name is not universally standardized.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>J2000-based day count.</returns>
	public static double CalculateModifiedJulianDate2000(double julianDate)
	{
		return julianDate - DoubleJ2000DateEpoch;
	}

	/// <summary>Calculates a normalized Julian Date using a caller-defined reference epoch.</summary>
	/// <remarks>NJD has no single universally accepted epoch. This method uses the explicitly supplied reference epoch.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <param name="referenceEpoch">Reference epoch expressed as a Julian Date.</param>
	/// <returns>Normalized Julian Date.</returns>
	public static double CalculateNormalizedJulianDate(double julianDate, double referenceEpoch)
	{
		return CalculateOffsetJulianDate(julianDate: julianDate, referenceEpoch: referenceEpoch);
	}

	/// <summary>Calculates the fractional part of a Julian Date.</summary>
	/// <remarks>The returned value is normalized to the range [0, 1), using the Julian Date noon boundary.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Fractional day.</returns>
	public static double CalculateJulianDayFraction(double julianDate)
	{
		return julianDate - Math.Floor(d: julianDate);
	}

	/// <summary>Calculates an offset Julian Date using a caller-defined reference epoch.</summary>
	/// <remarks>This generic form can represent application-specific EJD, OJD, or NJD conventions. Those abbreviations do not have one universally accepted epoch.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <param name="referenceEpoch">Reference epoch expressed as a Julian Date.</param>
	/// <returns>Number of days since the reference epoch.</returns>
	public static double CalculateOffsetJulianDate(double julianDate, double referenceEpoch)
	{
		return julianDate - referenceEpoch;
	}

	/// <summary>Calculates a configurable extended or normalized Julian Date.</summary>
	/// <remarks>Because EJD and NJD are used with differing conventions, the reference epoch must be supplied explicitly.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <param name="referenceEpoch">Reference epoch expressed as a Julian Date.</param>
	/// <returns>Offset Julian Date.</returns>
	public static double CalculateExtendedJulianDate(double julianDate, double referenceEpoch)
	{
		return CalculateOffsetJulianDate(julianDate: julianDate, referenceEpoch: referenceEpoch);
	}

	/// <summary>Calculates the Julian Century from J2000.0.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Julian centuries of 36525 days.</returns>
	public static double CalculateJulianCentury(double julianDate)
	{
		return (julianDate - DoubleJ2000EpochJulianDateCoefficient) / 36525.0;
	}

	/// <summary>Calculates the Julian Millennium from J2000.0.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Julian millennia of 365250 days.</returns>
	public static double CalculateJulianMillennium(double julianDate)
	{
		return (julianDate - DoubleJ2000EpochJulianDateCoefficient) / 365250.0;
	}

	/// <summary>Calculates the decimal year from a Julian Date.</summary>
	/// <remarks>The calculation uses the proleptic Gregorian calendar and the actual length of the corresponding calendar year.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Decimal Gregorian year.</returns>
	public static double CalculateDecimalYear(double julianDate)
	{
		DateTime date = ConvertJulianDateToCivilCalendar(julianDate: julianDate);
		DateTime start = new(year: date.Year, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
		DateTime end = start.AddYears(value: 1);
		double dayOfYear = (date - start).TotalDays;
		return date.Year + (dayOfYear / (end - start).TotalDays);
	}

	/// <summary>Calculates the fractional Gregorian year from a Julian Date.</summary>
	/// <remarks>This is an alias for the decimal year representation.</remarks>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Fractional Gregorian year.</returns>
	public static double CalculateFractionalYear(double julianDate)
	{
		return CalculateDecimalYear(julianDate: julianDate);
	}

	/// <summary>Calculates the day of year, including the fractional time of day, from a Julian Date.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>One-based fractional day of year.</returns>
	public static double CalculateDayOfYear(double julianDate)
	{
		DateTime date = ConvertJulianDateToCivilCalendar(julianDate: julianDate);
		DateTime start = new(year: date.Year, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
		return (date - start).TotalDays + 1.0;
	}

	/// <summary>Converts UTC Julian Date to TAI Julian Date using the supplied TAI-UTC offset.</summary>
	/// <remarks>The caller must provide the leap-second offset valid at the represented UTC instant. DateTime alone cannot represent the UTC leap-second label 23:59:60.</remarks>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds.</param>
	/// <returns>TAI Julian Date.</returns>
	public static double ConvertUtcToTai(double julianDateUtc, double taiMinusUtcSeconds)
	{
		return julianDateUtc + (taiMinusUtcSeconds / SecondsOfDay);
	}

	/// <summary>Converts TAI Julian Date to UTC Julian Date using the supplied TAI-UTC offset.</summary>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds.</param>
	/// <returns>UTC Julian Date.</returns>
	public static double ConvertTaiToUtc(double julianDateTai, double taiMinusUtcSeconds)
	{
		return julianDateTai - (taiMinusUtcSeconds / SecondsOfDay);
	}

	/// <summary>Converts TAI Julian Date to Terrestrial Time (TT).</summary>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <returns>TT Julian Date.</returns>
	public static double ConvertTaiToTt(double julianDateTai)
	{
		return julianDateTai + (DoubleTtTaiOffsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts Terrestrial Time (TT) Julian Date to TAI.</summary>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <returns>TAI Julian Date.</returns>
	public static double ConvertTtToTai(double julianDateTt)
	{
		return julianDateTt - (DoubleTtTaiOffsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts UTC Julian Date to TT using the supplied TAI-UTC offset.</summary>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds.</param>
	/// <returns>TT Julian Date.</returns>
	public static double ConvertUtcToTt(double julianDateUtc, double taiMinusUtcSeconds)
	{
		return ConvertTaiToTt(ConvertUtcToTai(julianDateUtc, taiMinusUtcSeconds));
	}

	/// <summary>Converts TT Julian Date to UTC using the supplied TAI-UTC offset.</summary>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <param name="taiMinusUtcSeconds">TAI minus UTC, in seconds.</param>
	/// <returns>UTC Julian Date.</returns>
	public static double ConvertTtToUtc(double julianDateTt, double taiMinusUtcSeconds)
	{
		return ConvertTaiToUtc(ConvertTtToTai(julianDateTt), taiMinusUtcSeconds);
	}

	/// <summary>Converts UTC Julian Date to UT1 using DUT1.</summary>
	/// <remarks>DUT1 is UT1 minus UTC and must be obtained from Earth orientation data for the requested date.</remarks>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="dut1Seconds">UT1 minus UTC, in seconds.</param>
	/// <returns>UT1 Julian Date.</returns>
	public static double ConvertUtcToUt1(double julianDateUtc, double dut1Seconds)
	{
		return julianDateUtc + (dut1Seconds / SecondsOfDay);
	}

	/// <summary>Converts UT1 Julian Date to UTC using DUT1.</summary>
	/// <param name="julianDateUt1">UT1 Julian Date.</param>
	/// <param name="dut1Seconds">UT1 minus UTC, in seconds.</param>
	/// <returns>UTC Julian Date.</returns>
	public static double ConvertUt1ToUtc(double julianDateUt1, double dut1Seconds)
	{
		return julianDateUt1 - (dut1Seconds / SecondsOfDay);
	}

	/// <summary>Approximates UT2 from UT1 using the conventional seasonal correction.</summary>
	/// <remarks>UT2 is a historical smoothed form of UT1. This low-order periodic approximation is not a substitute for official historical UT2 data.</remarks>
	/// <param name="julianDateUt1">UT1 Julian Date.</param>
	/// <returns>Approximate UT2 Julian Date.</returns>
	public static double ConvertUt1ToUt2(double julianDateUt1)
	{
		DateTime date = ConvertJulianDateToCivilCalendar(julianDateUt1);
		double fractionalYear = (date.DayOfYear - 1 + date.TimeOfDay.TotalDays) / (DateTime.IsLeapYear(date.Year) ? 366.0 : 365.0);
		double angle = 2.0 * Math.PI * fractionalYear;
		double correctionSeconds = (0.022 * Math.Sin(angle)) - (0.012 * Math.Cos(angle)) - (0.006 * Math.Sin(2.0 * angle)) + (0.007 * Math.Cos(2.0 * angle));
		return julianDateUt1 + (correctionSeconds / SecondsOfDay);
	}

	/// <summary>Converts TT Julian Date to TCG using the IAU linear transformation.</summary>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <returns>TCG Julian Date.</returns>
	public static double ConvertTtToTcg(double julianDateTt)
	{
		double referenceMjd = 43144.0;
		double elapsedDays = julianDateTt - DoubleModifiedJulianDateCoefficient - referenceMjd;
		return julianDateTt + (DoubleLg * elapsedDays);
	}

	/// <summary>Converts TCG Julian Date to TT using the IAU linear transformation.</summary>
	/// <param name="julianDateTcg">TCG Julian Date.</param>
	/// <returns>TT Julian Date.</returns>
	public static double ConvertTcgToTt(double julianDateTcg)
	{
		double referenceMjd = 43144.0;
		double elapsedDays = julianDateTcg - DoubleModifiedJulianDateCoefficient - referenceMjd;
		return julianDateTcg - (DoubleLg * elapsedDays / (1.0 + DoubleLg));
	}

	/// <summary>Approximates TDB from TT using the leading periodic terms of the solar-system time transformation.</summary>
	/// <remarks>This compact approximation has millisecond-level accuracy at best. Precision ephemeris work should use an IAU SOFA/ERFA implementation or a full Fairhead-Bretagnon model.</remarks>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <returns>Approximate TDB Julian Date.</returns>
	public static double ConvertTtToTdb(double julianDateTt)
	{
		double t = (julianDateTt - DoubleJ2000EpochJulianDateCoefficient) / 36525.0;
		double meanAnomaly = (357.5277233 + (35999.05034 * t)) * (Math.PI / 180.0);
		double correctionSeconds = (0.001657 * Math.Sin(meanAnomaly)) + (0.000022 * Math.Sin(2.0 * meanAnomaly));
		return julianDateTt + (correctionSeconds / SecondsOfDay);
	}

	/// <summary>Approximates TT from TDB by iteratively reversing the leading periodic correction.</summary>
	/// <param name="julianDateTdb">TDB Julian Date.</param>
	/// <returns>Approximate TT Julian Date.</returns>
	public static double ConvertTdbToTt(double julianDateTdb)
	{
		double result = julianDateTdb;
		for (int i = 0; i < 3; i++)
		{
			result = julianDateTdb - (ConvertTtToTdb(result) - result);
		}

		return result;
	}

	/// <summary>Converts TCB Julian Date to TDB using the IAU 2006 linear transformation.</summary>
	/// <param name="julianDateTcb">TCB Julian Date.</param>
	/// <returns>TDB Julian Date.</returns>
	public static double ConvertTcbToTdb(double julianDateTcb)
	{
		double offsetSeconds = (-DoubleLb * (julianDateTcb - DoubleTcbReferenceEpoch) * SecondsOfDay) + DoubleTdbAtT0Seconds;
		return julianDateTcb + (offsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts TDB Julian Date to TCB using the inverse IAU 2006 linear transformation.</summary>
	/// <param name="julianDateTdb">TDB Julian Date.</param>
	/// <returns>TCB Julian Date.</returns>
	public static double ConvertTdbToTcb(double julianDateTdb)
	{
		double numerator = julianDateTdb - DoubleTcbReferenceEpoch - (DoubleTdbAtT0Seconds / SecondsOfDay);
		return DoubleTcbReferenceEpoch + (numerator / (1.0 - DoubleLb));
	}

	/// <summary>Converts GPS Julian Date to TAI Julian Date.</summary>
	/// <param name="julianDateGps">GPS Julian Date.</param>
	/// <returns>TAI Julian Date.</returns>
	public static double ConvertGpsToTai(double julianDateGps)
	{
		return julianDateGps + (DoubleGpsTaiOffsetSeconds / SecondsOfDay);
	}

	/// <summary>Converts TAI Julian Date to GPS Julian Date.</summary>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <returns>GPS Julian Date.</returns>
	public static double ConvertTaiToGps(double julianDateTai)
	{
		return julianDateTai - (DoubleGpsTaiOffsetSeconds / SecondsOfDay);
	}

	/// <summary>Calculates the Barycentric Julian Date (BJD) using a geometric light-time correction.</summary>
	/// <remarks>The observer position relative to the solar-system barycenter and the target unit direction must use the same inertial frame and AU units. This geometric correction does not include Shapiro delay or other relativistic terms.</remarks>
	/// <param name="julianDate">Input Julian Date on the explicitly selected time scale.</param>
	/// <param name="observerBarycentricX">Observer barycentric X coordinate in AU.</param>
	/// <param name="observerBarycentricY">Observer barycentric Y coordinate in AU.</param>
	/// <param name="observerBarycentricZ">Observer barycentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>Geometrically corrected BJD on the same time scale as the input.</returns>
	public static double CalculateBarycentricJulianDate(double julianDate, double observerBarycentricX, double observerBarycentricY, double observerBarycentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		double dotProduct = (observerBarycentricX * targetUnitX) + (observerBarycentricY * targetUnitY) + (observerBarycentricZ * targetUnitZ);
		return julianDate + (dotProduct / DoubleSpeedOfLightAuPerDay);
	}

	/// <summary>Calculates BJD(UTC) using a geometric barycentric light-time correction.</summary>
	/// <remarks>The result remains on the UTC scale; this method does not convert UTC to TDB.</remarks>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="observerBarycentricX">Observer barycentric X coordinate in AU.</param>
	/// <param name="observerBarycentricY">Observer barycentric Y coordinate in AU.</param>
	/// <param name="observerBarycentricZ">Observer barycentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>BJD(UTC).</returns>
	public static double CalculateBarycentricJulianDateUtc(double julianDateUtc, double observerBarycentricX, double observerBarycentricY, double observerBarycentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateBarycentricJulianDate(julianDate: julianDateUtc, observerBarycentricX: observerBarycentricX, observerBarycentricY: observerBarycentricY, observerBarycentricZ: observerBarycentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates BJD(TT) using a geometric barycentric light-time correction.</summary>
	/// <remarks>The input must already be expressed in TT. The geometric correction does not include Shapiro delay.</remarks>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <param name="observerBarycentricX">Observer barycentric X coordinate in AU.</param>
	/// <param name="observerBarycentricY">Observer barycentric Y coordinate in AU.</param>
	/// <param name="observerBarycentricZ">Observer barycentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>BJD(TT).</returns>
	public static double CalculateBarycentricJulianDateTt(double julianDateTt, double observerBarycentricX, double observerBarycentricY, double observerBarycentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateBarycentricJulianDate(julianDate: julianDateTt, observerBarycentricX: observerBarycentricX, observerBarycentricY: observerBarycentricY, observerBarycentricZ: observerBarycentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates BJD(TDB) using a geometric barycentric light-time correction.</summary>
	/// <remarks>The input must already be expressed in TDB. The geometric correction does not include Shapiro delay.</remarks>
	/// <param name="julianDateTdb">TDB Julian Date.</param>
	/// <param name="observerBarycentricX">Observer barycentric X coordinate in AU.</param>
	/// <param name="observerBarycentricY">Observer barycentric Y coordinate in AU.</param>
	/// <param name="observerBarycentricZ">Observer barycentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>BJD(TDB).</returns>
	public static double CalculateBarycentricJulianDateTdb(double julianDateTdb, double observerBarycentricX, double observerBarycentricY, double observerBarycentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateBarycentricJulianDate(julianDate: julianDateTdb, observerBarycentricX: observerBarycentricX, observerBarycentricY: observerBarycentricY, observerBarycentricZ: observerBarycentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates HJD(UTC) using a geometric heliocentric light-time correction.</summary>
	/// <param name="julianDateUtc">UTC Julian Date.</param>
	/// <param name="earthHeliocentricX">Earth heliocentric X coordinate in AU.</param>
	/// <param name="earthHeliocentricY">Earth heliocentric Y coordinate in AU.</param>
	/// <param name="earthHeliocentricZ">Earth heliocentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>HJD(UTC).</returns>
	public static double CalculateHeliocentricJulianDateUtc(double julianDateUtc, double earthHeliocentricX, double earthHeliocentricY, double earthHeliocentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateHeliocentricJulianDate(julianDate: julianDateUtc, earthVectorX: earthHeliocentricX, earthVectorY: earthHeliocentricY, earthVectorZ: earthHeliocentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates HJD(TT) using a geometric heliocentric light-time correction.</summary>
	/// <param name="julianDateTt">TT Julian Date.</param>
	/// <param name="earthHeliocentricX">Earth heliocentric X coordinate in AU.</param>
	/// <param name="earthHeliocentricY">Earth heliocentric Y coordinate in AU.</param>
	/// <param name="earthHeliocentricZ">Earth heliocentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>HJD(TT).</returns>
	public static double CalculateHeliocentricJulianDateTt(double julianDateTt, double earthHeliocentricX, double earthHeliocentricY, double earthHeliocentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateHeliocentricJulianDate(julianDate: julianDateTt, earthVectorX: earthHeliocentricX, earthVectorY: earthHeliocentricY, earthVectorZ: earthHeliocentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates HJD(TDB) using a geometric heliocentric light-time correction.</summary>
	/// <param name="julianDateTdb">TDB Julian Date.</param>
	/// <param name="earthHeliocentricX">Earth heliocentric X coordinate in AU.</param>
	/// <param name="earthHeliocentricY">Earth heliocentric Y coordinate in AU.</param>
	/// <param name="earthHeliocentricZ">Earth heliocentric Z coordinate in AU.</param>
	/// <param name="targetUnitX">Target unit-vector X component.</param>
	/// <param name="targetUnitY">Target unit-vector Y component.</param>
	/// <param name="targetUnitZ">Target unit-vector Z component.</param>
	/// <returns>HJD(TDB).</returns>
	public static double CalculateHeliocentricJulianDateTdb(double julianDateTdb, double earthHeliocentricX, double earthHeliocentricY, double earthHeliocentricZ, double targetUnitX, double targetUnitY, double targetUnitZ)
	{
		return CalculateHeliocentricJulianDate(julianDate: julianDateTdb, earthVectorX: earthHeliocentricX, earthVectorY: earthHeliocentricY, earthVectorZ: earthHeliocentricZ, targetUnitX: targetUnitX, targetUnitY: targetUnitY, targetUnitZ: targetUnitZ);
	}

	/// <summary>Calculates a mission-specific planetary sol number.</summary>
	/// <remarks>Mission sol numbering is not globally standardized. Supply the mission's documented epoch and the adopted sol length.</remarks>
	/// <param name="julianDate">Input Julian Date.</param>
	/// <param name="missionEpochJulianDate">Julian Date of mission sol zero.</param>
	/// <param name="solLengthDays">Length of one local planetary sol in Earth days.</param>
	/// <returns>Zero-based fractional mission sol number.</returns>
	public static double CalculatePlanetarySolDate(double julianDate, double missionEpochJulianDate, double solLengthDays)
	{
		return (julianDate - missionEpochJulianDate) / solLengthDays;
	}

	/// <summary>Calculates a zero-based mission sol number from a planetary sol date.</summary>
	/// <param name="planetarySolDate">Fractional planetary sol date.</param>
	/// <returns>Integer sol number.</returns>
	public static long CalculatePlanetarySolNumber(double planetarySolDate)
	{
		return (long)Math.Floor(d: planetarySolDate);
	}

	/// <summary>Calculates a mission-specific Mars Sol Number (MSN).</summary>
	/// <remarks>MSN is mission-dependent and must not be confused with the continuous Mars Sol Date (MSD).</remarks>
	/// <param name="julianDate">Input Julian Date.</param>
	/// <param name="missionEpochJulianDate">Julian Date of mission sol zero.</param>
	/// <returns>Zero-based integer mission sol number.</returns>
	public static long CalculateMarsSolNumber(double julianDate, double missionEpochJulianDate)
	{
		return (long)Math.Floor(d: (julianDate - missionEpochJulianDate) / DoubleMarsSolLengthDays);
	}

	/// <summary>Calculates Mars Coordinated Time (MTC) in decimal Martian hours.</summary>
	/// <param name="marsSolDate">Mars Sol Date.</param>
	/// <returns>Time of day in the range [0, 24) Martian hours.</returns>
	public static double CalculateMarsCoordinatedTime(double marsSolDate)
	{
		double fraction = marsSolDate - Math.Floor(marsSolDate);
		return fraction * 24.0;
	}

	/// <summary>Calculates areocentric solar longitude (Ls) from the Martian true anomaly.</summary>
	/// <remarks>For the conventional orbital-element definition, Ls is the sum of true anomaly and longitude of perihelion, normalized to [0, 360) degrees.</remarks>
	/// <param name="trueAnomalyDegrees">Mars true anomaly in degrees.</param>
	/// <param name="longitudeOfPerihelionDegrees">Mars longitude of perihelion in degrees, in the same reference frame.</param>
	/// <returns>Areocentric solar longitude in degrees.</returns>
	public static double CalculateAreocentricSolarLongitude(double trueAnomalyDegrees, double longitudeOfPerihelionDegrees)
	{
		double solarLongitude = trueAnomalyDegrees + longitudeOfPerihelionDegrees;
		return ((solarLongitude % 360.0) + 360.0) % 360.0;
	}

	/// <summary>Converts a Julian Date to CCSDS Day Segmented Time (CDS) components.</summary>
	/// <remarks>The returned day count is relative to the CCSDS epoch. The sub-day component is expressed in milliseconds; higher-resolution CCSDS variants may use finer fields.</remarks>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <returns>A tuple containing elapsed days and milliseconds within the day.</returns>
	public static (long Days, uint Milliseconds) ConvertJulianDateToCcsdsDaySegmentedTime(double julianDateTai)
	{
		double elapsedDays = julianDateTai - DoubleCcsdsEpoch;
		long days = (long)Math.Floor(elapsedDays);
		uint milliseconds = (uint)Math.Floor((elapsedDays - days) * SecondsOfDay * 1000.0);
		return (days, milliseconds);
	}

	/// <summary>Converts a Julian Date to CCSDS Unsegmented Time Code (CUC) components.</summary>
	/// <remarks>The result is a numeric coarse/fine pair. The actual byte encoding depends on the mission-selected CUC epoch and field widths.</remarks>
	/// <param name="julianDateTai">TAI Julian Date.</param>
	/// <param name="epochJulianDate">Mission-defined CUC epoch as a Julian Date.</param>
	/// <returns>Elapsed whole seconds and fractional second.</returns>
	public static (long CoarseSeconds, double FineSeconds) ConvertJulianDateToCcsdsUnsegmentedTime(double julianDateTai, double epochJulianDate)
	{
		double elapsedSeconds = (julianDateTai - epochJulianDate) * SecondsOfDay;
		long coarseSeconds = (long)Math.Floor(elapsedSeconds);
		return (coarseSeconds, elapsedSeconds - coarseSeconds);
	}

	#endregion

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

	/// <summary>Converts a TT Julian Date to Mars Sol Date using the conventional Martian sol length.</summary>
	/// <remarks>The input is interpreted as TT. Use the UTC overload of CalculateMarsSolDate when a leap-second offset is available.</remarks>
	/// <param name="julianDate">Julian Date</param>
	/// <returns>Mars Sol Date</returns>
	public static double ConvertJulianDateToMarsSolDate(double julianDate)
	{
		return (julianDate - DoubleMarsSolDateCoefficient) / DoubleMarsSolLengthDays;
	}

	/// <summary>Converts a Julian Date to Unix Time (days since 1970-01-01). The result is obtained by subtracting the Unix Time coefficient from the given Julian Date.</summary>
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

	/// <summary>Converts Mars Sol Date to a TT Julian Date.</summary>
	/// <param name="marsSolDate">Mars Sol Date.</param>
	/// <returns>TT Julian Date.</returns>
	public static double ConvertMarsSolDateToJulianDate(double marsSolDate)
	{
		return DoubleMarsSolDateCoefficient + (marsSolDate * DoubleMarsSolLengthDays);
	}

	/// <summary>Converts a normalized Julian Date to Julian Date.</summary>
	/// <param name="normalizedJulianDate">Normalized Julian Date.</param>
	/// <param name="referenceEpoch">Reference epoch expressed as a Julian Date.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertNormalizedJulianDateToJulianDate(double normalizedJulianDate, double referenceEpoch)
	{
		return ConvertOffsetJulianDateToJulianDate(offsetJulianDate: normalizedJulianDate, referenceEpoch: referenceEpoch);
	}

	/// <summary>Converts a Julian Date to the J2000 date day count.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>Days from JD 2451545.0.</returns>
	public static double ConvertJulianDateToJ2000Date(double julianDate)
	{
		return CalculateJ2000Date(julianDate: julianDate);
	}

	/// <summary>Converts a Julian Date to MJD2000.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <returns>J2000-based day count.</returns>
	public static double ConvertJulianDateToModifiedJulianDate2000(double julianDate)
	{
		return CalculateModifiedJulianDate2000(julianDate: julianDate);
	}

	/// <summary>Converts a J2000 date day count to Julian Date.</summary>
	/// <param name="j2000Date">Days from JD 2451545.0.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertJ2000DateToJulianDate(double j2000Date)
	{
		return j2000Date + DoubleJ2000DateEpoch;
	}

	/// <summary>Converts MJD2000 to Julian Date.</summary>
	/// <param name="mjd2000">J2000-based day count.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertModifiedJulianDate2000ToJulianDate(double mjd2000)
	{
		return mjd2000 + DoubleJ2000DateEpoch;
	}

	/// <summary>Converts an offset Julian Date to Julian Date.</summary>
	/// <param name="offsetJulianDate">Days since the reference epoch.</param>
	/// <param name="referenceEpoch">Reference epoch as Julian Date.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertOffsetJulianDateToJulianDate(double offsetJulianDate, double referenceEpoch)
	{
		return offsetJulianDate + referenceEpoch;
	}

	/// <summary>Converts a Julian Century to Julian Date.</summary>
	/// <param name="julianCentury">Julian centuries from J2000.0.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertJulianCenturyToJulianDate(double julianCentury)
	{
		return DoubleJ2000EpochJulianDateCoefficient + (julianCentury * 36525.0);
	}

	/// <summary>Converts a Julian Millennium to Julian Date.</summary>
	/// <param name="julianMillennium">Julian millennia from J2000.0.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertJulianMillenniumToJulianDate(double julianMillennium)
	{
		return DoubleJ2000EpochJulianDateCoefficient + (julianMillennium * 365250.0);
	}

	/// <summary>Converts a decimal Gregorian year to Julian Date.</summary>
	/// <param name="decimalYear">Decimal Gregorian year.</param>
	/// <returns>Julian Date.</returns>
	public static double ConvertDecimalYearToJulianDate(double decimalYear)
	{
		int year = (int)Math.Floor(d: decimalYear);
		DateTime start = new(year: year, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);
		DateTime end = start.AddYears(value: 1);
		return CalculateJulianDate(date: start) + ((decimalYear - year) * (end - start).TotalDays);
	}

	/// <summary>Converts a Julian Date to the number of elapsed seconds since a specified epoch.</summary>
	/// <param name="julianDate">Julian Date.</param>
	/// <param name="epochJulianDate">Reference epoch as Julian Date.</param>
	/// <returns>Elapsed seconds.</returns>
	public static double ConvertJulianDateToElapsedSeconds(double julianDate, double epochJulianDate)
	{
		return (julianDate - epochJulianDate) * SecondsOfDay;
	}

	#endregion
}
