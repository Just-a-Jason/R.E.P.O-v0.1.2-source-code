using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200008B RID: 139
public class EnemyDirector : MonoBehaviour
{
	// Token: 0x0600058B RID: 1419 RVA: 0x00036966 File Offset: 0x00034B66
	private void Awake()
	{
		EnemyDirector.instance = this;
		this.despawnedDecreaseTimer = 60f * this.despawnedDecreaseMinutes;
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00036980 File Offset: 0x00034B80
	private void Start()
	{
		this.spawnIdlePauseTimer = 60f * Random.Range(2f, 3f) * this.spawnIdlePauseCurve.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		if (Random.Range(0, 100) < 20)
		{
			this.spawnIdlePauseTimer *= Random.Range(0.1f, 0.25f);
		}
		this.spawnIdlePauseTimer = Mathf.Max(this.spawnIdlePauseTimer, 1f);
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x000369F8 File Offset: 0x00034BF8
	private void Update()
	{
		if (LevelGenerator.Instance.Generated && this.spawnIdlePauseTimer > 0f)
		{
			this.spawnIdlePauseTimer -= Time.deltaTime;
			if (this.spawnIdlePauseTimer <= 0f)
			{
				using (List<EnemyParent>.Enumerator enumerator = this.enemiesSpawned.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.firstSpawnPointUsed)
						{
							this.spawnIdlePauseTimer = 2f;
						}
					}
				}
			}
			if (this.debugNoSpawnIdlePause)
			{
				this.spawnIdlePauseTimer = 0f;
			}
		}
		this.despawnedDecreaseTimer -= Time.deltaTime;
		if (this.despawnedDecreaseTimer <= 0f)
		{
			this.despawnedTimeMultiplier -= this.despawnedDecreasePercent;
			if (this.despawnedTimeMultiplier < 0f)
			{
				this.despawnedTimeMultiplier = 0f;
			}
			this.despawnedDecreaseTimer = 60f * this.despawnedDecreaseMinutes;
		}
		if (RoundDirector.instance.allExtractionPointsCompleted)
		{
			foreach (EnemyParent enemyParent in this.enemiesSpawned)
			{
				if (enemyParent.DespawnedTimer > 30f)
				{
					enemyParent.DespawnedTimerSet(0f, false);
				}
			}
			if (this.investigatePointTimer <= 0f)
			{
				if (this.extractionsDoneState == EnemyDirector.ExtractionsDoneState.StartRoom)
				{
					this.enemyActionAmount = 0f;
					this.despawnedTimeMultiplier = 0f;
					if (this.extractionDoneStateImpulse)
					{
						this.extractionDoneStateTimer = 10f;
						this.extractionDoneStateImpulse = false;
						foreach (EnemyParent enemyParent2 in this.enemiesSpawned)
						{
							if (enemyParent2.Spawned)
							{
								bool flag = false;
								foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
								{
									if (!playerAvatar.isDisabled && Vector3.Distance(enemyParent2.Enemy.transform.position, playerAvatar.transform.position) < 25f)
									{
										flag = true;
										break;
									}
								}
								if (!flag)
								{
									enemyParent2.SpawnedTimerPause(0f);
									enemyParent2.SpawnedTimerSet(0f);
								}
							}
						}
					}
					this.investigatePointTimer = this.investigatePointTime;
					List<LevelPoint> list = SemiFunc.LevelPointsGetInStartRoom();
					if (list.Count > 0)
					{
						SemiFunc.EnemyInvestigate(list[Random.Range(0, list.Count)].transform.position, 100f);
					}
					this.extractionDoneStateTimer -= this.investigatePointTime;
					if (this.extractionDoneStateTimer <= 0f)
					{
						this.extractionsDoneState = EnemyDirector.ExtractionsDoneState.PlayerRoom;
					}
				}
				else
				{
					List<LevelPoint> list2 = SemiFunc.LevelPointsGetInPlayerRooms();
					if (list2.Count > 0)
					{
						SemiFunc.EnemyInvestigate(list2[Random.Range(0, list2.Count)].transform.position, 100f);
					}
					this.investigatePointTimer = this.investigatePointTime;
					this.investigatePointTime = Mathf.Min(this.investigatePointTime + 2f, 30f);
				}
			}
			else
			{
				this.investigatePointTimer -= Time.deltaTime;
			}
		}
		float num = 0f;
		foreach (EnemyParent enemyParent3 in this.enemiesSpawned)
		{
			if (enemyParent3.Spawned && enemyParent3.playerClose && !enemyParent3.forceLeave)
			{
				bool flag2 = false;
				foreach (PlayerAvatar playerAvatar2 in SemiFunc.PlayerGetList())
				{
					foreach (RoomVolume x in playerAvatar2.RoomVolumeCheck.CurrentRooms)
					{
						foreach (RoomVolume y in enemyParent3.currentRooms)
						{
							if (x == y)
							{
								flag2 = true;
								break;
							}
						}
					}
				}
				if (flag2)
				{
					float num2 = 0f;
					if (enemyParent3.difficulty == EnemyParent.Difficulty.Difficulty3)
					{
						num2 += 2f;
					}
					else if (enemyParent3.difficulty == EnemyParent.Difficulty.Difficulty2)
					{
						num2 += 1f;
					}
					else
					{
						num2 += 0.5f;
					}
					num += num2 * enemyParent3.actionMultiplier;
				}
			}
		}
		if (num > 0f)
		{
			this.enemyActionAmount += num * Time.deltaTime;
		}
		else
		{
			this.enemyActionAmount -= 0.1f * Time.deltaTime;
			this.enemyActionAmount = Mathf.Max(0f, this.enemyActionAmount);
		}
		float num3 = 120f;
		if (this.debugShortActionTimer)
		{
			num3 = 5f;
		}
		if (this.enemyActionAmount > num3)
		{
			this.enemyActionAmount = 0f;
			LevelPoint levelPoint = SemiFunc.LevelPointGetFurthestFromPlayer(base.transform.position, 5f);
			if (levelPoint)
			{
				this.SetInvestigate(levelPoint.transform.position, float.MaxValue);
			}
			if (RoundDirector.instance.allExtractionPointsCompleted && this.extractionsDoneState == EnemyDirector.ExtractionsDoneState.PlayerRoom)
			{
				this.investigatePointTimer = 60f;
			}
			foreach (EnemyParent enemyParent4 in this.enemiesSpawned)
			{
				if (enemyParent4.Spawned)
				{
					enemyParent4.forceLeave = true;
				}
			}
		}
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x00037080 File Offset: 0x00035280
	public void AmountSetup()
	{
		this.amountCurve3Value = (int)this.amountCurve3.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		this.amountCurve2Value = (int)this.amountCurve2.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		this.amountCurve1Value = (int)this.amountCurve1.Evaluate(SemiFunc.RunGetDifficultyMultiplier());
		for (int i = 0; i < this.amountCurve3Value; i++)
		{
			this.PickEnemies(this.enemiesDifficulty3);
		}
		for (int j = 0; j < this.amountCurve2Value; j++)
		{
			this.PickEnemies(this.enemiesDifficulty2);
		}
		for (int k = 0; k < this.amountCurve1Value; k++)
		{
			this.PickEnemies(this.enemiesDifficulty1);
		}
		this.totalAmount = this.amountCurve1Value + this.amountCurve2Value + this.amountCurve3Value;
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00037144 File Offset: 0x00035344
	private void PickEnemies(List<EnemySetup> _enemiesList)
	{
		int num = DataDirector.instance.SettingValueFetch(DataDirector.Setting.RunsPlayed);
		_enemiesList.Shuffle<EnemySetup>();
		EnemySetup item = null;
		float num2 = -1f;
		foreach (EnemySetup enemySetup in _enemiesList)
		{
			if ((!enemySetup.levelsCompletedCondition || (RunManager.instance.levelsCompleted >= enemySetup.levelsCompletedMin && RunManager.instance.levelsCompleted <= enemySetup.levelsCompletedMax)) && num >= enemySetup.runsPlayed)
			{
				int num3 = 0;
				using (List<EnemySetup>.Enumerator enumerator2 = RunManager.instance.enemiesSpawned.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current == enemySetup)
						{
							num3++;
						}
					}
				}
				float num4 = 100f;
				if (enemySetup.rarityPreset)
				{
					num4 = enemySetup.rarityPreset.chance;
				}
				float maxInclusive = Mathf.Max(0f, num4 - 30f * (float)num3);
				float num5 = Random.Range(0f, maxInclusive);
				if (num5 > num2)
				{
					item = enemySetup;
					num2 = num5;
				}
			}
		}
		this.enemyList.Add(item);
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x000372A0 File Offset: 0x000354A0
	public EnemySetup GetEnemy()
	{
		EnemySetup enemySetup = this.enemyList[this.enemyListIndex];
		this.enemyListIndex++;
		int num = 0;
		using (List<EnemySetup>.Enumerator enumerator = RunManager.instance.enemiesSpawned.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == enemySetup)
				{
					num++;
				}
			}
		}
		int num2 = 4;
		while (num < 8 && num2 > 0)
		{
			RunManager.instance.enemiesSpawned.Add(enemySetup);
			num++;
			num2--;
		}
		return enemySetup;
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x00037344 File Offset: 0x00035544
	public void FirstSpawnPointAdd(EnemyParent _enemyParent)
	{
		List<LevelPoint> list = SemiFunc.LevelPointsGetAll();
		float num = 0f;
		LevelPoint levelPoint = null;
		foreach (LevelPoint levelPoint2 in list)
		{
			float num2 = Vector3.Distance(levelPoint2.transform.position, LevelGenerator.Instance.LevelPathTruck.transform.position);
			using (List<LevelPoint>.Enumerator enumerator2 = this.enemyFirstSpawnPoints.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == levelPoint2)
					{
						num2 = 0f;
						break;
					}
				}
			}
			if (num2 > num)
			{
				num = num2;
				levelPoint = levelPoint2;
			}
		}
		if (levelPoint)
		{
			_enemyParent.firstSpawnPoint = levelPoint;
			this.enemyFirstSpawnPoints.Add(levelPoint);
		}
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x00037434 File Offset: 0x00035634
	public void DebugResult()
	{
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x00037438 File Offset: 0x00035638
	public void SetInvestigate(Vector3 position, float radius)
	{
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			if (this.debugInvestigate)
			{
				Object.Instantiate<GameObject>(AssetManager.instance.debugEnemyInvestigate, position, Quaternion.identity).GetComponent<DebugEnemyInvestigate>().radius = radius;
			}
			foreach (EnemyParent enemyParent in this.enemiesSpawned)
			{
				if (!enemyParent.Spawned)
				{
					if (radius >= 15f)
					{
						enemyParent.DisableDecrease(5f);
					}
				}
				else if (enemyParent.Enemy.HasStateInvestigate && Vector3.Distance(position, enemyParent.Enemy.transform.position) / enemyParent.Enemy.StateInvestigate.rangeMultiplier < radius)
				{
					enemyParent.Enemy.StateInvestigate.Set(position);
				}
			}
		}
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x0003751C File Offset: 0x0003571C
	public void AddEnemyValuable(EnemyValuable _newValuable)
	{
		List<EnemyValuable> list = new List<EnemyValuable>();
		foreach (EnemyValuable enemyValuable in this.enemyValuables)
		{
			if (!enemyValuable)
			{
				list.Add(enemyValuable);
			}
		}
		foreach (EnemyValuable item in list)
		{
			this.enemyValuables.Remove(item);
		}
		this.enemyValuables.Add(_newValuable);
		if (this.enemyValuables.Count > 10)
		{
			EnemyValuable enemyValuable2 = this.enemyValuables[0];
			this.enemyValuables.RemoveAt(0);
			enemyValuable2.Destroy();
		}
	}

	// Token: 0x040008E6 RID: 2278
	private EnemyDirector.ExtractionsDoneState extractionsDoneState;

	// Token: 0x040008E7 RID: 2279
	private float extractionDoneStateTimer;

	// Token: 0x040008E8 RID: 2280
	private bool extractionDoneStateImpulse = true;

	// Token: 0x040008E9 RID: 2281
	public static EnemyDirector instance;

	// Token: 0x040008EA RID: 2282
	internal bool debugNoVision;

	// Token: 0x040008EB RID: 2283
	internal EnemySetup[] debugEnemy;

	// Token: 0x040008EC RID: 2284
	internal float debugEnemyEnableTime;

	// Token: 0x040008ED RID: 2285
	internal float debugEnemyDisableTime;

	// Token: 0x040008EE RID: 2286
	internal bool debugEasyGrab;

	// Token: 0x040008EF RID: 2287
	internal bool debugSpawnClose;

	// Token: 0x040008F0 RID: 2288
	internal bool debugDespawnClose;

	// Token: 0x040008F1 RID: 2289
	internal bool debugInvestigate;

	// Token: 0x040008F2 RID: 2290
	internal bool debugShortActionTimer;

	// Token: 0x040008F3 RID: 2291
	internal bool debugNoSpawnedPause;

	// Token: 0x040008F4 RID: 2292
	internal bool debugNoSpawnIdlePause;

	// Token: 0x040008F5 RID: 2293
	internal bool debugNoGrabMaxTime;

	// Token: 0x040008F6 RID: 2294
	public List<EnemySetup> enemiesDifficulty1;

	// Token: 0x040008F7 RID: 2295
	public List<EnemySetup> enemiesDifficulty2;

	// Token: 0x040008F8 RID: 2296
	public List<EnemySetup> enemiesDifficulty3;

	// Token: 0x040008F9 RID: 2297
	[Space]
	public AnimationCurve spawnIdlePauseCurve;

	// Token: 0x040008FA RID: 2298
	[Space]
	public AnimationCurve amountCurve1;

	// Token: 0x040008FB RID: 2299
	private int amountCurve1Value;

	// Token: 0x040008FC RID: 2300
	public AnimationCurve amountCurve2;

	// Token: 0x040008FD RID: 2301
	private int amountCurve2Value;

	// Token: 0x040008FE RID: 2302
	public AnimationCurve amountCurve3;

	// Token: 0x040008FF RID: 2303
	private int amountCurve3Value;

	// Token: 0x04000900 RID: 2304
	internal int totalAmount;

	// Token: 0x04000901 RID: 2305
	private List<EnemySetup> enemyList = new List<EnemySetup>();

	// Token: 0x04000902 RID: 2306
	private int enemyListIndex;

	// Token: 0x04000903 RID: 2307
	[Space]
	public float despawnedDecreaseMinutes;

	// Token: 0x04000904 RID: 2308
	public float despawnedDecreasePercent;

	// Token: 0x04000905 RID: 2309
	internal float despawnedTimeMultiplier = 1f;

	// Token: 0x04000906 RID: 2310
	private float despawnedDecreaseTimer;

	// Token: 0x04000907 RID: 2311
	private float investigatePointTimer;

	// Token: 0x04000908 RID: 2312
	private float investigatePointTime = 3f;

	// Token: 0x04000909 RID: 2313
	private float enemyActionAmount;

	// Token: 0x0400090A RID: 2314
	internal float spawnIdlePauseTimer;

	// Token: 0x0400090B RID: 2315
	[Space]
	public List<EnemyParent> enemiesSpawned;

	// Token: 0x0400090C RID: 2316
	internal List<EnemyValuable> enemyValuables = new List<EnemyValuable>();

	// Token: 0x0400090D RID: 2317
	internal List<LevelPoint> enemyFirstSpawnPoints = new List<LevelPoint>();

	// Token: 0x020002E6 RID: 742
	public enum ExtractionsDoneState
	{
		// Token: 0x040024ED RID: 9453
		StartRoom,
		// Token: 0x040024EE RID: 9454
		PlayerRoom
	}
}
