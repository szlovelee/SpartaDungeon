# SpartaDungeon

## 구조 정리

## Program

### ⭐️ 기본 세팅
- restingTimer는 '휴식하기'에서 쓰이는 객체
- GameState는 휴식 모드와 플레이모드를 전환하는 데 쓰이는 데이터 타입 / status 변수에 값이 할당되어 게임 상태를 체크한다.
- InventoryMode는 아이템 정렬 방식을 정하는 데 쓰이는 데이터 타입 / organize 변수에 할당되어 인벤토리 정렬 모드를 체크한다.
- GameDataSetting()
- DisplayGameIntro(): 입력값에 따라 상태보기, 인벤토리, 상점, 휴식, 던전 관련 기능을 하는 메서드를 호출. 휴식과 던전 입장의 경우는 조건에 따라 선택이 불가능 할 수 있다.


### 🫀 상태보기 관련 기능

- DisplayMyInfo(): 플레이어의 기본적인 속성을 보여주는 메서드. 


### 💼 인벤토리 관련 기능
- DisplayInventory(): 인벤토리의 기본 화면을 보여주는 역할을 하는 메서드. 입력값에 따라 추가적인 작업을 위한 메서드 호출이 가능하다.
- EquipmentManager(): 추가적인 기능 중 '장착 관리'를 담당하는 메서드
- Organize(): 추가적인 기능 중 '아이템 정렬 설정'을 담당하는 메서드. 아이템 정렬 모드를 변경하고 SortItems()를 호출한다.
- SortItems(): 실제로 아이템을 정럴하는 메서드. Organize() 뿐만 아니라 아이템 구매와 아이템 장착 등의 상황에서도 호출되어 인벤토리 정렬모드(organize)에 따라 다른 정렬이 실행된다.
- ItemUpgrade(): 추가적인 기능 중 '아이템 강화'를 담당하는 메서드. 아이템 중 강화 가능한 (구매한 아이템 중 무기와 방어구 / 레벨 5 미만) 아이템을 정렬하여 입력값에 따라 해당 아이템을 강화하도록 한다. 아이템 강화에 따라 아이템의 효과가 변경된다. 레벨에 따라 강화 금액이 달라진다. (아이템 원가의 0.1배, 0.2배, 0.5배, 1배)
- EffectManager(): 아이템이 가지고 있는 효과 지수를 플레이어의 공격력 혹은 방어력에 적용시키는 작업을 하는 메서드. 플레이어가 아이템을 착용 혹은 해제, 소모, 강화할 때 해당 내역이 플레이어의 상태에 반영되도록 한다.


### 🏠 상점 관련 기능
- DisplayStore()
- ItemBuy()
- ItemSell()


### 💤 휴식 관련 기능
- Resting()
- StartRestingMode()
- QuiteRestingMode()


### ⚔️ 던전 관련 기능
- DisplayDungeon()
- EnteringDungeon()
- FightEnd()
- LevelControl()


### ✨ 기타 기능
- WritingItem()
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



