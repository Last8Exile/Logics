using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

public static class MyJsonSerializer 
{
	public static JsonSerializer UpdatableSerialiser 
	{
		get 
		{
			if (mUpdatableSerialiser == null)
			{
				mUpdatableSerialiser = new JsonSerializer();
				mUpdatableSerialiser.Formatting = Formatting.Indented;
				mUpdatableSerialiser.TypeNameHandling = TypeNameHandling.Auto;
				mUpdatableSerialiser.Converters.Add(Singleton<UpdateConverter>.Instance);
				mUpdatableSerialiser.Converters.Add(Singleton<Vector3Converter>.Instance);
				mUpdatableSerialiser.Converters.Add(Singleton<Vector2Converter>.Instance);
			}
			return mUpdatableSerialiser;
		}
	}
	private static JsonSerializer mUpdatableSerialiser;

	public static JsonSerializer DefaultSerialiser 
	{
		get 
		{
			if (mDefaultSerialiser == null)
			{
				mDefaultSerialiser = new JsonSerializer();
				mDefaultSerialiser.Formatting = Formatting.Indented;
				mDefaultSerialiser.TypeNameHandling = TypeNameHandling.Auto;
				mDefaultSerialiser.Converters.Add(Singleton<Vector3Converter>.Instance);
				mDefaultSerialiser.Converters.Add(Singleton<Vector2Converter>.Instance);
			}
			return mDefaultSerialiser;
		}
	}
	private static JsonSerializer mDefaultSerialiser;


	public static string Serialize(object obj)
	{
		var serialiser = DefaultSerialiser;
		string result;
		using (var stream = new MemoryStream())
		{
			using (var streamWriter = new StreamWriter(stream))
			{
				serialiser.Serialize(streamWriter, obj);
				streamWriter.Flush();
				stream.Position = 0;

				using (var streamReader = new StreamReader(stream))
				{
					result = streamReader.ReadToEnd();
				}
			}
		}
		return result;
	}

	public static T Deserialize<T>(string data)
	{
		T obj;
		var deserializer = UpdatableSerialiser;
		using (var stream = new MemoryStream())
		using (var streamWriter = new StreamWriter(stream))
		{
			streamWriter.Write(data);
			streamWriter.Flush();
			stream.Position = 0;

			using (var streamReader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				obj = deserializer.Deserialize<T>(jsonReader);
			}
		}
		return obj;
	}
}

public class UpdateConverter : JsonConverter
{
	private bool mSelfUse;

	public override bool CanConvert(Type objectType)
	{
		if (mSelfUse)
		{
			mSelfUse = false;
			return false;
		}
		return typeof(IUpdateable).IsAssignableFrom(objectType);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var token = JToken.ReadFrom(reader);
		if (token.Type == JTokenType.Null)
		{
			existingValue = null;
			return null;
		}
		var lastVersionInfo = objectType.GetField("LastVersion", BindingFlags.NonPublic | BindingFlags.Static);
		var lastVersion = (int)lastVersionInfo.GetValue(null);
		var version = token.Value<int>(Version);
		while (version < lastVersion)
		{
			var migrateInfo = objectType.GetMethod("Migrate_" + version.ToString(), BindingFlags.NonPublic | BindingFlags.Static);
			version = (int)migrateInfo.Invoke(null, new object[1] { token });
		}
		token[Version] = version;
		using (var jsonReaderUpdated = token.CreateReader())
		{
			mSelfUse = true;
			existingValue = serializer.Deserialize(jsonReaderUpdated, objectType);
			mSelfUse = false;
		}
		return existingValue;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotSupportedException();
	}

	public override bool CanWrite {
		get {
			return false;
		}
	}

	private const string Version = "Version";
}

public interface IUpdateable
{
	
}

public class Vector3Converter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector3);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		throw new NotSupportedException();
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var vec = (Vector3)value;
		var obj = new JObject();
		obj["x"] = vec.x;
		obj["y"] = vec.y;
		obj["z"] = vec.z;
		obj.WriteTo(writer);
	}

	public override bool CanRead {
		get {
			return false;
		}
	}
}

public class Vector2Converter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector2);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		throw new NotSupportedException();
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var vec = (Vector2)value;
		var obj = new JObject();
		obj["x"] = vec.x;
		obj["y"] = vec.y;
		obj.WriteTo(writer);
	}

	public override bool CanRead {
		get {
			return false;
		}
	}
}