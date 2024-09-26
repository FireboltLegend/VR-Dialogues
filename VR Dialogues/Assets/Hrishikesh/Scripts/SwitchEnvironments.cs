using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SwitchEnvironments : MonoBehaviour
{
    [SerializeField] private Image overlay;
    [SerializeField] private string targetSceneName;
    [SerializeField] private float timeToFade;

    private bool fadingToWhite = false;

    private void Start()
    {
        overlay.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(SwitchToScene(targetSceneName));

        if (Time.time < 0.1f)
            overlay.color = Color.clear;

        if (fadingToWhite)
            overlay.color = new Color(1, 1, 1, Mathf.Clamp(overlay.color.a + (1f / timeToFade * Time.deltaTime), 0, 1));
        else
            overlay.color = new Color(1, 1, 1, Mathf.Clamp(overlay.color.a - (1f / timeToFade * Time.deltaTime), 0, 1));
    }

    public IEnumerator SwitchToScene(string name)
    {
        fadingToWhite = true;
        yield return new WaitForSeconds(timeToFade);
        SceneManager.LoadScene(targetSceneName);
    }
}
