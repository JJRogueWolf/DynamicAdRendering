using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class ApiParse : MonoBehaviour
{

    private void OnEnable()
    {
        GameManager.textOnlyEvent += textOnly;
        GameManager.textColorEvent += textColor;
        GameManager.frameOnlyEvent += frameOnly;
        GameManager.frameColorEvent += frameColor;
    }

    private void OnDisable()
    {
        GameManager.textOnlyEvent -= textOnly;
        GameManager.textColorEvent -= textColor;
        GameManager.frameOnlyEvent -= frameOnly;
        GameManager.frameColorEvent -= frameColor;
    }

    private void textOnly() { StartCoroutine(GetRequest("http://lab.greedygame.com/arpit-dev/unity-assignment/templates/text_only.json")); }
    private void textColor() { StartCoroutine(GetRequest("http://lab.greedygame.com/arpit-dev/unity-assignment/templates/text_color.json")); }
    private void frameOnly() { StartCoroutine(GetRequest("http://lab.greedygame.com/arpit-dev/unity-assignment/templates/frame_only.json")); }
    private void frameColor() { StartCoroutine(GetRequest("http://lab.greedygame.com/arpit-dev/unity-assignment/templates/frame_color.json")); }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page].Split('.')[0] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page].Split('.')[0] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    GameManager.adRendering = JsonUtility.FromJson<AdRendering>(webRequest.downloadHandler.text);
                    GameManager.onRender();
                    Debug.Log("Type = " + GameManager.adRendering.layers[0].getType());
                    break;
            }
        }
    }
}
