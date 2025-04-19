using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000216 RID: 534
public class CleanDirector : MonoBehaviour
{
	// Token: 0x06001149 RID: 4425 RVA: 0x0009A275 File Offset: 0x00098475
	private void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
		CleanDirector.instance = this;
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x0009A28C File Offset: 0x0009848C
	private void RandomlyRemoveExcessSpots()
	{
		if (PhotonNetwork.IsMasterClient || GameManager.instance.gameMode == 0)
		{
			foreach (CleanDirector.CleaningSpots cleaningSpots in this.cleaningSpots)
			{
				Interaction.InteractionType type = cleaningSpots.InteractionType;
				int cleaningSpotsMax = cleaningSpots.CleaningSpotsMax;
				Func<GameObject, bool> <>9__1;
				for (int i = this.CleanList.Count((GameObject spot) => spot.GetComponent<CleanSpotIdentifier>().InteractionType == type); i > cleaningSpotsMax; i--)
				{
					IEnumerable<GameObject> cleanList = this.CleanList;
					Func<GameObject, bool> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = ((GameObject spot) => spot.GetComponent<CleanSpotIdentifier>().InteractionType == type));
					}
					List<GameObject> list = cleanList.Where(predicate).ToList<GameObject>();
					int index = Random.Range(0, list.Count);
					GameObject gameObject = list[index];
					if (GameManager.instance.gameMode == 1)
					{
						if (gameObject.GetComponent<PhotonView>() == null)
						{
							Debug.LogWarning("Photon View not found for: " + gameObject.name);
						}
						this.CleanList.Remove(gameObject);
						PhotonNetwork.Destroy(gameObject);
					}
					else
					{
						this.CleanList.Remove(gameObject);
						Object.Destroy(gameObject);
					}
				}
			}
		}
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x0009A3F0 File Offset: 0x000985F0
	private void Update()
	{
		if (!this.RemoveExcessSpots && GameDirector.instance.currentState >= GameDirector.gameState.Start)
		{
			this.RandomlyRemoveExcessSpots();
			this.RemoveExcessSpots = true;
		}
	}

	// Token: 0x04001D1D RID: 7453
	public static CleanDirector instance;

	// Token: 0x04001D1E RID: 7454
	public List<GameObject> CleanList = new List<GameObject>();

	// Token: 0x04001D1F RID: 7455
	public List<CleanDirector.CleaningSpots> cleaningSpots;

	// Token: 0x04001D20 RID: 7456
	internal bool RemoveExcessSpots;

	// Token: 0x04001D21 RID: 7457
	private PhotonView photonView;

	// Token: 0x020003A1 RID: 929
	[Serializable]
	public class CleaningSpots
	{
		// Token: 0x0400282B RID: 10283
		public Interaction.InteractionType InteractionType;

		// Token: 0x0400282C RID: 10284
		public int CleaningSpotsMax;
	}
}
