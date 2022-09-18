using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Camera m_cCamera;
    [SerializeField] private TextMeshProUGUI m_cLevelText;
    [SerializeField] private TextMeshProUGUI m_cMusicText;
    [SerializeField] private GameObject m_cPlayerObject;
    [SerializeField] private PlayerCharacter m_cPlayerComponent;
    [SerializeField] private float m_fMusicFadeFactor = 1f;
    [SerializeField] private float m_fDEBUGPitchOverride = 0f;
    [SerializeField] private AudioSource m_cMusicA;
    [SerializeField] private AudioSource m_cMusicB;
    [SerializeField] private AudioSource m_cMusicC;
    [SerializeField] private AudioSource m_cMusicD;
    [SerializeField] private AudioSource m_cMusicE;
    [SerializeField] private AudioSource m_cMusicF;
    [SerializeField] private AudioSource m_cMusicG;

    private enum TrackType
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
    }

    private Rigidbody2D m_cPlayerBody;
    
    private int m_iLevel = 0;
    
    [SerializeField] 
    private float m_fLevelSize = 9.5f;

    private bool m_bReady = false;
    
    // Start is called before the first frame update
    void Start()
    {
        m_cMusicA.clip.LoadAudioData();
        m_cMusicB.clip.LoadAudioData();
        m_cMusicC.clip.LoadAudioData();
        m_cMusicD.clip.LoadAudioData();
        m_cMusicE.clip.LoadAudioData();
        m_cMusicF.clip.LoadAudioData();
        m_cMusicG.clip.LoadAudioData();
        
        m_cMusicA.loop = true;
        m_cMusicB.loop = true;
        m_cMusicC.loop = true;
        m_cMusicD.loop = true;
        m_cMusicE.loop = true;
        m_cMusicF.loop = true;
        m_cMusicG.loop = true;
        
        m_cMusicA.volume = 0.5f;
        m_cMusicB.volume = 0f;
        m_cMusicC.volume = 0f;
        m_cMusicD.volume = 0f;
        m_cMusicE.volume = 0f;
        m_cMusicF.volume = 0f;
        m_cMusicG.volume = 0f;
        
        m_cMusicA.Play();
        m_cMusicB.Play();
        m_cMusicC.Play();
        m_cMusicD.Play();
        m_cMusicE.Play();
        m_cMusicF.Play();
        m_cMusicG.Play();

        m_cPlayerBody = m_cPlayerObject.GetComponent<Rigidbody2D>();

        m_bReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bReady)
            return;
        
        // Update Cam
        if (m_iLevel != GetLevelFromPosition())
        {
            m_iLevel = GetLevelFromPosition();
            m_cCamera.transform.SetPositionAndRotation(new Vector3(0, (m_iLevel-1)*m_fLevelSize, -10), Quaternion.identity);

            m_cLevelText.text = $"Level: {m_iLevel}";
        }
        
        // Mix music for levels
        m_cMusicA.volume = Fade(m_cMusicA.volume, GetVolumeForTrackAndLevel(TrackType.A, m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicB.volume = Fade(m_cMusicB.volume, GetVolumeForTrackAndLevel(TrackType.B, m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicC.volume = Fade(m_cMusicC.volume, GetVolumeForTrackAndLevel(TrackType.C, m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicD.volume = Fade(m_cMusicD.volume, GetVolumeForTrackAndLevel(TrackType.D, m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicE.volume = Fade(m_cMusicE.volume, GetVolumeForTrackAndLevel(TrackType.E, m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicF.volume = Fade(m_cMusicF.volume, GetVolumeForTrackAndLevel(TrackType.F, m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicG.volume = Fade(m_cMusicG.volume, GetVolumeForTrackAndLevel(TrackType.G, m_iLevel), Time.deltaTime * m_fMusicFadeFactor);

        m_cMusicA.pitch = Fade(m_cMusicA.pitch, GetPitchForLevel(m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicB.pitch = Fade(m_cMusicB.pitch, GetPitchForLevel(m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicC.pitch = Fade(m_cMusicC.pitch, GetPitchForLevel(m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicD.pitch = Fade(m_cMusicD.pitch, GetPitchForLevel(m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicE.pitch = Fade(m_cMusicE.pitch, GetPitchForLevel(m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicF.pitch = Fade(m_cMusicF.pitch, GetPitchForLevel(m_iLevel), Time.deltaTime * m_fMusicFadeFactor);
        m_cMusicG.pitch = Fade(m_cMusicG.pitch, GetPitchForLevel(m_iLevel), Time.deltaTime * m_fMusicFadeFactor);

        m_cMusicText.text = $"A: {Round2(m_cMusicA.volume)} | B: {Round2(m_cMusicB.volume)} | C : {Round2(m_cMusicC.volume)} | D : {Round2(m_cMusicD.volume)} | E : {Round2(m_cMusicE.volume)} | F : {Round2(m_cMusicF.volume)} | G : {Round2(m_cMusicG.volume)}";
    }

    string Round2(float value)
    {
        return $"{Math.Round(value, 2)}";
    }
    
    float Fade(float value, float target, float factor)
    {
        return value + ((target - value) * factor);
    }

    /// <summary>
    /// Gets the volume level for tracks based on the game level
    /// </summary>
    /// <param name="track">Track type</param>
    /// <param name="level">Game Level</param>
    /// <returns>Volume level</returns>
    private float GetVolumeForTrackAndLevel(TrackType track, int level)
    {
        switch (track)
        {
            // Base track
            case TrackType.A:
                return 0.45f;
                //Get louder until level 5
                if (level <= 5)
                {
                    return Math.Min(0.55f, 0.5f + (0.025f * level)); // Max 0.55f
                }
                // Fade back down until 10
                else if (level <= 10)
                {
                    return Math.Max(0.45f, 0.6f - (0.1f * (level - 5))); // Min 0.45f
                }
                // Stay at 0.35 until after level 15
                else if (level <= 15)
                {
                    return 0.45f;
                }
                // Fade back up
                else
                {
                    return Math.Min(0.65f, 0.35f + (0.1f * (level - 15))); // Max 0.65f
                }

            // First adventure
            case TrackType.B:
                // Don't turn up until level 2
                if (level < 2)
                    return 0f;
                
                // Fade out after level 7
                if (level > 7)
                    return Math.Max(0f, 0.6f - (0.2f * (level-7))); // Min 0
                
                // Mix between level 2 and 7
                return Math.Min(0.6f, 0.6f + (0.03f * (level-2))); // Max 0.7f

            // Eerie 1
            case TrackType.C:
                // Don't turn up until level 6
                if (level <= 6)
                    return 0f;
                
                // Fade out after level 11
                if (level > 11)
                    return Math.Max(0f, 0.5f - (0.2f * (level-11))); // Min 0
                
                // Mix between level 6 and 11
                return Math.Min(0.6f, 0.5f + (0.03f * (level-6))); // Max 0.6f

            // Eerie 2
            case TrackType.D:
                // Don't turn up until level 10
                if (level <= 10)
                    return 0f;
                
                // Fade out after level 14
                if (level > 14)
                    return Math.Max(0f, 0.5f - (0.2f * (level-14))); // Min 0
                
                // Mix between level 10 and 14
                return Math.Min(0.6f, 0.5f + (0.03f * (level-10))); // Max 0.6f

            // Overcome
            case TrackType.E:
                // Don't turn up until level 13
                if (level <= 13)
                    return 0f;
                
                // Fade out after level 17
                if (level > 17)
                    return Math.Max(0f, 0.5f - (0.2f * (level-17))); // Min 0
                
                // Mix between level 13 and 17
                return Math.Min(0.6f, 0.5f + (0.03f * (level-13))); // Max 0.6f

            // Stabby one
            case TrackType.F:
                // Don't turn up until level 15
                if (level <= 15)
                    return 0f;
                
                // Go a bit quieter after level 18
                if (level > 18)
                    return Math.Max(0.3f, 0.5f - (0.1f * (level-18))); // Min 0.3f
                
                // Increase in volume after level 15
                return Math.Min(0.6f, 0.3f + (0.1f * (level-15))); // Max 0.6f

            // Electric Guitar
            case TrackType.G:
                // Don't turn up until level 17
                if (level < 17)
                    return 0f;
                
                // Increase in volume at that point
                return Math.Min(0.6f, 0.3f + (0.1f * (level-17))); // Max 0.6f
        }

        return 1f;
    }
    
    /// <summary>
    /// Gets the music pitch based on the level
    /// </summary>
    /// <param name="level">The game level</param>
    /// <returns>pitch level</returns>
    private float GetPitchForLevel(int level)
    {
        if (m_fDEBUGPitchOverride > 0f)
            return m_fDEBUGPitchOverride;
        
        // ToDo: alter this per level for whatever feels good
        
        // Increase the first 3
        if (level <= 3)
            return Math.Min(1.1f, 1f + (0.03f * level)); // Max 1.1f
        
        // After that go back to normal
            return Math.Max(1f, 1.25f - (0.1f * level)); // Min 1f
    }
    
    /// <summary>
    /// Gets the game level from the player's position
    /// </summary>
    /// <returns>Returns the level</returns>
    private int GetLevelFromPosition()
    {
        return Mathf.FloorToInt(1 + (m_cPlayerBody.position.y + (m_fLevelSize/2f)) / m_fLevelSize);
    }
}
