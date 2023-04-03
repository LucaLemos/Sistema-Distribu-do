using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInventoryController : MonoBehaviour
{
    [SerializeField]
    public UiInventoryPage inventoryPage;

    [Header("Server Identity")]
    [SerializeField]
    private ServerIdentity serverIdentity;

    void Update()
    {
        if(serverIdentity.IsControlavel()) {
            if (Input.GetKeyDown(KeyCode.I)) {
                if(inventoryPage.isActiveAndEnabled == false) {
                    inventoryPage.Show();
                }else {
                    inventoryPage.Hide();
                }
            }
        }
    }
}
