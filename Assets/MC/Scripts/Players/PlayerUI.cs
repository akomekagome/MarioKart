using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class PlayerUI : MonoBehaviour
{
    private PlayerId _playerId;

    public PlayerId PlayerId => _playerId;

    private AsyncSubject<Unit> _initializAsyncSubject = new AsyncSubject<Unit>();
    public IObservable<Unit> Initialized => _initializAsyncSubject;


    public void SetPlayerId(PlayerId playerId)
    {
        _playerId = playerId;
        _initializAsyncSubject.OnNext(Unit.Default);
        _initializAsyncSubject.OnCompleted();
    }
}
