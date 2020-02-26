using UnityEngine;
using UnityEngine.SceneManagement;
 
public class changeScene : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}