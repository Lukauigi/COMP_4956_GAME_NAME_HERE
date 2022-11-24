﻿using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Static Class to observe the two spawned fighter's character status.
/// Waits for changes to their name / health / stocks and updates the game ui.
/// Author(s): Jason Cheung
/// Date: Nov 19 2022
/// Remarks:
/// - CurrentHealth and Stocks are network properties that will notify this observer to update itself.
/// </summary>
public class NetworkFighterObserver : NetworkBehaviour
{
    // Static instance of NetworkFighterObserver so other scripts can access it
    public static NetworkFighterObserver Observer = null;

    // other scene objects to reference
    protected GameManager _gameManager;

    // Game UI Components to update
    [SerializeField] private RawImage _playerTwoAvatar;
    [SerializeField] private TextMeshProUGUI _playerTwoName;
    [SerializeField] private TextMeshProUGUI _playerTwoStocks;
    [SerializeField] private TextMeshProUGUI _playerTwoCurrentHealth;
    [SerializeField] private TextMeshProUGUI _playerTwoMaxHealth;

    [SerializeField] private RawImage _playerOneAvatar;
    [SerializeField] private TextMeshProUGUI _playerOneName;
    [SerializeField] private TextMeshProUGUI _playerOneStocks;
    [SerializeField] private TextMeshProUGUI _playerOneCurrentHealth;
    [SerializeField] private TextMeshProUGUI _playerOneMaxHealth;


    // the fighter they are controlling
    private NetworkObject playerOne;    
    private NetworkObject playerTwo;

    // player references - fusion gives them a player id
    private int playerOneRef = 0;
    private int playerTwoRef = 0;


    // previous character status values; used to compare and gather fighter stats
    private int prevPlayerOneCurrentHealth;
    private int prevPlayerOneStocks;
    private int prevPlayerTwoCurrentHealth;
    private int prevPlayerTwoStocks;


    // Awake is called when the script instance is being loaded
    public void Awake()
    {
        if (!Observer) Observer = this;
        Debug.Log("NetworkPlayerObserver instance awake: " + Observer);

        // hide this game object
        this.gameObject.SetActive(false);
    }


    // Start is called after Awake, and before Update
    public void Start()
    {
        CacheOtherObjects();
    }

    // Helper method to initialize OTHER game objects and their components
    private void CacheOtherObjects()
    {
        if (!_gameManager) _gameManager = GameManager.Manager;
    }

    // Method to cache the selected and spawned fighters
    [Rpc(sources: RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CachePlayers(int playerOneRef, NetworkObject playerOne, int playerTwoRef, NetworkObject playerTwo)
    {
        // assign player ref
        this.playerOneRef = playerOneRef;
        this.playerTwoRef = playerTwoRef;

        // assign network fighter
        this.playerOne = playerOne;
        this.playerTwo = playerTwo;

        // TODO - assign selected images

        // TODO - change set nicknames to the login'd names
        this.playerOne.gameObject.GetComponent<NetworkPlayer>().NickName = "Player " + playerOneRef.ToString();
        this.playerTwo.gameObject.GetComponent<NetworkPlayer>().NickName = "Player " + playerTwoRef.ToString();

        RPC_CacheFighterStatusUI();
    }

    // Method to initialize the fighter status ui based on the newly assigned network fighters
    [Rpc(sources: RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_CacheFighterStatusUI()
    {
        // TODO - set selected images
        // TODO - change set nicknames to the login'd names

        // set intial values
        if (playerOne) // check for null
        {
            _playerOneName.text = "Player " + playerOneRef.ToString();
            Debug.Log("updated player name text: " + _playerOneName.text);
            _playerOneMaxHealth.text = "/ " + playerOne.gameObject.GetComponent<Health>().CurrentHealth.ToString();

            // store the initial values to the "old" ones
            prevPlayerOneCurrentHealth = playerOne.gameObject.GetComponent<Health>().CurrentHealth;
            prevPlayerOneStocks = playerOne.gameObject.GetComponent<Stock>().Stocks;

            // continue caching the ui
            _playerOneCurrentHealth.text = prevPlayerOneCurrentHealth.ToString();
            _playerOneStocks.text = prevPlayerOneStocks.ToString();
        } else
        {
            Debug.Log("CacheFighterStatusUI Error - Player One null.");
        }

        if (playerTwo)
        {
            _playerTwoName.text = "Player " + playerTwoRef.ToString();
            Debug.Log("updated player name text: " + _playerTwoName.text);
            _playerTwoMaxHealth.text = "/ " + playerTwo.gameObject.GetComponent<Health>().CurrentHealth.ToString();

            // store the initial values to the "old" ones
            prevPlayerTwoCurrentHealth = playerTwo.gameObject.GetComponent<Health>().CurrentHealth;
            prevPlayerTwoStocks = playerTwo.gameObject.GetComponent<Stock>().Stocks;

            // continue caching the ui
            _playerTwoCurrentHealth.text = prevPlayerTwoCurrentHealth.ToString();
            _playerTwoStocks.text = prevPlayerTwoStocks.ToString();
        } else
        {
            Debug.Log("CacheFighterStatusUI Error - Player Two null.");
        }

        // unhide this game object
        this.gameObject.SetActive(true);

    }


    // Method to update the fighter status ui; health and stock changes
    public void UpdateFighterStatus()
    {
        // fighter status values to check
        int playerOneCurrentHealth = playerOne.gameObject.GetComponent<Health>().CurrentHealth;
        int playerOneStocks = playerOne.gameObject.GetComponent<Stock>().Stocks;

        int playerTwoCurrentHealth = playerTwo.gameObject.GetComponent<Health>().CurrentHealth;
        int playerTwoStocks = playerTwo.gameObject.GetComponent<Stock>().Stocks;

        // compare values with old stored ones
        // check player 1 health
        if (prevPlayerOneCurrentHealth != playerOneCurrentHealth)
        {
            UpdateFighterStatusUI(_playerOneCurrentHealth, playerOneCurrentHealth);
            prevPlayerOneCurrentHealth = playerOneCurrentHealth;
        }
        // check player 1 stocks
        if (prevPlayerOneStocks != playerOneStocks)
        {
            UpdateFighterStatusUI(_playerOneStocks, playerOneStocks);
            prevPlayerOneStocks = playerOneStocks;
        }

        // check player 2 health
        if (prevPlayerTwoCurrentHealth != playerTwoCurrentHealth)
        {
            UpdateFighterStatusUI(_playerTwoCurrentHealth, playerTwoCurrentHealth);
            prevPlayerTwoCurrentHealth = playerTwoCurrentHealth;
        }
        // check player 2 stocks
        if (prevPlayerTwoStocks != playerTwoStocks)
        {
            UpdateFighterStatusUI(_playerTwoStocks, playerTwoStocks);
            prevPlayerTwoStocks = playerTwoStocks;
        }

        // store the values again
        prevPlayerOneCurrentHealth = playerOneCurrentHealth;
        prevPlayerOneStocks = playerOneStocks;
        prevPlayerTwoCurrentHealth = playerTwoCurrentHealth;
        prevPlayerTwoStocks = playerTwoStocks;


    }

    // Helper method to update the fighter status ui for the passed element.
    private void UpdateFighterStatusUI(TextMeshProUGUI textObj, int newValue)
    {
        textObj.text = newValue.ToString();
    }


}
