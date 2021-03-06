using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Locations;
using System.Collections;
using System.Diagnostics;
using Android.Content.PM;
using System.Threading;
using TestApp.Points;
using Android.Graphics;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using System.IO;
using static Android.Gms.Maps.GoogleMap;

namespace TestApp
{
  //  [Activity(Label = "Route", ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/Theme2")]
    [Activity(AlwaysRetainTaskState = true, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleInstance, Label = "Route Creator", Theme = "@style/Theme2")]
  //  [IntentFilter(new[] { Intent.ActionAssist }, Categories = new[] { Intent.CategoryDefault })]

    public class CreateRoute : AppCompatActivity, IOnMapReadyCallback, ISnapshotReadyCallback //, ILocationListener
    {
        const long MIN_TIME = 5 * 1000; // Minimum time interval for update in seconds, i.e. 5 seconds.
        const long MIN_DISTANCE = 0;

        SupportToolbar toolbar;
        public LocationManager locationManager;
        public string locationProvider;
        public MarkerOptions markerOpt1;
        public MarkerOptions markerOpt2;
        public GoogleMap mMap;

       // public List<User> myInstance;
        public static string givenRouteName;
        static string routeInfo;
        static string routeDifficulty;
        static string routeType;
        static string routeUserId;

        public List<Location> points;

        public static Location startLocation;
        public static Location endLocation;
        public Location currentLocation;

        public static bool isChecked;
        public static bool alreadyDone;
        public static bool isReady;
        public static TextView routeStatus;
        public static ImageView statusImage;
        public static string routeId;

        public static double dist;

        public Stopwatch stopWatch;
        public static string elapsedTime;
        public static Activity activity;
        public Spinner spinner;
        public ToggleButton start;
        public static bool isPaused;

        static bool firstRun;
        ListView list;
        public int typeToDraw;
        public static Bitmap snapShot;
        TextView selectRouteType;
        public static int score;
        public int mypoints;
        Marker myMark;
        Marker myFirstMark;
        Button startCreate;
        ImageButton stopCreate;
        Location previousLocation;

        PolylineOptions trackingLine;
        List<Polyline> polylines;
        public bool Ischecked
        {

            set
            {

                isChecked = value;

                if (!alreadyDone && isChecked && isReady)
                {
                    //  startRouteSettings();
                    alreadyDone = true;
                }
            }

            get { return isChecked; }
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            MoveTaskToBack(true);

        }

        public void Share(string title, string content)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
                return;

            Bitmap b = BitmapFactory.DecodeResource(Resources, Resource.Drawable.test);
          
            var tempFilename = "test.png";
            var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var filePath = System.IO.Path.Combine(sdCardPath, tempFilename);
            using (var os = new FileStream(filePath, FileMode.Create))
            {
                b.Compress(Bitmap.CompressFormat.Png, 100, os);
            }
            b.Dispose();

            var imageUri = Android.Net.Uri.Parse($"file://{sdCardPath}/{tempFilename}");
            var sharingIntent = new Intent();
            sharingIntent.SetAction(Intent.ActionSend);
            sharingIntent.SetType("image/*");
            sharingIntent.PutExtra(Intent.ExtraText, content);
            sharingIntent.PutExtra(Intent.ExtraStream, imageUri);
            sharingIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
            StartActivity(Intent.CreateChooser(sharingIntent, title));
        }
      

        protected  override void OnCreate(Bundle savedInstanceState)
        {
           // RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.createRoute);

            activity = this;
            isPaused = false;
            firstRun = true;

            typeToDraw = 0;

            points = new List<Location>();
          //  myInstance = await Azure.getUserByAuthId(MainStart.auth0UserId);

            MapFragment mapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map);
            mMap = mapFrag.Map;

          

            toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            SupportActionBar.SetDisplayShowTitleEnabled(true);

            markerOpt1 = new MarkerOptions();
            markerOpt2 = new MarkerOptions();

            if (mMap != null)
            {
                mMap.MapType = GoogleMap.MapTypeTerrain;  // The GoogleMap object is ready to go.
            }
            mMap.UiSettings.ZoomControlsEnabled = true;
            mMap.UiSettings.CompassEnabled = true;
            //mMap.SetOnMyLocationChangeListener;

            //spinner = FindViewById<Spinner>(Resource.Id.createRoute);

            //spinner.ItemSelected += spinner_ItemSelected;

            list = FindViewById<ListView>(Resource.Id.createRoute);
            //  spinner_ItemSelected; 
          //  list.ItemSelected += List_ItemSelected;
           

          String[] array = { "Walking", "Running", "Hiking", "Bicycling", "Skiing","Kayaking" };
            ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemChecked, array);
            //spinner.Adapter = adapter;
            //spinner.SetSelection(0);
           // list.SetSelection(0);
            list.Adapter = adapter;
            list.ChoiceMode = Android.Widget.ChoiceMode.Single;
            list.SetItemChecked(0, true);
            routeType = "Walking";

            list.ItemClick += (a, e) =>
            {
                var item = this.list.GetItemAtPosition(e.Position);

                //Make a toast with the item name just to show it was clicked
              //  Toast.MakeText(this, item.ToString() + " Clicked!", ToastLength.Short).Show();

                if (e.Position == 0)
                {
                    Toast.MakeText(this, "Walking", ToastLength.Short).Show();
                    routeType = "Walking";
                    typeToDraw = 1;
                }
                else if (e.Position == 1)
                {
                    Toast.MakeText(this, "Running", ToastLength.Short).Show();
                    routeType = "Running";
                    typeToDraw = 2;
                }
                else if (e.Position == 2)
                {
                    Toast.MakeText(this, "Hiking", ToastLength.Short).Show();
                    routeType = "Hiking";
                     typeToDraw = 3;
                }
                else if (e.Position == 3)
                {
                    Toast.MakeText(this, "Bicycling", ToastLength.Short).Show();
                    routeType = "Bicycling";
                    typeToDraw = 4;
                }
                else if (e.Position == 4)
                {
                    Toast.MakeText(this, "Skiing", ToastLength.Short).Show();
                    routeType = "Skiing";
                    typeToDraw = 5;
                }

                else if (e.Position == 5)
                {
                    Toast.MakeText(this, "kayaking", ToastLength.Short).Show();
                    routeType = "kayaking";
                    typeToDraw = 6;
                }
            };


        


            statusImage = FindViewById<ImageView>(Resource.Id.imageStatus);
            TextView routeTitle = FindViewById<TextView>(Resource.Id.routeTitle);
             selectRouteType = FindViewById<TextView>(Resource.Id.textView1);
            routeStatus = FindViewById<TextView>(Resource.Id.statusRoute);
            routeStatus.Text = "Stauts: Idle";

          
             startCreate = FindViewById<Button>(Resource.Id.startRunning);
             stopCreate = FindViewById<ImageButton>(Resource.Id.stopRunning);
            stopCreate.Visibility = ViewStates.Invisible;

            //start = FindViewById<ToggleButton>(Resource.Id.toggleStart);
          
            //start.SetBackgroundResource(Resource.Drawable.rsz_running);
            //start.Text = "";
            
        

            routeId = "";

            startCreate.Click += (sender, e) =>
          {
              if (StartRouteService.serviceIsRunning == true)
              {
                  Toast.MakeText(this, "Cannot create a route while doing one!", ToastLength.Long).Show();

                  return;
              }
              selectRouteType.Visibility = ViewStates.Invisible;

              Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);

              alert.SetTitle("Tracking");
              alert.SetMessage("Do you want to enable live tracking?");
              alert.SetPositiveButton("Yes", (senderAlert, args) =>
              {
                  trackingLine = new PolylineOptions();
                  polylines = new List<Polyline>();
                  startRouteCreation();
                  App.Current.LocationService.LocationChanged += HandleLocationChanged;


              });

              alert.SetNegativeButton("No", (senderAlert, args) =>
              {
                  startRouteCreation();

              });
              //run the alert in UI thread to display in the screen
              RunOnUiThread(() =>
              {
                  alert.Show();
              });

             
              
          };

            stopCreate.Click += (sender, e) =>
            {

                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);

                alert.SetTitle("Finish route");
                alert.SetMessage("Do you want to finish current route?");
                alert.SetPositiveButton("Yes", (senderAlert, args) =>
                {
                    finishRoute();
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                 

                });
               
               
                    alert.Show();
               
             

            };

            //    start.Click += (sender, e) =>
            //    {

            //        ////Cannot create and do a route at the same time!
            //        if (StartRouteService.serviceIsRunning == true)
            //        {
            //            Toast.MakeText(this, "Cannot create a route while doing one!", ToastLength.Long).Show();

            //            return;
            //        }



            //        if (start.Checked)
            //        {
            //            startRouteCreation();
            //        }
            //        else
            //        {


            //            finishRoute();
            //        }





            //    };

        }



        public void finishRoute()
        {

            if (CreateRouteService.serviceIsRunning == false)
            {
                Toast.MakeText(this, "Nothing to stop!", ToastLength.Short).Show();
                return;

            }


            App.Current.LocationService.LocationChanged -= HandleLocationChanged;

            points = CreateRouteService.getPoints();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);


            if (points.Count == 0)
            {
                StopService(new Intent(this, typeof(CreateRouteService)));

                Finish();
                Intent myIntent = new Intent(this, typeof(CreateRoute));
                StartActivity(myIntent);
                Toast.MakeText(this, "Unable to create route, no gps data found!", ToastLength.Long).Show();
                return;
            }          
            try
            {

                points = filterLocationPoints(points);
                
            }
            catch (Exception)
            {


            }


            StopService(new Intent(this, typeof(CreateRouteService)));

            routeStatus.Text = "Stauts: Stopped";
            dist = 0;
            Toast.MakeText(this, "Ending route...", ToastLength.Short).Show();
            statusImage.SetImageResource(Resource.Drawable.red);
         
           if(polylines != null && polylines.Count > 0)
            {
                //foreach (var item in polylines)
                //{
                //    item.Remove();
                //}
                //polylines.Clear();
              //  mMap.Snapshot(this);
            }else
            drawRoute(typeToDraw);

          //  mMap.Snapshot(this);

            dist = calculateDistance();

            if (dist == 0)
            {
                dist = getDistanceForRoute(startLocation, endLocation);

            }

            //if (dist < 180)
            //{
            //    Toast.MakeText(this, "Route is too short to be uploaded!", ToastLength.Long).Show();
            //    App.Current.LocationService.LocationChanged -= HandleLocationChanged;

            //    StopService(new Intent(this, typeof(CreateRouteService)));
            //    Finish();
            //    return;
            //}
            //else
            //{

            //    mypoints = MyPoints.calculatePoints(routeType, dist);
            //    score = mypoints;

            //    //startDialogNameRoute();
            //    //firstRun = true;

            //}

            mypoints = MyPoints.calculatePoints(routeType, dist);
            score = mypoints;


          //  mMap.Snapshot(this);
            startDialogNameRoute();
            firstRun = true;


        }


        public void startDialogNameRoute()
        {

            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            DialogEndRouteCreationStats dialog = new DialogEndRouteCreationStats();
            dialog.DialogClosed += OnDialogClosedOne;
            dialog.Show(transaction, "Route Stats");


           
        }

         void OnDialogClosedOne(object sender, DialogEndRouteCreationStats.DialogEventArgs e)
        {
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            DialogStartRoute newDialog = new DialogStartRoute();
            newDialog.DialogClosed += OnDialogClosed;
            newDialog.Show(transaction, "Start Route");

        }

        async void OnDialogClosed(object sender, DialogStartRoute.DialogEventArgs e)
        {
            String[] returnData;
            returnData = e.ReturnValue.Split(',');
            givenRouteName = returnData[0];
            routeInfo = returnData[1];
            routeDifficulty = returnData[2];
            routeUserId = MainStart.userId; /// myInstance.FirstOrDefault().Id;


            try
            {

                routeStatus.Text = "Please wait for route to upload...";
            //    dist = calculateDistance();
                var first = points[0];



                List<Route> routeHere = await Azure.AddRoute(givenRouteName, routeInfo, dist.ToString(), "0", 1, routeDifficulty, routeType, routeUserId, elapsedTime, first.Latitude, first.Longitude);
                var addedDistance = await Azure.addToMyDistance(MainStart.userId, dist);
             
                string routeID = "";
             
                List<Route> te = await Azure.getLatestRouteId(routeUserId);
                routeID = te.FirstOrDefault().Id;


                if (points.Count != 0 && routeID != "")
                {
                    foreach (var item in points)
                    {

                        Azure.AddLocation(item.Latitude, item.Longitude, routeID);
                    }

                }
                else
                {

                    Toast.MakeText(this, "Cannot upload this route!", ToastLength.Long).Show();

                    return;
                }

                //if (dist == 0)
                //    dist = getDistanceForRoute(startLocation, endLocation);


                // mypoints = MyPoints.calculatePoints(routeType, (int)dist);
                mypoints = Convert.ToInt32(mypoints);
                var pointAdded = await Azure.addToMyPoints(routeUserId, mypoints);
              //  score = mypoints;
                statusImage.SetImageResource(Resource.Drawable.orange);
                Toast.MakeText(this, "Uploading successful", ToastLength.Long).Show();
                routeStatus.Text = "Status: Idle";
                //Toast.MakeText(this, "You have earned " + mypoints + " points on on a " + routeType + " route", ToastLength.Long).Show();

                //start.Enabled = true;

                selectRouteType.Visibility = ViewStates.Visible;
                list.Visibility = ViewStates.Visible;

            }
            catch (Exception)
            {

               
            }
            startCreate.Visibility = ViewStates.Visible;
            stopCreate.Visibility = ViewStates.Invisible;

            Finish();
            Intent myIntent = new Intent(this, typeof(CreateRoute));
            StartActivity(myIntent);
            


        }
        public void startRouteCreation()
        {
            //start.Enabled = false;
            //start.SetBackgroundColor(Color.Blue);
            startCreate.Visibility = ViewStates.Invisible;

            stopCreate.Visibility = ViewStates.Visible;
            stopCreate.Enabled = false;

            list.Visibility = ViewStates.Invisible;

            if (points.Count > 0)
            {

                points.Clear();
            }
        
            routeStatus.Text = "Acquiring your position...";

            var loc = App.Current.LocationService.getLastKnownLocation();

            if (loc != null)
            {
                mMap.Clear();
                mMap.MoveCamera(CameraUpdateFactory.ZoomIn());
                mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(loc.Latitude, loc.Longitude), 13));

                MarkerOptions markerMe = new MarkerOptions();
                markerMe.SetPosition(new LatLng(loc.Latitude, loc.Longitude));
                markerMe.SetTitle("My position");
                markerMe.Draggable(false);
                BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(MainStart.profilePic);
                markerMe.SetIcon(image);
                myFirstMark = mMap.AddMarker(markerMe);

            }


            StartService(new Intent(this, typeof(CreateRouteService)));
            isReady = false;
            Ischecked = false;
            alreadyDone = false;

            stopWatch = new Stopwatch();
            stopWatch.Start();
            stopCreate.Enabled = true;
        
        }

        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            if (myFirstMark != null)
                myFirstMark.Remove();

            drawRouteLive(e.Location);
        }


        public List<Location> filterLocationPoints(List<Location> points)
        {
            List<Location> sortedLocation = new List<Location>();
            double dist = 0;

                for (int i = 0; i < points.Count; i++)
            {
                if(points[i + 1] != null)
                {

                    var firstPoint = points[i];
                    var secondPoint = points[i + 1];

                    dist = getDistanceForRoute(firstPoint, secondPoint);
                    if(dist >= 8)
                    {
                        sortedLocation.Add(points[i]);

                    }

                }
          

            }

            return sortedLocation;
        }


        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;
        }

        // Set very accurate Accuracy for locating
      


        protected override void OnResume()
        {
              base.OnResume();
          

        }
        protected override void OnPause()
        {
          base.OnPause();
           
        }


     
        public double getDistanceForRoute(Location start, Location end)
        {

            double distance = 0;
            try
            {
                distance = calculateDistance(start.Latitude, start.Longitude, end.Latitude, end.Longitude);
            }
            catch (Exception)
            {

               
            }
            return distance;
        }

        public double calculateDistance()
        {

            double totalLength = 0;

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i + 1] != null)
                    totalLength += calculateDistance(points[i].Latitude, points[i].Longitude, points[i + 1].Latitude, points[i + 1].Longitude);

            }

            return totalLength;
        }

        public double calculateDistance(double fromLatitude, double fromLongitude, double toLatitude, double toLongitude)
        {

            float[] results = new float[1];
            int distance = 0;
            LatLng source = new LatLng(fromLatitude, fromLongitude);
            LatLng destination = new LatLng(toLatitude, toLongitude);

            try
            {
                Location.DistanceBetween(fromLatitude, fromLongitude, toLatitude, toLongitude, results);
            }
            catch (Exception)
            {

            }
            if (source.Equals(destination))
            {
                distance = 0;
            }
            else
            {
                distance = (int)results[0];


            }
            return distance;
        }


        public override bool OnCreateOptionsMenu(IMenu menu)

        {
            MenuInflater.Inflate(Resource.Menu.action_menu_nav, menu);

          

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {

                case Android.Resource.Id.Home:// Resource.Id.back:
                    OnBackPressed();
                    return true;

                case Resource.Id.home:

                    //Intent myIntent = new Intent(this, typeof(WelcomeScreen));
                    //StartActivity(myIntent);

                    OnBackPressed();
                   

                    return true;

                default:
                    return base.OnOptionsItemSelected(item);

            }


        }

    
        public void drawRouteLive(Location loc)
        {

            if(myMark != null)
            {
                myMark.Remove();
            }

       

            Bitmap flagStart = BitmapFactory.DecodeResource(Resources, Resource.Drawable.startF);
            var startPoint = BitmapDescriptorFactory.FromBitmap(IOUtilz.scaleDown(flagStart, 80, false)); //(Resource.Drawable.test);
           
            MarkerOptions liveTrackingMarker = new MarkerOptions();
            liveTrackingMarker.SetPosition(new LatLng(loc.Latitude, loc.Longitude));
            liveTrackingMarker.SetTitle("My position");
            liveTrackingMarker.Draggable(false);
            
            BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(MainStart.profilePic);
            liveTrackingMarker.SetIcon(image);
            myMark = mMap.AddMarker(liveTrackingMarker);

            Polyline line = null; 
            if(previousLocation != null && calculateDistance(loc.Latitude,loc.Longitude, previousLocation.Latitude, previousLocation.Longitude) >= 15 )
            {
                trackingLine.Add(new LatLng(loc.Latitude, loc.Longitude));
               line = mMap.AddPolyline(trackingLine);
                polylines.Add(line);
            }
            previousLocation = loc;

            mMap.Snapshot(this);
        }


        public void drawRoute(int type)
        {
            Location lastItem = points.LastOrDefault();
            Location firstElement = points.First();

            mMap.Clear();


            Bitmap flagStart = BitmapFactory.DecodeResource(Resources, Resource.Drawable.startF);
            var startPoint = BitmapDescriptorFactory.FromBitmap(IOUtilz.scaleDown(flagStart, 80, false)); //(Resource.Drawable.test);

            markerOpt1.SetPosition(new LatLng(firstElement.Latitude, firstElement.Longitude));
            markerOpt1.SetTitle("Starting Point");
            markerOpt1.Draggable(false);
            markerOpt1.SetSnippet("Starting point of route");

          //  markerOpt1.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));
            markerOpt1.SetIcon(startPoint);
            mMap.AddMarker(markerOpt1);


            Bitmap flagEnd = BitmapFactory.DecodeResource(Resources, Resource.Drawable.finishF);
            var endPoint = BitmapDescriptorFactory.FromBitmap(IOUtilz.scaleDown(flagEnd, 80, false)); //(Resource.Drawable.test);


            markerOpt2 = new MarkerOptions();
            markerOpt2.SetPosition(new LatLng(lastItem.Latitude, lastItem.Longitude));
            markerOpt2.SetTitle("Ending Point");
            markerOpt2.Draggable(false);
            markerOpt2.SetSnippet("End point of route");
           // markerOpt2.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));
            markerOpt2.SetIcon(endPoint);

            mMap.AddMarker(markerOpt2);

            mMap.MoveCamera(CameraUpdateFactory.ZoomIn());
            mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(firstElement.Latitude, firstElement.Longitude), 13));

            PolylineOptions opt = new PolylineOptions();

           
            if(type == 1)
            {
                opt.InvokeColor(Color.Black);

            }else if( type == 2)
            {
                opt.InvokeColor(Color.IndianRed);
            }else if(type == 3)
            {
                opt.InvokeColor(Color.BlueViolet);

            }
            else if (type == 4)
            {
                opt.InvokeColor(Color.DarkOrchid);
            }else if(type == 5)
            {
                opt.InvokeColor(Color.DarkOliveGreen);
            }else
            {

                opt.InvokeColor(Color.HotPink);
            }
            

            foreach (var item in points)
            {
                opt.Add(new LatLng(item.Latitude, item.Longitude));
            }

            mMap.AddPolyline(opt);

            mMap.Snapshot(this);

        }

        void ISnapshotReadyCallback.OnSnapshotReady(Bitmap snapshot)
        {
            snapShot = snapshot;
        }
    }







}


