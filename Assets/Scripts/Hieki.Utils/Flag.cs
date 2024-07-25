using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Hieki.Utils
{
    /// <summary>
    /// <see cref="Flag"/> is useful for enabling/disabling objects
    /// </summary>
    public sealed class Flag : MonoBehaviour
    {
        static List<ActionModel> m_ActionModels = new List<ActionModel>();
        static bool ready;
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()  //domain reloading
        {
            m_ActionModels = null;
            ready = false;
        }
#endif

        private void Awake()
        {
            if (ready)
            {
                Debug.LogWarning($"There's more than 1 {GetType()}", this);
            }
            ready = true;
            m_ActionModels = new List<ActionModel>(24);
        }

        private void Update()
        {
            for (int i = 0; i < m_ActionModels.Count; i++)
            {
                m_ActionModels[i].Update();

                if (m_ActionModels[i].timer >= m_ActionModels[i].duration)
                {
                    m_ActionModels[i].Complete();
                    m_ActionModels.RemoveAtSwapBack(i);
                    i--;
                    continue;
                }
            }
        }

        public static void Active(float duration, UnityAction OnStart, UnityAction OnComplete)
        {
            if (!ready)
            {
                GameObject activator = new GameObject("Aativator Manager");
                activator.AddComponent<Flag>();
            }

            //if (m_InactiveObject.Contains(activeObject))
            //{
            //    return;
            //}

            ActionModel actionModel = new ActionModel(duration, OnStart, OnComplete);

            actionModel.Start();
            //actionModel.timer = 0;
            m_ActionModels.Add(actionModel);
        }

        public static void Enable(GameObject gameObject, float duration, UnityAction<GameObject> OnEnabled = null)
        {
            Active(duration,
               () => gameObject.SetActive(false),
               () =>
               {
                   gameObject.SetActive(true);
                   OnEnabled?.Invoke(gameObject);
               });
        }

        public static void Disable(GameObject gameObject, float duration, UnityAction<GameObject> OnDisabled = null)
        {
            Active(duration,
                () => gameObject.SetActive(true),
                () =>
                {
                    gameObject.SetActive(false);
                    OnDisabled?.Invoke(gameObject);
                });
        }

        class ActionModel
        {
            public readonly float duration;
            public float timer;

            readonly UnityAction OnStart;
            readonly UnityAction OnComplete;

            public ActionModel(float duration, UnityAction OnStart, UnityAction OnComplete)
            {
                this.duration = duration;
                this.OnStart = OnStart;
                this.OnComplete = OnComplete;
                timer = 0;
            }

            public void Start()
            {
                OnStart?.Invoke();
            }

            public void Complete()
            {
                OnComplete?.Invoke();
            }

            public void Update()
            {
                timer += Time.deltaTime;
            }
        }
    }
}