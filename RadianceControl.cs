// The whole code in this file is by ygsbzr
using System;
using System.Collections;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
namespace DoubleBosses
{
    class RadianceControl : MonoBehaviour
    {
        private void Awake()
        {
            Abs1 = GameObject.Find("Absolute Radiance");
            Abs1PhControl = Abs1.LocateMyFSM("Phase Control");
            Abs1CL = Abs1.LocateMyFSM("Control");
            Abs1HM = Abs1.GetComponent<HealthManager>();
            gameObject.transform.SetPosition2D(30, 60);
        }
        private void Start()
        {
            Abs2 = UnityEngine.Object.Instantiate(Abs1);
            Abs2.SetActive(true);
            Abs2PhControl = Abs2.LocateMyFSM("Phase Control");
            Abs2CL = Abs2.LocateMyFSM("Control");
            Abs2HM = Abs2.GetComponent<HealthManager>();
            On.HealthManager.TakeDamage += KnightHit;
            StartCoroutine(InitHP());
            gameObject.transform.SetPosition2D(70, 60);
        }

        private void KnightHit(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            damageHP += hitInstance.DamageDealt;
            orig(self, hitInstance);
        }

        private IEnumerator InitHP()
        {
            yield return new WaitForSeconds(2f);
            Abs1HM.hp = 99999;
            Abs2HM.hp = 99999;
            flagP2 = false;
            flagP3 = false;
            flagP4 = false;
            flagP5 = false;
            flagDie = false;
            flag2 = false;
            endFlag = false;
        }
        private void OnDestroy()
        {
            On.HealthManager.TakeDamage -= KnightHit;
            damageHP = 0;
        }
        private void Update()
        {
            if (damageHP >= 700 && damageHP < 1488)
            {
                if (!flagP2)
                {
                    Abs1PhControl.SetState("Set Phase 2");
                    Abs2PhControl.SetState("Set Phase 2");
                    flagP2 = true;
                }
            }
            if (damageHP >= 1488 && damageHP < 2013)
            {
                if (!flagP3)
                {
                    Abs1PhControl.SetState("Set Phase 3");
                    Abs2PhControl.SetState("Set Phase 3");
                    flagP3 = true;
                }
            }
            if (damageHP >= 2013 && damageHP < 3325)
            {
                if (!flagP4)
                {
                    Abs1PhControl.SetState("Stun 1");
                    Abs2PhControl.SetState("Stun 1");
                    flagP4 = true;
                }
            }
            if (damageHP >= 3325 && damageHP < 3990)
            {
                if (!flagP5)
                {
                    Abs1PhControl.SetState("Set Ascend");
                    Abs2PhControl.SetState("Set Ascend");
                    flagP5 = true;
                }
            }
            if (damageHP >= 3990)
            {
                if (!flagDie)
                {
                    Abs1CL.SetState("Check Pos");
                    Abs2CL.SetState("Check Pos");
                    flagDie = true;
                }
            }
            if (Abs1CL.Fsm.ActiveState.Name == "Arena 2 Start" || Abs2CL.Fsm.ActiveState.Name == "Arena 2 Start")
            {
                if (!flag2)
                {
                    Abs1CL.SetState("Arena 2 Start");
                    Abs2CL.SetState("Arena 2 Start");
                    flag2 = true;
                }
            }
            if (Abs1CL.Fsm.ActiveState.Name == "Final Impact")
            {
                if (!endFlag)
                {
                    Abs2CL.SetState("Final Impact");
                    endFlag = true;
                }
            }
            if (Abs2CL.Fsm.ActiveState.Name == "Final Impact")
            {
                if (!endFlag)
                {
                    Abs1CL.SetState("Final Impact");
                    endFlag = true;
                }
            }

        }
        PlayMakerFSM Abs1PhControl;
        PlayMakerFSM Abs2PhControl;
        GameObject Abs1;
        GameObject Abs2 = new GameObject();
        PlayMakerFSM Abs1CL;
        PlayMakerFSM Abs2CL;
        HealthManager Abs1HM;
        HealthManager Abs2HM;
        int damageHP = 0;
        bool flagP2 = false;
        bool flagP3 = false;
        bool flagP4 = false;
        bool flagP5 = false;
        bool flagDie = false;
        bool flag2 = false;
        bool endFlag = false;
    }

}