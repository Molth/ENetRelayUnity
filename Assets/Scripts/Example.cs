using Unity.Netcode;
using UnityEngine;

namespace Erinn
{
    public class Example : NetworkBehaviour
    {
        private void Update()
        {
            if (!IsOwner)
                return;
            if (Input.GetKeyDown(KeyCode.D))
            {
                Log.Info(1);
                TestServerRpc();
            }
        }

        [ServerRpc]
        private void TestServerRpc()
        {
            Log.Info(2);
            TestClientRpc();
        }

        [ClientRpc]
        private void TestClientRpc()
        {
            Log.Info(3);
        }
    }
}