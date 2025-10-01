using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
namespace DoubleBosses
{

    class AllFinder : MonoBehaviour
    {
        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += FindAllScene;
        }

        private void FindAllScene(UnityEngine.SceneManagement.Scene _, UnityEngine.SceneManagement.Scene arg1)
        {
            var sceneHandlers = new Dictionary<string, Func<IEnumerator>>
        {

        { "GG_Radiance", () => FindAndCreate<RadianceControl>("Absolute Radiance") },
        { "GG_Watcher_Knights", () => FindAndCreate<WatchersControl>("Black Knight 1") },
        { "GG_Gruz_Mother", () => FindAndCreate<GruzMotherControl>("Giant Fly") },
        { "GG_Gruz_Mother_V", () => FindAndCreate<GruzMotherVControl>("Giant Fly") },
        { "GG_Vengefly", () => FindAndCreate<VengeflyKingControl>("Giant Buzzer Col") },
        { "GG_Vengefly_V", () => FindAndCreate<VengeflyKingVControl>("Giant Buzzer Col") },
        { "GG_Brooding_Mawlek", () => FindAndCreate<MawlekControl>("Mawlek Body") },
        { "GG_Brooding_Mawlek_V", () => FindAndCreate<MawlekControl>("Mawlek Body") },
        { "GG_False_Knight", () => FindAndCreate<FalseKnightControl>("False Knight New") },
        { "GG_Failed_Champion", () => FindAndCreate<FailedChampionControl>("False Knight Dream") },
        { "GG_Hornet_1", () => FindAndCreate<Hornet1Control>("Hornet Boss 1") },
        { "GG_Hornet_2", () => FindAndCreate<Hornet2Control>("Hornet Boss 2") },
        { "GG_Mega_Moss_Charger", () => FindAndCreate<MossChargerControl>("Mega Moss Charger") },
        { "GG_Flukemarm", () => FindAndCreate<FlukeMarmControl>("Fluke Mother") },
        { "GG_Mantis_Lords", () => FindAndCreate<LordsControl>("Mantis Lord") },
        { "GG_Mantis_Lords_V", () => FindAndCreate<SistersControl>("Mantis Lord") },
        { "GG_Oblobbles", () => FindAndCreate<OblobblesControl>("Mega Fat Bee") },
        { "GG_Hive_Knight", () => FindAndCreate<HiveKnightControl>("Hive Knight") },
        { "GG_Broken_Vessel", () => FindAndCreate<BrokenVesselControl>("Infected Knight") },
        { "GG_Lost_Kin", () => FindAndCreate<LostKinControl>("Lost Kin") },
        { "GG_Nosk", () => FindAndCreate<NoskControl>("Mimic Spider") },
        { "GG_Nosk_V", () => FindAndCreate<NoskVControl>("Mimic Spider") },
        { "GG_Nosk_Hornet", () => FindAndCreate<NoskHornetControl>("Hornet Nosk") },

        };

            if (sceneHandlers.TryGetValue(arg1.name, out var handler))
            {
                StartCoroutine(handler());
            }
        }

        private IEnumerator FindAndCreate<T>(string target) where T : Component
        {
            yield return new WaitWhile(() => GameObject.Find(target) == null);
            new GameObject().AddComponent<T>();
        }

        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= FindAllScene;
        }
    }
}