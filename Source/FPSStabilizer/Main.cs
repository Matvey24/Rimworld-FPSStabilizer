using Verse;
namespace FPSStabilizer
{
    using HarmonyLib;
    using System.Reflection;
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Linq;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public static class Main
    {
        public static float frametime;
        static Main(){
            frametime = 1000f / Application.targetFrameRate;

            Harmony harm = new Harmony("matvey24.FPSStabilizer");
            MethodBase original = AccessTools.Method(typeof(TickManager), nameof(TickManager.TickManagerUpdate));
            HarmonyMethod transpiler = new HarmonyMethod(typeof(Main), nameof(Transpiler));
            try
            {
                MethodInfo methodInfo = harm.Patch(original, null, null, transpiler, null);
            }
            catch (Exception e)
            {
                Log.Error($"Error during patching {original} with: transpliter {transpiler?.method}\n{e}");
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool patched_something = false;
            var l = instructions.ToList();
            foreach (var c in l)
            {
                if(!patched_something && c.opcode == OpCodes.Ldc_R4 && c.operand is float)
                {
                    if ((float)c.operand > 10) {
                        // this instruction is probably milliseconds
                        c.operand = frametime;
                        patched_something = true;
                        Log.Message("FPSStabilier patched to " + 1000 / frametime + " FPS");
                    }
                }
                yield return c;
            }
            if (!patched_something) {
                Log.Error("FPSStabilier not patched, instruction was not found");
            }
        }
    }
}
