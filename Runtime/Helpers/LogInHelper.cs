using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PipPop.Services.Helpers
{
    [Serializable]
    public class LogInHelper : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField user;
        [SerializeField]
        private TMP_InputField pwd;
        [SerializeField]
        private Button loginBtn;
        public string username;
        public string password;

        public void SetUser(){
            username=user.text;
        }
        public void SetPassword(){
            password= pwd.text;
        }
        
    }
}