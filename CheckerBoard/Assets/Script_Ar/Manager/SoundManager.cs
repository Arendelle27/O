using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class SoundManager:MonoSingleton<SoundManager>
    {
        [SerializeField, LabelText("音频混合"), Tooltip("放入音频混合组件")]
        public AudioMixer audioMixer;

        [SerializeField, LabelText("音乐音源"), Tooltip("放入音乐音源")]
        public AudioSource musicAudioSource;

        [SerializeField, LabelText("音效音源"), Tooltip("放入音效音源")]
        public AudioSource voiceAudioSource;

        [SerializeField, LabelText("音乐是否静音"), ReadOnly]
        private bool musicOn;
        public bool MusicOn
        {
            get
            {
                return musicOn;
            }
            set
            {
                musicOn = value;
                this.MusicMute(!musicOn);
            }
        }
        [SerializeField, LabelText("音效是否静音"), ReadOnly]
        private bool voiceOn;
        public bool VoiceOn
        {
            get
            {
                return voiceOn;
            }
            set
            {
                voiceOn = value;
                this.SoundMute(!voiceOn);
            }
        }
        [SerializeField, LabelText("音乐音量"), ReadOnly]
        private int musicVolume;
        public int MusicVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                musicVolume = value;
                this.SetVolume("Music",musicVolume);

            }
        }
        [SerializeField, LabelText("音效音量"), ReadOnly]
        private int voiceVolume;
        public int VoiceVolume
        {
            get
            {
                return voiceVolume;
            }
            set
            {
                voiceVolume = value;
                this.SetVolume("Sound",voiceVolume);
            }
        }

        private void Start()
        {
            this.MusicOn = SoundConfig.MusicOn;
            this.VoiceOn = SoundConfig.VoiceOn;
            this.MusicVolume = SoundConfig.MusicVolume;
            this.VoiceVolume = SoundConfig.VoiceVolume;
        }

        /// <summary>
        /// 音乐是否静音
        /// </summary>
        /// <param name="mute"></param>
        public void MusicMute(bool mute)
        {
            this.SetVolume("Music", mute?0:musicVolume);
        }

        /// <summary>
        /// 音效是否静音
        /// </summary>
        /// <param name="mute"></param>
        public void SoundMute(bool mute)
        {
            this.SetVolume("Sound", mute?0:voiceVolume);
        }

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void SetVolume(string name,int value)
        {
            float volume = value * 0.5f - 50f;
            this.audioMixer.SetFloat(name,volume);
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayMusic(string name) 
        {
            AudioClip clip=Resources.Load<AudioClip>(PathConfig.GetMusicPath(name));
            if(clip==null)
            {
                Debug.LogWarningFormat("PlayMusic:{0} not exited.",name);
                return;
            }
            if(musicAudioSource.isPlaying)
            {
                musicAudioSource.Stop();
            }

            musicAudioSource.clip=clip;
            musicAudioSource.Play();
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name"></param>
        public void PlayVoice(string name)
        {
            AudioClip clip=Resources.Load<AudioClip>(PathConfig.GetVoicePath(name));
            if(clip==null)
            {
                Debug.LogWarningFormat("PlaySound:{0} not exited.",name);
                return;
            }
            voiceAudioSource.PlayOneShot(clip);
        }

        public void RoundStart(int round)
        {
            switch (round)
            {
                case 0:
                    this.PlayMusic("bgm1");
                    break;
                //case 1:
                //    this.PlayMusic("bgm2");
                    //break;
                case 19:
                    this.PlayMusic("bgm2");
                    break;
                case 100:
                    this.PlayMusic("EndBgm");
                    break;
            }
        }


        protected void PlayClipOnAudioSource(AudioSource source,AudioClip clip,bool isLoop)
        {

        }
    }
}
