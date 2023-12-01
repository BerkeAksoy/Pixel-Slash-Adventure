using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    public Sprite selected, notSelected;

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void selectedImage()
    {
        image.sprite = selected;
    }

    public void notSelectedImage()
    {
        image.sprite = notSelected;
    }
}
