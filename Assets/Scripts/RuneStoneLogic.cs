using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneStoneLogic : MonoBehaviour
{
    public const float completionPerSecond = 0.1f;
    public const float colorChangePerSecond = 1f;
    private float ritualCompletion = 0;
    private bool triggered = false;

    public ParticleSystem particles;
    public Renderer renderer;

    private ProgressionLogic progression;

    // Start is called before the first frame update
    void Start()
    {
        particles.Stop();
        progression = GameObject.FindGameObjectWithTag("ProgressionLogic").GetComponent<ProgressionLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!triggered)
        {
            return;
        }
        if (ritualCompletion == 1)
        {
            progression.MarkCompleted();
            triggered = false;
            particles.Stop();
            return;
        }
        
        ritualCompletion = Mathf.Min(1.0f, ritualCompletion + completionPerSecond * Time.deltaTime);
        var hsl = renderer.material.GetVector("_Tint");
        var color = Color.HSVToRGB(hsl[0], hsl[1], hsl[2], true);
        //color.r = Mathf.Min(16.0f, color.b + colorChangePerSecond * Time.deltaTime);

        //color.r = Mathf.Min(5.0f, color.r + colorChangePerSecond * Time.deltaTime);
        color.b = Mathf.Min(14.0f, color.b + colorChangePerSecond * Time.deltaTime);
        //color.g = Mathf.Min(5.0f, color.g + colorChangePerSecond * Time.deltaTime);
        //color.r = 0; color.g = 0;
        Color.RGBToHSV(color, out hsl.x, out hsl.y, out hsl.z);
        renderer.material.SetVector("_Tint", hsl);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ritualCompletion == 1.0f) {
            return;
        }
        particles.Play();
        triggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        particles.Stop();
        triggered = false;
    }
}
