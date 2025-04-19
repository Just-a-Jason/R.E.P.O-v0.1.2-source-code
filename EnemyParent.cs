using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200008C RID: 140
[RequireComponent(typeof(PhotonView))]
public class EnemyParent : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06000596 RID: 1430 RVA: 0x0003764C File Offset: 0x0003584C
	private void Awake()
	{
		base.transform.parent = LevelGenerator.Instance.EnemyParent.transform;
		this.Enemy = base.GetComponentInChildren<Enemy>();
		if (EnemyDirector.instance.debugEnemy != null)
		{
			if (EnemyDirector.instance.debugEnemyEnableTime > 0f)
			{
				this.SpawnedTimeMax = EnemyDirector.instance.debugEnemyEnableTime;
				this.SpawnedTimeMin = this.SpawnedTimeMax;
			}
			if (EnemyDirector.instance.debugEnemyDisableTime > 0f)
			{
				this.DespawnedTimeMax = EnemyDirector.instance.debugEnemyDisableTime;
				this.DespawnedTimeMin = this.DespawnedTimeMax;
			}
		}
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x000376F2 File Offset: 0x000358F2
	private void Update()
	{
		if (SemiFunc.FPSImpulse1())
		{
			this.GetRoomVolume();
		}
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x00037701 File Offset: 0x00035901
	private IEnumerator Setup()
	{
		while (!this.SetupDone)
		{
			yield return new WaitForSeconds(0.1f);
		}
		LevelGenerator.Instance.EnemiesSpawned++;
		EnemyDirector.instance.enemiesSpawned.Add(this);
		if (LevelGenerator.Instance.EnemiesSpawned >= LevelGenerator.Instance.EnemiesSpawnTarget)
		{
			foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
			{
				foreach (EnemyParent enemyParent in EnemyDirector.instance.enemiesSpawned)
				{
					enemyParent.Enemy.PlayerAdded(playerAvatar.photonView.ViewID);
				}
			}
			if (GameManager.Multiplayer())
			{
				LevelGenerator.Instance.PhotonView.RPC("EnemyReadyRPC", RpcTarget.All, Array.Empty<object>());
			}
		}
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			yield break;
		}
		if (this.Enemy.HasRigidbody)
		{
			float y = this.Enemy.Rigidbody.transform.localPosition.y - this.Enemy.transform.localPosition.y;
			Vector3 b = new Vector3(0f, y, 0f);
			Vector3 position = this.Enemy.transform.position + b;
			this.Enemy.Rigidbody.rb.position = position;
			this.Enemy.Rigidbody.rb.rotation = this.Enemy.Rigidbody.followTarget.rotation;
			this.Enemy.Rigidbody.physGrabObject.Teleport(position, this.Enemy.Rigidbody.followTarget.rotation);
			this.Enemy.Rigidbody.physGrabObject.spawned = true;
			this.Enemy.Rigidbody.rb.isKinematic = false;
		}
		base.StartCoroutine(this.Logic());
		base.StartCoroutine(this.PlayerCloseLogic());
		yield break;
	}

	// Token: 0x06000599 RID: 1433 RVA: 0x00037710 File Offset: 0x00035910
	private IEnumerator Logic()
	{
		this.Despawn();
		this.DespawnedTimer = Random.Range(2f, 5f);
		for (;;)
		{
			if (this.Spawned)
			{
				if (this.SpawnedTimer <= 0f)
				{
					if (!this.playerClose || EnemyDirector.instance.debugDespawnClose)
					{
						this.Enemy.CurrentState = EnemyState.Despawn;
					}
				}
				else if (this.spawnedTimerPauseTimer > 0f)
				{
					this.spawnedTimerPauseTimer -= Time.deltaTime;
				}
				else if (!this.playerClose || EnemyDirector.instance.debugDespawnClose)
				{
					this.SpawnedTimer -= Time.deltaTime;
				}
			}
			else if (this.DespawnedTimer <= 0f)
			{
				this.Spawn();
			}
			else
			{
				this.DespawnedTimer -= Time.deltaTime;
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600059A RID: 1434 RVA: 0x0003771F File Offset: 0x0003591F
	private IEnumerator PlayerCloseLogic()
	{
		for (;;)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (PlayerAvatar playerAvatar in SemiFunc.PlayerGetList())
			{
				if (!playerAvatar.isDisabled)
				{
					Vector3 a = new Vector3(playerAvatar.transform.position.x, 0f, playerAvatar.transform.position.z);
					Vector3 b = new Vector3(this.Enemy.transform.position.x, 0f, this.Enemy.transform.position.z);
					float num = Vector3.Distance(a, b);
					if (num <= 6f)
					{
						flag2 = true;
						flag = true;
						break;
					}
					if (num <= 20f)
					{
						EnemyDirector.instance.spawnIdlePauseTimer = 0f;
						flag = true;
					}
				}
			}
			this.playerClose = flag;
			this.playerVeryClose = flag2;
			if (flag)
			{
				this.valuableSpawnTimer = 10f;
			}
			else if (this.valuableSpawnTimer > 0f)
			{
				this.valuableSpawnTimer -= 1f;
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x0003772E File Offset: 0x0003592E
	public void DisableDecrease(float _time)
	{
		this.DespawnedTimer -= _time;
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x0003773E File Offset: 0x0003593E
	public void SpawnedTimerSet(float _time)
	{
		if (this.Spawned)
		{
			this.SpawnedTimer = _time;
			if (_time == 0f)
			{
				this.Enemy.CurrentState = EnemyState.Despawn;
			}
		}
	}

	// Token: 0x0600059D RID: 1437 RVA: 0x00037764 File Offset: 0x00035964
	public void DespawnedTimerSet(float _time, bool _min = false)
	{
		if (!this.Spawned)
		{
			if (!_min)
			{
				this.DespawnedTimer = _time;
				return;
			}
			this.DespawnedTimer = Mathf.Min(this.DespawnedTimer, _time);
		}
	}

	// Token: 0x0600059E RID: 1438 RVA: 0x0003778B File Offset: 0x0003598B
	public void SpawnedTimerReset()
	{
		if (this.Spawned)
		{
			this.SpawnedTimer = Random.Range(this.SpawnedTimeMin, this.SpawnedTimeMax);
			if (this.Enemy.CurrentState == EnemyState.Despawn)
			{
				this.Enemy.CurrentState = EnemyState.Roaming;
			}
		}
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x000377C7 File Offset: 0x000359C7
	public void SpawnedTimerPause(float _time)
	{
		this.spawnedTimerPauseTimer = Mathf.Max(this.spawnedTimerPauseTimer, _time);
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x000377DC File Offset: 0x000359DC
	public void GetRoomVolume()
	{
		this.currentRooms.Clear();
		foreach (Collider collider in Physics.OverlapBox(this.Enemy.CenterTransform.position, Vector3.one / 2f, base.transform.rotation, LayerMask.GetMask(new string[]
		{
			"RoomVolume"
		})))
		{
			RoomVolume roomVolume = collider.transform.GetComponent<RoomVolume>();
			if (!roomVolume)
			{
				roomVolume = collider.transform.GetComponentInParent<RoomVolume>();
			}
			if (!this.currentRooms.Contains(roomVolume))
			{
				this.currentRooms.Add(roomVolume);
			}
		}
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x00037884 File Offset: 0x00035A84
	private void Spawn()
	{
		this.SpawnedTimer = Random.Range(this.SpawnedTimeMin, this.SpawnedTimeMax);
		this.Enemy.CurrentState = EnemyState.Spawn;
		if (GameManager.Multiplayer())
		{
			base.photonView.RPC("SpawnRPC", RpcTarget.All, Array.Empty<object>());
			return;
		}
		this.SpawnRPC();
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x000378D8 File Offset: 0x00035AD8
	[PunRPC]
	private void SpawnRPC()
	{
		if (this.Enemy.HasHealth)
		{
			this.Enemy.Health.OnSpawn();
		}
		if (this.Enemy.HasStateStunned)
		{
			this.Enemy.StateStunned.Spawn();
		}
		if (this.Enemy.HasJump)
		{
			this.Enemy.Jump.StuckReset();
		}
		this.Enemy.StuckCount = 0;
		this.Spawned = true;
		this.EnableObject.SetActive(true);
		this.Enemy.StateSpawn.OnSpawn.Invoke();
		this.Enemy.Spawn();
		if (!EnemyDirector.instance.debugNoSpawnedPause)
		{
			this.SpawnedTimerPause(Random.Range(3f, 4f) * 60f);
		}
		this.forceLeave = false;
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x000379AC File Offset: 0x00035BAC
	public void Despawn()
	{
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			return;
		}
		this.Enemy.CurrentState = EnemyState.Despawn;
		this.DespawnedTimer = Random.Range(this.DespawnedTimeMin, this.DespawnedTimeMax) * EnemyDirector.instance.despawnedTimeMultiplier;
		this.DespawnedTimer = Mathf.Max(this.DespawnedTimer, 1f);
		if (this.Enemy.HasRigidbody)
		{
			this.Enemy.Rigidbody.grabbed = false;
			this.Enemy.Rigidbody.grabStrengthTimer = 0f;
			this.Enemy.Rigidbody.GrabRelease();
		}
		if (GameManager.Multiplayer())
		{
			base.photonView.RPC("DespawnRPC", RpcTarget.All, Array.Empty<object>());
		}
		else
		{
			this.DespawnRPC();
		}
		if (this.Enemy.HasHealth && this.Enemy.Health.spawnValuable && this.Enemy.Health.healthCurrent <= 0)
		{
			if (this.valuableSpawnTimer > 0f && this.Enemy.Health.spawnValuableCurrent < this.Enemy.Health.spawnValuableMax)
			{
				GameObject gameObject = AssetManager.instance.enemyValuableSmall;
				if (this.difficulty == EnemyParent.Difficulty.Difficulty2)
				{
					gameObject = AssetManager.instance.enemyValuableMedium;
				}
				else if (this.difficulty == EnemyParent.Difficulty.Difficulty3)
				{
					gameObject = AssetManager.instance.enemyValuableBig;
				}
				Transform transform = this.Enemy.CustomValuableSpawnTransform;
				if (!transform)
				{
					transform = this.Enemy.CenterTransform;
				}
				if (!SemiFunc.IsMultiplayer())
				{
					Object.Instantiate<GameObject>(gameObject, transform.position, Quaternion.identity);
				}
				else
				{
					PhotonNetwork.InstantiateRoomObject("Valuables/" + gameObject.name, transform.position, Quaternion.identity, 0, null);
				}
				this.Enemy.Health.spawnValuableCurrent++;
			}
			this.DespawnedTimer *= 3f;
		}
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x00037BA2 File Offset: 0x00035DA2
	[PunRPC]
	private void DespawnRPC()
	{
		this.Spawned = false;
		this.EnableObject.SetActive(false);
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00037BB7 File Offset: 0x00035DB7
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.SetupDone);
			return;
		}
		this.SetupDone = (bool)stream.ReceiveNext();
	}

	// Token: 0x0400090E RID: 2318
	public string enemyName = "Dinosaur";

	// Token: 0x0400090F RID: 2319
	internal bool SetupDone;

	// Token: 0x04000910 RID: 2320
	internal bool Spawned = true;

	// Token: 0x04000911 RID: 2321
	internal Enemy Enemy;

	// Token: 0x04000912 RID: 2322
	[Space]
	public EnemyParent.Difficulty difficulty;

	// Token: 0x04000913 RID: 2323
	[Space]
	public float actionMultiplier = 1f;

	// Token: 0x04000914 RID: 2324
	[Space]
	public GameObject EnableObject;

	// Token: 0x04000915 RID: 2325
	[Space]
	public float SpawnedTimeMin;

	// Token: 0x04000916 RID: 2326
	public float SpawnedTimeMax;

	// Token: 0x04000917 RID: 2327
	[Space]
	public float DespawnedTimeMin;

	// Token: 0x04000918 RID: 2328
	public float DespawnedTimeMax;

	// Token: 0x04000919 RID: 2329
	[Space]
	public float SpawnedTimer;

	// Token: 0x0400091A RID: 2330
	public float DespawnedTimer;

	// Token: 0x0400091B RID: 2331
	private float spawnedTimerPauseTimer;

	// Token: 0x0400091C RID: 2332
	private float valuableSpawnTimer;

	// Token: 0x0400091D RID: 2333
	internal bool playerClose;

	// Token: 0x0400091E RID: 2334
	internal bool playerVeryClose;

	// Token: 0x0400091F RID: 2335
	internal bool forceLeave;

	// Token: 0x04000920 RID: 2336
	internal List<RoomVolume> currentRooms = new List<RoomVolume>();

	// Token: 0x04000921 RID: 2337
	internal LevelPoint firstSpawnPoint;

	// Token: 0x04000922 RID: 2338
	internal bool firstSpawnPointUsed;

	// Token: 0x020002E7 RID: 743
	public enum Difficulty
	{
		// Token: 0x040024F0 RID: 9456
		Difficulty1,
		// Token: 0x040024F1 RID: 9457
		Difficulty2,
		// Token: 0x040024F2 RID: 9458
		Difficulty3
	}
}
