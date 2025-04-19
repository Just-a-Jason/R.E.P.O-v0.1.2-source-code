using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class TrapDirector : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x06001189 RID: 4489 RVA: 0x0009BDF0 File Offset: 0x00099FF0
	private void Awake()
	{
		TrapDirector.instance = this;
	}

	// Token: 0x0600118A RID: 4490 RVA: 0x0009BDF8 File Offset: 0x00099FF8
	private void Start()
	{
		base.StartCoroutine(this.Generate());
	}

	// Token: 0x0600118B RID: 4491 RVA: 0x0009BE07 File Offset: 0x0009A007
	private void Update()
	{
		if (this.TrapCooldown > 0f)
		{
			this.TrapCooldown -= Time.deltaTime;
		}
	}

	// Token: 0x0600118C RID: 4492 RVA: 0x0009BE28 File Offset: 0x0009A028
	private IEnumerator Generate()
	{
		while (!LevelGenerator.Instance.Generated)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (GameManager.instance.gameMode == 0 || PhotonNetwork.IsMasterClient)
		{
			this.UpdateTrapList();
		}
		this.TrapListUpdated = true;
		yield break;
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x0009BE38 File Offset: 0x0009A038
	private void UpdateTrapList()
	{
		Dictionary<string, List<GameObject>> dictionary = new Dictionary<string, List<GameObject>>();
		foreach (GameObject gameObject in this.TrapList)
		{
			TrapTypeIdentifier component = gameObject.GetComponent<TrapTypeIdentifier>();
			if (component != null)
			{
				string trapType = component.trapType;
				if (!dictionary.ContainsKey(trapType))
				{
					dictionary[trapType] = new List<GameObject>();
				}
				dictionary[trapType].Add(gameObject);
			}
		}
		if (this.DebugAllTraps)
		{
			return;
		}
		foreach (KeyValuePair<string, List<GameObject>> keyValuePair in dictionary)
		{
			if (keyValuePair.Value.Count > 0)
			{
				GameObject item = keyValuePair.Value[Random.Range(0, keyValuePair.Value.Count)];
				this.SelectedTraps.Add(item);
			}
		}
		using (List<GameObject>.Enumerator enumerator = this.TrapList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GameObject gameObject2 = enumerator.Current;
				if (!this.SelectedTraps.Contains(gameObject2))
				{
					TrapTypeIdentifier component2 = gameObject2.GetComponent<TrapTypeIdentifier>();
					if (component2 != null)
					{
						if (component2.Trigger != null && component2.OnlyRemoveTrigger)
						{
							if (GameManager.instance.gameMode == 0)
							{
								Object.Destroy(component2.Trigger);
								component2.TriggerRemoved = true;
							}
							else
							{
								gameObject2.GetComponent<PhotonView>().RPC("DestroyTrigger", RpcTarget.AllBuffered, Array.Empty<object>());
							}
						}
						else if (GameManager.instance.gameMode == 0)
						{
							Object.Destroy(gameObject2);
						}
						else
						{
							gameObject2.GetComponent<PhotonView>().RPC("DestroyTrap", RpcTarget.AllBuffered, Array.Empty<object>());
						}
					}
				}
			}
			goto IL_278;
		}
		IL_1C0:
		int index = Random.Range(0, this.SelectedTraps.Count);
		GameObject gameObject3 = this.SelectedTraps[index];
		this.SelectedTraps.RemoveAt(index);
		TrapTypeIdentifier component3 = gameObject3.GetComponent<TrapTypeIdentifier>();
		if (component3 != null)
		{
			if (component3.Trigger != null)
			{
				if (GameManager.instance.gameMode == 0)
				{
					Object.Destroy(component3.Trigger);
					component3.TriggerRemoved = true;
				}
				else
				{
					gameObject3.GetComponent<PhotonView>().RPC("DestroyTrigger", RpcTarget.AllBuffered, Array.Empty<object>());
				}
			}
			else if (GameManager.instance.gameMode == 0)
			{
				Object.Destroy(gameObject3);
			}
			else
			{
				gameObject3.GetComponent<PhotonView>().RPC("DestroyTrap", RpcTarget.AllBuffered, Array.Empty<object>());
			}
		}
		IL_278:
		if (this.SelectedTraps.Count <= this.TrapCount)
		{
			return;
		}
		goto IL_1C0;
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x0009C0FC File Offset: 0x0009A2FC
	private string RandomType(Dictionary<string, List<GameObject>> trapsByType)
	{
		List<string> list = new List<string>(trapsByType.Keys);
		int index = Random.Range(0, list.Count);
		return list[index];
	}

	// Token: 0x0600118F RID: 4495 RVA: 0x0009C129 File Offset: 0x0009A329
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.TrapListUpdated);
			return;
		}
		this.TrapListUpdated = (bool)stream.ReceiveNext();
	}

	// Token: 0x04001D76 RID: 7542
	public static TrapDirector instance;

	// Token: 0x04001D77 RID: 7543
	public bool DebugAllTraps;

	// Token: 0x04001D78 RID: 7544
	[Space]
	public List<GameObject> TrapList = new List<GameObject>();

	// Token: 0x04001D79 RID: 7545
	public List<GameObject> SelectedTraps = new List<GameObject>();

	// Token: 0x04001D7A RID: 7546
	public float TrapCooldown;

	// Token: 0x04001D7B RID: 7547
	internal bool TrapListUpdated;

	// Token: 0x04001D7C RID: 7548
	public int TrapCount = 2;
}
