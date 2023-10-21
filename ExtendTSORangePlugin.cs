using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;
using XDMenuPlay.Customise;

namespace ExtendTSORange
{
    [BepInPlugin("uwu.speen.extendtsorange", "Extend Track Speed Override Range", "0.1.0")]
    public class ExtendTSORangePlugin : BaseUnityPlugin
    {
        public static BepInEx.Logging.ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Harmony.CreateAndPatchAll(typeof(ExtendTSORange));
            Harmony.CreateAndPatchAll(typeof(ExtendTSORangeUI));
        }

        [HarmonyPatch(typeof(PlayerSettingsData), MethodType.StaticConstructor)]
        public static class ExtendTSORange
        {
            [HarmonyPrefix]
            public static bool Prefix(PlayerSettingsData __instance)
            {
                AccessTools.Field(typeof(PlayerSettingsData), "TrackSpeedIntValueRange").SetValue(__instance, new IntRange(1, 300));
                return false;
            }
        }

        [HarmonyPatch(typeof(AccessibilitySettingsChangeHandler), "SetupTrackSpeedOption")]
        public class ExtendTSORangeUI
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                foreach (var instruction in instructions)
                {
                    if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte && (sbyte)instruction.operand == 10)
                    {
                        instruction.operand = (sbyte)1;
                    }
                    else if (instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand is sbyte && (sbyte)instruction.operand == 61)
                    {
                        instruction.opcode = OpCodes.Ldc_I4;
                        instruction.operand = 301;
                    }

                    yield return instruction;
                }
            }
        }
    }
}
