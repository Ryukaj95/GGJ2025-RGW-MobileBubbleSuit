using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Light : MonoBehaviour
{
    [SerializeField] private Color offColor = new Color(120, 120, 120);
    [SerializeField] private Color onColor;

    [SerializeField] private Image ledLight;

    bool isOn = false;

    private void Awake()
    {
        if (!ledLight) ledLight = GetComponent<Image>();

        TurnOff();

        // StartCoroutine(TestLight());
    }

    public void ToggleLight()
    {
        if (isOn)
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }

    public void TurnOn()
    {
        isOn = true;
        ledLight.color = onColor;
    }

    public void TurnOff()
    {
        isOn = false;
        ledLight.color = offColor;
    }

    private IEnumerator TestLight()
    {
        Debug.Log("TestLight: Start");
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("TestLight: ToggleLight");

            ToggleLight();
        }
    }
}
