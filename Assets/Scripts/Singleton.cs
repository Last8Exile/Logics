using System;

public static class Singleton<T> where T : class, new()
{
	public static T Instance
	{
		get 
		{
			if (mInstance == null)
				mInstance = new T();
			return mInstance;
		}
	}
	private static T mInstance;
}
