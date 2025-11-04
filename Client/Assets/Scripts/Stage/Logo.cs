using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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

        //Application.LoadLevel(Defines.GetScenesName(Defines.E_SCENES.TITLE));

        yield return null;
    }

}
