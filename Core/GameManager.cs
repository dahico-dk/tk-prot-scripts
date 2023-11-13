using Core;
using Core.Animation;
using Core.Combat;
using Core.Combat.Special;
using Core.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]GameObject PauseMenu;
    [SerializeField] GameObject AudioCamera;
    [SerializeField] Slider AudioSlider;
    [SerializeField] Slider SFXSlider;
    AudioSource source;
    public bool ultiState = false;
    void Start()
    {      
        source = AudioCamera.GetComponent<AudioSource>();
        AudioSlider.value = source.volume;
        SFXVolumeLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    void Pause()
    {       
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
        SetPlayer(false);

    }
    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        SetPlayer(true);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void VolumeLevel()
    {
        //var level = slider.value;
        source.volume = AudioSlider.value;
    }
    public void SFXVolumeLevel()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("SFX"))
        {
            var comp = item.GetComponent<AudioSource>();
            comp.volume = SFXSlider.value;
        }
    }
    void SetPlayer(bool status)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().enabled=status;
        player.GetComponent<PlayerRotation>().enabled = status;
        player.GetComponent<PlayerSpecialAttack>().enabled = status;
        player.GetComponent<PlayerCombat>().enabled = status;
        player.GetComponent<PlayerAnimate>().enabled = status;
        player.GetComponent<Health>().enabled = status;
    }

}
