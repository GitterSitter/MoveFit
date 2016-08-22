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
using Android.Text.Util;
using Android.Util;
using Android.Views;
using Android.Widget;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.WindowsAzure.MobileServices.Query;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Android.Gms.Maps.GoogleMap;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;




using Gcm.Client;
using TestApp.Push;
using System.Net;
using System.Text;
using System.IO;

namespace TestApp
{


    [Activity(Label = "MainMenu", Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainStart : AppCompatActivity, ILocationListener
    {
        public static MainStart instance;

        public readonly string logTag = "MainActivity";
        SupportToolbar mToolbar;
        ActionBarDrawerToggle mDrawerToggle;
        DrawerLayout mDrawerLayout;
        ListView mLeftDrawer;
        ListView mRightDrawer;
        ArrayAdapter mLeftAdapter;
        ArrayAdapter mRightAdapter;
        List<string> mLeftDataSet;
        List<string> mRightDataSet;
        public string[] table;
        public string text;
        public static string facebookUserId;
        public static string userId;
        public ImageView profilePicture;
        public static Bitmap profilePic;
        public string profilePictureUrl;

        TextView latText;
        TextView longText;
        TextView altText;
        TextView speedText;
        TextView bearText;
        TextView accText;

        Intent myIntent;

        public Azure azure;
        public static bool changed;
        public static Location loc;
        public static Location currentLocation;
        public static string userName;
        public static String[] array;
        public List<User> user;
        public static bool chk;
        public ProgressBar loadingImage;
        public static TextView _address;
        public Address oldAddress;
        public static GoogleMap mMap;
        TextView points;
        public static Activity mainActivity;
        public static User waitingUpload;
        public static User userInstanceOne;


        public static bool isOnline;
        public static ConnectivityManager connectivityManager;
        public static IMenuItem menItemOnlineIcion;
        public static IMenuItem menItemOnlineText;

        TextView messages;

        public static string myUserId;
        public static string myUserName;
        public static string userList;
        public static List<MessageDetail> currentMessagesWritten;
        public static List<UserDetail> listOfConnectedUsers;

        public List<NavDrawerItem> data;
        public Android.Views.ViewGroup.LayoutParams param;

        private RegisterClient registerClient;
        private static readonly String BACKEND_ENDPOINT = "http://appbackend201615.azurewebsites.net";

        private void RegisterWithGCM()
        {
            // Check to ensure everything's set up right
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);

            // Register for push notifications
            Log.Info("MainActivity", "Registering...");
             GcmClient.Register(this, Constants.SenderID);
        }


      
        public class NavDrawerAdapter :  BaseAdapter //ArrayAdapter<NavDrawerItem>
{
        
            private readonly Context context;
            private readonly int layoutResourceId;
            private List<NavDrawerItem> data;
            public NavDrawerAdapter(Context context, int layoutResourceId, List<NavDrawerItem> data) { 
               
                    this.context = context;
                    this.layoutResourceId = layoutResourceId;
                    this.data = data;
                   // fillAdapter();
            }
            //public void fillAdapter()
            //{
               
            //    data = new List<NavDrawerItem>();
            //    data.Add(new NavDrawerItem(Resource.Drawable.offline, "My Profile"));
            //    data.Add(new NavDrawerItem(Resource.Drawable.ic_action_help, "Scoreboard"));
            //    data.Add(new NavDrawerItem(Resource.Drawable.ic_action_help, "Routes"));
            //    data.Add(new NavDrawerItem(Resource.Drawable.ic_action_help, "Social"));
            //    data.Add(new NavDrawerItem(Resource.Drawable.ic_action_help, "Calculator"));
            //}
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
                //LayoutInflater inflater = ((Activity)context).LayoutInflater;

                //View v = inflater.Inflate(layoutResourceId, parent, false);
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
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.drawerLayout);
            TestIfGooglePlayServicesIsInstalled();
             mainActivity = this;

            instance = this;



             RegisterWithGCM();

            //ISharedPreferences preferences = Prefer.getDefaultSharedPreferences(getApplicationContext());
            //SharedPreferences.Editor editor = preferences.edit();
            //editor.putstring("PreferenceName", "YOUR PREFERENCE VALUE");
            //editor.commit;

          

        

            changed = false;
            user = null;
            chk = false;

            waitingUpload = null;
            userInstanceOne = null;

            isOnline = false;
           
            StartService(new Intent(this, typeof(SimpleService)));
            // connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);

            mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

            mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
         // mRightDrawer = FindViewById<ListView>(Resource.Id.right_drawer);

            mRightDrawer = FindViewById<ListView>(Resource.Id.ContactsListView);
            TextView routesNearby = FindViewById<TextView>(Resource.Id.routesNearby);
            TextView routesCreated = FindViewById<TextView>(Resource.Id.textView1);
            TextView friends = FindViewById<TextView>(Resource.Id.friends);
            TextView totalDistance = FindViewById<TextView>(Resource.Id.distance);

            TextView appTitle = FindViewById<TextView>(Resource.Id.titleApp);
            Typeface tf = Typeface.CreateFromAsset(Assets,
              "english111.ttf");

            appTitle.TextSize = 32;
            appTitle.Typeface = tf;



            points = FindViewById<TextView>(Resource.Id.points);

            routesCreated.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            routesCreated.TextSize = 15;

            friends.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            friends.TextSize = 15;

            totalDistance.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            totalDistance.TextSize = 15;

            points.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            points.TextSize = 15;

            routesNearby.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            routesNearby.TextSize = 15;
            

            totalDistance.Text = "Total Distance Moved: ";
            points.Text = "Score: ";
            friends.Text = "Friends Online: ";
            routesNearby.Text = "Routes nearby: ";

            ImageView pictureFriend1 = FindViewById<ImageView>(Resource.Id.pic1);
            ImageView pictureFriend2 = FindViewById<ImageView>(Resource.Id.pic2);
            ImageView pictureFriend3 = FindViewById<ImageView>(Resource.Id.pic3);

            TextView pers1 = FindViewById<TextView>(Resource.Id.pers1);
            TextView pers2 = FindViewById<TextView>(Resource.Id.pers2);
            TextView pers3 = FindViewById<TextView>(Resource.Id.pers3);



            messages = FindViewById<TextView>(Resource.Id.messageInfo);
            messages.SetTypeface(Typeface.SansSerif, TypefaceStyle.Normal);
            messages.TextSize = 15;

            var layout = FindViewById<RelativeLayout>(Resource.Id.messages);
            layout.Visibility = ViewStates.Invisible;


            array = Intent.GetStringArrayExtra("MyData");
            routesCreated.Text = "Routes Created: ";// "Greetings " + array[0] + "!";
            userName = array[0];
            profilePictureUrl = array[1];
            profilePic = IOUtilz.GetImageBitmapFromUrl(array[1]);

            //profilePicture = FindViewById<ImageView>(Resource.Id.profilePicture);
            //profilePicture.SetImageBitmap(profilePic);

            facebookUserId = array[2];
          

            //Fix to remove space from profile pic and loadingbar
            //  ((ViewGroup)loadingImage.Parent).RemoveView(loadingImage);


            mLeftDrawer.Tag = 0;
            //mRightDrawer.Tag = 1;
            mRightDrawer.Tag = 1;
         
            SetSupportActionBar(mToolbar);

            mLeftDataSet = new List<string>();
            //mLeftDataSet.Add("bump");
            //mLeftDataSet.Add("bump");
            //mLeftDataSet.Add("bump");
            //mLeftDataSet.Add("bump");
            //mLeftDataSet.Add("bump");

            //if (IOUtilz.IsKitKatWithStepCounter(PackageManager))
            //{
            //    // mLeftDataSet.Add("STEP COUNTER 2");
            //}

            mLeftDataSet.Add("Scoreboard");
            mLeftDataSet.Add("Routes");
            mLeftDataSet.Add("Social");
            mLeftDataSet.Add("Settings");
           // mLeftDataSet.Add("Messages");
            mLeftDataSet.Add("My Profile");

            //mLeftDataSet.Add("Scoreboard");
            //mLeftDataSet.Add("Routes");
            //mLeftDataSet.Add("Social");
            //mLeftDataSet.Add("Messages");
            //mLeftDataSet.Add("Step counter");
            //mLeftDataSet.Add("Calculator");
            //mLeftDataSet.Add("My profile");
            data = new List<NavDrawerItem>();

            data.Add(new NavDrawerItem(Resource.Drawable.test, "My Profile"));
            data.Add(new NavDrawerItem(Resource.Drawable.startFlag, "Scoreboard"));
            data.Add(new NavDrawerItem(Resource.Drawable.maps, "Routes"));
            data.Add(new NavDrawerItem(Resource.Drawable.perm_group_social_info, "Social"));
            data.Add(new NavDrawerItem(Resource.Drawable.perm_group_system_tools, "Settings"));

            //mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
            //mLeftDrawer.Adapter = mLeftAdapter;

            NavDrawerAdapter customAdapter = new NavDrawerAdapter(this, Resource.Layout.leftDrawerAdapter, data );
            mLeftDrawer.Adapter = customAdapter;

         

            mLeftDrawer.ItemClick += async (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                var item = customAdapter.GetItem(e.Position);
             
               
                if (e.Position == 1)
                {
                    myIntent = new Intent(this, typeof( ScoreBoardPersonActivity));
                    StartActivity(myIntent);
                }
                else if (e.Position == 2)
                {
                    myIntent = new Intent(this, typeof( RouteOverview));
                    StartActivity(myIntent);
                }
                else if (e.Position == 3)
                {
                    myIntent = new Intent(this, typeof(FriendsOverview));
                    StartActivity(myIntent);
                }
                else if (e.Position == 4)
                {
                    myIntent = new Intent(this, typeof(Settings));
                    StartActivity(myIntent);
                }


                else if (e.Position == 0)
                {

                    //myIntent = new Intent(this, typeof(ChatRoom));
                    //StartActivity(myIntent);



                    try
                    {



                        User instance = null;

                        if (user.Count != 0 || userInstanceOne != null)
                        {
                            instance = user.FirstOrDefault();

                            if (instance == null)
                                instance = userInstanceOne;

                        }

                        var list = await Azure.getUserInstanceByName(userName);
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




                //mLeftDrawer.ItemClick += async (object sender, AdapterView.ItemClickEventArgs e) => {
                //    var item = mLeftAdapter.GetItem(e.Position);
                //    if (e.Position == 0)
                //    {
                //        myIntent = new Intent(this, typeof(ScoreBoardPersonActivity));
                //        StartActivity(myIntent);
                //    }
                //    if (e.Position == 1)
                //    {
                //        myIntent = new Intent(this, typeof(RouteOverview));
                //        StartActivity(myIntent);
                //    }
                //    else if (e.Position == 2)
                //    {
                //        myIntent = new Intent(this, typeof(FriendsOverview));
                //        StartActivity(myIntent);
                //    }
                //    else if (e.Position == 3)
                //    {
                //        myIntent = new Intent(this, typeof(Calculator));
                //        StartActivity(myIntent);
                //    }


                //    else if (e.Position == 4)
                //    {

                //        //myIntent = new Intent(this, typeof(ChatRoom));
                //        //StartActivity(myIntent);



                //        try
                //        {



                //            User instance = null;

                //            if (user.Count != 0 || userInstanceOne != null)
                //            {
                //                instance = user.FirstOrDefault();

                //                if (instance == null)
                //                    instance = userInstanceOne;

                //            }

                //            var list = await Azure.getUserInstanceByName(userName);
                //            instance = list.FirstOrDefault();


                //            if (instance != null)
                //            {

                //                Bundle b = new Bundle();
                //                b.PutStringArray("MyData", new String[] {

                //            instance.UserName,
                //            instance.Sex,
                //            instance.Age.ToString(),
                //            instance.ProfilePicture,
                //            instance.Points.ToString(),
                //            instance.AboutMe,
                //            instance.Id


                //        });

                //                Intent myIntent = new Intent(this, typeof(UserProfile));
                //                myIntent.PutExtras(b);
                //                StartActivity(myIntent);

                //            }

                //        }
                //        catch (Exception)
                //        {


                //        }

                //    }



                //    };

                var RightAdapter = new ContactsAdapter(this);
            //   var contactsListView = FindViewById<ListView>(Resource.Id.ContactsListView);
            mRightDrawer.Adapter = RightAdapter;



            mDrawerToggle = new ActionBarDrawerToggle(
                this,                           //Host Activity
                mDrawerLayout,                  //DrawerLayout
                Resource.String.openDrawer,     //Opened Message
                Resource.String.closeDrawer     //Closed Message
            );

            //added
            mDrawerToggle.DrawerIndicatorEnabled = true;


            mDrawerLayout.SetDrawerListener(mDrawerToggle);


            //param = mDrawerLayout.LayoutParameters;
            //mDrawerLayout.DrawerSlide += MDrawerLayout_DrawerSlide;
            


            //addedds
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            

            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
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
                //This is the first the time the activity is ran

                SupportActionBar.SetTitle(Resource.String.closeDrawer);

                //int width = Resources.DisplayMetrics.WidthPixels / 10;
                //var paramters = mRightDrawer.LayoutParameters;

                //paramters.Width = width;
                //mRightDrawer.LayoutParameters = paramters;

                //mDrawerLayout.OpenDrawer(mRightDrawer);

            }

            Toast.MakeText(this, "Welcome "+ array[0]+"!", ToastLength.Long).Show();

            try
            {
                user = await Azure.userRegisteredOnline(userName);
                
                if (user.Count == 0)
                {

                    FragmentTransaction firstWelcome = FragmentManager.BeginTransaction();
                    DialogWelcome welcome= new DialogWelcome();
                    welcome.DialogClosed += OnDialogClosedWelcome;
                    welcome.Show(firstWelcome, "Welcome");




                    //FragmentTransaction transaction = FragmentManager.BeginTransaction();
                    //DialogUserInfo newDialog = new DialogUserInfo();
                    //newDialog.DialogClosed += OnDialogClosed;
                    //newDialog.Show(transaction, "User Info");
                    //waitingUpload = await Azure.AddUser();
                    //Toast.MakeText(this, "User Added!", ToastLength.Short).Show();

                    points.Text = "Score: 0";
                    friends.Text = "Friends Online: 0";
                    routesNearby.Text = "Routes Nearby: 0";
                    routesCreated.Text = "Routes Created 0";
                }
                else
                {
                   
                   waitingUpload = user.FirstOrDefault();
                   userId = user.FirstOrDefault().Id;
                   setPoints();
                   
                    userInstanceOne = waitingUpload;
                    var setOnline = await Azure.SetUserOnline(MainStart.userId, true);
                    isOnline = true;

                    List<User> userList = await Azure.getUsersFriends(MainStart.userId);

                    var friendCountOnline = 0;
                    foreach (var item in userList)
                    {
                        if (item.Online == true)
                        {
                            friendCountOnline++;
                        }
                    }
                    friends.Text = "Friends Online: " + friendCountOnline;

                    List<Route> routes = await Azure.nearbyRoutes();

                    routesNearby.Text = "Routes Nearby: " + routes.Count;

                    var routeList = await Azure.getMyRoutes(MainStart.userId);

                    routesCreated.Text = "Routes Created: " + routeList.Count;


                }










            }
            catch (Exception)
            {

            }



            // steps.Text = "Steps: 0";

            if (userInstanceOne != null && userInstanceOne.DistanceMoved != 0)
            {
                string unit = " km";
                double dist = 0;
                var test = IOUtilz.LoadPreferences();
                if (test[1] == 1)
                {
                    unit = " miles";
                    dist = (int)IOUtilz.ConvertKilometersToMiles(userInstanceOne.DistanceMoved / 1000);
                }
                else
                {
                    dist = userInstanceOne.DistanceMoved / 1000;
                }

               totalDistance.Text = "Total Distance Moved: "+ dist.ToString() + unit;


             

            }
            else
                totalDistance.Text = "Total Distance Moved: 0";

            //messagePushNotification();
            //connectToChat();

     
            //profilePicture = FindViewById<ImageView>(Resource.Id.profilePicture);

           
            initPersonTracker();
            
            try
            {

                //  List<User> topUsers = await Azure.getTop3People();
                List<User> top = await Azure.getUsersFriends(MainStart.userId);
                List<User> topUsers = top.FindAll(User => User.Id != null).OrderBy(User => User.Points).Take(3).ToList<User>();

                if (topUsers[0].UserName != "")
                {
                    pers1.Text = "#1 "+topUsers[0].UserName; 
                    pictureFriend1.SetImageBitmap(IOUtilz.GetImageBitmapFromUrl(topUsers[0].ProfilePicture));
                }
                


                if (topUsers[1].UserName != "")
                {
                    pers2.Text = "#2 " +topUsers[1].UserName; 
                    pictureFriend2.SetImageBitmap(IOUtilz.GetImageBitmapFromUrl(topUsers[1].ProfilePicture));
                }


                if (topUsers[2].UserName != "")
                {
                    pers3.Text = "#3 "+topUsers[2].UserName;  //"Test friend3 Score: 0";
                    pictureFriend3.SetImageBitmap(IOUtilz.GetImageBitmapFromUrl(topUsers[2].ProfilePicture));
                }


            }
            catch (Exception)
            {



            }

            //var lis = IOUtilz.LoadPreferences();
            //try
            //{
              
            //    totalDistance.Text = lis[0].ToString() + " Dist";
            //    points.Text = lis[1].ToString() + " Unit";
            //    friends.Text = lis[2].ToString() + " Interval";
            //}
            //catch (Exception)
            //{

              
            //}
                    
          


        }

        //private void MDrawerLayout_DrawerSlide(object sender, DrawerLayout.DrawerSlideEventArgs e)
        //{

          
        //      //  mRightDrawer.LayoutParameters = param;
        //      //mLeftDrawer.LayoutParameters = param;


           
        //}

        public async void connectToChat()
        {
            var hubConnection = new HubConnection("http://chatservices.azurewebsites.net/");
            var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");

            // currentMessagesWritten;
            //listOfconnectedUser;

            chatHubProxy.On<string, string, List<UserDetail>, List<MessageDetail>>("onConnected", (currentUserId, userName, connectedUsers, messageDetails) =>
            {

                //chatHubProxy.On<string, string, List<UserDetail>, List<MessageDetail>>("onConnected", (id, userName, ConnectedUsers, CurrentMessage) =>
                //{
                RunOnUiThread(() =>
                {

                    myUserId = currentUserId;
                    myUserName = userName;
                    listOfConnectedUsers = connectedUsers;
                    

                });

            });

          


            await hubConnection.Start();


            try
            {
             
               
                await chatHubProxy.Invoke("Connect", new object[] { MainStart.userName});
                Toast.MakeText(this, "You are now online!", ToastLength.Short).Show();

            }
            catch (Exception a)
            {
                throw a; 

            }


        }

        protected async override void OnStop()
        {
            base.OnStop();
            // Clean up: shut down the service when the Activity is no longer visible.

            //try
            //{
            //    var wait = await logOff();
            //}
            //catch (Exception)
            //{

              
            //}
           
            //adde
            InvalidateOptionsMenu();

            //Added!
            //  StopService(new Intent(this, typeof(SimpleService)));
            //   StopService(new Intent(this, typeof(LocationService)));
        }
        protected override void OnPause()
        {
           
            base.OnPause();

            //adde
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

            //var a = await Azure.SetUserOnline(userName, false);
          
            base.OnDestroy();

            // var b = await Azure.SetUserOnline(userName, false);
            var wait = await logOff();



        }

        public async Task<List<User>> logOff()
        {
            if (isOnline)
            {
                var user = await Azure.SetUserOnline(userId, false);
                isOnline = false;
            }
           
            StopService(new Intent(this, typeof(LocationService)));
            return user;
        }





        public async void messagePushNotification()
        {

            var hubConnection = new HubConnection("http://chatservices.azurewebsites.net/");
            var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");

            chatHubProxy.On<string, string, string, string>("sendPrivateMessage", (userId, userName, title, message) =>
            {
            // var firstName = userName.Substring(0, userName.IndexOf(" "));

            RunOnUiThread(() =>
            {

                TextView txt = new TextView(this);
                txt.Text = userName + ": " + message;
                txt.SetTextSize(ComplexUnitType.Sp, 20);
                txt.SetPadding(10, 10, 10, 10);


                messages.Text = "New message from:" + System.Environment.NewLine +
                userName;

                messageNotification(message, userName);


            });
            });




            //   **********************************************************



            //var hubConnection = new HubConnection("http://chatservices.azurewebsites.net/");
            //var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");
            //messages.Text = "No new messages";
            ////  int  messageCount = 0;

            //chatHubProxy.On<string, string, string>("sendPrivateMessage2", (userId, userName, message) =>
            //{

            //    //UpdateChatMessage has been called from server


            //    RunOnUiThread(() =>
            //    {

            //        //TextView txt = new TextView(this);
            //        //txt.Text = user + ": " + message;
            //        //txt.SetTextSize(Android.Util.ComplexUnitType.Sp, 20);
            //        //txt.SetPadding(10, 10, 10, 10);

            //        if (userName != MainStart.userName)
            //        {
            //         messages.Text = "New message from:" + System.Environment.NewLine +
            //         user.ToString();

            //            messageNotification(message, userName);

            //        }


            //    });
            //});

            //await hubConnection.Start();

        }

        
        public void messageNotification(string message,string user)
        {

            Notification.Builder builder;
            Notification notification;
            NotificationManager notificationManager;

            Intent newIntent = new Intent(this, typeof(Chat));
            // Pass some information to SecondActivity:
            newIntent.PutExtra(message, user);

            // Create a task stack builder to manage the back stack:
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);

            // Add all parents of SecondActivity to the stack: 
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainStart)));

            // Push the intent that starts SecondActivity onto the stack:
            stackBuilder.AddNextIntent(newIntent);

            const int pendingIntentId = 0;

            PendingIntent pendingIntent =
            PendingIntent.GetActivity(this, pendingIntentId, newIntent, PendingIntentFlags.OneShot);


            builder = new Notification.Builder(this)
           //   .SetContentIntent(pendingIntent)
              .SetContentTitle("MoveFit")
              .SetContentText(user + ": " +message)
              .SetDefaults(NotificationDefaults.Sound)
              .SetContentIntent(pendingIntent)
              .SetAutoCancel(true)
              // .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
              .SetSmallIcon(Resource.Drawable.tt);

            // Build the notification:
            notification = builder.Build();


            // Get the notification manager:
            notificationManager =
           GetSystemService(Context.NotificationService) as NotificationManager;



            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);


        }


        public async void setPoints()
        {
            List<User> userInstance = await Azure.getUserInstanceByName(userName);
            if (userInstance.Count != 0)
            {
                userId = userInstance.First().Id;
                User userPoints = await Azure.getMyPoints(userId);
                points.Text = "Score: " + userPoints.Points.ToString();
            }
        }

        //This is only being executed once
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


            //Toast.MakeText(this, "Your location has been updated!", ToastLength.Short).Show();


            var timer = new Timer((e) =>
            {
                var timerTest = Azure.updateUserLocation(userName);

            }, null, 0, Convert.ToInt32(TimeSpan.FromMinutes(4).TotalMilliseconds));

        }


        public void initPersonTracker()
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
            StartService(new Intent(this, typeof(LocationService)));
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
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {

                case Android.Resource.Id.Home:
                    //The hamburger icon was clicked which means the drawer toggle will handle the event
                    //all we need to do is ensure the right drawer is closed so the don't overlap
                    mDrawerLayout.CloseDrawer(mRightDrawer);
                    mDrawerToggle.OnOptionsItemSelected(item);


                    //adde
                    InvalidateOptionsMenu();
                    return true;

                case Resource.Id.messages:
                    var inten = new Intent(this, typeof(UsersFriends));             
                    StartActivity(inten);
                    //InvalidateOptionsMenu();
                   return true;

              


                case Resource.Id.action_help:
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("About MoveFit");
                    alert.SetMessage("About: This is a prototype app in a masters project. Developed by Trond Tufte" + System.Environment.NewLine +
                     "As this is a part of my thesis, I kindly ask if you could be so kind and help me by answering a survey. It will not taker more than one minutte of your time! :)" + System.Environment.NewLine
                     +
                     System.Environment.NewLine + "Instructions: Open right and left menu by sliding left and right with your finger from the sides towrds the middle" + System.Environment.NewLine
                    );
                    alert.SetPositiveButton("Take Survey",  (senderAlert, args) => {

                        var uri = Android.Net.Uri.Parse("https://www.surveymonkey.com/r/WT798BM");
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);

                    });

                    alert.SetNegativeButton("Cancel", (senderAlert, args) => {
                        //perform your own task for this conditional button click
                        

                    });

                    RunOnUiThread(() => {
                        alert.Show();
                    });

                 

                    if (mDrawerLayout.IsDrawerOpen(mRightDrawer))
                    {
                        //Right Drawer is already open, close it
                        mDrawerLayout.CloseDrawer(mRightDrawer);

                        //adde
                        InvalidateOptionsMenu();
                    }
                    else
                    {
                        //Right Drawer is closed, open it and just in case close left drawer
                        mDrawerLayout.OpenDrawer(mRightDrawer);
                        //  mDrawerLayout.CloseDrawer(mLeftDrawer);

                        //adde
                        InvalidateOptionsMenu();
                    }

                   
                    return true;


                // Starts movement tracker (indoor)
                case Resource.Id.action_alarm:

           

                    if (SimpleService.isRunning == false)
                    {
                        StartService(new Intent(this, typeof(SimpleService)));
                        Toast.MakeText(this, "Activity alarm activated", ToastLength.Short).Show();
                       
                    }
                    else if (SimpleService.isRunning == true)
                    {
                        StopService(new Intent(this, typeof(SimpleService)));
                        Toast.MakeText(this, "Activity alarm off", ToastLength.Short).Show();
                       
                    }

                    //adde
                    InvalidateOptionsMenu();

                    return true;


                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {


            MenuInflater.Inflate(Resource.Menu.action_menu, menu);
            menItemOnlineIcion = menu.FindItem(Resource.Id.statusOnline);
            menItemOnlineText = menu.FindItem(Resource.Id.statusOnlineText).SetTitle("Online");
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
            {
                outState.PutString("DrawerState", "Opened");
            }

            else
            {
                outState.PutString("DrawerState", "Closed");
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

                waitingUpload = await Azure.AddUser("", userName, gender, age, 0, profilePictureUrl, "0", "0", true, activityLevel, 0);
                userInstanceOne = waitingUpload;


                var newwaitingDownload = await Azure.getUserInstanceByName(userName);
                if (newwaitingDownload.Count != 0)
                    userInstanceOne = newwaitingDownload.FirstOrDefault();

                userId = userInstanceOne.Id;
             //   Toast.MakeText(this, "Welcome! :)", ToastLength.Short).Show();

                points.Text = "Score: 0";

                var setOnline = await Azure.SetUserOnline(userId, true);
                isOnline = true;

            }
            catch (Exception)
            {



            }


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




        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
            await geocoder.GetFromLocationAsync(currentLocation.Latitude, currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                System.Text.StringBuilder deviceAddress = new System.Text.StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                // Remove the last comma from the end of the address.
                if (_address.Text == deviceAddress.ToString())
                {

                }
                else
                    _address.Text = deviceAddress.ToString();
            }


            else
            {
                _address.Text = "Unable to determine the address";
            }
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
            catch (Exception es)
            {


            }
        }



        public async void OnLocationChanged(Location location)
        {
            currentLocation = location;
            if (currentLocation == null)
            {
                _address.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {


                try
                {

                  
                        Address address = await ReverseGeocodeCurrentLocation();
                        if (oldAddress != address)
                        {
                            oldAddress = address;
                            DisplayAddress(address);

                        }


                  
                    var currentPos = new LatLng(currentLocation.Latitude, currentLocation.Longitude);

                    setMarker(currentPos, mMap);
                    mMap.MoveCamera(CameraUpdateFactory.ZoomIn());
                    mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 14));

                }
                catch (Exception )
                {

                  
                }


            }
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


        public class ContactsAdapter : BaseAdapter, IOnMapReadyCallback, IOnMapClickListener
        {
            List<Contact> _contactList;
            public Activity _activity;
            public static List<User> nearbyUsers;
          

            public ContactsAdapter(Activity activity)
            {
                _activity = activity;
                FillContacts();
            }

            void FillContacts()
            {
                _contactList = new List<Contact>();
                _contactList.Add(new Contact
                {
                    Id = 123,
                    DisplayName = "1"
                });
            }


            public void OnMapReady(GoogleMap googleMap)
            {
                mMap = googleMap;
            }

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

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var view = convertView ?? _activity.LayoutInflater.Inflate(Resource.Layout.rightDrawerMenu, parent, false);
                var contactName = view.FindViewById<TextView>(Resource.Id.ContactName);
                contactName.Text = _contactList[position].DisplayName;

                MapFragment mapFrag = (MapFragment)_activity.FragmentManager.FindFragmentById(Resource.Id.map);
                mMap = mapFrag.Map;

                if (mMap != null)
                {
                    mMap.MapType = GoogleMap.MapTypeTerrain;  
                }

                try
                {

              
                mMap.SetOnMapClickListener(this);
                mMap.UiSettings.ZoomControlsEnabled = true;
                mMap.UiSettings.RotateGesturesEnabled = false;
                mMap.UiSettings.ScrollGesturesEnabled = false;

                contactName.Text = "You are displayed as Online";
                contactName.TextSize = 17;
                TextView bearText = view.FindViewById<TextView>(Resource.Id.bear);
                _address = view.FindViewById<TextView>(Resource.Id.location_text);
                Switch location = view.FindViewById<Switch>(Resource.Id.switch1);

                    location.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) {
                    if (e.IsChecked == true)
                    {
                        Toast.MakeText(_activity, "Your location tracking has been turned on", ToastLength.Long).Show();

                        _activity.StartService(new Intent(_activity, typeof(LocationService)));

                        var a = Azure.SetUserOnline(userId, true);
                        isOnline = true;

                        menItemOnlineIcion.SetIcon(Resource.Drawable.greenonline);
                            menItemOnlineText.SetTitle("Online");
                            contactName.Text = "You are displayed as Online";
                        }
                    else
                    {
                        Toast.MakeText(_activity, "Tracking stopped!", ToastLength.Long).Show();
                        _activity.StopService(new Intent(_activity, typeof(LocationService)));

                        var b = Azure.SetUserOnline(userId, false);
                        isOnline = false;
                        menItemOnlineIcion.SetIcon(Resource.Drawable.redoffline);
                            menItemOnlineText.SetTitle("Offline");
                            contactName.Text = "You are displayed as Offline";

                        }
                };

                   
                }
                catch (Exception)
                {

                   
                }

                return view;
            }


         
            public void OnMapClick(LatLng point)
            {
                try
                {
                    Intent myIntent = new Intent(_activity, typeof(GoogleMapsPeople));
                    _activity.StartActivity(myIntent);
                }
                catch (Exception )
                {

                  
                }

            }
        }

        public  override void OnBackPressed()
        {

            Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
            alert.SetTitle("Exit app");
            alert.SetMessage("Do you want to logout of the application?");
            alert.SetPositiveButton("Yes", async (senderAlert, args) => {
              
            base.OnBackPressed();
               
                var wait = await logOff();
                try
                {
                    WelcomeScreen.instance.FinishAffinity();
                    System.Environment.Exit(0);
                    WelcomeScreen.instance.Finish();
                    


                }
                catch (Exception a)
                {

                    throw a;
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
