using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BackEnd;
using System.Text;

public class UserData {
    public int level = 1;
    public float atk = 3.5f;
    public string info = string.Empty;
    public Dictionary<string, int> inventory = new Dictionary<string, int>();
    public List<string> equipment = new List<string>();

    public override string ToString() {
        StringBuilder result = new StringBuilder();
        result.AppendLine($"level : {level}");
        result.AppendLine($"atk : {atk}");
        result.AppendLine($"info : {info}");
        result.AppendLine($"inventory");

        foreach (var itemKey in inventory.Keys) {
            result.AppendLine($"| {itemKey} : {inventory[itemKey]}개");
        }

        result.AppendLine($"equipment");
        foreach (var equip in equipment) {
            result.AppendLine($"| {equip}");
        }

        return result.ToString();
    }
}

public class BackendGameData {
    private static BackendGameData _instance = null;

    public static BackendGameData Instance {
        get {
            if (_instance == null) {
                _instance = new BackendGameData();
            }

            return _instance;
        }
    }

    public static UserData userData;

    private string gameDataRowInDate = string.Empty;

    public void GameDataInsert() {
        
        if(userData == null) {
            userData = new UserData();
        }

        //userData 객체 생성 후 값 삽입
        Debug.Log("데이터를 초기화합니다.");
        userData.level = 1;
        userData.atk = 3.5f;
        userData.info = "친추는 언제나 환영입니다.";

        userData.equipment.Add("전사의 투구");
        userData.equipment.Add("강철 갑옷");
        userData.equipment.Add("헤르메스의 군화");

        userData.inventory.Add("빨간포션", 1);
        userData.inventory.Add("하얀포션", 1);
        userData.inventory.Add("파란포션", 1);

        Debug.Log("뒤끝 업데이트 목록에 해당 데이터들을 추가합니다.");

        //userData를 param으로 만든 후 
        Param param = new Param();
        param.Add("level", userData.level);
        param.Add("atk", userData.atk);
        param.Add("info", userData.info);
        param.Add("equipment", userData.equipment);
        param.Add("inventory", userData.inventory);

        //서버로 삽입
        Debug.Log("게임정보 데이터 삽입을 요청합니다.");

        var bro = Backend.GameData.Insert("USER_DATA", param);

        if(bro.IsSuccess()) {
            Debug.Log("게임정보 데이터 삽입에 성공했습니다. : " + bro);

            //삽입한 게임정보의 고유값  
            gameDataRowInDate = bro.GetInDate();
        }
        else {
            Debug.LogError("게임정보 데이터 삽입에 실패했습니다. : " + bro);
        }

    }

    public void GameDataGet() {

    }

    public void LevelUp() {

    }

    public void GameDataUpdate() {

    }

}
