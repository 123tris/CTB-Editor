using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CooldownManagerNamespace
{
    public static class CooldownManager
    {
        private class HiddenMonobehaviour : MonoBehaviour { }

        private static readonly MonoBehaviour mono;

        private static Dictionary<string, Coroutine> coroutines = new Dictionary<string, Coroutine>();

        static CooldownManager()
        {
            //This will not function with Unity 2019.3 and up when domain reloading is disabled
            GameObject obj = new GameObject("Cooldown Manager");
            UnityEngine.Object.DontDestroyOnLoad(obj);
            mono = obj.AddComponent<HiddenMonobehaviour>();
        }

        /// <summary>Delays <b>action</b> by <b>cooldownDurations (in seconds)</b></summary>
        /// <param name="cooldownDuration">Duration in seconds</param>
        public static Coroutine Cooldown(float cooldownDuration, Action action)
        {
            return mono.StartCoroutine(InternalCooldown(cooldownDuration, action));
        }

        /// <summary>Cooldown will delay the action by the cooldownDuration given (in seconds). If another coroutine with same name is running it will stop that one</summary>
        /// <param name="cooldownDuration">Duration in seconds</param>
        public static Coroutine Cooldown(float cooldownDuration, Action action, string name)
        {
            if (coroutines.ContainsKey(name))
                mono.StopCoroutine(coroutines[name]);

            coroutines[name] = mono.StartCoroutine(InternalCooldown(cooldownDuration, action));
            return coroutines[name];
        }

        public static Coroutine Cooldown<T>(float cooldownDuration, Action<T> action, T parameter)
        {
            return mono.StartCoroutine(InternalCooldown(cooldownDuration, action, parameter));
        }

        public static Coroutine Cooldown<T, U>(float cooldownDuration, Action<T, U> action, T param1, U param2)
        {
            return mono.StartCoroutine(InternalCooldown(cooldownDuration, action, param1, param2));
        }

        public static Coroutine Cooldown<T, U, X>(float cooldownDuration, Action<T, U, X> action, T param1, U param2, X param3)
        {
            return mono.StartCoroutine(InternalCooldown(cooldownDuration, action, param1, param2, param3));
        }

        private static IEnumerator InternalCooldown(float cooldownDuration, Action action)
        {
            yield return new WaitForSeconds(cooldownDuration);
            action.Invoke();
        }

        private static IEnumerator InternalCooldown<T>(float cooldownDuration, Action<T> action, T parameter)
        {
            yield return new WaitForSeconds(cooldownDuration);
            action.Invoke(parameter);
        }

        private static IEnumerator InternalCooldown<T, U>(float cooldownDuration, Action<T, U> action, T parameter1, U parameter2)
        {
            yield return new WaitForSeconds(cooldownDuration);
            action.Invoke(parameter1, parameter2);
        }

        private static IEnumerator InternalCooldown<T, U, X>(float cooldownDuration, Action<T, U, X> action, T parameter1, U parameter2, X parameter3)
        {
            yield return new WaitForSeconds(cooldownDuration);
            action.Invoke(parameter1, parameter2, parameter3);
        }

        public static Coroutine OnNextFrame(Action action)
        {
            return mono.StartCoroutine(InternalNextFrame(action));
        }

        private static IEnumerator InternalNextFrame(Action action)
        {
            yield return 0;
            action.Invoke();
        }

        public static void IterateOverTime(float duration, Action action, int iterationsPerSecond = 60)
        {
            mono.StartCoroutine(InternalIterateOverTime(duration, action, iterationsPerSecond));
        }

        private static IEnumerator InternalIterateOverTime(float duration, Action action, int iterationsPerSecond)
        {
            WaitForSeconds wait = new WaitForSeconds(1f / iterationsPerSecond);
            float startTime = Time.time;
            for (float timePassed = 0; timePassed < duration; timePassed = Time.time - startTime)
            {
                action.Invoke();
                yield return wait;
            }
        }
    }
}