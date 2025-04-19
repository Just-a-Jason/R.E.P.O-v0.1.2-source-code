using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class Module : MonoBehaviourPunCallbacks, IPunObservable
{
	// Token: 0x060007FF RID: 2047 RVA: 0x0004E0BE File Offset: 0x0004C2BE
	private void Start()
	{
		if (base.GetComponent<StartRoom>())
		{
			this.StartRoom = true;
			return;
		}
		base.transform.parent = LevelGenerator.Instance.LevelParent.transform;
		base.StartCoroutine(this.ReadyCheck());
	}

	// Token: 0x06000800 RID: 2048 RVA: 0x0004E0FC File Offset: 0x0004C2FC
	private IEnumerator ReadyCheck()
	{
		while (!this.ConnectingTop && !this.ConnectingRight && !this.ConnectingBottom && !this.ConnectingLeft && !this.First)
		{
			yield return new WaitForSeconds(0.1f);
		}
		this.SetupDone = true;
		foreach (ModulePropSwitch modulePropSwitch in base.GetComponentsInChildren<ModulePropSwitch>())
		{
			modulePropSwitch.Module = this;
			modulePropSwitch.Setup();
		}
		LevelGenerator.Instance.ModulesSpawned++;
		if (!this.wallsInside || !this.wallsMap || !this.levelPointsEntrance || !this.levelPointsWaypoints || !this.levelPointsRoomVolume || !this.levelPointsNavmesh || !this.levelPointsConnected || !this.lightsMax || !this.lightsPrefab || !this.roomVolumeDoors || !this.roomVolumeHeight || !this.roomVolumeSpace || !this.navmeshConnected || !this.navmeshPitfalls || !this.valuablesAllTypes || !this.valuablesMaxed || !this.valuablesSwitch || !this.valuablesSwitchNavmesh || !this.valuablesTest || !this.ModulePropSwitchSetup || !this.ModulePropSwitchNavmesh)
		{
			Debug.LogWarning("Module not checked off: " + base.name, base.gameObject);
		}
		yield break;
	}

	// Token: 0x06000801 RID: 2049 RVA: 0x0004E10C File Offset: 0x0004C30C
	private void ResetChecklist()
	{
		this.wallsInside = false;
		this.wallsMap = false;
		this.levelPointsEntrance = false;
		this.levelPointsWaypoints = false;
		this.levelPointsRoomVolume = false;
		this.levelPointsNavmesh = false;
		this.levelPointsConnected = false;
		this.lightsMax = false;
		this.lightsPrefab = false;
		this.roomVolumeDoors = false;
		this.roomVolumeHeight = false;
		this.roomVolumeSpace = false;
		this.navmeshConnected = false;
		this.navmeshPitfalls = false;
		this.valuablesAllTypes = false;
		this.valuablesMaxed = false;
		this.valuablesSwitch = false;
		this.valuablesSwitchNavmesh = false;
		this.valuablesTest = false;
		this.ModulePropSwitchSetup = false;
		this.ModulePropSwitchNavmesh = false;
	}

	// Token: 0x06000802 RID: 2050 RVA: 0x0004E1AC File Offset: 0x0004C3AC
	private void SetAllChecklist()
	{
		this.wallsInside = true;
		this.wallsMap = true;
		this.levelPointsEntrance = true;
		this.levelPointsWaypoints = true;
		this.levelPointsRoomVolume = true;
		this.levelPointsNavmesh = true;
		this.levelPointsConnected = true;
		this.lightsMax = true;
		this.lightsPrefab = true;
		this.roomVolumeDoors = true;
		this.roomVolumeHeight = true;
		this.roomVolumeSpace = true;
		this.navmeshConnected = true;
		this.navmeshPitfalls = true;
		this.valuablesAllTypes = true;
		this.valuablesMaxed = true;
		this.valuablesSwitch = true;
		this.valuablesSwitchNavmesh = true;
		this.valuablesTest = true;
		this.ModulePropSwitchSetup = true;
		this.ModulePropSwitchNavmesh = true;
	}

	// Token: 0x06000803 RID: 2051 RVA: 0x0004E24C File Offset: 0x0004C44C
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.ConnectingTop);
			stream.SendNext(this.ConnectingRight);
			stream.SendNext(this.ConnectingBottom);
			stream.SendNext(this.ConnectingLeft);
			stream.SendNext(this.First);
			return;
		}
		this.ConnectingTop = (bool)stream.ReceiveNext();
		this.ConnectingRight = (bool)stream.ReceiveNext();
		this.ConnectingBottom = (bool)stream.ReceiveNext();
		this.ConnectingLeft = (bool)stream.ReceiveNext();
		this.First = (bool)stream.ReceiveNext();
	}

	// Token: 0x04000EAD RID: 3757
	private Color colorPositive = Color.green;

	// Token: 0x04000EAE RID: 3758
	private Color colorNegative = new Color(1f, 0.74f, 0.61f);

	// Token: 0x04000EAF RID: 3759
	public bool wallsInside;

	// Token: 0x04000EB0 RID: 3760
	[Space]
	public bool wallsMap;

	// Token: 0x04000EB1 RID: 3761
	public bool levelPointsEntrance;

	// Token: 0x04000EB2 RID: 3762
	[Space]
	public bool levelPointsWaypoints;

	// Token: 0x04000EB3 RID: 3763
	[Space]
	public bool levelPointsRoomVolume;

	// Token: 0x04000EB4 RID: 3764
	[Space]
	public bool levelPointsNavmesh;

	// Token: 0x04000EB5 RID: 3765
	[Space]
	public bool levelPointsConnected;

	// Token: 0x04000EB6 RID: 3766
	public bool lightsMax;

	// Token: 0x04000EB7 RID: 3767
	[Space]
	public bool lightsPrefab;

	// Token: 0x04000EB8 RID: 3768
	public bool roomVolumeDoors;

	// Token: 0x04000EB9 RID: 3769
	[Space]
	public bool roomVolumeHeight;

	// Token: 0x04000EBA RID: 3770
	[Space]
	public bool roomVolumeSpace;

	// Token: 0x04000EBB RID: 3771
	public bool navmeshConnected;

	// Token: 0x04000EBC RID: 3772
	[Space]
	public bool navmeshPitfalls;

	// Token: 0x04000EBD RID: 3773
	public bool valuablesAllTypes;

	// Token: 0x04000EBE RID: 3774
	[Space]
	public bool valuablesMaxed;

	// Token: 0x04000EBF RID: 3775
	[Space]
	public bool valuablesSwitch;

	// Token: 0x04000EC0 RID: 3776
	[Space]
	public bool valuablesSwitchNavmesh;

	// Token: 0x04000EC1 RID: 3777
	[Space]
	public bool valuablesTest;

	// Token: 0x04000EC2 RID: 3778
	public bool ModulePropSwitchSetup;

	// Token: 0x04000EC3 RID: 3779
	[Space]
	public bool ModulePropSwitchNavmesh;

	// Token: 0x04000EC4 RID: 3780
	internal bool ConnectingTop;

	// Token: 0x04000EC5 RID: 3781
	internal bool ConnectingRight;

	// Token: 0x04000EC6 RID: 3782
	internal bool ConnectingBottom;

	// Token: 0x04000EC7 RID: 3783
	internal bool ConnectingLeft;

	// Token: 0x04000EC8 RID: 3784
	[Space]
	internal bool SetupDone;

	// Token: 0x04000EC9 RID: 3785
	internal bool First;

	// Token: 0x04000ECA RID: 3786
	[Space]
	internal int GridX;

	// Token: 0x04000ECB RID: 3787
	internal int GridY;

	// Token: 0x04000ECC RID: 3788
	public bool Explored;

	// Token: 0x04000ECD RID: 3789
	internal bool StartRoom;

	// Token: 0x0200030F RID: 783
	public enum Type
	{
		// Token: 0x040025C6 RID: 9670
		Normal,
		// Token: 0x040025C7 RID: 9671
		Passage,
		// Token: 0x040025C8 RID: 9672
		DeadEnd,
		// Token: 0x040025C9 RID: 9673
		Extraction
	}
}
