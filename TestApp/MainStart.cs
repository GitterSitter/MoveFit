﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Android.Gms.Maps.GoogleMap;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
//using Gcm.Client;
using System.Text;
using Android.Views.Animations;
using Android.Support.V7.Widget;

namespace TestApp
{


    [Activity(Label = "MainMenu", Theme = "@style/MyTheme", Icon = "@drawable/test", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainStart : AppCompatActivity, ILocationListener
    {
        public static MainStart instance;
        public readonly string logTag = "MainActivity"; 
        ActionBarDrawerToggle mDrawerToggle;
        DrawerLayout mDrawerLayout;
        ListView mLeftDrawer;
        ListView mRightDrawer;
        List<string> mLeftDataSet; 
        public string[] table;
        public string text;
        public static string auth0UserId;
        public static string userId;
        public ImageView profilePicture;
        public static Bitmap profilePic;
        public string profilePictureUrl;

        Intent myIntent;
        public Azure azure;
        public static bool changed;
        public static Location loc;
        public static Location currentLocation;
        public static string userName;
        public static String[] array;
        public List<User> user;
        public static bool chk;
        public static TextView _address;       
        public static GoogleMap mMap;      
        public static Activity mainActivity;
        public static User waitingUpload;
        public static User userInstanceOne;
        static Geocoder geocoder;
        public static bool isOnline;
       public static RecyclerView mRecyclerView;
      public static  RecyclerView.LayoutManager mLayoutManager;
     public static   RecyclerView.Adapter adpter;
        public static ConnectivityManager connectivityManager;
        public static IMenuItem menItemOnlineIcion;
        public static IMenuItem menItemOnlineText;
        public static IMenuItem menItemLogo;
        TextView messages;
        public static string pushNotifUserId;
        public static string myUserName;
        public static string userList;
        public static List<MessageDetail> currentMessagesWritten;
        public static List<UserDetail> listOfConnectedUsers;
        public List<NavDrawerItem> data;
        public string routesNearby;
        public string routesCreated;
        public string friends;
        public string totalDistance;
        public string points;
        public static List<User> top;
        public static List<User> topUsers;
        static TextView titleTopFriends;
        static ImageView pictureFriend1;
        static ImageView pictureFriend2;
        static ImageView pictureFriend3;
        static TextView pers2;
        static TextView pers3;
        static TextView pers1;
        public bool dialogOpen;
        Animation myAnimation;

        bool firstLocationUpdate;

        List<User> listOfUserFriends;


        List<User> moa;
        public class NavDrawerAdapter :  BaseAdapter 
{
        
            private readonly Context context;
            private readonly int layoutResourceId;
            private List<NavDrawerItem> data;
            public NavDrawerAdapter(Context context, int layoutResourceId, List<NavDrawerItem> data) { 
               
                    this.context = context;
                    this.layoutResourceId = layoutResourceId;
                    this.data = data;
                 
            }
          
            public override int Count
            {
                get { return data.Count; }//_contactList.Count; }
            }

            public override Java.Lang.Object GetItem(int position)
            {
                // could wrap a Contact in a Java.Lang.Object
                // to return it here if needed
                return data[position].name;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
        {
            
                var v = convertView ?? MainStart.mainActivity.LayoutInflater.Inflate(Resource.Layout.leftDrawerAdapter, parent, false);
            ImageView imageView = v.FindViewById<ImageView>(Resource.Id.img);
            TextView textView =v.FindViewById<TextView>(Resource.Id.titleMen);

            NavDrawerItem choice = data[position];

                if(position == 0)
                {
                    imageView.SetImageBitmap(profilePic);
                    textView.Text = choice.name;
                }else
                {
                    imageView.SetImageResource(choice.icon);
                    textView.Text = choice.name;
                }
         

            return v;
        }




        }

      

        public class NavDrawerItem
        {
            public int icon;
            public string name;
            public NavDrawerItem() { }
            public NavDrawerItem(int icon, string name)
            {
                this.icon = icon;
                this.name = name;
            }
         
        }

        

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.drawerLayout);
         
            TestIfGooglePlayServicesIsInstalled();
            SupportToolbar  mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);

            firstLocationUpdate = false;

            array = Intent.GetStringArrayExtra("MyData");
            userName = array[0];
            profilePictureUrl = array[1];
            profilePic = IOUtilz.GetImageBitmapFromUrl(array[1]);
            auth0UserId = array[3] + "-" + array[2];

            geocoder = new Geocoder(this);
            mainActivity = this;
            instance = this;
            dialogOpen = false;
            changed = false;
            user = null;
            chk = false;
            isOnline = false;
            waitingUpload = null;
            userInstanceOne = null;


          


            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            SetSupportActionBar(mToolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            mRightDrawer = FindViewById<ListView>(Resource.Id.ContactsListView);
            mLeftDrawer.Tag = 0;
            mRightDrawer.Tag = 1;      

            messages = FindViewById<TextView>(Resource.Id.messageInfo);
            messages.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            messages.TextSize = 24;

            ImageButton distButton = FindViewById<ImageButton>(Resource.Id.distanceButton);
            ImageButton routeButton = FindViewById<ImageButton>(Resource.Id.routeButton);
            ImageButton scoreButton = FindViewById<ImageButton>(Resource.Id.scoreButton);
            Bitmap icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.rsz_dist);
            distButton.SetImageBitmap(IOUtilz.getRoundedShape(icon));
            icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.rsz_route);
            routeButton.SetImageBitmap(IOUtilz.getRoundedShape(icon));
            icon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.rsz_score);
            scoreButton.SetImageBitmap(IOUtilz.getRoundedShape(icon));
            
            var layout = FindViewById<RelativeLayout>(Resource.Id.messages);
            mLeftDataSet = new List<string>();
          
            mLeftDataSet.Add("Scoreboard");
            mLeftDataSet.Add("Routes");
            mLeftDataSet.Add("Social");
            mLeftDataSet.Add("Settings");     
            mLeftDataSet.Add("My Profile");
            mLeftDataSet.Add("About");
            mLeftDataSet.Add("Logout");

            data = new List<NavDrawerItem>();
            data.Add(new NavDrawerItem(Resource.Drawable.test, "My Profile"));
            data.Add(new NavDrawerItem(Resource.Drawable.startFlag, "Scoreboard"));
            data.Add(new NavDrawerItem(Resource.Drawable.maps, "Routes"));
            data.Add(new NavDrawerItem(Resource.Drawable.perm_group_social_info, "Social"));
            data.Add(new NavDrawerItem(Resource.Drawable.perm_group_system_tools, "Settings"));
            data.Add(new NavDrawerItem(Resource.Drawable.ic_action_help, "About"));
            data.Add(new NavDrawerItem(Resource.Drawable.logout, "Logout"));

            NavDrawerAdapter customAdapter = new NavDrawerAdapter(this, Resource.Layout.leftDrawerAdapter, data );
            mLeftDrawer.Adapter = customAdapter;

            mLeftDrawer.ItemClick += async (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                var item = customAdapter.GetItem(e.Position);

                if (e.Position == 1)
                {
                    myIntent = new Intent(this, typeof(ScoreBoardPersonActivity));
                    StartActivity(myIntent);
                }
                else if (e.Position == 2)
                {
                    if (App.Current.LocationService.isGpsOn())
                    {
                        myIntent = new Intent(this, typeof(RouteOverview));
                        StartActivity(myIntent);
                    }
                    else
                    {
                        Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                        alert.SetTitle("GPS Off");
                        alert.SetMessage("Please turn on your GPS");
                        alert.SetNeutralButton("Ok", (senderAlert, args) =>
                        {

                        });

                        alert.Show();
                    }

                }
                else if (e.Position == 3)
                {
                    if (App.Current.LocationService.isGpsOn())
                    {
                        myIntent = new Intent(this, typeof(FriendsOverview));
                        StartActivity(myIntent);
                    }
                    else
                    {
                        Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                        alert.SetTitle("GPS Off");
                        alert.SetMessage("Please turn on your GPS");
                        alert.SetNeutralButton("Ok", (senderAlert, args) =>
                        {

                        });

                        alert.Show();
                    }

                }
                else if (e.Position == 4)
                {
                    myIntent = new Intent(this, typeof(Settings));
                    StartActivity(myIntent);
                }
                else if (e.Position == 5)
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("About MoveFit");
                    alert.SetMessage("The purpose of this app is to promote an active and social lifestyle." + System.Environment.NewLine +
                        "I have developed this app as a part of my master thesis." + System.Environment.NewLine + System.Environment.NewLine +
                     "In that regard, I kindly ask if you could help me by answering a survey. It will not take more than one minute of your time, and it will be anonymous :)" + System.Environment.NewLine
                    );
                    alert.SetPositiveButton("Take Survey", (senderAlert, args) => {

                        var uri = Android.Net.Uri.Parse("https://no.surveymonkey.com/r/LK39RKC");
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);

                    });

                    alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                        //perform your own task for this conditional button click


                    });

                    RunOnUiThread(() => {
                        alert.Show();
                    });

                }
                else if (e.Position == 6)
                {
                    var logoutApp = Azure.SetUserOnline(userId, false);

                   // Process.KillProcess(Android.OS.Process.MyPid());
                    Finish();
                    myIntent = new Intent(this, typeof(WelcomeScreen));
                    StartActivity(myIntent);
                    Toast.MakeText(this, "Logged out!", ToastLength.Long).Show();

                }


                else if (e.Position == 0)
                {

                    try
                    {

                        User instance = null;
                        var list = await Azure.getUserByAuthId(auth0UserId);
                        instance = list.FirstOrDefault();

                        if (instance != null)
                        {

                            Bundle b = new Bundle();
                            b.PutStringArray("MyData", new String[] {

                        instance.UserName,
                        instance.Sex,
                        instance.Age.ToString(),
                        instance.ProfilePicture,
                        instance.Points.ToString(),
                        instance.AboutMe,
                        instance.Id


                    });

                            Intent myIntent = new Intent(this, typeof(UserProfile));
                            myIntent.PutExtras(b);
                            StartActivity(myIntent);

                        }

                    }
                    catch (Exception)
                    {


                    }

                }

            };


            var RightAdapter = new RightDrawer(this);
            mRightDrawer.Adapter = RightAdapter;
            mDrawerToggle = new ActionBarDrawerToggle(
                this,                           //Host Activity
                mDrawerLayout,                  //DrawerLayout
                Resource.String.openDrawer,     //Opened Message
                Resource.String.closeDrawer     //Closed Message
            );
       

            mDrawerToggle.DrawerIndicatorEnabled = true;
            mDrawerLayout.AddDrawerListener(mDrawerToggle);
            mDrawerToggle.SyncState();

        
        

            if (mToolbar != null)
            {
              //  SupportActionBar.SetTitle(Resource.String.openDrawer);
                SetSupportActionBar(mToolbar);
            }


            if (savedInstanceState != null)
            {
                if (savedInstanceState.GetString("DrawerState") == "Opened")
                {
                    //SupportActionBar.SetTitle(Resource.String.openDrawer);
                }

                else
                {
                    //SupportActionBar.SetTitle(Resource.String.closeDrawer);
                }
            }

            else
            {

                 SupportActionBar.SetTitle(Resource.String.closeDrawer);

            }

            Toast.MakeText(this, "Welcome "+ array[0]+"!", ToastLength.Long).Show();

            try
            {
                user = await Azure.userRegisteredOnline(userName);
                List<Route> routes = await Azure.nearbyRoutes();


         
                routesNearby = "Routes Nearby: " + routes.Count;

            }
            catch (Exception)
            {
                routesNearby = "Routes Nearby: 0";
            }


            if (user.Count == 0)
                {
                    FragmentTransaction firstWelcome = FragmentManager.BeginTransaction();
                    DialogWelcome welcome= new DialogWelcome();
                    welcome.DialogClosed += OnDialogClosedWelcome;
                    welcome.Show(firstWelcome, "Welcome");

                  
                }
                else
                {
                   
                   waitingUpload = user.FirstOrDefault();
                   userId = user.FirstOrDefault().Id;
                   userInstanceOne = waitingUpload;

                StartService(new Intent(this, typeof(LocationService)));
                initPersonTracker();

                try
                {
                        var setOnline = await Azure.SetUserOnline(MainStart.userId, true);
                        isOnline = true;
                }
                   catch (Exception)
                    {
                       
                    }

                //currentLocation = App.Current.LocationService.getLastKnownLocation();
                //Azure.updateUserLocation(userName, currentLocation.Latitude, currentLocation.Longitude);

                var routeList = await Azure.getMyRoutes(MainStart.userId);
                    routesCreated = "Routes Created: " + routeList.Count;
                
                   points = "Score: " + userInstanceOne.Points;

                }






            if (userInstanceOne != null && userInstanceOne.DistanceMoved != 0)
            {
                string unit = " km";
                double dist = 0;
                var test = IOUtilz.LoadPreferences();

                if(test[3] == 2)
                {
                    menItemOnlineIcion.SetIcon(Resource.Drawable.redoffline);
                    menItemOnlineText.SetTitle("Offline");
                }else if (test[3] == 1 || test[3] == 0)
                {
                    menItemOnlineIcion.SetIcon(Resource.Drawable.greenonline);
                    menItemOnlineText.SetTitle("Online");
                }
                   
                if (test[1] == 1)
                {
                    unit = " miles";
                    dist = (int)IOUtilz.ConvertKilometersToMiles(userInstanceOne.DistanceMoved / 1000);
                }
                else
                {
                    dist = userInstanceOne.DistanceMoved / 1000;
                }

               totalDistance = "Total Distance Moved: "+ dist.ToString() + unit;
            }
            else
            totalDistance = "Total Distance Moved: 0";
            //   initPersonTracker();  

            //StartService(new Intent(this, typeof(LocationService)));
            //initPersonTracker();
           var fade = AnimationUtils.LoadAnimation(this, Resource.Animation.abc_fade_out);
            bool ready = true;

            distButton.Click += async (a, e) =>
            {
                if (ready)
                {
                    
                    ready = false;
                    var me = await Azure.getUserByAuthId(userId);

                    messages.Visibility = ViewStates.Visible;
                    messages.Text = "Distance Moved: "+ me.FirstOrDefault().DistanceMoved.ToString();
                    myAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_right_to_left);
                    myAnimation.Duration = 1600;
                    myAnimation.BackgroundColor = Color.LawnGreen;
                    messages.StartAnimation(myAnimation);
                    await Task.Delay(4000);
                    messages.StartAnimation(fade);
                    messages.Visibility = ViewStates.Invisible;
                    ready = true;
                }
                          

            };
            routeButton.Click += async(a, e) =>
            {
                if (ready) {

                    ready = false;
                 

                    messages.Visibility = ViewStates.Visible;
                messages.Text = routesCreated + " - " + routesNearby;
                myAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.abc_fade_in);
                myAnimation.Duration = 1600;
                myAnimation.BackgroundColor = Color.MediumVioletRed;         
                messages.StartAnimation(myAnimation);
                await Task.Delay(4000);
                messages.StartAnimation(fade);
                messages.Visibility = ViewStates.Invisible;
                    ready = true;
                }
            };
            scoreButton.Click +=  async (a, e) =>
            {
                if (ready)
                {
                  
                    ready = false;
                var me = await Azure.getUserByAuthId(userId);
                messages.Visibility = ViewStates.Visible;
                messages.Text = "Score: " + me.FirstOrDefault().Points;
                myAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_left_to_right);
              
                myAnimation.Duration = 1600;
                myAnimation.BackgroundColor = Color.Azure;
                
                messages.StartAnimation(myAnimation);
                await Task.Delay(4000);
                messages.StartAnimation(fade);
               
                messages.Visibility = ViewStates.Invisible;
                ready = true;

                }

            };



            top = await Azure.getUsersFriends(MainStart.userId);
            //   topUsers = top.FindAll(User => User.Id != null).OrderBy(User => User.Points).Take(3).Distinct().ToList<User>();
            topUsers  = (from friend in top
                         orderby friend.Points descending                 
                select friend ).Take(3).ToList<User>();

            try
            {


                if (topUsers.Count != 0)
                {
                    //borderLayout.Visibility = ViewStates.Visible;
                    titleTopFriends.Text = "Top active friends";
                    titleTopFriends.TextSize = 19;

                    if (topUsers[0].UserName != "")
                    {
                        pers1.Text = "#1 " + topUsers[0].UserName;
                        pictureFriend1.SetImageBitmap(IOUtilz.DownloadImageUrl(topUsers[0].ProfilePicture));

                    }


                    if (topUsers[1].UserName != "")
                    {
                        pers2.Text = "#2 " + topUsers[1].UserName;
                        pictureFriend2.SetImageBitmap(IOUtilz.DownloadImageUrl(topUsers[1].ProfilePicture));

                    }


                    if (topUsers[2].UserName != "")
                    {
                        pers3.Text = "#3 " + topUsers[2].UserName;  //"Test friend3 Score: 0";                                                                           //    pictureFriend3.SetImageBitmap(IOUtilz.GetImageBitmapFromUrl(topUsers[2].ProfilePicture));
                        pictureFriend3.SetImageBitmap(IOUtilz.DownloadImageUrl(topUsers[2].ProfilePicture));

                    }



                }


            }
            catch (Exception)
            {


            }


            moa = await Azure.getUserByAuthId(userId);


            mainActivity.RunOnUiThread(async () =>
            {
                listOfUserFriends = await Azure.getUsersFriends(MainStart.userId);
                adpter = new UserMessageFriendsAdapter(listOfUserFriends, mRecyclerView, this, this, adpter, moa);
                mRecyclerView.SetAdapter(adpter);

            });

            StartService(new Intent(this, typeof(SimpleService)));
           
        }


        protected  override void OnStop()
        {
            base.OnStop();
          
            InvalidateOptionsMenu();

        }
        protected override void OnPause()
        {
           
            base.OnPause();
            InvalidateOptionsMenu();
        }

        protected override void OnResume()
        {
         
            base.OnPause();
        }


        protected  override void OnStart()
        {
            base.OnStart();
         

        }
      
        protected async override void OnDestroy()
        {

            base.OnDestroy();
            Azure.SetUserOnline(userId, false);
            isOnline = false;
            StopService(new Intent(this, typeof(LocationService)));
         

        }

        public async Task<List<User>> logOff()
        {
            isOnline = false;
            StopService(new Intent(this, typeof(LocationService)));
            Azure.SetUserOnline(userId, false);
                     
            return user;
        }

        public bool Changed
        {
            get { return changed; }
            set
            {
                changed = value;

                if (changed && !chk)
                {

                    updateLocationTimer();
                }
                
            }
        }

        public void updateLocationTimer()
        {
            var timer = new Timer((e) =>
            {
                //  var timerTest = Azure.updateUserLocation(userName);
              var timerTest =  Azure.updateUserLocation(userName, currentLocation.Latitude, currentLocation.Longitude);

            }, null, 0, Convert.ToInt32(TimeSpan.FromMinutes(4).TotalMilliseconds));

        }


        public  void initPersonTracker()
        {

            //PersonTracker service
            App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
                Log.Debug(logTag, "ServiceConnected Event Raised");
                // notifies us of location changes from the system
                App.Current.LocationService.LocationChanged += HandleLocationChanged;
                //notifies us of user changes to the location provider (ie the user disables or enables GPS)
                App.Current.LocationService.ProviderDisabled += HandleProviderDisabled;
                App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
                // notifies us of the changing status of a provider (ie GPS no longer available)
                App.Current.LocationService.StatusChanged += HandleStatusChanged;
            };

            //Starts the person activity tracker
          //  StartService(new Intent(this, typeof(LocationService)));
        }

        public bool TestIfGooglePlayServicesIsInstalled()
        {
            int InstallGooglePlayServicesId = 1000;
            int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                string errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("", "There is a problem with Google Play Services on this device: {0} - {1}", queryResult, errorString);
                Dialog errorDialog = GoogleApiAvailability.Instance.GetErrorDialog(this, queryResult, InstallGooglePlayServicesId);
                ErrorDialogFragment dialogFrag = new ErrorDialogFragment();


                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Google play service missing");
                alert.SetMessage("Google play service missing!");
                alert.SetNeutralButton("Ok", (senderAlert, args) => {

                });

                dialogFrag.Show(FragmentManager, "GooglePlayServicesDialog");
            }
            return false;
        }
        public  override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {

                case Android.Resource.Id.Home:
                    mDrawerLayout.CloseDrawer(mRightDrawer);
                    mDrawerToggle.OnOptionsItemSelected(item);

                    InvalidateOptionsMenu();
                    return true;

                //case Resource.Id.messages:
                    
                //    RunOnUiThread(async () =>
                //    {
                //        List<User> friendList = null;
                //        friendList = await Azure.getUsersFriends(userId);
                       
                //        if (friendList.Count == 0)
                //        {
                //            Toast.MakeText(this, "No messages available!", ToastLength.Long).Show();

                //        } else 
                //        {
                //            var inten = new Intent(this, typeof(UsersFriends));
                //            StartActivity(inten);
                //            //InvalidateOptionsMenu();      
                //        }                          

                //    });


                //    return true;

                //case Resource.Id.action_help:
                    //Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    //alert.SetTitle("About MoveFit");
                    //alert.SetMessage("The purpose of this app is to promote an active and social lifestyle." + System.Environment.NewLine +
                    //    "I have developed this app as a part of my master thesis." + System.Environment.NewLine + System.Environment.NewLine +
                    // "In that regard, I kindly ask if you could help me by answering a survey. It will not take more than one minute of your time, and it will be anonymous :)" + System.Environment.NewLine                   
                    //); 
                    //alert.SetPositiveButton("Take Survey",  (senderAlert, args) => {

                    //    var uri = Android.Net.Uri.Parse("https://no.surveymonkey.com/r/LK39RKC");
                    //    var intent = new Intent(Intent.ActionView, uri);
                    //    StartActivity(intent);

                    //});

                    //alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                    //    //perform your own task for this conditional button click
                        

                    //});

                    //RunOnUiThread(() => {
                    //    alert.Show();
                    //});

               
                   
                 //   return true;

                case Resource.Id.right:
                    if (mDrawerLayout.IsDrawerOpen(mRightDrawer))
                    {
                        //Right Drawer is already open, close it
                        mDrawerLayout.CloseDrawer(mRightDrawer);

                        //adde
                        InvalidateOptionsMenu();
                    }
                    else
                    {
                        mDrawerLayout.CloseDrawer(mLeftDrawer);
                        //Right Drawer is closed, open it and just in case close left drawer
                        mDrawerLayout.OpenDrawer(mRightDrawer);
                        //  mDrawerLayout.CloseDrawer(mLeftDrawer);
                        RunOnUiThread(async () =>
                        {
                            listOfUserFriends = await Azure.getUsersFriends(MainStart.userId);
                            adpter = new UserMessageFriendsAdapter(listOfUserFriends, mRecyclerView, this, this, adpter, moa);
                            mRecyclerView.SetAdapter(adpter);
                        });
                           
                       
                        //adde
                        InvalidateOptionsMenu();
                    }
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {


            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
          
            var te = IOUtilz.LoadPreferences();
            if(te[3] == 0)
            {
       
                menItemOnlineIcion = menu.FindItem(Resource.Id.statusOnline).SetIcon(Resource.Drawable.redoffline);

                menItemOnlineText = menu.FindItem(Resource.Id.statusOnlineText).SetTitle("Offline");
               

            }
            else
            {
                menItemOnlineIcion = menu.FindItem(Resource.Id.statusOnline).SetIcon(Resource.Drawable.greenonline);

                //menItemOnlineIcion = menu.FindItem(Resource.Id.statusOnline);
                menItemOnlineText = menu.FindItem(Resource.Id.statusOnlineText).SetTitle("Online");

            }

            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {

            try
            {

           
            if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }

            else
            {
                outState.PutString("DrawerState", "Closed");
            }
            }
            catch (Exception)
            {

               
            }
            base.OnSaveInstanceState(outState);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            try
            {
                mDrawerToggle.SyncState();
            }
            catch (Exception)
            {


            }

        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            try
            {
                mDrawerToggle.OnConfigurationChanged(newConfig);
            }
            catch (Exception)
            {

            }

        }

         void OnDialogClosedWelcome(object sender, DialogWelcome.DialogEventArgs e)
        {
            dialogOpen = true;
            var list = e.ReturnValue;
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            DialogUserInfo newDialog = new DialogUserInfo();
            newDialog.DialogClosed += OnDialogClosed;
            newDialog.Show(transaction, "User Info");
        }

            async void OnDialogClosed(object sender, DialogUserInfo.DialogEventArgs e)
        {

            String[] returnData;
            returnData = e.ReturnValue.Split(',');
            string gender = returnData[0];
            string activityLevel = returnData[1];
            string ageString = returnData[2];
            int age = Convert.ToInt32(ageString);

            try
            {
                waitingUpload = await Azure.AddUser(auth0UserId,"", userName, gender, age, 0, profilePictureUrl, "0", "0", true, activityLevel, 0);
                userInstanceOne = waitingUpload;
                points = "Score: 0";
                routesCreated= "Routes Created: 0";
                userId = auth0UserId;
               
            }
            catch (Exception)
            {
            }

            try
            {
                var setOnline = await Azure.SetUserOnline(userId, true);
                isOnline = true;

               
                //currentLocation = App.Current.LocationService.getLastKnownLocation();
                //var timerTest = Azure.updateUserLocation(userName, currentLocation.Latitude, currentLocation.Longitude);


            }
            catch (Exception)
            {

               
            }
            StartService(new Intent(this, typeof(LocationService)));
            initPersonTracker();

            IOUtilz.SavePreferences(0, 100, 45, 1);
            dialogOpen = false;

        }

        /// Updates UI with location data
        public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
           
            currentLocation = e.Location;  
            OnLocationChanged(e.Location);

            Changed = true;
            changed = true;
            if (changed && !chk)
            {
                   chk = true;
            }
        }


        public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
        {

        }

        public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
        {

        }

        public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
        {

        }




       


        public static void setMarker(LatLng myPosition, GoogleMap mMap)
        {

            MarkerOptions markerOpt1;

            try
            {

                mMap.Clear();
                markerOpt1 = new MarkerOptions();
                markerOpt1.SetPosition(myPosition);
                markerOpt1.SetTitle("My Position");
                markerOpt1.SetSnippet("My Position");
                BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(MainStart.profilePic); //(Resource.Drawable.test);
                markerOpt1.SetIcon(image);
                mMap.AddMarker(markerOpt1);
                
            }
            catch (Exception)
            {


            }
        }
      
   

        public void OnLocationChanged(Location location)
        {
            currentLocation = location;

            var currentPos = new LatLng(currentLocation.Latitude, currentLocation.Longitude);

            if (!firstLocationUpdate)
            {
                Azure.updateUserLocation(userName, currentLocation.Latitude, currentLocation.Longitude);
                firstLocationUpdate = true;
            }

            //setMarker(currentPos, mMap);
           
            //mMap.MoveCamera(CameraUpdateFactory.ZoomIn());
            //// mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 14));
            //mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(currentPos, 14));


        }

        public void OnProviderDisabled(string provider)
        {

        }
        public void OnProviderEnabled(string provider)
        {

        }
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {

        }

    
        public class  RightDrawer : BaseAdapter  //, IOnMapReadyCallback, IOnMapClickListener,IOnMarkerClickListener
        {
            List<Contact> _contactList;
            public Activity activityRightDrawer;
            public static List<User> nearbyUsers;
            public Address oldAddress;
        


            public  RightDrawer(Activity activity) 
            {
                activityRightDrawer = activity;
                FillContacts();

                
            }



            //public async Task<string> lookupAddress()
            //{

            //    string addressValue = "";
            //    if (currentLocation == null)
            //    {
            //        _address.Text = "Unable to determine your location. Try again in a short while.";
            //        addressValue = _address.Text;
            //    }
            //    else
            //    {
            //       try
            //        {


            //            Address address = await ReverseGeocodeCurrentLocation();
            //            if (oldAddress != address)
            //            {
            //                oldAddress = address;
            //                addressValue = DisplayAddress(address);
            //            }
            //        }
            //        catch (Exception)
            //        {


            //        }
            //    }

            //    return addressValue;
            //}

            //async Task<Address> ReverseGeocodeCurrentLocation()
            //{
              
            //    IList<Address> addressList =
            //    await geocoder.GetFromLocationAsync(currentLocation.Latitude, currentLocation.Longitude, 10);
            //    Address address = addressList.FirstOrDefault();
            //    return address;
            //}

            //static string DisplayAddress(Address address)
            //{
            //    if (address != null)
            //    {
            //        StringBuilder deviceAddress = new StringBuilder();
            //        for (int i = 0; i < address.MaxAddressLineIndex; i++)
            //        {
            //            deviceAddress.AppendLine(address.GetAddressLine(i));
            //        }
            //        // Remove the last comma from the end of the address.
            //        if (_address.Text == deviceAddress.ToString())
            //        {

            //        }
            //        else
            //            _address.Text = deviceAddress.ToString();
            //    }

            //    else
            //    {
            //        _address.Text = "Unable to determine the address";
            //    }

            //    return _address.Text;
            //}

            void FillContacts()
            {
                _contactList = new List<Contact>();
                _contactList.Add(new Contact
                {
                    Id = 123,
                    DisplayName = ""
                });
            }

            //public void OnMapReady(GoogleMap googleMap)
            //{
            //    mMap = googleMap;
            //}

            class Contact
            {
                public long Id { get; set; }
                public string DisplayName { get; set; }

            }

            public override int Count
            {
                get { return _contactList.Count; }
            }

            public override Java.Lang.Object GetItem(int position)
            {

                return null;
            }

            public override long GetItemId(int position)
            {
                return _contactList[position].Id;
            }

            public override  View GetView(int position, View convertView, ViewGroup parent)
            {
               
                var view = convertView ?? activityRightDrawer.LayoutInflater.Inflate(Resource.Layout.rightDrawerMenu, parent, false);
                var contactName = view.FindViewById<TextView>(Resource.Id.ContactName);
                contactName.Text = "";
               
                //MapFragment mapFrag = (MapFragment)activityRightDrawer.FragmentManager.FindFragmentById(Resource.Id.map);
                //mMap = mapFrag.Map;

                //if (mMap != null)
                //{
                //    mMap.MapType = GoogleMap.MapTypeTerrain;  
                //}

                //try
                //{

                //    mMap.SetOnMapClickListener(this);
                //    mMap.SetOnMarkerClickListener(this);
                //    mMap.UiSettings.ZoomControlsEnabled = true;
                //    mMap.UiSettings.RotateGesturesEnabled = false;
                //    mMap.UiSettings.ScrollGesturesEnabled = false;


                //}
                //catch (Exception)
                //{


                //}

  
                    titleTopFriends = view.FindViewById<TextView>(Resource.Id.pers11);
                    pictureFriend1 = view.FindViewById<ImageView>(Resource.Id.pic1);
                    pictureFriend2 = view.FindViewById<ImageView>(Resource.Id.pic2);
                    pictureFriend3 = view.FindViewById<ImageView>(Resource.Id.pic3);
                    pers1 = view.FindViewById<TextView>(Resource.Id.pers1);
                    pers2 = view.FindViewById<TextView>(Resource.Id.pers2);
                    pers3 = view.FindViewById<TextView>(Resource.Id.pers3);

                
                //    TextView bearText = view.FindViewById<TextView>(Resource.Id.bear);
                   
                //    _address = view.FindViewById<TextView>(Resource.Id.location_text);



             

                mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.messagesRec);
                mLayoutManager = new LinearLayoutManager(activityRightDrawer);
                mRecyclerView.SetLayoutManager(mLayoutManager);

               
              

                //    ImageButton alarm = view.FindViewById<ImageButton>(Resource.Id.alarmButton);
                //    Switch location = view.FindViewById<Switch>(Resource.Id.switch1);

                //    alarm.Click += (a, e) =>
                //    {

                //        if (SimpleService.isRunning == false)
                //        {
                //            activityRightDrawer.StartService(new Intent(mainActivity, typeof(SimpleService)));
                //            Toast.MakeText(mainActivity, "Activity alarm activated", ToastLength.Short).Show();
                //        }
                //        else if (SimpleService.isRunning == true)
                //        {
                //            activityRightDrawer.StopService(new Intent(mainActivity, typeof(SimpleService)));
                //            Toast.MakeText(mainActivity, "Activity alarm off", ToastLength.Short).Show();

                //        }

                //    };


                //    location.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e)
                //    {
                //        if (e.IsChecked == true)
                //        {
                //            Toast.MakeText(activityRightDrawer, "Your location tracking has been turned on, you are now visible!", ToastLength.Long).Show();

                //            activityRightDrawer.StartService(new Intent(activityRightDrawer, typeof(LocationService)));
                //            try
                //            {
                //                var a = Azure.SetUserOnline(userId, true);
                //                isOnline = true;

                //            }
                //            catch (Exception)
                //            {


                //            }

                //            menItemOnlineIcion.SetIcon(Resource.Drawable.greenonline);
                //            menItemOnlineText.SetTitle("Online");

                //        }
                //        else
                //        {
                //            Toast.MakeText(activityRightDrawer, "Tracking stopped, you are now invisible!", ToastLength.Long).Show();
                //            activityRightDrawer.StopService(new Intent(activityRightDrawer, typeof(LocationService)));


                //            try
                //            {
                //                var b = Azure.SetUserOnline(userId, false);
                //                isOnline = false;
                //            }
                //            catch (Exception)
                //            {


                //            }

                //            menItemOnlineIcion.SetIcon(Resource.Drawable.redoffline);
                //            menItemOnlineText.SetTitle("Offline");


                //        }
                //    };






                return view;
            }

       

            //public bool OnMarkerClick(Marker marker)
            //{
               
            //    marker.Title = "My Location";
               
            //    try
            //    {
            //        mainActivity.RunOnUiThread( async () => {
            //            marker.Snippet = await lookupAddress();
            //            marker.ShowInfoWindow();
            //        });
                  
            //    }
            //    catch (Exception)
            //    {

                    
            //    }
              

            //    return true;

            //}
            //public void OnMapClick(LatLng point)
            //{
               

            //}
            //private async void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
            //{
            //    //markerClickEventArgs.Handled = true;
            //    //Marker marker = markerClickEventArgs.Marker;
            //    //marker.Snippet = "Finding Address..";
            //    //marker.Snippet = await lookupAddress();
            //}
          
       // }
        }

        public  override void OnBackPressed()
        {
            if (dialogOpen){
                return;
            }
            else { 
            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Exit app");
            alert.SetMessage("Do you want to logout of the application?");
            alert.SetPositiveButton("Yes", async (senderAlert, args) => {
              
            base.OnBackPressed();
               
                var wait = await logOff();
                try
                {
                    //WelcomeScreen.instance.FinishAffinity();
                    //Takes you to the home screen
                    //System.Environment.Exit(0);
                    //WelcomeScreen.instance.Finish();
                    // Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    Finish();
                    myIntent = new Intent(this, typeof(WelcomeScreen));
                    StartActivity(myIntent);

                }
                catch (Exception)
                {

                  //  throw a;
                }


           
               // base.OnBackPressed();
            });

            alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                //perform your own task for this conditional button click

            });
            //run the alert in UI thread to display in the screen
            RunOnUiThread(() => {
                alert.Show();
            });


            }

        }

    


    }


}
