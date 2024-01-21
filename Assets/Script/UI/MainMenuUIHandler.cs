using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{

    public TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickName"))
            inputField.text = PlayerPrefs.GetString("PlayerNickName");
    }

    public void OnJoinGameClicked()
    {
        PlayerPrefs.SetString("PlayerNickName", inputField.text);
        PlayerPrefs.Save();

        SceneManager.LoadScene("world1");
    }


}