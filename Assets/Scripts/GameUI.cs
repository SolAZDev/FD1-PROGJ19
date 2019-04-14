using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
    public int test;
    public Player player;
    public Slider Health;
    public Text Dash, Jump, Fragment;

    public void UpdateHeath () {
        Health.maxValue = player.MaxHealth;
        Health.value = player.Health;
        Dash.text = player.DashCount.ToString ();
        Jump.text = player.MaxJumpCount.ToString ();
        Fragment.text = player.FragmentCount.ToString ();
    }

}