using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Skill", menuName = "Game Stuff/Skill", order = 1)]
public class Skill : ScriptableObject {
    public int Cost = 3;
    public bool Projectile = false;
    public int Effect = 10;

}