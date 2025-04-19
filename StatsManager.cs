using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x0200016A RID: 362
public class StatsManager : MonoBehaviour
{
	// Token: 0x06000C20 RID: 3104 RVA: 0x0006BCE4 File Offset: 0x00069EE4
	private void Awake()
	{
		if (!StatsManager.instance)
		{
			StatsManager.instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06000C21 RID: 3105 RVA: 0x0006BD0F File Offset: 0x00069F0F
	public int TimePlayedGetHours(float _timePlayed)
	{
		return (int)(_timePlayed / 3600f);
	}

	// Token: 0x06000C22 RID: 3106 RVA: 0x0006BD19 File Offset: 0x00069F19
	public int TimePlayedGetMinutes(float _timePlayed)
	{
		return (int)(_timePlayed % 3600f / 60f);
	}

	// Token: 0x06000C23 RID: 3107 RVA: 0x0006BD29 File Offset: 0x00069F29
	public int TimePlayedGetSeconds(float _timePlayed)
	{
		return (int)(_timePlayed % 60f);
	}

	// Token: 0x06000C24 RID: 3108 RVA: 0x0006BD33 File Offset: 0x00069F33
	private void Update()
	{
		if (!SemiFunc.RunIsLobbyMenu() && !SemiFunc.IsMainMenu())
		{
			this.timePlayed += Time.deltaTime;
		}
	}

	// Token: 0x06000C25 RID: 3109 RVA: 0x0006BD58 File Offset: 0x00069F58
	private void Start()
	{
		this.dictionaryOfDictionaries.Add("runStats", this.runStats);
		this.dictionaryOfDictionaries.Add("itemsPurchased", this.itemsPurchased);
		this.dictionaryOfDictionaries.Add("itemsPurchasedTotal", this.itemsPurchasedTotal);
		this.dictionaryOfDictionaries.Add("itemsUpgradesPurchased", this.itemsUpgradesPurchased);
		this.dictionaryOfDictionaries.Add("itemBatteryUpgrades", this.itemBatteryUpgrades);
		this.dictionaryOfDictionaries.Add("playerHealth", this.playerHealth);
		this.dictionaryOfDictionaries.Add("playerUpgradeHealth", this.playerUpgradeHealth);
		this.dictionaryOfDictionaries.Add("playerUpgradeStamina", this.playerUpgradeStamina);
		this.dictionaryOfDictionaries.Add("playerUpgradeExtraJump", this.playerUpgradeExtraJump);
		this.dictionaryOfDictionaries.Add("playerUpgradeLaunch", this.playerUpgradeLaunch);
		this.dictionaryOfDictionaries.Add("playerUpgradeMapPlayerCount", this.playerUpgradeMapPlayerCount);
		this.dictionaryOfDictionaries.Add("playerColor", this.playerColor);
		this.dictionaryOfDictionaries.Add("playerUpgradeSpeed", this.playerUpgradeSpeed);
		this.dictionaryOfDictionaries.Add("playerUpgradeStrength", this.playerUpgradeStrength);
		this.dictionaryOfDictionaries.Add("playerUpgradeRange", this.playerUpgradeRange);
		this.dictionaryOfDictionaries.Add("playerUpgradeThrow", this.playerUpgradeThrow);
		this.dictionaryOfDictionaries.Add("playerInventorySpot1", this.playerInventorySpot1);
		this.dictionaryOfDictionaries.Add("playerInventorySpot2", this.playerInventorySpot2);
		this.dictionaryOfDictionaries.Add("playerInventorySpot3", this.playerInventorySpot3);
		this.dictionaryOfDictionaries.Add("playerHasCrown", this.playerHasCrown);
		this.dictionaryOfDictionaries.Add("item", this.item);
		this.dictionaryOfDictionaries.Add("itemStatBattery", this.itemStatBattery);
		this.doNotSaveTheseDictionaries.Add("playerInventorySpot1");
		this.doNotSaveTheseDictionaries.Add("playerInventorySpot2");
		this.doNotSaveTheseDictionaries.Add("playerInventorySpot3");
		this.doNotSaveTheseDictionaries.Add("playerColor");
		this.RunStartStats();
	}

	// Token: 0x06000C26 RID: 3110 RVA: 0x0006BF90 File Offset: 0x0006A190
	public void DictionaryFill(string dictionaryName, int value)
	{
		foreach (string key in new List<string>(this.dictionaryOfDictionaries[dictionaryName].Keys))
		{
			this.dictionaryOfDictionaries[dictionaryName][key] = value;
		}
	}

	// Token: 0x06000C27 RID: 3111 RVA: 0x0006C000 File Offset: 0x0006A200
	public void PlayerAdd(string _steamID, string _playerName)
	{
		this.SetPlayerHealthStart(_steamID, 100);
		this.PlayerInventorySpotsInit(_steamID);
		this.PlayerAddName(_steamID, _playerName);
		foreach (Dictionary<string, int> dictionary in this.AllDictionariesWithPrefix("player"))
		{
			if (!dictionary.ContainsKey(_steamID))
			{
				dictionary.Add(_steamID, 0);
			}
		}
		if (!this.playerColor.ContainsKey(_steamID))
		{
			this.playerColor[_steamID] = -1;
		}
	}

	// Token: 0x06000C28 RID: 3112 RVA: 0x0006C098 File Offset: 0x0006A298
	private void PlayerInventorySpotsInit(string _steamID)
	{
		if (!this.playerInventorySpot1.ContainsKey(_steamID))
		{
			this.playerInventorySpot1.Add(_steamID, -1);
		}
		if (!this.playerInventorySpot2.ContainsKey(_steamID))
		{
			this.playerInventorySpot2.Add(_steamID, -1);
		}
		if (!this.playerInventorySpot3.ContainsKey(_steamID))
		{
			this.playerInventorySpot3.Add(_steamID, -1);
		}
		if (!this.playerInventorySpot1Taken.ContainsKey(_steamID))
		{
			this.playerInventorySpot1Taken.Add(_steamID, 0);
		}
		if (!this.playerInventorySpot2Taken.ContainsKey(_steamID))
		{
			this.playerInventorySpot2Taken.Add(_steamID, 0);
		}
		if (!this.playerInventorySpot3Taken.ContainsKey(_steamID))
		{
			this.playerInventorySpot3Taken.Add(_steamID, 0);
		}
	}

	// Token: 0x06000C29 RID: 3113 RVA: 0x0006C148 File Offset: 0x0006A348
	public List<string> SaveFileGetPlayerNames(string fileName)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (!File.Exists(text))
		{
			Debug.LogWarning("Save file not found in " + text);
			return null;
		}
		ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
		if (ES3.KeyExists("playerNames", settings))
		{
			Dictionary<string, string> dictionary = ES3.Load<Dictionary<string, string>>("playerNames", settings);
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				list.Add(keyValuePair.Value);
			}
			return list;
		}
		Debug.LogWarning("Key 'playerNames' not found in loaded data.");
		return null;
	}

	// Token: 0x06000C2A RID: 3114 RVA: 0x0006C224 File Offset: 0x0006A424
	public float SaveFileGetTimePlayed(string _fileName)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			_fileName,
			"/",
			_fileName,
			".es3"
		});
		if (!File.Exists(text))
		{
			Debug.LogWarning("Save file not found in " + text);
			return 0f;
		}
		ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
		if (ES3.KeyExists("timePlayed", settings))
		{
			return ES3.Load<float>("timePlayed", settings);
		}
		Debug.LogWarning("Key 'timePlayed' not found in loaded data.");
		return 0f;
	}

	// Token: 0x06000C2B RID: 3115 RVA: 0x0006C2BC File Offset: 0x0006A4BC
	public void SaveFileCreate()
	{
		string fileName = "REPO_SAVE_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture);
		this.saveFileCurrent = fileName;
		this.SaveGame(fileName);
	}

	// Token: 0x06000C2C RID: 3116 RVA: 0x0006C2F9 File Offset: 0x0006A4F9
	public void SaveFileSave()
	{
		this.SaveGame(this.saveFileCurrent);
	}

	// Token: 0x06000C2D RID: 3117 RVA: 0x0006C308 File Offset: 0x0006A508
	public string SaveFileGetTeamName(string fileName)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (File.Exists(text))
		{
			ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
			return ES3.Load<string>("teamName", settings);
		}
		Debug.LogWarning("Save file not found in " + text);
		return null;
	}

	// Token: 0x06000C2E RID: 3118 RVA: 0x0006C380 File Offset: 0x0006A580
	public string SaveFileGetDateAndTime(string fileName)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (File.Exists(text))
		{
			ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
			return ES3.Load<string>("dateAndTime", settings);
		}
		Debug.LogWarning("Save file not found in " + text);
		return null;
	}

	// Token: 0x06000C2F RID: 3119 RVA: 0x0006C3F8 File Offset: 0x0006A5F8
	private string SaveFileGetRunStat(string fileName, string _runStat)
	{
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (!File.Exists(text))
		{
			Debug.LogWarning("Save file not found in " + text);
			return null;
		}
		ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
		Dictionary<string, Dictionary<string, int>> dictionary = ES3.Load<Dictionary<string, Dictionary<string, int>>>("dictionaryOfDictionaries", settings);
		if (dictionary == null || !dictionary.ContainsKey("runStats"))
		{
			Debug.LogWarning("Key 'runStats' not found in loaded data.");
			return null;
		}
		Dictionary<string, int> dictionary2 = dictionary["runStats"];
		if (dictionary2 != null && dictionary2.ContainsKey(_runStat))
		{
			return dictionary2[_runStat].ToString();
		}
		Debug.LogWarning("Key 'level' not found in 'runStats'.");
		return null;
	}

	// Token: 0x06000C30 RID: 3120 RVA: 0x0006C4BE File Offset: 0x0006A6BE
	public string SaveFileGetRunLevel(string fileName)
	{
		return this.SaveFileGetRunStat(fileName, "level");
	}

	// Token: 0x06000C31 RID: 3121 RVA: 0x0006C4CC File Offset: 0x0006A6CC
	public string SaveFileGetRunCurrency(string fileName)
	{
		return this.SaveFileGetRunStat(fileName, "currency");
	}

	// Token: 0x06000C32 RID: 3122 RVA: 0x0006C4DA File Offset: 0x0006A6DA
	public string SaveFileGetTotalHaul(string fileName)
	{
		return this.SaveFileGetRunStat(fileName, "totalHaul");
	}

	// Token: 0x06000C33 RID: 3123 RVA: 0x0006C4E8 File Offset: 0x0006A6E8
	private void RunStartStats()
	{
		this.runStats.Clear();
		this.runStats.Add("level", 0);
		this.runStats.Add("currency", 0);
		this.runStats.Add("lives", 3);
		this.runStats.Add("chargingStationCharge", 1);
		this.runStats.Add("totalHaul", 0);
		this.statsSynced = true;
		this.LoadItemsFromFolder();
		this.DictionaryFill("itemsPurchased", 0);
		this.DictionaryFill("itemsPurchasedTotal", 0);
		this.DictionaryFill("itemsUpgradesPurchased", 0);
		this.itemsPurchased["Item Power Crystal"] = 1;
		this.itemsPurchasedTotal["Item Power Crystal"] = 1;
		this.itemsPurchased["Item Cart Medium"] = 1;
		this.itemsPurchasedTotal["Item Cart Medium"] = 1;
		this.playerColorIndex = 0;
	}

	// Token: 0x06000C34 RID: 3124 RVA: 0x0006C5D1 File Offset: 0x0006A7D1
	private void PlayerAddName(string _steamID, string _playerName)
	{
		if (this.playerNames.ContainsKey(_steamID))
		{
			this.playerNames[_steamID] = _playerName;
			return;
		}
		this.playerNames.Add(_steamID, _playerName);
	}

	// Token: 0x06000C35 RID: 3125 RVA: 0x0006C5FC File Offset: 0x0006A7FC
	public Dictionary<string, int> FetchPlayerUpgrades(string _steamID)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Regex regex = new Regex("(?<!^)(?=[A-Z])");
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.dictionaryOfDictionaries)
		{
			if (keyValuePair.Key.StartsWith("playerUpgrade") && keyValuePair.Value.ContainsKey(_steamID))
			{
				string text = "";
				string[] array = regex.Split(keyValuePair.Key);
				bool flag = false;
				foreach (string text2 in array)
				{
					if (flag)
					{
						text = text + text2 + " ";
					}
					if (text2 == "Upgrade")
					{
						flag = true;
					}
				}
				text = text.Trim();
				int value = keyValuePair.Value[_steamID];
				dictionary.Add(text, value);
			}
		}
		return dictionary;
	}

	// Token: 0x06000C36 RID: 3126 RVA: 0x0006C700 File Offset: 0x0006A900
	public void DictionaryUpdateValue(string dictionaryName, string key, int value)
	{
		if (this.dictionaryOfDictionaries.ContainsKey(dictionaryName) && this.dictionaryOfDictionaries[dictionaryName].ContainsKey(key))
		{
			this.dictionaryOfDictionaries[dictionaryName][key] = value;
		}
	}

	// Token: 0x06000C37 RID: 3127 RVA: 0x0006C737 File Offset: 0x0006A937
	public void ItemUpdateStatBattery(string itemName, int value, bool sync = true)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		if (this.itemStatBattery.ContainsKey(itemName))
		{
			this.itemStatBattery[itemName] = value;
			if (sync)
			{
				PunManager.instance.UpdateStat("itemStatBattery", itemName, value);
			}
		}
	}

	// Token: 0x06000C38 RID: 3128 RVA: 0x0006C770 File Offset: 0x0006A970
	public void PlayerInventoryUpdate(string _steamID, string itemName, int spot, bool sync = true)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		int value = itemName.GetHashCode();
		if (itemName == "")
		{
			value = -1;
		}
		if (spot == 0)
		{
			this.playerInventorySpot1[_steamID] = value;
			if (this.playerInventorySpot1[_steamID] != -1)
			{
				this.playerInventorySpot1Taken[_steamID] = 1;
			}
			else
			{
				this.playerInventorySpot1Taken[_steamID] = 0;
			}
			if (sync)
			{
				PunManager.instance.UpdateStat("playerInventorySpot1", itemName, spot);
			}
		}
		if (spot == 1)
		{
			this.playerInventorySpot2[_steamID] = value;
			if (this.playerInventorySpot2[_steamID] != -1)
			{
				this.playerInventorySpot2Taken[_steamID] = 1;
			}
			else
			{
				this.playerInventorySpot2Taken[_steamID] = 0;
			}
			if (sync)
			{
				PunManager.instance.UpdateStat("playerInventorySpot2", itemName, spot);
			}
		}
		if (spot == 2)
		{
			this.playerInventorySpot3[_steamID] = value;
			if (this.playerInventorySpot3[_steamID] != -1)
			{
				this.playerInventorySpot3Taken[_steamID] = 1;
			}
			else
			{
				this.playerInventorySpot3Taken[_steamID] = 0;
			}
			if (sync)
			{
				PunManager.instance.UpdateStat("playerInventorySpot3", itemName, spot);
			}
		}
	}

	// Token: 0x06000C39 RID: 3129 RVA: 0x0006C890 File Offset: 0x0006AA90
	public void ItemFetchName(string itemName, ItemAttributes itemAttributes, int photonViewID)
	{
		string name = itemName;
		bool flag = false;
		foreach (string text in this.item.Keys)
		{
			if (text.Contains('/') && text.Split('/', StringSplitOptions.None)[0] == itemName && !this.takenItemNames.Contains(text))
			{
				name = text;
				this.takenItemNames.Add(text);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.ItemAdd(itemName, itemAttributes, photonViewID);
			return;
		}
		PunManager.instance.SetItemName(name, itemAttributes, photonViewID);
	}

	// Token: 0x06000C3A RID: 3130 RVA: 0x0006C93C File Offset: 0x0006AB3C
	public void StuffNeedingResetAtTheEndOfAScene()
	{
		this.takenItemNames.Clear();
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			this.playerInventorySpot1Taken[playerAvatar.steamID] = 0;
			this.playerInventorySpot2Taken[playerAvatar.steamID] = 0;
			this.playerInventorySpot3Taken[playerAvatar.steamID] = 0;
		}
	}

	// Token: 0x06000C3B RID: 3131 RVA: 0x0006C9D0 File Offset: 0x0006ABD0
	public int GetIndexThatHoldsThisItemFromItemDictionary(string itemName)
	{
		int num = 0;
		using (Dictionary<string, Item>.KeyCollection.Enumerator enumerator = this.itemDictionary.Keys.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == itemName)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x06000C3C RID: 3132 RVA: 0x0006CA38 File Offset: 0x0006AC38
	public string GetItemNameFromIndexInItemDictionary(int index)
	{
		if (index >= 0 && index < this.itemDictionary.Count)
		{
			return this.itemDictionary.Keys.ElementAt(index);
		}
		return null;
	}

	// Token: 0x06000C3D RID: 3133 RVA: 0x0006CA60 File Offset: 0x0006AC60
	public void ItemAdd(string itemName, ItemAttributes itemAttributes = null, int photonViewID = -1)
	{
		int num = 1;
		foreach (string text in this.item.Keys)
		{
			if (text.Contains('/'))
			{
				string[] array = text.Split('/', StringSplitOptions.None);
				if (array[0] == itemName)
				{
					int num2 = int.Parse(array[1]);
					if (num2 >= num)
					{
						num = num2 + 1;
					}
				}
			}
		}
		int indexThatHoldsThisItemFromItemDictionary = this.GetIndexThatHoldsThisItemFromItemDictionary(itemName);
		itemName = itemName + "/" + num.ToString();
		this.AddingItem(itemName, indexThatHoldsThisItemFromItemDictionary, photonViewID, itemAttributes);
	}

	// Token: 0x06000C3E RID: 3134 RVA: 0x0006CB10 File Offset: 0x0006AD10
	private void AddingItem(string itemName, int index, int photonViewID, ItemAttributes itemAttributes)
	{
		PunManager.instance.AddingItem(itemName, index, photonViewID, itemAttributes);
	}

	// Token: 0x06000C3F RID: 3135 RVA: 0x0006CB24 File Offset: 0x0006AD24
	public void ItemRemove(string instanceName)
	{
		string text = instanceName.Split('/', StringSplitOptions.None)[0];
		if (this.item.ContainsKey(instanceName))
		{
			Dictionary<string, int> dictionary = this.itemsPurchased;
			string key = text;
			dictionary[key]--;
			this.itemsPurchased[text] = Mathf.Max(0, this.itemsPurchased[text]);
		}
		else
		{
			Debug.LogError("Item " + text + " not found in item dictionary");
		}
		if (this.item.ContainsKey(instanceName))
		{
			this.item.Remove(instanceName);
			this.itemStatBattery.Remove(instanceName);
		}
	}

	// Token: 0x06000C40 RID: 3136 RVA: 0x0006CBC4 File Offset: 0x0006ADC4
	public void ItemPurchase(string itemName)
	{
		Dictionary<string, int> dictionary = this.itemsPurchased;
		dictionary[itemName]++;
		dictionary = this.itemsPurchasedTotal;
		dictionary[itemName]++;
		if (this.itemDictionary[itemName].physicalItem)
		{
			this.ItemAdd(itemName, null, -1);
		}
	}

	// Token: 0x06000C41 RID: 3137 RVA: 0x0006CC20 File Offset: 0x0006AE20
	private List<Dictionary<string, int>> AllDictionariesWithPrefix(string prefix)
	{
		List<Dictionary<string, int>> list = new List<Dictionary<string, int>>();
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.dictionaryOfDictionaries)
		{
			if (keyValuePair.Key.StartsWith(prefix) && keyValuePair.Value != null)
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	// Token: 0x06000C42 RID: 3138 RVA: 0x0006CC98 File Offset: 0x0006AE98
	public int GetBatteryLevel(string itemName)
	{
		return this.itemStatBattery[itemName];
	}

	// Token: 0x06000C43 RID: 3139 RVA: 0x0006CCA6 File Offset: 0x0006AEA6
	public void SetBatteryLevel(string itemName, int value)
	{
		this.itemStatBattery[itemName] = value;
	}

	// Token: 0x06000C44 RID: 3140 RVA: 0x0006CCB5 File Offset: 0x0006AEB5
	public int GetItemPurchased(Item _item)
	{
		return this.itemsPurchased[_item.itemAssetName];
	}

	// Token: 0x06000C45 RID: 3141 RVA: 0x0006CCC8 File Offset: 0x0006AEC8
	public void SetItemPurchase(Item _item, int value)
	{
		this.itemsPurchased[_item.itemAssetName] = value;
	}

	// Token: 0x06000C46 RID: 3142 RVA: 0x0006CCDC File Offset: 0x0006AEDC
	public int GetItemsUpgradesPurchased(string itemName)
	{
		return this.itemsUpgradesPurchased[itemName];
	}

	// Token: 0x06000C47 RID: 3143 RVA: 0x0006CCEA File Offset: 0x0006AEEA
	public int GetItemsUpgradesPurchasedTotal(string itemName)
	{
		return this.itemsPurchasedTotal[itemName];
	}

	// Token: 0x06000C48 RID: 3144 RVA: 0x0006CCF8 File Offset: 0x0006AEF8
	public void SetItemsUpgradesPurchased(string itemName, int value)
	{
		this.itemsUpgradesPurchased[itemName] = value;
	}

	// Token: 0x06000C49 RID: 3145 RVA: 0x0006CD08 File Offset: 0x0006AF08
	public void AddItemsUpgradesPurchased(string itemName)
	{
		Dictionary<string, int> dictionary = this.itemsUpgradesPurchased;
		int num = dictionary[itemName];
		dictionary[itemName] = num + 1;
	}

	// Token: 0x06000C4A RID: 3146 RVA: 0x0006CD30 File Offset: 0x0006AF30
	public void SetPlayerColor(string _steamID, int _colorIndex = -1)
	{
		if (_colorIndex != -1)
		{
			this.playerColor[_steamID] = _colorIndex;
			return;
		}
		if (this.playerColor[_steamID] == -1)
		{
			this.playerColor[_steamID] = this.playerColorIndex;
			this.playerColorIndex++;
		}
	}

	// Token: 0x06000C4B RID: 3147 RVA: 0x0006CD7E File Offset: 0x0006AF7E
	public int GetPlayerColor(string _steamID)
	{
		return this.playerColor[_steamID];
	}

	// Token: 0x06000C4C RID: 3148 RVA: 0x0006CD8C File Offset: 0x0006AF8C
	public void SetPlayerHealthStart(string _steamID, int health)
	{
		if (!this.playerHealth.ContainsKey(_steamID))
		{
			this.playerHealth[_steamID] = health;
		}
	}

	// Token: 0x06000C4D RID: 3149 RVA: 0x0006CDA9 File Offset: 0x0006AFA9
	public void SetPlayerHealth(string _steamID, int health, bool setInShop)
	{
		if (SemiFunc.RunIsShop() && !setInShop)
		{
			return;
		}
		this.playerHealth[_steamID] = health;
	}

	// Token: 0x06000C4E RID: 3150 RVA: 0x0006CDC3 File Offset: 0x0006AFC3
	public int GetPlayerHealth(string _steamID)
	{
		if (!this.playerHealth.ContainsKey(_steamID))
		{
			return 0;
		}
		return this.playerHealth[_steamID];
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x0006CDE1 File Offset: 0x0006AFE1
	public int GetPlayerMaxHealth(string _steamID)
	{
		if (!this.playerUpgradeHealth.ContainsKey(_steamID))
		{
			return 0;
		}
		return this.playerUpgradeHealth[_steamID] * 20;
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x0006CE02 File Offset: 0x0006B002
	public int GetRunStatCurrency()
	{
		return this.runStats["currency"];
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x0006CE14 File Offset: 0x0006B014
	public int GetRunStatLives()
	{
		return this.runStats["lives"];
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0006CE26 File Offset: 0x0006B026
	public int GetRunStatLevel()
	{
		return this.runStats["level"];
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x0006CE38 File Offset: 0x0006B038
	public int GetRunStatSaveLevel()
	{
		int result = 0;
		if (this.runStats.ContainsKey("save level"))
		{
			result = this.runStats["save level"];
		}
		return result;
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x0006CE6B File Offset: 0x0006B06B
	public int GetRunStatTotalHaul()
	{
		return this.runStats["totalHaul"];
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x0006CE80 File Offset: 0x0006B080
	private void DebugSync()
	{
		int num = 0;
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.dictionaryOfDictionaries)
		{
			foreach (string key in new List<string>(keyValuePair.Value.Keys))
			{
				keyValuePair.Value[key] = 1;
				num++;
			}
		}
		SemiFunc.StatSyncAll();
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x0006CF30 File Offset: 0x0006B130
	public void ResetAllStats()
	{
		this.saveFileReady = false;
		ItemManager.instance.ResetAllItems();
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.dictionaryOfDictionaries)
		{
			keyValuePair.Value.Clear();
		}
		this.takenItemNames.Clear();
		this.runStats.Clear();
		this.timePlayed = 0f;
		this.RunStartStats();
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x0006CFC0 File Offset: 0x0006B1C0
	private void LoadItemsFromFolder()
	{
		Item[] array = Resources.LoadAll<Item>(this.folderPath);
		int i = 0;
		while (i < array.Length)
		{
			Item item = array[i];
			if (!string.IsNullOrEmpty(item.itemAssetName))
			{
				if (!this.itemDictionary.ContainsKey(item.itemAssetName))
				{
					this.itemDictionary.Add(item.itemAssetName, item);
				}
				using (List<Dictionary<string, int>>.Enumerator enumerator = this.AllDictionariesWithPrefix("item").GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Dictionary<string, int> dictionary = enumerator.Current;
						dictionary.Add(item.itemAssetName, 0);
					}
					goto IL_92;
				}
				goto IL_88;
			}
			goto IL_88;
			IL_92:
			i++;
			continue;
			IL_88:
			Debug.LogWarning("Item with empty or null itemName found and will be skipped.");
			goto IL_92;
		}
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x0006D07C File Offset: 0x0006B27C
	public void EmptyAllBatteries()
	{
		foreach (string key in new List<string>(this.itemStatBattery.Keys))
		{
			this.itemStatBattery[key] = 0;
		}
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x0006D0E0 File Offset: 0x0006B2E0
	public void BuyAllItems()
	{
		foreach (string itemName in new List<string>(this.itemDictionary.Keys))
		{
			this.ItemPurchase(itemName);
		}
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x0006D140 File Offset: 0x0006B340
	private void ManualEntry()
	{
		List<KeyValuePair<string, Item>> list = new List<KeyValuePair<string, Item>>();
		foreach (KeyValuePair<string, Item> keyValuePair in this.itemDictionary)
		{
			string itemAssetName = keyValuePair.Value.itemAssetName;
			if (!string.IsNullOrEmpty(itemAssetName))
			{
				list.Add(new KeyValuePair<string, Item>(itemAssetName, keyValuePair.Value));
			}
			else
			{
				Debug.LogWarning("Item with empty or null name found and will be skipped.");
			}
		}
		this.itemDictionary.Clear();
		foreach (KeyValuePair<string, Item> keyValuePair2 in list)
		{
			if (!this.itemDictionary.ContainsKey(keyValuePair2.Key))
			{
				this.itemDictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
			}
			else
			{
				Debug.LogWarning("Duplicate key found: " + keyValuePair2.Key + ". This entry will be skipped.");
			}
		}
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x0006D254 File Offset: 0x0006B454
	public List<string> SaveFileGetAll()
	{
		List<string> list = new List<string>();
		string text = Application.persistentDataPath + "/saves";
		if (Directory.Exists(text))
		{
			using (var enumerator = (from dir in Directory.GetDirectories(text)
			select new
			{
				Path = dir,
				CreationTime = Directory.GetCreationTime(dir)
			} into x
			orderby x.CreationTime descending
			select x).ToList().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					var <>f__AnonymousType = enumerator.Current;
					string fileName = Path.GetFileName(<>f__AnonymousType.Path);
					list.Add(fileName);
				}
				return list;
			}
		}
		Debug.LogWarning("Saves directory not found at " + text);
		return list;
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x0006D330 File Offset: 0x0006B530
	public void SaveFileDelete(string saveFileName)
	{
		string text = Application.persistentDataPath + "/saves/" + saveFileName;
		if (Directory.Exists(text))
		{
			Directory.Delete(text, true);
			Debug.Log("Deleted save file and all backups for '" + saveFileName + "'");
			return;
		}
		Debug.LogWarning("Save folder not found: " + text);
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x0006D384 File Offset: 0x0006B584
	public void SaveGame(string fileName)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		this.dateAndTime = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
		string text = Application.persistentDataPath + "/saves";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = text + "/" + fileName;
		if (!Directory.Exists(text2))
		{
			Directory.CreateDirectory(text2);
		}
		string text3 = text2 + "/" + fileName + ".es3";
		if (File.Exists(text3))
		{
			int num = 1;
			string text4;
			do
			{
				text4 = string.Concat(new string[]
				{
					text2,
					"/",
					fileName,
					"_BACKUP",
					num.ToString(),
					".es3"
				});
				num++;
			}
			while (File.Exists(text4));
			File.Move(text3, text4);
		}
		ES3Settings settings = new ES3Settings(text3, ES3.EncryptionType.AES, this.totallyNormalString, null);
		Dictionary<string, Dictionary<string, int>> dictionary = new Dictionary<string, Dictionary<string, int>>();
		foreach (string key in this.doNotSaveTheseDictionaries)
		{
			if (this.dictionaryOfDictionaries.ContainsKey(key))
			{
				dictionary[key] = this.dictionaryOfDictionaries[key];
				this.dictionaryOfDictionaries.Remove(key);
			}
		}
		ES3.Save<string>("teamName", this.teamName, settings);
		ES3.Save<string>("dateAndTime", this.dateAndTime, settings);
		ES3.Save<float>("timePlayed", this.timePlayed, settings);
		ES3.Save<Dictionary<string, string>>("playerNames", this.playerNames, settings);
		ES3.Save<Dictionary<string, Dictionary<string, int>>>("dictionaryOfDictionaries", this.dictionaryOfDictionaries, settings);
		foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in dictionary)
		{
			this.dictionaryOfDictionaries[keyValuePair.Key] = keyValuePair.Value;
		}
		this.PlayersAddAll();
		this.saveFileReady = true;
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x0006D59C File Offset: 0x0006B79C
	private void PlayersAddAll()
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			this.PlayerAdd(playerAvatar.steamID, playerAvatar.playerName);
			this.SetPlayerColor(playerAvatar.steamID, -1);
		}
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x0006D614 File Offset: 0x0006B814
	public void LoadGame(string fileName)
	{
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			return;
		}
		string text = string.Concat(new string[]
		{
			Application.persistentDataPath,
			"/saves/",
			fileName,
			"/",
			fileName,
			".es3"
		});
		if (File.Exists(text))
		{
			this.saveFileCurrent = fileName;
			ES3Settings settings = new ES3Settings(text, ES3.EncryptionType.AES, this.totallyNormalString, null);
			this.teamName = ES3.Load<string>("teamName", settings);
			this.dateAndTime = ES3.Load<string>("dateAndTime", settings);
			this.timePlayed = ES3.Load<float>("timePlayed", settings);
			this.playerNames = ES3.Load<Dictionary<string, string>>("playerNames", settings);
			using (Dictionary<string, Dictionary<string, int>>.Enumerator enumerator = ES3.Load<Dictionary<string, Dictionary<string, int>>>("dictionaryOfDictionaries", settings).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, Dictionary<string, int>> keyValuePair = enumerator.Current;
					Dictionary<string, int> dictionary;
					if (this.dictionaryOfDictionaries.TryGetValue(keyValuePair.Key, out dictionary))
					{
						using (Dictionary<string, int>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								KeyValuePair<string, int> keyValuePair2 = enumerator2.Current;
								dictionary[keyValuePair2.Key] = keyValuePair2.Value;
							}
							continue;
						}
					}
					this.dictionaryOfDictionaries.Add(keyValuePair.Key, new Dictionary<string, int>(keyValuePair.Value));
				}
				goto IL_166;
			}
		}
		Debug.LogWarning("Save file not found in " + text);
		IL_166:
		RunManager.instance.levelsCompleted = this.GetRunStatLevel();
		RunManager.instance.runLives = this.GetRunStatLives();
		RunManager.instance.loadLevel = this.GetRunStatSaveLevel();
		this.PlayersAddAll();
		this.saveFileReady = true;
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x0006D7E0 File Offset: 0x0006B9E0
	public void UpdateCrown(string steamID)
	{
		foreach (string key in new List<string>(this.playerHasCrown.Keys))
		{
			this.playerHasCrown[key] = 0;
		}
		if (this.playerHasCrown.ContainsKey(steamID))
		{
			this.playerHasCrown[steamID] = 1;
		}
	}

	// Token: 0x04001390 RID: 5008
	public static StatsManager instance;

	// Token: 0x04001391 RID: 5009
	public string folderPath = "ScriptableObjects";

	// Token: 0x04001392 RID: 5010
	internal string dateAndTime;

	// Token: 0x04001393 RID: 5011
	internal string teamName = "R.E.P.O.";

	// Token: 0x04001394 RID: 5012
	private string totallyNormalString = "Why would you want to cheat?... :o It's no fun. :') :'D";

	// Token: 0x04001395 RID: 5013
	public Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

	// Token: 0x04001396 RID: 5014
	public Dictionary<string, int> runStats = new Dictionary<string, int>();

	// Token: 0x04001397 RID: 5015
	public Dictionary<string, int> itemsPurchased = new Dictionary<string, int>();

	// Token: 0x04001398 RID: 5016
	public Dictionary<string, int> itemsUpgradesPurchased = new Dictionary<string, int>();

	// Token: 0x04001399 RID: 5017
	public Dictionary<string, int> itemBatteryUpgrades = new Dictionary<string, int>();

	// Token: 0x0400139A RID: 5018
	public Dictionary<string, int> itemsPurchasedTotal = new Dictionary<string, int>();

	// Token: 0x0400139B RID: 5019
	public Dictionary<string, int> playerHealth = new Dictionary<string, int>();

	// Token: 0x0400139C RID: 5020
	public Dictionary<string, int> playerUpgradeHealth = new Dictionary<string, int>();

	// Token: 0x0400139D RID: 5021
	public Dictionary<string, int> playerUpgradeStamina = new Dictionary<string, int>();

	// Token: 0x0400139E RID: 5022
	public Dictionary<string, int> playerUpgradeExtraJump = new Dictionary<string, int>();

	// Token: 0x0400139F RID: 5023
	public Dictionary<string, int> playerUpgradeLaunch = new Dictionary<string, int>();

	// Token: 0x040013A0 RID: 5024
	public Dictionary<string, int> playerUpgradeMapPlayerCount = new Dictionary<string, int>();

	// Token: 0x040013A1 RID: 5025
	internal Dictionary<string, int> playerColor = new Dictionary<string, int>();

	// Token: 0x040013A2 RID: 5026
	private int playerColorIndex;

	// Token: 0x040013A3 RID: 5027
	public Dictionary<string, int> playerUpgradeSpeed = new Dictionary<string, int>();

	// Token: 0x040013A4 RID: 5028
	public Dictionary<string, int> playerUpgradeStrength = new Dictionary<string, int>();

	// Token: 0x040013A5 RID: 5029
	public Dictionary<string, int> playerUpgradeThrow = new Dictionary<string, int>();

	// Token: 0x040013A6 RID: 5030
	public Dictionary<string, int> playerUpgradeRange = new Dictionary<string, int>();

	// Token: 0x040013A7 RID: 5031
	public Dictionary<string, int> playerInventorySpot1 = new Dictionary<string, int>();

	// Token: 0x040013A8 RID: 5032
	public Dictionary<string, int> playerInventorySpot2 = new Dictionary<string, int>();

	// Token: 0x040013A9 RID: 5033
	public Dictionary<string, int> playerInventorySpot3 = new Dictionary<string, int>();

	// Token: 0x040013AA RID: 5034
	public Dictionary<string, int> playerInventorySpot1Taken = new Dictionary<string, int>();

	// Token: 0x040013AB RID: 5035
	public Dictionary<string, int> playerInventorySpot2Taken = new Dictionary<string, int>();

	// Token: 0x040013AC RID: 5036
	public Dictionary<string, int> playerInventorySpot3Taken = new Dictionary<string, int>();

	// Token: 0x040013AD RID: 5037
	public Dictionary<string, int> playerHasCrown = new Dictionary<string, int>();

	// Token: 0x040013AE RID: 5038
	public Dictionary<string, int> item = new Dictionary<string, int>();

	// Token: 0x040013AF RID: 5039
	public Dictionary<string, int> itemStatBattery = new Dictionary<string, int>();

	// Token: 0x040013B0 RID: 5040
	[HideInInspector]
	public float chargingStationCharge = 1f;

	// Token: 0x040013B1 RID: 5041
	public Dictionary<string, Dictionary<string, int>> dictionaryOfDictionaries = new Dictionary<string, Dictionary<string, int>>();

	// Token: 0x040013B2 RID: 5042
	[HideInInspector]
	public bool statsSynced;

	// Token: 0x040013B3 RID: 5043
	internal List<string> takenItemNames = new List<string>();

	// Token: 0x040013B4 RID: 5044
	internal float timePlayed;

	// Token: 0x040013B5 RID: 5045
	internal Dictionary<string, string> playerNames = new Dictionary<string, string>();

	// Token: 0x040013B6 RID: 5046
	internal string saveFileCurrent;

	// Token: 0x040013B7 RID: 5047
	internal bool saveFileReady;

	// Token: 0x040013B8 RID: 5048
	internal List<string> doNotSaveTheseDictionaries = new List<string>();

	// Token: 0x0200034D RID: 845
	[Serializable]
	public class SerializableDictionary
	{
		// Token: 0x040026FB RID: 9979
		public List<string> keys = new List<string>();

		// Token: 0x040026FC RID: 9980
		public List<StatsManager.SerializableInnerDictionary> values = new List<StatsManager.SerializableInnerDictionary>();
	}

	// Token: 0x0200034E RID: 846
	[Serializable]
	public class SerializableInnerDictionary
	{
		// Token: 0x040026FD RID: 9981
		public List<string> keys = new List<string>();

		// Token: 0x040026FE RID: 9982
		public List<int> values = new List<int>();
	}
}
