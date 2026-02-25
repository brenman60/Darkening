using UnityEngine;

public class HouseLight : MonoBehaviour
{
    private new Light light;

    private void Start()
    {
        light = GetComponent<Light>();

        GameManager.NightSectionChanged += SectionChanged;
    }

    private void SectionChanged()
    {
        switch (GameManager.NightSection)
        {
            case NightSection.Survival:
                light.enabled = true;
                break;
            case NightSection.Darkening:
                light.enabled = false;
                break;
        }
    }

    private void OnDestroy()
    {
        GameManager.NightSectionChanged -= SectionChanged;
    }
}
