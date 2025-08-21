using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Satchel;
using HutongGames.PlayMaker.Actions;
using System.Reflection;
using System.Collections;
using HutongGames.PlayMaker;
using Satchel.Futils;

namespace DoubleBosses
{
    public static class SpawnerExtensions
    {
        static GameObject GetCorpse<T>(this GameObject prefab)
    where T : EnemyDeathEffects
        {
            var deathEffects = prefab.GetComponentInChildren<T>(true);

            if (deathEffects == null)
                return null;

            var rootType = deathEffects.GetType();

            System.Reflection.FieldInfo GetCorpseField(Type t)
            {
                return t.GetField("corpse", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            }

            while (rootType != typeof(EnemyDeathEffects) && rootType != null)
            {
                if (GetCorpseField(rootType) != null)
                    break;
                rootType = rootType.BaseType;
            }

            if (rootType == null)
                return null;

            var corpsePrefab = (GameObject)GetCorpseField(rootType).GetValue(deathEffects);

            if (corpsePrefab == null)
            {
                return null;
            }
            else
            {
                return corpsePrefab;
            }
        }
    }
}
