using UnityEngine;

namespace GASSample.Gameplay
{
    /// <summary>
    /// Spawns the player character at the <see cref="PlayerStart"/> of the scene and connects it to the
    /// camera. One spawner per gameplay scene is enough for a single-player demo.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private GASSamplePlayerCharacter characterPrefab;

        public GASSamplePlayerCharacter Spawn()
        {
            PlayerStart start = FindFirstObjectByType<PlayerStart>();
            Vector3 position = start != null ? start.transform.position : transform.position;
            Quaternion rotation = start != null ? start.transform.rotation : transform.rotation;

            GASSamplePlayerCharacter character = Instantiate(characterPrefab, position, rotation);

            PlayerCamera camera = FindFirstObjectByType<PlayerCamera>();
            if (camera != null)
            {
                character.SetCamera(camera);
                camera.SetTarget(character.CameraFocus);
            }

            return character;
        }
    }
}
