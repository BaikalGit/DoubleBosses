using GlobalEnums;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Modding.Utils;
using Osmi.Game;
using System.Collections;
using System.Globalization;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            Modding.Logger.Log($"Hello! I exist! ==============================================================================");
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


            PlayMakerFSM hiveKnightFSM = HiveKnights[1].LocateMyFSM("Control");
            hiveKnightFSM.GetState("Glob Strike").Actions[3] = new CustomFsmAction()
            {
                method = () => {
                    extraGlobsFSM.SendEvent("FIRE");

                }
            };
            hiveKnightFSM.DisableAction("Fall",1);
            hiveKnightFSM.DisableAction("Intro", 3);
            hiveKnightFSM.DisableAction("Intro", 1);
            hiveKnightFSM.DisableAction("Intro End", 2);
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
            GameObject BattleSub = GameObject.Find("Battle Sub");
            PlayMakerFSM BattleSubFSM = BattleSub.LocateMyFSM("Start");
            if (BattleSubFSM != null)
            {
                Modding.Logger.Log($"The current state is: {BattleSubFSM.ActiveStateName}");
            }
        }
    }
}
