using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GPX_1_1;
using KEUtils;
using TrainingCenterDatabaseV2;

namespace Exercise_Analyzer {
    partial class MainForm {
        private const string NS_GPX_1_1 = "http://www.topografix.com/GPX/1/1";
        private const string NS_GPX_TrainingCenterDatabase_v2 = "http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2";
        private const string NS_TrackPointExtension_v1 = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1";
        private const string NS_TrackPointExtension_v2 = "http://www.garmin.com/xmlschemas/TrackPointExtension/v2";

        private void test(string fileName) {
            if (String.IsNullOrEmpty(fileName)) return;
            try {
                string ext = Path.GetExtension(fileName);
                if (String.IsNullOrEmpty(ext)) {
                    Utils.errMsg("Extension must be GPX or TCX");
                    return;
                }
                if (ext.ToLower().Equals(".gpx")) {
                    testGpx(fileName);
                } else if (ext.ToLower().Equals(".tcx")) {
                    testTcx2(fileName);
                }
            } catch (Exception ex) {
                Utils.excMsg("Failed to read " + inputFile, ex);
                return;
            }
        }

        private void testGpx(string fileName) {
            writeInfo("Test GPX");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(gpxType),
                NS_GPX_1_1);

            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            bool canDeserialize = xmlSerializer.CanDeserialize(reader);
            if (!canDeserialize) {
                writeInfo("Cannot deserialize:" + fileName);
                return;
            }
            gpxType gpxType = (gpxType)xmlSerializer.Deserialize(reader);

            writeInfo(fileName);
            writeInfo("Version=" + gpxType.version);
            trkType[] tracks = gpxType.trk;
            extensionsType extensions;
            XmlElement[] extensionElements;
            int nTrk = 0, nSeg = 0, nTkpt = 0;
            string hr, cad;
            foreach (trkType trk in tracks) {
                writeInfo("Track: " + nTrk++);
                nSeg = 0;
                foreach (trksegType seg in trk.trkseg) {
                    writeInfo("Segment: " + nSeg++);
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
                        writeInfo(
                           String.Format("{0:d4} lat={1:f6} lon={2:f6} ele={3:f6} hr={4} cad={5}",
                          nTkpt++, wpt.lat, wpt.lon, wpt.ele, hr, cad));
                    }
                }
            }
        }

        private void testTcx(string fileName) {
            writeInfo("Test TCX");
            writeInfo(fileName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TrainingCenterDatabase_t),
                NS_GPX_TrainingCenterDatabase_v2);

            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            bool canDeserialize = xmlSerializer.CanDeserialize(reader);
            //if (!canDeserialize) {
            //    writeInfo("Cannot deserialize:" + fileName);
            //    return;
            //}
            TrainingCenterDatabase_t tcx = (TrainingCenterDatabase_t)xmlSerializer.Deserialize(reader);

            writeInfo(fileName);
            writeInfo("nActivities=" + tcx.Activities.Activity.Count);
            writeInfo("nCourses=" + tcx.Courses.Count);
            writeInfo("nExtensions=" + tcx.Extensions.Any.Count);
            writeInfo("nWorkouts=" + tcx.Workouts.Count);

            List<Activity_t> activityList;
            List<ActivityLap_t> lapList;
            List<Trackpoint_t> trackpointList;

            Position_t position;
            string hr, cad;
            int nAct = 0, nTrk = 0, nSeg = 0, nTkpt = 0;
            double lat, lon, ele;

            // Get Author info
            AbstractSource_t author = tcx.Author;
            writeInfo("author=" + author.Name);

            // Loop over activities
            activityList = tcx.Activities.Activity;
            nAct = 0;
            foreach (Activity_t activity in activityList) {
                nAct++;
                // Loop over laps (are like tracks in GPX)
                nTrk = 0;
                lapList = activity.Lap;
                foreach (ActivityLap_t lap in lapList) {
                    nTrk++;
                    // Loop over trackpoints
                    nTkpt = 0;
                    trackpointList = lap.Track;
                    foreach (Trackpoint_t tpt in trackpointList) {
                        nSeg++;
                        position = tpt.Position;
                        lat = position.LatitudeDegrees;
                        lon = position.LongitudeDegrees;
                        ele = tpt.AltitudeMeters;
                        hr = tpt.HeartRateBpm.ToString();
                        cad = tpt.Cadence.ToString();
                        writeInfo(
                           String.Format("{0:d4} lat={1:f6} lon={2:f6} ele={3:f6} hr={4} cad={5}",
                          nTkpt++, lat, lon, ele, hr, cad));
                    }
                }
            }
        }

        private void testTcx2(string fileName) {
            writeInfo("Test TCX with XmlDocument");
            writeInfo(fileName);

            XDocument tcx = XDocument.Load(fileName);

            foreach (XElement element in tcx.Descendants().
                Where(p => p.Name.LocalName == "Name"
                && p.Parent.Name.LocalName == "Author")) {
                writeInfo("Author: " + element.Value);
            }

            IEnumerable<XElement> activities =
                from item in tcx.Descendants()
                where item.Name.LocalName == "Activity"
                select item;
            writeInfo("nActivities=" + activities.Count());

            IEnumerable<XElement> other =
                from item in tcx.Elements()
                where item.Name.LocalName == "Courses"
                select item;
            writeInfo("nCourses=" + other.Count());
            other =
                from item in tcx.Elements()
                where item.Name.LocalName == "Workouts"
                select item;
            writeInfo("nWorkouts=" + other.Count());
            other =
                from item in tcx.Elements()
                where item.Name.LocalName == "Extensions"
                select item;
            writeInfo("nExtensions=" + other.Count());
            writeInfo("Data:");

            // Loop over Activities, Laps, Tracks, and Trackpoints
            int nActivity = 0, nLaps, nTrks, nTpts;
            double lat, lon, ele;
            string hr, cad;
            foreach (XElement activity in activities) {
                writeInfo("Activity " + nActivity++);
                foreach (XElement element in activity.Descendants().
                    Where(p => p.Name.LocalName == "Name"
                    && p.Parent.Name.LocalName == "Creator")) {
                    writeInfo("Creator: " + element.Value);
                }
                foreach (XElement element in activity.Elements().
                    Where(p => p.Name.LocalName == "Notes")) {
                    writeInfo("Notes: " + element.Value);
                }
                nLaps = 0;
                IEnumerable<XElement> laps =
                    from item in activity.Elements()
                    where item.Name.LocalName == "Lap"
                    select item;
                foreach (XElement lap in laps) {
                    writeInfo("Lap " + nLaps++);
                    foreach (XElement element in lap.Elements().
                        Where(p => p.Name.LocalName == "TotalTimeSeconds")) {
                        writeInfo("TotalTimeSeconds: " + element.Value);
                    }
                    foreach (XElement element in lap.Elements().
                        Where(p => p.Name.LocalName == "DistanceMeters")) {
                        writeInfo("DistanceMeters: " + element.Value);
                    }
                    foreach (XElement element in lap.Elements().
                        Where(p => p.Name.LocalName == "AvgSpeed")) {
                        writeInfo("AvgSpeed: " + element.Value);
                    }
                    foreach (XElement element in lap.Elements().
                        Where(p => p.Name.LocalName == "MaximumSpeed")) {
                        writeInfo("MaximumSpeed: " + element.Value);
                    }
                    foreach (XElement element in lap.Elements().
                        Where(p => p.Name.LocalName == "Calories")) {
                        writeInfo("Calories: " + element.Value);
                    }
                    foreach (XElement element in lap.Descendants().
                        Where(p => p.Name.LocalName == "Value"
                        && p.Parent.Name.LocalName == "AverageHeartRateBpm")) {
                        writeInfo("AverageHeartRateBpm: " + element.Value);
                    }
                    foreach (XElement element in lap.Descendants().
                        Where(p => p.Name.LocalName == "Value"
                        && p.Parent.Name.LocalName == "MaximumHeartRateBpm")) {
                        writeInfo("MaximumHeartRateBpm: " + element.Value);
                    }
                    nTrks = 0;
                    IEnumerable<XElement> trks =
                       from item in lap.Elements()
                       where item.Name.LocalName == "Track"
                       select item;
                    foreach (XElement trk in trks) {
                        writeInfo("Track " + nTrks++);
                        nTpts = 0;
                        IEnumerable<XElement> tpts =
                           from item in trk.Elements()
                           where item.Name.LocalName == "Trackpoint"
                           select item;
                        foreach (XElement tpt in tpts) {
                            lat = lon = ele = 0;
                            hr = cad = "";
#if false
                            foreach (XElement elem in from item in tpt.Descendants()
                                                      select item) {
                                writeInfo("name=" + elem.Name);
                            }
#endif
                            foreach (XElement element in tpt.Elements().
                                Where(p => p.Name.LocalName == "LatitudeDegrees")) {
                                lat = Double.Parse(element.Value);
                            }
                            foreach (XElement element in tpt.Elements().
                                Where(p => p.Name.LocalName == "LongitudeDegrees")) {
                                lon = Double.Parse(element.Value);
                            }
                            foreach (XElement element in tpt.Elements().
                               Where(p => p.Name.LocalName == "AltitudeMeters")) {
                                ele = Double.Parse(element.Value);
                            }
                            foreach (XElement element in tpt.Descendants().
                                Where(p => p.Name.LocalName == "Value" &&
                                p.Parent.Name.LocalName == "HeartRateBpm")) {
                                hr = element.Value;
                            }
                            foreach (XElement element in tpt.Elements().
                               Where(p => p.Name.LocalName == "Cadence")) {
                                cad = element.Value;
                            }
                            writeInfo(
                            String.Format("{0:d4} lat={1:f6} lon={2:f6} ele={3:f6} hr={4} cad={5}",
                                nTpts++, lat, lon, ele, hr, cad));
                        }
                    }
                }
            }
        }
    }
}
