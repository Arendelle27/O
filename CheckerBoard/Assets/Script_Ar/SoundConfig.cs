using Managers;
using UnityEngine;

    public class SoundConfig
    {
        /// <summary>
        /// ��������
        /// </summary>
        public static bool MusicOn
        {
            get
            {
                return PlayerPrefs.GetInt("Music", 1) == 1;
            }
            set
            {
                PlayerPrefs.SetInt("Music", value ? 1 : 0);
                SoundManager.Instance.MusicOn = value;
            }
        }

        /// <summary>
        /// ������Ч
        /// </summary>
        public static bool VoiceOn
        {
            get
            {
                return PlayerPrefs.GetInt("Voice", 1) == 1;
            }
            set
            {
                PlayerPrefs.SetInt("Voice", value ? 1 : 0);
                SoundManager.Instance.VoiceOn = value;
            }
        }

        /// <summary>
        /// �޸���������
        /// </summary>
        public static int MusicVolume
        {
            get
            {
                return PlayerPrefs.GetInt("MusicVolume", 100);
            }
            set
            {
                PlayerPrefs.SetInt("MusicVolume", value);
                SoundManager.Instance.MusicVolume = value;
            }
        }

        /// <summary>
        /// �޸���Ч����
        /// </summary>
        public static int VoiceVolume
        {
            get
            {
                return PlayerPrefs.GetInt("VoiceVolume", 100);
            }
            set
            {
                PlayerPrefs.SetInt("VoiceVolume", value);
                SoundManager.Instance.VoiceVolume = value;
            }
        }

        ~SoundConfig()
        {
            PlayerPrefs.Save();
        }
    }
