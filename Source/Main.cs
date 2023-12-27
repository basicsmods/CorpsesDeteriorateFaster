using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection.Emit;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;
using Verse;
using Verse.AI.Group;
using Verse.Noise;
using static HarmonyLib.Code;
using static RimWorld.MechClusterSketch;

namespace CorpsesDeteriorateFaster
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        static Main()
        {
            var harmony = new Harmony("basics.corpsesdeterioratefaster");
            harmony.PatchAll();
        }
    }

    
//    [HarmonyPatch(typeof(ThingDefGenerator_Corpses), nameof(ThingDefGenerator_Corpses.ImpliedCorpseDefs), MethodType.Enumerator)]
//    public class ThingDefGenerator_Corpses_ImpliedCorpseDefs
//    {
//        /*
//        The decompiled code for this function includes
//
//			thingDef2.SetStatBaseValue(StatDefOf.DeteriorationRate, 1f);
//        	
//		which in IL looks like
//
//			ldsfld class RimWorld.StatDef RimWorld.StatDefOf::DeteriorationRate
//			ldc.r4    1
//			call      void RimWorld.StatExtension::SetStatBaseValue(class Verse.BuildableDef, class RimWorld.StatDef, float32)
//
//		so we just want to change that 1 to something else.
//
//
//        ldsfld RimWorld.StatDef RimWorld.StatDefOf::DeteriorationRate
//        ldc.r4 1
//        */
//        public static IEnumerable<CodeInstruction> XTranspiler(IEnumerable<CodeInstruction> instructions)
//        {
//            var list = instructions.ToList();
//            //int idx;
//
//
//
//            for (int i = 0; i < list.Count; i++)
//            {
//                if (list[i].ToString() == "ldsfld RimWorld.StatDef RimWorld.StatDefOf::DeteriorationRate")
//                {
//                    list[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 25f);
//                    Log.Message("CorpsesDeteriorateFaster: Changed 'ldc.r4 1' to 'ldc.r4 25'. $basics$");
//                    Log.Message("CorpsesDeteriorateFaster: Mod enabled.");
//                    return list;
//                }
//            }
//
//            Log.Error("CorpsesDeteriorateFaster: Cannot find ldsfld RimWorld.StatDef RimWorld.StatDefOf::DeteriorationRate in ThingDefGenerator_Corpses::ImpliedCorpseDefs::MoveNext. $basics$");
//            Log.Error("CorpsesDeteriorateFaster: Mod not enabled.");
//
//            /*
//			Log.Message("$basics$ start");
//			foreach ( var instruction in list )
//			{
//				Log.Message(instruction.ToString());
//			}
//            Log.Message("$basics$ end");
//			*/
//
//            /*
//            var f_Notify_MemberExitedMap = AccessTools.Method(typeof(Faction), nameof(Faction.Notify_MemberExitedMap));
//			idx = list.FirstIndexOf(instr => instr.Calls(f_Notify_MemberExitedMap));
//			if (idx < 0 || idx >= list.Count)
//				Log.Error("RescueGoodwillFix: Cannot find call Faction::Notify_MemberExitedMap() in Pawn::ExitMap. $basics$");
//            else
//			{
//				list[idx - 1] = new CodeInstruction(OpCodes.Ldloc_3);
//				Log.Message("RescueGoodwillFix: Changed ldloc_s to ldloc_3. $basics$");
//				Log.Message("RescueGoodwillFix: Mod enabled.");
//			}
//            */
//            return list;
//        }
//
//        /*static IEnumerable<ThingDef> Postfix(IEnumerable<ThingDef> values)
//        {
//            foreach (ThingDef thingDef in values)
//            {
//                thingDef.SetStatBaseValue(StatDefOf.DeteriorationRate, 25f);
//                yield return thingDef;
//            }
//            Log.Message("CorpsesDeteriorateFaster: Postfixed. $basics$");
//        }*/
//    }

    [HarmonyPatch(typeof(SteadyEnvironmentEffects), nameof(SteadyEnvironmentEffects.FinalDeteriorationRate), new Type[] { typeof(Thing), typeof(bool), typeof(bool), typeof(TerrainDef), typeof(List<string>)})]
    public class SteadyEnvironmentEffects_ImpliedCorpseDefs
    {
        static void Postfix(ref float __result, Thing t, bool roofed)
        {
            Corpse corpse = t as Corpse;
            if (corpse != null && corpse.InnerPawn != null && corpse.InnerPawn.RaceProps != null && !corpse.InnerPawn.RaceProps.IsMechanoid)
            {
                __result = __result * 40;
                if (__result > 45) __result = 65;
            }
        }
    }


//    [StaticConstructorOnStartup]
//    public static class UIAssets
//    {
//        public static readonly Material AimPieMaterialPatch = SolidColorMaterials.SimpleSolidColorMaterial(new UnityEngine.Color(0f, 0.5f, 1f, 0.9f), false);
//    }
//
//    [HarmonyPatch(typeof(Stance_Warmup), nameof(Stance_Warmup.StanceDraw), new Type[] { })]
//    public class Stance_Warmup_StanceDraw
//    {
//        static void Postfix(ref Stance_Warmup __instance)
//        {
//            if (__instance.drawAimPie && Find.Selector.IsSelected(__instance.stanceTracker.pawn))
//            {
//                // This is the check used in the vanilla method. We don't need to do anything special
//                // if this is true since the vanilla method would have called the correct method anyways.
//                return;
//            }
//            // These extra checks cause the AimPie to be drawn even when the shooter isn't selected 
//            // IFF the shooter is hostile to the player AND the shooter's target is a friendly pawn / mech.
//            // The AimPie won't be drawn if the target is another enemy (like from berserk) or the target
//            // is a building (like a turret / wall / door).
//            // I should maybe make it ignore friendly non-playerFaction targets as well since I don't really
//            // care if the enemies are aiming at pawns who aren't part of my colony.
//            if (__instance.drawAimPie && (__instance.stanceTracker.pawn.HostileTo(Faction.OfPlayer) &&
//                __instance.focusTarg.Pawn != null && !__instance.focusTarg.Pawn.HostileTo(Faction.OfPlayer)))
//            {
//                // This is the same call that the default method makes when the default if block is taken.
//                GenDraw.DrawAimPie(__instance.stanceTracker.pawn, __instance.focusTarg, (int)((float)__instance.ticksLeft * __instance.pieSizeFactor), 0.2f);
//            }
//        }
//    }
//
//    
//    [HarmonyPatch(typeof(GenDraw), nameof(GenDraw.DrawAimPieRaw), new Type[] { typeof(Vector3), typeof(float), typeof(int) })]
//    public class GenDraw_DrawAimPieRaw
//    {
//        /*static bool Prefix(Vector3 center, float facing, int degreesWide)
//        {
//            // Log.Message("$basics$ in gendraw_drawaimpieraw prefix.");
//            if (degreesWide <= 0)
//            {
//                return false;
//            }
//            if (degreesWide > 360)
//            {
//                degreesWide = 360;
//            }
//            center += Quaternion.AngleAxis(facing, Vector3.up) * Vector3.forward * 0.8f;
//            Material NewMaterial_basics = SolidColorMaterials.SimpleSolidColorMaterial(new UnityEngine.Color(0f, 0.5f, 1f, 0.9f), false);
//            UnityEngine.Graphics.DrawMesh(MeshPool.pies[degreesWide], center, Quaternion.AngleAxis(facing + (float)(degreesWide / 2) - 90f, Vector3.up), NewMaterial_basics, 0);
//            return false;
//        }*/
//
//        /*
//        ldarg.2 NULL
//        ldc.i4.0 NULL
//        bgt.s Label0
//        ret NULL
//        ldarg.2 NULL [Label0]
//        ldc.i4 360
//        ble.s Label1
//        ldc.i4 360
//        starg.s 2
//        ldarg.0 NULL [Label1]
//        ldarg.1 NULL
//        call static UnityEngine.Vector3 UnityEngine.Vector3::get_up()
//        call static UnityEngine.Quaternion UnityEngine.Quaternion::AngleAxis(System.Single angle, UnityEngine.Vector3 axis)
//        call static UnityEngine.Vector3 UnityEngine.Vector3::get_forward()
//        call static UnityEngine.Vector3 UnityEngine.Quaternion::op_Multiply(UnityEngine.Quaternion rotation, UnityEngine.Vector3 point)
//        ldc.r4 0.8
//        call static UnityEngine.Vector3 UnityEngine.Vector3::op_Multiply(UnityEngine.Vector3 a, System.Single d)
//        call static UnityEngine.Vector3 UnityEngine.Vector3::op_Addition(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
//        starg.s 0
//        ldsfld UnityEngine.Mesh[] Verse.MeshPool::pies
//        ldarg.2 NULL
//        ldelem.ref NULL
//        ldarg.0 NULL
//        ldarg.1 NULL
//        ldarg.2 NULL
//        ldc.i4.2 NULL
//        div NULL
//        conv.r4 NULL
//        add NULL
//        ldc.r4 90
//        sub NULL
//        call static UnityEngine.Vector3 UnityEngine.Vector3::get_up()
//        call static UnityEngine.Quaternion UnityEngine.Quaternion::AngleAxis(System.Single angle, UnityEngine.Vector3 axis)
//        ldsfld UnityEngine.Material Verse.GenDraw::AimPieMaterial
//        ldc.i4.0 NULL
//        call static System.Void UnityEngine.Graphics::DrawMesh(UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Material material, System.Int32 layer)
//        ret NULL
//
//        to
//
//        ldarg.2 NULL
//        ldc.i4.0 NULL
//        bgt.s Label0
//        ret NULL
//        ldarg.2 NULL [Label0]
//        ldc.i4 360
//        ble.s Label1
//        ldc.i4 360
//        starg.s 2
//        ldarg.0 NULL [Label1]
//        ldarg.1 NULL
//        call static UnityEngine.Vector3 UnityEngine.Vector3::get_up()
//        call static UnityEngine.Quaternion UnityEngine.Quaternion::AngleAxis(System.Single angle, UnityEngine.Vector3 axis)
//        call static UnityEngine.Vector3 UnityEngine.Vector3::get_forward()
//        call static UnityEngine.Vector3 UnityEngine.Quaternion::op_Multiply(UnityEngine.Quaternion rotation, UnityEngine.Vector3 point)
//        ldc.r4 0.8
//        call static UnityEngine.Vector3 UnityEngine.Vector3::op_Multiply(UnityEngine.Vector3 a, System.Single d)
//        call static UnityEngine.Vector3 UnityEngine.Vector3::op_Addition(UnityEngine.Vector3 a, UnityEngine.Vector3 b)
//        starg.s 0
//        ldsfld UnityEngine.Mesh[] Verse.MeshPool::pies
//        ldarg.2 NULL
//        ldelem.ref NULL
//        ldarg.0 NULL
//        ldarg.1 NULL
//        ldarg.2 NULL
//        ldc.i4.2 NULL
//        div NULL
//        conv.r4 NULL
//        add NULL
//        ldc.r4 90
//        sub NULL
//        call static UnityEngine.Vector3 UnityEngine.Vector3::get_up()
//        call static UnityEngine.Quaternion UnityEngine.Quaternion::AngleAxis(System.Single angle, UnityEngine.Vector3 axis)
//        <-->ldsfld UnityEngine.Material Verse.GenDraw::AimPieMaterial
//        <-->
//        ldsfld UnityEngine.Material CorpsesDeteriorateFaster.UIAssets::AimPieMaterialPatch
//        <-->
//        ldc.i4.0 NULL
//        call static System.Void UnityEngine.Graphics::DrawMesh(UnityEngine.Mesh mesh, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, UnityEngine.Material material, System.Int32 layer)
//        ret NULL
//        */
//        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
//        {
//            var list = instructions.ToList();
//            var newList = new List<CodeInstruction>();
//            
//            bool found1 = false;
//            for (int i = 0; i < list.Count; i++)
//            {
//                
//                if (list[i].ToString() == "ldsfld UnityEngine.Material Verse.GenDraw::AimPieMaterial")
//                {
//                    // ldloc.0
//                    found1 = true;
//                    newList.Add(new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(UIAssets), nameof(UIAssets.AimPieMaterialPatch))));
//                } else
//                {
//                    newList.Add(list[i]);
//                }
//            }
//
//            if (!found1)
//            {
//                Log.Error("$basics$ Highlight AimTime Icon: found1 failed. Patch not applied.");
//                return list;
//            }
//
//            Log.Message("$basics$ Highlight AimTime Icon: Patch applied successfully.");
//            return newList;
//            /*
//            Log.Message("$basics$ start");
//            Log.Message(newList.Count.ToString());
//            foreach (var instruction in newList)
//            {
//                Log.Message(instruction.ToString());
//            }
//            Log.Message("$basics$ end");
//
//            return newList;
//            */
//
//
//            /*
//            Log.Message("$basics$ start");
//			foreach ( var instruction in list )
//			{
//				Log.Message(instruction.ToString());
//			}
//            Log.Message("$basics$ end");
//			*/
//            //return list;
//        }
//    }
//
//    [HarmonyPatch(typeof(ProjectileCE), "TryCollideWith", new Type[] { typeof(Thing) })]
//    public class ProjectileCE_TryCollideWith
//    {
//        static bool Prefix(Thing thing, ref bool __result, ProjectileCE __instance)
//        {
//            if (thing != __instance.intendedTarget.Thing && __instance.launcher is Pawn && thing is Pawn)
//            {
//                var launcherPawn = __instance.launcher as Pawn;
//                var thingPawn = thing as Pawn;
//                //Log.Message("launcher: " + __instance.launcher.Label);
//                //Log.Message("TryCollideWith " + thing.Label);
//                //Log.Message("launcher position: " + thingPawn.Position);
//                //Log.Message("pawn position: " + launcherPawn.Position);
//                if (launcherPawn != null && thingPawn != null)
//                {
//                    if (!launcherPawn.HostileTo(thingPawn))
//                    {
//                        //var x_diff = Math.Abs(thingPawn.Position.x - thing.Position.x);
//                        //var y_diff = Math.Abs(thingPawn.Position.y - thing.Position.y);
//                        //var z_diff = Math.Abs(thingPawn.Position.z - thing.Position.z);
//                        //if (x_diff + z_diff + y_diff < 3)
//                        //{
//                        //    Log.Message("Skipping friendly-fire due to pawns being close enough.");
//                        //    return false;
//                        //}
//                        var x_diff = thingPawn.Position.x - thing.Position.x;
//                        var z_diff = thingPawn.Position.z - thing.Position.z;
//                        if (x_diff == 0 || z_diff == 0)
//                        {
//                            //Log.Message("Ignoring friendly-fire since these pawns must be leaning and it's dumb they'd shoot into a leaning ally.");
//                            __result = false;
//                            return false;
//                        }
//                    }
//                }
//            }
//            return true;
//        }
//    }
}
