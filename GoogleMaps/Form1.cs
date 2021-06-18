using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GoogleMaps
{
    public partial class Form1 : Form
    {
        // initialize the overlays
        GMap.NET.WindowsForms.GMapOverlay fromMarkers = new GMap.NET.WindowsForms.GMapOverlay("fromMarkers");
        GMap.NET.WindowsForms.GMapOverlay toMarkers = new GMap.NET.WindowsForms.GMapOverlay("toMarkers");
        GMap.NET.WindowsForms.GMapOverlay routes = new GMap.NET.WindowsForms.GMapOverlay("routes");

        public Form1()
        {
            InitializeComponent();
        }

        private void map_Load(object sender, EventArgs e)
        {
            // get your api key, refer at https://www.youtube.com/watch?v=eXJ1qkTMLw8
            GMap.NET.MapProviders.GMapProviders.GoogleMap.ApiKey = "<YOUR KEY HERE>";

            // initialize primary coordinates. you can set your own.
            double lat = 14.588493;
            double lng = 121.0427838;

            // initialize map behavior
            map.ShowCenter = false;
            map.MapProvider = GMap.NET.MapProviders.GMapProviders.GoogleMap;
            map.DragButton = MouseButtons.Left;
            map.Position = new GMap.NET.PointLatLng(lat, lng);
            map.MinZoom = 11;
            map.MaxZoom = 17;
            map.Zoom = 11;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // remove existing overlays
            map.Overlays.Remove(fromMarkers);
            map.Overlays.Remove(toMarkers);
            map.Overlays.Remove(routes);

            if (fromMarkers.Markers.Count > 0)
            {
                fromMarkers.Markers.RemoveAt(0);
            }

            if (toMarkers.Markers.Count > 0)
            {
                toMarkers.Markers.RemoveAt(0);
            }

            if (routes.Routes.Count > 0)
            {
                routes.Routes.RemoveAt(0);
            }

            map.Refresh();

            // get the addresses
            String from = txtFrom.Text.Trim();
            String to = txtTo.Text.Trim();
            Console.WriteLine(from);
            Console.WriteLine(to);

            // initialize from at to coordinates
            double fromlat;
            double fromlng;
            double tolat;
            double tolng;

            // if addresses filled up
            if (!from.Equals("") && !to.Equals(""))
            {
                GMap.NET.GeoCoderStatusCode statusCodeFrom;
                GMap.NET.GeoCoderStatusCode statusCodeTo;

                // get the geocode of the addresses
                var fromlatlng = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetPoint(from, out statusCodeFrom);
                var tolatlng = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetPoint(to, out statusCodeTo);

                // if geocoder status ok, process everything
                if (statusCodeFrom == GMap.NET.GeoCoderStatusCode.OK && statusCodeTo == GMap.NET.GeoCoderStatusCode.OK)
                {
                    // get the from latlng
                    double.TryParse(fromlatlng?.Lat.ToString(), out fromlat);
                    double.TryParse(fromlatlng?.Lng.ToString(), out fromlng);
                    var fromPosition = new GMap.NET.PointLatLng(fromlat, fromlng);

                    // initialize from marker
                    var fromMarker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(fromPosition, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red);
                    fromMarkers.Markers.Add(fromMarker);
                    map.Overlays.Add(fromMarkers);

                    // get the to latlng
                    double.TryParse(tolatlng?.Lat.ToString(), out tolat);
                    double.TryParse(tolatlng?.Lng.ToString(), out tolng);
                    var toPosition = new GMap.NET.PointLatLng(tolat, tolng);

                    // initialize the to marker
                    var toMarker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(toPosition, GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red);
                    toMarkers.Markers.Add(toMarker);
                    map.Overlays.Add(toMarkers);

                    // initialize route
                    var route = GMap.NET.MapProviders.GMapProviders.GoogleMap.GetRoute(fromPosition, toPosition, false, false, 11);

                    var r = new GMap.NET.WindowsForms.GMapRoute(route.Points, "My Route");
                    routes.Routes.Add(r);
                    map.Overlays.Add(routes);

                    // set the zoom and coordinates
                    map.Position = new GMap.NET.PointLatLng(fromlat, fromlng);
                    map.Zoom = 14;

                    // get the distance
                    var distance = route.Distance;

                    // display the distance on console
                    Console.WriteLine(distance);
                }
            }
        }
    }
}
