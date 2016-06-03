﻿using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Views.Animations;
using Android.Animation;
using Android.Graphics;
using Android.Support.V4.Widget;
using System.ComponentModel;
using System.Threading;

namespace TestApp
{
    [Activity(Label = "Routes all")]
    public class UsersRoutes : Activity
    {
        private RecyclerView mRecyclerView;
        private RecyclerView.LayoutManager mLayoutManager;
        private RecyclerView.Adapter mAdapter;
        private List<Route> routes;
		

		SwipeRefreshLayout mSwipeRefreshLayout;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            // Set our view from the "main" layout resource
			SetContentView(Resource.Layout.UsersRoutes);

			mSwipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.userRoutes);
			mSwipeRefreshLayout.SetColorSchemeColors(Color.Orange, Color.Green, Color.Yellow, Color.Turquoise,Color.Turquoise);
			mSwipeRefreshLayout.Refresh += mSwipeRefreshLayout_Refresh;



            mRecyclerView = FindViewById<RecyclerView>(Resource.Id.recycleUserRoutes);
            //Create our layout manager
            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);



            routes = new List<Route>();
            List<Route> routeList = await Azure.getRoutes();

            foreach (Route x in routeList)
            {

                routes.Add(x);
            }


            mAdapter = new UsersRoutesAdapter(routeList, mRecyclerView, this);
            mRecyclerView.SetAdapter(mAdapter);



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
			RunOnUiThread(() => { mSwipeRefreshLayout.Refreshing = false; });
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			//Will run on separate thread
			Thread.Sleep(3000);
		}


        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.actionbar_cardview, menu);
        //    return base.OnCreateOptionsMenu(menu);
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item)
        //{
        //    switch(item.ItemId)
        //    {
        //        case Resource.Id.add:
        //            //Add button clicked
        //          //  users.Add(new Email() { Name = "New Name", Subject = "New Subject", Message = "New Message" });
        //            mAdapter.NotifyItemInserted(users.Count - 1);
        //            return true;
        //    }
        //    return base.OnOptionsItemSelected(item);
        //}




    }

    public class UsersRoutesAdapter : RecyclerView.Adapter
    {
        private List<Route> mRoutes;
        private RecyclerView mRecyclerView;
        private Context mContext;
        private int mCurrentPosition = -1;

		public UsersRoutesAdapter(List<Route> routes, RecyclerView recyclerView, Context context)
        {
            mRoutes = routes;
            mRecyclerView = recyclerView;
            mContext = context;

        }

        public class MyView : RecyclerView.ViewHolder
        {
            public View mMainView { get; set; }
            public TextView mUserName { get; set; }
            public TextView mStatus { get; set; }
            public TextView mText { get; set; }
            public ImageView mProfilePicture { get; set; }
            public ImageButton mStartRouteFlag { get; set; }

            public MyView (View view) : base(view)
            {
                mMainView = view;
            }
        }

     
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

                //card view
                View userRoutes = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.userRouteContent, parent, false);

                ImageView profile = userRoutes.FindViewById<ImageView>(Resource.Id.profilePicture);
                TextView name = userRoutes.FindViewById<TextView>(Resource.Id.nameId);
                TextView status = userRoutes.FindViewById<TextView>(Resource.Id.statusId);
                TextView text = userRoutes.FindViewById<TextView>(Resource.Id.textId);
                ImageButton startRoute = userRoutes.FindViewById<ImageButton>(Resource.Id.startRoute);
                MyView view = new MyView(userRoutes) { mUserName = name, mStatus = status, mText = text, mProfilePicture = profile, mStartRouteFlag = startRoute };

            return view;
  
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {

            // First view
                Bitmap userImage;
                MyView myHolder = holder as MyView;
                myHolder.mMainView.Click += mMainView_Click;
                myHolder.mUserName.Text = mRoutes[position].Name;
                myHolder.mStartRouteFlag.Click += StartRouteFlag_Click;
                myHolder.mText.Text = mRoutes[position].Info;

                 userImage = null;
            if (userImage == null)
            {
                myHolder.mProfilePicture.SetImageResource(Resource.Drawable.maps);
            }
            else
                myHolder.mProfilePicture.SetImageBitmap(userImage);

            if (position > mCurrentPosition)
            {
                int currentAnim = Resource.Animation.slide_left_to_right;
                SetAnimation(myHolder.mMainView, currentAnim);
                mCurrentPosition = position;
            }



        }

        private void StartRouteFlag_Click(object sender, EventArgs e)
        {


            string test = "test";

            //test = myHolder.mUserName.Text;
            Bundle b = new Bundle();
            b.PutStringArray("MyData", new String[] {
            test

                 });

            Intent myIntent = new Intent(mContext, typeof(StartRoute));
            myIntent.PutExtras(b);
            mContext.StartActivity(myIntent);
      
    }

        private void SetAnimation(View view, int currentAnim)
        {
            Animator animator = AnimatorInflater.LoadAnimator(mContext, Resource.Animation.flip);
            animator.SetTarget(view);
            animator.Start();
             //Animation anim = AnimationUtils.LoadAnimation(mContext, currentAnim);
             //view.StartAnimation(anim);
        }

        void mMainView_Click(object sender, EventArgs e)
        {
            int position = mRecyclerView.GetChildPosition((View)sender);
           
        }

        public override int ItemCount
        {
            get { return mRoutes.Count; }
        }
    }
}

