#undef verbose
#undef debugging
using System;
using System.Collections.Generic;
using System.IO;
using GPS_UTILS;
using www.garmin.com.xmlschemas.TrainingCenterDatabase.v2;
using www.topografix.com.GPX_1_1;

namespace InterpolateGPS {
    class Program {
        public static readonly string NL = Environment.NewLine;
        private const string INPUT_FILE = @"C:\Users\evans\Documents\GPSLink\Polar\Kenneth_Evans_2020-03-23_18-24-20_Walking_Proud_Lake.tcx";
        private const string INTERP_FILE = @"C:\Users\evans\Documents\GPSLink\Polar\Problems\Kenneth_Evans_2020-03-23_18-24-20_Walking_Proud_Lake.patch.gpx";
        // This is necessary if the track comes back over where it started
        // Can get it using BaseCamp
        private const int maxIndex = 1927;
        private const string OUTPUT_FILE_DIR = @"C:\Users\evans\Documents\GPSLink\Interpolated";
        private const string OUTPUT_FILE_ID = ".interpolated";
        private const string FORMATTED_FILE_DIR = @"C:\Users\evans\Documents\GPSLink\Interpolated";
        private const string FORMATTED_FILE_ID = ".formatted";
        private static string inputFileName = INPUT_FILE;
        private static string interpFileName = INTERP_FILE;

        static void Main(string[] args) {
            // Create output file name
            string outFileName = OUTPUT_FILE_DIR + @"\"
                + Path.GetFileNameWithoutExtension(inputFileName)
                + OUTPUT_FILE_ID + Path.GetExtension(inputFileName);
            string formattedFileName = FORMATTED_FILE_DIR + @"\"
                + Path.GetFileNameWithoutExtension(inputFileName)
                + FORMATTED_FILE_ID + Path.GetExtension(inputFileName);
            Console.WriteLine("From " + inputFileName);
            interpolate(inputFileName, interpFileName, outFileName, formattedFileName);
        }

        /// <summary>
        /// Interpolates the lat lon values from the GPX interpFile to the
        /// TCX inputFile and saves the result to the outputFile.
        /// </summary>
        /// <param name="inputFile">Original file</param>
        /// <param name="interpFile">File with lat lon to interpolate</param>
        /// <param name="outPutFile">Output file</param>
        private static void interpolate(string inputFile, string interpFile,
                   string outPutFile, string formattedFile) {
            Console.WriteLine("Interpolate GPS");
            Console.WriteLine("To " + inputFileName);
            Console.WriteLine("From " + interpFileName);
            Console.WriteLine("Formatted Destination: " + formattedFile);
            Console.WriteLine("Save Destination: " + outPutFile);
            Console.WriteLine();

            gpx gpxType = gpx.Load(interpFile);
            TrainingCenterDatabase tcx = TrainingCenterDatabase.Load(inputFile);
            tcx.Save(formattedFile);
            Console.WriteLine("Wrote " + formattedFile);

            Console.WriteLine("Input file: " + Path.GetFileName(inputFile));
            if (tcx.Activities != null) {
                Console.WriteLine("nActivities=" + tcx.Activities.Activity.Count);
            } else {
                Console.WriteLine("nActivities=" + 0);
                Console.WriteLine("No activities to process");
                return;
            }
            Console.WriteLine("Interp file: " + Path.GetFileName(interpFile));
            Console.WriteLine("Version=" + gpxType.version);

            IList<trkType> tracks = gpxType.trk;

            IList<Activity_t> activityList1;
            IList<ActivityLap_t> lapList1;
            IList<Track_t> trackList1;
            IList<Trackpoint_t> trackpointList1;

            Position_t position;
            int nActs, nLaps, nTrks, nTkpts;
            double lat, lon, dist, totalDist;

            // Process interp file (GPX)
            List<LatLon> latLonList = new List<LatLon>();
            IList<trkType> tracks1 = gpxType.trk;
            int nSeg;

            nTrks = nTkpts = 0;
            totalDist = 0;
            foreach (trkType trk in tracks1) {
                Console.WriteLine("Track: " + nTrks++);
                nSeg = 0;
                foreach (trksegType seg in trk.trkseg) {
                    Console.WriteLine("Segment: " + nSeg++);
                    nTrks = 0;
                    bool first = true;
                    double latPrev = 0, lonPrev = 0;
                    dist = 0;
                    foreach (wptType wpt in seg.trkpt) {
                        nTkpts++;
                        lat = (double)wpt.lat;
                        lon = (double)wpt.lon;
                        if (!first) {
                            dist = GpsUtils.greatCircleDistance(lat, lon, latPrev, lonPrev);
                            totalDist += dist;
                        }
                        latLonList.Add(new LatLon(lat, lon, totalDist));
                        first = false;
                        latPrev = lat;
                        lonPrev = lon;
                    }
                }
            }
            int nInterp = latLonList.Count;
            if (nInterp < 2) {
                Console.WriteLine("Interp file has only " + nInterp + " items");
                return;
            }
            Console.WriteLine("Interpolating from " + nInterp + " items");
            LatLon latLonFirst = latLonList[0];
            LatLon latLonLast = latLonList[nInterp - 1];

            // Process input file (TCX)
            nActs = nLaps = nTrks = nTkpts = 0;
            double distFirst = Double.MaxValue, distLast = Double.MaxValue;
            LatLon latLonFirstMatch = null;
            LatLon latLonLastMatch = null;
            int indexFirstLatLon = -1;
            int indexFirst = -1;
            int indexLast = -1;
            // Loop over activities
            activityList1 = tcx.Activities.Activity;
            foreach (Activity_t activity in activityList1) {
                Console.WriteLine("Activity " + nActs);
                nActs++;
                if (nActs > 1) {
                    Console.WriteLine("Only the first Activity is processed");
                    continue;
                }
                // Loop over laps (are like tracks in GPX)
                nLaps = 0;
                lapList1 = activity.Lap;
                foreach (ActivityLap_t lap in lapList1) {
                    Console.WriteLine("Lap (Track) " + nLaps);
                    nLaps++;
                    if (nLaps > 1) {
                        Console.WriteLine("Only the first Lap is processed");
                        continue;
                    }
                    // Loop over tracks
                    trackList1 = lap.Track;
                    nTrks = 0;
                    foreach (Track_t trk in trackList1) {
                        Console.WriteLine("Track (Segment) " + nTrks);
                        nTrks++;
                        if (nTrks > 1) {
                            Console.WriteLine("Only the first track is processed");
                            continue;
                        }
                        // Loop over trackpoints
                        nTkpts = 0;
                        trackpointList1 = trk.Trackpoint;
                        foreach (Trackpoint_t tpt in trackpointList1) {
                            //if (trackpointList1.IndexOf(tpt) > maxIndex) break;
                            nTkpts++;
                            if (tpt.Position != null) {
                                position = tpt.Position;
                                lat = position.LatitudeDegrees;
                                lon = position.LongitudeDegrees;
#if verbose
                                Console.WriteLine(trackpointList1.IndexOf(tpt)
                                   + " lat=" + lat + " lon=" + lon + " time=" + tpt.Time);
#endif
                                if (indexFirstLatLon < 0) indexFirstLatLon = trackpointList1.IndexOf(tpt);
                            } else {
                                Console.WriteLine(trackpointList1.IndexOf(tpt) +
                                    " Warning: No Position for input activity="
                                    + nActs + " lap=" + nLaps + " trk=" + nTrks
                                    + " tkpt=" + nTkpts);
                                continue;
                            }
                            // Find first match to first lat lon in interp
                            dist = GpsUtils.greatCircleDistance(lat, lon, latLonFirst.Lat, latLonFirst.Lon);
                            if (dist < distFirst) {
                                distFirst = dist;
                                latLonFirstMatch = new LatLon(lat, lon, 0, tpt.Time);
                                indexFirst = trackpointList1.IndexOf(tpt);
                            }
                            // Find last match to last lat lon in interp
                            dist = GpsUtils.greatCircleDistance(lat, lon, latLonLast.Lat, latLonLast.Lon);
                            if (dist <= distLast) {
                                distLast = dist;
                                latLonLastMatch = new LatLon(lat, lon, 0.0, tpt.Time);
                                indexLast = trackpointList1.IndexOf(tpt);
                                //Console.WriteLine(indexLast + " dist=" + dist);
                            }
                        }
                    }
                }
            }
            double firstDist = latLonList[0].Distance;
            double lastDist = latLonList[latLonList.Count - 1].Distance;
#if debugging
            Console.WriteLine("latLonFirst: lat="
                + latLonFirst.Lat + " lon=" + latLonFirst.Lon);
            Console.WriteLine("latLonLast: lat="
                + latLonLast.Lat + " lon=" + latLonLast.Lon);
            Console.WriteLine("firstDist=" + firstDist + " m lastDist=" + lastDist + " m");
#endif 
            if (indexFirst < 0) {
                Console.WriteLine("Did not find a match to the first item in interp");
                return;
            }
            if (indexLast < 0) {
                Console.WriteLine("Did not find a match to the last item in interp");
                return;
            }
            if (indexFirst == indexLast) {
                Console.WriteLine("Warning: Match to first and last item is the same point");
            }
            if (indexFirst > indexLast) {
                Console.WriteLine("Did not find a match for the" +
                    " first item before the one for the last item");
                Console.WriteLine("  Therefore using index where first lat lon values were specified: "
                    + indexFirstLatLon);
                indexFirst = indexFirstLatLon;
            }
            Console.WriteLine("Matching from index " + indexFirst + " to " + indexLast);
            // Loop over these indices
            Activity_t activity0 = activityList1[0];
            ActivityLap_t lap0 = activity0.Lap[0];
            Track_t trk0 = lap0.Track[0];
            Trackpoint_t tpt0;
            long firstTime = trk0.Trackpoint[indexFirst].Time.Ticks;
            long lastTime = trk0.Trackpoint[indexLast].Time.Ticks;
            long deltaTime = lastTime - firstTime;
            double speed = TimeSpan.TicksPerSecond * totalDist / deltaTime;
#if debugging
            Console.WriteLine("TotalDist=" + $"{GpsUtils.M2MI * totalDist:f2}"
                + " mi Duration=" + new TimeSpan(deltaTime)
                + " Speed=" + $"{GpsUtils.M2MI / GpsUtils.SEC2HR * speed:f2}"
                + " mph");
#endif
            for (int i = indexFirst; i <= indexLast; i++) {
                tpt0 = trk0.Trackpoint[i];
                if (tpt0.Position == null) {
                    Console.WriteLine(i +
                        " Warning: No Position for tkpt=" + nTkpts + "skipping");
                    continue;
                }
                long thisTime = tpt0.Time.Ticks - firstTime;
                double latInterp = 0, lonInterp = 0;
                long time0, time1;
                double val0, val1, deltaVal, deltaDist, distInterp;
                // Assume dist is proportional to time
                dist = totalDist * thisTime / deltaTime;
                if (dist == 0) {
                    latInterp = latLonList[0].Lat;
                    lonInterp = latLonList[0].Lon;
#if debugging
                    Console.WriteLine(i + " latInterp=" + $"{latInterp:f8}"
                        + " lonInterp=" + $"{lonInterp:f8}"
                        + " dist=" + $"{dist:f4}"
                    );
#endif
                } else {
                    // Linearly interpolate
                    for (int j = 1; j < latLonList.Count; j++) {
                        if (dist > latLonList[j].Distance) continue;
                        time0 = latLonList[j - 1].Time.Ticks;
                        time1 = latLonList[j].Time.Ticks;
                        distInterp = dist - latLonList[j - 1].Distance;
                        deltaDist = latLonList[j].Distance - latLonList[j - 1].Distance;
                        val0 = latLonList[j - 1].Lat;
                        val1 = latLonList[j].Lat;
                        deltaVal = val1 - val0;
                        latInterp = val0;
                        if (deltaVal != 0) {
                            latInterp = val0 + distInterp * deltaVal / deltaDist;
                        }
                        val0 = latLonList[j - 1].Lon;
                        val1 = latLonList[j].Lon;
                        deltaVal = val1 - val0;
                        lonInterp = val0;
                        if (deltaVal != 0) {
                            lonInterp = val0 + distInterp * deltaVal / deltaDist;
                        }
#if debugging
                        Console.WriteLine(i + " latInterp =" + $"{latInterp:f8}"
                            + " j=" + j
                            + " lonInterp=" + $"{lonInterp:f8}"
                            + " deltaVal=" + $"{deltaVal:f8}"
                            + " distInterp=" + $"{distInterp:f4}"
                            + " deltaDist=" + $"{deltaDist:f4}");
#endif
                        // Don't do any more
                        break;
                    }
                    if(latInterp == 0 || lonInterp ==0) {
                        Console.WriteLine(i + " Not found"
                            + " dist=" + $"{dist:f4}"
                            + " m firstDist=" + $"{firstDist:f4}"
                            + " m lastDist=" + $"{lastDist:f4}"
                            + " m");
                    }
                }
                position = tpt0.Position;
#if debugging
                Console.WriteLine(i + " Before: "
                    + " LatitudeDegrees=" + position.LatitudeDegrees
                    + " LongitudeDegrees=" + position.LongitudeDegrees
                    + " latInterp=" + latInterp
                    + " lonInterp=" + lonInterp);
#endif
                position.LatitudeDegrees = latInterp;
                position.LongitudeDegrees = lonInterp;
#if debugging
                Console.WriteLine(i + " After: "
                    + " LatitudeDegrees=" + position.LatitudeDegrees
                    + " LongitudeDegrees=" + position.LongitudeDegrees);
#endif
            }
            tcx.Save(outPutFile);
            Console.WriteLine("Wrote " + outPutFile);
        }
    }


    public class LatLon {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public double Distance { get; set; }
        public DateTime Time { get; set; }

        public LatLon(double lat, double lon, double distance) {
            Lat = lat;
            Lon = lon;
            Distance = distance;
        }

        public LatLon(double lat, double lon, double distance, DateTime time) {
            Lat = lat;
            Lon = lon;
            Distance = distance;
            Time = time;
        }

    }
}
