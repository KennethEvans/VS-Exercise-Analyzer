#define convertTimeToUTC

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GeoTimeZone;
using Newtonsoft.Json;
using TimeZoneConverter;
using TimeZoneNames;

namespace Exercise_Analyzer {
    public class ExerciseData {
        public static readonly String NL = Environment.NewLine;
        public const int START_TIME_THRESHOLD_SECONDS = 300;
        public static readonly double NO_MOVE_SPEED = GpsUtils.NO_MOVE_SPEED;

        public string FileName { get; set; }
        public int NTracks { get; set; }
        public int NSegments { get; set; }
        public int NTrackPoints { get; set; }
        public int NHrValues { get; set; }
        public string TZId { get; set; } = null;
        public bool TZInfoFromLatLon { get; set; } = false;
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public DateTime HrStartTime { get; set; } = DateTime.MinValue;
        public DateTime HrEndTime { get; set; } = DateTime.MinValue;
        public double Distance { get; set; }  // m
        public TimeSpan Duration { get; set; }
        public TimeSpan HrDuration { get; set; }
        public string Creator { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public double LatStart { get; set; } = Double.NaN;
        public double LatMax { get; set; } = -Double.MaxValue;
        public double LatMin { get; set; } = Double.MaxValue;
        public double LonStart { get; set; } = Double.NaN;
        public double LonMax { get; set; } = -Double.MaxValue;
        public double LonMin { get; set; } = Double.MaxValue;
        public double EleStart { get; set; } = Double.NaN; // m
        public double EleMax { get; set; } = -Double.MaxValue;  // m
        public double EleMin { get; set; } = Double.MaxValue;  // m
        public double EleLoss { get; set; }  // m
        public double EleGain { get; set; }  // m
        public double EleAvg { get; set; }  // m
        public double SpeedAvg { get; set; } // m/s
        public double SpeedAvgSimple { get; set; } // m/s
        public double SpeedAvgMoving { get; set; } // m/s
        public double SpeedMax { get; set; } // m/s
        public double SpeedMin { get; set; } // m/s
        public double HrAvg { get; set; } = 0;
        public int HrMax { get; set; } = Int32.MinValue;
        public int HrMin { get; set; } = Int32.MaxValue;

        public static ExerciseData processTcx(string fileName) {
            ExerciseData data = new ExerciseData();
            data.FileName = fileName;

            XDocument doc = XDocument.Load(fileName);
            XElement tcx = doc.Root;

            IEnumerable<XElement> activities =
                from item in tcx.Descendants()
                where item.Name.LocalName == "Activity"
                select item;

            // Loop over Activities, Laps, Tracks, and Trackpoints
            List<long> timeValsList = new List<long>();
            List<long> speedTimeValsList = new List<long>();
            List<Double> speedValsList = new List<double>();
            List<double> eleValsList = new List<double>();
            List<long> hrTimeValsList = new List<long>();
            List<double> hrValsList = new List<double>();
            double prevLat = 0, prevLon = 0;
            long startTime = long.MaxValue;
            long endTime = 0;
            double deltaLength, speed;
            double prevTime = -1;
            double deltaTime;
            long lastTimeValue = -1;

            int nAct = 0, nLaps = 0, nSegs = 0, nTpts = 0, nHr = 0;
            double lat, lon, ele, distance = 0, hrSum = 0;
            DateTime time;
            int hr;
            foreach (XElement activity in activities) {
                nAct++;
                foreach (XElement elem in activity.Descendants().
                    Where(p => p.Name.LocalName == "Name"
                    && p.Parent.Name.LocalName == "Creator")) {
                    data.Creator = elem.Value;
                }
                IEnumerable<XElement> laps =
                    from item in activity.Elements()
                    where item.Name.LocalName == "Lap"
                    select item;
                foreach (XElement lap in laps) {
                    nLaps++;
                    IEnumerable<XElement> trks =
                       from item in lap.Elements()
                       where item.Name.LocalName == "Track"
                       select item;
                    foreach (XElement trk in trks) {
                        nSegs++;
                        if (nSegs > 1) {
                            // Use NaN to make a break between segments
                            hrValsList.Add(Double.NaN);
                            hrTimeValsList.Add(lastTimeValue);
                            speedValsList.Add(Double.NaN);
                            speedTimeValsList.Add(lastTimeValue);
                            eleValsList.Add(Double.NaN);
                            timeValsList.Add(lastTimeValue);
                        }
                        IEnumerable<XElement> tpts =
                           from item in trk.Elements()
                           where item.Name.LocalName == "Trackpoint"
                           select item;
                        foreach (XElement tpt in tpts) {
                            nTpts++;
                            lat = lon = ele = Double.NaN;
                            hr = 0;
                            time = DateTime.MinValue;
                            foreach (XElement elem in tpt.Descendants()) {
                                if (elem.Name.LocalName == "LatitudeDegrees") {
                                    lat = (double)elem;
                                } else if (elem.Name.LocalName == "LongitudeDegrees") {
                                    lon = (double)elem;
                                } else if (elem.Name.LocalName == "AltitudeMeters") {
                                    ele = (double)elem;
                                } else if (elem.Name.LocalName == "Time") {
                                    // Fix for bad times in Polar GPX
                                    time = ((DateTime)elem).ToUniversalTime();
                                    if (time.Ticks < startTime) {
                                        startTime = time.Ticks;
                                    }
                                    if (time.Ticks > endTime) {
                                        endTime = time.Ticks;
                                    }
                                    timeValsList.Add(time.Ticks);
                                } else if (elem.Name.LocalName == "Value" &&
                                    elem.Parent.Name.LocalName == "HeartRateBpm") {
                                    hr = (int)elem;
                                }
                            }
                            if (time != DateTime.MinValue) {
                                if (data.StartTime == DateTime.MinValue) {
                                    data.StartTime = time;
                                }
                                data.EndTime = time;
                            }
                            if (!Double.IsNaN(lat) && !Double.IsNaN(lon)) {
                                if (Double.IsNaN(data.LatStart)) {
                                    data.LatStart = lat;
                                    data.LonStart = lon;
                                }
                                if (lat > data.LatMax) data.LatMax = lat;
                                if (lat < data.LatMin) data.LatMin = lat;
                                if (lon > data.LonMax) data.LonMax = lon;
                                if (lon < data.LonMin) data.LonMin = lon;
                                if (prevTime != -1) {
                                    // Should be the second track point
                                    deltaLength = GpsUtils.greatCircleDistance(
                                        prevLat, prevLon, lat, lon);
                                    distance += deltaLength;
                                    deltaTime = time.Ticks - prevTime;
                                    speed = deltaTime > 0
                                        ? TimeSpan.TicksPerSecond * deltaLength / deltaTime
                                        : 0;
                                    // Convert from m/sec to mi/hr
                                    speedValsList.Add(speed);
                                    speedTimeValsList
                                        .Add(time.Ticks - (long)Math.Round(.5 * deltaTime));
                                    if (Double.IsNaN(ele)) eleValsList.Add(0.0);
                                }
                                prevTime = time.Ticks;
                                prevLat = lat;
                                prevLon = lon;
                            }
                            if (!Double.IsNaN(ele)) {
                                if (Double.IsNaN(data.EleStart)) {
                                    data.EleStart = ele;
                                }
                                if (ele > data.EleMax) data.EleMax = ele;
                                if (ele < data.EleMin) data.EleMin = ele;
                                eleValsList.Add(ele);
                            }
                            if (hr > 0) {
                                if (time != DateTime.MinValue) {
                                    if (data.HrStartTime == DateTime.MinValue) {
                                        data.HrStartTime = time;
                                    }
                                    data.HrEndTime = time;
                                    hrSum += hr;
                                    nHr++;
                                    hrValsList.Add((double)hr);
                                    hrTimeValsList.Add(time.Ticks);
                                }
                            }
                        }
                    }
                }
            }

            // End of loops, process what was obtained
            data.Distance = distance;
            data.processValues(timeValsList, speedValsList, speedTimeValsList,
            eleValsList, hrValsList, hrTimeValsList, nLaps, nSegs, nTpts, nHr);
            return data;
        }

        public static ExerciseData processGpx(string fileName) {
            ExerciseData data = new ExerciseData();
            data.FileName = fileName;

            XDocument doc = XDocument.Load(fileName);
            XElement gpx = doc.Root;

            foreach (XAttribute attr in gpx.Attributes()) {
                if (attr.Name == "creator") {
                    data.Creator = attr.Value; ;
                }
            }


            // STL files have this information
            foreach (XElement elem in gpx.Elements().
                Where(p => p.Name.LocalName == "metadata")) {
                foreach (XElement elem1 in elem.Elements()) {
                    if (elem1.Name.LocalName == "category") {
                        data.Category = elem1.Value;
                    } else if (elem1.Name.LocalName == "location") {
                        data.Location = elem1.Value;
                    } else if (elem1.Name.LocalName == "author") {
                        foreach (XElement elem2 in elem1.Elements()) {
                            if (elem2.Name.LocalName == "name") { }
                            // Handle author;
                        }
                    }
                }
            }

            IEnumerable<XElement> trks =
                  from item in gpx.Elements()
                  where item.Name.LocalName == "trk"
                  select item;

            // Loop over Activities, Laps, Tracks, and Trackpoints
            // Loop over Activities, Laps, Tracks, and Trackpoints
            List<long> timeValsList = new List<long>();
            List<long> speedTimeValsList = new List<long>();
            List<Double> speedValsList = new List<double>();
            List<double> eleValsList = new List<double>();
            List<long> hrTimeValsList = new List<long>();
            List<double> hrValsList = new List<double>();
            double prevLat = 0, prevLon = 0;
            long startTime = long.MaxValue;
            long endTime = 0;
            double deltaLength, speed;
            double prevTime = -1;
            double deltaTime;
            long lastTimeValue = -1;

            int nSegs = 0, nTrks = 0, nTpts = 0, nHr = 0;
            double lat, lon, ele, distance = 0, hrSum = 0;
            DateTime time;
            int hr;
            foreach (XElement trk in trks) {
                nTrks++;
                IEnumerable<XElement> segs =
                    from item in trk.Elements()
                    where item.Name.LocalName == "trkseg"
                    select item;
                foreach (XElement seg in segs) {
                    nSegs++;
                    if (nSegs > 1) {
                        // Use NaN to make a break between segments
                        hrValsList.Add(Double.NaN);
                        hrTimeValsList.Add(lastTimeValue);
                        speedValsList.Add(Double.NaN);
                        speedTimeValsList.Add(lastTimeValue);
                        eleValsList.Add(Double.NaN);
                        timeValsList.Add(lastTimeValue);
                    }
                    IEnumerable<XElement> tpts =
                       from item in seg.Elements()
                       where item.Name.LocalName == "trkpt"
                       select item;
                    foreach (XElement tpt in tpts) {
                        nTpts++;
                        lat = lon = ele = Double.NaN;
                        hr = 0;
                        time = DateTime.MinValue;
                        foreach (XAttribute attr in tpt.Attributes()) {
                            if (attr.Name == "lat") {
                                lat = (double)attr;
                            } else if (attr.Name == "lon") {
                                lon = (double)attr;
                            }
                        }
                        foreach (XElement elem in from item in tpt.Elements()
                                                  select item) {
                            if (elem.Name.LocalName == "ele") {
                                ele = (double)elem;
                            } else if (elem.Name.LocalName == "time") {
                                // Fix for bad times in Polar GPX
                                time = ((DateTime)elem).ToUniversalTime();
                                if (time.Ticks < startTime) {
                                    startTime = time.Ticks;
                                }
                                if (time.Ticks > endTime) {
                                    endTime = time.Ticks;
                                }
                                timeValsList.Add(time.Ticks);
                            }
                        }
                        foreach (XElement elem in from item in tpt.Descendants()
                                                  select item) {
                            if (elem.Name.LocalName == "hr") {
                                hr = (int)elem;
                            }
                        }
                        if (time != DateTime.MinValue) {
                            if (data.StartTime == DateTime.MinValue) {
                                data.StartTime = time;
                            }
                            data.EndTime = time;
                        }
                        if (!Double.IsNaN(lat) && !Double.IsNaN(lon)) {
                            if (Double.IsNaN(data.LatStart)) {
                                data.LatStart = lat;
                                data.LonStart = lon;
                            }
                            if (lat > data.LatMax) data.LatMax = lat;
                            if (lat < data.LatMin) data.LatMin = lat;
                            if (lon > data.LonMax) data.LonMax = lon;
                            if (lon < data.LonMin) data.LonMin = lon;
                            if (prevTime != -1) {
                                // Should be the second track point
                                deltaLength = GpsUtils.greatCircleDistance(
                                    prevLat, prevLon, lat, lon);
                                distance += deltaLength;
                                deltaTime = time.Ticks - prevTime;
                                speed = deltaTime > 0
                                    ? TimeSpan.TicksPerSecond * deltaLength / deltaTime
                                    : 0;
                                // Convert from m/sec to mi/hr
                                speedValsList.Add(speed);
                                speedTimeValsList
                                    .Add(time.Ticks - (long)Math.Round(.5 * deltaTime));
                                if (Double.IsNaN(ele)) eleValsList.Add(0.0);
                            }
                            prevTime = time.Ticks;
                            prevLat = lat;
                            prevLon = lon;
                        }
                        if (!Double.IsNaN(ele)) {
                            if (Double.IsNaN(data.EleStart)) {
                                data.EleStart = ele;
                            }
                            if (ele > data.EleMax) data.EleMax = ele;
                            if (ele < data.EleMin) data.EleMin = ele;
                            eleValsList.Add(ele);
                        }
                        if (hr > 0) {
                            if (time != DateTime.MinValue) {
                                if (data.HrStartTime == DateTime.MinValue) {
                                    data.HrStartTime = time;
                                }
                                data.HrEndTime = time;
                                hrSum += hr;
                                nHr++;
                                hrValsList.Add((double)hr);
                                hrTimeValsList.Add(time.Ticks);
                            }
                        }
                    }
                }
            }

            // End of loops, process what was obtained
            data.Distance = distance;
            data.processValues(timeValsList, speedValsList, speedTimeValsList,
            eleValsList, hrValsList, hrTimeValsList, nTrks, nSegs, nTpts, nHr);
            return data;
        }

        private void processValues(List<long> timeValsList,
            List<double> speedValsList, List<long> speedTimeValsList,
            List<double> eleValsList,
            List<double> hrValsList, List<long> hrTimeValsList,
            int nTracks, int nSegments, int nTrackPoints, int nHrValues) {

            // Convert to arrays
            long[] timeVals = timeValsList.ToArray();
            double[] speedVals = speedValsList.ToArray();
            long[] speedTimeVals = speedTimeValsList.ToArray();
            double[] eleVals = eleValsList.ToArray();
            double[] hrVals = hrValsList.ToArray();
            long[] hrTimeVals = hrTimeValsList.ToArray();

            NTracks = nTracks;
            NSegments = nSegments;
            NTrackPoints = nTrackPoints;
            NHrValues = nHrValues;

            setLocationAndCategoryFromFileName(FileName);

            // Convert times to the time zone of the location
            if (!Double.IsNaN(LatStart) && !Double.IsNaN(LonStart) &&
                StartTime != DateTime.MinValue && EndTime != DateTime.MinValue) {
                try {
                    TZId = getTimeZoneIdForLocation(LatStart, LonStart);
                    TZInfoFromLatLon = true;
                } catch (Exception) {
                    TZId = TimeZoneInfo.Local.Id;
                }
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(TZId);
                StartTime = TimeZoneInfo.ConvertTimeFromUtc(StartTime, tzi);
                EndTime = TimeZoneInfo.ConvertTimeFromUtc(EndTime, tzi);
            } else {
                TZId = TimeZoneInfo.Local.Id;
            }
            int nTimeVals = timeVals.Length;

            if (StartTime != DateTime.MinValue && EndTime != DateTime.MinValue) {
                TimeSpan duration = EndTime - StartTime;
                Duration = duration;
                if (duration.TotalMilliseconds > 0) {
                    SpeedAvgSimple = 1000 * Distance / duration.TotalMilliseconds;
                }
            }

            // Process arrays
            // HR
            double[] stats = null, stats1 = null;
            if (hrVals.Length > 0 && HrStartTime != DateTime.MinValue &&
                HrEndTime != DateTime.MinValue) {
                HrDuration = HrEndTime - HrStartTime;
                stats = getTimeAverageStats(hrVals, hrTimeVals, 0);
                if (stats != null) {
                    HrMin = (int)Math.Round(stats[0]);
                    HrMax = (int)Math.Round(stats[1]);
                    HrAvg = stats[2];
                } else {
                    // Get simple average
                    stats = getSimpleStats(hrVals, hrTimeVals, 0);
                    if (stats != null) {
                        HrMin = (int)Math.Round(stats[0]);
                        HrMax = (int)Math.Round(stats[1]);
                        HrAvg = stats[2];
                    }
                }
            }

            // Speed
            if (speedVals.Length > 0) {
                stats = getTimeAverageStats(speedVals, speedTimeVals, 0);
                if (stats != null) {
                    SpeedMin = stats[0];
                    SpeedMax = stats[1];
                    SpeedAvg = stats[2];
                } else {
                    // Get simple average
                    stats = getSimpleStats(speedVals, speedTimeVals, 0);
                    if (stats != null) {
                        SpeedMin = stats[0];
                        SpeedMax = stats[1];
                        SpeedAvg = stats[2];
                    }
                }
                // Moving average
                stats = getTimeAverageStats(speedVals, speedTimeVals, NO_MOVE_SPEED);
                if (stats != null) {
                    SpeedAvgMoving = stats[2];
                } else {
                    // Get simple average
                    stats = getSimpleStats(speedVals, speedTimeVals, NO_MOVE_SPEED);
                    if (stats != null) {
                        SpeedAvgMoving = stats[2];
                    }
                }

            }

            // Elevation
            if (eleVals.Length > 0) {
                stats = getTimeAverageStats(eleVals, timeVals, 0);
                stats1 = getEleStats(eleVals, timeVals);
                if (stats != null) {
                    EleMin = stats[0];
                    EleMax = stats[1];
                    EleAvg = stats[2];
                }
                if (stats1 != null) {
                    EleGain = stats1[0];
                    EleLoss = stats1[1];
                }
            } else {
                // Get simple average
                stats = getSimpleStats(eleVals, timeVals, 0);
                if (stats != null) {
                    EleMin = stats[0];
                    EleMax = stats[1];
                    EleAvg = stats[2];
                }
            }
        }

        public void setLocationAndCategoryFromFileName(string fileName) {
            if (Creator.ToLower().Contains("polar")) {
                string name = Path.GetFileNameWithoutExtension(fileName);
                string[] tokens = name.Split('_');

                if (tokens.Length < 4) {
                    // Not the expected form for the filename
                    if (String.IsNullOrEmpty(Category)) Category = "Polar";
                    if (String.IsNullOrEmpty(Location)) Location = "Polar";
                    return;
                }
                string[] tokens1;
                int date = 0;
                int time = 0;
                // Look for the date and time tokens
                for (int i = 1; i < tokens.Length; i++) {
                    tokens1 = tokens[i].Split('-');
                    if (tokens1.Length == 3) {
                        if (date == 0) date = i;
                        else if (time == 0) {
                            time = i;
                            break;
                        }
                    }
                }
                if (time == 0 || time > tokens.Length - 3) {
                    // Date and time not found
                    if (String.IsNullOrEmpty(Category)) Category = "Polar";
                    if (String.IsNullOrEmpty(Location)) Location = "Polar";
                    return;
                }
                if (String.IsNullOrEmpty(Category)) Category = tokens[time + 1];
                // Assume Category has no _'s.  Then the rest is Location
                if (String.IsNullOrEmpty(Location)) {
                    Location = tokens[time + 2];
                }
                for (int i = time + 3; i < tokens.Length; i++) {
                    Location += " " + tokens[i];
                }
            }
        }


#if false
        /// <summary>
        /// Uses Geo Time Zone and Time Zone Converter NuGet packages.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="utcDate"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeForLocation(double lat, double lon, DateTime utcDate) {
            TimeZoneInfo tzInfo = GetTimeZoneInfoForLocation(lat, lon);
            DateTime convertedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzInfo);
            return convertedTime;
        }

        /// <summary>
        /// Uses Geo Time Zone and Time Zone Converter NuGet packages.
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="utcDate"></param>
        /// <returns></returns>
        public static TimeZoneInfo GetTimeZoneInfoForLocation(double lat, double lon) {
            string tzIana = TimeZoneLookup.GetTimeZone(lat, lon).Result;
            string tzId = TZConvert.IanaToWindows(tzIana);
            TimeZoneInfo tzInfo = TimeZoneInfo.FindSystemTimeZoneById(tzId);
            return tzInfo;
        }
#endif

        public static string getTimeZoneIdForLocation(double lat, double lon) {
            string tzIana = TimeZoneLookup.GetTimeZone(lat, lon).Result;
            string tzId = TZConvert.IanaToWindows(tzIana);
            return tzId;
        }

        /// <summary>
        /// Gets an info string for this ExerciseData.
        /// Uses Geo Time Zone and Time Zone Converter NuGet packages.
        /// </summary>
        /// <returns></returns>
        public string info() {
            if (String.IsNullOrEmpty(FileName)) {
                return "No fileName defined";
            }
            string info = FileName + NL;
            info += "Creator: " + Creator + NL;
            info += "Category: " + Category + NL;
            info += "Location: " + Location + NL;
            info += "NTracks=" + NTracks + " Nsegments=" + NSegments
                + " NTrackPoints=" + NTrackPoints + NL;
            if (TZId == null) {
                info += "TimeZone: " + "Not defined" + NL;
            } else {
                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(TZId);
                TimeZoneValues abbrs = TZNames.GetAbbreviationsForTimeZone(tzi.Id, "en-US");
                info += "TimeZone: " + tzi
                    + " " + (tzi.IsDaylightSavingTime(StartTime) ?
                    abbrs.Daylight : abbrs.Standard) + NL;
            }
            info += "TimeZoneInfoFromLatLon: " + TZInfoFromLatLon + NL;
            info += "Start Time: " + StartTime + " End Time: " + EndTime + NL;
            info += "Duration: " + Duration + NL;
            info += "Distance: " + String.Format("{0:f2} mi", GpsUtils.M2MI * Distance) + NL;
            info += "Average Speed: " + String.Format("{0:f2} mi/hr",
                GpsUtils.M2MI / GpsUtils.SEC2HR * SpeedAvg) + NL;
            string hrFormat = "Heart Rate: Avg={0:f1} Min={1:f0} Max={2:f0}";
            info += ((HrAvg > 0) ?
                String.Format(hrFormat, HrAvg, HrMin, HrMax) :
                "Heart Rate: No heart rate data") + NL;

            info += "NTracks= " + NTracks + " Nsegments=" + NSegments
                + " NTrackPoints=" + NTrackPoints + NL;
            string eleFormat = "Elevation: Start={0:f0} Min={1:f0} Max={2:f0} Gain={3:f0} Loss={4:f0} ft";
            info += (!Double.IsNaN(EleStart) ?
                    String.Format(eleFormat, GpsUtils.M2MI * EleStart,
                    GpsUtils.M2FT * EleMin, GpsUtils.M2MI * EleMax,
                    GpsUtils.M2FT * (EleMax - EleStart),
                    GpsUtils.M2FT * (EleStart - EleMin)) :
                    "Elevation: No elevation data") + NL;
            string boundsFormat = "Bounds: LatMin={0:f6} LatMax=={1:f6} LonMin={2:f6} LonMax={3:f6}";
            info += (!Double.IsNaN(LatStart) ?
                    String.Format(boundsFormat, LatMin, LatMax, LonMin, LonMax) :
                    "Bounds: No location data") + NL;

            // Strip the final NL
            info = info.Substring(0, info.Length - NL.Length);

            return info;
        }

        /**
         * Gets the statistics from the given values and time values by averaging
         * over the values, not over the actual time.
         * 
         * @param vals
         * @param timeVals
         * @return {min, max, avg} or null on error.
         */

        public static double[] getSimpleStats(double[] vals, long[] timeVals,
            double omitBelow) {
            // System.out.println("vals: " + vals.Length + ", timeVals: "
            // + timeVals.Length);
            if (vals.Length != timeVals.Length) {
                //Utils.errMsg("getSimpleStats: Array sizes (vals: " + vals.Length
                //    + ", timeVals: " + timeVals.Length + ") do not match");
                return null;
            }
            int len = vals.Length;
            if (len == 0) {
                return new double[] { 0, 0, 0 };
            }
            double max = -Double.MaxValue;
            double min = Double.MaxValue;
            double sum = 0;
            double val;
            int nVals = 0;
            for (int i = 0; i < len; i++) {
                val = vals[i];
                if (Double.IsNaN(val)) continue;
                if (val < omitBelow) continue;
                nVals++;
                sum += val;
                if (val > max) {
                    max = val;
                }
                if (val < min) {
                    min = val;
                }
            }
            if (nVals == 0) {
                return null;
            }
            sum /= nVals;
            return new double[] { min, max, sum };
        }

        /**
         * Gets the statistics from the given values and time values by averaging
         * over the values weighted by the time.
         * 
         * @param vals
         * @param timeVals
         * @return {min, max, avg} or null on error.
         */
        public static double[] getTimeAverageStats(double[] vals, long[] timeVals,
            double omitBelow) {
            // System.out.println("vals: " + vals.length + ", timeVals: "
            // + timeVals.Length);
            if (vals.Length != timeVals.Length) {
                //Utils
                //    .errMsg("getTimeAverageStats: Array sizes (vals: " + vals.Length
                //        + ", timeVals: " + timeVals.Length + ") do not match");
                return null;
            }
            int len = vals.Length;
            if (len == 0) {
                return new double[] { 0, 0, 0 };
            }
            if (len < 2) {
                return new double[] { vals[0], vals[0], vals[0] };
            }
            double max = -Double.MaxValue;
            double min = Double.MaxValue;
            double sum = 0;
            double val;
            // Check for NaN
            for (int i = 0; i < len; i++) {
                val = vals[i];
                if (Double.IsNaN(val)) {
                    return null;
                }
            }

            // Loop over values.
            double totalWeight = 0;
            double weight = 0; ;
            for (int i = 0; i < len; i++) {
                val = vals[i];
                if (Double.IsNaN(val)) continue;
                if (val < omitBelow) continue;
                if (i == 0) {
                    weight = .5 * (timeVals[i + 1] - timeVals[i]);
                } else if (i == len - 1) {
                    weight = .5 * (timeVals[i] - timeVals[i - 1]);
                } else {
                    weight = .5 * (timeVals[i] - timeVals[i - 1]);
                }
                totalWeight += weight;
                // Shoudn't happen
                sum += val * weight;
                if (val > max) {
                    max = val;
                }
                if (val < min) {
                    min = val;
                }
            }
            if (totalWeight == 0) {
                return null;
            }
            sum /= (totalWeight);
            return new double[] { min, max, sum };
        }

        /**
         * Gets the elevation statistics from the given values and time values.
         * These include elevation gain and loss.
         * 
         * @param vals
         * @param timeVals
         * @return {gain, loss} or null on error.
         */
        public static double[] getEleStats(double[] vals, long[] timeVals) {
            // System.out.println("vals: " + vals.Length + ", timeVals: "
            // + timeVals.Length);
            if (vals.Length != timeVals.Length) {
                //Utils.errMsg("getSimpleStats: Array sizes (vals: " + vals.Length
                //    + ", timeVals: " + timeVals.Length + ") do not match");
                return null;
            }
            int len = vals.Length;
            if (len == 0) {
                return new double[] { 0, 0 };
            }
            double gain = 0;
            double loss = 0;
            double val;
            int nVals = 0;
            for (int i = 1; i < len; i++) {
                val = vals[i] - vals[i - 1];
                if (Double.IsNaN(val)) continue;
                nVals++;
                if (val > 0) {
                    gain += val;
                } else if (val < 0) {
                    loss += -val;
                }
            }
            if (nVals == 0) {
                return null;
            }
            return new double[] { gain, loss };
        }

        public static string formatDuration(TimeSpan duration) {
            int days = duration.Days;
            int hours = duration.Hours;
            int minutes = duration.Minutes;
            int seconds = duration.Seconds;
            string val = "";
            if (days > 0) val += days + "d ";
            if (hours > 0) val += hours + "h ";
            if (minutes > 0) val += minutes + "m ";
            if (seconds > 0) val += seconds + "s ";
            return val;
        }

        public static string formatSpeed(double speed) {
            if (speed == 0) return "";
            return $"{GpsUtils.M2MI / GpsUtils.SEC2HR * speed:f2}";
        }

        public static string formatPaceSec(double speed) {
            if (speed == 0) return "";
            double pace = GpsUtils.SEC2HR / GpsUtils.M2MI * 3600 / speed;
            return $"{pace:f2}";
        }

        public static string formatPace(double speed) {
            if (speed == 0) return "";
            double pace = GpsUtils.SEC2HR / GpsUtils.M2MI * 3600 / speed;
            TimeSpan span =
                new TimeSpan((long)(Math.Round(pace * TimeSpan.TicksPerSecond)));
            return formatDuration(span);
        }

        public static string formatHeartRateAvg(double hr) {
            if (hr == 0) return "";
            return $"{hr:f1}";
        }

        public static string formatElevation(double elevation) {
            if (Double.IsNaN(elevation) ||
                elevation == -Double.MaxValue || elevation == Double.MaxValue) {
                return "";
            }
            return $"{GpsUtils.M2FT * elevation:f2}";
        }

        public static string formatTimeStl(DateTime time) {
            if (time == DateTime.MinValue) return "";
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string formatMonthStl(DateTime time) {
            if (time == DateTime.MinValue) return "";
            return (time.Month - 1).ToString();
        }

        [JsonIgnore]
        public string Source
        {
            get
            {
                if (String.IsNullOrEmpty(Creator)) {
                    return "NA";
                } else if (Creator.ToLower().Contains("polar")) {
                    return "Polar";
                } else if (Creator.ToLower().Contains("sportstracklive")) {
                    return "STL";
                } else if (Creator.ToLower().Contains("gpx inspector")) {
                    return "GPX Inspector";
                } else if (Creator.ToLower().Contains("gpslink")) {
                    return "GpsLink";
                } else if (Creator.ToLower().Contains("mapsource")) {
                    return "MapSource";
                } else {
                    return "Other";
                }
            }
        }

        [JsonIgnore]
        public DateTime StartTimeRounded
        {
            get
            {
                TimeSpan tolerance =
                    new TimeSpan(START_TIME_THRESHOLD_SECONDS *
                    TimeSpan.TicksPerSecond);
                var halfIntervalTicks = (tolerance.Ticks + 1) >> 1;

                return StartTime.AddTicks(halfIntervalTicks -
                    ((StartTime.Ticks + halfIntervalTicks) % tolerance.Ticks));
            }
        }

        [JsonIgnore]
        public string Extension
        {
            get
            {
                if (String.IsNullOrEmpty(FileName)) return null;
                return Path.GetExtension(FileName);
            }
        }

        [JsonIgnore]
        public string SimpleFileName
        {
            get
            {
                if (String.IsNullOrEmpty(FileName)) return null;
                return Path.GetFileName(FileName);
            }
        }

        [JsonIgnore]
        public string FileNameWithoutExtension
        {
            get
            {
                if (String.IsNullOrEmpty(FileName)) return null;
                return Path.GetFileNameWithoutExtension(FileName);
            }
        }

        [JsonIgnore]
        public bool IsTcx
        {
            get
            {
                return Path.GetExtension(FileName).ToLower() == ".tcx";
            }
        }

        [JsonIgnore]
        public bool IsGpx
        {
            get
            {
                return Path.GetExtension(FileName).ToLower() == ".gpx";
            }
        }
    }
}