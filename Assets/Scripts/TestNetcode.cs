using Unity.Netcode;
using UnityEngine;

namespace Erinn
{
    public class TestNetcode : NetworkBehaviour
    {
        public Rigidbody2D Rb2D;
        public Vector2 Direction;

        private void Update()
        {
            if (!IsOwner)
                return;
            var direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            InputServerRpc(direction);
        }

        private void FixedUpdate()
        {
            if (IsServer)
                Rb2D.velocity = Direction * 2f;
        }

        private void OnValidate()
        {
            Rb2D = GetComponent<Rigidbody2D>();
        }

        [ServerRpc]
        private void InputServerRpc(Vector2 direction)
        {
            Direction = direction;
        }
    }
}