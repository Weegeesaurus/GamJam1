using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpoint;
    public bool active = false;
    public bool finalPoint = false;
    public float flashbang = 12;
    [SerializeField]
    private float startingGlow = 1.5f;
    private float startY;

    private MeshRenderer render;
    private Material mat;
    private Color originalColor;
    private float flash;
    private GameObject checkpointText;

    void Start()
    {
        checkpointText = GameObject.Find("Canvas/Checkpoint Text");
        flash = flashbang;
        render = GetComponent<MeshRenderer>();
        mat = render.material;
        originalColor = mat.color;
        float factor = Mathf.Pow(2, startingGlow);
        Color color = new Color(originalColor.r * factor, originalColor.g * factor, originalColor.b * factor);
        mat.color = color;

        startY = transform.position.y;
    }

    void Update()
    {
        if (active)
        {
            transform.Rotate(new Vector3(0, 140, 0) * Time.deltaTime);
        }
        else
        {
            transform.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (active)
        {
            if (transform.position.y < startY+4f)
            {
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y,startY+2f,.1f), transform.position.z);
            }
            if (flash >2)
            {
                float factor = Mathf.Pow(2, flash);
                Color color = new Color(originalColor.r * factor, originalColor.g * factor, originalColor.b * factor);
                mat.color = color;
                flash -= Time.deltaTime*3;
            }
        }
    }

    public void SelfCheck(int num)
    {
        if (checkpoint < num)
        {
            Destroy(gameObject);
        }
    }

    public bool Activate(int num)
    {
        if (checkpoint > num)
        {
            float factor = Mathf.Pow(2, flashbang);
            Color color = new Color(originalColor.r * factor, originalColor.g * factor, originalColor.b * factor);
            mat.color = color;
            active = true;
            checkpointText.GetComponent<CheckpointText>().Activate();
            if (finalPoint)
            {
                checkpointText.GetComponent<CheckpointText>().ChangeText("Level Completed!");
            }
            return true;
        }
        return false;
    }
    void OnDrawGizmos()
    {
        // Draw a semitransparent blue cube at the transforms position
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        //Handles.Label(transform.position, checkpoint.ToString());
    }
}
