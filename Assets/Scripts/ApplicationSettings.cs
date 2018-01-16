using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ApplicationSettings 
{
	public static string Load(string resourceName)
	{
		var path = Application.persistentDataPath + "/Settings/" + resourceName + ".dat";
		if (!File.Exists (path))
			throw new UnityException ("Невозможно загрузить " + resourceName + ". Не найден соответсвующий файл");
		
		string result;
		using (var streamReader = new StreamReader (path)) 
				result = streamReader.ReadToEnd ();
		return result;
	}

	public static void Save(string resourceName, string content)
	{
		var path = Application.persistentDataPath + "/Settings/" + resourceName + ".dat";
		var directory = Application.persistentDataPath + "/Settings";
		if (!Directory.Exists (directory))
			Directory.CreateDirectory (directory);


		using (var streamWriter = new StreamWriter (path,false)) 
			streamWriter.WriteLine (content);
	}
}
