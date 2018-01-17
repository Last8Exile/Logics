using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorsManager : MonoBehaviour {

    public static CursorsManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.Find("CursorsManager").GetComponent<CursorsManager>();
                mInstance.Init();
            }
            return mInstance;
        }
    }
    private static CursorsManager mInstance;

    public void Init()
    {

    }

    [SerializeField]
    private Texture2D mResizeCursor;

    public void SetCursor(CursorType type)
    {
        switch (type)
        {
            case CursorType.Default:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
            case CursorType.Resize:
                Cursor.SetCursor(mResizeCursor, Vector2.one*32, CursorMode.Auto);
                break;
            default:
                throw new ArgumentOutOfRangeException("type", type, null);
        }
    }
}

public enum CursorType
{
    Default,
    Resize
}
