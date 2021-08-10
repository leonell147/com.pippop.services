using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PipPop.Services.Helpers;
using UnityEngine;
using UnityEngine.Networking;

namespace PipPop.Services
{
    public class CoreServices : MonoBehaviour
    {
        private  string coreURL= "https://pippopcore.herokuapp.com/" ;
        public  string token=null;
        [Serializable]
        public struct data{
            public string username;
            public string password;
        };
        private struct dataLogin{public string token;}
        public System.Action<ResponsePCHelper> onUpdatePC;
        public void LogIn(LogInHelper log)
        {   
            StartCoroutine(CoroutineLogIn(log));
        }


        private  IEnumerator CoroutineLogIn(LogInHelper log)
        {
            //this 
            string logindataJsonString= JsonUtility.ToJson(log);

            var request = new UnityWebRequest (coreURL + "auth/generate-token", "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(logindataJsonString);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            Debug.Log(request.responseCode);

            if((request.result==UnityWebRequest.Result.ConnectionError) || request.result==UnityWebRequest.Result.ProtocolError){
                Debug.Log(request.error);
            }
            else{
                var responseData = request.downloadHandler.data;
                token = Encoding.UTF8.GetString(responseData);
                dataLogin helper= JsonUtility.FromJson<dataLogin>(token);
                token= helper.token;
            }

        }
        public static void SignUp()
        {
            Application.OpenURL("https://request.pippopstudios.com/signup");
        }
        public  bool IsLogedIn()
        {
            return (!(token == null));
        }


        public void Trading(string operation,int amount, int gameid){
            StartCoroutine(CoroutineTrading(operation, amount, gameid));  
        }

        private IEnumerator CoroutineTrading(string operation, int amount, int gameid){
            if(IsLogedIn()){
                string buffer;
                TradingHelper helper= new TradingHelper();
                helper.action=operation;
                helper.gameId=gameid;
                helper.pipcoins=amount;
                string transaction= JsonUtility.ToJson(helper);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(transaction);


                var request = new UnityWebRequest (coreURL + "api/balance/trading", "POST");
                request.SetRequestHeader("token", token);
                request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
                yield return request.SendWebRequest();

                if((request.result==UnityWebRequest.Result.ConnectionError) || request.result==UnityWebRequest.Result.ProtocolError){
                    Debug.Log(request.error);
                }
                else{
                    var responseData = request.downloadHandler.data;
                    buffer= Encoding.UTF8.GetString(responseData);
                    ResponsePCHelper update=JsonUtility.FromJson<ResponsePCHelper>(buffer);
                    onUpdatePC.Invoke(update);
                }
            }
        }

    }
}