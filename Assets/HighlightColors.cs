﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightColors : MonoBehaviour {

    GameManager _gameManager;
    UIMenuHandler menuHandler;

    [SerializeField]
    ParticleSystem[] childParticles;
    [SerializeField]
    Vector3[] selectedSize;
    [SerializeField]
    MeshRenderer[] meshes;

	// Use this for initialization
	void Start () {
        _gameManager = FindObjectOfType<GameManager>();
        menuHandler = FindObjectOfType<UIMenuHandler>();

        childParticles = GetComponentsInChildren<ParticleSystem>();

        meshes = GetComponentsInChildren<MeshRenderer>();
        selectedSize = new Vector3[meshes.Length];


	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < selectedSize.Length; i++)
        {
            selectedSize[i] = meshes[i].transform.localScale;
        }
        for (int i = 0; i < childParticles.Length; i++)
        {
            var shape = childParticles[i].shape;
            shape.scale = selectedSize[i];
        }

        if(menuHandler.activeMenu.allPlayersNeedToBeReady && !menuHandler.activeMenu.isColorPickMenu)
        {
            for(int m = 0; m<meshes.Length; m++)
            {
                Color color = _gameManager.Colors[m];
                color.a = 0.5f;
                meshes[m].material.color = color;

                var main = childParticles[m].main;
                main.startColor = color;
                
            }

        }
	}
}
