package md5cf27010e14af20e69784a5a54418b85f;


public class FaceBookFriendsActivity
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("TestApp.FaceBookFriendsActivity, TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", FaceBookFriendsActivity.class, __md_methods);
	}


	public FaceBookFriendsActivity () throws java.lang.Throwable
	{
		super ();
		if (getClass () == FaceBookFriendsActivity.class)
			mono.android.TypeManager.Activate ("TestApp.FaceBookFriendsActivity, TestApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
