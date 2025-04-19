using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000089 RID: 137
[RequireComponent(typeof(PhotonView))]
public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06000575 RID: 1397 RVA: 0x00035E98 File Offset: 0x00034098
	private void Awake()
	{
		this.photonTransformView = base.transform.parent.GetComponentInChildren<PhotonTransformView>();
		this.EnemyParent = base.GetComponentInParent<EnemyParent>();
		this.PhotonView = base.GetComponent<PhotonView>();
		this.Vision = base.GetComponent<EnemyVision>();
		if (this.Vision)
		{
			this.HasVision = true;
		}
		this.VisionMask = SemiFunc.LayerMaskGetVisionObstruct() + LayerMask.GetMask(new string[]
		{
			"HideTriggers"
		});
		this.PlayerDistance = base.GetComponent<EnemyPlayerDistance>();
		if (this.PlayerDistance)
		{
			this.HasPlayerDistance = true;
		}
		this.OnScreen = base.GetComponent<EnemyOnScreen>();
		if (this.OnScreen)
		{
			this.HasOnScreen = true;
		}
		this.PlayerRoom = base.GetComponent<EnemyPlayerRoom>();
		if (this.PlayerRoom)
		{
			this.HasPlayerRoom = true;
		}
		this.NavMeshAgent = base.GetComponent<EnemyNavMeshAgent>();
		if (this.NavMeshAgent)
		{
			this.HasNavMeshAgent = true;
		}
		this.AttackStuckPhysObject = base.GetComponent<EnemyAttackStuckPhysObject>();
		if (this.AttackStuckPhysObject)
		{
			this.HasAttackPhysObject = true;
		}
		this.StateInvestigate = base.GetComponent<EnemyStateInvestigate>();
		if (this.StateInvestigate)
		{
			this.HasStateInvestigate = true;
		}
		this.StateChaseBegin = base.GetComponent<EnemyStateChaseBegin>();
		if (this.StateChaseBegin)
		{
			this.HasStateChaseBegin = true;
		}
		this.StateChase = base.GetComponent<EnemyStateChase>();
		if (this.StateChase)
		{
			this.HasStateChase = true;
		}
		this.StateLookUnder = base.GetComponent<EnemyStateLookUnder>();
		if (this.StateLookUnder)
		{
			this.HasStateLookUnder = true;
		}
		this.StateDespawn = base.GetComponent<EnemyStateDespawn>();
		if (this.StateDespawn)
		{
			this.HasStateDespawn = true;
		}
		this.StateSpawn = base.GetComponent<EnemyStateSpawn>();
		if (this.StateSpawn)
		{
			this.HasStateSpawn = true;
		}
		this.StateStunned = base.GetComponent<EnemyStateStunned>();
		if (this.StateStunned)
		{
			this.HasStateStunned = true;
		}
		this.Health = base.GetComponent<EnemyHealth>();
		if (this.Health)
		{
			this.HasHealth = true;
		}
		if (!this.CenterTransform)
		{
			Debug.LogError("Center Transform not set in " + base.gameObject.name, base.gameObject);
		}
	}

	// Token: 0x06000576 RID: 1398 RVA: 0x000360E9 File Offset: 0x000342E9
	private void Start()
	{
		if (!GameManager.Multiplayer() || PhotonNetwork.IsMasterClient)
		{
			this.MasterClient = true;
			return;
		}
		this.MasterClient = false;
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x00036108 File Offset: 0x00034308
	private void Update()
	{
		if (SemiFunc.IsMultiplayer() && !this.MasterClient)
		{
			float num = 1f / (float)PhotonNetwork.SerializationRate;
			float num2 = this.PositionDistance / num;
			this.moveDirection = (this.PositionTarget - base.transform.position).normalized;
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.PositionTarget, num2 * Time.deltaTime);
		}
		if (this.MasterClient)
		{
			this.Stunned = false;
			if (this.HasStateStunned && this.StateStunned.stunTimer > 0f)
			{
				this.Stunned = true;
			}
		}
		if (this.FreezeTimer > 0f)
		{
			this.FreezeTimer -= Time.deltaTime;
		}
		if (this.TeleportedTimer > 0f)
		{
			this.StuckCount = 0;
			this.TeleportedTimer -= Time.deltaTime;
		}
		if (this.ChaseTimer > 0f)
		{
			this.ChaseTimer -= Time.deltaTime;
		}
		if (this.DisableChaseTimer > 0f)
		{
			this.DisableChaseTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x00036239 File Offset: 0x00034439
	public void Spawn()
	{
		this.Stunned = false;
		this.FreezeTimer = 0f;
	}

	// Token: 0x06000579 RID: 1401 RVA: 0x0003624D File Offset: 0x0003444D
	public bool IsStunned()
	{
		return this.Stunned;
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x00036255 File Offset: 0x00034455
	public void DisableChase(float time)
	{
		this.DisableChaseTimer = time;
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x0003625E File Offset: 0x0003445E
	public void SetChaseTimer()
	{
		this.ChaseTimer = 0.1f;
	}

	// Token: 0x0600057C RID: 1404 RVA: 0x0003626B File Offset: 0x0003446B
	public bool CheckChase()
	{
		return this.ChaseTimer > 0f;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x0003627C File Offset: 0x0003447C
	public void SetChaseTarget(PlayerAvatar playerAvatar)
	{
		if (EnemyDirector.instance.debugNoVision)
		{
			return;
		}
		if (this.DisableChaseTimer > 0f)
		{
			return;
		}
		if (!this.HasVision)
		{
			return;
		}
		if (!playerAvatar.isDisabled)
		{
			this.Vision.VisionTrigger(playerAvatar.photonView.ViewID, playerAvatar, false, false);
			if (!this.HasStateChase)
			{
				return;
			}
			if (!this.CheckChase() || this.CurrentState == EnemyState.ChaseSlow)
			{
				this.CurrentState = EnemyState.ChaseBegin;
				this.TargetPlayerViewID = playerAvatar.photonView.ViewID;
				this.TargetPlayerAvatar = playerAvatar;
			}
		}
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00036308 File Offset: 0x00034508
	public LevelPoint TeleportToPoint(float minDistance, float maxDistance)
	{
		LevelPoint levelPoint = null;
		if (SemiFunc.EnemySpawnIdlePause())
		{
			levelPoint = this.EnemyParent.firstSpawnPoint;
		}
		else
		{
			if (RoundDirector.instance.allExtractionPointsCompleted)
			{
				levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, minDistance, maxDistance, true);
			}
			if (!levelPoint)
			{
				levelPoint = SemiFunc.LevelPointGetPlayerDistance(base.transform.position, minDistance, maxDistance, false);
			}
		}
		if (levelPoint)
		{
			this.TeleportPosition = new Vector3(levelPoint.transform.position.x, levelPoint.transform.position.y, levelPoint.transform.position.z);
			this.EnemyTeleported(this.TeleportPosition);
		}
		return levelPoint;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x000363B8 File Offset: 0x000345B8
	public LevelPoint GetLevelPointAhead(Vector3 currentTargetPosition)
	{
		LevelPoint result = null;
		Vector3 normalized = (currentTargetPosition - base.transform.position).normalized;
		LevelPoint levelPoint = null;
		float num = 1000f;
		foreach (LevelPoint levelPoint2 in LevelGenerator.Instance.LevelPathPoints)
		{
			if (levelPoint2)
			{
				float num2 = Vector3.Distance(levelPoint2.transform.position, currentTargetPosition);
				if (num2 < num)
				{
					num = num2;
					levelPoint = levelPoint2;
				}
			}
		}
		if (!levelPoint)
		{
			return null;
		}
		float num3 = -1f;
		foreach (LevelPoint levelPoint3 in levelPoint.ConnectedPoints)
		{
			if (levelPoint3)
			{
				Vector3 normalized2 = (levelPoint3.transform.position - levelPoint.transform.position).normalized;
				float num4 = Vector3.Dot(normalized, normalized2);
				if (num4 > num3)
				{
					num3 = num4;
					result = levelPoint3;
				}
			}
		}
		return result;
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x000364F0 File Offset: 0x000346F0
	public void Freeze(float time)
	{
		if (GameManager.instance.gameMode == 0)
		{
			this.FreezeRPC(time);
			return;
		}
		base.photonView.RPC("FreezeRPC", RpcTarget.All, new object[]
		{
			time
		});
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x00036526 File Offset: 0x00034726
	[PunRPC]
	public void FreezeRPC(float time)
	{
		this.FreezeTimer = time;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0003652F File Offset: 0x0003472F
	public void PlayerAdded(int photonID)
	{
		if (this.HasVision)
		{
			this.Vision.PlayerAdded(photonID);
		}
		if (this.HasOnScreen)
		{
			this.OnScreen.PlayerAdded(photonID);
		}
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x0003655C File Offset: 0x0003475C
	public void PlayerRemoved(int photonID)
	{
		if (this.StateChaseBegin != null && this.StateChaseBegin.TargetPlayer != null && this.StateChaseBegin.TargetPlayer.photonView.ViewID == photonID)
		{
			this.StateChaseBegin.TargetPlayer = null;
			this.CurrentState = EnemyState.Roaming;
		}
		if (this.TargetPlayerAvatar != null && this.TargetPlayerAvatar.photonView.ViewID == photonID)
		{
			this.TargetPlayerAvatar = PlayerController.instance.playerAvatarScript;
			this.TargetPlayerViewID = this.TargetPlayerAvatar.photonView.ViewID;
		}
		if (this.HasVision)
		{
			this.Vision.PlayerRemoved(photonID);
		}
		if (this.HasOnScreen)
		{
			this.OnScreen.PlayerRemoved(photonID);
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00036624 File Offset: 0x00034824
	public void EnemyTeleported(Vector3 teleportPosition)
	{
		base.transform.position = teleportPosition;
		if (this.HasNavMeshAgent)
		{
			this.NavMeshAgent.Warp(teleportPosition);
		}
		if (this.HasRigidbody)
		{
			this.Rigidbody.Teleport();
		}
		if (GameManager.instance.gameMode == 0)
		{
			this.EnemyTeleportedRPC(teleportPosition);
			return;
		}
		base.photonView.RPC("EnemyTeleportedRPC", RpcTarget.All, new object[]
		{
			teleportPosition
		});
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x00036698 File Offset: 0x00034898
	[PunRPC]
	private void EnemyTeleportedRPC(Vector3 teleportPosition)
	{
		this.PositionDistance = 0f;
		this.PositionTarget = teleportPosition;
		this.TeleportPosition = teleportPosition;
		base.transform.position = teleportPosition;
		this.TeleportedTimer = 1f;
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x000366CC File Offset: 0x000348CC
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(this.CurrentState);
			stream.SendNext(this.TargetPlayerViewID);
			stream.SendNext(this.Stunned);
			return;
		}
		this.PositionTarget = (Vector3)stream.ReceiveNext();
		this.PositionDistance = Vector3.Distance(base.transform.position, this.PositionTarget);
		this.CurrentState = (EnemyState)stream.ReceiveNext();
		this.TargetPlayerViewID = (int)stream.ReceiveNext();
		this.Stunned = (bool)stream.ReceiveNext();
		foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
		{
			if (!playerAvatar.isDisabled && playerAvatar.photonView.ViewID == this.TargetPlayerViewID)
			{
				this.TargetPlayerAvatar = playerAvatar;
				break;
			}
		}
	}

	// Token: 0x04000892 RID: 2194
	internal PhotonView PhotonView;

	// Token: 0x04000893 RID: 2195
	internal EnemyParent EnemyParent;

	// Token: 0x04000894 RID: 2196
	internal bool MasterClient;

	// Token: 0x04000895 RID: 2197
	public EnemyType Type = EnemyType.Medium;

	// Token: 0x04000896 RID: 2198
	[Space]
	public EnemyState CurrentState;

	// Token: 0x04000897 RID: 2199
	private EnemyState PreviousState;

	// Token: 0x04000898 RID: 2200
	private int CurrentStateIndex;

	// Token: 0x04000899 RID: 2201
	[Space]
	public Transform CenterTransform;

	// Token: 0x0400089A RID: 2202
	public Transform KillLookAtTransform;

	// Token: 0x0400089B RID: 2203
	public Transform CustomValuableSpawnTransform;

	// Token: 0x0400089C RID: 2204
	internal LayerMask VisionMask;

	// Token: 0x0400089D RID: 2205
	private Vector3 PositionTarget;

	// Token: 0x0400089E RID: 2206
	private float PositionDistance;

	// Token: 0x0400089F RID: 2207
	internal int StuckCount;

	// Token: 0x040008A0 RID: 2208
	internal EnemyVision Vision;

	// Token: 0x040008A1 RID: 2209
	internal bool HasVision;

	// Token: 0x040008A2 RID: 2210
	internal EnemyPlayerDistance PlayerDistance;

	// Token: 0x040008A3 RID: 2211
	internal bool HasPlayerDistance;

	// Token: 0x040008A4 RID: 2212
	internal EnemyOnScreen OnScreen;

	// Token: 0x040008A5 RID: 2213
	internal bool HasOnScreen;

	// Token: 0x040008A6 RID: 2214
	internal EnemyPlayerRoom PlayerRoom;

	// Token: 0x040008A7 RID: 2215
	internal bool HasPlayerRoom;

	// Token: 0x040008A8 RID: 2216
	internal EnemyRigidbody Rigidbody;

	// Token: 0x040008A9 RID: 2217
	internal bool HasRigidbody;

	// Token: 0x040008AA RID: 2218
	internal EnemyNavMeshAgent NavMeshAgent;

	// Token: 0x040008AB RID: 2219
	internal bool HasNavMeshAgent;

	// Token: 0x040008AC RID: 2220
	internal EnemyAttackStuckPhysObject AttackStuckPhysObject;

	// Token: 0x040008AD RID: 2221
	internal bool HasAttackPhysObject;

	// Token: 0x040008AE RID: 2222
	internal EnemyStateInvestigate StateInvestigate;

	// Token: 0x040008AF RID: 2223
	internal bool HasStateInvestigate;

	// Token: 0x040008B0 RID: 2224
	internal EnemyStateChaseBegin StateChaseBegin;

	// Token: 0x040008B1 RID: 2225
	internal bool HasStateChaseBegin;

	// Token: 0x040008B2 RID: 2226
	internal EnemyStateChase StateChase;

	// Token: 0x040008B3 RID: 2227
	internal bool HasStateChase;

	// Token: 0x040008B4 RID: 2228
	internal EnemyStateLookUnder StateLookUnder;

	// Token: 0x040008B5 RID: 2229
	internal bool HasStateLookUnder;

	// Token: 0x040008B6 RID: 2230
	internal EnemyStateDespawn StateDespawn;

	// Token: 0x040008B7 RID: 2231
	internal bool HasStateDespawn;

	// Token: 0x040008B8 RID: 2232
	internal EnemyStateSpawn StateSpawn;

	// Token: 0x040008B9 RID: 2233
	internal bool HasStateSpawn;

	// Token: 0x040008BA RID: 2234
	private bool Stunned;

	// Token: 0x040008BB RID: 2235
	internal EnemyStateStunned StateStunned;

	// Token: 0x040008BC RID: 2236
	internal bool HasStateStunned;

	// Token: 0x040008BD RID: 2237
	internal EnemyGrounded Grounded;

	// Token: 0x040008BE RID: 2238
	internal bool HasGrounded;

	// Token: 0x040008BF RID: 2239
	internal EnemyJump Jump;

	// Token: 0x040008C0 RID: 2240
	internal bool HasJump;

	// Token: 0x040008C1 RID: 2241
	internal EnemyHealth Health;

	// Token: 0x040008C2 RID: 2242
	internal bool HasHealth;

	// Token: 0x040008C3 RID: 2243
	internal PlayerAvatar TargetPlayerAvatar;

	// Token: 0x040008C4 RID: 2244
	internal int TargetPlayerViewID;

	// Token: 0x040008C5 RID: 2245
	protected internal float TeleportedTimer;

	// Token: 0x040008C6 RID: 2246
	protected internal Vector3 TeleportPosition;

	// Token: 0x040008C7 RID: 2247
	[HideInInspector]
	public float FreezeTimer;

	// Token: 0x040008C8 RID: 2248
	private float ChaseTimer;

	// Token: 0x040008C9 RID: 2249
	internal float DisableChaseTimer;

	// Token: 0x040008CA RID: 2250
	private PhotonTransformView photonTransformView;

	// Token: 0x040008CB RID: 2251
	[Space]
	public bool SightingStinger;

	// Token: 0x040008CC RID: 2252
	public bool EnemyNearMusic;

	// Token: 0x040008CD RID: 2253
	internal Vector3 moveDirection;
}
