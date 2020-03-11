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
                    testGpx2(fileName);
                } else if (ext.ToLower().Equals(".tcx")) {
                    testTcx2(fileName);
                }
            } catch (Exception ex) {
                Utils.excMsg("Failed to read " + inputFile, ex);
                return;
            }
        }

        private void testGpx(string fileName) {
            writeInfo(NL + "Test GPX");
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
                            foreach (XmlElement elem in extensionElements) {
                                if (elem == null || !elem.HasChildNodes) continue;
                                XmlNodeList children = elem.ChildNodes;
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
            writeInfo(NL + "Test TCX");
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
            writeInfo(NL + "Test TCX with XmlDocument");
            writeInfo(fileName);

            XDocument doc = XDocument.Load(fileName);
            XElement tcx = doc.Root;

            // Note that it is better to use Element in foreach enquiries
            // Except use Descendents when the Parent is needed
            foreach (XElement elem in tcx.Descendants().
                Where(p => p.Name.LocalName == "Name"
                && p.Parent.Name.LocalName == "Author")) {
                writeInfo("Author: " + elem.Value);
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

            // Loop over Activities, Laps, Tracks, and Trackpoints
            int nActivity = 0, nLaps, nTrks, nTpts;
            double lat, lon, ele;
            int hr, cad;
            DateTime time;
            foreach (XElement activity in activities) {
                writeInfo("Activity " + nActivity++);
                foreach (XElement elem in activity.Descendants().
                    Where(p => p.Name.LocalName == "Name"
                    && p.Parent.Name.LocalName == "Creator")) {
                    writeInfo("Creator: " + elem.Value);
                }
                foreach (XElement elem in activity.Elements().
                    Where(p => p.Name.LocalName == "Notes")) {
                    writeInfo("Notes: " + elem.Value);
                }
                nLaps = 0;
                IEnumerable<XElement> laps =
                    from item in activity.Elements()
                    where item.Name.LocalName == "Lap"
                    select item;
                foreach (XElement lap in laps) {
                    writeInfo("Lap " + nLaps++);
                    foreach (XElement elem in lap.Elements()) {
                        if (elem.Name.LocalName == "TotalTimeSeconds") {
                            writeInfo("TotalTimeSeconds: " + elem.Value);
                        } else if (elem.Name.LocalName == "DistanceMeters") {
                            writeInfo("DistanceMeters: " + elem.Value);
                        } else if (elem.Name.LocalName == "AvgSpeed") {
                            writeInfo("AvgSpeed: " + elem.Value);
                        } else if (elem.Name.LocalName == "MaximumSpeed") {
                            writeInfo("MaximumSpeed: " + elem.Value);
                        } else if (elem.Name.LocalName == "Calories") {
                            writeInfo("Calories: " + elem.Value);
                        }
                    }
                    foreach (XElement elem in lap.Descendants().
                        Where(p => p.Name.LocalName == "Value"
                        && p.Parent.Name.LocalName == "AverageHeartRateBpm")) {
                        writeInfo("AverageHeartRateBpm: " + elem.Value);
                    }
                    foreach (XElement elem in lap.Descendants().
                        Where(p => p.Name.LocalName == "Value"
                        && p.Parent.Name.LocalName == "MaximumHeartRateBpm")) {
                        writeInfo("MaximumHeartRateBpm: " + elem.Value);
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
                            hr = cad = 0;
                            time = DateTime.MinValue;
#if false
                            foreach (XElement elem in from item in tpt.Descendants()
                                                      select item) {
                                writeInfo("name=" + elem.Name);
                            }
#endif
                            foreach (XElement elem in tpt.Descendants()) {
                                if (elem.Name.LocalName == "LatitudeDegrees") {
                                    lat = (double)elem;
                                } else if (elem.Name.LocalName == "LongitudeDegrees") {
                                    lon = (double)elem;
                                } else if (elem.Name.LocalName == "AltitudeMeters") {
                                    ele = (double)elem;
                                } else if (elem.Name.LocalName == "Time") {
                                    time = (DateTime)elem;
                                } else if (elem.Name.LocalName == "Cadence") {
                                    cad = (int)elem;
                                } else if (elem.Name.LocalName == "Value" &&
                                    elem.Parent.Name.LocalName == "HeartRateBpm") {
                                    hr = (int)elem;
                                }
                            }
                            writeInfo(
                            String.Format("{0:d4} lat={1:f6} lon={2:f6} ele={3:f6} time={4}",
                                nTpts++, lat, lon, ele, time));
                            writeInfo(
                            String.Format("  hr={0} cad={1}",
                                hr, cad));
                        }
                    }
                }
            }
        }

        private void testGpx2(string fileName) {
            writeInfo(NL + "Test GPX with XmlDocument");
            writeInfo(fileName);

            XDocument doc = XDocument.Load(fileName);
            XElement gpx = doc.Root;
#if false
            writeInfo("nElements=" + gpx.Elements().Count());
            foreach (XElement elem in from item in gpx.Elements()
                                      select item) {
                writeInfo("Name=" + elem.Name);
            }
#endif
            // Note that it is better to use Element in foreach enquiries
            // Except use Descendents when the Parent is needed
            foreach (XAttribute attr in gpx.Attributes()) {
                if (attr.Name == "creator") {
                    writeInfo("Creator: " + attr.Value);
                } else if (attr.Name == "version") {
                    writeInfo("Version: " + attr.Value);
                }
            }
            foreach (XElement elem in gpx.Elements().
                Where(p => p.Name.LocalName == "metadata")) {
                foreach (XElement elem1 in elem.Elements()) {
                    if (elem1.Name.LocalName == "link") {
                        writeInfo("Link: " + elem1.Value);
                    } else if (elem1.Name.LocalName == "author") {
                        foreach (XElement elem2 in elem1.Elements()) {
                            if (elem2.Name.LocalName == "name") { }
                            writeInfo("Author: " + elem2.Value);
                        }
                    } else if (elem1.Name.LocalName == "bounds") {
                        string bounds = "";
                        foreach (XAttribute attr in elem1.Attributes()) {
                            bounds += attr.Name + "=" + attr.Value + " ";
                        }
                        if (!string.IsNullOrEmpty(bounds)) {
                            writeInfo("Bounds: " + bounds);
                        }
                    } else if (elem1.Name.LocalName == "category") {
                        writeInfo("Category: " + elem1.Value);
                    } else if (elem1.Name.LocalName == "location") {
                        writeInfo("Location: " + elem1.Value);
                    } else if (elem1.Name.LocalName == "time") {
                        writeInfo("Time: " + elem1.Value);
                    }
                }
            }
            IEnumerable<XElement> trks =
                from item in gpx.Elements()
                where item.Name.LocalName == "trk"
                select item;
            writeInfo("nTracks=" + trks.Count());

            // Loop over Tracks, Segments, and Trackpoints
            int nTrks = 0, nSegs, nTpts;
            double lat, lon, ele, speed, distance;
            int hr, cad, sat;
            DateTime time;
            foreach (XElement trk in trks) {
                writeInfo("Track " + nTrks++);
                foreach (XElement elem in trk.Elements().
                    Where(p => p.Name.LocalName == "Name")) {
                    writeInfo("Name: " + elem.Value);
                }
                foreach (XElement elem in trk.Elements().
                    Where(p => p.Name.LocalName == "Notes")) {
                    writeInfo("Notes: " + elem.Value);
                }
                nSegs = 0;
                IEnumerable<XElement> segs =
                    from item in trk.Elements()
                    where item.Name.LocalName == "trkseg"
                    select item;
                foreach (XElement seg in segs) {
                    writeInfo("Seg " + nSegs++);
                    nTpts = 0;
                    IEnumerable<XElement> tpts =
                       from item in seg.Elements()
                       where item.Name.LocalName == "trkpt"
                       select item;
                    writeInfo("nTrackpoints=" + tpts.Count());
                    foreach (XElement tpt in tpts) {
                        lat = lon = ele = speed = distance = 0;
                        hr = cad = sat =0;
                        time = DateTime.MinValue;
#if false
                        writeInfo("nElements=" + tpt.Elements().Count());
                        foreach (XElement elem in from item in tpt.Elements()
                                                  select item) {
                            writeInfo("Name=" + elem.Name);
                        }
#endif
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
                            } else if (elem.Name.LocalName == "sat") {
                                sat = (int)elem;
                            }
                        }
                        foreach (XElement elem in from item in tpt.Descendants()
                                                  select item) {
                            if (elem.Name.LocalName == "hr") {
                                hr = (int)elem;
                            } else if (elem.Name.LocalName == "cad") {
                                cad = (int)elem;
                            } else if (elem.Name.LocalName == "distance") {
                                distance = (double)elem;
                            } else if (elem.Name.LocalName == "speed") {
                                speed = (double)elem;
                            }
                        }
                        writeInfo(
                          String.Format("{0:d4} lat={1:f6} lon={2:f6} " +
                          "ele={3:f6} time={4}",
                              nTpts++, lat, lon, ele, time));
                        writeInfo(
                          String.Format("  hr={0} cad={1} dist={2:f6} speed={3:f6} sat={4}",
                              hr, cad, distance, speed, sat));
                    }
                }
            }
        }
    }
}
