using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class PlayerNameChecker : MonoBehaviour
{
	// Token: 0x06000EF9 RID: 3833 RVA: 0x00088B00 File Offset: 0x00086D00
	private void Update()
	{
		if (GameDirector.instance.currentState != GameDirector.gameState.Main || (Map.Instance && Map.Instance.Active) || !GameplayManager.instance.playerNames)
		{
			return;
		}
		if (this.checkTimer <= 0f)
		{
			this.checkTimer = 0.25f;
			List<PlayerAvatar> list = new List<PlayerAvatar>();
			Camera main = Camera.main;
			foreach (RaycastHit raycastHit in Physics.SphereCastAll(main.transform.position, 0.25f, main.transform.forward, 15f, LayerMask.GetMask(new string[]
			{
				"PlayerVisuals"
			}), QueryTriggerInteraction.Collide))
			{
				PlayerAvatarVisuals componentInParent = raycastHit.collider.GetComponentInParent<PlayerAvatarVisuals>();
				if (componentInParent)
				{
					PlayerAvatar playerAvatar = componentInParent.playerAvatar;
					if (!list.Contains(playerAvatar))
					{
						Vector3 direction = main.transform.position - raycastHit.point;
						RaycastHit raycastHit2;
						if (!Physics.Raycast(raycastHit.point, direction, out raycastHit2, direction.magnitude, SemiFunc.LayerMaskGetVisionObstruct() - LayerMask.GetMask(new string[]
						{
							"Player"
						}), QueryTriggerInteraction.Collide))
						{
							playerAvatar.worldSpaceUIPlayerName.Show();
							list.Add(playerAvatar);
						}
					}
				}
			}
			return;
		}
		this.checkTimer -= Time.deltaTime;
	}

	// Token: 0x04001933 RID: 6451
	private float checkTimer;
}
