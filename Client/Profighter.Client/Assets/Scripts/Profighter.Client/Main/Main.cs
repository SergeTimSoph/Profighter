using Profighter.Client.Camera;
using Profighter.Client.Character;
using Profighter.Client.SceneManagement;
using Profighter.Client.UI;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private Character localCharacterPrefab;

    [SerializeField]
    private WorldStreamer worldStreamer;

    [SerializeField]
    private OrbitCamera orbitCamera;

    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;

        DeviceScreenInfo.Setup();
    }

    private void Start()
    {
        var localCharacter = Instantiate(localCharacterPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity);
        worldStreamer.Setup(localCharacter.transform);
        localCharacter.Setup(orbitCamera, worldStreamer.WorldObjectsRoot, worldStreamer);
    }
}
