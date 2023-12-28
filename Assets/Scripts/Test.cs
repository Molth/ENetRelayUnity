using Unity.Netcode;
using UnityEngine;

namespace Erinn
{
    public class Test : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                NetworkManager.Singleton.StartHost();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}