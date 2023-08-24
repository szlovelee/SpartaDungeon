# SpartaDungeon

*내일 배움 캠프 개인 과제*

https://szloveleesz.tistory.com/25 (약간의 부가 설명)


(8월 23일)
- 선택요구사항 1 ~ 9 완료
- 코드 수정 예정


(8월 24일)
- 아이템 구매 및 판매 문제 해결
- 주석 추가

# 메서드 및 구성 요소 설명

## Program

### ⭐️ 기본 세팅
- **restingTimer**는 '휴식하기'에서 쓰이는 변수
- **GameState**는 휴식 모드와 플레이모드를 전환하는 데 쓰이는 데이터 타입 / **status** 변수에 값이 할당되어 게임 상태를 체크한다.
- **InventoryMode**는 아이템 정렬 방식을 정하는 데 쓰이는 데이터 타입 / **organize** 변수에 할당되어 인벤토리 정렬 모드를 체크한다.
- **GameDataSetting()**
- **DisplayGameIntro()**: 입력값에 따라 상태보기, 인벤토리, 상점, 휴식, 던전 관련 기능을 하는 메서드를 호출. 휴식과 던전 입장의 경우는 조건에 따라 선택이 불가능 할 수 있다.


### 🫀 상태보기 관련 기능

- **DisplayMyInfo()**: 플레이어의 기본적인 속성을 보여주는 메서드. 


### 💼 인벤토리 관련 기능
- **DisplayInventory()**: 인벤토리의 기본 화면을 보여주는 역할을 하는 메서드. 플레이어가 가지고 있는 아이템 (player.PossessedItems)과 관련된 작업을 한다. 입력값에 따라 추가적인 작업을 위한 메서드 호출이 가능하다.
- **EquipmentManager()**: 추가적인 기능 중 '장착 관리'를 담당하는 메서드
- **Organize()**: 추가적인 기능 중 '아이템 정렬 설정'을 담당하는 메서드. 아이템 정렬 모드를 변경하고 SortItems()를 호출한다.
- **SortItems()**: 실제로 아이템을 정럴하는 메서드. Organize() 뿐만 아니라 아이템 구매와 아이템 장착 등의 상황에서도 호출되어 인벤토리 정렬모드(organize)에 따라 다른 정렬이 실행된다.
- **ItemUpgrade()**: 추가적인 기능 중 **'아이템 강화'** 를 담당하는 메서드. 아이템 중 강화 가능한 (구매한 아이템 중 무기와 방어구 / 레벨 5 미만) 아이템을 정렬하여 입력값에 따라 해당 아이템을 강화하도록 한다. 아이템 강화에 따라 아이템의 효과가 변경된다. 레벨에 따라 강화 금액이 달라진다. (아이템 원가의 0.1배, 0.2배, 0.5배, 1배)
- **EffectManager()**: 아이템이 가지고 있는 효과 지수를 플레이어의 공격력 혹은 방어력에 적용시키는 작업을 하는 메서드. 플레이어가 아이템을 착용 혹은 해제, 소모, 강화할 때 해당 내역이 플레이어의 상태에 반영되도록 한다.


### 🏠 상점 관련 기능
- **DisplayStore()**: 상점의 기본 화면을 보여주는 역할을 하는 메서드. 상점에 있는 아이템 목록을 보여주고, 입력에 따라 아이템 구매 및 판매 기능을 호출한다.
- **ItemBuy()**: 아이템 구매 관련 메서드. 플레이어의 아이템 구매 상태, 재화를 고려하여 아이템 구매가 이루어지도록 한다. 
- **ItemSell()**: 플레이어의 인벤토리에서 기본 아이템(item.Price == 0)을 제외한 목록에서 아이템을 판매할 수 있도록 한다. 강화 가능한 아이템의 경우, **강화 정도에 따라 다른 금액이 측정**되어 판매 보상(Gold)를 얻는다.


### 💤 휴식 관련 기능
- **Resting()**: 휴식하기 기본 화면을 보여주는 메서드. 게임 모드 (휴식 모드 or 플레이 중)를 보여주고, 게임 모드를 전환할 수 있도록 한다. 휴식모드에서는 **30초에 한 번씩 현재 체력이 +1** 되고, **던전에 입장할 수 없다**. 현재 게임 모드에 따라 플레이어의 선택지가 달라진다. (플레이 모드일 경우 휴식하기로, 휴식 모드일 경우 휴식 중단으로) 선택에 따라 아래 두 메서드가 호출된다.
- **StartRestingMode()**: restingTimer 변수에 실제 타이머 객체를 할당하여 시간 체크 및 이벤트 호출, 체력 회복 메서드 실행을 할 수 있도록 한다.
- **OnTimerEvent()**: 타이머에 의해 설정된 주기마다 호출된다. 이 메서드를 통해 플레이어의 체력이 회복되고, 체력이 최대치가 되면 자동으로 멈추게(QuitRestingMode() 호출) 설정되어 있다.
- **QuitRestingMode()**: 타이머를 멈추고, 게임 모드를 변경한다.


### ⚔️ 던전 관련 기능
- **DisplayDungeon()**: 던전 입장 기본 화면을 보여주는 메서드. 현재 플레이어의 총 방어력과 스테이지 난이도 및 권장 방어력을 보여준다.
- **EnteringDungeon()**: 플레이어의 스테이지 선택에 따라 스테이지 정보를 한 번 더 보여주고, **플레이어의 방어력이 권장 방어력보다 낮을 경우 "현재 방어력이 낮습니다"라는 문구**가 함께 출력된다. 플레이어가 입장을 선택했을 경우, "전투 중..."이라는 문구를 5초 노출시키고, FightEnd()메서드를 호출하여 던전 탐험 결과로 넘어간다.
- **FightEnd()**: 던전 탐험 결과를 출력한다. 던전 탐험 성공 여부는 dungeon 객체를 통해 DungeonFight()메서드를 통해 결정된다. DungeonFight가 반환하는 값과 보상 설정이 반영되어 보상과 레벨업 여부 등이 출력된다. 던전 입장을 할 경우 체력 소모가 불가피하므로, 탐험 후의 체력도 함께 출력되도록 했다.
- **LevelControl()**: 던전 탐험의 보상으로 경험치를 얻게 되는데, 플레이어의 현재 경험치가 최대 경험치를 넘었는지 판별한 후, 넘었을 경우 레벨이 오르도록 하는 메서드이다.


### ✨ 기타 기능
- **WritingItem()** : 인벤토리와 상점 등에서 아이템을 목록으로 보여줄 때 쓰인다. 기본적으로 아이템의 이름, 종류, 효과, 그리고 설명을 담고 있는데, 쓰일 때 뒤에 추가적인 내용들이 붙는다.
- CheckVailidInput()
- **AnswerClear()** : 입력되어 콘솔에 남은 내용을 지워야 하는 경우가 많아 추가한 메서드.



## Character

### 필드 및 프로퍼티
- **PossessedItems**
- **Atk / AddAtk**
- **Def / AddDef**
- **Hp / CurrentHp**
- **MaxExp / CurrentExp**
- **Level**
- Name, Job, Gold

### 생성자



## Item

### Enum과 프로퍼티
- **enum ItemType**
- **Level**
- **Price / SellingPrice**
- **Equipped**
- **Count**
- **Effect**
- **EffectType**
- Name / Desc / Equipped

  
### 생성자



## Store

### Item / StoreItems
- Item 객체 생성
- **StoreItems**



## Dungeon

### 프로퍼티
- RecDef
- Reward
- **AdditionalReward**
- DungeonInfo
- RewardExp

### 생성자
- switch문 활용

### 메서드
- **DungeonFight()**
- **DungeonResult()**



