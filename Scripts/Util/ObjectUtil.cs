using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Utils
{
    public class ObjectUtil
    {
        public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
    
            if (!component)
            {
                component = gameObject.AddComponent<T>();
            }
    
            return component;
        }
        
        public static GameObject FindChild(GameObject gameObject, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(gameObject, name, recursive);
    
            return !transform ? null : transform.gameObject;
        }
    
        public static T FindChild<T>(GameObject gameObject, string name = null, bool recursive = false) where T : Object
        {
            if ((object)gameObject == null) return null;
    
            // 현재 오브젝트의 직속적인 자식 오브젝트들만 찾음
            if (!recursive)
            {
                for (var i = 0; i < gameObject.transform.childCount; i++)
                {
                    Transform transform = gameObject.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || transform.name.Equals(name))
                    {
                        T component = transform.GetComponent<T>();
                        if (component != null) return component;
                    }
                }
            }
            else
            {
                // 비활성화된 오브젝트까지 찾음
                foreach (T component in gameObject.GetComponentsInChildren<T>(true))
                {
                    if (string.IsNullOrEmpty(name) || component.name.Equals(name))
                    {
                        return component;
                    }
                }
            }
    
            return null;
        }
    }

    public static class ExtensionObject
    {
        public static void BindEvent(this GameObject gameObject, Action<PointerEventData> action, UI_Base.MouseUIEvent mouseUIEvent = UI_Base.MouseUIEvent.Click)
        {
            UI_Base.BindEvent(gameObject, action, mouseUIEvent);
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return ObjectUtil.GetOrAddComponent<T>(gameObject);
        }
        
        public static bool AlmostEquals(this float target, float second, float floatDiff)
        {
            return Mathf.Abs(target - second) < floatDiff;
        }
    }

    public class CastingValue
    {
        /// <summary>
        /// 정수형을 실수형(0.0 ~ 1.0)으로 변환
        /// </summary>
        /// <param name="targetValue">변환이 필요한 정수형</param>
        /// <returns>실수형 (0.0 ~ 1.0)</returns>
        public static float IntToFloat(int targetValue)
        {
            return (float)targetValue / 100;
        }
    }

    
    
}
