﻿using System;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using System.Collections.Generic;
using Android.Util;
using Android.Locations;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Content.PM;
using System.Net;
using Android.Graphics.Drawables;
using Android.Gms.Maps;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Query;
using Android.Provider;
using System.Linq;
using static Android.Gms.Maps.GoogleMap;

namespace TestApp
	{

   
    [Activity (Label = "MainMenu",Theme = "@style/MyTheme",ScreenOrientation = ScreenOrientation.Portrait)]
		public class MainStart : ActionBarActivity,ILocationListener,IMobileServiceLocalStore
    {
			 public readonly string logTag = "MainActivity";
			 SupportToolbar mToolbar;
			 MyActionBarDrawerToggle mDrawerToggle;
			 DrawerLayout mDrawerLayout;
			 ListView mLeftDrawer;
			 ListView mRightDrawer;
			 ArrayAdapter mLeftAdapter;
			 ArrayAdapter mRightAdapter;
			 List<string> mLeftDataSet;
			 List<string> mRightDataSet;
			 public string[] table;
			 public string text;
			 public string userID;
			 public ImageView profilePicture;
		     public Bitmap profilePic;
			
			TextView latText;
			TextView longText;
			TextView altText;
			TextView speedText;
			TextView bearText;
			TextView accText;
			LatLng start;
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

        protected override async void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);
				// Set our view from the "main" layout resource
				SetContentView (Resource.Layout.drawerLayout);

				changed = false;
                user = null;
                chk = false;




                mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
				mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
				mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
             // mRightDrawer = FindViewById<ListView>(Resource.Id.right_drawer);

                mRightDrawer = FindViewById<ListView>(Resource.Id.ContactsListView);
               
			    TextView textview1 = FindViewById<TextView> (Resource.Id.textView1);
                loadingImage = FindViewById<ProgressBar>(Resource.Id.progressBar);

                array = Intent.GetStringArrayExtra ("MyData");
				textview1.Text = "Greetings "+ array[0] + "!";
				userName = array [0];
				profilePic = IOUtilz.GetImageBitmapFromUrl(array[1]);
                 profilePicture = FindViewById<ImageView>(Resource.Id.profilePicture);
                 profilePicture.SetImageBitmap(profilePic);
                loadingImage.Visibility = ViewStates.Invisible;
                 userID = array[2];
            //Removes the whole progress bar view, showing only the profile picture

            //Fix to remove space from profile pic and loadingbar
            //  ((ViewGroup)loadingImage.Parent).RemoveView(loadingImage);


          

            mLeftDrawer.Tag = 0;
                //mRightDrawer.Tag = 1;
                mRightDrawer.Tag = 1;
                SetSupportActionBar(mToolbar);

				mLeftDataSet = new List<string>();
				mLeftDataSet.Add ("Profile");
				mLeftDataSet.Add ("Map");
				mLeftDataSet.Add ("Score board");
				mLeftDataSet.Add ("Calculator");
				mLeftDataSet.Add ("Routes");
                mLeftDataSet.Add ("Friends");
                mLeftDataSet.Add ("People nearby");
                mLeftDataSet.Add("Messages");
                mLeftDataSet.Add("Create Route");


            mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;

				mLeftDrawer.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
					var item =	mLeftAdapter.GetItem (e.Position);
					if(e.Position == 0){
						Bundle b = new Bundle ();
						b.PutStringArray ("MyData", new String[] {
							array[0],
							array[1],
							array[2]
						});

					myIntent = new Intent (this, typeof(UserOverview));
						myIntent.PutExtras(b);
						StartActivity(myIntent);

					}else if(e.Position == 1){
						myIntent = new Intent (this, typeof(GoogleMapsActivity));
						StartActivity(myIntent);
					}else if(e.Position == 2){
						myIntent = new Intent (this, typeof(ScoreBoardActivity));
						StartActivity(myIntent);
					}else if(e.Position == 3){
						myIntent = new Intent (this, typeof(Calculator));
						StartActivity(myIntent);
					}else if(e.Position == 4){
                        myIntent = new Intent(this, typeof(UsersRoutes));
                        StartActivity(myIntent);
                    }
                  
                    if (e.Position == 5)
                    {
                        myIntent = new Intent(this, typeof(UsersFriends));
                        StartActivity(myIntent);
                    }
                    else if (e.Position == 6)
                    {
                        myIntent = new Intent(this, typeof(UsersNearby));
                        StartActivity(myIntent);
                    }
                
                    else if (e.Position == 7)
                    {
                        myIntent = new Intent(this, typeof(MainActivity));
                        StartActivity(myIntent);
                    }
                    else if (e.Position == 8)
                    {
                        myIntent = new Intent(this, typeof(CreateRoute));
                        StartActivity(myIntent);
                    }

                };

               var RightAdapter = new ContactsAdapter(this);
          //  var contactsListView = FindViewById<ListView>(Resource.Id.ContactsListView);
            mRightDrawer.Adapter = RightAdapter;

				mDrawerToggle = new MyActionBarDrawerToggle(
					this,							//Host Activity
					mDrawerLayout,					//DrawerLayout
					Resource.String.openDrawer,		//Opened Message
					Resource.String.closeDrawer		//Closed Message
				);

				mDrawerLayout.SetDrawerListener(mDrawerToggle);
				SupportActionBar.SetHomeButtonEnabled(true);
				SupportActionBar.SetDisplayShowTitleEnabled(true);
				mDrawerToggle.SyncState();
				SupportActionBar.SetTitle(Resource.String.openDrawer);

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
				}


            try
            {
                user = await Azure.userRegisteredOnline(userName);
                if (user.Count == 0)
                {
                    Azure.AddUser();
                    Toast.MakeText(this, "Added to item in azure!", ToastLength.Short).Show();
                }

                //Azure.AddLocation();
                //Azure.AddRoute();
                Azure.SetUserOnline(userName, true);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }



                latText = FindViewById<TextView> (Resource.Id.lat);
				longText = FindViewById<TextView> (Resource.Id.longx);
				altText = FindViewById<TextView> (Resource.Id.alt);
				speedText = FindViewById<TextView> (Resource.Id.speed);
				bearText = FindViewById<TextView> (Resource.Id.bear);
				accText = FindViewById<TextView> (Resource.Id.acc);
    
                
                profilePicture = FindViewById<ImageView> (Resource.Id.profilePicture);
                initPersonTracker();

            //var timer = new System.Threading.Timer((e) =>
            //{
            //    MyMethod();
            //}, null, 0, TimeSpan.FromMinutes(5).TotalMilliseconds);

            //Starts the counter for the location updater
            //startTime = DateTime.UtcNow;
            //if (startTime.AddSeconds(20) > DateTime.UtcNow && changed)
            //{
            //    Azure.updateUserLocation(userName);
            //    Toast.MakeText(this, "Your location has been updated!", ToastLength.Short).Show();

            //}


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
            Toast.MakeText(this, "Your location has been updated!", ToastLength.Short).Show();
            var timer = new System.Threading.Timer((e) =>
            {
                Azure.updateUserLocation(userName);

            }, null, 0, Convert.ToInt32(TimeSpan.FromMinutes(1).TotalMilliseconds));

        }


        public void initPersonTracker ()
		{

			//PersonTracker service
			App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
				Log.Debug (logTag, "ServiceConnected Event Raised");
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


			public override bool OnOptionsItemSelected (IMenuItem item)
			{		
				switch (item.ItemId)
				{

				case Android.Resource.Id.Home:
					//The hamburger icon was clicked which means the drawer toggle will handle the event
					//all we need to do is ensure the right drawer is closed so the don't overlap
					mDrawerLayout.CloseDrawer (mRightDrawer);
					mDrawerToggle.OnOptionsItemSelected(item);
					return true;

				case Resource.Id.action_refresh:
					Toast.MakeText (this, "Refreshing...", ToastLength.Short).Show ();
					this.Recreate ();
					return true;

			case Resource.Id.action_help:
				if (mDrawerLayout.IsDrawerOpen (mRightDrawer)) {
					//Right Drawer is already open, close it
					mDrawerLayout.CloseDrawer (mRightDrawer);
				} else {
					//Right Drawer is closed, open it and just in case close left drawer
					mDrawerLayout.OpenDrawer (mRightDrawer);
					mDrawerLayout.CloseDrawer (mLeftDrawer);
				}

				Android.App.AlertDialog alertMessage = new Android.App.AlertDialog.Builder (this).Create ();
				alertMessage.SetTitle ("Info");
				alertMessage.SetMessage ("User: " + userID + System.Environment.NewLine + System.Environment.NewLine + "About: This is a prototype app in a masters project. Developed blablabla.." + System.Environment.NewLine +
				System.Environment.NewLine +
				"Instructions: Open right and left menu by sliding left and right" + System.Environment.NewLine
				+ "Two services have been started, one that tracks the users location, and one that "
				+ "tracks activity, e.g how much a user moves. The app triggers an alarm every three minutes, this will only be triggered" +
				" when the user has been inactive for an hour or so in the finished app.");
				alertMessage.Show ();
					return true;





                // Starts movement tracker (indoor)
                case Resource.Id.action_alarm:

                    if (SimpleService.status == false || SimpleService.Status == false)
                    {
                        StartService(new Intent(this, typeof(SimpleService)));
                        Toast.MakeText(this, "Activity alarm activated", ToastLength.Short).Show();
                        SimpleService.status = true;
                        SimpleService.Status = true;
                    }
                    else if(SimpleService.status == true || SimpleService.Status == false)
                    {
                        StopService(new Intent(this, typeof(SimpleService)));
                        Toast.MakeText(this, "Activity alarm off", ToastLength.Short).Show();
                        SimpleService.status = false;
                        SimpleService.Status = false;
                    }

                    return true;


                default:
					return base.OnOptionsItemSelected (item);
				}
			}

			public override bool OnCreateOptionsMenu (IMenu menu)
			{
				MenuInflater.Inflate (Resource.Menu.action_menu, menu);
				return base.OnCreateOptionsMenu (menu);
			}

			protected override void OnSaveInstanceState (Bundle outState)
			{
				if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
				{
					outState.PutString("DrawerState", "Opened");
				}

				else
				{
					outState.PutString("DrawerState", "Closed");
				}

				base.OnSaveInstanceState (outState);
			}

			protected override void OnPostCreate (Bundle savedInstanceState)
			{
				base.OnPostCreate (savedInstanceState);
				mDrawerToggle.SyncState();
			}

			public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
			{
				base.OnConfigurationChanged (newConfig);
				mDrawerToggle.OnConfigurationChanged(newConfig);
			}



        protected override void OnStop()
        {
            base.OnStop();
            // Clean up: shut down the service when the Activity is no longer visible.
           

            //Added!
           //  StopService(new Intent(this, typeof(SimpleService)));
          //   StopService(new Intent(this, typeof(LocationService)));
        }
        protected override void OnPause()
			{
				Log.Debug (logTag, "Location app is moving to background");
				base.OnPause();
			}

			protected override void OnResume()
			{
				Log.Debug (logTag, "Location app is moving into foreground");
				base.OnPause();
			}

  

            protected async override void OnDestroy ()
			{
            Azure.SetUserOnline(userName, false);
            logOff();
            Log.Debug(logTag, "Location app is becoming inactive");
            base.OnDestroy();
            Azure.SetUserOnline(userName, false);
            logOff();

            Finish();

        }

        public async void logOff()
        {
            user = await Azure.SetUserOnline(userName, false);
        }

			/// Updates UI with location data
			public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
			{
				Location location = e.Location;
				currentLocation = location;

                // these events are on a background thread, need to update on the UI thread
            RunOnUiThread (() => {

					latText.Text = String.Format ("Latitude: {0}", location.Latitude);
					longText.Text = String.Format ("Longitude: {0}", location.Longitude);
					altText.Text = String.Format ("Altitude: {0}", location.Altitude);
					speedText.Text = String.Format ("Speed: {0}", location.Speed);
					accText.Text = String.Format ("Accuracy: {0}", location.Accuracy);
					bearText.Text = String.Format ("Bearing: {0}", location.Bearing);

				});


            OnLocationChanged(location);

            Changed = true;
            changed = true;
            if (changed && !chk)
            {
                chk = true;
            }
        }

			public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
			{
				Log.Debug (logTag, "Location provider disabled event raised");
			}

			public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
			{
				Log.Debug (logTag, "Location provider enabled event raised");
			}

			public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
			{
				Log.Debug (logTag, "Location status changed, event raised");
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
                if(_address.Text == deviceAddress.ToString())
                {

                }else
                _address.Text = deviceAddress.ToString();
            }


            else
            {
                _address.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }
        public async void OnLocationChanged (Location location)
			{
            currentLocation = location;
            if (currentLocation == null)
            {
                _address.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {

                //_address.Text = string.Format("{0:f6},{1:f6}", currentLocation.Latitude, currentLocation.Longitude);

                Address address = await ReverseGeocodeCurrentLocation();
                if(oldAddress != address)
                {
                    oldAddress = address;
                    DisplayAddress(address);
                   
                }

                //    currentPos = new LatLng(currentLocation.Latitude, currentLocation.Longitude);
                //setMarker(currentPos);


            }
        }
			public void OnProviderDisabled (string provider)
			{

			}
			public void OnProviderEnabled (string provider)
			{

			}
			public void OnStatusChanged (string provider, Availability status, Bundle extras)
			{

			}

            public Task InitializeAsync()
            {
                throw new NotImplementedException();
            }

            public Task<global::Newtonsoft.Json.Linq.JToken> ReadAsync(MobileServiceTableQueryDescription query)
            {
                throw new NotImplementedException();
            }

            public Task UpsertAsync(string tableName, IEnumerable<global::Newtonsoft.Json.Linq.JObject> items, bool ignoreMissingColumns)
            {
                throw new NotImplementedException();
            }

            public Task DeleteAsync(MobileServiceTableQueryDescription query)
            {
           
                throw new NotImplementedException();
            }

            public Task DeleteAsync(string tableName, IEnumerable<string> ids)
            {
                throw new NotImplementedException();
            }

            public Task<global::Newtonsoft.Json.Linq.JObject> LookupAsync(string tableName, string id)
            {
                throw new NotImplementedException();
            }



        public class ContactsAdapter :BaseAdapter, IOnMapReadyCallback, IOnMapClickListener
        {
            List<Contact> _contactList;
            Activity _activity;

    
            public static GoogleMap mMap;
            public static List<User> nearbyUsers;
            public static ImageButton findMoreFriends;
          
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
            var view = convertView ?? _activity.LayoutInflater.Inflate(
                Resource.Layout.rightDrawerMenu, parent, false);
            var contactName = view.FindViewById<TextView>(Resource.Id.ContactName);
            contactName.Text = _contactList[position].DisplayName;

                MapFragment mapFrag = (MapFragment)_activity.FragmentManager.FindFragmentById(Resource.Id.map);
                GoogleMap mMap = mapFrag.Map;

                if (mMap != null)
                {
                    mMap.MapType = GoogleMap.MapTypeTerrain;  // The GoogleMap object is ready to go.
                }

                
                contactName.Text = "";
           
                TextView altText;
                TextView speedText;
                TextView bearText;
                TextView accText;

                mMap.SetOnMapClickListener(this);

                altText = view.FindViewById<TextView>(Resource.Id.alt);
                speedText = view.FindViewById<TextView>(Resource.Id.speed);
                bearText = view.FindViewById<TextView>(Resource.Id.bear);
                accText = view.FindViewById<TextView>(Resource.Id.acc);
                _address = view.FindViewById<TextView>(Resource.Id.location_text);

                Switch location = view.FindViewById<Switch>(Resource.Id.switch1);
                location.CheckedChange += delegate (object sender, CompoundButton.CheckedChangeEventArgs e) {
                    if (e.IsChecked == true)
                    {
                        Toast.MakeText(_activity, "Tracking...", ToastLength.Long).Show();
                        _activity.StartService(new Intent(_activity, typeof(LocationService)));
                       
                        Azure.SetUserOnline(userName, true);
                        Android.App.AlertDialog alertMessage = new Android.App.AlertDialog.Builder(_activity).Create();
                        alertMessage.SetTitle("User location tracking");
                        alertMessage.SetMessage("Your location tracking has been turned on");
                        alertMessage.Show();

                    }
                    else
                    {
                        Toast.MakeText(_activity, "Tracking stopped!", ToastLength.Long).Show();
                        _activity.StopService(new Intent(_activity, typeof(LocationService)));
                       
                        Azure.SetUserOnline(userName, false);

                        Android.App.AlertDialog alertMessage = new Android.App.AlertDialog.Builder(_activity).Create();
                        alertMessage.SetTitle("User location tracking");
                        alertMessage.SetMessage("Your location tracking has been turned off");
                        alertMessage.Show();



                    }
                };
              
                findMoreFriends = view.FindViewById<ImageButton>(Resource.Drawable.lupe);
                _activity.RunOnUiThread(async () =>
                {
                   try
                    {
                       
                       // markOnMap(nearbyUsers, mMap);
                        // findMoreFriends.Click += startFriendMap_Click;
                  //    await AddressInitiate();
                    }
                    catch(Exception e)
                    {

                    }
              

                });


     
                return view;
        }

            public static void setMarker(LatLng myPosition, Bitmap pic, GoogleMap mMap)
            {

                MarkerOptions markerOpt1;

                try
                {
                    //if (markerOpt1 != null)
                    //    markerOpt1.Dispose();

                    markerOpt1 = new MarkerOptions();
                    markerOpt1.SetPosition(myPosition);
                    markerOpt1.SetTitle("My Position");
                    markerOpt1.SetSnippet("test");
                    BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(pic); //(Resource.Drawable.test);
                    markerOpt1.SetIcon(image); //BitmapDescriptorFactory.DefaultMarker (BitmapDescriptorFactory.HueCyan));
                    mMap.AddMarker(markerOpt1);
                  //  mMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(myPosition, 15));
                }
                catch (Exception es)
                {


                }
            }
            public async static void markOnMap(List<User> users, GoogleMap mMap)
            {

                try
                {

                    nearbyUsers = await Azure.getImagesOnMap();
                    foreach (User x in nearbyUsers)
                    {
                        setMarker(new LatLng(Convert.ToDouble(x.Lat), Convert.ToDouble(x.Lon)), IOUtilz.GetImageBitmapFromUrl(x.ProfilePicture), mMap);

                    }


                }
                catch (Exception ex)
                {

                }

            }

            public void OnMapClick(LatLng point)
            {
                Intent myIntent = new Intent(_activity, typeof(GoogleMapsPeople));
                _activity.StartActivity(myIntent);
            }
        }  // Contact adapter end










    }


	    }

