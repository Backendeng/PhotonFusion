using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine;

public class NetworkRunnerHandler : MonoBehaviour
{

    public NetworkRunner networkRunnerPrefab;

    NetworkRunner networkRunner;

    //private void Awake()
    //{
    //    NetworkRunner networkRunnerInScene = FindObjectOfType<NetworkRunner>();

    //    // If we already have a network runner in the scene then we should not crate another one but rather use the existing one
    //    if (networkRunnerInScene != null)
    //        networkRunner = networkRunnerInScene;
    //}

    // Start is called before the first frame update
    void Start()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network runner";

            //if (SceneManager.GetActiveScene().name != "MainMenu")
            //{
                var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
            //}

            Debug.Log($"Server NetworkRunner Started.");
        }
    }

    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        // create a new network runner, old one is being shut down
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network runner - Migrated";

        var clientTask = InitializeNetworkRunnerHostMigration(networkRunner, hostMigrationToken);

        Debug.Log($"Host migraiton started");
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager != null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneManager = sceneManager,
        });
    }

    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            //GameMode = gameMode, // ignore, game mode comes with the host migration token
            //Address = address,
            //Scene = scene,
            //SessionName = "TestRoom",
            //Initialized = initialized,
            SceneManager = sceneManager,
            HostMigrationToken = hostMigrationToken, // contains all necessarhy info to restart the runner
            HostMigrationResume = HostMigrationResume // this will be invoke tho resume in simuation
        });
    }

    void HostMigrationResume(NetworkRunner runner)
    {

    }



}
