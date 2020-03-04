#undef test

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GPX1_1;

namespace ExerciseTest {

    class Program {
        public static readonly string NL = Environment.NewLine;
        private const string NS_TrackPointExtension_v1 = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1";
        private const string NS_TrackPointExtension_v2 = "http://www.garmin.com/xmlschemas/TrackPointExtension/v2";
        private const string TEST_FILE1 = @"C:\Users\evans\Documents\GPSLink\Test\Short.gpx";
        private const string TEST_FILE2 = @"C:\Users\evans\Documents\GPSLink\Test\Proud-Lake-Simplified.gpx";
        private const string TEST_FILE3 = @"C:\Users\evans\Documents\GPSLink\Test\Kenneth_Evans_TPV2.gpx";
        private static string fileName = TEST_FILE3;

        static void Main(string[] args) {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(gpxType),
                "http://www.topografix.com/GPX/1/1");

            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            gpxType gpxType = (gpxType)xmlSerializer.Deserialize(reader);

            Console.WriteLine("Exercise Test");
            Console.WriteLine(fileName);
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
