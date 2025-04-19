using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200017A RID: 378
public class MapBacktrack : MonoBehaviour
{
	// Token: 0x06000CB1 RID: 3249 RVA: 0x0006FEF8 File Offset: 0x0006E0F8
	private void Start()
	{
		this.path = new NavMeshPath();
		for (int i = 0; i < this.amount; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.pointPrefab, base.transform);
			this.points.Add(gameObject.GetComponent<MapBacktrackPoint>());
			gameObject.transform.name = string.Format("Point {0}", i);
		}
		base.StartCoroutine(this.Backtrack());
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0006FF6C File Offset: 0x0006E16C
	private IEnumerator Backtrack()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.5f);
		foreach (LevelPoint levelPoint in LevelGenerator.Instance.LevelPathPoints)
		{
			if (levelPoint.Room.Truck)
			{
				this.truckDestination = levelPoint.transform.position;
				break;
			}
		}
		for (;;)
		{
			Vector3 lastNavmeshPosition = PlayerController.instance.playerAvatarScript.LastNavmeshPosition;
			Vector3 vector = lastNavmeshPosition;
			if (RoundDirector.instance.allExtractionPointsCompleted)
			{
				vector = this.truckDestination;
			}
			else if (RoundDirector.instance.extractionPointCurrent)
			{
				vector = RoundDirector.instance.extractionPointCurrent.transform.position;
			}
			bool flag = false;
			if (Map.Instance.Active)
			{
				MapLayer layerParent = Map.Instance.GetLayerParent(lastNavmeshPosition.y + 1f);
				MapLayer layerParent2 = Map.Instance.GetLayerParent(vector.y + 1f);
				if (layerParent.layer == layerParent2.layer)
				{
					flag = true;
				}
			}
			if (!Map.Instance.Active || (flag && Vector3.Distance(lastNavmeshPosition, vector) < 10f))
			{
				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				NavMesh.CalculatePath(lastNavmeshPosition, vector, -1, this.path);
				this.currentPoint = 0;
				this.currentPointPosition = lastNavmeshPosition;
				this.currentPointCorner = 0;
				while (this.currentPoint < this.points.Count)
				{
					bool flag2 = false;
					float num = this.spacing;
					while (!flag2 && this.currentPointCorner < this.path.corners.Length)
					{
						float num2 = Vector3.Distance(this.currentPointPosition, this.path.corners[this.currentPointCorner]);
						if (num2 < num)
						{
							this.currentPointPosition = this.path.corners[this.currentPointCorner];
							num -= num2;
							this.currentPointCorner++;
						}
						else
						{
							this.currentPointPosition = Vector3.Lerp(this.currentPointPosition, this.path.corners[this.currentPointCorner], num / num2);
							if (Map.Instance.GetLayerParent(this.currentPointPosition.y + 1f).layer == Map.Instance.PlayerLayer)
							{
								this.points[this.currentPoint].Show(true);
							}
							else
							{
								this.points[this.currentPoint].Show(false);
							}
							Vector3 a = new Vector3(this.currentPointPosition.x, 0f, this.currentPointPosition.z);
							this.points[this.currentPoint].transform.position = a * Map.Instance.Scale + Map.Instance.OverLayerParent.position;
							this.currentPoint++;
							flag2 = true;
						}
					}
					if (this.currentPointCorner >= this.path.corners.Length)
					{
						this.currentPoint = this.points.Count;
					}
					yield return new WaitForSeconds(this.pointWait);
				}
				foreach (MapBacktrackPoint _point in this.points)
				{
					while (_point.animating)
					{
						yield return new WaitForSeconds(0.05f);
					}
					_point = null;
				}
				List<MapBacktrackPoint>.Enumerator enumerator2 = default(List<MapBacktrackPoint>.Enumerator);
				yield return new WaitForSeconds(this.pointWait);
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x0400143F RID: 5183
	public GameObject pointPrefab;

	// Token: 0x04001440 RID: 5184
	private List<MapBacktrackPoint> points = new List<MapBacktrackPoint>();

	// Token: 0x04001441 RID: 5185
	[Space]
	public int amount;

	// Token: 0x04001442 RID: 5186
	public float spacing;

	// Token: 0x04001443 RID: 5187
	public float pointWait;

	// Token: 0x04001444 RID: 5188
	public float resetWait;

	// Token: 0x04001445 RID: 5189
	private int currentPoint;

	// Token: 0x04001446 RID: 5190
	private Vector3 currentPointPosition;

	// Token: 0x04001447 RID: 5191
	private int currentPointCorner;

	// Token: 0x04001448 RID: 5192
	private Vector3 truckDestination;

	// Token: 0x04001449 RID: 5193
	private NavMeshPath path;
}
