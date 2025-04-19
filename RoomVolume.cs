using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class RoomVolume : MonoBehaviour
{
	// Token: 0x06000816 RID: 2070 RVA: 0x0004E6A8 File Offset: 0x0004C8A8
	private void Start()
	{
		this.Module = base.GetComponentInParent<Module>();
		RoomVolume[] componentsInParent = base.GetComponentsInParent<RoomVolume>();
		for (int i = 0; i < componentsInParent.Length; i++)
		{
			if (componentsInParent[i] != this)
			{
				Object.Destroy(this);
				return;
			}
		}
		base.StartCoroutine(this.Setup());
	}

	// Token: 0x06000817 RID: 2071 RVA: 0x0004E6F5 File Offset: 0x0004C8F5
	private IEnumerator Setup()
	{
		yield return new WaitForSeconds(0.1f);
		foreach (BoxCollider boxCollider in base.GetComponentsInChildren<BoxCollider>())
		{
			Vector3 halfExtents = boxCollider.size * 0.5f;
			halfExtents.x *= Mathf.Abs(boxCollider.transform.lossyScale.x);
			halfExtents.y *= Mathf.Abs(boxCollider.transform.lossyScale.y);
			halfExtents.z *= Mathf.Abs(boxCollider.transform.lossyScale.z);
			Collider[] array = Physics.OverlapBox(boxCollider.transform.TransformPoint(boxCollider.center), halfExtents, boxCollider.transform.rotation, LayerMask.GetMask(new string[]
			{
				"Other"
			}), QueryTriggerInteraction.Collide);
			for (int j = 0; j < array.Length; j++)
			{
				LevelPoint component = array[j].transform.GetComponent<LevelPoint>();
				if (component)
				{
					component.Room = this;
				}
			}
		}
		if (!this.Extraction && !this.Truck && !this.Module.StartRoom && !SemiFunc.RunIsShop())
		{
			foreach (BoxCollider boxCollider2 in base.GetComponentsInChildren<BoxCollider>())
			{
				Vector3 scale = boxCollider2.size * 0.5f;
				scale.x *= Mathf.Abs(boxCollider2.transform.lossyScale.x);
				scale.y *= Mathf.Abs(boxCollider2.transform.lossyScale.y);
				scale.z *= Mathf.Abs(boxCollider2.transform.lossyScale.z);
				Vector3 position = boxCollider2.transform.TransformPoint(boxCollider2.center);
				Quaternion rotation = boxCollider2.transform.rotation;
				this.MapModule = Map.Instance.AddRoomVolume(base.gameObject, position, rotation, scale, this.Module);
			}
		}
		yield break;
	}

	// Token: 0x06000818 RID: 2072 RVA: 0x0004E704 File Offset: 0x0004C904
	public void SetExplored()
	{
		if (this.Explored)
		{
			return;
		}
		this.Explored = true;
		if (this.MapModule)
		{
			this.MapModule.Hide();
		}
	}

	// Token: 0x04000EE0 RID: 3808
	public bool Truck;

	// Token: 0x04000EE1 RID: 3809
	public bool Extraction;

	// Token: 0x04000EE2 RID: 3810
	public Color Color = Color.blue;

	// Token: 0x04000EE3 RID: 3811
	[Space]
	public ReverbPreset ReverbPreset;

	// Token: 0x04000EE4 RID: 3812
	public RoomAmbience RoomAmbience;

	// Token: 0x04000EE5 RID: 3813
	public Module Module;

	// Token: 0x04000EE6 RID: 3814
	public MapModule MapModule;

	// Token: 0x04000EE7 RID: 3815
	private bool Explored;
}
