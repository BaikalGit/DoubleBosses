using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding.Utils;
using Osmi.Game;
using Osmi.SimpleFSM;
using Satchel;
using Steamworks;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.UI.GridLayoutGroup;


namespace DoubleBosses
{
#pragma warning disable CS0618

    class GruzMotherControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 94.84f, 20.43f),
            (3, 0, 102f, 16f),
            (4, 0, 86f, 25f),
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            

            Scene next = UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Gruz_Mother");

            GameObject battleCtrl = next.GetRootGameObjects()
                    .First(go => go.name == "Battle Scene");
            PlayMakerFSM battleCtrlFsm = battleCtrl.LocateMyFSM("Battle Control");

            GameObject enemies = next.GetRootGameObjects()
                    .First(go => go.name == "_Enemies");
            var fly = new[] { enemies.Child("Giant Fly"!) }.ToList();


            StartCoroutine(InitStuff(fly, battleCtrlFsm));

        }
        private IEnumerator InitStuff(List<GameObject> fly, PlayMakerFSM battleCtrlFsm)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject[] extraFly = extrasInfo
                .Map(info => {
                    GameObject prefab = fly[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = "Giant Fly " + info.num;
                    return extra;
                })
                .ToArray();
            fly.AddRange(extraFly);
            battleCtrlFsm.FsmVariables.FindFsmInt("Battle Enemies").Value = fly.Count;
        }
        bool AllGiantFlyDead()
        {
            string[] flyNames = { "Giant Fly", "Giant Fly 2", "Giant Fly 3", "Giant Fly 4" };
            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllGiantFlyDead())
            {
                BossSceneController.Instance.EndBossScene();
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }

    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    class GruzMotherVControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 94.84f, 20.43f),
            (3, 0, 102f, 16f),
            (4, 0, 86f, 25f),
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }

            Scene next = UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Gruz_Mother_V");

            GameObject battleCtrl = next.GetRootGameObjects()
                    .First(go => go.name == "Battle Scene");
            PlayMakerFSM battleCtrlFsm = battleCtrl.LocateMyFSM("Battle Control");


            GameObject enemies = next.GetRootGameObjects()
                    .First(go => go.name == "_Enemies");
            var fly = new[] { enemies.Child("Giant Fly"!) }.ToList();


            StartCoroutine(InitStuff(fly, battleCtrlFsm));

        }
        private IEnumerator InitStuff(List<GameObject> fly, PlayMakerFSM battleCtrlFsm)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject[] extraFly = extrasInfo
                .Map(info => {
                    GameObject prefab = fly[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = "Giant Fly " + info.num;
                    return extra;
                })
                .ToArray();
            fly.AddRange(extraFly);
            battleCtrlFsm.FsmVariables.FindFsmInt("Battle Enemies").Value = fly.Count;
            fly.ShareHealth(0, "Milfs_Hard");


            StartCoroutine(InitHP());
        }
        IEnumerator InitHP()
        {
            yield return new WaitForSeconds(2f);
            GameObject.Find("Giant Fly").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Fly 2").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Fly 3").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Fly 4").GetComponent<HealthManager>().hp = 99999;
        }
        bool AllGiantFlyVDead()
        {
            string[] flyNames = { "Giant Fly", "Giant Fly 2", "Giant Fly 3", "Giant Fly 4" };

            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllGiantFlyVDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class VengeflyKingControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y, bool faceRight)[] extrasInfo = new[] {
            (2, 0, 34.36f, 17.82f, true),
            (3, 0, 42.73f, 21.24f, true),
            (4, 0, 52.16f, 21.24f, false)
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }

            var buzzers = new[] { GameObject.Find("Giant Buzzer Col") }
            .ToList();


            GameObject[] extraBuzzer = extrasInfo
                .Map(info => {
                    GameObject prefab = buzzers[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = "Giant Buzzer Col " + info.num;
                    extra.transform.SetScaleX(info.faceRight ? -1 : 1);
                    return extra;
                })
                .ToArray();
            buzzers.AddRange(extraBuzzer);
            //buzzers.ShareHealth(0, "DoubleBuzzers");

            //StartCoroutine(InitHP());

        }
        bool AllGiantBuzzerDead()
        {
            string[] flyNames = { "Giant Buzzer Col", "Giant Buzzer Col 2", "Giant Buzzer Col 3", "Giant Buzzer Col 4" };

            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllGiantBuzzerDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class VengeflyKingVControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y, bool faceRight)[] extrasInfo = new[] {
            (2, 0, 34.04f, 17.21f, true),
            (3, 0, 34.04f, 19.75f, true),
            (4, 0, 36.08f, 18.41f, true),
            (5, 1, 55.90f, 18.41f, false),
            (6, 1, 60.38f, 18.41f, false),
            (7, 1, 60.38f, 21.70f, false)
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }

            var buzzers = new[] { GameObject.Find("Giant Buzzer Col"), GameObject.Find("Giant Buzzer Col (1)") }
            .ToList();


            GameObject[] extraBuzzer = extrasInfo
                .Map(info => {
                    GameObject prefab = buzzers[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = "Giant Buzzer Col " + info.num;
                    extra.transform.SetScaleX(info.faceRight ? -1 : 1);
                    return extra;
                })
                .ToArray();
            buzzers.AddRange(extraBuzzer);

            var buzzersPairs = new[] { ("Giant Buzzer Col", "Giant Buzzer Col 2", "Giant Buzzer Col 3", "Giant Buzzer Col 4"),
                ("Giant Buzzer Col (1)", "Giant Buzzer Col 5", "Giant Buzzer Col 6", "Giant Buzzer Col 7") };

            foreach (var (buzzerA, buzzerB, buzzerC, buzzerD) in buzzersPairs)
            {
                var pair = buzzers
                    .Filter(buzzer => buzzer.name == buzzerA || buzzer.name == buzzerB || buzzer.name == buzzerC || buzzer.name == buzzerD)
                    .ToList();


                if (pair.Count == 4)
                {
                    pair.ShareHealth(0, "DoubleBuzzers");
                }
            }

            StartCoroutine(InitHP());

        }

        private IEnumerator InitHP()
        {
            yield return new WaitForSeconds(1f);
            GameObject.Find("Giant Buzzer Col").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Buzzer Col 2").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Buzzer Col 3").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Buzzer Col 4").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Buzzer Col (1)").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Buzzer Col 5").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Buzzer Col 6").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Giant Buzzer Col 7").GetComponent<HealthManager>().hp = 99999;
        }
        bool AllGiantBuzzerVDead()
        {
            string[] flyNames = { "Giant Buzzer Col", "Giant Buzzer Col 2", "Giant Buzzer Col 3", "Giant Buzzer Col 4",
            "Giant Buzzer Col (1)", "Giant Buzzer Col 5", "Giant Buzzer Col 6", "Giant Buzzer Col 7" };

            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllGiantBuzzerVDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class MawlekControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 34.36f, 17.82f),
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }

            var mawleks = new[] { GameObject.Find("Mawlek Body") }
            .ToList();


            GameObject[] extraMawleks = extrasInfo
                .Map(info => {
                    GameObject prefab = mawleks[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    //extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = "Mawlek Body " + info.num;
                    return extra;
                })
                .ToArray();
            mawleks.AddRange(extraMawleks);
        }
        bool AllMawlekDead()
        {
            string[] flyNames = { "Mawlek Body", "Mawlek Body 2" };

            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllMawlekDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class FalseKnightControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 19.23f, 33.40f),
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Scene next = UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_False_Knight");

            GameObject battleCtrl = next.GetRootGameObjects()
                    .First(go => go.name == "Battle Scene");
            PlayMakerFSM battleCtrlFsm = battleCtrl.LocateMyFSM("Battle Control");

            var falseKnights = new[] { battleCtrl.Child("False Knight New")! }
            .ToList();


            GameObject[] extraFalseKnights = extrasInfo
                .Map(info => {
                    GameObject prefab = falseKnights[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = "False Knight New " + info.num;
                    return extra;
                })
                .ToArray();

            falseKnights.AddRange(extraFalseKnights);
            battleCtrlFsm.FsmVariables.FindFsmInt("Battle Enemies").Value = falseKnights.Count;

            //StartCoroutine(InitHP());

        }

        private IEnumerator InitHP()
        {
            yield return new WaitForSeconds(1f);
            GameObject.Find("False Knight New").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("False Knight New 2").GetComponent<HealthManager>().hp = 99999;
        }
        bool AllFallseKnightDead()
        {
            string[] flyNames = { "False Knight New", "False Knight New 2" };

            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllFallseKnightDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------


    class FailedChampionControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 48.7f, 31.5f),
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Scene next = UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Failed_Champion");

            GameObject battleCtrl = next.GetRootGameObjects()
                    .First(go => go.name == "Battle Scene");
            PlayMakerFSM battleCtrlFsm = battleCtrl.LocateMyFSM("Battle Control");

            var falseKnights = new[] { GameObject.Find("False Knight Dream")! }
            .ToList();


            GameObject[] extraFalseKnights = extrasInfo
                .Map(info => {
                    GameObject prefab = falseKnights[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = "False Knight Dream " + info.num;
                    return extra;
                })
                .ToArray();

            falseKnights.AddRange(extraFalseKnights);
            battleCtrlFsm.FsmVariables.FindFsmInt("Battle Enemies").Value = falseKnights.Count;
            for(int i = 0; i <= 1; i++)
            {
                PlayMakerFSM DreamChampionFSM = falseKnights[i].LocateMyFSM("FalseyControl");
                DreamChampionFSM.RemoveAction("State 1", 4);
                DreamChampionFSM.RemoveAction("Stun Start", 11);
                DreamChampionFSM.RemoveAction("Recover", 4);
                DreamChampionFSM.RemoveAction("State 2", 4);
                DreamChampionFSM.RemoveAction("Rage Slam", 5);
                DreamChampionFSM.RemoveAction("Jump 2", 6);
                DreamChampionFSM.RemoveAction("Init 2", 10);
                DreamChampionFSM.RemoveAction("Steam 2", 7);
                DreamChampionFSM.RemoveAction("Floor Break", 22);
                DreamChampionFSM.RemoveAction("Steam", 5);
                DreamChampionFSM.RemoveAction("Start Fall", 6);
                DreamChampionFSM.RemoveAction("S Jump", 1);
                DreamChampionFSM.RemoveAction("JA Slam", 7);
                DreamChampionFSM.RemoveAction("JA Jump", 5);
                DreamChampionFSM.RemoveAction("Slam", 3);
                DreamChampionFSM.RemoveAction("S Land", 0);
            }




            //StartCoroutine(InitHP());

        }

        private IEnumerator InitHP()
        {
            yield return new WaitForSeconds(1f);
            GameObject.Find("False Knight Dream").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("False Knight Dream 2").GetComponent<HealthManager>().hp = 99999;
        }
        bool AllFailedChampion()
        {
            string[] flyNames = { "False Knight Dream", "False Knight Dream 2" };

            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllFailedChampion())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            

        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class Hornet1Control : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y, bool faceRight)[] extrasInfo = new[] {
            (2, 0, 18.5f, 30.0f, true),
        };
        public float throwMaxTravelTime = .8f;
        public Needle needle;
        public NeedleTink needleTink;
        public float throwWindUpTime = .03f;
        protected Ray throwRay;
        public float throwDistance = 12f;
        protected RaycastHit2D throwRaycast;



        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Scene next = UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Hornet_1");

            GameObject BossHolder = next.GetRootGameObjects()
                    .First(go => go.name == "Boss Holder");
            PlayMakerFSM bossHolderFSM = BossHolder.LocateMyFSM("FSM");

            var hornets = new[] { BossHolder.Child("Hornet Boss 1")! }
            .ToList();

            GameObject[] extraHornets = extrasInfo
                .Map(info => {
                    GameObject prefab = hornets[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.transform.SetScaleX(info.faceRight ? -1 : 1);
                    extra.name = "Hornet Boss 1 " + info.num;
                    return extra;
                })
                .ToArray();

            hornets.AddRange(extraHornets);
            

            var deathEffects = hornets[0].GetComponentInChildren<EnemyDeathEffects>(true);
            var rootType = deathEffects.GetType();
            var corpse = (GameObject)rootType.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);
            

            if (corpse != null)
            {
                PlayMakerFSM CorpseFSM = corpse.LocateMyFSM("Control");
                CorpseFSM.RemoveAction("Blow", 4);
                CorpseFSM.RemoveAction("Blow", 5);
                CorpseFSM.RemoveAction("Blow", 6);
            }
            PlayMakerFSM hornetFSM2 = hornets[1].LocateMyFSM("Control");
            PlayMakerFSM hornetFSM1 = hornets[0].LocateMyFSM("Control");


            var thrown = hornetFSM2.GetState("Thrown");
            thrown.AddCustomAction(() => { StartCoroutine(ThrowAbortTimer()); });
            

            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Hornet_1").FindGameObject("Needle") != null) {
                needle = Instantiate(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Hornet_1").FindGameObject("Needle")).AddComponent<Needle>();
                Modding.Logger.Log($"Successfully added needle");
            }
            if (GameObject.Find("Needle Tink") != null)
            {
                needleTink = Instantiate(GameObject.Find("Needle Tink")).AddComponent<NeedleTink>();
                Modding.Logger.Log($"Successfully added needleTINK");
            }


            hornetFSM2.GetState("Throw").Actions[7] = new CustomFsmAction()
            {
                method = () => {
                    needleTink.SetParent(needle.transform);
                    DoThrowNeedle();
                }
            };

            hornetFSM2.GetState("Throw Recover").Actions[1] = new CustomFsmAction()
            {
                method = () => {
                    needleTink.SetParent(null);
                }
            };

            hornetFSM2.GetState("Stun Start").Actions[14] = new CustomFsmAction()
            {
                method = () => {
                    needle.Stop();
                }
            };

            hornetFSM2.GetState("Stun Start").Actions[17] = new CustomFsmAction()
            {
                method = () => {
                    needleTink.SetParent(null);
                }
            };

            hornetFSM2.RemoveAction("Throw", 12);
            hornetFSM2.RemoveAction("Throw", 9);
            hornetFSM2.RemoveAction("Throw", 6);
            hornetFSM2.RemoveAction("Throw Recover", 4);





            Modding.Logger.Log($"Successfully deleted and edited hornets fsm");






            /*
            hornetFSM2.GetState("Throw").Actions = new HutongGames.PlayMaker.FsmStateAction[]
            {
                new CustomFsmAction()
                {
                    method = () => {
                        Vector3 currentPosition = hornets[1].transform.position;

                        Vector2 throwOrigin = currentPosition;
                        Vector2 throwDirection = Vector2.left;

                        //TODO: custom enhancement, get the unity vector direction to the hero and throw along that line
                        HeroController hero = HeroController.instance;
                        var direction = Physics2DSM.GetDirectionToTarget(hornets[1], hero.gameObject);

                        if( direction.right )
                        {
                            throwDirection = Vector2.right;
                        }
                        throwRay = new Ray( throwOrigin, throwDirection );
                        throwRaycast = Physics2D.Raycast( throwOrigin, throwDirection, throwDistance, 1 << 8 );
                        needleTink.SetParent(needle.transform);
                        needle.Play(hornets[1], throwWindUpTime, throwMaxTravelTime, throwRay, throwDistance);
                        hornetFSM2.SetState("Thrown");
                    }
                }
            };
            */
        }

        bool AllHornets()
        {
            string[] flyNames = { "Hornet Boss 1", "Hornet Boss 1 2" };

            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {

            if (AllHornets())
            {
                BossSceneController.Instance.EndBossScene();
            }
            /*
            GameObject Hornet = GameObject.Find("Hornet Boss 1 2");
            PlayMakerFSM hornetFSM2 = Hornet.LocateMyFSM("Control");
            Modding.Logger.Log($"The current state is: {hornetFSM2.ActiveStateName}");
            */
        }

        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
        protected virtual IEnumerator ThrowAbortTimer()
        {
            yield return new WaitForSeconds(throwMaxTravelTime);
            PlayMakerFSM hornetFSM1 = GameObject.Find("Hornet Boss 1 2").LocateMyFSM("Control");
            if (hornetFSM1.ActiveStateName == "Thrown")
                hornetFSM1.SendEvent("NEEDLE RETURN");
            yield break;
        }
        protected virtual void DoThrowNeedle()
        {
            var hornetNeedle = GameObject.Find("Hornet Boss 1 2");
            Vector3 currentPosition = hornetNeedle.transform.position;

            Vector2 throwOrigin = currentPosition;
            Vector2 throwDirection = Vector2.left;
            HeroController hero = HeroController.instance;


            static CollisionDirection GetDirectionToTargetHornet(GameObject self, GameObject target, float toleranceX = 0.1f, float toleranceY = 0.5f)
            {
                CollisionDirection direction = new CollisionDirection();
                float num = self.transform.position.x;
                float num2 = self.transform.position.y;
                float num3 = target.transform.position.x;
                float num4 = target.transform.position.y;

                direction.right = (num < num3) && Mathf.Abs(num - num3) > toleranceX;
                direction.left = (num > num3) && Mathf.Abs(num - num3) > toleranceX;
                direction.above = (num2 < num4) && Mathf.Abs(num2 - num4) > toleranceY;
                direction.below = (num2 > num4) && Mathf.Abs(num2 - num4) > toleranceY;

                return direction;
            }
            var direction = GetDirectionToTargetHornet(hornetNeedle, hero.gameObject);

            if (direction.right)
            {
                throwDirection = Vector2.right;
            }
            throwRay = new Ray(throwOrigin, throwDirection);
            throwRaycast = Physics2D.Raycast(throwOrigin, throwDirection, throwDistance, 1 << 8);
            needle.Play(GameObject.Find("Hornet Boss 1 2"), throwWindUpTime, throwMaxTravelTime, throwRay, throwDistance);
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class Hornet2Control : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y, bool faceRight)[] extrasInfo = new[] {
            (2, 0, 18.5f, 30.0f, true),
        };
        public float throwMaxTravelTime = .8f;
        public Needle needle;
        public NeedleTink needleTink;
        public float throwWindUpTime = .03f;
        protected Ray throwRay;
        public float throwDistance = 12f;
        protected RaycastHit2D throwRaycast;



        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Scene next = UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Hornet_2");

            GameObject BossHolder = next.GetRootGameObjects()
                    .First(go => go.name == "Boss Holder");
            PlayMakerFSM bossHolderFSM = BossHolder.LocateMyFSM("FSM");

            var hornets = new[] { BossHolder.Child("Hornet Boss 2")! }
            .ToList();

            GameObject[] extraHornets = extrasInfo
                .Map(info => {
                    GameObject prefab = hornets[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.transform.SetScaleX(info.faceRight ? -1 : 1);
                    extra.name = "Hornet Boss 2 " + info.num;
                    return extra;
                })
                .ToArray();

            hornets.AddRange(extraHornets);


            var deathEffects = hornets[0].GetComponentInChildren<EnemyDeathEffects>(true);
            var rootType = deathEffects.GetType();
            var corpse = (GameObject)rootType.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);


            if (corpse != null)
            {
                PlayMakerFSM CorpseFSM = corpse.LocateMyFSM("Control");
                CorpseFSM.RemoveAction("Blow", 4);
                CorpseFSM.RemoveAction("Blow", 5);
                CorpseFSM.RemoveAction("Blow", 6);
            }
            PlayMakerFSM hornetFSM2 = hornets[1].LocateMyFSM("Control");
            PlayMakerFSM hornetFSM1 = hornets[0].LocateMyFSM("Control");


            var thrown = hornetFSM2.GetState("Thrown");
            thrown.AddCustomAction(() => { StartCoroutine(ThrowAbortTimer()); });


            if (UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Hornet_2").FindGameObject("Needle") != null)
            {
                needle = Instantiate(UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Hornet_2").FindGameObject("Needle")).AddComponent<Needle>();
                Modding.Logger.Log($"Successfully added needle");
            }
            if (GameObject.Find("Needle Tink") != null)
            {
                needleTink = Instantiate(GameObject.Find("Needle Tink")).AddComponent<NeedleTink>();
                Modding.Logger.Log($"Successfully added needleTINK");
            }


            hornetFSM2.GetState("Throw").Actions[5] = new CustomFsmAction()
            {
                method = () => {
                    needleTink.SetParent(needle.transform);
                    DoThrowNeedle();
                }
            };

            hornetFSM2.GetState("Throw Recover").Actions[0] = new CustomFsmAction()
            {
                method = () => {
                    needleTink.SetParent(null);
                }
            };

            hornetFSM2.GetState("Stun Start").Actions[18] = new CustomFsmAction()
            {
                method = () => {
                    needle.Stop();
                }
            };

            hornetFSM2.GetState("Stun Start").Actions[19] = new CustomFsmAction()
            {
                method = () => {
                    needleTink.SetParent(null);
                }
            };

            hornetFSM2.RemoveAction("Throw", 10);
            hornetFSM2.RemoveAction("Throw", 7);
            hornetFSM2.RemoveAction("Throw", 4);
            hornetFSM2.RemoveAction("Throw Recover", 4);

            hornets.ShareHealth(0, "Horckolds");


            StartCoroutine(InitHP());
        }
        IEnumerator InitHP()
        {
            yield return new WaitForSeconds(2f);
            GameObject.Find("Hornet Boss 2").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Hornet Boss 2 2").GetComponent<HealthManager>().hp = 99999;
        }


        bool AllHornets()
        {
            string[] flyNames = { "Hornet Boss 2", "Hornet Boss 2 2" };

            return flyNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {

            if (AllHornets())
            {
                BossSceneController.Instance.EndBossScene();
            }
            /*
            GameObject Hornet = GameObject.Find("Hornet Boss 1 2");
            PlayMakerFSM hornetFSM2 = Hornet.LocateMyFSM("Control");
            Modding.Logger.Log($"The current state is: {hornetFSM2.ActiveStateName}");
            */
        }

        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
        protected virtual IEnumerator ThrowAbortTimer()
        {
            yield return new WaitForSeconds(throwMaxTravelTime);
            PlayMakerFSM hornetFSM1 = GameObject.Find("Hornet Boss 2 2").LocateMyFSM("Control");
            if (hornetFSM1.ActiveStateName == "Thrown")
                hornetFSM1.SendEvent("NEEDLE RETURN");
            yield break;
        }
        protected virtual void DoThrowNeedle()
        {
            var hornetNeedle = GameObject.Find("Hornet Boss 2 2");
            Vector3 currentPosition = hornetNeedle.transform.position;

            Vector2 throwOrigin = currentPosition;
            Vector2 throwDirection = Vector2.left;
            HeroController hero = HeroController.instance;


            static CollisionDirection GetDirectionToTargetHornet(GameObject self, GameObject target, float toleranceX = 0.1f, float toleranceY = 0.5f)
            {
                CollisionDirection direction = new CollisionDirection();
                float num = self.transform.position.x;
                float num2 = self.transform.position.y;
                float num3 = target.transform.position.x;
                float num4 = target.transform.position.y;

                direction.right = (num < num3) && Mathf.Abs(num - num3) > toleranceX;
                direction.left = (num > num3) && Mathf.Abs(num - num3) > toleranceX;
                direction.above = (num2 < num4) && Mathf.Abs(num2 - num4) > toleranceY;
                direction.below = (num2 > num4) && Mathf.Abs(num2 - num4) > toleranceY;

                return direction;
            }
            var direction = GetDirectionToTargetHornet(hornetNeedle, hero.gameObject);

            if (direction.right)
            {
                throwDirection = Vector2.right;
            }
            throwRay = new Ray(throwOrigin, throwDirection);
            throwRaycast = Physics2D.Raycast(throwOrigin, throwDirection, throwDistance, 1 << 8);
            needle.Play(GameObject.Find("Hornet Boss 2 2"), throwWindUpTime, throwMaxTravelTime, throwRay, throwDistance);
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class MossChargerControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 60.5f, 12f),
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }

            var chargers = new[] { GameObject.Find("Mega Moss Charger") }
            .ToList();



            GameObject[] extraChargers = extrasInfo
                .Map(info => {
                    GameObject prefab = chargers[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = prefab.transform.position.y };
                    extra.name = "Mega Moss Charger " + info.num;
                    return extra;
                })
                .ToArray();
            chargers.AddRange(extraChargers);
            PlayMakerFSM chargerFSM2 = chargers[1].LocateMyFSM("Mossy Control");
            chargerFSM2.GetState("Hidden").Actions = new HutongGames.PlayMaker.FsmStateAction[]{new CustomFsmAction(){method = () => { chargerFSM2.SetState("Emerge Pause");}}};
        }
        bool AllChargersDead()
        {
            string[] chargersNames = { "Mega Moss Charger", "Mega Moss Charger 2" };

            return chargersNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllChargersDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class FlukeMarmControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 25f, 5.4f),
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }

            var mothers = new[] { GameObject.Find("Fluke Mother") }
            .ToList();



            GameObject[] extraMothers = extrasInfo
                .Map(info => {
                    GameObject prefab = mothers[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = prefab.transform.position.y };
                    extra.name = "Fluke Mother " + info.num;
                    return extra;
                })
                .ToArray();
            mothers.AddRange(extraMothers);
        }
        bool AllMothersDead()
        {
            string[] mothersNames = { "Fluke Mother", "Fluke Mother 2" };

            return mothersNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllMothersDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class LordsControl : MonoBehaviour
    {
        internal static readonly (string num, int prefab, float x, float y)[] extrasInfo = new[] {
            ("2",    0, 25f, 5.4f),
        };

        List<GameObject> lords;
        bool Trigered1 = false;
        bool Trigered2 = false;

        void Start()
        {
            if (BossSequenceController.IsInSequence)
            {
                return;
            }
        }

        private IEnumerator InitStuff1(GameObject lord1)
        {
            yield return new WaitForSeconds(0.5f);
            lords = new[] { lord1 }
                .ToList();

            GameObject[] extraLords = extrasInfo
                .Map(info => {
                    GameObject prefab = lords[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = 28.51f, y = 29.14f };
                    extra.name = "Mantis Lord " + info.num;
                    return extra;
                })
                .ToArray();
            lords.AddRange(extraLords);

            var LordPairs = new[] {
                ("Mantis Lord", "Mantis Lord 2")
                };

            foreach (var (LordA, LordB) in LordPairs)
            {
                var pair = lords
                    .Filter(knight => knight.name == LordA || knight.name == LordB)
                    .ToList();


                if (pair.Count == 2)
                {
                    pair.ShareHealth(0, "Mantis Lord First");
                }
            }
            StartCoroutine(InitStuffHealth1());
        }
        IEnumerator InitStuffHealth1()
        {
            yield return new WaitForSeconds(2f);
            GameObject.Find("Mantis Lord").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Mantis Lord 2").GetComponent<HealthManager>().hp = 99999;
        }

        private IEnumerator InitStuff2(GameObject lordS2)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject BattleSube = GameObject.Find("Battle Sub");
            Transform subTransform = BattleSube.transform;
            List<Transform> children = new List<Transform>();
            foreach (Transform child in subTransform)
            {
                children.Add(child);
                child.parent = null;
            }

            var extraSub = GameObject.Instantiate(BattleSube, BattleSube.transform.parent);
            extraSub.name = "Battle Sub 2";

            foreach (Transform child in children)
            {
                child.parent = BattleSube.transform;
            }

            foreach (Transform child in extraSub.transform)
            {
                if (child.name == "Mantis Lord S2" || child.name == "Mantis Lord S1")
                {
                    Destroy(child.gameObject);
                }
            }

            GameObject lordS1 = GameObject.Find("Mantis Lord S1");
            var extra1 = GameObject.Instantiate(lordS1, extraSub.transform);
            extra1.transform.position = lordS1.transform.position with { x = 28.51f, y = 29.14f };
            extra1.name = "Mantis Lord S1 2";

            var extra2 = GameObject.Instantiate(lordS2, extraSub.transform);
            extra2.transform.position = lordS2.transform.position with { x = 28.51f, y = 29.14f };
            extra2.name = "Mantis Lord S2 2";

            GameObject[] extraextraLords = [extra1, extra2];
            lords.AddRange(extraextraLords);

            GameObject MantisBattle = GameObject.Find("Mantis Battle");
            PlayMakerFSM BattleControl = MantisBattle.LocateMyFSM("Battle Control");
            BattleControl.FsmVariables.FindFsmInt("Battle Enemies").Value = lords.Count;


            PlayMakerFSM extraSubFSM = extraSub.LocateMyFSM("Start");

            extraSubFSM.GetState("Init").Actions[1] = new CustomFsmAction()
            {
                method = () => {
                    extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 1").Value = GameObject.Find("Mantis Lord S1 2");
                }
            };
            extraSubFSM.GetState("Init").Actions[2] = new CustomFsmAction()
            {
                method = () => {
                    extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 2").Value = GameObject.Find("Mantis Lord S2 2");
                }
            };

            extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 1").Value = GameObject.Find("Mantis Lord S1 2");
            extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 2").Value = GameObject.Find("Mantis Lord S2 2");

            extraSubFSM.SetState("Init Pause");


        }


        void Update()
        {
            GameObject lord1 = GameObject.Find("Mantis Lord");
            if (lord1 != null && !Trigered1)
            {
                StartCoroutine(InitStuff1(lord1));
                Trigered1 = true;
            }

            GameObject lordS2 = GameObject.Find("Mantis Lord S2");

            if (lordS2 != null && !Trigered2)
            {
                StartCoroutine(InitStuff2(lordS2));
                Trigered2 = true;
            }

            GameObject BattleSub = GameObject.Find("Battle Sub 2");
            PlayMakerFSM BattleSubFSM = BattleSub.LocateMyFSM("Start");
            if (BattleSub != null)
            {
                foreach (Transform child in BattleSub.transform)
                {
                    Modding.Logger.Log($"Kids: {child.gameObject}");
                }
                Modding.Logger.Log($"The current state is: {BattleSubFSM.ActiveStateName}");
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class SistersControl : MonoBehaviour
    {
        internal static readonly (string num, int prefab, float x, float y)[] extrasInfo = new[] {
            ("2",    0, 25f, 5.4f),
        };

        List<GameObject> lords;
        bool Trigered1 = false;
        bool Trigered2 = false;
        
        void Start()
        {
            if (BossSequenceController.IsInSequence)
            {
                return;
            }
        }

        private IEnumerator InitStuff1(GameObject lord1)
        {
            yield return new WaitForSeconds(0.5f);
            lords = new[] { lord1}
            .ToList();

            GameObject[] extraLords = extrasInfo
                .Map(info => {
                    GameObject prefab = lords[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = 28.51f, y = 29.14f };
                    extra.name = "Mantis Lord " + info.num;
                    return extra;
                })
                .ToArray();
            lords.AddRange(extraLords);

            var LordPairs = new[] {
                ("Mantis Lord", "Mantis Lord 2")
                };

            foreach (var (LordA, LordB) in LordPairs)
            {
                var pair = lords
                    .Filter(knight => knight.name == LordA || knight.name == LordB)
                    .ToList();


                if (pair.Count == 2)
                {
                    pair.ShareHealth(0, "Mantis Lord First");
                }
            }
            StartCoroutine(InitStuffHealth1());
        }
        IEnumerator InitStuffHealth1()
        {
            yield return new WaitForSeconds(2f);
            GameObject.Find("Mantis Lord").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Mantis Lord 2").GetComponent<HealthManager>().hp = 99999;
        }

        private IEnumerator InitStuff2(GameObject lordS3)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject BattleSube = GameObject.Find("Battle Sub");

            Transform subTransform = BattleSube.transform;
            List<Transform> children = new List<Transform>();
            foreach (Transform child in subTransform)
            {
                children.Add(child);
                child.parent = null;
            }

            var extraSub = GameObject.Instantiate(BattleSube, BattleSube.transform.parent);
            extraSub.name = "Battle Sub 2";

            foreach (Transform child in children)
            {
                child.parent = BattleSube.transform;
            }

            foreach (Transform child in extraSub.transform)
            {
                if (child.name == "Mantis Lord S1" || child.name == "Mantis Lord S2" || child.name == "Mantis Lord S3")
                {
                    Destroy(child.gameObject);
                }
            }


            GameObject lordS1 = GameObject.Find("Mantis Lord S1");
            var extra1 = GameObject.Instantiate(lordS1, extraSub.transform);
            extra1.transform.position = lordS1.transform.position with { x = 28.51f, y = 29.14f };
            extra1.name = "Mantis Lord S1 2";

            GameObject lordS2 = GameObject.Find("Mantis Lord S2");
            var extra2 = GameObject.Instantiate(lordS2, extraSub.transform);
            extra2.transform.position = lordS2.transform.position with { x = 28.51f, y = 29.14f };
            extra2.name = "Mantis Lord S2 2";

            var extra3 = GameObject.Instantiate(lordS3, extraSub.transform);
            extra3.transform.position = lordS3.transform.position with { x = 28.51f, y = 29.14f };
            extra3.name = "Mantis Lord S3 2";


            GameObject MantisBattle = GameObject.Find("Mantis Battle");
            PlayMakerFSM BattleControl = MantisBattle.LocateMyFSM("Battle Control");
            Modding.Logger.Log($"Battle Eneemies: {BattleControl.FsmVariables.FindFsmInt("Battle Enemies").Value}");

            GameObject[] extraextraLords = [extra1, extra2, extra3];
            lords.AddRange(extraextraLords);
            //sorry for the magic number. It really should be six, but for some unknown for me reason it gets 5 out of "lords.Count". Idk why so i'm just sticking a number here :(
            BattleControl.FsmVariables.FindFsmInt("Battle Enemies").Value = 6;
            Modding.Logger.Log($"Battle Eneemies: {BattleControl.FsmVariables.FindFsmInt("Battle Enemies").Value}");


            PlayMakerFSM extraSubFSM = extraSub.LocateMyFSM("Start");

            extraSubFSM.GetState("Init").Actions[1] = new CustomFsmAction()
            {
                method = () => {
                    extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 1").Value = GameObject.Find("Mantis Lord S1 2");
                }
            };
            extraSubFSM.GetState("Init").Actions[2] = new CustomFsmAction()
            {
                method = () => {
                    extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 2").Value = GameObject.Find("Mantis Lord S2 2");
                }
            };
            extraSubFSM.GetState("Init").Actions[3] = new CustomFsmAction()
            {
                method = () => {
                    extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 3").Value = GameObject.Find("Mantis Lord S3 2");
                }
            };

            extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 1").Value = GameObject.Find("Mantis Lord S1 2");
            extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 2").Value = GameObject.Find("Mantis Lord S2 2");
            extraSubFSM.FsmVariables.FindFsmGameObject("Mantis 3").Value = GameObject.Find("Mantis Lord S3 2");

            extraSubFSM.SetState("Init Pause");


        }



        void Update()
        {
            GameObject MantisBattle = GameObject.Find("Mantis Battle");
            PlayMakerFSM BattleControl = MantisBattle.LocateMyFSM("Battle Control");
            Modding.Logger.Log($"Battle Enemies: {BattleControl.FsmVariables.FindFsmInt("Battle Enemies").Value}");

            GameObject lord1 = GameObject.Find("Mantis Lord");
            if (lord1 != null && !Trigered1)
            {
                StartCoroutine(InitStuff1(lord1));
                Trigered1 = true;
            }

            GameObject lordS3 = GameObject.Find("Mantis Lord S3");
            if (lordS3 != null && !Trigered2)
            {
                StartCoroutine(InitStuff2(lordS3));
                Trigered2 = true;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class OblobblesControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 90.71f, 13.67f),
            (2, 1, 114.7f, 15.01f),
        };
        List<GameObject> oblobbles;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            StartCoroutine(InitStuff());

        }
        private IEnumerator InitStuff()
        {
            yield return new WaitForSeconds(2f);
            oblobbles = new[] { GameObject.Find("Mega Fat Bee"), GameObject.Find("Mega Fat Bee (1)") }.ToList();

            GameObject[] extraOblobbles = extrasInfo
                .Map(info => {
                    GameObject prefab = oblobbles[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            oblobbles.AddRange(extraOblobbles);
        }
        bool AllOblobblesDead()
        {
            string[] oblobblesNames = { "Mega Fat Bee", "Mega Fat Bee (1)", "Mega Fat Bee 2", "Mega Fat Bee (1) 2" };

            return oblobblesNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllOblobblesDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class HiveKnightControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 67.67f, 40.34f),
        };
        List<GameObject> HiveKnights;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ***************************************************");
            StartCoroutine(InitStuff());

        }
        private IEnumerator InitStuff()
        {
            yield return new WaitForSeconds(5f);
            GameObject hiveKnight = GameObject.Find("Hive Knight");
            HiveKnights = new[] { hiveKnight }.ToList();
            GameObject[] extraHiveKnights = extrasInfo
                .Map(info => {
                    GameObject prefab = HiveKnights[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = 28.69393f };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            HiveKnights.AddRange(extraHiveKnights);

            GameObject Globs = GameObject.Find("Globs");
            var extraGlobs= GameObject.Instantiate(Globs, Globs.transform.parent);
            var extraGlobsFSM = extraGlobs.LocateMyFSM("Control");
            extraGlobs.name = "Globs 2";

            GameObject beeDropper1 = GameObject.Find("Bee Dropper");
            var extraDropper1 = GameObject.Instantiate(beeDropper1, beeDropper1.transform.parent);
            var extraDropper1FSM = extraDropper1.LocateMyFSM("Control");
            extraDropper1.name = "Bee Dropper 2";
            extraDropper1.transform.parent = null;

            GameObject beeDropper2 = GameObject.Find("Bee Dropper (1)");
            var extraDropper2 = GameObject.Instantiate(beeDropper2, beeDropper2.transform.parent);
            var extraDropper2FSM = extraDropper2.LocateMyFSM("Control");
            extraDropper2.name = "Bee Dropper (1) 2";
            extraDropper2.transform.parent = null;

            GameObject beeDropper3 = GameObject.Find("Bee Dropper (2)");
            var extraDropper3 = GameObject.Instantiate(beeDropper3, beeDropper3.transform.parent);
            var extraDropper3FSM = extraDropper3.LocateMyFSM("Control");
            extraDropper3.name = "Bee Dropper (2) 2";
            extraDropper3.transform.parent = null;

            GameObject beeDropper4 = GameObject.Find("Bee Dropper (3)");
            var extraDropper4 = GameObject.Instantiate(beeDropper4, beeDropper4.transform.parent);
            var extraDropper4FSM = extraDropper4.LocateMyFSM("Control");
            extraDropper4.name = "Bee Dropper (3) 2";
            extraDropper4.transform.parent = null;

            GameObject beeDropper5 = GameObject.Find("Bee Dropper (4)");
            var extraDropper5 = GameObject.Instantiate(beeDropper5, beeDropper5.transform.parent);
            var extraDropper5FSM = extraDropper5.LocateMyFSM("Control");
            extraDropper5.name = "Bee Dropper (4) 2";
            extraDropper5.transform.parent = null;

            GameObject beeDropper6 = GameObject.Find("Bee Dropper (5)");
            var extraDropper6 = GameObject.Instantiate(beeDropper6, beeDropper6.transform.parent);
            var extraDropper6FSM = extraDropper6.LocateMyFSM("Control");
            extraDropper1.name = "Bee Dropper (5) 2";
            extraDropper6.transform.parent = null;

            GameObject beeDropper7 = GameObject.Find("Bee Dropper (6)");
            var extraDropper7 = GameObject.Instantiate(beeDropper7, beeDropper7.transform.parent);
            var extraDropper7FSM = extraDropper7.LocateMyFSM("Control");
            extraDropper7.name = "Bee Dropper (6) 2";
            extraDropper7.transform.parent = null;


            PlayMakerFSM hiveKnightFSM = HiveKnights[1].LocateMyFSM("Control");
            hiveKnightFSM.Fsm.GetFsmFloat("Left X").Value = 15f;
            hiveKnightFSM.Fsm.GetFsmFloat("Right X").Value = 35f;
            hiveKnightFSM.Fsm.GetFsmFloat("Ground Y").Value = 29f;
            hiveKnightFSM.GetState("Glob Strike").Actions[3] = new CustomFsmAction()
            {
                method = () => {
                    extraGlobsFSM.SendEvent("FIRE");

                }
            };
            hiveKnightFSM.GetState("Roar Recover").Actions[0] = new CustomFsmAction()
            {
                method = () => {
                    extraDropper1FSM.SendEvent("SWARM");
                    extraDropper2FSM.SendEvent("SWARM");
                    extraDropper3FSM.SendEvent("SWARM");
                    extraDropper4FSM.SendEvent("SWARM");
                    extraDropper5FSM.SendEvent("SWARM");
                    extraDropper6FSM.SendEvent("SWARM");
                    extraDropper7FSM.SendEvent("SWARM");

                }
            };
            hiveKnightFSM.DisableAction("Fall",4);
            hiveKnightFSM.DisableAction("Fall",2);
            hiveKnightFSM.DisableAction("Fall",1);
            hiveKnightFSM.DisableAction("Intro", 3);
            hiveKnightFSM.DisableAction("Intro", 1);
            hiveKnightFSM.DisableAction("Intro End", 2);
            hiveKnightFSM.DisableAction("Jump", 3);
        }
        bool AllHiveKnightsDead()
        {
            string[] hiveKnightsNames = { "Hive Knight", "Hive Knight 2" };

            return hiveKnightsNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllHiveKnightsDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            PlayMakerFSM fsm = GameObject.Find("Hive Knight 2").LocateMyFSM("Control");
            if (fsm != null)
            {
                if (fsm.ActiveStateName == "Sleep")
                {
                    fsm.SendEvent("WAKE");
                }
                else if (fsm.ActiveStateName == "Fall")
                {
                    Collider2D hiveKnightCollider = fsm.gameObject.GetComponent<Collider2D>();
                    hiveKnightCollider.enabled = true;
                    fsm.SendEvent("LAND");
                }
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class BrokenVesselControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 18.92f, 38.82f),
        };
        List<GameObject> BrokenVessels;
        bool doneCorpse = false;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            BrokenVessels = new[] { GameObject.Find("Infected Knight")}.ToList();
            var deathEffects = BrokenVessels[0].GetComponentInChildren<EnemyDeathEffects>(true);
            Modding.Logger.Log($"{deathEffects}");
            var rootType = deathEffects.GetType();

            var corpse = (GameObject)rootType.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);
            Modding.Logger.Log($"{corpse}");
            if (corpse != null)
            {
                Modding.Logger.Log($"We got into Corpse!");
                PlayMakerFSM corpseFSM = corpse.LocateMyFSM("corpse");
                corpseFSM.DisableAction("Blow", 7);
                corpseFSM.DisableAction("BG Open", 1);
                Modding.Logger.Log($"We done doin the Corpse!!!!");
            }

            GameObject[] extraBrokenVessels = extrasInfo
                .Map(info => {
                    GameObject prefab = BrokenVessels[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            BrokenVessels.AddRange(extraBrokenVessels);
        }
        bool AllBrokenVesselsDead()
        {
            string[] BrokenVesselsNames = { "Infected Knight", "Infected Knight 2" };

            return BrokenVesselsNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllBrokenVesselsDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            if (doneCorpse != true)
            {
                var deathEffects2 = BrokenVessels[1].GetComponentInChildren<EnemyDeathEffects>(true);
                Modding.Logger.Log($"{deathEffects2}");
                var rootType2 = deathEffects2.GetType();

                var corpse2 = (GameObject)rootType2.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects2);
                Modding.Logger.Log($"{corpse2}");
                if (corpse2 != null)
                {
                    Modding.Logger.Log($"We got into Corpse!");
                    PlayMakerFSM corpseFSM2 = corpse2.LocateMyFSM("corpse");
                    corpseFSM2.DisableAction("Blow", 7);
                    corpseFSM2.DisableAction("BG Open", 1);
                    Modding.Logger.Log($"We done doin the Corpse!!!!");
                    doneCorpse = true;
                }
            }


        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class LostKinControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 18.92f, 38.82f),
        };
        List<GameObject> LostKins;
        bool doneCorpse = false;
        bool doneDreamOrb = false;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            LostKins = new[] { GameObject.Find("Lost Kin") }.ToList();
            var deathEffects = LostKins[0].GetComponentInChildren<EnemyDeathEffects>(true);
            Modding.Logger.Log($"{deathEffects}");
            var rootType = deathEffects.GetType();

            var corpse = (GameObject)rootType.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);
            Modding.Logger.Log($"{corpse}");
            if (corpse != null)
            {
                Modding.Logger.Log($"We got into Corpse!");
                PlayMakerFSM corpseFSM = corpse.LocateMyFSM("corpse");
                corpseFSM.DisableAction("BG Open", 2);
                Modding.Logger.Log($"We done doin the Corpse!!!!");
            }

            GameObject[] extraLostKins = extrasInfo
                .Map(info => {
                    GameObject prefab = LostKins[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            LostKins.AddRange(extraLostKins);
            PlayMakerFSM Kin2FSM = LostKins[1].LocateMyFSM("IK Control");
            Kin2FSM.DisableAction("Set X GG", 0);
        }
        bool AllLostKinsDead()
        {
            string[] LostKinsNames = { "Lost Kin", "Lost Kin 2" };

            return LostKinsNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllLostKinsDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            if (doneCorpse != true)
            {
                var deathEffects2 = LostKins[1].GetComponentInChildren<EnemyDeathEffects>(true);
                Modding.Logger.Log($"{deathEffects2}");
                var rootType2 = deathEffects2.GetType();

                var corpse2 = (GameObject)rootType2.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects2);
                Modding.Logger.Log($"{corpse2}");
                if (corpse2 != null)
                {
                    Modding.Logger.Log($"We got into Corpse!");
                    PlayMakerFSM corpseFSM2 = corpse2.LocateMyFSM("corpse");
                    corpseFSM2.DisableAction("BG Open", 2);
                    Modding.Logger.Log($"We done doin the Corpse!!!!");
                    doneCorpse = true;
                }
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class NoskControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 77.33f, 4.4f),
        };
        List<GameObject> Nosks;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            Nosks = new[] { GameObject.Find("Mimic Spider") }.ToList();
            GameObject[] extraNosks = extrasInfo
                .Map(info => {
                    GameObject prefab = Nosks[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            Nosks.AddRange(extraNosks);
        }
        bool AllNosksDead()
        {
            string[] NosksNames = { "Mimic Spider", "Mimic Spider 2" };

            return NosksNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllNosksDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class NoskVControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 79.00f, 5.4f),
        };
        List<GameObject> Nosks;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            Nosks = new[] { GameObject.Find("Mimic Spider") }.ToList();
            GameObject[] extraNosks = extrasInfo
                .Map(info => {
                    GameObject prefab = Nosks[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = info.y };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            Nosks.AddRange(extraNosks);
        }
        bool AllNosksDead()
        {
            string[] NosksNames = { "Mimic Spider", "Mimic Spider 2" };

            return NosksNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllNosksDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class NoskHornetControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 38.34f, 18.00f),
        };
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            GameObject BattleSube = GameObject.Find("Battle Scene");
            Transform subTransform = BattleSube.transform;
            List<Transform> children = new List<Transform>();
            foreach (Transform child in subTransform)
            {
                children.Add(child);
                child.parent = null;
            }

            var extraSub = GameObject.Instantiate(BattleSube, BattleSube.transform.parent);
            extraSub.name = "Battle Scene 2";

            foreach (Transform child in children)
            {
                child.parent = BattleSube.transform;
            }

            foreach (Transform child in extraSub.transform)
            {
                if (child.name == "Hornet Nosk" || child.name == "Nosk Transform")
                {
                    Destroy(child.gameObject);
                }
            }
            PlayMakerFSM extraSubFSM = extraSub.LocateMyFSM("Battle Control");

            GameObject noskTransform = GameObject.Find("Nosk Transform");
            var extraTransform = GameObject.Instantiate(noskTransform, extraSub.transform);
            extraTransform.transform.position = noskTransform.transform.position with { x = 38.34f, y = 14.80f };
            extraTransform.transform.SetScaleX(-1);
            extraTransform.name = "Nosk Transform 2";
            var colliderHornetTransform = extraTransform.GetComponent<Collider2D>();
            colliderHornetTransform.enabled = false;
            PlayMakerFSM extraTransformFSM = extraTransform.LocateMyFSM("detect");
            extraTransformFSM.RemoveAction("Detect", 2);
            extraTransformFSM.RemoveAction("Detect", 1);
            extraTransformFSM.RemoveAction("Detect", 0);
            extraTransformFSM.RemoveAction("Detect No Stay", 1);
            extraTransformFSM.RemoveAction("Detect No Stay", 0);


            GameObject noskHornet = GameObject.Find("Hornet Nosk");
            var extraNoskHornet = GameObject.Instantiate(noskHornet, extraSub.transform);
            extraNoskHornet.transform.position = noskHornet.transform.position with { x = 38.34f, y = 14.80f };
            extraNoskHornet.transform.SetScaleX(-1);
            extraNoskHornet.name = "Hornet Nosk 2";
            var meshHornet = extraNoskHornet.GetComponent<MeshRenderer>();
            var colliderHornet = extraNoskHornet.GetComponent<Collider2D>();
            var origHornetcollider = noskHornet.GetComponent<Collider2D>();
            var extraNoskHornetHead = extraNoskHornet.Child("Head Box");
            extraNoskHornetHead.active = false;
            meshHornet.enabled = false;
            colliderHornet.enabled = false;

            var buzDust1 = GameObject.Find("Buzzer Dust (1)").GetComponent<Collider2D>();
            var buzDust2 = GameObject.Find("Buzzer Dust (2)").GetComponent<Collider2D>();
            var buzDust3 = GameObject.Find("Buzzer Dust (3)").GetComponent<Collider2D>();
            var buzDust4 = GameObject.Find("Buzzer Dust (4)").GetComponent<Collider2D>();
            var buzDust5 = GameObject.Find("Buzzer Dust (5)").GetComponent<Collider2D>();
            var buzDust6 = GameObject.Find("Buzzer Dust 1 (1)").GetComponent<Collider2D>();
            var buzDust7 = GameObject.Find("Buzzer Dust 2 (1)").GetComponent<Collider2D>();
            var buzDust8 = GameObject.Find("Buzzer Dust 3 (1)").GetComponent<Collider2D>();
            var buzDust9 = GameObject.Find("Buzzer Dust 4 (1)").GetComponent<Collider2D>();
            var buzDust10= GameObject.Find("Buzzer Dust 5 (1)").GetComponent<Collider2D>();
            var buzDust11= GameObject.Find("Buzzer Dust 6 (1)").GetComponent<Collider2D>();



            Physics2D.IgnoreCollision(colliderHornet, origHornetcollider);
            Physics2D.IgnoreCollision(colliderHornet, buzDust1);
            Physics2D.IgnoreCollision(colliderHornet, buzDust2);
            Physics2D.IgnoreCollision(colliderHornet, buzDust3);
            Physics2D.IgnoreCollision(colliderHornet, buzDust4);
            Physics2D.IgnoreCollision(colliderHornet, buzDust5);
            Physics2D.IgnoreCollision(colliderHornet, buzDust6);
            Physics2D.IgnoreCollision(colliderHornet, buzDust7);
            Physics2D.IgnoreCollision(colliderHornet, buzDust8);
            Physics2D.IgnoreCollision(colliderHornet, buzDust9);
            Physics2D.IgnoreCollision(colliderHornet, buzDust10);
            Physics2D.IgnoreCollision(colliderHornet, buzDust11);


            Physics2D.IgnoreCollision(origHornetcollider, buzDust1);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust2);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust3);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust4);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust5);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust6);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust7);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust8);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust9);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust10);
            Physics2D.IgnoreCollision(origHornetcollider, buzDust11);



            GameObject globDropper = GameObject.Find("Glob Dropper");
            var extraGlobDropper = GameObject.Instantiate(globDropper, extraSub.transform);
            extraGlobDropper.name = "Glob Dropper 2";
            PlayMakerFSM extraGlobDropperFSM = extraGlobDropper.LocateMyFSM("Dropper");


            extraSubFSM.RemoveAction("Init", 8);
            extraSubFSM.RemoveAction("Init", 7);
            extraSubFSM.RemoveAction("Init", 6);
            extraSubFSM.RemoveAction("Init", 3);
            extraSubFSM.RemoveAction("Init", 2);
            extraSubFSM.RemoveAction("Init", 1);
            extraSubFSM.RemoveAction("Init", 0);
            extraSubFSM.RemoveAction("GG Music", 1);
            extraSubFSM.RemoveAction("GG Music", 0);
            extraSubFSM.RemoveAction("Arena", 2);
            extraSubFSM.RemoveAction("Arena", 0);
            extraSubFSM.RemoveAction("Transform 1", 6);
            extraSubFSM.RemoveAction("Transform 1", 3);
            extraSubFSM.RemoveAction("Transform 1", 2);
            extraSubFSM.RemoveAction("Transform 1", 1);
            extraSubFSM.RemoveAction("Transform 1", 0);
            extraSubFSM.RemoveAction("Transform 2", 1);
            extraSubFSM.RemoveAction("Transform 3", 1);
            extraSubFSM.RemoveAction("Transform 4", 7);
            extraSubFSM.RemoveAction("Transform 4", 6);
            extraSubFSM.RemoveAction("Transform 4", 5);
            extraSubFSM.RemoveAction("Transform 4", 4);
            extraSubFSM.RemoveAction("Transform 4", 3);
            extraSubFSM.RemoveAction("Transform 4", 1);
            extraSubFSM.RemoveAction("Battle Start", 11);
            extraSubFSM.RemoveAction("Battle Start", 10);
            extraSubFSM.RemoveAction("Battle Start", 9);
            extraSubFSM.RemoveAction("Battle Start", 8);
            extraSubFSM.RemoveAction("Battle Start", 7);
            extraSubFSM.RemoveAction("Battle Start", 6);
            extraSubFSM.RemoveAction("Battle Start", 5);
            extraSubFSM.RemoveAction("Battle Start", 4);

            PlayMakerFSM BattleSubeFSM = BattleSube.LocateMyFSM("Battle Control");

            BattleSubeFSM.GetState("Arena").Actions = new HutongGames.PlayMaker.FsmStateAction[]
            {
                new CustomFsmAction()
                {
                    method = () => {
                        extraSubFSM.SendEvent("ENTER");
                        GameObject.Find("Entry Plat").active = false;
                    }
                }
            };

            extraSubFSM.GetState("Init").Actions[0] = new CustomFsmAction()
            {
                method = () => {
                    extraSubFSM.FsmVariables.FindFsmGameObject("Hornet Nosk").Value = extraNoskHornet;
                }
            };
            extraSubFSM.GetState("Init").Actions[1] = new CustomFsmAction()
            {
                method = () => {
                    extraSubFSM.FsmVariables.FindFsmGameObject("Nosk Transform").Value = extraTransform;
                }
            };


            PlayMakerFSM extraNoskHornetFSM = extraNoskHornet.LocateMyFSM("Hornet Nosk");
            PlayMakerFSM noskHornetFSM = noskHornet.LocateMyFSM("Hornet Nosk");
            extraNoskHornetFSM.GetState("Set Pos").Actions[0] = new CustomFsmAction()
            {
                method = () => {
                    meshHornet.enabled = true;
                }
            };
            extraNoskHornetFSM.GetState("Init").Actions[8] = new CustomFsmAction()
            {
                method = () => {
                    extraNoskHornetFSM.FsmVariables.FindFsmGameObject("Glob Dropper").Value = extraGlobDropper;
                }
            };
            extraNoskHornetFSM.GetState("Globs").Actions[0] = new CustomFsmAction()
            {
                method = () => {
                    extraGlobDropperFSM.SendEvent("DROP");
                }
            };
            /*
            extraNoskHornetFSM.GetState("Summon").Actions[1] = new CustomFsmAction()
            {
                method = () => {
                    if (noskHornetFSM.FsmVariables.FindFsmInt("Enemy Count").Value >= 12)
                    {
                        noskHornetFSM.SendEvent("FINISHED");
                    }
                    else
                    {
                        noskHornetFSM.SendEvent("");
                    }
                }
            };
            noskHornetFSM.GetState("Summon").Actions[1] = new CustomFsmAction()
            {
                method = () => {
                    if (noskHornetFSM.FsmVariables.FindFsmInt("Enemy Count").Value >= 12)
                    {
                        noskHornetFSM.SendEvent("FINISHED");
                    }
                    else
                    {
                        noskHornetFSM.SendEvent("");
                    }
                }
            };
            */
            extraSubFSM.FsmVariables.FindFsmGameObject("Hornet Nosk").Value         = extraNoskHornet;
            extraSubFSM.FsmVariables.FindFsmGameObject("Nosk Transform").Value      = extraTransform;
            extraNoskHornetFSM.FsmVariables.FindFsmGameObject("Glob Dropper").Value = extraGlobDropper;

        }
        bool AllHornetNosksDead()
        {
            string[] HornetNosksNames = { "Hornet Nosk", "Hornet Nosk 2" };

            return HornetNosksNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllHornetNosksDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            var balloonSpawners = GameObject.FindObjectsOfType<GameObject>()
            .Where(go => go.name == "Enemy Pusher")
            .ToArray();

            Modding.Logger.Log($"Found {balloonSpawners.Length} balloon spawners");
            
            foreach (var spawner in balloonSpawners)
            {
                var buzDustSpawnerCol = spawner.GetComponent<CircleCollider2D>();
                var colliderHornet = GameObject.Find("Hornet Nosk 2").GetComponent<Collider2D>();
                var origHornetCollider = GameObject.Find("Hornet Nosk").GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(colliderHornet, buzDustSpawnerCol);
                Physics2D.IgnoreCollision(origHornetCollider, buzDustSpawnerCol);
                Modding.Logger.Log($"Ignoring collisions for spawner: {spawner.name}");
            }
            /*
            var allColliders = FindObjectsOfType<CircleCollider2D>();
            var allColliders2D = FindObjectsOfType<Collider2D>();
            for (int i = 0; i < allColliders.Length; i++)
            {
                for (int j = i + 1; j < allColliders2D.Length; j++)
                {
                    var col1 = allColliders[i];
                    var col2 = allColliders2D[j];

                    if (col1 != null && col2 != null &&
                        col1.enabled && col2.enabled &&
                        col1.IsTouching(col2))
                    {
                        Modding.Logger.Log($"FRAME {Time.frameCount}: COLLISION - {col1.gameObject.name} <-> {col2.gameObject.name}");
                    }
                }
            }*/



        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class CollectorControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 77.33f, 4.4f),
        };
        List<GameObject> Collectors;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            Collectors = new[] { GameObject.Find("Jar Collector") }.ToList();
            GameObject[] extraCollectors = extrasInfo
                .Map(info => {
                    GameObject prefab = Collectors[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = prefab.transform.GetPositionX(), y = prefab.transform.GetPositionY() };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            Collectors.AddRange(extraCollectors);
        }
        bool AllCollectorsDead()
        {
            string[] CollectorsNames = { "Jar Collector", "Jar Collector 2" };

            return CollectorsNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllCollectorsDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class CollectorVControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 77.33f, 4.4f),
        };
        List<GameObject> CollectorsV;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            CollectorsV = new[] { GameObject.Find("Jar Collector") }.ToList();
            GameObject[] extraCollectors = extrasInfo
                .Map(info => {
                    GameObject prefab = CollectorsV[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = prefab.transform.GetPositionX(), y = prefab.transform.GetPositionY() };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            CollectorsV.AddRange(extraCollectors);
            CollectorsV.ShareHealth(0, "Collectors");


            StartCoroutine(InitHP());
        }
        IEnumerator InitHP()
        {
            yield return new WaitForSeconds(2f);
            GameObject.Find("Jar Collector").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Jar Collector 2").GetComponent<HealthManager>().hp = 99999;
        }
        bool AllCollectorsVDead()
        {
            string[] CollectorsVNames = { "Jar Collector", "Jar Collector 2" };

            return CollectorsVNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllCollectorsVDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0) // Log only if there are tracked bosses
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class TamerControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 90.36f, 6.5f),
        };
        List<GameObject> Lancers;
        PlayMakerFSM Lancer1FSMControl;
        PlayMakerFSM Lancer2FSMControl;

        PlayMakerFSM Lancer1FSMDeath;
        PlayMakerFSM Lancer2FSMDeath;

        bool doneCorpse = false;

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            Lancers = new[] { GameObject.Find("Lancer") }.ToList();
            GameObject[] extraLancers = extrasInfo
                .Map(info => {
                    GameObject prefab = Lancers[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = info.x, y = prefab.transform.GetPositionY() };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            Lancers.AddRange(extraLancers);
            GameObject prefabLobster = GameObject.Find("Lobster");
            GameObject extraLobster = GameObject.Instantiate(prefabLobster);
            extraLobster.name = prefabLobster.name + " 2";
            extraLobster.transform.position = prefabLobster.transform.position with { x = 90.36f, y = prefabLobster.transform.GetPositionY() };

            Lancer1FSMControl = Lancers[0].LocateMyFSM("Control");
            Lancer2FSMControl = Lancers[1].LocateMyFSM("Control");

            Lancer1FSMDeath = Lancers[0].LocateMyFSM("Death Detect");
            Lancer2FSMDeath = Lancers[1].LocateMyFSM("Death Detect");

            PlayMakerFSM extraLobsterFSM = extraLobster.LocateMyFSM("Control");
            extraLobsterFSM.SendEvent("WAKE");
            var deathEffects = prefabLobster.GetComponentInChildren<EnemyDeathEffects>(true);
            Modding.Logger.Log($"{deathEffects}");
            var rootType = deathEffects.GetType();
            var corpse = (GameObject)rootType.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);
            Modding.Logger.Log($"{corpse}");
            if (corpse != null)
            {
                Modding.Logger.Log($"We got into Corpse!");
                PlayMakerFSM corpseFSM = corpse.LocateMyFSM("Death");
                corpseFSM.DisableAction("Init", 9);
                corpseFSM.DisableAction("Init", 8);
                corpseFSM.DisableAction("Init", 6);

                corpseFSM.DisableAction("Steam", 11);
                corpseFSM.DisableAction("Steam", 10);
                corpseFSM.DisableAction("Steam", 6);
                corpseFSM.DisableAction("Steam", 5);
                corpseFSM.DisableAction("Steam", 4);
                corpseFSM.DisableAction("Steam", 2);
                corpseFSM.DisableAction("Steam", 1);

                corpseFSM.DisableAction("Ready", 2);
                corpseFSM.DisableAction("Ready", 1);

                corpseFSM.DisableAction("Blow",  10);
                corpseFSM.DisableAction("Blow",  9);
                corpseFSM.DisableAction("Blow",  8);
                corpseFSM.DisableAction("Blow",  7);
                corpseFSM.DisableAction("Blow",  6);
                corpseFSM.DisableAction("Blow",  4);
                corpseFSM.DisableAction("Blow",  2);
                Modding.Logger.Log($"We done doin the Corpse!!!!");
            }
            Lancer2FSMControl.GetState("Init").Actions[6] = new CustomFsmAction()
            {
                method = () => Lancer2FSMControl.FsmVariables.FindFsmGameObject("Lobster").Value = extraLobster
            };

            Lancer1FSMDeath.GetState("Set").Actions = new HutongGames.PlayMaker.FsmStateAction[]
            {
                new CustomFsmAction()
                {
                    method = () => {
                        DoubleBosses.BossDeathStatus["Lancer"] = true;
                        Lancer1FSMControl.FsmVariables.FindFsmBool("Death").Value = true;
                        //Lancers[0].LocateMyFSM("recoil").FsmVariables.FindFsmInt("Recoil per second").Value = 0;
                    }
                }
            };
            Lancer2FSMDeath.GetState("Set").Actions = new HutongGames.PlayMaker.FsmStateAction[]
            {
                new CustomFsmAction()
                {
                    method = () => {
                        DoubleBosses.BossDeathStatus["Lancer 2"] = true;
                        Lancer2FSMControl.FsmVariables.FindFsmBool("Death").Value = true;
                        //Lancers[1].LocateMyFSM("recoil").FsmVariables.FindFsmInt("Recoil per second").Value = 0;
                    }
                }
            };

            /*
            Lancer1FSMDeath.RemoveAction("Set", 1);
            Lancer1FSMDeath.RemoveAction("Set", 0);
            Lancer2FSMDeath.RemoveAction("Set", 1);
            Lancer2FSMDeath.RemoveAction("Set", 0);
            */
        }
        bool AllLancersDead()
        {
            string[] LancersNames = { "Lancer", "Lancer 2", "Lobster", "Lobster 2" };

            return LancersNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllLancersDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0)
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            if (doneCorpse != true)
            {
                var deathEffects2 = GameObject.Find("Lobster 2").GetComponentInChildren<EnemyDeathEffects>(true);
                Modding.Logger.Log($"{deathEffects2}");
                var rootType2 = deathEffects2.GetType();

                var corpse2 = (GameObject)rootType2.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects2);
                Modding.Logger.Log($"{corpse2}");
                if (corpse2 != null)
                {
                    Modding.Logger.Log($"We got into Corpse!");
                    PlayMakerFSM corpseFSM2 = corpse2.LocateMyFSM("Death");
                    corpseFSM2.DisableAction("Init", 9);
                    corpseFSM2.DisableAction("Init", 8);
                    corpseFSM2.DisableAction("Init", 6);

                    corpseFSM2.DisableAction("Steam", 11);
                    corpseFSM2.DisableAction("Steam", 10);
                    corpseFSM2.DisableAction("Steam", 6);
                    corpseFSM2.DisableAction("Steam", 5);
                    corpseFSM2.DisableAction("Steam", 4);
                    corpseFSM2.DisableAction("Steam", 2);
                    corpseFSM2.DisableAction("Steam", 1);

                    corpseFSM2.DisableAction("Ready", 2);
                    corpseFSM2.DisableAction("Ready", 1);

                    corpseFSM2.DisableAction("Blow", 10);
                    corpseFSM2.DisableAction("Blow", 9);
                    corpseFSM2.DisableAction("Blow", 8);
                    corpseFSM2.DisableAction("Blow", 7);
                    corpseFSM2.DisableAction("Blow", 6);
                    corpseFSM2.DisableAction("Blow", 4);
                    corpseFSM2.DisableAction("Blow", 2);
                    Modding.Logger.Log($"We done doin the Corpse!!!!");
                    doneCorpse = true;
                }
            }
            if (DoubleBosses.BossDeathStatus.TryGetValue("Lobster", out bool isDead) && isDead) { Lancer1FSMDeath.SetState("Set"); }
            if (DoubleBosses.BossDeathStatus.TryGetValue("Lobster 2", out bool isDead2) && isDead2) { Lancer2FSMDeath.SetState("Set"); }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class CGControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 94.84f, 20.43f),
        };
        List<GameObject> CGs;
        List<PlayMakerFSM> firstLasers;
        List<PlayMakerFSM> secondLasers;
        PlayMakerFSM CG1fsm;
        PlayMakerFSM CG2fsm;
        void Start()
        {
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            if (BossSequenceController.IsInSequence)
            {
                return;
            }

            CGs = new[] { GameObject.Find("Mega Zombie Beam Miner (1)") }.ToList();
            GameObject[] extraCG = extrasInfo
            .Map(info => {
                GameObject prefab = CGs[info.prefab];
                var extra = GameObject.Instantiate(prefab);
                extra.transform.position = prefab.transform.position with { x = prefab.transform.position.x - 15f, y = prefab.transform.position.y };
                extra.transform.SetScaleX(-1);
                extra.name = "Mega Zombie Beam Miner (1)" + " " + info.num;
                return extra;
            })
            .ToArray();
            CGs.AddRange(extraCG);
            CG1fsm = CGs[0].LocateMyFSM("Beam Miner");
            CG2fsm = CGs[1].LocateMyFSM("Beam Miner");
            GameObject beamTurret1 = GameObject.Find("Laser Turret Mega (1)");
            GameObject beamTurret2 = GameObject.Find("Laser Turret Mega (2)");
            GameObject beamTurret3 = GameObject.Find("Laser Turret Mega (3)");
            GameObject beamTurret4 = GameObject.Find("Laser Turret Mega (4)");
            firstLasers = new[] { beamTurret1.LocateMyFSM("Laser Bug Mega"), beamTurret2.LocateMyFSM("Laser Bug Mega"), beamTurret3.LocateMyFSM("Laser Bug Mega"), beamTurret4.LocateMyFSM("Laser Bug Mega") }.ToList();

            GameObject beamTurret5 = GameObject.Instantiate(beamTurret1);
            beamTurret5.name = "Laser Turret Mega (5)";
            beamTurret5.transform.position = beamTurret1.transform.position with { x = beamTurret1.transform.position.x + 1, y = beamTurret1.transform.position.y };

            GameObject beamTurret6 = GameObject.Instantiate(beamTurret2);
            beamTurret6.name = "Laser Turret Mega (6)";
            beamTurret6.transform.position = beamTurret2.transform.position with { x = beamTurret2.transform.position.x + 1, y = beamTurret2.transform.position.y };

            GameObject beamTurret7 = GameObject.Instantiate(beamTurret3);
            beamTurret7.name = "Laser Turret Mega (7)";
            beamTurret7.transform.position = beamTurret3.transform.position with { x = beamTurret3.transform.position.x + 1, y = beamTurret3.transform.position.y };

            GameObject beamTurret8 = GameObject.Instantiate(beamTurret4);
            beamTurret8.name = "Laser Turret Mega (8)";
            beamTurret8.transform.position = beamTurret4.transform.position with { x = beamTurret4.transform.position.x + 1, y = beamTurret4.transform.position.y };
            secondLasers = new[] { beamTurret5.LocateMyFSM("Laser Bug Mega"), beamTurret6.LocateMyFSM("Laser Bug Mega"), beamTurret7.LocateMyFSM("Laser Bug Mega"), beamTurret8.LocateMyFSM("Laser Bug Mega") }.ToList();


            CG1fsm.GetState("Lasers").Actions[0] = new CustomFsmAction()
            {
                method = () => {
                    turnOnFirstLasers();
                }
            };

            CG2fsm.GetState("Lasers").Actions[0] = new CustomFsmAction()
            {
                method = () => {
                    turnOnSecondLasers();
                }
            };
            CG2fsm.RemoveAction("Roar",5);
            CG2fsm.RemoveAction("Roar",4);

            GameObject extraBeam = GameObject.Instantiate(beamTurret1.transform.Find("Beam").gameObject);
            GameObject extraBeamBall = GameObject.Instantiate(beamTurret1.transform.Find("Beam Ball").gameObject);
            GameObject extraBeamImpact = GameObject.Instantiate(beamTurret1.transform.Find("Beam Impact").gameObject);

            CG2fsm.FsmVariables.FindFsmGameObject("Beam Ball").Value = extraBeamBall;
            CG2fsm.FsmVariables.FindFsmGameObject("Beam").Value = extraBeam;
            CG2fsm.FsmVariables.FindFsmGameObject("Beam Impact").Value = extraBeamImpact;
        }
        bool AllCGDead()
        {
            string[] CGNames = { "Mega Zombie Beam Miner (1)", "Mega Zombie Beam Miner (1) 2"};
            return CGNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllCGDead())
            {
                BossSceneController.Instance.EndBossScene();
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }

        protected virtual void turnOnFirstLasers()
        {
            for(int i = 0; i<4;i++)
            {
                firstLasers[i].SetState("Aim");
            }
        }
        protected virtual void turnOnSecondLasers()
        {
            for (int i = 0; i < 4; i++)
            {
                secondLasers[i].SetState("Aim");
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class enragedCGControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 94.84f, 20.43f),
        };
        List<GameObject> eCGs;
        void Start()
        {
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            
            eCGs = new[] { GameObject.Find("Zombie Beam Miner Rematch") }.ToList();
            GameObject[] extraCG = extrasInfo
            .Map(info => {
                GameObject prefab = eCGs[info.prefab];
                var extra = GameObject.Instantiate(prefab);
                extra.transform.position = prefab.transform.position with { x = prefab.transform.position.x - 15f, y = prefab.transform.position.y };
                extra.transform.SetScaleX(-1);
                extra.name = "Zombie Beam Miner Rematch" + " " + info.num;
                return extra;
            })
            .ToArray();
            eCGs.AddRange(extraCG);
            PlayMakerFSM eCG1fsm = eCGs[0].LocateMyFSM("Beam Miner");
            PlayMakerFSM eCG2fsm = eCGs[1].LocateMyFSM("Beam Miner");
            GameObject beamTurret1 = GameObject.Find("Laser Turret Mega (1)");
            GameObject beamTurret2 = GameObject.Find("Laser Turret Mega (2)");
            GameObject beamTurret3 = GameObject.Find("Laser Turret Mega (3)");
            GameObject beamTurret4 = GameObject.Find("Laser Turret Mega");
            GameObject beamTurret5 = GameObject.Instantiate(beamTurret1);
            beamTurret5.name = "Laser Turret Mega (5)";
            beamTurret5.transform.position = beamTurret1.transform.position with { x = beamTurret1.transform.position.x + 1, y = beamTurret1.transform.position.y };

            GameObject beamTurret6 = GameObject.Instantiate(beamTurret2);
            beamTurret6.name = "Laser Turret Mega (6)";
            beamTurret6.transform.position = beamTurret2.transform.position with { x = beamTurret2.transform.position.x + 1, y = beamTurret2.transform.position.y };

            GameObject beamTurret7 = GameObject.Instantiate(beamTurret3);
            beamTurret7.name = "Laser Turret Mega (7)";
            beamTurret7.transform.position = beamTurret3.transform.position with { x = beamTurret3.transform.position.x + 1, y = beamTurret3.transform.position.y };

            GameObject beamTurret8 = GameObject.Instantiate(beamTurret4);
            beamTurret8.name = "Laser Turret Mega (8)";
            beamTurret8.transform.position = beamTurret4.transform.position with { x = beamTurret4.transform.position.x + 1, y = beamTurret4.transform.position.y };

            eCG2fsm.RemoveAction("Roar", 5);
            eCG2fsm.RemoveAction("Roar", 4);

            GameObject extraBeam = GameObject.Instantiate(beamTurret1.transform.Find("Beam").gameObject);
            GameObject extraBeamBall = GameObject.Instantiate(beamTurret1.transform.Find("Beam Ball").gameObject);
            GameObject extraBeamImpact = GameObject.Instantiate(beamTurret1.transform.Find("Beam Impact").gameObject);

            eCG2fsm.FsmVariables.FindFsmGameObject("Beam Ball").Value = extraBeamBall;
            eCG2fsm.FsmVariables.FindFsmGameObject("Beam").Value = extraBeam;
            eCG2fsm.FsmVariables.FindFsmGameObject("Beam Impact").Value = extraBeamImpact;
        }
        bool AllECGDead()
        {
            string[] eCGNames = { "Zombie Beam Miner Rematch", "Zombie Beam Miner Rematch 2" };
            return eCGNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }

        void Update()
        {
            if (AllECGDead())
            {
                BossSceneController.Instance.EndBossScene();
            }
        }


        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class uumuuControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 90.36f, 6.5f),
        };
        List<GameObject> uumie;
        PlayMakerFSM uumuu1FSM;
        PlayMakerFSM uumuu2FSM;
        GameObject multizapsExtra;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            uumie = new[] { GameObject.Find("Mega Jellyfish GG") }.ToList();
            GameObject[] extraUumuu = extrasInfo
                .Map(info => {
                    GameObject prefab = uumie[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = prefab.transform.position.x + 5f, y = prefab.transform.GetPositionY() };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            uumie.AddRange(extraUumuu);
            PlayMakerFSM bScene = GameObject.Find("Battle Scene").LocateMyFSM("Control");
            bScene.RemoveAction("End", 2);
            bScene.RemoveAction("End", 0);

            //HutongGames.PlayMaker.Actions.SetPolygonCollider();
            uumuu1FSM = uumie[0].LocateMyFSM("Mega Jellyfish");
            uumuu2FSM = uumie[1].LocateMyFSM("Mega Jellyfish");
            uumuu2FSM.SetState("Wake Pause");
            
            multizapsExtra = GameObject.Instantiate(GameObject.Find("Mega Jellyfish Multizaps"));
            MeshRenderer enterSpriteRenderer = uumie[1].transform.Find("Entry Sprite").gameObject.GetComponent<MeshRenderer>();
            uumuu2FSM.GetState("Init").Actions[1] = new CustomFsmAction()
            {
                method = () => {
                    uumuu2FSM.FsmVariables.FindFsmGameObject("Multizaps").Value = multizapsExtra;
                }
            };
            uumuu2FSM.GetState("Start").Actions[4] = new CustomFsmAction()
            {
                method = () => {
                    enterSpriteRenderer.enabled = false;    
                }
            };
        }
        bool AllUumieDead()
        {
            string[] UumuuNames = { "Mega Jellyfish GG", "Mega Jellyfish GG 2"};

            return UumuuNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }
        void Update()
        {
            if (AllUumieDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0)
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            //Modding.Logger.Log($"Current state of the uumu is: {uumuu2FSM.ActiveStateName}");
            Modding.Logger.Log($"Pattern is: {uumuu2FSM.FsmVariables.FindFsmGameObject("Multizaps").Value}");
            uumuu2FSM.FsmVariables.FindFsmGameObject("Multizaps").Value = multizapsExtra;
        }

        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class TLControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 90.36f, 6.5f),
        };
        List<GameObject> traitors;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            traitors = new[] { GameObject.Find("Mantis Traitor Lord") }.ToList();
            GameObject[] extraTraitor = extrasInfo
                .Map(info => {
                    GameObject prefab = traitors[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = prefab.transform.position.x - 15f, y = prefab.transform.GetPositionY() };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            traitors.AddRange(extraTraitor);
            PlayMakerFSM bScene = GameObject.Find("Battle Scene").LocateMyFSM("Battle Control");
            bScene.RemoveAction("End", 4);
            bScene.RemoveAction("End", 1);
        }
        bool AllTraitorsDead()
        {
            string[] UumuuNames = { "Mantis Traitor Lord", "Mantis Traitor Lord 2" };

            return UumuuNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }
        void Update()
        {
            if (AllTraitorsDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0)
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }

        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class GPControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 90.36f, 6.5f),
        };
        List<GameObject> princes;
        PlayMakerFSM GPFSM2;
        GameObject extraTink;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            princes = new[] { GameObject.Find("Grey Prince") }.ToList();
            GameObject[] extraPrince = extrasInfo
                .Map(info => {
                    GameObject prefab = princes[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = prefab.transform.position.x - 15f, y = prefab.transform.GetPositionY() };
                    extra.name = prefab.name + " " + info.num;
                    return extra;
                })
                .ToArray();
            princes.AddRange(extraPrince);
            GPFSM2 = princes[1].LocateMyFSM("Control");

            GameObject chargeTink = GameObject.Find("Charge Tink");
            extraTink  = GameObject.Instantiate(chargeTink);
            GPFSM2.GetState("Init").Actions[10] = new CustomFsmAction() 
            {
                method = () => {
                    GPFSM2.FsmVariables.FindFsmGameObject("Charge Tink").Value = extraTink;
                }
            };
        }
        bool AllPrincesDead()
        {
            string[] PrincesNames = { "Grey Prince", "Grey Prince 2" };

            return PrincesNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }
        void Update()
        {
            if (AllPrincesDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0)
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            Modding.Logger.Log($"Current state of Grey Prince is: {GPFSM2.ActiveStateName}");
            GPFSM2.FsmVariables.FindFsmGameObject("Charge Tink").Value = extraTink;
        }

        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class PVControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 90.36f, 6.5f),
        };
        List<GameObject> vessels;
        PlayMakerFSM vesselFSM;
        PlayMakerFSM vessel2FSM;
        bool doneCorpse = false;
        List<PlayMakerFSM> burstsFSM;
        List<GameObject> bursts;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            vessels = new[] { GameObject.Find("HK Prime") }.ToList();
            GameObject[] extraVessel = extrasInfo
                .Map(info => {
                    GameObject prefab = vessels[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = prefab.transform.position.x - 15f, y = prefab.transform.GetPositionY() };
                    extra.name = prefab.name + " " + info.num;
                    extra.transform.SetScaleX(-1);
                    return extra;
                })
                .ToArray();
            vessels.AddRange(extraVessel);
            vesselFSM = vessels[0].LocateMyFSM("Control");
            vessel2FSM = vessels[1].LocateMyFSM("Control");
            if (Modding.ModHooks.ModEnabled("QoL")) // I might be the only one in the world who actually used this feature
            {
                PlayMakerFSM control = vessels[1].LocateMyFSM("Control");
                Modding.Logger.Log("Working inside QoL");
                //control.GetState("Init").ChangeTransition("FINISHED", "Intro Roar");

                ((HutongGames.PlayMaker.Actions.Wait)control.GetState("Intro 2").Actions[3]).time = 0.01f;
                ((HutongGames.PlayMaker.Actions.Wait)control.GetState("Intro 1").Actions[0]).time = 0.01f;
                ((HutongGames.PlayMaker.Actions.Wait)control.GetState("Intro Roar").Actions[7]).time = 1f;
                //HutongGames.PlayMaker.Actions.Wait;
            }
            var deathEffects = vessels[0].GetComponentInChildren<EnemyDeathEffects>(true);
            Modding.Logger.Log($"{deathEffects}");
            var rootType = deathEffects.GetType();
            var corpse = (GameObject)rootType.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects);
            if (corpse != null)
            {
                Modding.Logger.Log($"We got into Corpse!");
                PlayMakerFSM corpseFSM = corpse.LocateMyFSM("corpse");
                corpseFSM.DisableAction("Music", 4);
                corpseFSM.DisableAction("Music", 3);
                corpseFSM.DisableAction("Music",0);

                corpseFSM.DisableAction("End Scene",0);
            }

            vessel2FSM.RemoveAction("Init", 63);
            GameObject extraFocusBlast1 = GameObject.Instantiate(GameObject.Find("HK Prime Blast"));
            extraFocusBlast1.name = "HK Prime Blast (6)";

            GameObject extraFocusBlast2 = GameObject.Instantiate(GameObject.Find("HK Prime Blast (1)"));
            extraFocusBlast2.name = "HK Prime Blast (7)";
            GameObject extraFocusBlast3 = GameObject.Instantiate(GameObject.Find("HK Prime Blast (2)"));
            extraFocusBlast3.name = "HK Prime Blast (8)";
            GameObject extraFocusBlast4 = GameObject.Instantiate(GameObject.Find("HK Prime Blast (3)"));
            extraFocusBlast4.name = "HK Prime Blast (9)";
            GameObject extraFocusBlast5 = GameObject.Instantiate(GameObject.Find("HK Prime Blast (4)"));
            extraFocusBlast5.name = "HK Prime Blast (10)";
            GameObject extraFocusBlast6 = GameObject.Instantiate(GameObject.Find("HK Prime Blast (5)"));
            extraFocusBlast6.name = "HK Prime Blast (11)";
            burstsFSM = new[] { extraFocusBlast1.LocateMyFSM("Control"), extraFocusBlast2.LocateMyFSM("Control"), extraFocusBlast3.LocateMyFSM("Control"), extraFocusBlast4.LocateMyFSM("Control"), extraFocusBlast5.LocateMyFSM("Control"), extraFocusBlast6.LocateMyFSM("Control") }.ToList();
            bursts = new[] { extraFocusBlast1, extraFocusBlast2, extraFocusBlast3, extraFocusBlast4, extraFocusBlast5, extraFocusBlast6 }.ToList();
            
            for(int i = 0; i < 6; i++)
            {
                burstsFSM[i].FsmVariables.FindFsmGameObject("Blast").Value = GameObject.Instantiate(bursts[i].transform.Find("Blast").gameObject);
            }






            vessel2FSM.GetState("Focus Burst").Actions[0] = new CustomFsmAction()
            {
                method = () => {
                    turnOnBursts();
                }
            };
        }
        bool AllVesselsDead()
        {
            string[] VesselNames = { "HK Prime", "HK Prime 2" };

            return VesselNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }
        void Update()
        {
            if (AllVesselsDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0)
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
            if (doneCorpse != true)
            {
                var deathEffects2 = vessels[1].GetComponentInChildren<EnemyDeathEffects>(true);
                Modding.Logger.Log($"{deathEffects2}");
                var rootType2 = deathEffects2.GetType();

                var corpse2 = (GameObject)rootType2.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(deathEffects2);
                Modding.Logger.Log($"{corpse2}");
                if (corpse2 != null)
                {
                    Modding.Logger.Log($"We got into Corpse!2");
                    PlayMakerFSM corpseFSM2 = corpse2.LocateMyFSM("corpse");
                    corpseFSM2.DisableAction("Music", 4);
                    corpseFSM2.DisableAction("Music", 3);
                    corpseFSM2.DisableAction("Music", 0);

                    corpseFSM2.DisableAction("End Scene", 0);
                    doneCorpse = true;
                }
            }
        }

        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
        protected virtual void turnOnBursts()
        {
            for (int i = 0; i < 6; i++)
            {
                burstsFSM[i].SetState("Wait");
                var blast = burstsFSM[i].FsmVariables.FindFsmGameObject("Blast").Value;
                //i totally stole this code from @hien-ngo29 in github, he made a Hollow Knight usable skill for the knight.
                Vector3 pos = new();
                Vector3 heroPos = HeroController.instance.gameObject.transform.position;
                pos.x = heroPos.x - 20 + (8 * i) + UnityEngine.Random.Range(-1.5f, 1.5f);
                pos.y = 6.4f + (i % 2 != 0 ? UnityEngine.Random.Range(11.88f, 14.08f) : UnityEngine.Random.Range(7.88f, 10.08f)) - 7f;
                blast.transform.position = pos;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    class NGControl : MonoBehaviour
    {
        internal static readonly (int num, int prefab, float x, float y)[] extrasInfo = new[] {
            (2, 0, 90.36f, 6.5f),
        };
        List<GameObject> grimms;
        PlayMakerFSM grimmFSM;
        PlayMakerFSM grimm2FSM;
        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
            grimms = new[] { GameObject.Find("Grimm Control") }.ToList();
            GameObject[] extraGrimm = extrasInfo
                .Map(info => {
                    GameObject prefab = grimms[info.prefab];
                    var extra = GameObject.Instantiate(prefab);
                    extra.transform.position = prefab.transform.position with { x = prefab.transform.position.x - 15f, y = prefab.transform.GetPositionY() };
                    extra.name = prefab.name + " " + info.num;
                    extra.transform.Find("Nightmare Grimm Boss").gameObject.name = "Nightmare Grimm Boss 2";
                    return extra;
                })
                .ToArray();
            grimms.AddRange(extraGrimm);
            grimmFSM = grimms[0].LocateMyFSM("Control");
            grimm2FSM = grimms[1].LocateMyFSM("Control");
            grimmFSM.RemoveAction("End", 0);
            grimm2FSM.RemoveAction("End", 0);
            grimmFSM.RemoveAction("State 1",0);
            grimm2FSM.RemoveAction("State 1",0);
            PlayMakerFSM grimmActual1FSM = GameObject.Find("Nightmare Grimm Boss").LocateMyFSM("Control");
            PlayMakerFSM grimmActual2FSM = GameObject.Find("Nightmare Grimm Boss 2").LocateMyFSM("Control");

            grimmActual1FSM.GetState("Send NPC Event").Actions = new HutongGames.PlayMaker.FsmStateAction[]
            {
                new CustomFsmAction()
                {
                    method = () => {
                        DoubleBosses.BossDeathStatus["Nightmare Grimm Boss"] = true;
                    }
                }
            };
            grimmActual2FSM.GetState("Send NPC Event").Actions = new HutongGames.PlayMaker.FsmStateAction[]
            {
                new CustomFsmAction()
                {
                    method = () => {
                        DoubleBosses.BossDeathStatus["Nightmare Grimm Boss 2"] = true;
                    }
                }
            };
            ((HutongGames.PlayMaker.Actions.FindChild)grimm2FSM.GetState("Init").Actions[11]).childName = "Nightmare Grimm Boss 2";
        }
        bool AllGrimmsDead()
        {
            string[] VesselNames = { "Nightmare Grimm Boss", "Nightmare Grimm Boss 2" };

            return VesselNames.All(name => DoubleBosses.BossDeathStatus.TryGetValue(name, out bool isDead) && isDead);
        }
        void Update()
        {
            if (AllGrimmsDead())
            {
                BossSceneController.Instance.EndBossScene();
            }

            if (DoubleBosses.BossDeathStatus.Count > 0)
            {
                Modding.Logger.Log("Boss Death Status:\n" +
                                    string.Join("\n", DoubleBosses.BossDeathStatus.Select(kv => $"{kv.Key}: {(kv.Value ? "Dead" : "Alive")}")));
            }
        }

        void OnDestroy()
        {
            foreach (string bossName in DoubleBosses.trackedBosses)
            {
                if (DoubleBosses.BossDeathStatus.TryGetValue(bossName, out _))
                {
                    DoubleBosses.BossDeathStatus[bossName] = false;
                    Modding.Logger.Log($"[Boss Reset] {bossName} status reset to false.");
                }
            }
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------

    class WatchersControl : MonoBehaviour
    {
        bool alreadyDone1 = false;
        bool alreadyDone2 = false;
        bool alreadyDone3 = false;
        bool alreadyDone4 = false;
        bool alreadyDone5 = false;
        internal static readonly (int num, int prefab, float x, bool faceRight)[] extrasInfo = new[] {
            (7, 0, 55.82f, false),
            (8, 1, 26.91f, true),
            (9, 2, 45.17f, true),
            (10, 3, 32.14f, true),
            (11, 4, 61.238f, true),
            (12, 5, 37.362f, false)
        };

        void Start()
        {

            if (BossSequenceController.IsInSequence)
            {
                return;
            }



            Scene next = UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Watcher_Knights");

            GameObject battleCtrl = next.GetRootGameObjects()
                    .First(go => go.name == "Battle Control");

            next.GetRootGameObjects()
                .First(go => go.name == "Boss Scene Controller")
                .transform.Find("door_dreamEnter")
                .SetPositionX(42.42f);

            PlayMakerFSM battleCtrlFsm = battleCtrl.LocateMyFSM("Battle Control");


            var knights = new[] { 1, 2, 3, 4, 5, 6 }
                    .Map(s => "Black Knight " + s)
                    .Map(name => battleCtrl.Child(name)!)
                    .ToList();


            GameObject[] extraKnights = extrasInfo
                .Map(info => {
                    GameObject prefab = knights[info.prefab];
                    var extra = GameObject.Instantiate(prefab, prefab.transform.parent);
                    extra.transform.position = prefab.transform.position with { x = info.x };
                    extra.name = "Black Knight " + info.num;
                    extra.transform.SetScaleX(info.faceRight ? -1 : 1);
                    return extra;
                })
                .ToArray();

            knights.AddRange(extraKnights);
            battleCtrlFsm.FsmVariables.FindFsmInt("Battle Enemies").Value = knights.Count;





            PlayMakerFSM knight1Fsm = GameObject.Find("Black Knight 1")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight2Fsm = GameObject.Find("Black Knight 2")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight3Fsm = GameObject.Find("Black Knight 3")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight4Fsm = GameObject.Find("Black Knight 4")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight5Fsm = GameObject.Find("Black Knight 5")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight6Fsm = GameObject.Find("Black Knight 6")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight7Fsm = GameObject.Find("Black Knight 7")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight8Fsm = GameObject.Find("Black Knight 8")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight9Fsm = GameObject.Find("Black Knight 9")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight10Fsm = GameObject.Find("Black Knight 10")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight11Fsm = GameObject.Find("Black Knight 11")?.LocateMyFSM("Black Knight");
            PlayMakerFSM knight12Fsm = GameObject.Find("Black Knight 12")?.LocateMyFSM("Black Knight");



            battleCtrlFsm.GetState("Knight 1").Actions[2] = new CustomFsmAction()
            {
                method = () => knight7Fsm?.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("WAKE"))
            };
            battleCtrlFsm.GetState("Knight 2").Actions[2] = new CustomFsmAction()
            {
                method = () => knight8Fsm?.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("WAKE"))
            };
            battleCtrlFsm.GetState("Knight 3").Actions[2] = new CustomFsmAction()
            {
                method = () => knight9Fsm?.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("WAKE"))
            };
            battleCtrlFsm.GetState("Knight 4").Actions[2] = new CustomFsmAction()
            {
                method = () => knight10Fsm?.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("WAKE"))
            };
            battleCtrlFsm.GetState("Knight 5").Actions[2] = new CustomFsmAction()
            {
                method = () => knight11Fsm?.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("WAKE"))
            };
            battleCtrlFsm.GetState("Knight 6").Actions[2] = new CustomFsmAction()
            {
                method = () => knight12Fsm?.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("WAKE"))
            };




            var knightPairs = new[] {
                ("Black Knight 1", "Black Knight 7"),
                ("Black Knight 2", "Black Knight 8"),
                ("Black Knight 3", "Black Knight 9"),
                ("Black Knight 4", "Black Knight 10"),
                ("Black Knight 5", "Black Knight 11"),
                ("Black Knight 6", "Black Knight 12")
            };

            foreach (var (knightA, knightB) in knightPairs)
            {
                var pair = knights
                    .Filter(knight => knight.name == knightA || knight.name == knightB)
                    .ToList();


                if (pair.Count == 2)
                {
                    pair.ShareHealth(0, "DoubleWatchers");
                }
            }
            StartCoroutine(InitHP());
        }


        private IEnumerator InitHP()
        {
            yield return new WaitForSeconds(1f);
            GameObject.Find("Black Knight 1").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 2").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 3").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 4").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 5").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 6").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 7").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 8").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 9").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 10").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 11").GetComponent<HealthManager>().hp = 99999;
            GameObject.Find("Black Knight 12").GetComponent<HealthManager>().hp = 99999;
        }



        void Update()
        {
            GameObject battleCtrl = UnityEngine.SceneManagement.SceneManager.GetSceneByName("GG_Watcher_Knights").GetRootGameObjects().First(go => go.name == "Battle Control");
            PlayMakerFSM battleCtrlFsm = battleCtrl.LocateMyFSM("Battle Control");
            if (battleCtrlFsm == null)
            {
                Modding.Logger.LogError("BattleCtrlFsm is still null in Update!");
                return;
            }


            string currentState = battleCtrlFsm.ActiveStateName;

            if (!alreadyDone1 && GameObject.Find("Black Knight 1") == null)
            {
                Modding.Logger.Log("Nice! Black Knight 1 is dead! Triggering NEXT event.");
                //battleCtrlFsm.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("NEXT"));
                battleCtrlFsm.SetState("Knight 2");
                battleCtrlFsm.SetState("Pause 1");
                alreadyDone1 = true;
            }
            if (!alreadyDone2 && GameObject.Find("Black Knight 2") == null)
            {
                Modding.Logger.Log("Nice! Black Knight 2 is dead! Triggering NEXT event.");
                //battleCtrlFsm.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("NEXT"));
                if (currentState == ("Pause 1"))
                {
                    battleCtrlFsm.SetState("Knight 3");
                }
                battleCtrlFsm.SetState("Pause 2");
                alreadyDone2 = true;
            }
            if (!alreadyDone3 && GameObject.Find("Black Knight 3") == null)
            {
                Modding.Logger.Log("Nice! Black Knight 3 is dead! Triggering NEXT event.");
                //battleCtrlFsm.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("NEXT"));
                if (currentState == ("Pause 2"))
                {
                    battleCtrlFsm.SetState("Knight 4");
                }
                battleCtrlFsm.SetState("Pause 3");
                alreadyDone3 = true;
            }
            if (!alreadyDone4 && GameObject.Find("Black Knight 4") == null)
            {
                Modding.Logger.Log("Nice! Black Knight 4 is dead! Triggering NEXT event.");
                //battleCtrlFsm.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("NEXT"));
                if (currentState == ("Pause 3"))
                {
                    battleCtrlFsm.SetState("Knight 5");
                }
                battleCtrlFsm.SetState("Pause 4");
                alreadyDone4 = true;
            }
            if (!alreadyDone5 && GameObject.Find("Black Knight 5") == null)
            {
                Modding.Logger.Log("Nice! Black Knight 5 is dead! Triggering NEXT event.");
                //battleCtrlFsm.Fsm.ProcessEvent(FsmEvent.GetFsmEvent("NEXT"));
                if (currentState == ("Pause 4"))
                {
                    battleCtrlFsm.SetState("Knight 6");
                }
                alreadyDone5 = true;
            }
            var state = battleCtrlFsm.GetState("Knight 1");
        }
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------
    
    class FooControl : MonoBehaviour
    {
        void Start()
        {
            if (BossSequenceController.IsInSequence)
            {
                return;
            }
        }
        void Update()
        {
            /*
            GameObject BattleSub = GameObject.Find("Battle Sub");
            PlayMakerFSM BattleSubFSM = BattleSub.LocateMyFSM("Start");
            if (BattleSubFSM != null)
            {
                Modding.Logger.Log($"The current state is: {BattleSubFSM.ActiveStateName}");
            }
            */
        }
    }
}
