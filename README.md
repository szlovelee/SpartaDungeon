# SpartaDungeon

## 구조 정리

## Program

### ⭐️ 기본 세팅
- restingTimer는 '휴식하기'에서 쓰이는 변수
- GameState는 휴식 모드와 플레이모드를 전환하는 데 쓰이는 데이터 타입 / status 변수에 값이 할당되어 게임 상태를 체크한다.
- InventoryMode는 아이템 정렬 방식을 정하는 데 쓰이는 데이터 타입 / organize 변수에 할당되어 인벤토리 정렬 모드를 체크한다.
- GameDataSetting()
- DisplayGameIntro(): 입력값에 따라 상태보기, 인벤토리, 상점, 휴식, 던전 관련 기능을 하는 메서드를 호출. 휴식과 던전 입장의 경우는 조건에 따라 선택이 불가능 할 수 있다.


### 🫀 상태보기 관련 기능

- DisplayMyInfo(): 플레이어의 기본적인 속성을 보여주는 메서드. 


### 💼 인벤토리 관련 기능
- DisplayInventory(): 인벤토리의 기본 화면을 보여주는 역할을 하는 메서드. 플레이어가 가지고 있는 아이템 (player.PossessedItems)과 관련된 작업을 한다. 입력값에 따라 추가적인 작업을 위한 메서드 호출이 가능하다.
- EquipmentManager(): 추가적인 기능 중 '장착 관리'를 담당하는 메서드
- Organize(): 추가적인 기능 중 '아이템 정렬 설정'을 담당하는 메서드. 아이템 정렬 모드를 변경하고 SortItems()를 호출한다.
- SortItems(): 실제로 아이템을 정럴하는 메서드. Organize() 뿐만 아니라 아이템 구매와 아이템 장착 등의 상황에서도 호출되어 인벤토리 정렬모드(organize)에 따라 다른 정렬이 실행된다.
- ItemUpgrade(): 추가적인 기능 중 **'아이템 강화'** 를 담당하는 메서드. 아이템 중 강화 가능한 (구매한 아이템 중 무기와 방어구 / 레벨 5 미만) 아이템을 정렬하여 입력값에 따라 해당 아이템을 강화하도록 한다. 아이템 강화에 따라 아이템의 효과가 변경된다. 레벨에 따라 강화 금액이 달라진다. (아이템 원가의 0.1배, 0.2배, 0.5배, 1배)
- EffectManager(): 아이템이 가지고 있는 효과 지수를 플레이어의 공격력 혹은 방어력에 적용시키는 작업을 하는 메서드. 플레이어가 아이템을 착용 혹은 해제, 소모, 강화할 때 해당 내역이 플레이어의 상태에 반영되도록 한다.


### 🏠 상점 관련 기능
- DisplayStore(): 상점의 기본 화면을 보여주는 역할을 하는 메서드. 상점에 있는 아이템 목록을 보여주고, 입력에 따라 아이템 구매 및 판매 기능을 호출한다.
- ItemBuy(): 아이템 구매 관련 메서드. 플레이어의 아이템 구매 상태, 재화를 고려하여 아이템 구매가 이루어지도록 한다. 
- ItemSell(): 플레이어의 인벤토리에서 기본 아이템(item.Price == 0)을 제외한 목록에서 아이템을 판매할 수 있도록 한다. 강화 가능한 아이템의 경우, **강화 정도에 따라 다른 금액이 측정**되어 판매 보상(Gold)를 얻는다.


### 💤 휴식 관련 기능
- Resting(): 휴식하기 기본 화면을 보여주는 메서드. 게임 모드 (휴식 모드 or 플레이 중)를 보여주고, 게임 모드를 전환할 수 있도록 한다. 휴식모드에서는 **30초에 한 번씩 현재 체력이 +1** 되고, **던전에 입장할 수 없다**. 현재 게임 모드에 따라 플레이어의 선택지가 달라진다. (플레이 모드일 경우 휴식하기로, 휴식 모드일 경우 휴식 중단으로) 선택에 따라 아래 두 메서드가 호출된다.
- StartRestingMode(): restingTimer 변수에 실제 타이머 객체를 할당하여 시간 체크 및 이벤트 호출, 체력 회복 메서드 실행을 할 수 있도록 한다.
- OnTimerEvent(): 타이머에 의해 설정된 주기마다 호출된다. 이 메서드를 통해 플레이어의 체력이 회복되고, 체력이 최대치가 되면 자동으로 멈추게(QuitRestingMode() 호출) 설정되어 있다.
- QuitRestingMode(): 타이머를 멈추고, 게임 모드를 변경한다.


### ⚔️ 던전 관련 기능
- DisplayDungeon()
- EnteringDungeon()
- FightEnd()
- LevelControl()


### ✨ 기타 기능
- WritingItem() : 인벤토리와 상점 등에서 아이템을 목록으로 보여줄 때 쓰인다. 기본적으로 아이템의 이름, 종류, 효과, 그리고 설명을 담고 있는데, 쓰일 때 뒤에 추가적인 내용들이 붙는다.
- CheckVailidInput()
- AnswerClear()
---
## Character
---
## Item
---
## Store
---
## Dungeon



