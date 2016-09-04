﻿using System.Linq;
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
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
//using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using System.ComponentModel;
using System.Threading;
using Android.Graphics;

namespace TestApp
{

    public class FriendRequestFragment : Fragment
    {
        public static RecyclerView mRecyclerView;
        private RecyclerView.LayoutManager mLayoutManager;
        private RecyclerView.Adapter mAdapter;
        SwipeRefreshLayout mSwipeRefreshLayout;
       //public SupportToolbar toolbar;
        public List<User> myFriends;
        public static List<User> me;
       
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View view;
            myFriends = FriendsOverview.friendRequests;
            me = FriendsOverview.me;

            view = inflater.Inflate(Resource.Layout.FriendRequestRecycleView, container, false);

      
            //TextView txt = new TextView(this.Activity);
            //txt.Text = "Could not find any!";
            //txt.SetTextSize(Android.Util.ComplexUnitType.Sp, 20);
            //txt.SetPadding(10, 10, 10, 10);
            //txt.Gravity = Android.Views.GravityFlags.CenterVertical;
            //txt.Gravity = Android.Views.GravityFlags.CenterHorizontal;
            //txt.SetBackgroundColor(Color.Black);
            //txt.SetTextColor(Color.Black);
            //txt.TextSize = 20;
            //layout.AddView(txt);

            mRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recycleFriendRequest);
            //Create our layout manager
            //mLayoutManager = new LinearLayoutManager(this.Activity);
            //mRecyclerView.SetLayoutManager(mLayoutManager);

            //mAdapter = new UsersFriendRequestAdapter(myFriends, mRecyclerView, this.Activity, this.Activity, mAdapter);
            //mRecyclerView.SetAdapter(mAdapter);

            //LinearLayout layout = view.FindViewById<LinearLayout>(Resource.Id.layout);
             if (myFriends.Count != 0)
            {
                mLayoutManager = new LinearLayoutManager(this.Activity);
                mRecyclerView.SetLayoutManager(mLayoutManager);

                mAdapter = new UsersFriendRequestAdapter(myFriends, mRecyclerView, this.Activity, this.Activity, mAdapter);
                mRecyclerView.SetAdapter(mAdapter);

            }
            else

            {
                TextView txt = view.FindViewById<TextView>(Resource.Id.empty);

                mRecyclerView.Visibility = ViewStates.Invisible;
                txt.Visibility = ViewStates.Visible;
            }



            return view;

        }



        void mSwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Activity.RunOnUiThread(() => { mSwipeRefreshLayout.Refreshing = false; });
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Will run on separate thread
            //  Thread.Sleep(2000);
            myFriends =  FriendsOverview.updateFriendList().Result;
            mAdapter = new UsersFriendRequestAdapter(myFriends, mRecyclerView, this.Activity, this.Activity, mAdapter);
            mRecyclerView.SetAdapter(mAdapter);
            mAdapter.NotifyDataSetChanged();
        }

        public override void OnResume()
        {
            base.OnResume();

        }


        public override void OnPause()
        {
            base.OnPause();

        }


        public override void OnDestroy()
        {
            base.OnDestroy();

        }


        public override void OnLowMemory()
        {
            base.OnLowMemory();

        }


    }

}