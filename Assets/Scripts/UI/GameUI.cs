using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Player player;

    [Space]
    [SerializeField] private Slider health;
    [SerializeField] private Slider stamina;
    [SerializeField] private Slider swingTimer;
}
