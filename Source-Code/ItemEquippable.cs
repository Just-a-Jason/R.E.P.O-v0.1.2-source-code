using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000151 RID: 337
public class ItemEquippable : MonoBehaviourPunCallbacks
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000B35 RID: 2869 RVA: 0x00063CDE File Offset: 0x00061EDE
	private Rigidbody rb
	{
		get
		{
			return base.GetComponent<Rigidbody>();
		}
	}

	// Token: 0x06000B36 RID: 2870 RVA: 0x00063CE6 File Offset: 0x00061EE6
	private void Start()
	{
		this.physGrabObject = base.GetComponent<PhysGrabObject>();
	}

	// Token: 0x06000B37 RID: 2871 RVA: 0x00063CF4 File Offset: 0x00061EF4
	public bool IsEquipped()
	{
		return this.currentState == ItemEquippable.ItemState.Equipped;
	}

	// Token: 0x06000B38 RID: 2872 RVA: 0x00063CFF File Offset: 0x00061EFF
	private bool CollisionCheck()
	{
		return false;
	}

	// Token: 0x06000B39 RID: 2873 RVA: 0x00063D04 File Offset: 0x00061F04
	public void RequestEquip(int spot, int requestingPlayerId = -1)
	{
		if (this.IsEquipped() || this.currentState == ItemEquippable.ItemState.Unequipping)
		{
			return;
		}
		if (SemiFunc.IsMultiplayer())
		{
			base.photonView.RPC("RPC_RequestEquip", RpcTarget.MasterClient, new object[]
			{
				spot,
				requestingPlayerId
			});
			return;
		}
		this.RPC_RequestEquip(spot, -1);
	}

	// Token: 0x06000B3A RID: 2874 RVA: 0x00063D5C File Offset: 0x00061F5C
	[PunRPC]
	private void RPC_RequestEquip(int spotIndex, int physGrabberPhotonViewID)
	{
		bool flag = SemiFunc.IsMultiplayer();
		if (this.currentState != ItemEquippable.ItemState.Idle)
		{
			return;
		}
		if (flag)
		{
			base.photonView.RPC("RPC_UpdateItemState", RpcTarget.All, new object[]
			{
				2,
				spotIndex,
				physGrabberPhotonViewID
			});
			return;
		}
		this.RPC_UpdateItemState(2, spotIndex, physGrabberPhotonViewID);
	}

	// Token: 0x06000B3B RID: 2875 RVA: 0x00063DB8 File Offset: 0x00061FB8
	[PunRPC]
	private void RPC_UpdateItemState(int state, int spotIndex, int ownerId)
	{
		bool flag = SemiFunc.IsMultiplayer();
		PlayerAvatar playerAvatar = PlayerAvatar.instance;
		if (SemiFunc.IsMultiplayer())
		{
			PhotonView photonView = PhotonView.Find(ownerId);
			playerAvatar = ((photonView != null) ? photonView.GetComponent<PlayerAvatar>() : null);
		}
		InventorySpot inventorySpot = null;
		if (flag)
		{
			if (PhysGrabber.instance.photonView.ViewID == ownerId)
			{
				if (spotIndex != -1)
				{
					inventorySpot = Inventory.instance.GetSpotByIndex(spotIndex);
				}
			}
			else
			{
				inventorySpot = null;
			}
		}
		else if (spotIndex != -1)
		{
			inventorySpot = Inventory.instance.GetSpotByIndex(spotIndex);
		}
		bool flag2 = false;
		if (inventorySpot == null)
		{
			flag2 = true;
		}
		if (inventorySpot != null && inventorySpot.IsOccupied())
		{
			flag2 = true;
		}
		if (Inventory.instance.IsItemEquipped(this))
		{
			flag2 = true;
		}
		if (state == 2)
		{
			string instanceName = base.GetComponent<ItemAttributes>().instanceName;
			StatsManager.instance.PlayerInventoryUpdate(playerAvatar.steamID, instanceName, spotIndex, true);
			this.currentState = ItemEquippable.ItemState.Equipped;
			if (!flag2)
			{
				this.equippedSpot = inventorySpot;
				InventorySpot inventorySpot2 = this.equippedSpot;
				if (inventorySpot2 != null)
				{
					inventorySpot2.EquipItem(this);
				}
			}
			else
			{
				this.equippedSpot = null;
			}
		}
		else
		{
			InventorySpot inventorySpot3 = this.equippedSpot;
			if (inventorySpot3 != null)
			{
				inventorySpot3.UnequipItem();
			}
			this.equippedSpot = null;
		}
		this.inventorySpotIndex = spotIndex;
		this.currentState = (ItemEquippable.ItemState)state;
		this.ownerPlayerId = ownerId;
		this.stateStart = true;
		this.UpdateVisuals();
	}

	// Token: 0x06000B3C RID: 2876 RVA: 0x00063EE0 File Offset: 0x000620E0
	private void IsEquippingAndUnequippingTimer()
	{
		if (this.isEquippingTimer > 0f)
		{
			if (this.isEquippingTimer <= 0f)
			{
				this.isEquipping = false;
			}
			this.isEquippingTimer -= Time.deltaTime;
		}
		if (this.isUnequippingTimer > 0f)
		{
			if (this.isUnequippingTimer <= 0f)
			{
				this.isUnequipping = false;
			}
			this.isUnequippingTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000B3D RID: 2877 RVA: 0x00063F54 File Offset: 0x00062154
	public void RequestUnequip()
	{
		if (!this.IsEquipped())
		{
			return;
		}
		this.currentState = ItemEquippable.ItemState.Unequipping;
		if (SemiFunc.IsMultiplayer())
		{
			base.photonView.RPC("RPC_StartUnequip", RpcTarget.All, new object[]
			{
				this.ownerPlayerId
			});
			return;
		}
		this.RPC_StartUnequip(this.ownerPlayerId);
	}

	// Token: 0x06000B3E RID: 2878 RVA: 0x00063FAA File Offset: 0x000621AA
	[PunRPC]
	private void RPC_StartUnequip(int requestingPlayerId)
	{
		if (this.ownerPlayerId != requestingPlayerId)
		{
			return;
		}
		if (!SemiFunc.IsMultiplayer() || PhysGrabber.instance.photonView.ViewID == this.ownerPlayerId)
		{
			this.PerformUnequip(requestingPlayerId);
		}
	}

	// Token: 0x06000B3F RID: 2879 RVA: 0x00063FDC File Offset: 0x000621DC
	private void PerformUnequip(int requestingPlayerId)
	{
		this.unequipTimer = 0.4f;
		this.SetRotation();
		this.currentState = ItemEquippable.ItemState.Unequipping;
		this.physGrabObject.OverrideDeactivateReset();
		if (SemiFunc.IsMultiplayer())
		{
			this.RayHitTestNew(1f);
			base.photonView.RPC("RPC_CompleteUnequip", RpcTarget.MasterClient, new object[]
			{
				requestingPlayerId,
				this.teleportPosition
			});
			return;
		}
		this.RayHitTestNew(1f);
		this.RPC_CompleteUnequip(requestingPlayerId, this.teleportPosition);
	}

	// Token: 0x06000B40 RID: 2880 RVA: 0x00064068 File Offset: 0x00062268
	private bool RayHitTestNew(float distance)
	{
		int layerMask = SemiFunc.LayerMaskGetVisionObstruct() & ~LayerMask.GetMask(new string[]
		{
			"Ignore Raycast",
			"CollisionCheck"
		});
		RaycastHit raycastHit;
		if (Camera.main && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, distance, layerMask))
		{
			this.teleportPosition = raycastHit.point;
		}
		else
		{
			this.teleportPosition = Camera.main.transform.position + Camera.main.transform.forward * distance;
		}
		return this.CollisionCheck();
	}

	// Token: 0x06000B41 RID: 2881 RVA: 0x00064118 File Offset: 0x00062318
	private bool RayHitTest(float distance)
	{
		int layerMask = SemiFunc.LayerMaskGetVisionObstruct() & ~LayerMask.GetMask(new string[]
		{
			"Ignore Raycast",
			"CollisionCheck"
		});
		if (Camera.main)
		{
			RaycastHit raycastHit;
			Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out raycastHit, distance, layerMask);
		}
		return this.CollisionCheck();
	}

	// Token: 0x06000B42 RID: 2882 RVA: 0x00064187 File Offset: 0x00062387
	private Vector3 GetUnequipPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06000B43 RID: 2883 RVA: 0x00064194 File Offset: 0x00062394
	[PunRPC]
	private void RPC_CompleteUnequip(int physGrabberPhotonViewID, Vector3 teleportPos)
	{
		PhysGrabber physGrabber;
		if (SemiFunc.IsMultiplayer())
		{
			physGrabber = PhotonView.Find(physGrabberPhotonViewID).GetComponent<PhysGrabber>();
		}
		else
		{
			physGrabber = PhysGrabber.instance;
		}
		StatsManager.instance.PlayerInventoryUpdate(physGrabber.playerAvatar.steamID, "", this.inventorySpotIndex, true);
		Transform visionTransform = physGrabber.playerAvatar.PlayerVisionTarget.VisionTransform;
		this.physGrabObject.Teleport(teleportPos, Quaternion.LookRotation(visionTransform.transform.forward, Vector3.up));
		this.rb.isKinematic = false;
		int num = (this.equippedSpot != null) ? this.equippedSpot.inventorySpotIndex : -1;
		InventorySpot inventorySpot = this.equippedSpot;
		if (inventorySpot != null)
		{
			inventorySpot.UnequipItem();
		}
		this.equippedSpot = null;
		this.ownerPlayerId = -1;
		if (SemiFunc.IsMultiplayer())
		{
			base.photonView.RPC("RPC_UpdateItemState", RpcTarget.All, new object[]
			{
				3,
				num,
				physGrabberPhotonViewID
			});
			return;
		}
		this.RPC_UpdateItemState(3, num, -1);
	}

	// Token: 0x06000B44 RID: 2884 RVA: 0x0006429A File Offset: 0x0006249A
	private void UpdateVisuals()
	{
		if (this.currentState == ItemEquippable.ItemState.Equipped)
		{
			this.SetItemActive(false);
			return;
		}
		if (this.currentState == ItemEquippable.ItemState.Idle)
		{
			this.SetItemActive(true);
			return;
		}
		if (this.currentState == ItemEquippable.ItemState.Unequipping)
		{
			base.StartCoroutine(this.AnimateUnequip());
		}
	}

	// Token: 0x06000B45 RID: 2885 RVA: 0x000642D3 File Offset: 0x000624D3
	private void SetItemActive(bool isActive)
	{
	}

	// Token: 0x06000B46 RID: 2886 RVA: 0x000642D5 File Offset: 0x000624D5
	private IEnumerator AnimateUnequip()
	{
		float duration = 0.2f;
		float elapsed = 0f;
		Vector3 originalScale = base.transform.localScale;
		Vector3 targetScale = Vector3.one;
		List<Collider> colliders = new List<Collider>();
		colliders.AddRange(base.GetComponents<Collider>());
		colliders.AddRange(base.GetComponentsInChildren<Collider>());
		if (SemiFunc.IsMasterClientOrSingleplayer())
		{
			this.physGrabObject.OverrideMass(0.1f, 0.1f);
		}
		this.isUnequipping = true;
		this.isUnequippingTimer = 0.2f;
		Collider _unequipCollider = null;
		bool _hasUnequipCollider = false;
		foreach (Collider collider in colliders)
		{
			PhysGrabObjectBoxCollider component = collider.GetComponent<PhysGrabObjectBoxCollider>();
			if (component && component.unEquipCollider)
			{
				collider.enabled = true;
				_hasUnequipCollider = true;
				_unequipCollider = collider;
				colliders.Remove(collider);
				break;
			}
		}
		if (_hasUnequipCollider)
		{
			using (List<Collider>.Enumerator enumerator = colliders.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Collider collider2 = enumerator.Current;
					collider2.enabled = false;
				}
				goto IL_1F2;
			}
		}
		using (List<Collider>.Enumerator enumerator = colliders.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Collider collider3 = enumerator.Current;
				collider3.enabled = true;
			}
			goto IL_1F2;
		}
		IL_19C:
		float t = elapsed / duration;
		base.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
		elapsed += Time.deltaTime;
		yield return null;
		IL_1F2:
		if (elapsed >= duration)
		{
			if (_hasUnequipCollider)
			{
				_unequipCollider.enabled = false;
				foreach (Collider collider4 in colliders)
				{
					collider4.enabled = true;
				}
			}
			this.isUnequipping = false;
			this.isUnequippingTimer = 0f;
			base.transform.localScale = targetScale;
			this.ForceGrab();
			this.forceGrabTimer = 0.2f;
			yield break;
		}
		goto IL_19C;
	}

	// Token: 0x06000B47 RID: 2887 RVA: 0x000642E4 File Offset: 0x000624E4
	private void ForceGrab()
	{
		if (!SemiFunc.IsMultiplayer())
		{
			PhysGrabber.instance.OverrideGrab(this.physGrabObject);
			return;
		}
		if (PhysGrabber.instance.photonView.ViewID == this.ownerPlayerId)
		{
			PhysGrabber.instance.OverrideGrab(this.physGrabObject);
		}
	}

	// Token: 0x06000B48 RID: 2888 RVA: 0x00064330 File Offset: 0x00062530
	private IEnumerator AnimateEquip()
	{
		float duration = 0.1f;
		float elapsed = 0f;
		Vector3 originalScale = base.transform.localScale;
		Vector3 targetScale = originalScale * 0.01f;
		List<Collider> list = new List<Collider>();
		list.AddRange(base.GetComponents<Collider>());
		list.AddRange(base.GetComponentsInChildren<Collider>());
		this.isEquipping = true;
		this.isEquippingTimer = 0.2f;
		using (List<Collider>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Collider collider = enumerator.Current;
				collider.enabled = false;
			}
			goto IL_10F;
		}
		IL_BB:
		float t = elapsed / duration;
		base.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
		elapsed += Time.deltaTime;
		yield return null;
		IL_10F:
		if (elapsed >= duration)
		{
			this.isEquipping = false;
			this.isEquippingTimer = 0f;
			base.transform.localScale = targetScale;
			yield break;
		}
		goto IL_BB;
	}

	// Token: 0x06000B49 RID: 2889 RVA: 0x00064340 File Offset: 0x00062540
	public void ForceUnequip(Vector3 dropPosition, int physGrabberPhotonViewID)
	{
		if (this.currentState == ItemEquippable.ItemState.Idle)
		{
			return;
		}
		dropPosition += Random.insideUnitSphere * 0.2f;
		if (SemiFunc.IsMultiplayer())
		{
			base.photonView.RPC("RPC_ForceUnequip", RpcTarget.All, new object[]
			{
				dropPosition,
				physGrabberPhotonViewID
			});
			return;
		}
		this.RPC_ForceUnequip(dropPosition, physGrabberPhotonViewID);
	}

	// Token: 0x06000B4A RID: 2890 RVA: 0x000643A8 File Offset: 0x000625A8
	[PunRPC]
	private void RPC_ForceUnequip(Vector3 dropPosition, int physGrabberPhotonViewID)
	{
		PlayerAvatar playerAvatar = PlayerAvatar.instance;
		if (SemiFunc.IsMultiplayer())
		{
			PhotonView photonView = PhotonView.Find(physGrabberPhotonViewID);
			playerAvatar = ((photonView != null) ? photonView.GetComponent<PlayerAvatar>() : null);
		}
		if (this.currentState == ItemEquippable.ItemState.Idle)
		{
			return;
		}
		this.ownerPlayerId = -1;
		this.currentState = ItemEquippable.ItemState.Unequipping;
		StatsManager.instance.PlayerInventoryUpdate(playerAvatar.steamID, "", this.inventorySpotIndex, true);
		if (this.equippedSpot)
		{
			this.equippedSpot.UnequipItem();
			this.equippedSpot = null;
		}
		this.UpdateVisuals();
		this.physGrabObject.OverrideDeactivateReset();
		this.physGrabObject.Teleport(dropPosition, Quaternion.identity);
		base.StartCoroutine(this.AnimateUnequip());
		this.SetItemActive(true);
	}

	// Token: 0x06000B4B RID: 2891 RVA: 0x00064460 File Offset: 0x00062660
	private void WasEquippedTimer()
	{
		if (this.isEquippedPrev != this.isEquipped)
		{
			this.wasEquippedTimer = 0.5f;
			this.isEquippedPrev = this.isEquipped;
		}
		if (this.wasEquippedTimer > 0f)
		{
			this.wasEquippedTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06000B4C RID: 2892 RVA: 0x000644B4 File Offset: 0x000626B4
	private void Update()
	{
		if (SemiFunc.RunIsArena())
		{
			return;
		}
		this.WasEquippedTimer();
		this.IsEquippingAndUnequippingTimer();
		switch (this.currentState)
		{
		case ItemEquippable.ItemState.Idle:
			this.StateIdle();
			break;
		case ItemEquippable.ItemState.Equipping:
			this.StateEquipping();
			break;
		case ItemEquippable.ItemState.Equipped:
			this.StateEquipped();
			break;
		case ItemEquippable.ItemState.Unequipping:
			this.StateUnequipping();
			break;
		}
		if (this.unequipTimer > 0f)
		{
			this.unequipTimer -= Time.deltaTime;
		}
		if (this.equipTimer > 0f)
		{
			this.equipTimer -= Time.deltaTime;
		}
		if (this.itemEquipCubeShowTimer > 0f)
		{
			this.itemEquipCubeShowTimer -= Time.deltaTime;
			if (this.itemEquipCubeShowTimer <= 0f)
			{
				Vector3 localScale = base.transform.localScale;
				base.transform.localScale = Vector3.one;
				base.transform.localScale = localScale;
			}
		}
	}

	// Token: 0x06000B4D RID: 2893 RVA: 0x000645A4 File Offset: 0x000627A4
	private void StateIdleStart()
	{
		if (!this.stateStart)
		{
			return;
		}
		this.stateStart = false;
	}

	// Token: 0x06000B4E RID: 2894 RVA: 0x000645B8 File Offset: 0x000627B8
	private void StateIdle()
	{
		if (this.currentState != ItemEquippable.ItemState.Idle)
		{
			return;
		}
		this.StateIdleStart();
		this.isEquipped = false;
		if (this.forceGrabTimer > 0f)
		{
			this.forceGrabTimer -= Time.deltaTime;
			if (this.forceGrabTimer <= 0f)
			{
				this.ForceGrab();
			}
		}
	}

	// Token: 0x06000B4F RID: 2895 RVA: 0x0006460D File Offset: 0x0006280D
	private void StateEquippingStart()
	{
		if (!this.stateStart)
		{
			return;
		}
		this.stateStart = false;
	}

	// Token: 0x06000B50 RID: 2896 RVA: 0x0006461F File Offset: 0x0006281F
	private void StateEquipping()
	{
		if (this.currentState != ItemEquippable.ItemState.Equipping)
		{
			return;
		}
		this.StateEquippingStart();
		this.currentState = ItemEquippable.ItemState.Equipped;
		this.isEquipped = true;
	}

	// Token: 0x06000B51 RID: 2897 RVA: 0x00064640 File Offset: 0x00062840
	private void StateEquippedStart()
	{
		if (!this.stateStart)
		{
			return;
		}
		this.stateStart = false;
		AssetManager.instance.soundEquip.Play(this.physGrabObject.midPoint, 1f, 1f, 1f, 1f);
		base.StartCoroutine(this.AnimateEquip());
	}

	// Token: 0x06000B52 RID: 2898 RVA: 0x0006469C File Offset: 0x0006289C
	private void StateEquipped()
	{
		if (this.currentState != ItemEquippable.ItemState.Equipped)
		{
			return;
		}
		this.StateEquippedStart();
		foreach (PhysGrabber physGrabber in this.physGrabObject.playerGrabbing)
		{
			physGrabber.OverrideGrabRelease();
		}
		if (!this.isEquipped)
		{
			this.equipTimer = 0.5f;
		}
		this.isEquipped = true;
		if (this.physGrabObject.transform.localScale.magnitude < 0.1f)
		{
			this.physGrabObject.OverrideDeactivate(0.1f);
		}
	}

	// Token: 0x06000B53 RID: 2899 RVA: 0x0006474C File Offset: 0x0006294C
	private void StateUnequippingStart()
	{
		if (!this.stateStart)
		{
			return;
		}
		this.stateStart = false;
	}

	// Token: 0x06000B54 RID: 2900 RVA: 0x00064760 File Offset: 0x00062960
	private void StateUnequipping()
	{
		AssetManager.instance.soundUnequip.Play(this.physGrabObject.midPoint, 1f, 1f, 1f, 1f);
		if (this.currentState != ItemEquippable.ItemState.Unequipping)
		{
			return;
		}
		this.currentState = ItemEquippable.ItemState.Idle;
		this.isEquipped = false;
	}

	// Token: 0x06000B55 RID: 2901 RVA: 0x000647B4 File Offset: 0x000629B4
	private void SetRotation()
	{
		this.physGrabObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
		this.physGrabObject.rb.rotation = this.physGrabObject.transform.rotation;
	}

	// Token: 0x06000B56 RID: 2902 RVA: 0x0006480A File Offset: 0x00062A0A
	private void OnDestroy()
	{
		if (this.equippedSpot)
		{
			this.equippedSpot.UnequipItem();
		}
	}

	// Token: 0x0400123C RID: 4668
	[FormerlySerializedAs("_currentState")]
	[SerializeField]
	private ItemEquippable.ItemState currentState;

	// Token: 0x0400123D RID: 4669
	public Sprite ItemIcon;

	// Token: 0x0400123E RID: 4670
	private InventorySpot equippedSpot;

	// Token: 0x0400123F RID: 4671
	private int ownerPlayerId = -1;

	// Token: 0x04001240 RID: 4672
	internal bool isEquipped;

	// Token: 0x04001241 RID: 4673
	internal bool isEquippedPrev;

	// Token: 0x04001242 RID: 4674
	internal float wasEquippedTimer;

	// Token: 0x04001243 RID: 4675
	internal bool isUnequipping;

	// Token: 0x04001244 RID: 4676
	internal bool isEquipping;

	// Token: 0x04001245 RID: 4677
	private float isUnequippingTimer;

	// Token: 0x04001246 RID: 4678
	private float isEquippingTimer;

	// Token: 0x04001247 RID: 4679
	public LayerMask ObstructionLayers;

	// Token: 0x04001248 RID: 4680
	public SemiFunc.emojiIcon itemEmojiIcon;

	// Token: 0x04001249 RID: 4681
	internal string itemEmoji;

	// Token: 0x0400124A RID: 4682
	internal int inventorySpotIndex;

	// Token: 0x0400124B RID: 4683
	internal float unequipTimer;

	// Token: 0x0400124C RID: 4684
	internal float equipTimer;

	// Token: 0x0400124D RID: 4685
	private const float animationDuration = 0.4f;

	// Token: 0x0400124E RID: 4686
	private PhysGrabObject physGrabObject;

	// Token: 0x0400124F RID: 4687
	private bool stateStart = true;

	// Token: 0x04001250 RID: 4688
	private float itemEquipCubeShowTimer;

	// Token: 0x04001251 RID: 4689
	private Vector3 teleportPosition;

	// Token: 0x04001252 RID: 4690
	private float forceGrabTimer;

	// Token: 0x04001253 RID: 4691
	internal PhysGrabber latestOwner;

	// Token: 0x02000341 RID: 833
	public enum ItemState
	{
		// Token: 0x040026C6 RID: 9926
		Idle,
		// Token: 0x040026C7 RID: 9927
		Equipping,
		// Token: 0x040026C8 RID: 9928
		Equipped,
		// Token: 0x040026C9 RID: 9929
		Unequipping
	}
}
