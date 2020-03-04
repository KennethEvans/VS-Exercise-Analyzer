using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GPX_1_1;

namespace Exercise_Analyzer {
    partial class MainForm {
        private const string NS_GPX_1_1 = "http://www.topografix.com/GPX/1/1";
        private const string NS_TrackPointExtension_v1 = "http://www.garmin.com/xmlschemas/TrackPointExtension/v1";
        private const string NS_TrackPointExtension_v2 = "http://www.garmin.com/xmlschemas/TrackPointExtension/v2";

        private void test(string fileName) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(gpxType),
                NS_GPX_1_1);

            FileStream fs = new FileStream(fileName, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);
            gpxType gpxType = (gpxType)xmlSerializer.Deserialize(reader);

            writeInfo("Test");
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
    }
}
