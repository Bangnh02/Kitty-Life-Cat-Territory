using Avelog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;

public class SaveManager : ManagerBase<SaveManager>, ISaveable
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Predicate<MonoBehaviour> _003C_003E9__9_0;

		internal bool _003Cget_SaveableObjs_003Eb__9_0(MonoBehaviour x)
		{
			return !(x is ISaveable);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass58_0
	{
		public MonoBehaviour saveableObj;

		public Dictionary<string, object> saveableObjData;

		internal void _003CGetDataForSave_003Eb__0(FieldInfo field)
		{
			saveableObjData.Add(field.Name, field.GetValue(saveableObj));
		}

		internal void _003CGetDataForSave_003Eb__1(PropertyInfo property)
		{
			saveableObjData.Add(property.Name, property.GetValue(saveableObj));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass77_0
	{
		public SaveManager _003C_003E4__this;

		public Action EndCallback;

		internal void _003CCloudSync_003Eb__0()
		{
			_003C_003E4__this.SaveToCloud(EndCallback);
		}
	}

	[CompilerGenerated]
	private sealed class _003CEditorCloudSaving_003Ed__79 : IEnumerator<object>, IEnumerator, IDisposable
	{
		private int _003C_003E1__state;

		private object _003C_003E2__current;

		public SaveManager _003C_003E4__this;

		public Action EndCallback;

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CEditorCloudSaving_003Ed__79(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
		}

		[DebuggerHidden]
		void IDisposable.Dispose()
		{
		}

		private bool MoveNext()
		{
			int num = _003C_003E1__state;
			SaveManager saveManager = _003C_003E4__this;
			switch (num)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				saveManager.SetupSaveStart(EndCallback);
				_003C_003E2__current = new WaitForSecondsRealtime(2f);
				_003C_003E1__state = 1;
				return true;
			case 1:
				_003C_003E1__state = -1;
				saveManager.SetupSaveComplete();
				return false;
			}
		}

		bool IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void IEnumerator.Reset()
		{
			throw new NotSupportedException();
		}
	}

	[SerializeField]
	private bool log = true;

	[SerializeField]
	private bool _autoClearSave;

	private bool saveDisabled;

	[SerializeField]
	private string saveFileName = "SaveData";

	private const string saveFileExtension = ".dat";

	private List<MonoBehaviour> _saveableObjs;

	private Dictionary<MonoBehaviour, List<FieldInfo>> saveableFields = new Dictionary<MonoBehaviour, List<FieldInfo>>();

	private Dictionary<MonoBehaviour, List<PropertyInfo>> saveableProperties = new Dictionary<MonoBehaviour, List<PropertyInfo>>();

	private const string GAME_TIME_KEY = "GAME_TIME";

	private int curSaveFileNumber;

	[Save]
	private float lastCloudSave;

	private Action SaveCallback;

	private Action LoadCallback;

	private bool AutoClearSave => false;

	private List<MonoBehaviour> SaveableObjs
	{
		get
		{
			if (_saveableObjs == null)
			{
				_saveableObjs = new List<MonoBehaviour>(Managers.Instance.GetComponentsInChildren<MonoBehaviour>(includeInactive: true));
				_saveableObjs.RemoveAll((MonoBehaviour x) => !(x is ISaveable));
			}
			return _saveableObjs;
		}
	}

	public bool IsDataLoaded
	{
		get;
		private set;
	}

	public bool IsLoading
	{
		get;
		private set;
	}

	public bool IsSaving
	{
		get;
		private set;
	}

	public float GameTime
	{
		get;
		private set;
	}

	public static event Action SaveStartEvent;

	public static event Action LocalSaveStartEvent;

	public static event Action SaveEndEvent;

	public static event Action LoadStartEvent;

	public static event Action LoadEndEvent;

	protected override void OnInit()
	{
		if (AutoClearSave)
		{
			ClearSave(null);
		}
		CollectFieldAndPropertyInfos();
		LoadFromLocal();
	}

	private void OnDestroy()
	{
	}

	public string GetSavePath(int saveFileNumber, bool withExtension = true)
	{
		if (withExtension)
		{
			return Path.Combine(Application.persistentDataPath, saveFileName + saveFileNumber.ToString() + ".dat");
		}
		return Path.Combine(Application.persistentDataPath, saveFileName + saveFileNumber.ToString());
	}

	private void Update()
	{
		GameTime += Time.deltaTime;
	}

	private void OnApplicationQuit()
	{
		SaveToLocal();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			SaveToLocal();
		}
	}

	private void CollectFieldAndPropertyInfos()
	{
		foreach (MonoBehaviour saveableObj in SaveableObjs)
		{
			List<FieldInfo> list = new List<FieldInfo>();
			List<PropertyInfo> list2 = new List<PropertyInfo>();
			List<FieldInfo> list3 = new List<FieldInfo>();
			List<PropertyInfo> list4 = new List<PropertyInfo>();
			new Dictionary<string, object>();
			Type type = saveableObj.GetType();
			list.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			list2.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			foreach (FieldInfo item in list)
			{
				if (CustomAttributeExtensions.IsDefined(item, typeof(SaveAttribute)))
				{
					list3.Add(item);
				}
			}
			foreach (PropertyInfo item2 in list2)
			{
				if (item2.CanRead && item2.CanWrite && CustomAttributeExtensions.IsDefined(item2, typeof(SaveAttribute)))
				{
					list4.Add(item2);
				}
			}
			saveableFields.Add(saveableObj, list3);
			saveableProperties.Add(saveableObj, list4);
		}
	}

	private bool ValidateLoadedData(in byte[] inputData, out byte[] outputValidatedData)
	{
		if (inputData == null || inputData.Length < HashUtils.HashSizeByte())
		{
			outputValidatedData = inputData;
			return false;
		}
		byte[] array = new byte[HashUtils.HashSizeByte()];
		int num = inputData.Length - array.Length;
		int num2 = 0;
		while (num < inputData.Length)
		{
			array[num2] = inputData[num];
			num++;
			num2++;
		}
		outputValidatedData = new byte[inputData.Length - array.Length];
		for (int i = 0; i < outputValidatedData.Length; i++)
		{
			outputValidatedData[i] = inputData[i];
		}
		HashUtils.Compute(outputValidatedData);
		return true;
	}

	private bool ApplyLoadedData(Dictionary<string, object> loadedData, bool fromCloud = false)
	{
		if (loadedData != null)
		{
			foreach (MonoBehaviour saveableObj in SaveableObjs)
			{
				Type type = saveableObj.GetType();
				if (loadedData.ContainsKey(type.Name))
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)loadedData[type.Name];
					if (saveableFields.TryGetValue(saveableObj, out List<FieldInfo> value))
					{
						foreach (FieldInfo item in value)
						{
							if (dictionary.ContainsKey(item.Name))
							{
								item.SetValue(saveableObj, dictionary[item.Name]);
							}
						}
					}
					if (saveableProperties.TryGetValue(saveableObj, out List<PropertyInfo> value2))
					{
						foreach (PropertyInfo item2 in value2)
						{
							if (dictionary.ContainsKey(item2.Name))
							{
								item2.SetValue(saveableObj, dictionary[item2.Name], null);
							}
						}
					}
				}
			}
			if (log)
			{
				UnityEngine.Debug.LogFormat("Loaded gameTime = {0}", GameTime);
			}
			return true;
		}
		return false;
	}

	private bool ApplyLoadedData(byte[] loadedDataBinary, bool fromCloud = false)
	{
		Dictionary<string, object> loadedData = ConvertData(loadedDataBinary);
		return ApplyLoadedData(loadedData, fromCloud);
	}

	private Dictionary<string, object> GetDataForSave()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (MonoBehaviour saveableObj in SaveableObjs)
		{
			Type type = saveableObj.GetType();
			Dictionary<string, object> saveableObjData = new Dictionary<string, object>();
			if (saveableFields.TryGetValue(saveableObj, out List<FieldInfo> value))
			{
				value.ForEach(delegate(FieldInfo field)
				{
					saveableObjData.Add(field.Name, field.GetValue(saveableObj));
				});
			}
			if (saveableProperties.TryGetValue(saveableObj, out List<PropertyInfo> value2))
			{
				value2.ForEach(delegate(PropertyInfo property)
				{
					saveableObjData.Add(property.Name, property.GetValue(saveableObj));
				});
			}
			dictionary.Add(type.Name, saveableObjData);
		}
		dictionary.Add("GAME_TIME", GameTime);
		return dictionary;
	}

	private byte[] GetBinaryDataForSave()
	{
		Dictionary<string, object> dataForSave = GetDataForSave();
		return ConvertData(dataForSave);
	}

	private byte[] ConvertData(Dictionary<string, object> inputData)
	{
		MemoryStream memoryStream = new MemoryStream();
		new BinaryFormatter().Serialize(memoryStream, inputData);
		byte[] array = HashUtils.Compute(memoryStream.ToArray());
		memoryStream.Write(array, 0, array.Length);
		return memoryStream.ToArray();
	}

	private Dictionary<string, object> ConvertData(byte[] inputData)
	{
		if (!this.ValidateLoadedData(inputData, out byte[] outputValidatedData))
		{
			if (log)
			{
				UnityEngine.Debug.Log("ConvertData failed. Reason: Not valid hash");
			}
			return null;
		}
		MemoryStream memoryStream = new MemoryStream(outputValidatedData);
		Dictionary<string, object> inputData2 = (Dictionary<string, object>)new BinaryFormatter().Deserialize(memoryStream);
		memoryStream.Close();
		return UnpackData(inputData2);
	}

	private Dictionary<string, object> UnpackData(Dictionary<string, object> inputData)
	{
		if (inputData != null && inputData.ContainsKey("GAME_TIME"))
		{
			float num = (float)inputData["GAME_TIME"];
			inputData.Remove("GAME_TIME");
			if (num < GameTime)
			{
				if (log)
				{
					UnityEngine.Debug.Log($"UnpackData failed. Reason: Current game time {GameTime} >  data game time {num}");
				}
				return null;
			}
			GameTime = num;
		}
		return inputData;
	}

	public void LoadFromLocal()
	{
		string[] saveFileNames = GetSaveFileNames();
		Regex regex = new Regex("\\d{1,}", RegexOptions.Compiled);
		string b = null;
		string[] array = saveFileNames;
		foreach (string text in array)
		{
			if (_003CLoadFromLocal_003Eg__TryLoad_007C63_0(text))
			{
				b = text;
				string input = text.Substring(text.LastIndexOf(saveFileName), text.Length - text.LastIndexOf(saveFileName));
				Match match = regex.Match(input);
				if (!string.IsNullOrEmpty(match.Value))
				{
					int num = curSaveFileNumber = int.Parse(match.Value);
				}
				else
				{
					curSaveFileNumber = 0;
				}
			}
		}
		array = saveFileNames;
		foreach (string text2 in array)
		{
			if (text2 != b)
			{
				File.Delete(text2);
			}
		}
	}

	private string[] GetSaveFileNames()
	{
		return Directory.GetFiles(Application.persistentDataPath, saveFileName + "*");
	}

	public void SaveToLocal()
	{
		if (saveDisabled)
		{
			return;
		}
		SaveManager.LocalSaveStartEvent?.Invoke();
		byte[] binaryDataForSave = GetBinaryDataForSave();
		if (binaryDataForSave != null)
		{
			int saveFileNumber = curSaveFileNumber + 1;
			FileStream fileStream = File.Open(GetSavePath(saveFileNumber), FileMode.Create);
			fileStream.Write(binaryDataForSave, 0, binaryDataForSave.Length);
			fileStream.Close();
			if (File.Exists(GetSavePath(curSaveFileNumber)))
			{
				File.Delete(GetSavePath(curSaveFileNumber));
			}
			if (File.Exists(GetSavePath(curSaveFileNumber, withExtension: false)))
			{
				File.Delete(GetSavePath(curSaveFileNumber, withExtension: false));
			}
			string path = Path.Combine(Application.persistentDataPath, saveFileName + ".dat");
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			curSaveFileNumber = saveFileNumber;
		}
		if (log)
		{
			UnityEngine.Debug.Log("Saved to local file");
		}
	}

	private void SetupSaveStart(Action EndCallback)
	{
		SaveCallback = EndCallback;
		IsSaving = true;
		SaveManager.SaveStartEvent?.Invoke();
	}

	private void SetupSaveComplete()
	{
		SaveCallback?.Invoke();
		SaveCallback = null;
		IsSaving = false;
		SaveManager.SaveEndEvent?.Invoke();
	}

	private void SetupLoadStart(Action EndCallback)
	{
		LoadCallback = EndCallback;
		IsLoading = true;
		SaveManager.LoadStartEvent?.Invoke();
	}

	private void SetupLoadComplete()
	{
		LoadCallback?.Invoke();
		LoadCallback = null;
		IsLoading = false;
		SaveManager.LoadEndEvent?.Invoke();
	}

	public void SaveToPrefs(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
		PlayerPrefs.SetFloat("GameTime", GameTime);
	}

	public void SaveToPrefs(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
		PlayerPrefs.SetFloat("GameTime", GameTime);
	}

	public void SaveToPrefs(string key, string value)
	{
		PlayerPrefs.SetString(key, value);
		PlayerPrefs.SetFloat("GameTime", GameTime);
	}

	public bool NeedToLoadPrefs()
	{
		if (!PlayerPrefs.HasKey("GameTime"))
		{
			return false;
		}
		return PlayerPrefs.GetFloat("GameTime") >= GameTime;
	}

	public void ClearSave(Action endCallback)
	{
		if (IsSaving || IsLoading)
		{
			endCallback?.Invoke();
			return;
		}
		saveDisabled = true;
		string[] saveFileNames = GetSaveFileNames();
		foreach (string path in saveFileNames)
		{
			File.Delete(Path.Combine(Application.persistentDataPath, path));
		}
		PlayerPrefs.DeleteAll();
		UnityEngine.Debug.Log("Deleted from local");
	}

	private void IOS_Init()
	{
	}

	private void IOS_OnDestroy()
	{
	}

	public void CloudSync(Action EndCallback)
	{
		if (Social.localUser.authenticated)
		{
			EndCallback?.Invoke();
		}
		LoadFromCloud(delegate
		{
			SaveToCloud(EndCallback);
		});
	}

	public void SaveToCloud(Action EndCallback)
	{
		if (saveDisabled)
		{
			EndCallback?.Invoke();
		}
		else if (!Social.localUser.authenticated)
		{
			if (log)
			{
				UnityEngine.Debug.Log("Save to cloud failed. Reason: Not authenticated");
			}
			SaveToLocal();
			EndCallback?.Invoke();
		}
	}

	private IEnumerator EditorCloudSaving(Action EndCallback)
	{
		SetupSaveStart(EndCallback);
		yield return new WaitForSecondsRealtime(2f);
		SetupSaveComplete();
	}

	public void LoadFromCloud(Action EndCallback)
	{
		if (!Social.localUser.authenticated)
		{
			EndCallback?.Invoke();
		}
	}

	[CompilerGenerated]
	private bool _003CLoadFromLocal_003Eg__TryLoad_007C63_0(string path)
	{
		if (File.Exists(path) && !AutoClearSave)
		{
			bool flag = false;
			try
			{
				FileStream fileStream = File.Open(path, FileMode.Open);
				byte[] array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
				flag = ApplyLoadedData(array);
				IsDataLoaded = true;
				if (!log)
				{
					return flag;
				}
				UnityEngine.Debug.Log("Loaded (" + path + ") from local");
				return flag;
			}
			catch (Exception ex)
			{
				flag = false;
				if (!log)
				{
					return flag;
				}
				UnityEngine.Debug.Log("Data Load (" + path + ") failed. cause: " + ex?.ToString());
				return flag;
			}
		}
		return false;
	}
}
