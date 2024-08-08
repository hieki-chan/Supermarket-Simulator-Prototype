using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Supermarket.MVP;
using Supermarket.Player;
using Hieki.Pubsub;

[Serializable]
public class SettingsPresenter : Presenter<PlaySettings, SettingsUI>
{
    const string fileName = "settings.set";

    [NonEditable, SerializeField]
    bool shouldSave;
    CancellationTokenSource tokenSource = new CancellationTokenSource();

    Topic musicVolumeTopic = Topic.FromMessage<MusicVolumeMessage>();
    Topic sfxVolumeTopic = Topic.FromMessage<SFXVolumeMessage>();

    IPublisher publisher = new Publisher();

    public override void Initialize()
    {
        model = LocalSaveLoad.Load<PlaySettings>(fileName);
        if(model == null)
        {
            model = new PlaySettings()
            {
                resolution = (int)(QualitySettings.count * .85f),
                music = 1,
                sfx = 1,
                cameraSensitivity = 1,
            };
            LocalSaveLoad.Save(model, fileName);
        }

        //Resolution
        view.resolution.wholeNumbers = true;
        view.resolution.minValue = 0;
        view.resolution.maxValue = QualitySettings.count;
        view.resolution.value = model.resolution;
        view.resolution.onValueChanged.AddListener(OnSetQuality);
        QualitySettings.SetQualityLevel(model.resolution);

        //Music
        view.music.wholeNumbers = false;
        view.music.minValue = 0.01f;
        view.music.maxValue = 2;
        view.music.value = model.music;
        view.music.onValueChanged.AddListener(OnSetMusicVolume);

        //SFX
        view.sfx.wholeNumbers = false;
        view.sfx.minValue = 0.01f;
        view.sfx.maxValue = 2;
        view.sfx.value = model.sfx;
        view.sfx.onValueChanged.AddListener(OnSetSFXVolume);

        //Camera Sensitivity
        view.cameraSensitivity.wholeNumbers = false;
        view.cameraSensitivity.minValue = .25f;
        view.cameraSensitivity.maxValue = 1.75f;
        view.cameraSensitivity.value = model.cameraSensitivity;
        view.cameraSensitivity.onValueChanged.AddListener(OnSetCamSens);
        publisher.Publish(PlayerTopics.camSentvtTopic, new CamSensitivityMessage(model.cameraSensitivity));
    }

    private void OnSetQuality(float value)
    {
        int intVal = (int)value;
        model.resolution = intVal;
        QualitySettings.SetQualityLevel(intVal);

        SaveSettings();
    }

    private void OnSetMusicVolume(float value)
    {
        model.music = value;
        publisher.Publish(musicVolumeTopic, new MusicVolumeMessage(model.music));

        SaveSettings();
    }

    private void OnSetSFXVolume(float value)
    {
        model.sfx = value;
        publisher.Publish(sfxVolumeTopic, new SFXVolumeMessage(model.sfx));

        SaveSettings();
    }

    private void OnSetCamSens(float value)
    {
        model.cameraSensitivity = value;
        publisher.Publish(PlayerTopics.camSentvtTopic, new CamSensitivityMessage(model.cameraSensitivity));

        SaveSettings();
    }

    private void SaveSettings()
    {
        if (shouldSave == false)
        {
            SaveSettingsAsync().Forget();
        }
        shouldSave = true;
    }

    private async UniTaskVoid SaveSettingsAsync()
    {
        await UniTask.Delay(1000, false, PlayerLoopTiming.Update, tokenSource.Token);

        if(shouldSave)
        {
            LocalSaveLoad.Save(model, fileName);
            shouldSave = false;
        }
    }

    public override void OnDestroy()
    {
        tokenSource.Cancel();
        tokenSource.Dispose();
    }
}
