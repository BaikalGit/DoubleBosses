using System;
using System.Collections;
using System.Collections.Generic;
using Modding;
using Osmi.Utils;
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
            AllFinder allFinder = GameManager.instance.gameObject.GetComponent<AllFinder>();
            if (allFinder != null)
            {
                UnityEngine.Object.Destroy(allFinder);
            }
            Instance = null;
        }
    }
}