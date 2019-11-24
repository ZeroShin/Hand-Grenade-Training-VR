using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ScenesMgr : MonoBehaviour
{

    public GameObject t_Touch;

    public GameObject m_Popup;

   

    public enum SceneNumber { Title, Main }
    public void OnClickNewGame()
    {

        SceneManager.LoadScene((int)SceneNumber.Main);
        Debug.Log("young bye !~ ");
        SceneManager.LoadScene("Main");
    }



    public void OnTouch()
    {
        t_Touch.SetActive(true);
    }

    public void OnClickBack()
    {
        SceneManager.LoadScene((int)SceneNumber.Title);
    }

   
    
    
    
    


}
