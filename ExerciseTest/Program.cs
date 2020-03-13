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
        private const string TEST_TCX_FILE1 = @"C:\Users\evans\Documents\GPSLink\Test\Kenneth_Evans_2020-02-24_15-43-34_Walking_Milford.tcx";
        private static string gpxFileName = TEST_GPX_FILE2;
        private static string tcxFileName = TEST_TCX_FILE1;

        static void Main(string[] args) {
            Console.WriteLine("Exercise Test");
            gpxTest();
            tcxTest();

        }

#if linqtoxsd
        private static void gpxTest() {
        Console.WriteLine(NL + "Exercise Test GPX");
            Console.WriteLine(gpxFileName);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(gpxType), NS_GPX_1_1);
            FileStream fs = new FileStream(gpxFileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            gpxType gpxType = (gpxType)xmlSerializer.Deserialize(reader);

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

        private static void tcxTest() {
            Console.WriteLine(NL + "Exercise Test TCX");
            Console.WriteLine(tcxFileName);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(TrainingCenterDatabase_t),
                NS_TrainingCenterDatabase_v2);
            FileStream fs = new FileStream(tcxFileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            TrainingCenterDatabase_t tcx = (TrainingCenterDatabase_t)xmlSerializer.Deserialize(reader);

#if false
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
