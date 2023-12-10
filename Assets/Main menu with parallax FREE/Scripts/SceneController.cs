using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    private MenuController mainMenu;

    [SerializeField,Tooltip("Input the index of your room")]
    public int roomIndex;

	// Use this for initialization
	void Start () {
        mainMenu = FindObjectOfType<MenuController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (gameObject.transform.GetSiblingIndex() == 0 && !Menu_Controller.instance.backgroundsController.GetComponent<Animation>().isPlaying)
        {

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Menu_Controller.instance.closeScenes();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                Menu_Controller.instance.advanceScene();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                Menu_Controller.instance.goBackScene();
            }

            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(roomIndex);
            }

        }

	}
}
