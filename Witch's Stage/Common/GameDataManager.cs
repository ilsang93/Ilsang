using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager instance;
    public UserData UserData { get => userData; }
    private UserData userData;

    [SerializeField] private UserData dummyUserData = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        GetUserData();
    }

    public void GetUserData()
    {
        //TODO 서버로부터 유저 데이터를 받아온다.
        //TODO Steam API를 이용해 유저 ID 취득 후 key 값으로 이용해 파이어 베이스 등을 이용해 유저 데이터를 취득한다.

        // 더미 데이터를 입력한다.
        userData = dummyUserData;
    }
}
