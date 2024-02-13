# Hi5ive
 ✨✨ 오픈소스프로젝트 ✨✨ <br>
 유니티로 구현하는 VR 멀티 플레이어 경쟁 달리기 게임 <br>
 멀티 플레이 시스템을 통해 여러 플레이어들과 함께 목표 지점을 향해 경쟁할 수 있다. <br>
 아이템과 에너지 시스템과 같은 재미 요소를 통합하여, 매 플레이마다 새로운 즐거움을 발견할 수 있다
 <br>Unity Netcode for Gameobject, thebackend.io 등 사용 중 <br>
 Unity: 2021.3.8f1

## 1. 프로젝트 개요
### 1) 개발 배경
### 2) 필요성

## 2. 프로젝트 개발 내용
### 1) 최종 목표
### 2) 개발 내용
1. 전체 시스템 구성도
2. 주요 기능별 씬 흐름도
   ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/692fcb89-bb4f-4004-b344-191c05fd1bde)
3. 함수/클래스 다이어그램
4. 주요 스크립트 설명
  - PlayerCollision.cs
     
     플레이어와 장애물과의 충돌 처리, 에너지 시스템 관련 스크립트이다. <br>
     장애물 충돌처리는 서버에서 관리하도록 하였다.<br>
     OnCollisionEnter() 함수를 사용하여서 장애물과 충돌이 일어난 경우 에너지를 증가시키고 잠깐동안 기절을 한다.<br>
     에너지를 최대치까지 채우게 되면 일정시간동안 RunEffect 파티클을 Play하고 플레이어의 속도를 증가시키며 장애물과의 충돌처리를 무시하도록 한다.
    
  - PlayerController.cs

     플레이어의 이동과 점프를 관리하는 스크립트이다.
     <br>Update함수에서 플레이어 이동키 입력을 입력받고 입력이 들어오면 [ClientRpc]와 [ServerRpc]를 이용하여서 다른플레이어와 본인의 이동을 동기화 시킨다.
     <br>Update함수에서 플레이어 점프또한 입력을 받고 똑같이 동기화를 시킨다.
     <br>플레이어 이동은 각자의 플레이어가 가진 Speed값에 따라 이동속도가 달라진다.

  - EndPoint.cs

    목적지에 플레이어가 충돌하면 Rank를 1증가시키고 [ClientRpc] , [ServerRpc]를 통해 증가한 Rank값을 동기화시킨다.
    <br>Rank를 UI로 출력한다.
    <br>collision.gameObjcect.GetComponent<PlayerEndPoint>().UpdateRank(rank)를 통해 충돌한 플레이어에 rank값을 넘겨준다.

  - TimeManager.cs

    출발 카운트다운과 도착 카운트다운을 관리하는 스크립트이다.
    <br>[ServerRpc]와 [ClientRpc]를 사용하여 시간 동기화를 하였다.
    <br>시작 카운트다운은 호스트가 들어오거나 새로운 플레이어가 들어오면 시간을 갱신하도록 하였다.
     도착 카운트다운의 경우 최초 1등 플레이어가 도착하면 카운트다운을 시작한다.

  - SpeedUpHandler.cs

    속도 증가 발판에 대한 스크립트이다.
    <br>OnCollisionEnter()를 사용하여서 플레이어가 해당 발판을 밟았을 때 속도를 증가시키도록 만들었다. [Rpc]를 사용하여 플레이어 속도를 동기화 시켰다.

5. 장르 특성화 방안

### 3) 오픈소스 활용
#### 1. 활용한 오픈 소스 소개
   멀티플레이 기능을 구현하기 위해 유니티 Netcode for Gameobect를, 로그인 기능을 구현하기 위해 thebackend.io의 뒤끝 sdk를 활용하였다.
   
#### 2. 오픈 소스 사용법 및 활요한 기능
  - Unity Netcode for Gameobject
    1. 월드의 배경이 아닌 오브젝트, 즉, 플레이어와 상호작용하는 오브젝트들은 모두 동기화 되어야 한다. 따라서, 동기화가 필요한 모든 오브젝트들은 NetworkObject 스크립트가 필요하며, 위치가 변경되거나 물리작용을 수행하기 위해서는 NetworkRigidbody 스크립트와 ClientNetworkTransform 스크립트가 필요하다. <br>
   ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/b6d372af-80c7-4f34-9852-76a95e758bd8) <br>
   만약, 월드에서 동적으로 생성하기 위해서는 NetworkPrefabList에 선언이 되어있어야 한다. <br>
   ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/1ed491bc-ddf3-41b6-b5c1-0769490cc134)  <br> <br>
     2. RPC 함수 선언 시, 상단에 [ClientRpc] 또는 [ServerRpc]를 선언하고, 함수 이름의 마지막에는 ClientRpc 또는 ServerRpc가 포함되어야 한다. <br>
     ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/610fe0ad-2a9d-4b76-a8cd-8a92e7b0c9ec) <br>
     ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/14f53fba-74fb-4b6a-8c9f-28d3a04545ac) <br>
     위와 같이 중요한 변수 등은 isServer를 통해 서버에서 관리하며 변경된 변수는 ClientRpc를 통해 클라이언트들과 동기화 한다. 

    
  - the backend 뒤끝sdk
    1.	뒤끝 SDK 유니티 적용
    2.	뒤끝 콘솔에서 프로젝트 인증 키 발급
    3.	유니티 뒤끝 인스펙터에 인증값 적용
    4.	스크립트 작성 및 적용

## 4. 사용 설명서
  1. 회원가입 하기 <br>
     ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/9ff0676f-4ab2-415c-bde6-04b48e032357)
    게임 실행 이후, 회원가입하기 버튼을 클릭한다.
아이디, 비밀번호, 닉네임을 입력하고 회원가입을 클릭한다.
회원가입 이후, 로그인 화면으로 가기 버튼을 클릭한다. 

  2. 로그인 하기 <br>
     ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/24f70bae-b2ec-42e5-8260-6c595745a107)
     회원가입한 아이디와 비밀번호를 입력하고 로그인을 클릭한다.

  3. 호스트/클라이언트 선택
    ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/f01d2304-2ade-42f1-b0f0-8de325b38a11) <br>
    방장의 IP를 입력하고 접속을 누른다.

- 서버 생성
        방장으로 게임을 실행하기 위해 같이 플레이할 플레이어들에게 검색된 IP를 알려
준다. 같이 플레이 하기 위해서 모든 플레이어는 같은 와이파이로 접속해야 한다.
“호스트로 시작”을 누르면 게임이 시작된다.

- 로컬 서버 접속
        방장의 IP를 입력하고 접속을 누른다.

### 게임 플레이
 - 시작
   
카운트다운이 끝나면 캐릭터를 조작할 수 있다.
이동은 WASD로 각각 앞, 좌, 뒤, 우로 움직이며 스페이스바를 통해 점프할 수 있다. 
스페이스바를 연속해서 두 번 누르면 2단으로 점프한다.
   
 - 랜덤 아이템
   
     ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/069802b9-a84b-441e-82cb-55b0efb8e66e) <br>
    랜덤 아이템과 충돌하면 랜덤으로 속도가 빨라지거나 느려진다.

 - 스피드업 발판

   화살표 모양의 스피드업 발판을 밟으면 발판의 종류에 따라 속도가 일정량 증가한다.
일반 발판과 특수 발판으로 구성 되어있다. (일반 발판 속도 9, 특수 발판 속도 12)
속도가 증가된 상태에서 스피드업 발판을 밟으면 속도가 갱신된다.
현재 속도보다 느린 스피드업 발판을 밟으면 갱신이 되지 않는다.


      - 일반 발판

      ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/d60a8d6c-eef7-4710-9788-ff9e3b14d864)

      - 특수 발판

        ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/9ae3b9d5-8dd1-4241-9ad3-3d6fc1a44e89)

 
 - 방해물

   ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/a9f2c99b-f682-4b71-9347-b85f749cf086) <br>
   ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/b2d60023-f032-4c75-8c38-47861c6bfa14) <br>
   방해물과 충돌하면 일정시간 기절하며 에너지를 20 획득한다.
충돌에는 쿨다운이 있으므로 연속해서 충돌하지 않는다.


 - 에너지 시스템

   ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/7f1b3c69-c81c-467c-b21c-c0990c32e0d6) <br>
   화면 좌측 상단에 누적된 에너지를 확인할 수 있다. 에너지는 방해물과 충돌 시 증가하며 각 방해물 마다 증가되는 양은 다르다. 에너지를 모두 모으게 되면 자동으로 Ultimate 모드로 진입한다.
Ultimate 모드에서는 속도가 상당량 증가하며 모든 충돌을 무시하고 달린다. 이 상태는 5초간 지속되며 이후에는 에너지가 모두 소진되어 0으로 돌아간다.


 - 도착 지점

   ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/a5c3ad51-ea58-4f6b-9d46-28250ae3caf3)

 - 엔딩

   ![image](https://github.com/k1sihyeon/Hi5ive/assets/96001080/3612776b-d0c7-4422-9a0f-18f4f78c2bce)

## 5. 고찰
### 1) 문제 및 해결 방안
 - 문제 1. Unity Netcode <br>
   Netcode for Gameobjects는 2022년 말에 출시되어 참고할 수 있는 예제, 코드가 절대적으로 부족하였으며, 동기화 과정인 RPC 에서도 어려움을 겪었다.<br>
   => 공식 문서와 넷코드의 이전 버전인 MLAPI를 참고함으로써 문제를 해결해 나갔다.<br>
   
 - 문제 2. EndPoint <br>
    등수를 계산하는 EndPointManger.cs 에서 목적지에 충돌하는 순서대로 순위를 처리하고자 하였지만 충돌 처리가 2번씩 반복되었다. <br>
⇒ coolDowm 플래그 변수와 코루틴을 통해 충돌 처리에 간격을 두어 해결함<br>

 - 문제 3. PlayerSkinController<br>
   플레이어가 캐릭터의 색깔을 선택하고 이를 게임에 적용해 동기화하고자 하였지만 RPC 동기화의 문제로 인해 오류가 발생했다.<br>
⇒ 각 플레이어의 게임 씬에서 다른 플레이어들의 색깔을 임의로 설정하여 다른 플레이어들을 구분할 수 있도록 하였다.

   
### 2) 한계점
 - 현재 개발한 프로젝트의 한계점
     - 멀티 플레이 게임으로 제작했으나 같은 라우터 내 내부 네트워크로만 작동이 가능하다. 따라서 향후 Unity Relay, Unity Lobby등 개발이 필요하다.
     - 플레이 가능한 스테이지가 1개로 다양한 스테이지 개발이 필요하다
     - 로그인 기능이 구현되어 있지만 유저를 관리하는 기능이 없다
     - 속도 증가, 충돌 이외에 플레이어간의 다양한 상호작용이 필요하다
     - 빌드 수행 시, 마지막으로 편집한 씬을 제외한 다른 씬의 쉐이더가 사라진다.

 - 개발 계획 대비 변경점 등<br>
   중간 점검이후의 계획에서 추가 스테이지와 Unity Lobby/Relay 등은 개발하지 못하였지만, 게임의 디테일을 높일 수 있도록 사운드, 애니메이션, 카메라 효과, 엔딩 씬 등을 개발하게 되었다.

## 협업 규칙
- [INITIAL] : repository를 생성하고 최초에 파일을 업로드 할 때
- [ADD] : 신규 파일 추가
- [UPDATE] : 코드 변경이 일어날때
- [REFACTOR] : 코드를 리팩토링 했을때
- [FIX] : 잘못된 링크 정보 변경, 필요한 모듈 추가 및 삭제
- [REMOVE] : 파일 제거
- [STYLE] : 디자인 관련 변경사항

## Naming Convention
- 파일명 규칙
  - 파일명은 기본적으로 스네이크케이스로 작성한다 : snake_case
- 변수명 규칙
  - 변수명은 기본적으로 카멜케이스로 작성한다 : camelCase
- 클래스명 규칙
  - 클래스명은 기본적으로 파스칼케이스로 작성한다 : PascalCase
