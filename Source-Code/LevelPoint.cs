using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020000E6 RID: 230
public class LevelPoint : MonoBehaviour
{
	// Token: 0x06000812 RID: 2066 RVA: 0x0004E614 File Offset: 0x0004C814
	private void Start()
	{
		LevelGenerator.Instance.LevelPathPoints.Add(this);
		if (this.Truck)
		{
			LevelGenerator.Instance.LevelPathTruck = this;
		}
		if (base.GetComponentInParent<StartRoom>())
		{
			this.inStartRoom = true;
		}
		if (this.ModuleConnect)
		{
			base.StartCoroutine(this.ModuleConnectSetup());
		}
		base.StartCoroutine(this.NavMeshCheck());
	}

	// Token: 0x06000813 RID: 2067 RVA: 0x0004E67A File Offset: 0x0004C87A
	private IEnumerator NavMeshCheck()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.5f);
		bool flag = false;
		NavMeshHit navMeshHit;
		if (!NavMesh.SamplePosition(base.transform.position, out navMeshHit, 0.5f, -1))
		{
			flag = true;
			Debug.LogError("Level Point not on Navmesh! Fix!", base.gameObject);
		}
		if (!this.Room)
		{
			flag = true;
			Debug.LogError("Level Point did not find a room volume!! Fix!!!", base.gameObject);
		}
		foreach (LevelPoint levelPoint in this.ConnectedPoints)
		{
			if (!levelPoint)
			{
				flag = true;
				Debug.LogError("Level Point not fully connected! Fix!!", base.gameObject);
			}
			else
			{
				bool flag2 = false;
				using (List<LevelPoint>.Enumerator enumerator2 = levelPoint.ConnectedPoints.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current == this)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					flag = true;
					Debug.LogError("Level Point not fully connected! Fix!!", base.gameObject);
				}
			}
		}
		if (flag && Application.isEditor)
		{
			Object.Instantiate<GameObject>(AssetManager.instance.debugLevelPointError, base.transform.position, Quaternion.identity);
		}
		yield break;
	}

	// Token: 0x06000814 RID: 2068 RVA: 0x0004E689 File Offset: 0x0004C889
	private IEnumerator ModuleConnectSetup()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		float num = 999f;
		foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (levelPoint.ModuleConnect)
			{
				float num2 = Vector3.Distance(base.transform.position, levelPoint.transform.position);
				if (num2 < 15f && num2 < num && Vector3.Dot(levelPoint.transform.forward, base.transform.forward) <= -0.8f && Vector3.Dot(levelPoint.transform.forward, (base.transform.position - levelPoint.transform.position).normalized) > 0.8f)
				{
					num = num2;
					this.ConnectedPoints.Add(levelPoint);
				}
			}
		}
		this.ModuleConnected = true;
		yield break;
	}

	// Token: 0x04000ED7 RID: 3799
	public bool DebugMeshActive = true;

	// Token: 0x04000ED8 RID: 3800
	public Mesh DebugMesh;

	// Token: 0x04000ED9 RID: 3801
	internal bool inStartRoom;

	// Token: 0x04000EDA RID: 3802
	[Space]
	public bool ModuleConnect;

	// Token: 0x04000EDB RID: 3803
	public bool Truck;

	// Token: 0x04000EDC RID: 3804
	private bool ModuleConnected;

	// Token: 0x04000EDD RID: 3805
	public RoomVolume Room;

	// Token: 0x04000EDE RID: 3806
	[Space]
	public List<LevelPoint> ConnectedPoints;

	// Token: 0x04000EDF RID: 3807
	[HideInInspector]
	public List<LevelPoint> AllLevelPoints;
}
