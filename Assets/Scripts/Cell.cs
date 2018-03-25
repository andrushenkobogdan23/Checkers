using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] GameObject Selected;
    public Vector2 Position;
    public bool IsSelect;

    public void SelectCell() {
           Selected.SetActive(true);
        IsSelect = true;
    }

    public void DeSelectCell() {  
           Selected.SetActive(false);
        IsSelect = false;
    }


}

