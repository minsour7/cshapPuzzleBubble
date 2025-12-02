using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 추가

public class Title : MonoBehaviour
{
    public Text LogoText;

    void Start()
    {
        StartCoroutine("RunFadeOut");
    }

    IEnumerator RunFadeOut()
    {
        LogoText.lineSpacing = 0.0f;

        while (LogoText.lineSpacing <= 0.68)
        {
            LogoText.lineSpacing += 0.06f;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1.2f);

        // Application.LoadLevel → SceneManager.LoadScene로 변경
        SceneManager.LoadScene(Defines.GetScenesName(Defines.E_SCENES.TITLE));

        yield return null;
    }
}
