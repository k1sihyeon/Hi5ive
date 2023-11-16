using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;

namespace Hi5ive.Network.Lobby {

    public class LobbySingleton : MonoBehaviour {
        public static LobbySingleton instance { get; private set; }

        private string playerName;

        private void Awake() {
            instance = this;
        }

        public async void Authenticate(string playerName) {
            this.playerName = playerName;
            InitializationOptions initOptions = new InitializationOptions();
            initOptions.SetProfile(playerName);

            await UnityServices.InitializeAsync(initOptions);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}

