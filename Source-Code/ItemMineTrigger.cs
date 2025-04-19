using System;
using UnityEngine;

// Token: 0x02000158 RID: 344
public class ItemMineTrigger : MonoBehaviour
{
	// Token: 0x06000B82 RID: 2946 RVA: 0x00066088 File Offset: 0x00064288
	private void Start()
	{
		this.parentPhysGrabObject = base.GetComponentInParent<PhysGrabObject>();
		this.itemMine = base.GetComponentInParent<ItemMine>();
		if (!SemiFunc.IsMasterClientOrSingleplayer())
		{
			Object.Destroy(this);
			return;
		}
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x000660B0 File Offset: 0x000642B0
	private void OnTriggerEnter(Collider other)
	{
		if (this.targetAcquired || !this.itemMine || this.itemMine.state != ItemMine.States.Armed)
		{
			return;
		}
		if (!this.PassesTriggerChecks(other))
		{
			return;
		}
		this.TryAcquireTarget(other);
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x000660E8 File Offset: 0x000642E8
	private void OnTriggerStay(Collider other)
	{
		if (this.targetAcquired || !this.itemMine || this.itemMine.state != ItemMine.States.Armed)
		{
			return;
		}
		if (!this.PassesTriggerChecks(other))
		{
			return;
		}
		this.visionCheckTimer += Time.deltaTime;
		if (this.visionCheckTimer > 0.5f)
		{
			this.visionCheckTimer = 0f;
			this.TryAcquireTarget(other);
		}
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x00066154 File Offset: 0x00064354
	private bool PassesTriggerChecks(Collider other)
	{
		PhysGrabObject componentInParent = other.GetComponentInParent<PhysGrabObject>();
		if (this.enemyTrigger)
		{
			if (!componentInParent || !componentInParent.isEnemy)
			{
				return false;
			}
		}
		else if (componentInParent && componentInParent.isEnemy && !this.itemMine.triggeredByEnemies)
		{
			return false;
		}
		if (componentInParent && !this.itemMine.triggeredByRigidBodies && !componentInParent.isEnemy && !componentInParent.isPlayer)
		{
			return false;
		}
		PlayerAvatar exists = other.GetComponentInParent<PlayerAvatar>();
		PlayerController componentInParent2 = other.GetComponentInParent<PlayerController>();
		if (componentInParent2)
		{
			exists = componentInParent2.playerAvatarScript;
		}
		return (!componentInParent || this.itemMine.triggeredByPlayers || (!componentInParent.isPlayer && !exists)) && (!componentInParent || componentInParent.isEnemy || componentInParent.grabbed || componentInParent.rb.velocity.magnitude >= 0.1f || componentInParent.rb.angularVelocity.magnitude >= 0.1f) && (!(componentInParent ? componentInParent.GetComponent<PlayerTumble>() : null) || this.itemMine.triggeredByPlayers);
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00066284 File Offset: 0x00064484
	private void TryAcquireTarget(Collider other)
	{
		if (this.targetAcquired)
		{
			return;
		}
		PhysGrabObject componentInParent = other.GetComponentInParent<PhysGrabObject>();
		PlayerAvatar playerAvatar = other.GetComponentInParent<PlayerAvatar>();
		PlayerAccess componentInParent2 = other.GetComponentInParent<PlayerAccess>();
		PlayerController playerController = componentInParent2 ? componentInParent2.GetComponentInChildren<PlayerController>() : null;
		Vector3 position = this.itemMine.transform.position;
		if (componentInParent)
		{
			Vector3 midPoint = componentInParent.midPoint;
			if (!this.VisionObstruct(position, midPoint, componentInParent))
			{
				if (componentInParent.isEnemy)
				{
					this.LockOnTarget(ItemMineTrigger.TargetType.Enemy, componentInParent, playerAvatar, playerController);
					return;
				}
				if (!componentInParent.isPlayer && componentInParent != this.parentPhysGrabObject)
				{
					this.LockOnTarget(ItemMineTrigger.TargetType.RigidBody, componentInParent, playerAvatar, playerController);
					return;
				}
			}
		}
		if (playerAvatar)
		{
			Vector3 position2 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
			if (!this.VisionObstruct(position, position2, null))
			{
				this.LockOnTarget(ItemMineTrigger.TargetType.Player, componentInParent, playerAvatar, playerController);
				return;
			}
		}
		if (playerController)
		{
			playerAvatar = playerController.playerAvatarScript;
			if (playerAvatar)
			{
				Vector3 position3 = playerAvatar.PlayerVisionTarget.VisionTransform.position;
				if (!this.VisionObstruct(position, position3, null))
				{
					this.LockOnTarget(ItemMineTrigger.TargetType.Player, componentInParent, playerAvatar, playerController);
					return;
				}
			}
		}
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x00066398 File Offset: 0x00064598
	private void LockOnTarget(ItemMineTrigger.TargetType type, PhysGrabObject physObj, PlayerAvatar playerAvatar, PlayerController playerController)
	{
		if (!this.itemMine)
		{
			return;
		}
		switch (type)
		{
		case ItemMineTrigger.TargetType.Enemy:
			this.itemMine.wasTriggeredByEnemy = true;
			this.itemMine.triggeredPhysGrabObject = physObj;
			this.itemMine.triggeredTransform = physObj.transform;
			this.itemMine.triggeredPosition = physObj.transform.position;
			break;
		case ItemMineTrigger.TargetType.RigidBody:
			this.itemMine.wasTriggeredByRigidBody = true;
			this.itemMine.triggeredPhysGrabObject = physObj;
			this.itemMine.triggeredTransform = physObj.transform;
			this.itemMine.triggeredPosition = physObj.transform.position;
			break;
		case ItemMineTrigger.TargetType.Player:
			this.itemMine.wasTriggeredByPlayer = true;
			if (playerAvatar)
			{
				this.itemMine.triggeredPlayerAvatar = playerAvatar;
				PlayerTumble tumble = playerAvatar.tumble;
				if (tumble)
				{
					this.itemMine.triggeredPlayerTumble = tumble;
					this.itemMine.triggeredPhysGrabObject = tumble.physGrabObject;
				}
				this.itemMine.triggeredTransform = playerAvatar.PlayerVisionTarget.VisionTransform;
				this.itemMine.triggeredPosition = playerAvatar.PlayerVisionTarget.VisionTransform.position;
			}
			else if (physObj)
			{
				PlayerTumble componentInParent = physObj.GetComponentInParent<PlayerTumble>();
				if (componentInParent)
				{
					this.itemMine.triggeredPlayerAvatar = componentInParent.playerAvatar;
					this.itemMine.triggeredPlayerTumble = componentInParent;
					this.itemMine.triggeredPhysGrabObject = componentInParent.physGrabObject;
					this.itemMine.triggeredTransform = componentInParent.playerAvatar.PlayerVisionTarget.VisionTransform;
					this.itemMine.triggeredPosition = componentInParent.playerAvatar.PlayerVisionTarget.VisionTransform.position;
				}
			}
			break;
		}
		this.targetAcquired = true;
		this.itemMine.SetTriggered();
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x00066568 File Offset: 0x00064768
	private bool VisionObstruct(Vector3 start, Vector3 end, PhysGrabObject targetPhysObj)
	{
		int layerMask = SemiFunc.LayerMaskGetVisionObstruct();
		Vector3 normalized = (end - start).normalized;
		float maxDistance = Vector3.Distance(start, end);
		foreach (RaycastHit raycastHit in Physics.RaycastAll(start, normalized, maxDistance, layerMask))
		{
			if (raycastHit.collider.CompareTag("Wall") || raycastHit.collider.CompareTag("Ceiling"))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040012AB RID: 4779
	private PhysGrabObject parentPhysGrabObject;

	// Token: 0x040012AC RID: 4780
	private ItemMine itemMine;

	// Token: 0x040012AD RID: 4781
	public bool enemyTrigger;

	// Token: 0x040012AE RID: 4782
	private bool targetAcquired;

	// Token: 0x040012AF RID: 4783
	private float visionCheckTimer;

	// Token: 0x02000346 RID: 838
	private enum TargetType
	{
		// Token: 0x040026E1 RID: 9953
		None,
		// Token: 0x040026E2 RID: 9954
		Enemy,
		// Token: 0x040026E3 RID: 9955
		RigidBody,
		// Token: 0x040026E4 RID: 9956
		Player
	}
}
