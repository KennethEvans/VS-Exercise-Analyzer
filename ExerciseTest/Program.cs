#undef test
#define linqtoxsd

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml.Linq;
#if linqtoxsd
using www.topografix.com.GPX_1_1;
#else
using GPX_1_1;
#endif
using www.garmin.com.xmlschemas.TrainingCenterDatabase.v2;

namespace ExerciseTest {

    class Program {
        public static readonly string NL = Environment.NewLine;
        private const string NS_GPX_1_1 = "http://www.topografix.com/GPX/1/1";
        private const string NS_TrainingCenterDatabase_v2 = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2";
        private const string NS_TrackPointExtension_v1 = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1";
        private const string NS_TrackPointExtension_v2 = "http://www.garmin.com/xmlschemas/TrackPointExtension/v2";
        private const string TEST_GPX_FILE1 = @"C:\Users\evans\Documents\GPSLink\Test\Short.gpx";
        private const string TEST_GPX_FILE2 = @"C:\Users\evans\Documents\GPSLink\Test\Proud-Lake-Simplified.gpx";
        private const string TEST_GPX_FILE3 = @"C:\Users\evans\Documents\GPSLink\Test\Test-GPX-Inspector-TPV2.gpx";
        private const string TEST_GPX_FILE4 = @"C:\Users\evans\Documents\GPSLink\STL\track2014-06-30-Workout-Rehab-1475018-Combined.gpx";
        private const string TEST_TCX_FILE1 = @"C:\Users\evans\Documents\GPSLink\Test\Kenneth_Evans_2020-02-24_15-43-34_Walking_Milford.tcx";
        private const string TEST_TCX_FILE2 = @"C:\Users\evans\Documents\GPSLink\Test\Kenneth_Evans_2020-03-04_15-31-00_Test_TCDB2.tcx";
        private const string TEST_TCX_FILE3 = @"C:\Users\evans\Documents\GPSLink\Test\TCX-Lat-Lon-Ele.tcx";
        private static string gpxFileName = TEST_GPX_FILE2;
        private static string tcxFileName = TEST_TCX_FILE3;

        static void Main(string[] args) {
            Console.WriteLine("Exercise Test");
            gpxTest(gpxFileName);
            tcxTest(tcxFileName);

        }

#if linqtoxsd
        private static void gpxTest(string fileName) {
            Console.WriteLine(NL + "Exercise Test GPX");
            Console.WriteLine(fileName);

            gpx gpxType = gpx.Load(fileName);

#if false
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(gpxType), NS_GPX_1_1);
            FileStream fs = new FileStream(gpxFileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            gpxType gpxType = (gpxType)xmlSerializer.Deserialize(reader);
#endif

            Console.WriteLine("Version=" + gpxType.version);
            IList<trkType> tracks = gpxType.trk;
            extensionsType extensions;
            IEnumerable<XElement> extensionElements;
            int nTrk = 0, nSeg = 0, nTkpt = 0;
            string hr, cad;
            foreach (trkType trk in tracks) {
                Console.WriteLine("Track: " + nTrk++);
                nSeg = 0;
                foreach (trksegType seg in trk.trkseg) {
                    Console.WriteLine("Segment: " + nSeg++);
                    nTrk = 0;
                    foreach (wptType wpt in seg.trkpt) {
                        hr = cad = "";
                        extensions = wpt.extensions;
                        if (extensions != null) {
                            extensionElements = extensions.Any;
                            foreach (XElement element in extensionElements) {
                                if (element == null || !element.HasElements) continue;
                                IEnumerable<XElement> children = element.Elements();
                                foreach (XElement child in children) {
                                    XNamespace ns = child.GetDefaultNamespace();
                                    if (ns.NamespaceName.Equals(NS_TrackPointExtension_v1)) {
                                        if (child.Name.LocalName.Equals("hr")) hr = child.Value;
                                        else if (child.Name.LocalName.Equals("cad")) cad = child.Value;
                                    } else if (child.Name.LocalName.Equals(NS_TrackPointExtension_v2)) {
                                        if (child.Name.LocalName.Equals("hr")) hr = child.Value;
                                        else if (child.Name.LocalName.Equals("cad")) cad = child.Value;
                                    }
                                }
                            }
                        }
                        Console.WriteLine(
                           String.Format("{0:d4} lat={1:f6} lon={2:f6} ele={3:f6} hr={4} cad={5}",
                          nTkpt++, wpt.lat, wpt.lon, wpt.ele, hr, cad));
                    }
                }
            }
        }
#else
            private static void gpxTest() {
        Console.WriteLine(NL + "Exercise Test GPX");
            Console.WriteLine(gpxFileName);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(gpxType), NS_GPX_1_1);
            FileStream fs = new FileStream(gpxFileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            gpxType gpxType = (gpxType)xmlSerializer.Deserialize(reader);

            Console.WriteLine("Version=" + gpxType.version);
            trkType[] tracks = gpxType.trk;
            extensionsType extensions;
            XmlElement[] extensionElements;
            int nTrk = 0, nSeg = 0, nTkpt = 0;
            string hr, cad;
            foreach (trkType trk in tracks) {
                Console.WriteLine("Track: " + nTrk++);
                nSeg = 0;
                foreach (trksegType seg in trk.trkseg) {
                    Console.WriteLine("Segment: " + nSeg++);
                    nTrk = 0;
                    foreach (wptType wpt in seg.trkpt) {
                        hr = cad = "";
                        extensions = wpt.extensions;
                        if (extensions != null) {
                            extensionElements = extensions.Any;
                            foreach (XmlElement element in extensionElements) {
                                if (element == null || !element.HasChildNodes) continue;
                                XmlNodeList children = element.ChildNodes;
                                foreach (XmlNode child in children) {
                                    if (child.NamespaceURI.Equals(NS_TrackPointExtension_v1)) {
                                        if (child.LocalName.Equals("hr")) hr = child.InnerText;
                                        else if (child.LocalName.Equals("cad")) cad = child.InnerText;
                                    } else if (child.NamespaceURI.Equals(NS_TrackPointExtension_v2)) {
                                        if (child.LocalName.Equals("hr")) hr = child.InnerText;
                                        else if (child.LocalName.Equals("cad")) cad = child.InnerText;
                                    }
                                }
                            }
                        }
                        Console.WriteLine(
                           String.Format("{0:d4} lat={1:f6} lon={2:f6} ele={3:f6} hr={4} cad={5}",
                          nTkpt++, wpt.lat, wpt.lon, wpt.ele, hr, cad));
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Reads TCX files using LinqToXsd.
        /// </summary>
        /// <param name="fileName"></param>
        private static void tcxTest(string fileName) {
            Console.WriteLine(NL + "Exercise Test TCX");
            Console.WriteLine(fileName);

#if false
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TrainingCenterDatabase_t),
                NS_TrainingCenterDatabase_v2);
            FileStream fs = new FileStream(tcxFileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            TrainingCenterDatabase_t tcx = (TrainingCenterDatabase_t)xmlSerializer.Deserialize(reader);
#endif

            TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(fileName);

            if (tcx.Activities != null) {
                Console.WriteLine("nActivities=" + tcx.Activities.Activity.Count);
            } else {
                Console.WriteLine("nActivities=" + tcx.Activities.Activity.Count);
            }
            if (tcx.Courses != null) {
                Console.WriteLine("nCourses=" + tcx.Courses.Course.Count);
            } else {
                Console.WriteLine("nCourses=" + 0);
            }
            if (tcx.Workouts != null) {
                Console.WriteLine("nWorkouts=" + tcx.Workouts.Workout.Count);
            } else {
                Console.WriteLine("nWorkouts=" + 0);
            }

            IList<Activity_t> activityList;
            IList<ActivityLap_t> lapList;
            IList<Track_t> trackList;
            IList<Trackpoint_t> trackpointList;

            Position_t position;
            string hr, cad;
            int nActs = 0, nLaps, nSegs = 0, nTkpts = 0;
            double lat, lon, ele;

            // Get Author info
            if (tcx.Author != null) {
                AbstractSource_t author = tcx.Author;
                Console.WriteLine("author=" + author.Name);
            }

            // Loop over activities
            if (tcx.Activities == null) {
                Console.WriteLine("There are no Activities");
                return;
            }
            activityList = tcx.Activities.Activity;
            nActs = 0;
            foreach (Activity_t activity in activityList) {
                Console.WriteLine("Activity " + nActs);
                nActs++;
                // Loop over laps (are like tracks in GPX)
                nLaps = 0;
                lapList = activity.Lap;
                foreach (ActivityLap_t lap in lapList) {
                    Console.WriteLine("Lap (Track) " + nLaps);
                    nLaps++;
                    // Loop over tracks
                    trackList = lap.Track;
                    nSegs = 0;
                    foreach (Track_t trk in trackList) {
                        Console.WriteLine("Track (Segment) " + nSegs);
                        nSegs++;
                        // Loop over trackpoints
                        nTkpts = 0;
                        trackpointList = trk.Trackpoint;
                        foreach (Trackpoint_t tpt in trackpointList) {
                            nSegs++;
                            if (tpt.Position != null) {
                                position = tpt.Position;
                                lat = position.LatitudeDegrees;
                                lon = position.LongitudeDegrees;
                            } else {
                                lat = lon = Double.NaN;
                            }
                            if (tpt.AltitudeMeters != null) {
                                ele = tpt.AltitudeMeters.Value;
                            } else {
                                ele = Double.NaN;
                            }
                            if (tpt.HeartRateBpm != null) {
                                hr = tpt.HeartRateBpm.Value.ToString();
                            } else {
                                hr = "NA";
                            }
                            if (tpt.Cadence != null) {
                                cad = tpt.Cadence.ToString();
                            } else {
                                cad = "NA";
                            }
                            Console.WriteLine(
                               String.Format("{0:d4} lat={1:f6} lon={2:f6} ele={3:f6} hr={4} cad={5}",
                              nTkpts++, lat, lon, ele, hr, cad));
                        }
                    }
                }
            }

#if false
            // Using XmlSerializer
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TrainingCenterDatabase_t),
            NS_TrainingCenterDatabase_v2);
            FileStream fs = new FileStream(tcxFileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            TrainingCenterDatabase_t tcx = (TrainingCenterDatabase_t)xmlSerializer.Deserialize(reader);
#endif
        }

#if test
        private static TrackPointExtensionv1.TrackPointExtension_t getTrackPointExtensionV1(XmlElement element) {
            if (element == null) return null;
            try {
                using (StringReader reader = new StringReader(element.OuterXml)) {
                    XmlSerializer serializer = new XmlSerializer(typeof(TrackPointExtensionv1.TrackPointExtension_t));
                    return (TrackPointExtensionv1.TrackPointExtension_t)serializer.Deserialize(reader);
                }
            } catch (Exception ex) {
                Console.WriteLine("getTrackPointExtension failed: "
                    + ex.GetType() + NL + ex.StackTrace);
                return null;
            }
        }

        private static TrackPointExtensionv2.TrackPointExtension_t getTrackPointExtensionV2(XmlElement element) {
            if (element == null) return null;
            try {
                using (StringReader reader = new StringReader(element.OuterXml)) {
                    XmlSerializer serializer = new XmlSerializer(typeof(TrackPointExtensionv2.TrackPointExtension_t));
                    return (TrackPointExtensionv2.TrackPointExtension_t)serializer.Deserialize(reader);
                }
            } catch (Exception ex) {
                Console.WriteLine("getTrackPointExtension failed: "
                    + ex.GetType() + NL + ex.StackTrace);
                return null;
            }
        }
#endif
    }
}
