using UnityEngine;

public class GameEndKey : MonoBehaviour
{
    //ESCキーでゲーム終了
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
