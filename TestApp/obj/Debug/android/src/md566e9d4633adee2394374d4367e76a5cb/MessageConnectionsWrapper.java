package md566e9d4633adee2394374d4367e76a5cb;


public class MessageConnectionsWrapper
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("TestApp.MessageConnectionsWrapper, moveFit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MessageConnectionsWrapper.class, __md_methods);
	}


	public MessageConnectionsWrapper () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MessageConnectionsWrapper.class)
			mono.android.TypeManager.Activate ("TestApp.MessageConnectionsWrapper, moveFit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
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