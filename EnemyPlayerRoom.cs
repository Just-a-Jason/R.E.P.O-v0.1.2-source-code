using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000A1 RID: 161
public class EnemyPlayerRoom : MonoBehaviour
{
	// Token: 0x0600062B RID: 1579 RVA: 0x0003B2C5 File Offset: 0x000394C5
	private void Start()
	{
		this.LogicActive = true;
		base.StartCoroutine(this.Logic());
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x0003B2DB File Offset: 0x000394DB
	private void OnEnable()
	{
		if (!this.LogicActive)
		{
			this.LogicActive = true;
			base.StartCoroutine(this.Logic());
		}
	}

	// Token: 0x0600062D RID: 1581 RVA: 0x0003B2F9 File Offset: 0x000394F9
	private void OnDisable()
	{
		this.LogicActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x0600062E RID: 1582 RVA: 0x0003B308 File Offset: 0x00039508
	private IEnumerator Logic()
	{
		while (GameDirector.instance.PlayerList.Count == 0)
		{
			yield return new WaitForSeconds(1f);
		}
		for (;;)
		{
			this.SameAny = false;
			this.SameLocal = false;
			foreach (RoomVolume x in this.RoomVolumeCheck.CurrentRooms)
			{
				foreach (PlayerAvatar playerAvatar in GameDirector.instance.PlayerList)
				{
					foreach (RoomVolume y in playerAvatar.RoomVolumeCheck.CurrentRooms)
					{
						if (x == y)
						{
							if (!playerAvatar.isDisabled)
							{
								this.SameAny = true;
							}
							if (playerAvatar.isLocal)
							{
								this.SameLocal = true;
								break;
							}
						}
					}
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x04000A1F RID: 2591
	public RoomVolumeCheck RoomVolumeCheck;

	// Token: 0x04000A20 RID: 2592
	private bool LogicActive;

	// Token: 0x04000A21 RID: 2593
	internal bool SameAny;

	// Token: 0x04000A22 RID: 2594
	internal bool SameLocal;
}
