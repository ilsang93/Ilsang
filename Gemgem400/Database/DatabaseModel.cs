using Newtonsoft.Json;

using System;
using System.Collections.Generic;

namespace GemgemAr
{
    // 기본값들 모음
    public static class DEFAULT
    {
        public const string MOTION_@@@동작1@@@ = "@@@동작1@@@";
        public const string MOTION_@@@동작2@@@ = "@@@동작2@@@";
        public const string MOTION_@@@동작3@@@ = "@@@동작3@@@";

        // 아무동작이 없는 사용자의 경우 기본값처리
        public static List<string> MOTIONS = new List<string> { MOTION_C2, MOTION_H2, MOTION_F3 };
    }

    // 로그인을 해주는 사용자정보 (주로 보호자)
    // TMI, 보통은 사용자 = 플레이어이지만, 알밤은 아동 재활치료라 사용자 != 플레이어, 사용자가 구매및로그인을 해주지만 플레이는 자녀가 하게됨
    public class UserModelFromPrisma
    {
        public string id;
        public string phoneNumber;
        public string name;
        public string email;
        public List<ChildModelFromPrisma> children;

        [JsonIgnore]
        public static readonly UserModelFromPrisma TestUser = new UserModelFromPrisma
        {
            id = "@@@UUID@@@",
            phoneNumber = "@@@테스트 전화 번호@@@",   // 테스트용도
            name = "@@@테스트 이름@@@",
            email = "@@@테스트 메일@@@"
        };
    }

    // 환자정보 (주로 보호자의 자녀)
    [Serializable]
    public class ChildModelFromPrisma
    {
        public string Id;
        public string Name;
        [JsonConverter(typeof(BoolConverter))]
        public bool LeftHand;
        [JsonConverter(typeof(BoolConverter))]
        public bool RightHand;
        public List<string> Gestures;

        public int Stage = 1;
        public int level = 1;
        public int exp = 55;
        public bool privacyAccepted = false;

        [JsonIgnore]
        public static readonly ChildModelFromPrisma LeftHandUser = new ChildModelFromPrisma()
        {
            Id = "00000000-0000-0000-0000-000000000000",
            Stage = 1,
            level = 1,
            Name = "왼손체험",
            LeftHand = true,
            RightHand = false,
            exp = 55,
            Gestures = DEFAULT.MOTIONS,
        };

        [JsonIgnore]
        public static readonly ChildModelFromPrisma RightHandUser = new ChildModelFromPrisma()
        {
            Id = "00000000-0000-0000-0000-000000000001",
            Stage = 1,
            level = 1,
            Name = "오른손체험",
            LeftHand = false,
            RightHand = true,
            Gestures = DEFAULT.MOTIONS,
        };
    }
}