using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class RoomVolumeCheck : MonoBehaviour
{
	// Token: 0x0600081A RID: 2074 RVA: 0x0004E741 File Offset: 0x0004C941
	private void Awake()
	{
		if (base.GetComponentInParent<PlayerAvatar>())
		{
			this.player = true;
		}
		this.Mask = LayerMask.GetMask(new string[]
		{
			"RoomVolume"
		});
		this.CheckStart();
	}

	// Token: 0x0600081B RID: 2075 RVA: 0x0004E77B File Offset: 0x0004C97B
	private void OnEnable()
	{
		this.CheckStart();
	}

	// Token: 0x0600081C RID: 2076 RVA: 0x0004E783 File Offset: 0x0004C983
	private void OnDisable()
	{
		this.checkActive = false;
		base.StopAllCoroutines();
	}

	// Token: 0x0600081D RID: 2077 RVA: 0x0004E794 File Offset: 0x0004C994
	public void CheckSet()
	{
		this.inTruck = false;
		this.inExtractionPoint = false;
		this.CurrentRooms.Clear();
		Vector3 localScale = this.currentSize;
		if (localScale == Vector3.zero)
		{
			localScale = base.transform.localScale;
		}
		foreach (Collider collider in Physics.OverlapBox(base.transform.position + base.transform.rotation * this.CheckPosition, localScale / 2f, base.transform.rotation, this.Mask))
		{
			RoomVolume roomVolume = collider.transform.GetComponent<RoomVolume>();
			if (!roomVolume)
			{
				roomVolume = collider.transform.GetComponentInParent<RoomVolume>();
			}
			if (!this.CurrentRooms.Contains(roomVolume))
			{
				this.CurrentRooms.Add(roomVolume);
			}
			if (roomVolume.Truck)
			{
				this.inTruck = true;
			}
			if (roomVolume.Extraction)
			{
				this.inExtractionPoint = true;
			}
		}
		if (this.player && this.CurrentRooms.Count > 0)
		{
			bool flag = true;
			MapModule mapModule = this.CurrentRooms[0].MapModule;
			foreach (RoomVolume roomVolume2 in this.CurrentRooms)
			{
				if (mapModule != roomVolume2.MapModule)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				this.CurrentRooms[0].SetExplored();
			}
		}
	}

	// Token: 0x0600081E RID: 2078 RVA: 0x0004E930 File Offset: 0x0004CB30
	private IEnumerator Check()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.5f);
		for (;;)
		{
			if (this.PauseCheckTimer > 0f)
			{
				this.PauseCheckTimer -= 0.5f;
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				this.CheckSet();
				if (!this.Continuous)
				{
					break;
				}
				if (this.player)
				{
					yield return new WaitForSeconds(0.1f);
				}
				else
				{
					yield return new WaitForSeconds(0.5f);
				}
			}
		}
		yield break;
	}

	// Token: 0x0600081F RID: 2079 RVA: 0x0004E93F File Offset: 0x0004CB3F
	private void CheckStart()
	{
		if (!this.checkActive)
		{
			this.checkActive = true;
			base.StartCoroutine(this.Check());
		}
	}

	// Token: 0x04000EE8 RID: 3816
	public List<RoomVolume> CurrentRooms;

	// Token: 0x04000EE9 RID: 3817
	public bool Continuous = true;

	// Token: 0x04000EEA RID: 3818
	internal float PauseCheckTimer;

	// Token: 0x04000EEB RID: 3819
	private LayerMask Mask;

	// Token: 0x04000EEC RID: 3820
	[Space]
	public bool DebugCheckPosition;

	// Token: 0x04000EED RID: 3821
	public Vector3 CheckPosition = Vector3.one;

	// Token: 0x04000EEE RID: 3822
	public Vector3 currentSize = Vector3.one;

	// Token: 0x04000EEF RID: 3823
	internal bool inTruck;

	// Token: 0x04000EF0 RID: 3824
	internal bool inExtractionPoint;

	// Token: 0x04000EF1 RID: 3825
	private bool player;

	// Token: 0x04000EF2 RID: 3826
	private bool checkActive;
}
