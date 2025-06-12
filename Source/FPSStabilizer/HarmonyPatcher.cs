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
        Harmony harm;
        MethodBase original;
        HarmonyMethod transpiler;

        MethodInfo info;

        public static string message;
        public static bool patchable
        {
            get {
                return instruction_idx >= 0;
            } 
        }

        static int instruction_idx;
        static float fps;

        public HarmonyPatcher()
        {
            harm = new Harmony("matvey24.FPSStabilizer");
            original = AccessTools.Method(typeof(TickManager), nameof(TickManager.TickManagerUpdate));
            transpiler = new HarmonyMethod(typeof(HarmonyPatcher), nameof(Transpiler));
            instruction_idx = -1;
            message = null;
            info = null;
        }
        public void patch(float FPS)
        {
            try
            {
                if (info != null)
                    harm.Unpatch(original, info);
                fps = FPS;
                info = harm.Patch(original, null, null, transpiler, null);
            }
            catch (Exception e)
            {
                message = $"Error during patching {original} with: transpiler {transpiler?.method}\n{e}";
            }
        }

        public static void find_instruction(IEnumerable<CodeInstruction> instructions)
        {
            int idx = -1;

            for(int i = 0; i < instructions.Count(); ++i) {
                CodeInstruction c = instructions.ElementAt(i);
                if (c.opcode != OpCodes.Ldc_R4 || (float)c.operand < 10)
                    continue;

                if (idx != -1)
                {
                    message = "Can not patch, two similar instructions was found";
                    instruction_idx = -2;
                    return;
                }

                idx = i;
            }
            if (idx == -1)
            {
                message = "Can not patch, no instruction was found";
                instruction_idx = -2;
                return;
            }
            instruction_idx = idx;
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            if(instruction_idx == -1)
            {
                find_instruction(instructions);
            }
            if (instruction_idx == -2)
                return instructions;
            float time = 1000f / fps;

            CodeInstruction to_patch = instructions.ElementAt(instruction_idx);
            to_patch.operand = time;
            message = "Patched to " + 1000f / time + " FPS";

            return instructions;
        }
    }
}
