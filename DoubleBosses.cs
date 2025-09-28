using HutongGames.PlayMaker;
using Modding;
using Osmi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DoubleBosses
{
    public class DoubleBosses : Mod, ITogglableMod
    {

#pragma warning disable CS8632

        public static DoubleBosses? Instance { get; private set; }
        public static DoubleBosses UnsafeInstance => Instance!;
        new public string GetName() => "DoubleBosses";
        public override string GetVersion() => "1.0.0.0";

        public static readonly HashSet<string> trackedBosses = new()
        {
    "Giant Fly", "Giant Fly 2", "Giant Fly 3", "Giant Fly 4",
    "Giant Buzzer Col", "Giant Buzzer Col 2", "Giant Buzzer Col 3", "Giant Buzzer Col 4", "Giant Buzzer Col 5", "Giant Buzzer Col 6", "Giant Buzzer Col 7", "Giant Buzzer Col (1)",
    "Mawlek Body", "Mawlek Body 2",
    "False Knight New", "False Knight New 2",
    "False Knight Dream", "False Knight Dream 2",
    "Hornet Boss 1", "Hornet Boss 1 2",
    "Hornet Boss 2", "Hornet Boss 2 2",
    "Mega Moss Charger", "Mega Moss Charger 2",
    "Fluke Mother", "Fluke Mother 2",
    "Mega Fat Bee", "Mega Fat Bee (1)", "Mega Fat Bee 2", "Mega Fat Bee (1) 2",
    "Hive Knight", "Hive Knight 2",
    "Infected Knight", "Infected Knight 2",
    "Lost Kin", "Lost Kin 2",
        };

        public static Dictionary<string, bool> BossDeathStatus = new Dictionary<string, bool>();

        public override void Initialize()
        {
            if (Instance != null)
            {
                LogWarn("Attempting to initialize multiple times, operation rejected");
                return;
            }
            Instance = this;
            ModHooks.NewGameHook += AddFinder;
            ModHooks.SavegameLoadHook += Load;
            On.HealthManager.SendDeathEvent += sendDeathEvent;
            On.BossSceneController.EndBossScene += HookEndBossScene;
        }
        private void sendDeathEvent(On.HealthManager.orig_SendDeathEvent orig, HealthManager self)
        {
            string bossName = self.gameObject.name;

            if (trackedBosses.Contains(bossName))
            {
                BossDeathStatus[bossName] = true;
                return;
            }

            orig(self);
        }
        private void HookEndBossScene(On.BossSceneController.orig_EndBossScene orig, BossSceneController self)
        {
            //DeepSeek wrote this one. I have no idea how this call history check works, but it is useful so i'm putting it here
            var stackTrace = new System.Diagnostics.StackTrace(1);
            var callerFrame = stackTrace.GetFrame(0);
            var callerMethod = callerFrame.GetMethod();

            Modding.Logger.Log($"[Boss Reset] EndBossScene called by: {callerMethod.DeclaringType?.Name}.{callerMethod.Name}");
            LogCallStack(stackTrace);

            orig(self);
        }

        private void LogCallStack(System.Diagnostics.StackTrace stackTrace)
        {
            Modding.Logger.Log("[Boss Reset] Call stack:");
            for (int i = 0; i < Math.Min(stackTrace.FrameCount, 10); i++)
            {
                var frame = stackTrace.GetFrame(i);
                var method = frame.GetMethod();
                if (method != null)
                {
                    Modding.Logger.Log($"[Boss Reset]   [{i}] {method.DeclaringType?.Name}.{method.Name}");

                }
            }
            LogActiveFsmStates();
        }

        private void LogActiveFsmStates()
        {
            try
            {
                Type playMakerFSMType = null;

                string[] possibleTypeNames = {
                    "PlayMakerFSM",
                    "HutongGames.PlayMaker.Fsm",
                    "HutongGames.PlayMaker.PlayMakerFSM",
                    "Fsm",
                    "FsmComponent"
                };

                        string[] possibleAssemblies = {
                    "Assembly-CSharp",
                    "Assembly-CSharp-firstpass",
                    "PlayMaker",
                    "HutongGames.PlayMaker"
                };

                // Try combinations
                foreach (var typeName in possibleTypeNames)
                {
                    foreach (var assembly in possibleAssemblies)
                    {
                        string fullTypeName = $"{typeName}, {assembly}";
                        playMakerFSMType = Type.GetType(fullTypeName);

                        if (playMakerFSMType != null)
                        {
                            Modding.Logger.Log($"[Boss Reset] Found FSM type: {fullTypeName}");
                            break;
                        }
                    }
                    if (playMakerFSMType != null) break;
                }

                if (playMakerFSMType == null)
                {
                    Modding.Logger.Log("[Boss Reset] Could not find PlayMakerFSM type in any assembly");
                    return;
                }

                var allFsms = UnityEngine.Object.FindObjectsOfType(playMakerFSMType);
                Modding.Logger.Log($"[Boss Reset] Found {allFsms.Length} FSMs in scene");

                foreach (var fsmComponent in allFsms)
                {
                    var gameObjectProp = playMakerFSMType.GetProperty("gameObject");
                    var gameObject = gameObjectProp?.GetValue(fsmComponent) as UnityEngine.GameObject;

                    if (gameObject != null && gameObject.name.Contains("Infected Knight"))
                    {
                        var fsmNameProp = playMakerFSMType.GetProperty("FsmName");
                        var activeStateNameProp = playMakerFSMType.GetProperty("ActiveStateName");

                        string fsmName = fsmNameProp?.GetValue(fsmComponent) as string ?? "Unknown";
                        string stateName = activeStateNameProp?.GetValue(fsmComponent) as string ?? "Unknown";

                        Modding.Logger.Log($"[Boss Reset] BOSS FSM: {gameObject.name} -> {fsmName} : {stateName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Modding.Logger.LogError($"[Boss Reset] Error scanning FSMs: {ex.Message}");
            }
        }
        private void Load(int id)
        {
            AddFinder();
        }
        private void AddFinder()
        {
            GameManager.instance.gameObject.AddComponent<AllFinder>();
        }
        public void Unload()
        {
            ModHooks.NewGameHook -= AddFinder;
            ModHooks.SavegameLoadHook -= Load;
            On.HealthManager.SendDeathEvent -= sendDeathEvent;
            On.BossSceneController.EndBossScene -= HookEndBossScene;
            AllFinder allFinder = GameManager.instance.gameObject.GetComponent<AllFinder>();
            if (allFinder != null)
            {
                UnityEngine.Object.Destroy(allFinder);
            }
            Instance = null;
        }
    }
}