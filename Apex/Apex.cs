using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using RestSharp;

namespace Apex
{
    class Apex : BaseScript
    {
        PlayerList playerList = new PlayerList();
        private static Apex _apex;
        public static WebClient client = new WebClient();
        public static string ApiUrl = "https://apexecs.com/api/test";

        //private HashSet<CommandProcessor> CommandHandlers;

        public static Apex GetInstance()
        {
            return _apex;
        }

        public Apex()
        {
            Debug.WriteLine("Starting Apex...");

            _apex = this;

            // Config.Init();
            // Log.Init();
            // ClassLoader.Init();

            RegisterEventHandler("playerConnecting", new Action<Player, string, CallbackDelegate>(HandlePlayerConnecting));
            RegisterEventHandler("apex:requestAccount", new Action<Player, int>(HandleAccountRequest));
            RegisterEventHandler("playerDropped", new Action<Player, CallbackDelegate>(HandlePlayerDropped));
            RegisterEventHandler("apex:position", new Action<Player, string, string>(HandlePlayerPosition));
        }

        public void RegisterEventHandler(string name, Delegate action)
        {
            EventHandlers[name] += action;
        }

        // Download account information from Apex servers
        private void HandleAccountRequest([FromSource] Player source, int sessionID)
        {
            Debug.WriteLine($"[APEX | {playerList.Count()}/32] Requesting account data for player {source.Name} (Session #{sessionID})");
        }

        // Handle a player position event
        private void HandlePlayerPosition([FromSource] Player source, string name, string position)
        {
            NameValueCollection request = new NameValueCollection
            {
                { "type", "position" },
                { "position", position },
                { "player", source.ToString() },
                { "username", name }
            };

            client.UploadValues(ApiUrl, request);
        }

        // Connecting
        private void HandlePlayerConnecting([FromSource] Player source, string name, CallbackDelegate setKickReason)
        {
            if(!IsSteamAllowed(source.Identifiers["steam"]))
            {
                Debug.WriteLine($"[APEX | {playerList.Count()}/32] {source.Name} attempted to connect as server ID {source.Handle} (Steam ID {source.Identifiers["steam"]}) but did not have a Steam profile linked to an Apex account.");
                setKickReason("This server is running Apex, which requires you to link your Steam profile to an account before you can connect. Please visit https://apexecs.com/register to create an account.");
                Function.Call(Hash.CANCEL_EVENT);
            }
        }

        // Dropped connection
        private void HandlePlayerDropped([FromSource] Player source, CallbackDelegate @delegate)
        {
            NameValueCollection request = new NameValueCollection
            {
                { "type", "dropped" },
                { "player", source.ToString() }
            };

            client.UploadValues(ApiUrl, request);
        }

        // Check Steam ID
        bool IsSteamAllowed(string id)
        {
            Debug.WriteLine($"Checking Steam ID {id} against Apex game authentication servers.");

            //var request = new RestRequest();
            //request.Resource = "game/auth/steam";
            //request.AddParameter("id", id, ParameterType.UrlSegment);

            //var res = ApexAPI.Execute(request);



            //var client = new RestClient("https://apexecs.com/api");
           // var r = new RestRequest("game/auth/steam", Method.POST);
            //r.AddParameter("id", id);

            // execute the request
            //IRestResponse r = client.Execute(request);
           // var content = r.Content; // raw content as string

            //Debug.WriteLine($"Received response: {content}");


            /**var steamIDHex = new Dictionary<string, string>
            {
                { "id", id }
            };

            var content = new FormUrlEncodedContent(steamIDHex);
            var response = client.PostAsync("https://apexecs.com/api/auth/game/steam", content);
            var responseString = response.Content.ReadAsStringAsync();**/

            //
            // 76561198056987290
            if (id == "110000105c3da9a")
                return true;
            return false;
        }
    }
}
