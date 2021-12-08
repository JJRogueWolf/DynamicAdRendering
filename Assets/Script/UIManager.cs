using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject FrontPanel;
    [SerializeField]
    private GameObject RenderPanel;

    [SerializeField]
    private GameObject ButtonPanel;
    [SerializeField]
    private GameObject InputPanel;

    [SerializeField]
    private InputField inputText;

    [SerializeField]
    private Text toastText;

    [SerializeField]
    private Font poppinsFont;

    private void OnEnable()
    {
        GameManager.render += onRender;
        GameManager.showToast += showToast;
        GameManager.state = GameManager.State.None;
        FrontPanel.SetActive(true);
        RenderPanel.SetActive(false);
        ButtonPanel.SetActive(true);
        InputPanel.SetActive(false);
        toastText.enabled = false;
    }

    private void OnDisable()
    {
        GameManager.render -= onRender;
        GameManager.showToast -= showToast;
        clearRender();
    }

    private void HandleRender()
    {
        FrontPanel.SetActive(false);
        RenderPanel.SetActive(true);
        switch (GameManager.state)
        {
            case GameManager.State.TextOnly:
                GameManager.onTextOnly();
                break;
            case GameManager.State.TextColor:
                GameManager.onTextColor();
                break;
            case GameManager.State.FrameOnly:
                GameManager.onFrameOnly();
                break;
            case GameManager.State.FrameColor:
                GameManager.onFrameColor();
                break;
        }
    }

    public void onTextOnlyClicked()
    {
        ButtonPanel.SetActive(false);
        InputPanel.SetActive(true);
        GameManager.state = GameManager.State.TextOnly;
    }

    public void onTextColorClicked()
    {
        ButtonPanel.SetActive(false);
        InputPanel.SetActive(true);
        GameManager.state = GameManager.State.TextColor;
    }

    public void onFrameOnlyClicked()
    {
        GameManager.state = GameManager.State.FrameOnly;
        HandleRender();
    }

    public void onFrameColorClicked()
    {
        GameManager.state = GameManager.State.FrameColor;
        HandleRender();
    }

    public void onContinueClicked()
    {
        if (inputText.text.Equals(""))
        {
            GameManager.onShowToast("Input Field Is Empty", 2);
            return;
        }
        GameManager.inputString = inputText.text;
        HandleRender();
    }

    public void onCancelClicked()
    {
        GameManager.state = GameManager.State.None;
        ButtonPanel.SetActive(true);
        InputPanel.SetActive(false);
    }

    public void onBackClicked()
    {
        GameManager.state = GameManager.State.None;
        FrontPanel.SetActive(true);
        RenderPanel.SetActive(false);
        ButtonPanel.SetActive(true);
        InputPanel.SetActive(false);
        clearRender();
    }

    private void clearRender()
    {
        foreach (Transform child in RenderPanel.transform)
        {
            if (child.tag != "back")
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void showToast(string text, float duration) {
        StartCoroutine(showToastCOR(text, duration));
    }

    private IEnumerator showToastCOR(string text, float duration) {
        Color orginalColor = toastText.color;
        toastText.text = text;
        toastText.enabled = true;
        yield return fadeInAndOut(toastText, true, 0.5f);
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        yield return fadeInAndOut(toastText, false, 0.5f);
        toastText.enabled = false;
        toastText.color = orginalColor;
    }

    IEnumerator fadeInAndOut(Text targetText, bool fadeIn, float duration)
    {
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }

    private void onRender()
    {
        foreach(Layer layer in GameManager.adRendering.layers)
        {
            if (layer.getType() == Type.Text)
            {
                if (GameObject.Find("/TextRender") == null)
                    renderText(layer);
            }
            else if (layer.getType() == Type.Frame)
            {
                if (GameObject.Find("/FrameRender") == null)
                    renderFrame(layer);
            }
        }
    }

    private void renderText(Layer layer)
    {
        GameObject textObject = new GameObject("TextRender");
        textObject.transform.SetParent(RenderPanel.transform);

        Text myText = textObject.AddComponent<Text>();
        myText.font = poppinsFont;
        myText.resizeTextForBestFit = true;
        myText.resizeTextMinSize = 20;
        myText.resizeTextMaxSize = 150;
        myText.rectTransform.sizeDelta = new Vector2(layer.placement[0].position.width, layer.placement[0].position.height);
        myText.rectTransform.position = new Vector3(layer.placement[0].position.x, layer.placement[0].position.y, 0);
        myText.rectTransform.localScale = Vector3.one;
        myText.alignment = TextAnchor.MiddleCenter;
        if (layer.operations != null && layer.operations[0].name.Equals("color"))
        {
            Color newCol;
            if (ColorUtility.TryParseHtmlString(layer.operations[0].argument, out newCol))
            {
                myText.color = newCol;
                if (newCol.a == 0)
                {
                    InvalidText("Alpha is 0, on coverting to RGBA!\nHex code : " + layer.operations[0].argument);
                }
            }
        }
        else
        {
            myText.color = Color.black;
        }
        myText.text = GameManager.inputString;
    }

    private void renderFrame(Layer layer)
    {
        GameObject frameObject = new GameObject("FrameRender");
        frameObject.transform.SetParent(RenderPanel.transform);

        RawImage frame = frameObject.AddComponent<RawImage>();
        frame.rectTransform.sizeDelta = new Vector2(layer.placement[0].position.width, layer.placement[0].position.height);
        frame.rectTransform.position = new Vector3(layer.placement[0].position.x, layer.placement[0].position.y, 0);
        frame.rectTransform.localScale = Vector3.one;
        StartCoroutine(DownloadImage(frame, layer.path));
        if (layer.operations != null && layer.operations[0].name.Equals("color"))
        {
            Color newCol;
            if (ColorUtility.TryParseHtmlString(layer.operations[0].argument, out newCol))
            {
                frame.color = newCol;
                if(newCol.a == 0)
                {
                    InvalidText("Invalid Image.\nAlpha is 0, on coverting to RGBA!\nHex code : " + layer.operations[0].argument);
                }
            }
        }
    }

    IEnumerator DownloadImage(RawImage image ,string MediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }
        else
        {
            InvalidText("Invalid Image.\n" + request.error);
            Debug.Log(request.error);
        }
    }

    private void InvalidText(string text)
    {
        GameObject InvalidTextObject = new GameObject("InvalidTextRender");
        InvalidTextObject.transform.SetParent(RenderPanel.transform);
        Text myText = InvalidTextObject.AddComponent<Text>();
        myText.font = poppinsFont;
        myText.resizeTextForBestFit = true;
        myText.resizeTextMinSize = 20;
        myText.resizeTextMaxSize = 150;
        myText.rectTransform.sizeDelta = new Vector2(600, 400);
        myText.rectTransform.position = Vector3.zero;
        myText.rectTransform.localScale = Vector3.one;
        myText.color = Color.black;
        myText.alignment = TextAnchor.MiddleCenter;
        myText.text = text;
    }
}
