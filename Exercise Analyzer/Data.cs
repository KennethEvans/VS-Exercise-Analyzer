using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Exercise_Analyzer {
    public class ExerciseData {
        public static readonly String NL = Environment.NewLine;

        public string FileName { get; set; }
        public List<string> AssociatedFiles { get; set; } = new List<string>();
        public int NTracks { get; set; }
        public int NSegments { get; set; }
        public int NTrackPoints { get; set; }
        public int NHrValues { get; set; }
        public DateTime StartTime { get; set; } = DateTime.MinValue;
        public DateTime EndTime { get; set; } = DateTime.MinValue;
        public DateTime HrStartTime { get; set; } = DateTime.MinValue;
        public DateTime HrEndTime { get; set; } = DateTime.MinValue;
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
        public string Creator { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public double LatStart { get; set; } = Double.NaN;
        public double LatMax { get; set; } = -Double.MaxValue;
        public double LatMin { get; set; } = Double.MaxValue;
        public double LonStart { get; set; } = Double.NaN;
        public double LonMax { get; set; } = -Double.MaxValue;
        public double LonMin { get; set; } = Double.MaxValue;
        public double EleStart { get; set; } = Double.NaN;
        public double EleMax { get; set; } = -Double.MaxValue;
        public double EleMin { get; set; } = Double.MaxValue;
        public double SpeedAvg { get; set; }
        public double SpeedAvgSimple { get; set; }
        public double SpeedMax { get; set; }
        public double SpeedMin { get; set; }
        public double HrAvg { get; set; } = 0;
        public int HrMax { get; set; } = Int32.MinValue;
        public int HrMin { get; set; } = Int32.MaxValue;

        public static ExerciseData processTcx(string fileName) {
            ExerciseData data = new ExerciseData();
            data.FileName = fileName;
            data.AssociatedFiles.Add(fileName);

            XDocument doc = XDocument.Load(fileName);
            XElement tcx = doc.Root;

            IEnumerable<XElement> activities =
                from item in tcx.Descendants()
                where item.Name.LocalName == "Activity"
                select item;

            // Loop over Activities, Laps, Tracks, and Trackpoints
            int nAct = 0, nLaps = 0, nTrks = 0, nTpts = 0, nHr = 0;
            double lat, lon, ele, distance = 0, hrSum = 0, hrMax = 0;
            double latPrev = Double.NaN, lonPrev = Double.NaN;
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
                        nTrks++;
                        IEnumerable<XElement> tpts =
                           from item in trk.Elements()
                           where item.Name.LocalName == "Trackpoint"
                           select item;
                        latPrev = lonPrev = Double.NaN;
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
                                    time = (DateTime)elem;
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
                            }
                            if (!Double.IsNaN(ele)) {
                                if (Double.IsNaN(data.EleStart)) {
                                    data.EleStart = ele;
                                }
                                if (ele > data.EleMax) data.EleMax = ele;
                                if (ele < data.EleMin) data.EleMin = ele;
                            }
                            if (hr > 0) {
                                if (time != DateTime.MinValue) {
                                    if (data.HrStartTime == DateTime.MinValue) {
                                        data.HrStartTime = time;
                                    }
                                    data.HrEndTime = time;
                                }
                                nHr++;
                                hrSum += hr;
                                if (hr > data.HrMax) data.HrMax = hr;
                                if (hr < data.HrMin) data.HrMin = hr;
                            }
                            if (!Double.IsNaN(lat) && !Double.IsNaN(lon)) {
                                if (!Double.IsNaN(latPrev) && !Double.IsNaN(lonPrev)) {
                                    distance += GpsUtils.greatCircleDistance(latPrev, lonPrev, lat, lon);
                                }
                                latPrev = lat;
                                lonPrev = lon;
                            }
                        }
                    }
                }
            }
            data.NTracks = nLaps;
            data.NSegments = nTrks;
            data.NTrackPoints = nTpts;
            data.NHrValues = nHr;
            if (nHr > 0) {
                data.HrAvg = hrSum / nHr;
            }
            data.Distance = distance;
            if (data.StartTime != DateTime.MinValue && data.EndTime != DateTime.MinValue) {
                TimeSpan duration = data.EndTime - data.StartTime;
                data.Duration = duration;
                if (duration.TotalMilliseconds > 0) {
                    data.SpeedAvg = 1000 * distance / duration.TotalMilliseconds;
                }
            }
            data.setLocationAndCategoryFromFileName(fileName);

            return data;
        }

        public static ExerciseData processGpx(string fileName) {
            ExerciseData data = new ExerciseData();
            data.FileName = fileName;
            data.AssociatedFiles.Add(fileName);

            XDocument doc = XDocument.Load(fileName);
            XElement gpx = doc.Root;

            foreach (XAttribute attr in gpx.Attributes()) {
                if (attr.Name == "creator") {
                    data.Creator = attr.Value; ;
                }
            }


            // STL files hve this information
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
            int nSegs = 0, nTrks = 0, nTpts = 0, nHr = 0;
            double lat, lon, ele, distance = 0, hrSum = 0, hrMax = 0;
            double latPrev = Double.NaN, lonPrev = Double.NaN;
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
                    IEnumerable<XElement> tpts =
                       from item in seg.Elements()
                       where item.Name.LocalName == "trkpt"
                       select item;
                    latPrev = lonPrev = Double.NaN;
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
                                time = (DateTime)elem;
                            }
                        }
                        foreach (XElement elem in from item in tpt.Descendants()
                                                  select item) {
                            if (elem.Name == "hr") {
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
                        }
                        if (!Double.IsNaN(ele)) {
                            if (Double.IsNaN(data.EleStart)) {
                                data.EleStart = ele;
                            }
                            if (ele > data.EleMax) data.EleMax = ele;
                            if (ele < data.EleMin) data.EleMin = ele;
                        }
                        if (hr > 0) {
                            if (time != DateTime.MinValue) {
                                if (data.HrStartTime == DateTime.MinValue) {
                                    data.HrStartTime = time;
                                }
                                data.HrEndTime = time;
                            }
                            nHr++;
                            hrSum += hr;
                            if (hr > data.HrMax) data.HrMax = hr;
                            if (hr < data.HrMin) data.HrMin = hr;
                        }
                        if (!Double.IsNaN(lat) && !Double.IsNaN(lon)) {
                            if (!Double.IsNaN(latPrev) && !Double.IsNaN(lonPrev)) {
                                distance += GpsUtils.greatCircleDistance(latPrev, lonPrev, lat, lon);
                            }
                            latPrev = lat;
                            lonPrev = lon;
                        }
                    }
                }
            }
            data.NTracks = nSegs;
            data.NSegments = nTrks;
            data.NTrackPoints = nTpts;
            data.NHrValues = nHr;
            if (nHr > 0) {
                data.HrAvg = hrSum / nHr;
            }
            data.Distance = distance;
            if (data.StartTime != DateTime.MinValue && data.EndTime != DateTime.MinValue) {
                TimeSpan duration = data.EndTime - data.StartTime;
                data.Duration = duration;
                if (duration.TotalMilliseconds > 0) {
                    data.SpeedAvg = 1000 * distance / duration.TotalMilliseconds;
                }
            }
            data.setLocationAndCategoryFromFileName(fileName);

            return data;
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
                    String.Format(eleFormat, EleStart, EleMin, EleMax,
                    EleMax - EleStart, EleStart - EleMin) :
                    "Elevation: No elevation data") + NL;
            string boundsFormat = "Bounds: LatMin={0:f6} LatMax=={1:f6} LonMin={2:f6} LonMax={3:f6}";
            info += (!Double.IsNaN(LatStart) ?
                    String.Format(boundsFormat, LatMin, LatMax, LonMin, LonMax) :
                    "Bounds: No location data") + NL;

            // Strip the final NL
            info = info.Substring(0, info.Length - NL.Length);

            return info;
        }

        public class SaveSet {
            public List<ExerciseData> ExerciseData { get; set; }

            public SaveSet(List<ExerciseData> exerciseData) {
                ExerciseData = exerciseData;
            }
        }
    }
}
