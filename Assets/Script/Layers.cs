using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    None,
    Text,
    Frame
}

[Serializable]
public class AdRendering
{
    public Layer[] layers;
}

[Serializable]
public class Layer
{
    public string type;
    public string path;
    public Placement[] placement;
    public Operation[] operations;

    public Type getType()
    {
        if(type.Equals("text"))
        {
            return Type.Text;
        } 
        else if (type.Equals("frame"))
        {
            return Type.Frame;
        } 
        else
        {
            return Type.None;
        }
    }
}

[Serializable]
public class Placement
{
    public Position position;
}

[Serializable]
public class Position
{
    public int x;
    public int y;
    public int width;
    public int height;
}

[Serializable]
public class Operation
{
    public string name;
    public string argument;
}

//{
//    "layers":[
//        {
//        "type":"frame",
//            "path":"http://lab.greedygame.com/arpit-dev/unity-assignment/assets/wcc2_f2.png",
//            "placement": [
//                {
//            "position":{
//                "x":0,
//                        "y":0,
//                        "width":650,
//                        "height":230
//                    }
//        }],
//            "operations": [
//                {
//            "name":"color",
//                    "argument":"#77FF0000"
//                }
//            ]
//        }
//    ]
//}
