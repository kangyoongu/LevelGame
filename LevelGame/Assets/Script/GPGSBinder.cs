using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Events;
using GooglePlayGames.BasicApi.SavedGame;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class GPGSBinder : MonoBehaviour
{
    static GPGSBinder inst = new GPGSBinder();
    public static GPGSBinder Inst => inst;

    

    ISavedGameClient SavedGame =>
        PlayGamesPlatform.Instance.SavedGame;

    IEventsClient Events =>
        PlayGamesPlatform.Instance.Events;


    /*
    void Init()
    {
        PlayGamesPlatform.Activate();
    }


    public void Login()
    {
        Init();
        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            Debug.Log(success);
        });
    }*/

    public void ShowAchievementUI() =>
        Social.ShowAchievementsUI();

    public void UnlockAchievement(string gpgsId, Action<bool> onUnlocked = null) =>
        Social.ReportProgress(gpgsId, 100, success => onUnlocked?.Invoke(success));

    public void IncrementAchievement(string gpgsId, int steps, Action<bool> onUnlocked = null) =>
        PlayGamesPlatform.Instance.IncrementAchievement(gpgsId, steps, success => onUnlocked?.Invoke(success));


    public void ShowAllLeaderboardUI() =>
        Social.ShowLeaderboardUI();

    public void ShowTargetLeaderboardUI(string gpgsId) =>
        ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(gpgsId);

    public void ReportLeaderboard(string gpgsId, long score, Action<bool> onReported = null) =>
        Social.ReportScore(score, gpgsId, success => onReported?.Invoke(success));

    public void LoadAllLeaderboardArray(string gpgsId, Action<UnityEngine.SocialPlatforms.IScore[]> onloaded = null) =>
        Social.LoadScores(gpgsId, onloaded);

    public void LoadCustomLeaderboardArray(string gpgsId, int rowCount, LeaderboardStart leaderboardStart,
        LeaderboardTimeSpan leaderboardTimeSpan, Action<bool, LeaderboardScoreData> onloaded = null)
    {
        PlayGamesPlatform.Instance.LoadScores(gpgsId, leaderboardStart, rowCount, LeaderboardCollection.Public, leaderboardTimeSpan, data =>
        {
            onloaded?.Invoke(data.Status == ResponseStatus.Success, data);
        });
    }


    public void IncrementEvent(string gpgsId, uint steps)
    {
        Events.IncrementEvent(gpgsId, steps);
    }

    public void LoadEvent(string gpgsId, Action<bool, IEvent> onEventLoaded = null)
    {
        Events.FetchEvent(DataSource.ReadCacheOrNetwork, gpgsId, (status, iEvent) =>
        {
            onEventLoaded?.Invoke(status == ResponseStatus.Success, iEvent);
        });
    }

    public void LoadAllEvent(Action<bool, List<IEvent>> onEventsLoaded = null)
    {
        Events.FetchAllEvents(DataSource.ReadCacheOrNetwork, (status, events) =>
        {
            onEventsLoaded?.Invoke(status == ResponseStatus.Success, events);
        });
    }
    public string GooglePlayToken;
    public string GooglePlayError;

    public async Task Authenticate()
    {
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
        await UnityServices.InitializeAsync();

        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google was successful.");
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, async code =>
                {
                    Debug.Log("Google Login Login with Google was successful.");
                    GUIUtility.systemCopyBuffer = code;
                    Debug.Log($"Auth code is {code}");
                    GooglePlayToken = code;

                    Debug.Log("Google Login 2" + " " + Social.localUser.id + " / " + Social.localUser.userName);

                    await AuthenticateWithUnity();
                });
            }
            else
            {
                GooglePlayError = "Failed to retrieve GPG auth code";
                Debug.Log("Google Login" + "Failed to retrieve GPG auth code");
                Debug.LogError("Login Unsuccessful");
            }
        });
    }

    private async Task AuthenticateWithUnity()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGoogleAsync(GooglePlayToken);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            throw;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            throw;
        }

    }

    private void Start()
    {
        _ = Authenticate();
    }
}