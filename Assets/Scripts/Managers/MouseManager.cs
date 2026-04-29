using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;   //消息事件的头文件
using System;

//[System.Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }  //触发事件 + 同时发送一个坐标（位置）
public class MouseManager : Singleton<MouseManager>
{
    //public static MouseManager Instance;

    RaycastHit hitInfo;

    public Texture2D point, doorway, attack, target, arrow;

    //public EventVector3 OnMouseClicked;
    public event Action<Vector3> OnMouseClicked;        //Action是系统自带的委托
    public event Action<GameObject> OnEnemyClicked;
    //private void Awake()
    //{
    //    if (Instance != null)
    //    {
    //        Destroy(gameObject);
    //    }
    //    Instance = this;
    //}

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
}

    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
            switch(hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;

            }
        }
    }
    void MouseControl()
    {
        if(Input.GetMouseButtonDown(0)&&hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseClicked?.Invoke(hitInfo.point);      //如果MouseClicked为空就不执行
            
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);

            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);


            if (hitInfo.collider.gameObject.CompareTag("Portal"))
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
        }
    }

}
