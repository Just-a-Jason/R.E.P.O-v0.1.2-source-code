using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000215 RID: 533
public class AssetManager : MonoBehaviour
{
	// Token: 0x06001146 RID: 4422 RVA: 0x0009A1F1 File Offset: 0x000983F1
	private void Awake()
	{
		if (AssetManager.instance == null)
		{
			AssetManager.instance = this;
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.mainCamera = Camera.main;
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x0009A21E File Offset: 0x0009841E
	public void PhysImpactEffect(Vector3 _position)
	{
		this.physImpactEffectSound.Play(_position, 1f, 1f, 1f, 1f);
		Object.Instantiate<GameObject>(this.physImpactEffect, _position, Quaternion.identity);
	}

	// Token: 0x04001CF1 RID: 7409
	public PhysicMaterial physicMaterialStickyExtreme;

	// Token: 0x04001CF2 RID: 7410
	public PhysicMaterial physicMaterialSlipperyExtreme;

	// Token: 0x04001CF3 RID: 7411
	public PhysicMaterial physicMaterialSlippery;

	// Token: 0x04001CF4 RID: 7412
	public PhysicMaterial physicMaterialDefault;

	// Token: 0x04001CF5 RID: 7413
	public PhysicMaterial physicMaterialPlayerMove;

	// Token: 0x04001CF6 RID: 7414
	public PhysicMaterial physicMaterialPlayerIdle;

	// Token: 0x04001CF7 RID: 7415
	public PhysicMaterial physicMaterialPhysGrabObject;

	// Token: 0x04001CF8 RID: 7416
	public PhysicMaterial physicMaterialSlipperyPlus;

	// Token: 0x04001CF9 RID: 7417
	public AnimationCurve animationCurveImpact;

	// Token: 0x04001CFA RID: 7418
	public AnimationCurve animationCurveWooshAway;

	// Token: 0x04001CFB RID: 7419
	public AnimationCurve animationCurveWooshIn;

	// Token: 0x04001CFC RID: 7420
	public AnimationCurve animationCurveInOut;

	// Token: 0x04001CFD RID: 7421
	public AnimationCurve animationCurveClickInOut;

	// Token: 0x04001CFE RID: 7422
	public AnimationCurve animationCurveEaseInOut;

	// Token: 0x04001CFF RID: 7423
	public Sound soundEquip;

	// Token: 0x04001D00 RID: 7424
	public Sound soundUnequip;

	// Token: 0x04001D01 RID: 7425
	public Sound soundDeviceTurnOn;

	// Token: 0x04001D02 RID: 7426
	public Sound soundDeviceTurnOff;

	// Token: 0x04001D03 RID: 7427
	public Sound batteryChargeSound;

	// Token: 0x04001D04 RID: 7428
	public Sound batteryDrainSound;

	// Token: 0x04001D05 RID: 7429
	public Sound batteryLowBeep;

	// Token: 0x04001D06 RID: 7430
	public Sound batteryLowWarning;

	// Token: 0x04001D07 RID: 7431
	public List<Color> playerColors;

	// Token: 0x04001D08 RID: 7432
	public GameObject enemyValuableSmall;

	// Token: 0x04001D09 RID: 7433
	public GameObject enemyValuableMedium;

	// Token: 0x04001D0A RID: 7434
	public GameObject enemyValuableBig;

	// Token: 0x04001D0B RID: 7435
	public GameObject surplusValuableSmall;

	// Token: 0x04001D0C RID: 7436
	public GameObject surplusValuableMedium;

	// Token: 0x04001D0D RID: 7437
	public GameObject surplusValuableBig;

	// Token: 0x04001D0E RID: 7438
	[Space]
	public Mesh valuableMeshTiny;

	// Token: 0x04001D0F RID: 7439
	public Mesh valuableMeshSmall;

	// Token: 0x04001D10 RID: 7440
	public Mesh valuableMeshMedium;

	// Token: 0x04001D11 RID: 7441
	public Mesh valuableMeshBig;

	// Token: 0x04001D12 RID: 7442
	public Mesh valuableMeshWide;

	// Token: 0x04001D13 RID: 7443
	public Mesh valuableMeshTall;

	// Token: 0x04001D14 RID: 7444
	public Mesh valuableMeshVeryTall;

	// Token: 0x04001D15 RID: 7445
	public GameObject prefabTeleportEffect;

	// Token: 0x04001D16 RID: 7446
	public GameObject debugEnemyInvestigate;

	// Token: 0x04001D17 RID: 7447
	public GameObject debugLevelPointError;

	// Token: 0x04001D18 RID: 7448
	public GameObject physImpactEffect;

	// Token: 0x04001D19 RID: 7449
	public Sound physImpactEffectSound;

	// Token: 0x04001D1A RID: 7450
	internal Color colorYellow = new Color(1f, 0.55f, 0f);

	// Token: 0x04001D1B RID: 7451
	internal Camera mainCamera;

	// Token: 0x04001D1C RID: 7452
	public static AssetManager instance;
}
