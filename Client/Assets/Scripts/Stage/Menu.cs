using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 추가

public class Menu : MonoBehaviour
{
    void Start()
    {
        StartCoroutine("RunFadeOut");
    }

    IEnumerator RunFadeOut()
    {
       

        yield return new WaitForSeconds(1.2f);

        // Application.LoadLevel → SceneManager.LoadScene로 변경
        SceneManager.LoadScene(Defines.GetScenesName(Defines.E_SCENES.LOBBY));

        yield return null;
    }
}
