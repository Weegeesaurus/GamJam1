using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointText : MonoBehaviour
{
    public bool active = false;
    private float fadeTime;
    public float timeVisible;
    public float floatSpeed;

    private RectTransform tf;
    private Text txt;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<RectTransform>();
        txt = GetComponent<Text>();
        startPos = tf.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (fadeTime < 3*(timeVisible/4))
            {
                tf.position = new Vector3(tf.position.x, tf.position.y + floatSpeed * Time.deltaTime, tf.position.z);
                if (fadeTime < 2*timeVisible/3)
                {
                    txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, fadeTime/(timeVisible/2));
                    if (txt.color.a <= 0)
                    {
                        //active = false;
                    }
                }
            }
            fadeTime -= Time.deltaTime;
        }
    }

    public void Activate()
    {
        fadeTime = timeVisible;
        active = true;
        tf.position = startPos;
        txt.color = new Color(txt.color.r, txt.color.g, txt.color.b, 1);
    }
    public void ChangeText(string text)
    {
        txt.text = text;
    }
}
