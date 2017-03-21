using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CursorOptions {
    public string name;
    public Texture2D texture;
    public Vector2 hotSpot;
    public CursorMode mode;
}

public class CursorManager : MonoBehaviour {
    [SerializeField]
    private CursorOptions[] m_cursors;
    private CursorOptions m_activeCursor;

    private void Start() {
        ElephantControl.OnAimStarted += OnAimStarted;
        ElephantControl.OnAimEnded += OnAimEnded;
        InGameMenu.OnGamePaused += OnGamePaused;
        InGameMenu.OnGameResumed += OnGameResumed;
    }

    private void OnGameResumed() {
        SetActive("Game");
    }

    private void OnGamePaused() {
        SetActive("Menu");
    }

    private void OnDestroy() {
        ElephantControl.OnAimStarted -= OnAimStarted;
        ElephantControl.OnAimEnded -= OnAimEnded;
    }

    private void OnAimEnded() {
        SetActive("Game");
    }

    private void OnAimStarted() {
        SetActive("Aim");
    }

    private void SetActive(string name) {
        if (m_activeCursor != null && m_activeCursor.name == name) {
            return;
        }

        var options = Find(name);
        if (options == null) {
            return;
        }

        SetActive(options);
    }

    private void SetActive(CursorOptions cursor) {
        m_activeCursor = cursor;
        Cursor.SetCursor(cursor.texture, cursor.hotSpot, cursor.mode);
    }

    private CursorOptions Find(string name) {
        foreach (var c in m_cursors) {
            if (c.name == name) {
                return c;
            }
        }

        return null;
    }
}
