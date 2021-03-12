//-------------------------------------------------
//                    TNet 3
// Copyright © 2012-2018 Tasharen Entertainment Inc
//-------------------------------------------------

using UnityEngine;
using TNet;
using System.IO;
using System.Collections;
using UnityTools = TNet.UnityTools;

/// <summary>
/// This script provides a main menu for all examples.
/// The menu is created in Unity's built-in Immediate Mode GUI system.
/// The menu makes use of the following TNet functions:
/// - TNManager.Connect
/// - TNManager.JoinChannel
/// - TNManager.LeaveChannel
/// - TNManager.Disconnect
/// - TNServerInstance.Start
/// - TNServerInstance.Stop
/// </summary>

[ExecuteInEditMode]
public class ExampleMenu : TNEventReceiver
{
    static ExampleMenu mInst = null;

    const float buttonWidth = 400f;
    const float buttonHeight = 40f;

    public enum IPType
    {
        IP_v_4,
        IP_v_6,
    }

    public int serverTcpPort = 5127;
    public IPType addressFamily = IPType.IP_v_4;
    public string mainMenu = "Example Menu";
    public string[] examples;

    [HideInInspector] public GUIStyle button;
    [HideInInspector] public GUIStyle text;
    [HideInInspector] public GUIStyle textLeft;
    [HideInInspector] public GUIStyle input;

    string mAddress = "127.0.0.1";
    string mMessage = "";
    float mAlpha = 0f;

    /// <summary>
    /// Keep only one instance of this object.
    /// 只保留该对象的一个实例。
    /// </summary>

    void Awake()
    {
        if (UnityEngine.Application.isPlaying)
        {
            // Choose IPv6 or IPv4
            TcpProtocol.defaultListenerInterface = (addressFamily == IPType.IP_v_6) ? System.Net.IPAddress.IPv6Any : System.Net.IPAddress.Any;

            // TNet will automatically switch UDP to IPv6 if TCP uses it, but you can specify an explicit one if you like
            // UdpProtocol.defaultNetworkInterface = useIPv6 ? System.Net.IPAddress.IPv6Any : System.Net.IPAddress.Any;

            if (mInst == null)
            {
                mInst = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
    }

    /// <summary>
    /// Start listening for incoming UDP packets right away.
    /// 马上开始监听传入的UDP数据包。
    /// </summary>

    void Start()
    {
        if (UnityEngine.Application.isPlaying)
        {
            // Start resolving IPs
            Tools.ResolveIPs(null);

            // We don't want mobile devices to dim their screen and go to sleep while the app is running
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }

    /// <summary>
    /// Adjust the server list's alpha based on whether it should be shown or not.
    /// </summary>

    void Update()
    {
        if (UnityEngine.Application.isPlaying)
        {
            float target = (LobbyClient.knownServers.list.size == 0) ? 0f : 1f;
            mAlpha = UnityTools.SpringLerp(mAlpha, target, 8f, Time.deltaTime);
        }
    }

    /// <summary>
    /// Show the GUI for the examples.
    /// </summary>

    void OnGUI()
    {
        if (!TNManager.isConnected)
        {
            DrawConnectMenu();
        }
        else
        {
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			if (!Application.isPlaying || Application.loadedLevelName == mainMenu)
#else
            if (!UnityEngine.Application.isPlaying || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == mainMenu)
#endif
            {
                DrawSelectionMenu();
            }
            else if (TNManager.channels.size > 0)
            {
                DrawExampleMenu();
            }
            DrawDisconnectButton();
        }
        DrawDebugInfo();
    }

    /// <summary>
    /// 如果客户端还没有连接到服务器，就会显示此菜单。
    /// This menu is shown if the client has not yet connected to the server.
    /// </summary>
    void DrawConnectMenu()
    {
        Rect rect = new Rect(Screen.width * 0.5f - 200f * 0.5f - mAlpha * 120f,
            Screen.height * 0.5f - 100f, 200f, 220f);

        // Show a half-transparent box around the upcoming UI
        GUI.color = new Color(1f, 1f, 1f, 0.5f);
        GUI.Box(UnityTools.PadRect(rect, 8f), "");
        GUI.color = Color.white;

        GUILayout.BeginArea(rect);
        {
            GUILayout.Label("Server Address", text);
            mAddress = GUILayout.TextField(mAddress, input, GUILayout.Width(200f));

            if (GUILayout.Button("Connect", button))
            {
                // We want to connect to the specified destination when the button is clicked on.
                // "OnConnect" function will be called sometime later with the result.
                TNManager.Connect(mAddress);
                mMessage = "Connecting...";
            }

            if (TNServerInstance.isActive)
            {
                GUI.backgroundColor = Color.red;

                if (GUILayout.Button("Stop the Server", button))
                {
                    // Stop the server, saving all the data
                    TNServerInstance.Stop();
                    mMessage = "Server stopped";
                }
            }
            else
            {
                GUI.backgroundColor = Color.green;

                if (GUILayout.Button("Start a LAN Server", button))
                {
#if UNITY_WEBPLAYER
					mMessage = "Can't host from the Web Player due to Unity's security restrictions";
#else
                    //启动一个本地服务器，如果可能，加载保存的数据
                    //服务器的UDP端口无关紧要，因为它是可选的，
                    //客户端通过Packet.ResponseSetUDP得到通知。
                    // Start a local server, loading the saved data if possible
                    // The UDP port of the server doesn't matter much as it's optional,
                    // and the clients get notified of it via Packet.ResponseSetUDP.
                    int udpPort = Random.Range(10000, 40000);
                    LobbyClient lobby = GetComponent<LobbyClient>();

                    if (lobby == null)
                    {
                        if (TNServerInstance.Start(serverTcpPort, udpPort, "server.dat"))
                            TNManager.Connect();
                    }
                    else
                    {
                        TNServerInstance.Type type = (lobby is TNUdpLobbyClient) ?
                            TNServerInstance.Type.Udp : TNServerInstance.Type.Tcp;

                        if (TNServerInstance.Start(serverTcpPort, udpPort, lobby.remotePort, "server.dat", type))
                            TNManager.Connect();
                    }
                    mMessage = "Server started";
#endif
                }

                // Start a local server that doesn't use sockets. It's ideal for testing and for single player gameplay.
                // 启动一个不使用套接字的本地服务器。这是测试和单人游戏的理想选择。
                if (GUILayout.Button("Start a Virtual Server", button))
                {
                    mMessage = "Server started";
                    TNServerInstance.Start("server.dat");
                    TNManager.Connect();
                }
            }
            GUI.backgroundColor = Color.white;

            if (!string.IsNullOrEmpty(mMessage)) GUILayout.Label(mMessage, text);
        }
        GUILayout.EndArea();

        if (mAlpha > 0.01f)
        {
            rect.x = rect.x + (Screen.width - rect.xMin - rect.xMax) * mAlpha;
            DrawServerList(rect);
        }
    }

    /// <summary>
    /// This menu is shown when a connection has been established and the player has not yet joined any channel.
    /// 当一个连接已经建立并且玩家还没有加入任何频道时，这个菜单就会显示出来。
    /// </summary>
    void DrawSelectionMenu()
    {
        int count = examples.Length;

        Rect rect = new Rect(
            Screen.width * 0.5f - buttonWidth * 0.5f,
            Screen.height * 0.5f - buttonHeight * 0.5f * count,
            buttonWidth, buttonHeight);

        for (int i = 0; i < count; ++i)
        {
            string sceneName = examples[i];

            if (GUI.Button(rect, sceneName, button))
            {
                // 单击按钮时，加入指定的通道。When a button is clicked, join the specified channel.
                // 创建通道的人也设置了将由每个加入的人加载的场景。
                // Whoever creates the channel also sets the scene that will be loaded by everyone who joins.
                // 在本例中，我们指定我们刚刚点击的场景的名称。
                // In this case, we are specifying the name of the scene we've just clicked on.
                TNManager.JoinChannel(i + 1, sceneName, true, 255, null);
            }
            rect.y += buttonHeight;
        }
        rect.y += 20f;
    }

    /// <summary>
    /// This menu is shown if the player has joined a channel.
    /// 如果玩家加入了一个频道，这个菜单就会显示出来。
    /// </summary>
    void DrawExampleMenu()
    {
        Rect rect = new Rect(0f, Screen.height - buttonHeight, 200f, buttonHeight);

        if (GUI.Button(rect, "Main Menu", button))
        {
            // Leaving the channel will cause the "OnLeaveChannel" to be sent out.
            TNManager.LeaveAllChannels();
        }
    }

    /// <summary>
    /// 当连接建立或连接失败时，将调用此函数。
    /// 连接到一个服务器并不意味着连接的玩家现在可以立即
    /// 看到彼此，因为他们还没有加入一个频道。只有已经加入的玩家
    /// 某些通道能够在同一通道中看到并与其他玩家交互。
    /// 你可以打电话给TNManager。如果你愿意，可以在这里加入通道，但在这个例子中，我们让玩家自己选择。
    /// This function is called when a connection is either established or it fails to connect.
    /// Connecting to a server doesn't mean that the connected players are now immediately able
    /// to see each other, as they have not yet joined a channel. Only players that have joined
    /// some channel are able to see and interact with other players in the same channel.
    /// You can call TNManager.JoinChannel here if you like, but in this example we let the player choose.
    /// </summary>

    protected override void OnConnect(bool success, string message)
    {
        Debug.Log("Connected: " + success + " " + message + " (Player ID #" + TNManager.playerID + ")");
        mMessage = message;

        // Make it possible to use UDP using a random port
        if (success && !TNServerInstance.isLocal) TNManager.StartUDP(Random.Range(10000, 50000));
    }

    /// <summary>
    /// Simply print a message when disconnected. If you have a "disconnected" scene, you would load it in OnDisconnect.
    /// </summary>

    protected override void OnDisconnect()
    {
        Debug.Log("Disconnected");
    }

    /// <summary>
    /// OnJoinChannel notification is broadcast whenever we join a channel, successfully or not.
    /// </summary>

    protected override void OnJoinChannel(int channelID, bool success, string msg)
    {
        Debug.Log("Joined channel #" + channelID + " " + success + " " + msg);
    }

    /// <summary>
    /// OnLeaveChannel notification is broadcast whenever we leave a channel.
    /// It's also sent prior to OnDisconnect() when the player gets disconnected
    /// </summary>

    protected override void OnLeaveChannel(int channelID)
    {
        Debug.Log("Left channel #" + channelID);

        if (TNManager.channels.size == 0)
        {
#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel(mainMenu);
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenu);
#endif
        }
    }

    /// <summary>
    /// The disconnect button is only shown if we are currently connected.
    /// </summary>

    void DrawDisconnectButton()
    {
        Rect rect = new Rect(Screen.width - 200f, Screen.height - buttonHeight, 200f, buttonHeight);

        if (GUI.Button(rect, "Disconnect", button))
        {
            // Disconnecting while in some channel will cause "OnLeaveChannel" to be sent out first,
            // followed by "OnDisconnect". Disconnecting while not in a channel will only trigger
            // "OnDisconnect".
            TNManager.Disconnect();
        }
    }

    /// <summary>
    /// 打印一些额外的信息，如ping和这是哪种连接类型。
    /// Print some additional information such as ping and which type of connection this is.
    /// </summary>
    void DrawDebugInfo()
    {

        GUILayout.Label("LAN: " + Tools.localAddress.ToString(), textLeft);

        if (UnityEngine.Application.isPlaying)
        {
            if (Tools.isExternalIPReliable)
                GUILayout.Label("WAN: " + Tools.externalAddress, textLeft);
            else GUILayout.Label("WAN: Resolving...", textLeft);

            if (TNManager.isConnected)
                GUILayout.Label("Ping: " + TNManager.ping + " (" + (TNManager.canUseUDP ? "TCP+UDP" : "TCP") + ")", textLeft);
        }
    }

    /// <summary>
    /// 绘制已知的局域网服务器列表。
    /// Draw the list of known LAN servers.
    /// </summary>
    void DrawServerList(Rect rect)
    {
        GUI.color = new Color(1f, 1f, 1f, mAlpha * mAlpha * 0.5f);
        GUI.Box(UnityTools.PadRect(rect, 8f), "");
        GUI.color = new Color(1f, 1f, 1f, mAlpha * mAlpha);

        GUILayout.BeginArea(rect);
        {
            GUILayout.Label("LAN Server List", text);

            // List of discovered servers已发现的服务器列表
            List<ServerList.Entry> list = LobbyClient.knownServers.list;

            // Server list example script automatically collects servers that have recently announced themselves
            // 服务器列表示例脚本自动收集最近发布自己的服务器
            for (int i = 0; i < list.size; ++i)
            {
                var ent = list.buffer[i];

                // NOTE: I am using 'internalAddress' here because I know all servers are hosted on LAN.
                // 注意:我在这里使用“internalAddress”，因为我知道所有的服务器都托管在局域网上。
                // 如果你的主机不在你的局域网内，你应该使用“externalAddress”来代替。
                // If you are hosting outside of your LAN, you should probably use 'externalAddress' instead.
                if (GUILayout.Button(ent.internalAddress.ToString(), button))
                {
                    TNManager.Connect(ent.internalAddress, ent.internalAddress);
                    mMessage = "Connecting...";
                }
            }
        }
        GUILayout.EndArea();
        GUI.color = Color.white;
    }
}
