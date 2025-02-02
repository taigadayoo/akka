using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditCanvas : MonoBehaviour
{
    public Canvas Canvas;

    private bool _isVisible = true;
  
    private void HandleButtonInput(ButtonType? buttonType)
    {

        if (buttonType == ButtonType.South)
        {
            _isVisible = !_isVisible;
            Canvas.gameObject.SetActive(!_isVisible);
        }
    }
}
