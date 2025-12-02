using System.Linq;
using System.Threading.Tasks;
using MainTool.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace MainTool.Infrastructure
{
    public class ConfigService
    {
        public string BaseEndPoint;
        
        public bool IsInitialized { get; private set; }
        public bool HasError { get; private set; }

        public async Task Init()
        {
            try
            {
                using (UnityWebRequest request = UnityWebRequest.Get(Constants.CONFIG_URL))
                {
                    UnityWebRequestAsyncOperation operation = request.SendWebRequest();
                    
                    while (!operation.isDone)
                        await Task.Yield();
                    
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogWarning($"Config request failed: {request.error}");
                        HasError = true;
                        BaseEndPoint = string.Empty;
                        IsInitialized = true;
                        return;
                    }

                    string jsonResponse = request.downloadHandler.text;
                
                    if (string.IsNullOrEmpty(jsonResponse))
                    {
                        Debug.LogWarning("Config response is empty");
                        BaseEndPoint = string.Empty;
                        IsInitialized = true;
                        return;
                    }

                    JObject tagetObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);

                    string baseEndPoint = tagetObject
                        .Properties()
                        .Select(p => p.Value.ToString())
                        .FirstOrDefault(v => v.StartsWith("http"));

                    BaseEndPoint = baseEndPoint ?? string.Empty;
                    IsInitialized = true;
                    Debug.Log($"BaseEndPoint = {BaseEndPoint}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Exception in ConfigService.Init: {ex.Message}");
                BaseEndPoint = string.Empty;
                HasError = true;
                IsInitialized = true;
            }
        }
    }
}