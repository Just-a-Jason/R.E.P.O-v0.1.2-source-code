using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000A4 RID: 164
public class EnemyVision : MonoBehaviour
{
	// Token: 0x0600064C RID: 1612 RVA: 0x0003D015 File Offset: 0x0003B215
	private void Awake()
	{
		this.Enemy = base.GetComponent<Enemy>();
		base.StartCoroutine(this.Vision());
		this.VisionLogicActive = true;
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0003D037 File Offset: 0x0003B237
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.VisionLogicActive = false;
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x0003D046 File Offset: 0x0003B246
	private void OnEnable()
	{
		if (!this.VisionLogicActive)
		{
			base.StartCoroutine(this.Vision());
			this.VisionLogicActive = true;
		}
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x0003D064 File Offset: 0x0003B264
	public void PlayerAdded(int photonID)
	{
		this.VisionsTriggered.TryAdd(photonID, 0);
		this.VisionTriggered.TryAdd(photonID, false);
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x0003D082 File Offset: 0x0003B282
	public void PlayerRemoved(int photonID)
	{
		this.VisionsTriggered.Remove(photonID);
		this.VisionTriggered.Remove(photonID);
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0003D09E File Offset: 0x0003B29E
	private IEnumerator Vision()
	{
		this.VisionLogicActive = true;
		if (GameManager.Multiplayer() && !PhotonNetwork.IsMasterClient)
		{
			yield break;
		}
		while (this.VisionsTriggered.Count == 0)
		{
			yield return new WaitForSeconds(this.VisionCheckTime);
		}
		for (;;)
		{
			if (this.DisableTimer <= 0f && !EnemyDirector.instance.debugNoVision)
			{
				if (!this.Enemy.HasStateChaseBegin || this.Enemy.CurrentState != EnemyState.ChaseBegin)
				{
					this.HasVision = false;
					bool[] array = new bool[GameDirector.instance.PlayerList.Count];
					if (this.PhysObjectVision)
					{
						float radius = this.PhysObjectVisionRadius;
						if (this.PhysObjectVisionRadiusOverride > 0f)
						{
							radius = this.PhysObjectVisionRadiusOverride;
						}
						foreach (Collider collider in Physics.OverlapSphere(this.VisionTransform.position, radius, SemiFunc.LayerMaskGetPhysGrabObject()))
						{
							if (collider.CompareTag("Phys Grab Object"))
							{
								PhysGrabObject componentInParent = collider.GetComponentInParent<PhysGrabObject>();
								if (componentInParent && componentInParent.playerGrabbing.Count > 0)
								{
									Vector3 direction = componentInParent.centerPoint - this.VisionTransform.position;
									RaycastHit[] array3 = Physics.RaycastAll(this.VisionTransform.position, direction, direction.magnitude, this.Enemy.VisionMask);
									bool flag = true;
									if (array3.Length != 0)
									{
										RaycastHit[] array4 = array3;
										int j = 0;
										while (j < array4.Length)
										{
											RaycastHit raycastHit = array4[j];
											if (!raycastHit.transform.CompareTag("Phys Grab Object") && !raycastHit.transform.CompareTag("Enemy"))
											{
												goto IL_2B7;
											}
											PhysGrabObject componentInParent2 = raycastHit.transform.GetComponentInParent<PhysGrabObject>();
											if (!componentInParent2 || (!(componentInParent2 == componentInParent) && (!this.Enemy.HasRigidbody || !(raycastHit.transform.GetComponentInParent<EnemyRigidbody>() == this.Enemy.Rigidbody))))
											{
												goto IL_2B7;
											}
											IL_2BA:
											j++;
											continue;
											IL_2B7:
											flag = false;
											goto IL_2BA;
										}
									}
									if (flag && Vector3.Dot(this.VisionTransform.forward, direction.normalized) >= this.PhysObjectVisionDot)
									{
										int num = 0;
										using (List<PlayerAvatar>.Enumerator enumerator = GameDirector.instance.PlayerList.GetEnumerator())
										{
											while (enumerator.MoveNext())
											{
												if (enumerator.Current == componentInParent.playerGrabbing[0].playerAvatar)
												{
													array[num] = true;
												}
												num++;
											}
										}
									}
								}
							}
						}
					}
					int num2 = 0;
					foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
					{
						bool flag2 = false;
						if (!playerAvatar.isDisabled)
						{
							int viewID = playerAvatar.photonView.ViewID;
							if (playerAvatar.enemyVisionFreezeTimer > 0f)
							{
								if (this.VisionTriggered[viewID])
								{
									this.VisionTrigger(viewID, playerAvatar, false, false);
								}
								num2++;
							}
							else
							{
								this.VisionTriggered[viewID] = false;
								float num3 = Vector3.Distance(this.VisionTransform.position, playerAvatar.transform.position);
								if (array[num2] || num3 <= this.VisionDistance)
								{
									bool flag3 = playerAvatar.isCrawling;
									bool flag4 = playerAvatar.isCrouching;
									if (playerAvatar.isTumbling)
									{
										flag4 = true;
										flag3 = false;
									}
									if (this.StandOverrideTimer > 0f)
									{
										flag4 = false;
										flag3 = false;
									}
									Transform transform = null;
									Transform transform2 = null;
									Vector3 direction2 = playerAvatar.PlayerVisionTarget.VisionTransform.transform.position - this.VisionTransform.position;
									Collider[] array5 = Physics.OverlapSphere(this.VisionTransform.position, 0.01f, this.Enemy.VisionMask);
									if (array5.Length != 0)
									{
										foreach (Collider collider2 in array5)
										{
											if (!collider2.transform.CompareTag("Enemy"))
											{
												if (collider2.transform.CompareTag("Player"))
												{
													transform = collider2.transform;
												}
												if (collider2.transform.GetComponentInParent<PlayerTumble>())
												{
													transform = collider2.transform;
												}
												if (!collider2.transform.GetComponentInParent<EnemyRigidbody>())
												{
													transform2 = collider2.transform;
												}
											}
										}
									}
									if (!transform2)
									{
										RaycastHit[] array6 = Physics.RaycastAll(this.VisionTransform.position, direction2, this.VisionDistance, this.Enemy.VisionMask);
										float num4 = 1000f;
										foreach (RaycastHit raycastHit2 in array6)
										{
											if (!raycastHit2.transform.CompareTag("Enemy"))
											{
												if (raycastHit2.transform.CompareTag("Player"))
												{
													transform = raycastHit2.transform;
												}
												if (!raycastHit2.transform.GetComponentInParent<EnemyRigidbody>())
												{
													if (raycastHit2.transform.GetComponentInParent<PlayerTumble>())
													{
														transform = raycastHit2.transform;
													}
													float num5 = Vector3.Distance(this.VisionTransform.position, raycastHit2.point);
													if (num5 < num4)
													{
														num4 = num5;
														transform2 = raycastHit2.transform;
													}
												}
											}
										}
									}
									if (array[num2] || (transform && transform == transform2))
									{
										float num6 = Vector3.Dot(this.VisionTransform.forward, direction2.normalized);
										bool flag5 = false;
										if (flag4)
										{
											if (num3 <= this.VisionDistanceCloseCrouch)
											{
												flag5 = true;
											}
										}
										else if (num3 <= this.VisionDistanceClose)
										{
											flag5 = true;
										}
										if (flag5)
										{
											this.VisionsTriggered[viewID] = this.VisionsToTrigger;
										}
										bool flag6 = false;
										if (flag3 && this.Enemy.CurrentState != EnemyState.LookUnder)
										{
											if (num6 >= this.VisionDotCrawl)
											{
												flag6 = true;
											}
										}
										else if (flag4 && this.Enemy.CurrentState != EnemyState.LookUnder)
										{
											if (num6 >= this.VisionDotCrouch)
											{
												flag6 = true;
											}
										}
										else if (num6 >= this.VisionDotStanding)
										{
											flag6 = true;
										}
										if (array[num2] || flag6 || flag5)
										{
											flag2 = true;
											bool flag7 = false;
											if (flag3 && this.Enemy.CurrentState != EnemyState.LookUnder)
											{
												if (this.VisionsTriggered[viewID] >= this.VisionsToTriggerCrawl)
												{
													flag7 = true;
												}
											}
											else if (flag4 && this.Enemy.CurrentState != EnemyState.LookUnder)
											{
												if (this.VisionsTriggered[viewID] >= this.VisionsToTriggerCrouch)
												{
													flag7 = true;
												}
											}
											else if (this.VisionsTriggered[viewID] >= this.VisionsToTrigger)
											{
												flag7 = true;
											}
											bool culled = false;
											if (this.Enemy.HasOnScreen)
											{
												if (GameManager.instance.gameMode == 0)
												{
													if (this.Enemy.OnScreen.CulledLocal)
													{
														culled = true;
													}
												}
												else if (this.Enemy.OnScreen.CulledPlayer[playerAvatar.photonView.ViewID])
												{
													culled = true;
												}
											}
											if (flag7 || flag5)
											{
												this.VisionTrigger(viewID, playerAvatar, culled, flag5);
											}
										}
									}
									if (flag2)
									{
										Dictionary<int, int> visionsTriggered = this.VisionsTriggered;
										int i = viewID;
										int j = visionsTriggered[i];
										visionsTriggered[i] = j + 1;
									}
									else
									{
										this.VisionsTriggered[viewID] = 0;
									}
									num2++;
								}
							}
						}
					}
				}
				if (this.StandOverrideTimer > 0f)
				{
					this.StandOverrideTimer -= this.VisionCheckTime;
				}
				yield return new WaitForSeconds(this.VisionCheckTime);
			}
			else
			{
				this.DisableTimer -= Time.deltaTime;
				foreach (PlayerAvatar playerAvatar2 in GameDirector.instance.PlayerList)
				{
					int viewID2 = playerAvatar2.photonView.ViewID;
					this.VisionTriggered[viewID2] = false;
					this.VisionsTriggered[viewID2] = 0;
				}
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x0003D0B0 File Offset: 0x0003B2B0
	public void VisionTrigger(int playerID, PlayerAvatar player, bool culled, bool playerNear)
	{
		this.VisionTriggered[playerID] = true;
		this.VisionsTriggered[playerID] = Mathf.Max(this.VisionsTriggered[playerID], this.VisionsToTrigger);
		this.onVisionTriggeredID = playerID;
		this.onVisionTriggeredPlayer = player;
		this.onVisionTriggeredCulled = culled;
		this.onVisionTriggeredNear = playerNear;
		this.onVisionTriggered.Invoke();
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x0003D115 File Offset: 0x0003B315
	public void DisableVision(float time)
	{
		this.DisableTimer = time;
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x0003D11E File Offset: 0x0003B31E
	public void StandOverride(float time)
	{
		this.StandOverrideTimer = time;
	}

	// Token: 0x04000A8E RID: 2702
	private Enemy Enemy;

	// Token: 0x04000A8F RID: 2703
	internal bool HasVision;

	// Token: 0x04000A90 RID: 2704
	internal float DisableTimer;

	// Token: 0x04000A91 RID: 2705
	internal float StandOverrideTimer;

	// Token: 0x04000A92 RID: 2706
	private float VisionTimer;

	// Token: 0x04000A93 RID: 2707
	private float VisionCheckTime = 0.25f;

	// Token: 0x04000A94 RID: 2708
	public Transform VisionTransform;

	// Token: 0x04000A95 RID: 2709
	[Header("Base")]
	public float VisionDistance = 10f;

	// Token: 0x04000A96 RID: 2710
	public Dictionary<int, int> VisionsTriggered = new Dictionary<int, int>();

	// Token: 0x04000A97 RID: 2711
	public Dictionary<int, bool> VisionTriggered = new Dictionary<int, bool>();

	// Token: 0x04000A98 RID: 2712
	[Header("Close")]
	public float VisionDistanceClose = 3.5f;

	// Token: 0x04000A99 RID: 2713
	public float VisionDistanceCloseCrouch = 2f;

	// Token: 0x04000A9A RID: 2714
	[Header("Dot")]
	public float VisionDotStanding = 0.4f;

	// Token: 0x04000A9B RID: 2715
	public float VisionDotCrouch = 0.6f;

	// Token: 0x04000A9C RID: 2716
	public float VisionDotCrawl = 0.9f;

	// Token: 0x04000A9D RID: 2717
	[Header("Phys Object Vision")]
	public bool PhysObjectVision = true;

	// Token: 0x04000A9E RID: 2718
	private float PhysObjectVisionRadius = 10f;

	// Token: 0x04000A9F RID: 2719
	public float PhysObjectVisionRadiusOverride = -1f;

	// Token: 0x04000AA0 RID: 2720
	public float PhysObjectVisionDot = 0.4f;

	// Token: 0x04000AA1 RID: 2721
	[Header("Triggers")]
	public int VisionsToTrigger = 4;

	// Token: 0x04000AA2 RID: 2722
	public int VisionsToTriggerCrouch = 10;

	// Token: 0x04000AA3 RID: 2723
	public int VisionsToTriggerCrawl = 20;

	// Token: 0x04000AA4 RID: 2724
	[Header("Events")]
	public UnityEvent onVisionTriggered;

	// Token: 0x04000AA5 RID: 2725
	internal int onVisionTriggeredID;

	// Token: 0x04000AA6 RID: 2726
	internal PlayerAvatar onVisionTriggeredPlayer;

	// Token: 0x04000AA7 RID: 2727
	internal bool onVisionTriggeredCulled;

	// Token: 0x04000AA8 RID: 2728
	internal bool onVisionTriggeredNear;

	// Token: 0x04000AA9 RID: 2729
	internal float onVisionTriggeredDistance;

	// Token: 0x04000AAA RID: 2730
	private bool VisionLogicActive;
}
