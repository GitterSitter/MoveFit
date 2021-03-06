using System;
using Android;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using System.Collections.Generic;
using Android.Graphics;
using Android.Content;
using Android.Support.V7.App;
using static Android.Gms.Maps.GoogleMap;
using Java.Util;
using System.Threading.Tasks;

namespace TestApp
{

    public class StartMapFragment : Fragment, IOnMapReadyCallback, IOnMarkerClickListener
    {

        MapView mMapView;
        private GoogleMap mMap;
        int[] unit;    
        List<User> me;
        //   public SupportToolbar toolbar;
        HashMap mHashMapRoute;
        HashMap mHashMapUser;

        public string routeName;
        public string routeInfo;
        public string routeRating;
        public string routeDifficulty;
        public string routeLength;
        public string routeType;
        public int routeTrips;
        public string routeId;
        public string routeTime;
        public string routeUserId;

        public int activeScreen;

        public string userName;
        public string userGender;
        public int userAge;
        public string userProfileImage;
        public int userPoints;
        public string userAboutMe;
        public string userID;
        Bitmap picMe;
        BitmapDescriptor imageMe;
        Marker myMark;
       
        public override  View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.startMapFragment, container, false);
            Button createRoute = view.FindViewById<Button>(Resource.Id.createRoute);
            mHashMapUser = new HashMap();

            if (this.Activity.Class.SimpleName != "FriendsOverview")
            {
                 Bitmap icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.startFlag);
                //  TextView createRouteLabel = view.FindViewById<TextView>(Resource.Id.textRoute);
              
                createRoute.Click += (sender, e) =>
                {
                    Intent myIntent = new Intent(this.Activity, typeof(CreateRoute));
                    StartActivity(myIntent);
                };
                
                mHashMapRoute = new HashMap();
                me = RouteOverview.me;
                unit = RouteOverview.unit;
                activeScreen = 0;
            }
            else
            {
             
                activeScreen = 1;
              
                me = FriendsOverview.me;
                unit = FriendsOverview.unit;
                createRoute.Visibility = ViewStates.Invisible;
               

               
            }

            App.Current.LocationService.LocationChanged += HandleCustomEvent;

            mMapView = view.FindViewById<MapView>(Resource.Id.map);

            try
            {
                mMapView.OnCreate(savedInstanceState);
                mMapView.OnResume();
            }
            catch (Exception)
            {

               
            }



            try
            {

                MapsInitializer.Initialize(this.Activity.ApplicationContext);
                mMapView.GetMapAsync(this);
            }
            catch (Exception)
            {

            }


        

            return view;

        }


      public void setUp()
        {
            picMe = MainStart.profilePic;
            imageMe = BitmapDescriptorFactory.FromBitmap(IOUtilz.scaleDown(picMe, 70, false)); //(Resource.Drawable.test);

            Location loc = null;
            try
            {
                loc = App.Current.LocationService.getLastKnownLocation();

            }
            catch (Exception)
            {

            }


            LatLng myPos = null;


            try
            {
                if (loc.Latitude != 000000 && loc.Longitude != 000000 && loc != null)
                {
                    myPos = new LatLng(loc.Latitude, loc.Longitude);
                }
                else
                    myPos = new LatLng(MainStart.currentLocation.Latitude, MainStart.currentLocation.Longitude);


                if (myPos == null)
                    myPos = new LatLng(MainStart.userInstanceOne.Lat, MainStart.userInstanceOne.Lon);

            }
            catch (Exception)
            {

            }



            if (this.Activity.Class.SimpleName != "FriendsOverview")
            {
                foreach (var item in RouteOverview.routes)
                {
                    try
                    {
                        setMarkerRoutes(item);
                    }
                    catch (Exception)
                    {


                    }

                }

            }
            else
            {

                foreach (var item in FriendsOverview.myFriends)
                {

                    if (item != null && item.Online == true)
                    {

                            setMarkerUser(item);
                    
                    }

                }

            }


            try
            {
                mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myPos, 11));
                myMark = mMap.AddMarker(new MarkerOptions()
               .SetPosition(myPos)
               .SetTitle(MainStart.userName)
               .SetSnippet("My Location").SetIcon(imageMe));

                mHashMapUser.Put(myMark, MainStart.userId);

            }
            catch (Exception )
            {

                
            }


        }

        public void setMarkerRoutes(Route route)
        {

            if(route != null)
            {

           
            try
            {

          
            LatLng myPosition = new LatLng(route.Lat, route.Lon);
            float[] result = new float[1];
            Location.DistanceBetween(me[0].Lat, me[0].Lon, route.Lat, route.Lon, result);
            int dist = Convert.ToInt32(result[0]);

            if (unit[1] == 0)
            {
                dist = dist / 1000;
            }
            else if(dist == 0)
                {
                    dist = 0;
                }
                else
                dist = Convert.ToInt32(IOUtilz.ConvertKilometersToMiles(dist / 1000));



                Bitmap largeIcon = null;
                BitmapDescriptor image = null;
                if (route.RouteType == "Walking")
                {
                    largeIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.wa);
                    image = BitmapDescriptorFactory.FromBitmap(IOUtilz.getRoundedShape(largeIcon)); //(Resource.Drawable.test);

                    
                }
                else if (route.RouteType == "Running")
                {
                    largeIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.ru);
                    image = BitmapDescriptorFactory.FromBitmap(IOUtilz.getRoundedShape(largeIcon)); //(Resource.Drawable.test);

                }
                else if (route.RouteType == "Hiking")
                {
                    largeIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.tr);
                    image = BitmapDescriptorFactory.FromBitmap(IOUtilz.getRoundedShape(largeIcon)); //(Resource.Drawable.test);


                }
                else if (route.RouteType == "Bicycling")
                {
                    largeIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.cy);
                    image = BitmapDescriptorFactory.FromBitmap(IOUtilz.getRoundedShape(largeIcon)); //(Resource.Drawable.test);


                }
                else if (route.RouteType == "Skiing")
                {
                    largeIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.sk);
                    image = BitmapDescriptorFactory.FromBitmap(IOUtilz.getRoundedShape(largeIcon)); //(Resource.Drawable.test);


                }
                else if (route.RouteType == "Kayaking")
                {
                    largeIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.ka);
                    image = BitmapDescriptorFactory.FromBitmap(IOUtilz.getRoundedShape(largeIcon)); //(Resource.Drawable.test);


                }


                Marker m = mMap.AddMarker(new MarkerOptions()
           .SetPosition(myPosition)
           .SetTitle(route.Name + "(" + route.Difficulty + ")")
           .SetSnippet(dist.ToString() + RouteOverview.distanceUnit).SetIcon(image));
           
                mHashMapRoute.Put(m, route.Id);



            }
            catch (Exception a)
            {
              

            }

            }

        }


        public void setMarkerUser(User user)
        {

            if(user != null)
            {

           
            try
            {


                LatLng myPosition = new LatLng(user.Lat, user.Lon);
                float[] result = new float[1];
                Location.DistanceBetween(me[0].Lat, me[0].Lon, user.Lat, user.Lon, result);
                int dist = Convert.ToInt32(result[0]);

                if (unit[1] == 0)
                {
                    dist = dist / 1000;
                }
                else if (dist == 0)
                {
                    dist = 0;
                }
                else
                    dist = Convert.ToInt32(IOUtilz.ConvertKilometersToMiles(dist / 1000));


               var  pic = IOUtilz.GetImageBitmapFromUrl(user.ProfilePicture);
              var image = BitmapDescriptorFactory.FromBitmap(pic);

               
                 
                Marker mark = mMap.AddMarker(new MarkerOptions()
               .SetPosition(myPosition)
               .SetTitle(user.UserName)
               .SetSnippet(dist.ToString() +" km").SetIcon(image));
                mHashMapUser.Put(mark, user.Id);
              
            }
            catch (Exception a)
            {
                   

            }

            }

        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mMap = googleMap;

            mMap.MapType = GoogleMap.MapTypeTerrain;  // The GoogleMap object is ready to go.
            mMap.UiSettings.ZoomControlsEnabled = true;
            mMap.UiSettings.RotateGesturesEnabled = true;
            mMap.UiSettings.ScrollGesturesEnabled = true;
            mMap.SetOnMarkerClickListener(this);

            //picMe = MainStart.profilePic;
            //imageMe = BitmapDescriptorFactory.FromBitmap(IOUtilz.scaleDown(picMe, 70, false)); //(Resource.Drawable.test);

            //Location loc = null;
            //try
            //{
            //     loc = App.Current.LocationService.getLastKnownLocation();

            //}
            //catch (Exception)
            //{

            //}


            //LatLng myPos = null;


            //try
            //{
            //    if(loc.Latitude != 000000 && loc.Longitude != 000000 && loc != null)
            //    {
            //        myPos = new LatLng(loc.Latitude, loc.Longitude);
            //    }else
            //        myPos = new LatLng(MainStart.currentLocation.Latitude, MainStart.currentLocation.Longitude);


            //    if (myPos == null)
            //    myPos = new LatLng(MainStart.userInstanceOne.Lat, MainStart.userInstanceOne.Lon);

            //}
            //catch (Exception)
            //{

            //}



            //if (this.Activity.Class.SimpleName != "FriendsOverview")
            //{
            //    foreach (var item in RouteOverview.routes)
            //    {
            //        try
            //        {
            //            setMarkerRoutes(item);
            //        }
            //        catch (Exception)
            //        {


            //        }

            //    }

            //}else
            //{

            //    foreach (var item in FriendsOverview.myFriends)
            //    {

            //        if(item.Online == true)
            //        {


            //            try
            //            {
            //                setMarkerUser(item);
            //            }
            //            catch (Exception a)
            //            {

            //                throw a;
            //            }


            //        }

            //    }

            //}


            //try
            //{
            //    mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myPos, 11));
            //    myMark = mMap.AddMarker(new MarkerOptions()
            //   .SetPosition(myPos)
            //   .SetTitle(MainStart.userName)
            //   .SetSnippet("My Location").SetIcon(imageMe));
            //    mHashMapUser.Put(myMark, MainStart.userId);
            //}
            //catch (Exception r)
            //{

            //    throw r;
            //}

            setUp();

        }


        public bool OnMarkerClick(Marker marker)
        {


            if (marker == myMark || marker.Equals(myMark)) 
                return false;

            Activity.RunOnUiThread( async () =>
            {

     
                if(activeScreen == 0)
                {

                    string routeIdentification = (string)mHashMapRoute.Get(marker);
                    List<Route> routeList = await Azure.getRouteById(routeIdentification);
                    Route routeInstance = routeList[0];


                    routeName = routeInstance.Name;
                    routeInfo = routeInstance.Info;
                    routeDifficulty = routeInstance.Difficulty;
                    routeLength = routeInstance.Distance;
                    routeType = routeInstance.RouteType;
                    routeRating = routeInstance.Review;
                    routeTrips = routeInstance.Trips;
                    routeId = routeInstance.Id;
                    routeTime = routeInstance.Time;
                    routeUserId = routeInstance.User_id;



                    Bundle b = new Bundle();
                    b.PutStringArray("MyData", new String[] {

            routeName,
            routeInfo,
            routeDifficulty,
            routeLength,
            routeType,
            routeRating,
            routeTrips.ToString(),
            routeId,
            routeTime,
            routeUserId

        });

                    Intent myIntent = new Intent(this.Activity, typeof(StartRoute));
                    myIntent.PutExtras(b);
                    Activity.StartActivity(myIntent);
                }


                else
                {

                    string userIdentiication = (string)mHashMapUser.Get(marker);

                    User userInstance =  await Azure.getUser(userIdentiication);

                    userName = userInstance.UserName;
                    userGender = userInstance.Sex;
                    userAge = userInstance.Age;
                    userProfileImage = userInstance.ProfilePicture;
                    userPoints = userInstance.Points;
                    userAboutMe = userInstance.AboutMe;
                    userID = userInstance.Id;

                    Bundle ba = new Bundle();
                    ba.PutStringArray("MyData", new String[] {

                    userName,
                    userGender,
                    userAge.ToString(),
                    userProfileImage,
                    userPoints.ToString(),
                    userAboutMe,
                    userID

                });

                    Intent myIntent = new Intent(this.Activity, typeof(UserProfile));
                    myIntent.PutExtras(ba);
                    this.Activity.StartActivity(myIntent);


                }


            });


            return true;

        }


        void HandleCustomEvent(object e, LocationChangedEventArgs a)
        {
            if(myMark != null)
            myMark.Remove();

            Location loc = null;
            try
            {
                loc = App.Current.LocationService.getLastKnownLocation();

            }
            catch (Exception)
            {

              
            }
            LatLng locat = null; 

            if (loc == null)
            {
                //   loc.Latitude = MainStart.userInstanceOne.Lat;
                //   loc.Longitude = MainStart.userInstanceOne.Lon;
                

                locat = new LatLng(MainStart.userInstanceOne.Lat, MainStart.userInstanceOne.Lon);
            }else
            {

                locat = new LatLng(loc.Latitude, loc.Longitude);
            }

            try
            {

          
            myMark = mMap.AddMarker(new MarkerOptions()
               .SetPosition(locat)
               .SetTitle(MainStart.userName)
               .SetSnippet("My Location").SetIcon(imageMe));

                mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng (a.Location.Latitude,a.Location.Longitude), 11));

            }
            catch (Exception)
            {

              
            }


        }



        public override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;

            //if(mHashMapUser == null)
            //mHashMapUser = new HashMap();

       

        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnResume()
        {
            base.OnResume();
           mMapView.OnResume();

            //if (mHashMapUser == null)
            //    mHashMapUser = new HashMap();
        }

       
        public override void OnPause()
        {
            base.OnPause();
          mMapView.OnPause();
        }

        
     public override void OnDestroy()
        {
            base.OnDestroy();
            App.Current.LocationService.LocationChanged -= HandleCustomEvent;
            mMapView.OnDestroy();
        }

      
  

    }

}