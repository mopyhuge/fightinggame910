using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Lists")]
    public Color[] player_colors;
    public List<PlayerController> players_list = new List<PlayerController>();
    public Transform[] spawn_points;

    [Header("prefab refs")]
    public GameObject deathEffectprefab;
    public GameObject playerContPrefab;

    [Header("Components")]
    private AudioSource audio;
    public AudioClip[] game_fx;
    public Transform containerGroup;
    public TextMeshProUGUI timeText;

    [Header("level vars")]
    public float startTime;
    public float curTime;
    List<PlayerController> winningplayers;
    public bool canJoin;


    public static GameManager instance;

    private void Awake()
    {
        canJoin = true;
        audio = GetComponent<AudioSource>();
        instance = this;
        containerGroup = GameObject.FindGameObjectWithTag("UIContainer").GetComponent<Transform>();
        startTime = PlayerPrefs.GetFloat("roundTimer", 100);
        winningplayers = new List<PlayerController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        curTime = startTime;
        timeText.text = curTime.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime <= 0)
        {
            int highscore = 0;
            int index = 0;
            foreach (PlayerController player in players_list)
            {
                if (player.score > highscore)
                {
                    winningplayers.Clear();
                    highscore = player.score;
                    index = players_list.IndexOf(player);
                    winningplayers.Add(player);
                }
                else if(player.score == highscore)
                {
                    winningplayers.Add(player);
                }
                
            }

            if(winningplayers.Count > 1)
            {
                canJoin = false;
                foreach(PlayerController player in players_list)
                {
                    if (!winningplayers.Contains(player))
                    {
                        player.drop_out();
                    }
                }
                curTime = 30;
            }
            else
            {
                PlayerPrefs.SetInt("colorIndex", index);
                SceneManager.LoadScene("Win");
            }
        }
    }

    public void FixedUpdate()
    {
        curTime -= Time.deltaTime;
        timeText.text = ((int)curTime).ToString();
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        if (canJoin)
        {
            audio.PlayOneShot(game_fx[0]);
            player.GetComponentInChildren<SpriteRenderer>().color = player_colors[players_list.Count];

            PlayerContainerUI cont = Instantiate(playerContPrefab, containerGroup).GetComponent<PlayerContainerUI>();
            player.GetComponent<PlayerController>().setUI(cont);
            cont.initialize(player_colors[players_list.Count]);

            players_list.Add(player.GetComponent<PlayerController>());
            player.transform.position = spawn_points[Random.Range(0, spawn_points.Length)].position;
        }
    }
}
