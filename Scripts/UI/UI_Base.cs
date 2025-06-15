using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Object = UnityEngine.Object;

public abstract class UI_Base : MonoBehaviour
{
    public enum MouseUIEvent
    {
        Click, // 마우스 클릭이 진행된 시점
        Enter, // 마우스가 특정 UI위에 올라갈 경우
        Exit,  // 마우스가 특정 UI에서 벗어날 경우
        Up     // 마우스 클릭이 끝나는 시점에 실행
    }
    
    protected Dictionary<Type, UnityEngine.Object[]> ObjectDictionary = new Dictionary<Type, Object[]>();
    
    public abstract void Init();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] typeNames = Enum.GetNames(type);
        Object[] objects = new Object[typeNames.Length];
        ObjectDictionary.Add(typeof(T), objects);

        for (var i = 0; i < typeNames.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[i] = ObjectUtil.FindChild(gameObject, typeNames[i], true);
            }
            else
            {
                objects[i] = ObjectUtil.FindChild<T>(gameObject, typeNames[i], true);
            }

            if (objects[i] == null)
            {
                
                print($"{gameObject.name} 에서 {typeNames[i]} Bind({typeof(T).Name}) 실패!");
            }
        }
    }

    protected T Get<T>(int index) where T : Object
    {
        if (!ObjectDictionary.TryGetValue(typeof(T), out Object[] objects)) return null;

        return objects[index] as T;
    }

    public static void BindEvent(GameObject uiObject, Action<PointerEventData> action, MouseUIEvent mouseEventType = MouseUIEvent.Click )
    {
        UI_PointerEventHandler mouseEvent = ObjectUtil.GetOrAddComponent<UI_PointerEventHandler>(uiObject);

        switch (mouseEventType)
        {
            case MouseUIEvent.Click :
                mouseEvent.OnClickHandler -= action;
                mouseEvent.OnClickHandler += action;
                break;
                
            case MouseUIEvent.Enter :
                mouseEvent.OnEnterHandler -= action;
                mouseEvent.OnEnterHandler += action;
                break;
            
            case MouseUIEvent.Exit :
                mouseEvent.OnExitHandler -= action;
                mouseEvent.OnExitHandler += action;
                break;
            
            case MouseUIEvent.Up :
                mouseEvent.OnUpHandler -= action;
                mouseEvent.OnUpHandler += action;
                break;
        }
    }

}
