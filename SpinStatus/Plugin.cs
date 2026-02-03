using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Net;

namespace SpinStatus;

[BepInProcess("SpinRhythm.exe")]
[BepInPlugin(Guid, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string Guid = "xyz.bacur.plugins.spinstatus";
    public const string Name = "SpinStatus";
    public const string Version = "0.4.0";

    internal static new ManualLogSource Logger;
    private static Harmony _harmony;

    public static HttpServer server;
    public static int port = 38304;

    private void Awake()
    {
        Logger = base.Logger;

        server = new HttpServer(IPAddress.Any, port); 
        
        server.AddWebSocketService<Server.ServerBehavior>("/");
        server.Start();

        Logger.LogInfo($"Server started on port {port} (Broadcast enabled)");

        _harmony = new Harmony(Guid);
        _harmony.PatchAll(typeof(Patches.NoteEventHandler));
        _harmony.PatchAll(typeof(Patches.SceneEventHandler));
    }

    private void OnDestroy()
    {
        if (server != null)
        {
            server.Stop();
        }
        _harmony.UnpatchSelf();
    }
}
