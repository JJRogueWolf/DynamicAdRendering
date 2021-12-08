using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        None,
        TextOnly,
        TextColor,
        FrameOnly,
        FrameColor
    }

    public static State state;
    public static string inputString = "";

    public static AdRendering adRendering;

    public delegate void TextOnly();
    public delegate void TextColor();
    public static event TextOnly textOnlyEvent;
    public static event TextColor textColorEvent;
    public delegate void FrameOnly();
    public delegate void FrameColor();
    public static event FrameOnly frameOnlyEvent;
    public static event FrameColor frameColorEvent;

    public delegate void Render();
    public static event Render render;

    public static void onTextOnly()
    {
        if (textOnlyEvent != null)
        {
            textOnlyEvent.Invoke();
        }
    }

    public static void onTextColor()
    {
        if (textColorEvent != null)
        {
            textColorEvent.Invoke();
        }
    }

    public static void onFrameOnly()
    {
        if (frameOnlyEvent != null)
        {
            frameOnlyEvent.Invoke();
        }
    }

    public static void onFrameColor()
    {
        if (frameColorEvent != null)
        {
            frameColorEvent.Invoke();
        }
    }

    public static void onRender()
    {
        if (render != null)
        {
            render.Invoke();
        }
    }
}
