using System;
using System.Collections.Generic;

// Token: 0x020001D9 RID: 473
[Serializable]
public class KeyBindingSaveData
{
	// Token: 0x04001AB1 RID: 6833
	public Dictionary<InputKey, List<string>> bindingOverrides;

	// Token: 0x04001AB2 RID: 6834
	public Dictionary<InputKey, bool> inputToggleStates;

	// Token: 0x04001AB3 RID: 6835
	public Dictionary<InputPercentSetting, int> inputPercentSettings;
}
