using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class PlayerCanvas : MonoBehaviour
{
    private PlayerId _playerId;

    public PlayerId PlayerId => _playerId;

    private AsyncSubject<Unit> _initializAsyncSubject = new AsyncSubject<Unit>();
    public IObservable<Unit> Initialized => _initializAsyncSubject;


    private void SetPlayerId(PlayerId playerId)
    {
        _playerId = playerId;
        _initializAsyncSubject.OnNext(Unit.Default);
        _initializAsyncSubject.OnCompleted();
    }
}
