using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using GemgemAr;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace GemgemAr
{
    /// <summary>
    /// 캐릭터 아이템을 관리하는 클래스
    /// </summary>
    public class CharacterCardManager : MonoBehaviour
    {
        private static CharacterData[] charaDataList;
        [SerializeField]
        private Button charCardPrefab;
        [SerializeField]
        private Toggle havingToggle;
        public Dictionary<string, int> itemCntMap;
        private readonly List<CharacterCardController> charaCardList = new List<CharacterCardController>();


        public async void ReloadCollectionCanv()
        {
            // 카드 리스트와 Instantiate한 자식 게임오브젝트 삭제 초기화
            charaCardList.Clear();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            await EditItemsCanvas();

            charaDataList = Resources.LoadAll<CharacterData>("@@@캐릭터 데이터 스크립터블 오브젝트 경로@@@")
                .ToArray();

            Debug.Log("Loaded " + charaDataList.Length + " ScriptableObjects");

            List<ItemModel> items = FindObjectOfType<MainMenuManager>().items;
            foreach (CharacterData scriptableObject in charaDataList)
            {
                // 서버 데이터로 스크립터블 오브젝트 데이터를 최신화 환다.
                try
                {
                    ItemModel item = items.Find(x => x.Id.Equals(scriptableObject.Id));
                    if (item != null)
                    {
                        scriptableObject.FlavorText = item.Description;
                        scriptableObject.FlavorTextInGacha = item.Description;
                        scriptableObject.Name = item.Name;
                    }
                    else
                    {
                        Debug.Log("서버 데이터로 스크립터블 오브젝트 데이터를 최신화 환다. 실패");
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("서버 데이터로 스크립터블 오브젝트 데이터를 최신화 환다. 실패");
                    Debug.Log(e);
                }

                Debug.Log("Loaded ScriptableObject: " + scriptableObject.name);
                Button charCard = Instantiate(charCardPrefab);

                charCard.GetComponent<CharacterCardController>().Init(scriptableObject);

                Debug.Log("카드 소지 " + scriptableObject.Id + " : " + itemCntMap.ContainsKey(scriptableObject.Id));
                if (itemCntMap.ContainsKey(scriptableObject.Id))
                {
                    charCard.GetComponent<CharacterCardController>().CardCount = itemCntMap[scriptableObject.Id];
                }
                else
                {
                    charCard.GetComponent<CharacterCardController>().CardCount = 0;
                }
                charCard.transform.SetParent(transform, false);

                charCard.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UISFXManager.Instance.PlayCommonSfx(CommonSfx.BUTTONTAB_ui_button_simple_click_03);
                    FindObjectOfType<CardDetailController>().ShowDetail(charCard.GetComponent<CharacterCardController>().CharacterData, charCard.GetComponent<CharacterCardController>().CardCount);
                });
                charaCardList.Add(charCard.GetComponent<CharacterCardController>());
            }

            havingToggle.onValueChanged.AddListener((bool val) =>
            {
                if (val)
                {
                    foreach (CharacterCardController card in charaCardList)
                    {
                        if (card.CardCount <= 0)
                        {
                            card.gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    foreach (CharacterCardController card in charaCardList)
                    {
                        card.gameObject.SetActive(true);
                    }
                }
            });
        }

        private async Task EditItemsCanvas()
        {
            itemCntMap = await GatchaService.SelectItemsMap();
            FindObjectOfType<DinoShapeManager>().DinoShapeStart(itemCntMap);
        }

        public static void SetGatchaCard(string characterId, GameObject charCard)
        {
            foreach (CharacterData scriptableObject in charaDataList)
            {
                if (scriptableObject.Id.Equals(characterId))
                {
                    charCard.GetComponent<CharacterCardController>().Init(scriptableObject);
                    break;
                }
            }
        }
    }

}