using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMgr : MonoBehaviour
{
    public GameObject army1;

    public GameObject m_Popup;

    public GameObject startUi;

    public GameObject Hand;


    public void OnClickPopup()
    {
        Debug.Log("HEY YOUNG ");
        m_Popup.SetActive(true);


    }

    public void OnClickedPopup()
    {
        m_Popup.SetActive(false);

        startUi.SetActive(true);


    }

    public void OnclickSol()
    {
        army1.SetActive(true);
    }

    public void Onclickhand()
    {
        Hand.SetActive(true);
    }
}
