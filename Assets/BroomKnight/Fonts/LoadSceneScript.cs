using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneScript : MonoBehaviour {
    public string sceneName = "";

    private IEnumerator musicTurnDown;
    public AudioSource musicSource;
	// Use this for initialization
	void Awake () {
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<AudioSource>().Play();

            if (musicTurnDown == null)
            {
                musicTurnDown = MusicTurnDown();
                StartCoroutine(musicTurnDown);
            }
            SceneManager.LoadScene(sceneName);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<AudioSource>().Play();
            Application.Quit();
        }

    }

    IEnumerator MusicTurnDown()
    {
        while(this != null)
        {
            musicSource.volume -= Time.unscaledDeltaTime * 35;
            yield return new WaitForEndOfFrame();
        }
    }
}
