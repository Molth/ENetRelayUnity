using Unity.Netcode;
using UnityEngine;

namespace Erinn
{
    public sealed class Test : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                NetworkManager.Singleton.StartHost();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                NetworkManager.Singleton.StartClient();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                NetworkManager.Singleton.StartServer();
            }
        }
    }
}