using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfilePage : MonoBehaviour
{
    [SerializeField]
    private TMP_Text Username;

    /// <summary>
    /// Start is called before the first frame update and update the user name to the welcome message.
    /// </summary>
    void Start()
    {
        Username.text = "Welcome " + PlayerPrefs.GetString("PlayerName") + "!";

    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
}
