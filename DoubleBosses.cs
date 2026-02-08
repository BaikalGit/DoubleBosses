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
    public class GlobalSettingsClass
    {
        public bool hardMode = true;
    }

    public class DoubleBosses : Mod, ITogglableMod, IGlobalSettings<GlobalSettingsClass>, IMenuMod
    {

#pragma warning disable CS8632
        public static GlobalSettingsClass GS { get; set; } = new GlobalSettingsClass();
        public void OnLoadGlobal(GlobalSettingsClass s)
        {
            GS = s;
        }

        public GlobalSettingsClass OnSaveGlobal()
        {
            return GS;
        }
        public bool ToggleButtonInsideMenu { get; }
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            return new List<IMenuMod.MenuEntry>
        {
            new IMenuMod.MenuEntry {
                Name = "Hard Mode",
                Description = "Makes every boss share their health, so you have to deal with both of them, untill they or you die",
                Values = new string[] {
                    "Off",
                    "On"
                },
                Saver = opt => GS.hardMode = opt switch {
                    0 => false,
                    1 => true,
                    // This should never be called
                    _ => throw new InvalidOperationException()
                },
                Loader = () => GS.hardMode switch {
                    false => 0,
                    true => 1,
                }
            }
        };
        }

        public static DoubleBosses? Instance { get; private set; }
        public static DoubleBosses UnsafeInstance => Instance!;
        new public string GetName() => "DoubleBosses";
        public override string GetVersion() => "0.2.4.0";

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
    "Mimic Spider", "Mimic Spider 2",
    "Hornet Nosk", "Hornet Nosk 2",
    "Jar Collector", "Jar Collector 2",
    "Lancer", "Lancer 2","Lobster", "Lobster 2",
    "Mega Zombie Beam Miner (1)", "Mega Zombie Beam Miner (1) 2",
    "Zombie Beam Miner Rematch", "Zombie Beam Miner Rematch 2",
    "Mega Jellyfish GG", "Mega Jellyfish GG 2",
    "Mantis Traitor Lord", "Mantis Traitor Lord 2",
    "Grey Prince", "Grey Prince 2",
    "HK Prime", "HK Prime 2",
    "Nightmare Grimm Boss", "Nightmare Grimm Boss 2",
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