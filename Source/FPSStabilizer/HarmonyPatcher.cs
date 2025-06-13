using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace FPSStabilizer
{
    class HarmonyPatcher
    {
        public static float target_frametime = 16;
        public static string message = "";

        public static void init()
        {
            Harmony harm = new Harmony("matvey24.FPSStabilizer");
            MethodBase original = AccessTools.Method(typeof(TickManager), nameof(TickManager.TickManagerUpdate));
            HarmonyMethod transpiler = new HarmonyMethod(typeof(HarmonyPatcher), nameof(Transpiler));

            try
            {
                harm.Patch(original, null, null, transpiler, null);
            }
            catch (Exception e)
            {
                message = $"Error during patching {original} with: transpiler {transpiler?.method}\n{e}";
            }
        }

        public static int find_instruction(IEnumerable<CodeInstruction> instructions)
        {
            int idx = -1;
            for(int i = 0; i < instructions.Count(); ++i) {
                CodeInstruction c = instructions.ElementAt(i);
                if (c.opcode != OpCodes.Ldc_R4 || (float)c.operand < 10)
                    continue;

                if (idx != -1)
                {
                    message = "Can not patch, two similar instructions was found";
                    return -1;
                }

                idx = i;
            }
            if (idx == -1)
            {
                message = "Can not patch, no instruction was found";
                return -1;
            }
            return idx;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            int idx = find_instruction(instructions);
            if (idx == -1)
                return instructions;

            CodeInstruction to_patch = instructions.ElementAt(idx);
            to_patch.opcode = OpCodes.Ldsfld;
            to_patch.operand = AccessTools.Field(typeof(HarmonyPatcher), "target_frametime");
            message = "Patched";
            return instructions;
        }
    }
}
