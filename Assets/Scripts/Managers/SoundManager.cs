using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    // MP3 Player   -> AudioSource
    // MP3 음원     -> AudioClip
    // 관객(귀)     -> AudioListener

    float _effectVolumn = 0.6f;
    float _bgmVolumn = 0.6f;

    public float EffectVolumn
    {
        get {
            if (PlayerPrefs.HasKey("Effect"))
                _effectVolumn = PlayerPrefs.GetFloat("Effect");
            else
            {
                _effectVolumn = 0.6f;
                PlayerPrefs.SetFloat("Effect", _effectVolumn);
            }
            return _effectVolumn;
        }
        set {
            _effectVolumn = Mathf.Clamp01(value);
            UpdateVolumn();
            PlayerPrefs.SetFloat("Effect", _effectVolumn);
        }
    }

    public float BGMVolumn
    {
        get
        {
            if (PlayerPrefs.HasKey("BGM"))
                _bgmVolumn = PlayerPrefs.GetFloat("BGM");
            else
            {
                _bgmVolumn = 0.6f;
                PlayerPrefs.SetFloat("BGM", _bgmVolumn);
            }
            return _bgmVolumn;
        }
        set
        {
            _bgmVolumn = Mathf.Clamp01(value);
            UpdateVolumn();
            PlayerPrefs.SetFloat("BGM", _bgmVolumn);
        }
    }


    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                _audioSources[i].dopplerLevel = 0;
                _audioSources[i].reverbZoneMix = 0;
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f, float volumn = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch, volumn);
    }

    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f, float volumn = 1.0f)
	{
        if (audioClip == null)
            return;

		if (type != Define.Sound.Effect)
		{
			AudioSource audioSource = _audioSources[(int)type];
			if (audioSource.isPlaying)
				audioSource.Stop();

			audioSource.pitch = pitch;
			audioSource.clip = audioClip;
            audioSource.volume = _effectVolumn *  volumn;

            if (type == Define.Sound.Bgm)
                audioSource.volume = _bgmVolumn * volumn;

            if (type == Define.Sound.Movement)
                audioSource.volume = _effectVolumn * 0.3f * volumn;
			audioSource.Play();
		}
		else
		{
			AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            
			audioSource.pitch = pitch;
            audioSource.volume = _effectVolumn * volumn * 0.5f;
            audioSource.PlayOneShot(audioClip);
		}
	}

    void UpdateVolumn()
    {
        _audioSources[(int)Define.Sound.Bgm].volume = 1.0f * _bgmVolumn;
        _audioSources[(int)Define.Sound.Movement].volume = 1.0f * _effectVolumn * 0.3f;
    }


    public void Stop(Define.Sound type = Define.Sound.Effect)
    {
        if (_audioSources[(int)type].isPlaying)
            _audioSources[(int)type].Stop();
    }

	public AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
		if (path.Contains("Sounds/") == false)
			path = $"Sounds/{path}";

		AudioClip audioClip = null;

		if (type == Define.Sound.Bgm)
		{
			audioClip = Managers.Resource.Load<AudioClip>(path);
		}
		else
		{
			if (_audioClips.TryGetValue(path, out audioClip) == false)
			{
				audioClip = Managers.Resource.Load<AudioClip>(path);
				_audioClips.Add(path, audioClip);
			}
		}

		if (audioClip == null)
			Debug.Log($"AudioClip Missing ! {path}");

		return audioClip;
    }
}
