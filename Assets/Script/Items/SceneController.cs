
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private float _sceneFadeDuration = 0.5f;

    private SceneFade _sceneFade;
    private bool _isLoading;

    private void Awake()
    {
        _sceneFade = GetComponentInChildren<SceneFade>(true);
    }

    private IEnumerator Start()
    {
        if (_sceneFade != null)
            yield return _sceneFade.FadeInCoroutine(_sceneFadeDuration);
    }

    public void LoadScene(string sceneName)
    {
        if (_isLoading || string.IsNullOrEmpty(sceneName))
            return;

        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        _isLoading = true;

        if (_sceneFade != null)
            yield return _sceneFade.FadeOutCoroutine(_sceneFadeDuration);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        if (operation != null)
            yield return operation;
        else
            _isLoading = false;
    }
}
