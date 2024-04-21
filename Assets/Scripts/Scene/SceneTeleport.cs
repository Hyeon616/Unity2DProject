using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    //[SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_MainMenu;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3();



    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        PlayerController player = collision.GetComponent<PlayerController>();

        if(player != null )
        {

            float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f)  ? player.transform.position.x : scenePositionGoto.x; 
            float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f)  ? player.transform.position.y : scenePositionGoto.y;

           // float zPosition = 0f;

            // Scene ¿Ãµø
            //SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));

        }

    }


}
