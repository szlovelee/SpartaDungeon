using System.Reflection.Emit;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;
using System;
using System.IO;
using Newtonsoft.Json;



internal class Program
{
    private static Character player;
    static System.Timers.Timer restingTimer;

    enum GameState  // 게임 중 모드와 휴식 모드 (status로 관리)
    {
        Playing,
        Resting
    }

    enum InventoryMode  // 인벤토리 정렬 모드 (organize로 관리)
    {
        ItemLevel,
        ItemType
    }

    static InventoryMode organize; 
    static GameState status;

    static void Main(string[] args)
    {
        GameDataSetting();
        DisplayGameIntro();
    }

    static void GameDataSetting()
    {
        // 캐릭터 정보 세팅
        player = new Character("Chad", "전사", 1, 10, 5, 100, 1500);
        status = GameState.Playing;

        // 아이템 정보 세팅
        Item ironArmor = new Item("무쇠 갑옷", 5, Item.ItemType.Shield, "무쇠로 만들어져 튼튼한 갑옷입니다.", 1, 0, false);
        Item oldSword = new Item("낡은 검", 2, Item.ItemType.Weapon, "쉽게 볼 수 있는 낡은 검입니다.", 1, 0, false);

        player.PossessedItems = new List<Item> { ironArmor, oldSword };
        SortItems();
    }

    static void DisplayGameIntro()  //시작화면
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("  -------------------------------------------------------  ");
        Console.WriteLine(" |                                                       | ");
        Console.WriteLine(" |        스파르타 마을에 오신 여러분 환영합니다.        | ");
        Console.WriteLine(" |  이곳에서 전전으로 들어가기 전 활동을 할 수 있습니다. | ");
        Console.WriteLine(" |                                                       | ");
        Console.WriteLine("  -------------------------------------------------------  ");
        Console.ResetColor();
        Console.WriteLine();

        Console.Write("현재 모드 : ");
        if (status == GameState.Playing) Console.ForegroundColor = ConsoleColor.DarkGreen;
        else Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(status.ToString());
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("1. 상태 보기");
        Console.WriteLine("2. 인벤토리");
        Console.WriteLine("3. 상점");
        Console.WriteLine("4. 휴식하기");
        Console.WriteLine("5. 던전 입장");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(1, 5);
        Console.ResetColor();
        switch (input)
        {
            case 1:
                DisplayMyInfo();
                break;

            case 2:
                DisplayInventory();
                break;
            case 3:
                DisplayStore();
                break;
            case 4:
                if (player.CurrentHp == player.Hp)
                {
                    AnswerClear();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("현재 체력이 최대치입니다.");
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    DisplayGameIntro();
                }
                else
                Resting(); break;
            case 5:
                if (status == GameState.Resting)
                {
                    AnswerClear();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("휴식 모드입니다.");
                    Console.WriteLine("던전 입장을 원하시면 플레이 모드로 바꿔주세요.");
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    DisplayGameIntro();
                }
                else if (player.CurrentHp == 0)
                {
                    AnswerClear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("체력이 부족합니다. 휴식을 취하거나 음식을 섭취한 후 다시 시도하세요.");
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    DisplayGameIntro();
                }
                else DisplayDungeon();
                break;
        }
    }


    static void DisplayMyInfo()     // 상태보기
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ----------  ");
        Console.WriteLine(" | 상태보기 | ");
        Console.WriteLine("  ----------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("캐릭터의 정보르 표시합니다.");
        Console.ResetColor();
        Console.WriteLine();
        Console.WriteLine($"Lv.{player.Level}");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"exp({player.CurrentExp} / {player.MaxExp})");
        Console.ResetColor();
        Console.WriteLine("----------------------------------");
        Console.WriteLine($"{player.Name}({player.Job})");
        Console.WriteLine();
        Console.Write("체력   : ");
        if (player.CurrentHp >= 50) Console.ForegroundColor = ConsoleColor.Green;
        else Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(player.CurrentHp);
        Console.ResetColor();
        Console.WriteLine($" / {player.Hp}");
        Console.WriteLine($"공격력 : {player.Atk + player.AddAtk}");
        Console.WriteLine($"방어력 : {player.Def + player.AddDef}");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"Gold   : {player.Gold} G");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 0);
        Console.ResetColor();
        switch (input)
        {
            case 0:
                DisplayGameIntro();
                break;
        }
    }


    static void DisplayInventory()  // 인벤토리
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ----------  ");
        Console.WriteLine(" | 인벤토리 | ");
        Console.WriteLine("  ----------  ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        Console.ResetColor();
        Console.WriteLine();

        // 아이템에 의해 변동 가능한 상태 디스플레이
        Console.Write("체력   : ");
        if (player.CurrentHp >= 50) Console.ForegroundColor = ConsoleColor.Green;
        else Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(player.CurrentHp);
        Console.ResetColor();
        Console.WriteLine($" / {player.Hp}");
        if (player.AddAtk != 0) Console.WriteLine($"공격력 {player.Atk} + {player.AddAtk}");
        else Console.WriteLine($"공격력 {player.Atk}");
        if (player.AddDef != 0) Console.WriteLine($"방어력 {player.Def} + {player.AddDef}");
        else Console.WriteLine($"방어력 {player.Def}");
        Console.WriteLine();


        Console.WriteLine("[아이템 목록]");
        Console.WriteLine();

        foreach (Item item in player.PossessedItems)    //WritingItem 메소드에 추가적으로 표기해야 할 내용 더해서 하나씩 출력
        {
            Console.Write("- ");
            if (item.Equipped) Console.Write("[E]");
            else Console.Write("   ");
            WritingItem(item);
            Console.SetCursorPosition(88, Console.CursorTop -1);
            if (item.Level != null) Console.WriteLine($" Lv. {item.Level} ");
            else Console.WriteLine($" 보유: {item.Count} 개 ");
        }
        Console.WriteLine();


        Console.WriteLine();
        Console.WriteLine("1. 장착 관리");
        Console.WriteLine("2. 아이템 정렬 설정");
        Console.WriteLine("3. 아이템 강화");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 3);
        Console.ResetColor();

        switch (input)
        {
            case 0:
                DisplayGameIntro();
                break;
            case 1:
                EquipmentManager();
                break;
            case 2:
                Organize();
                break;
            case 3:
                ItemUpgrade();
                break;
        }
    }


    static void EquipmentManager()  // 장착 관리
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ----------------------  ");
        Console.WriteLine(" | 인벤토리 - 장착 관리 | ");
        Console.WriteLine("  ----------------------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        Console.ResetColor();
        Console.WriteLine();

        // 아이템에 의해 변동 가능한 상태 디스플레이
        // (아이템에 의한 변동 내역이 잘 보이도록 아이템에 의해 더해진 추가 공격력 및 방어력 따로 표시)
        Console.Write("체력   : ");
        if (player.CurrentHp >= 50) Console.ForegroundColor = ConsoleColor.Green;
        else Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(player.CurrentHp);
        Console.ResetColor();
        Console.WriteLine($" / {player.Hp}");
        if (player.AddAtk != 0) Console.WriteLine($"공격력 {player.Atk} + {player.AddAtk}");
        else Console.WriteLine($"공격력 {player.Atk}");
        if (player.AddDef != 0) Console.WriteLine($"방어력 {player.Def} + {player.AddDef}");
        else Console.WriteLine($"방어력 {player.Def}");
        Console.WriteLine();

        Console.WriteLine("[아이템 목록]");
        Console.WriteLine();

        int count = 1;
        foreach (Item item in player.PossessedItems)  //WritingItem + 추가 정보 
        {
            Console.Write($"{count++}.");
            if (item.Equipped) Console.Write("[E]");
            else Console.Write("   ");
            WritingItem(item);
            Console.SetCursorPosition(88, Console.CursorTop - 1);
            if (item.Level != null) Console.WriteLine($" Lv. {item.Level} ");
            else Console.WriteLine($" 보유: {item.Count} 개 ");
        }
        Console.WriteLine();


        Console.WriteLine();
        Console.WriteLine($"1 ~ {player.PossessedItems.Count}. 해당 아이템 장착 및 소모");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, player.PossessedItems.Count);
        Console.ResetColor();

        if (input == 0)
        {
            DisplayInventory();
        }
        else
        {
            EffectManager(player.PossessedItems[input - 1], false); // 아이템 효과를 적용시키는 메서드 호출 (파라미터가 이따구인 이유는 이 메서드 자체가 더럽기 때문)
            SortItems();
            EquipmentManager();
        }
    }  


    static void Organize()  // 아이템 정렬 설정
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine(" -----------------------------  ");
        Console.WriteLine(" | 인벤토리 - 아이템 정렬 설정 | ");
        Console.WriteLine("  -----------------------------  ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine($"현재 모드: {organize.ToString()}");
        Console.WriteLine();

        Console.WriteLine();        // 현재 아이템 정렬 모드에 따라 다르게 출력 (이 부분 더 좋은 방법 있는지 찾아보기,,)
        if (organize == InventoryMode.ItemType) Console.WriteLine("1. 레벨순으로 정렬하기");
        else Console.WriteLine("1. 종류별로 정렬하기");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 1);
        Console.ResetColor();

        if (input == 0)
        {
            DisplayInventory();
            return;
        }
        else if (organize == InventoryMode.ItemType)        // 여기서도 현재 아이템 정렬 모드에 따라 다르게 작용하기 때문에 더 좋은 방법 찾아보기
        {
            organize = InventoryMode.ItemLevel;
            SortItems();
            DisplayInventory();
            return;
        }
        else
        {
            organize = InventoryMode.ItemType;
            SortItems();
            DisplayInventory();
            return;
        }
    } 


    static void SortItems() // 아이템 정렬 실행. 정렬 모드 변경 및 아이템 구매 / 강화 시 호출됨
    {
        if (organize == InventoryMode.ItemType)
        {
            player.PossessedItems = player.PossessedItems.OrderByDescending(i => i.Equipped)
                .ThenBy(i => i.Type)
                .ThenByDescending(i => i.Effect)
                .ThenByDescending(i => i.Level)
                .ToList();
        }
        else
        {
            player.PossessedItems = player.PossessedItems.OrderByDescending(i => i.Equipped)
               .ThenByDescending(i => i.Level)
               .ThenBy(i => i.Type)
               .ThenByDescending(i => i.Effect)
               .ToList();
        }
    }


    static void ItemUpgrade()   // 아이템 강화 
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ------------------------  ");
        Console.WriteLine(" | 인벤토리 - 아이템 강화 | ");
        Console.WriteLine("  ------------------------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        Console.ResetColor();
        Console.WriteLine();

        Console.Write("체력   : ");
        if (player.CurrentHp >= 50) Console.ForegroundColor = ConsoleColor.Green;
        else Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(player.CurrentHp);
        Console.ResetColor();
        Console.WriteLine($" / {player.Hp}");
        if (player.AddAtk != 0) Console.WriteLine($"공격력 {player.Atk} + {player.AddAtk}");
        else Console.WriteLine($"공격력 {player.Atk}");
        if (player.AddDef != 0) Console.WriteLine($"방어력 {player.Def} + {player.AddDef}");
        else Console.WriteLine($"방어력 {player.Def}");
        Console.WriteLine();

        Console.WriteLine("[보유 골드]");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"{player.Gold} G");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("[강화 가능한 아이템]");
        Console.WriteLine();

        List<Item> tempList = new List<Item>();
        List<int> upgradeFee = new List<int>();

        foreach (Item item in player.PossessedItems)
        {
            if (item.Level == null || item.Level == 5 || item.Price == 0) continue;     // 소모품, 최고레벨 아이템, 기본템 제외

            switch (item.Level)
            {
                case 1:
                    upgradeFee.Add((int)(item.Price * 0.1f));
                    break;
                case 2:
                    upgradeFee.Add((int)(item.Price * 0.2f));

                    break;
                case 3:
                    upgradeFee.Add((int)(item.Price * 0.5f));

                    break;
                case 4:
                    upgradeFee.Add(item.Price);
                    break;
            }   // 해당 아이템의 현재 레벨을 바탕으로 판매 가격 설정

            Console.Write($"{tempList.Count + 1}.");
            if (item.Equipped) Console.Write("[E]");
            else Console.Write("   ");
            WritingItem(item);
            Console.SetCursorPosition(88, Console.CursorTop - 1);
            Console.Write("          |");
            Console.SetCursorPosition(88, Console.CursorTop);
            Console.Write($" Lv. {item.Level} ");
            Console.SetCursorPosition(99, Console.CursorTop);
            Console.WriteLine($" {upgradeFee[tempList.Count]} G ");

            tempList.Add(item);            
        }

        Console.WriteLine();        // 강화 가능한 아이템 수에 따른 다른 출력
        if (tempList.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("강화 가능한 아이템이 없습니다.");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
        }
        else if (tempList.Count == 1)
        {
            Console.WriteLine();
            Console.WriteLine("1. 해당 아이템 강화");
        }
        else
        {
            Console.WriteLine();
            Console.WriteLine($"1 ~ {tempList.Count}. 해당 아이템 강화");
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, tempList.Count);
        Console.ResetColor();

        if (input == 0)
        {
            DisplayInventory();
            return;
        }
        else
        {
            if (player.Gold >= upgradeFee[input - 1])       // 강화 가능한 경우 (재화 충분)
            {
                Item selected = tempList[input - 1];

                int effect = (int)(selected.Effect * 1.2f); // 레벨업 시 현재 효과의 1.2배가 되도록 값 저장

                
                selected.Effect = effect;
                ++selected.Level;

                player.Gold -= upgradeFee[input - 1];

                EffectManager(selected, true);  // 아이템의 변동 내역이 플레이어에게 적용되도록

                SortItems();

                Console.WriteLine();

                AnswerClear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("강화가 완료되었습니다.");
                Console.ResetColor();
                Thread.Sleep(2000);

            }
            else 
            {
                AnswerClear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Gold가 부족합니다.");
                Console.ResetColor();
                Thread.Sleep(2000);
            }
            ItemUpgrade();
        }
    }


    static void EffectManager(Item item, bool isUpgrade)    // 아이템 효과 적용 (진짜 제발 수정)
    {// isUpgrade는 이 메서드가 호출되는 경로가 강화에서인지 아닌지 판별..
        switch (item.Type)
        {
            case Item.ItemType.Weapon: 
                if (item.Equipped == true && !isUpgrade)
                {
                    player.AddAtk = 0;
                    item.Equipped = false;
                }
                else 
                {
                    if (!isUpgrade)
                    {
                        Item equipped = player.PossessedItems.Find(i => i.Equipped == true && i.Type == Item.ItemType.Weapon);  // item과 동일한 타입의 아이템 중 장착된 것
                        if (equipped  != null)
                        {
                            equipped.Equipped = false;
                        }
                        item.Equipped = true;
                    }
                    player.AddAtk = item.Effect;
                }

                break;

            case Item.ItemType.Shield:
                if(item.Equipped == true && !isUpgrade)
                {
                    player.AddDef = 0;
                    item.Equipped = false;
                }
                else
                {
                    player.AddDef = item.Effect;
                    if (!isUpgrade)
                    {
                        Item equipped = player.PossessedItems.Find(i => i.Equipped  == true && i.Type == Item.ItemType.Shield);  // item과 동일한 타입의 아이템 중 장착된 것
                        if (equipped != null)
                        {
                            equipped.Equipped = false;
                        }
                        item.Equipped = true;
                    }
                }
                break;

            case Item.ItemType.Food:
                int addedHp = item.Effect;
                if (player.CurrentHp == player.Hp)
                {
                    AnswerClear();
                    Console.Write("이미 체력이 최대치입니다.");
                    Thread.Sleep(2000);
                }
                else
                {
                    if (player.CurrentHp + addedHp > player.Hp) player.CurrentHp = player.Hp;
                    else player.CurrentHp += addedHp;
                    --item.Count;
                    if (item.Count == 0) player.PossessedItems.Remove(item);
                }
                break;

            case Item.ItemType.Potion:
                int AddHp = item.Effect;
                if (player.CurrentHp == player.Hp)
                {
                    player.CurrentHp += AddHp;
                }
                player.Hp += AddHp;
                --item.Count;
                if (item.Count == 0) player.PossessedItems.Remove(item);
                break;
        }
    }


    static void DisplayStore()          // 상점
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ------  ");
        Console.WriteLine(" | 상점 | ");
        Console.WriteLine("  ------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("[보유 골드]");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"{player.Gold} G");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("[상품]");
        Console.WriteLine();

        foreach (Item item in Store.StoreItems)
        {
            Console.Write("-    ");
            WritingItem(item);
            Console.SetCursorPosition(88, Console.CursorTop - 1); 
            Console.WriteLine($" {item.Price} G ");

        }
        Console.WriteLine();

        Console.WriteLine();
        Console.WriteLine("1. 아이템 구매");
        Console.WriteLine("2. 아이템 판매");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 2);
        Console.ResetColor();

        switch (input)
        {
            case 0:
                DisplayGameIntro();
                break;
            case 1:
                ItemBuy();
                break;
            case 2:
                ItemSell();
                break;
        }
    }


    static void ItemBuy()   // 아이템 구매
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  --------------------  ");
        Console.WriteLine(" | 상점 - 아이템 구매 | ");
        Console.WriteLine("  --------------------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("[보유 골드]");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"{player.Gold} G");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("[상품]");
        Console.WriteLine();

        int count = 1;
        foreach (Item item in Store.StoreItems)
        {
            Console.Write("-    ");            
            WritingItem(item);
            Console.SetCursorPosition(3, Console.CursorTop - 1);
            Console.Write($"{count++}.");
            Console.SetCursorPosition(88, Console.CursorTop);

            if (player.PossessedItems.Find(i => i.Name == item.Name) != null && item.Level != null) Console.WriteLine(" 구매 완료 ");
            else Console.WriteLine($" {item.Price} G ");
        }
        Console.WriteLine();

        Console.WriteLine();
        Console.WriteLine("1 ~ 12. 해당 아이템 구매");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 12);
        Console.ResetColor();

       
        if (input == 0) DisplayStore();
        else
        {
            Item sellected = Store.StoreItems[input - 1];
            if (player.PossessedItems.IndexOf(sellected) != -1 && sellected.Level != null) // 구매했는데 소모품이 아닐 경우 (중복구매 방지)
            {
                AnswerClear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("이미 구매한 아이템입니다.");
                Thread.Sleep(2000);
            }
            else if (sellected.Price <= player.Gold)
            {
                AnswerClear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("구매를 완료했습니다.");

                Item item = player.PossessedItems.Find(i => i.Name == sellected.Name);
                if (item == null)
                {
                    player.PossessedItems.Add(DeepClone<Item>(sellected));
                    item = player.PossessedItems.Find(i => i.Name == sellected.Name);
                }

                if (item.Level == null)
                {
                    ++item.Count;
                }
                player.Gold -= sellected.Price;
                SortItems();

                Thread.Sleep(2000);

            }
            else if (sellected.Price > player.Gold)
            {
                AnswerClear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Gold가 부족합니다.");
                Console.ResetColor();
                Thread.Sleep(2000);

            }
            Console.ResetColor();
            ItemBuy();
        }
    }


    static void ItemSell()  // 아이템 판매
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  --------------------  ");
        Console.WriteLine(" | 상점 - 아이템 판매 | ");
        Console.WriteLine("  --------------------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("[보유 골드]");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"{player.Gold} G");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine("[보유 아이템]");
        Console.WriteLine();

        List<Item> Sellable = new List<Item>();     // 판매 가능한 아이템 리스트 생성

        foreach (Item sellable in player.PossessedItems)
        {
            if (sellable.Price != 0) Sellable.Add(sellable);
        }

        if (Sellable.Count == 0)
        {
            Console.WriteLine("판매 가능한 아이템이 없습니다.");
            Console.WriteLine();
        }
        else
        {
            int count = 1;
            foreach (Item item in Sellable)
            {
                Console.Write("-  ");
                Console.Write($"{count++}.");
                WritingItem(item);
                Console.SetCursorPosition(88, Console.CursorTop - 1);
                Console.Write("             |");
                Console.SetCursorPosition(88, Console.CursorTop);
                if (item.Level != null) Console.Write($" Lv. {item.Level} ");
                else Console.Write($" 보유: {item.Count} 개 ");

                switch (item.Level)
                {
                    case 2:
                        item.SellignPrice = (int)(item.Price * 0.95);
                        break;
                    case 3:
                        item.SellignPrice = item.Price;
                        break;
                    case 4:
                        item.SellignPrice = (int)(item.Price * 1.2);
                        break;
                    case 5:
                        item.SellignPrice = (int)(item.Price * 1.7);
                        break;
                    default:
                        item.SellignPrice = (int)(item.Price * 0.85);
                        break;
                }

                Console.SetCursorPosition(102, Console.CursorTop);
                Console.WriteLine($" {item.SellignPrice} G ");
            }
            Console.WriteLine();


            Console.WriteLine();
            if (player.PossessedItems.Count == 3) Console.WriteLine("1. 해당 아이템 판매");
            else Console.WriteLine($"1 ~ {player.PossessedItems.Count - 2}. 해당 아이템 판매");
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, Sellable.Count);
        Console.ResetColor();

        if (input == 0) DisplayStore();
        else
        {
            AnswerClear();
            AnswerClear();
            AnswerClear();
            AnswerClear();
            Console.WriteLine();
            Console.Write($"{Sellable[input - 1].Name}을(를) 판매하시겠습니까?");
            Console.ForegroundColor = ConsoleColor.Red;
            if (Sellable[input - 1].Level > 1) Console.WriteLine(" 강화 완료된 아이템입니다.");
            else Console.WriteLine();
            Console.ResetColor();
            
            Console.WriteLine("1. 판매");
            Console.WriteLine("0. 취소");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">> ");

            int answer = CheckValidInput(0, 1);
            if (answer == 0)
            {
                ItemSell();
            }
            else
            {
                Item sellected = Sellable[input - 1]; 

                player.Gold += sellected.SellignPrice;


                if (sellected.Level == null)
                {
                    --sellected.Count;
                    if (sellected.Count <= 0) player.PossessedItems.Remove(sellected);
                }
                else
                {
                    if (sellected.Equipped == true)     // 장착 중인 아이템 판매 시 추가 방어력 및 추가 공격력 리셋
                    {
                        if (sellected.Type == Item.ItemType.Weapon) player.AddAtk = 0;
                        else if (sellected.Type == Item.ItemType.Shield) player.AddDef = 0;
                    }
                    player.PossessedItems.Remove(sellected);
                }

                AnswerClear();
                AnswerClear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("판매가 완료되었습니다.");
                Console.ResetColor();
                Thread.Sleep(2000);
            }

            ItemSell();
        }

    }


    static void WritingItem(Item item)  // 아이템 목록 나열
    {
        Console.Write("                 |        |                  |                                    |");
        Console.SetCursorPosition(7, Console.CursorTop);
        Console.Write($" {item.Name}");
        Console.SetCursorPosition(23, Console.CursorTop);
        Console.Write($" {item.Type.ToString()}");
        Console.SetCursorPosition(32, Console.CursorTop);
        Console.Write($" {item.EffectType}");
        Console.SetCursorPosition(44, Console.CursorTop);
        Console.Write($" + {item.Effect}");
        Console.SetCursorPosition(51, Console.CursorTop);
        Console.WriteLine($" {item.Desc}");
    }


    static void Resting()   // 휴식 모드 설정
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ----------  ");
        Console.WriteLine(" | 휴식하기 | ");
        Console.WriteLine("  ----------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("휴식 모드에서 통해 체력을 회복할 수 있습니다.");
        Console.ResetColor();
        Console.WriteLine();
        Console.Write("현재 모드 : ");
        if (status == GameState.Playing) Console.ForegroundColor = ConsoleColor.DarkGreen;
        else Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(status.ToString());
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        if (status == GameState.Playing) Console.WriteLine("1. 휴식하기");
        else Console.WriteLine("1. 휴식 중지");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 1);
        Console.ResetColor();

        if (input == 0) DisplayGameIntro();
        else if (status == GameState.Playing) StartRestingMode();
        else QuitRestingMode();

    }


    static void StartRestingMode()  // 휴식 시작
    {
        status = GameState.Resting;
        restingTimer = new System.Timers.Timer(30000);
        restingTimer.Elapsed += OnTimerEvent;
        restingTimer.Start();
        if (player.CurrentHp == player.Hp) DisplayGameIntro();
        else Resting();
    }


    static void QuitRestingMode()   // 휴식 종료
    {
        restingTimer.Stop();
        status = GameState.Playing;
        Resting();
    }


    static void OnTimerEvent(Object source, System.Timers.ElapsedEventArgs e)   // 휴식 중 실행되는 내용 (체력 회복)
    {
        ++player.CurrentHp;
        if (player.CurrentHp == player.Hp) QuitRestingMode();
    }


    static void DisplayDungeon()    // 던전 입장
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  -----------  ");
        Console.WriteLine(" | 던전 입장 | ");
        Console.WriteLine("  -----------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine($"나의 방어력: {player.Def + player.AddDef}");
        Console.WriteLine();

        Console.WriteLine("1. 난이도   ★☆☆   | 권장 방어력: 15");
        Console.WriteLine("2. 난이도   ★★☆   | 권장 방어력: 40");
        Console.WriteLine("3. 난이도   ★★★   | 권장 방어력: 55");
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 3);
        Console.ResetColor();

        switch (input)
        {
            case 0:
                DisplayGameIntro();
                break;
            case 1:
            case 2:
            case 3:
                EnteringDungeon(input);
                break;
        }

    }


    static void EnteringDungeon(int input)  // 던전 입장 
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  -----------  ");
        Console.WriteLine(" | 던전 입장 | ");
        Console.WriteLine("  -----------  ");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine($"나의 방어력: {player.Def + player.AddDef}");
        Console.WriteLine();

        Dungeon dungeon = new Dungeon(input);

        AnswerClear();
        Console.WriteLine(dungeon.DungeonInfo);
        Console.WriteLine();
        Console.Write("던전에 입장하시겠습니까?");

        Console.ForegroundColor = ConsoleColor.Red;
        if (dungeon.RecDef > player.Def + player.AddDef) Console.WriteLine("    현재 방어력이 낮습니다.");
        else Console.WriteLine();
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.WriteLine("1. 입장");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int answer = CheckValidInput(0, 1);
        Console.ResetColor();

        switch (answer)
        {
            case 0:
                DisplayDungeon();
                break;
            case 1:
                Console.Clear();
                Console.WriteLine("전투 중...");
                Thread.Sleep(5000);

                FightEnd(dungeon);

                break;

        }
    }


    static void FightEnd(Dungeon dungeon)   // 던전 탐험 결과 
    {
        Console.Clear();

        Console.BackgroundColor = ConsoleColor.Magenta;
        Console.WriteLine("  ----------------  ");
        Console.WriteLine(" | 던전 탐험 결과 | ");
        Console.WriteLine("  ----------------  ");
        Console.ResetColor();
        Console.WriteLine();

        if (dungeon.DungeonFight(player))
        {
            LevelControl(dungeon);
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine("던전 클리어");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("[탐험 보상]");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"기본 보상 {dungeon.Reward} G");
            Console.WriteLine($"추가 보상 {dungeon.AdditionalReward} G");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"exp +{dungeon.RewardExp}");
            if (player.CurrentExp == 0)
            {
                Console.WriteLine();
                Console.BackgroundColor = ConsoleColor.Green;
                Console.WriteLine($"* 레벨업! Lv.{player.Level -1} -> Lv.{player.Level}");
            }
            Console.ResetColor();
        }
        else
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("던전 실패");
            Console.ResetColor();

        }

        Console.WriteLine();
        Console.Write("현재 체력 ");
        if (player.CurrentHp >= 50) Console.ForegroundColor = ConsoleColor.Green;
        else Console.ForegroundColor = ConsoleColor.Red;
        Console.Write(player.CurrentHp);
        Console.ResetColor();
        Console.WriteLine($" / {player.Hp}");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("0. 나가기");
        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("------------------------------");
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0,0);
        Console.ResetColor();

        if (input == 0) DisplayGameIntro();
    }


    static void LevelControl(Dungeon dungeon)   // 경험치 계산 & 레벨에 반영
    {
        player.CurrentExp += dungeon.RewardExp;
        if (player.CurrentExp >= player.MaxExp)
        {
            ++player.Level;
            player.CurrentExp = 0;
            player.MaxExp = player.Level * 100;
            player.Atk += 1;
            player.Def += 2;
        }
    }


    static int CheckValidInput(int min, int max)    // 입력값 확인
    {
        while (true)
        {
            string input = Console.ReadLine();
            Console.ResetColor();

            bool parseSuccess = int.TryParse(input, out var ret);
            if (parseSuccess)
            {
                if (ret >= min && ret <= max)
                    return ret;
            }

            AnswerClear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("잘못된 입력입니다.");
            Console.ResetColor();
            Thread.Sleep(2000);

            AnswerClear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("------------------------------");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">> ");
        }
    }

    static void AnswerClear()       // 입력된 내용 지우기
    {
        Console.SetCursorPosition(0, Console.CursorTop - 2);

        for (int i = 0; i < 3; i++)  // 3줄 지우기
        {
            Console.Write(new string(' ', Console.WindowWidth)); // 현재 줄 지우기
        }

        Console.SetCursorPosition(0, Console.CursorTop - 2);
    }

    static Item DeepClone<Item>(Item item)  //  아이템 깊은 복사 (상점에서 구매 시 호출)
    {
        return JsonConvert.DeserializeObject<Item>(JsonConvert.SerializeObject(item));
    }

}


public class Character
{
    public List<Item> PossessedItems;

    public string Name { get; }
    public string Job { get; }
    public int Level { get; set; }
    public int Atk { get; set; }
    public int Def { get; set; }
    public int Hp { get; set; }
    public int Gold { get; set; }
    public int AddAtk { get; set; }
    public int AddDef { get; set; }
    public int CurrentHp { get; set; }
    public int MaxExp { get; set; }
    public int CurrentExp { get; set; }


    public Character(string name, string job, int level, int atk, int def, int hp, int gold)
    {
        Name = name;
        Job = job;
        Level = level;
        Atk = atk;
        Def = def;
        Hp = hp;
        Gold = gold;
        CurrentHp = hp;
        MaxExp = level * 100;
        CurrentExp = 0;
    }
}

[Serializable]
public class Item
{
    public enum ItemType
    {
        Shield,
        Weapon,
        Food,
        Potion
    }

    public string Name { get; }
    public int Effect { get; set; }
    public ItemType Type { get; }     // 아이템 분류
    public string Desc { get; }     // 아이템 설명
    public int? Level { get; set; }
    public int Price { get; }
    public int SellignPrice { get; set; }
    public bool Equipped { get; set; }
    public int Count { get; set; }
    public string EffectType
    {
        get
        {
            switch (Type)
            {
                case ItemType.Weapon:
                    return "공격력";
                case ItemType.Shield:
                    return "방어력";
                case ItemType.Food:
                    return "체력 회복";
                case ItemType.Potion:
                    return "체력 최대치";
                default:
                    return "";
            }
        }
    }
    public Item(string name, int effect, ItemType type, string desc, int? level, int price, bool equipped)
    {
        Name = name;
        Effect = effect;
        Type = type;
        Desc = desc;
        Level = level;
        Price = price;
        Equipped = equipped;
        Count = 0;
    }
}

class Store
{
    static Item invincibleShield = new Item("무적의 방패", 10 , Item.ItemType.Shield, "고대 전사가 들었다 전해지는 방패", 1, 1000, false);
    static Item dragonGuard = new Item("용의 수호", 30, Item.ItemType.Shield, "용의 비늘로 만든 강력한 방패", 1, 5000, false);
    static Item celestialArmor = new Item("천상의 갑주", 50 , Item.ItemType.Shield, "천계의 갑옷", 1, 10000, false);

    static Item windbladeBow = new Item("검풍의 활", 5, Item.ItemType.Weapon, "바람을 타는 검처럼 화살을 쏘는 활", 1, 1000, false);
    static Item dragonSword = new Item("비룡의 검", 10, Item.ItemType.Weapon, "용의 비늘로 만든 강력한 방패", 1, 5000, false);
    static Item heavenlySpear = new Item("천무의 창", 20, Item.ItemType.Weapon, "천계의 갑옷", 1, 10000, false);

    static Item potato = new Item("감자", 10 , Item.ItemType.Food, "맛은 없지만 필요한 비상식량", null, 200, false);
    static Item chicken = new Item("백숙", 30, Item.ItemType.Food, "가성비 좋은 닭요리", null, 500, false);
    static Item boyang = new Item("삼선보양탕", 70, Item.ItemType.Food, "체력 회복에 좋은 보양탕", null, 1000, false);   

    static Item sweetJuice = new Item("달달주스", 2, Item.ItemType.Potion, "힘을 나게 해주는 신비한 주스", null, 500, false);
    static Item midPotion = new Item("중급 보약", 5, Item.ItemType.Potion, "가성비 좋은 닭요리", null, 800, false);
    static Item toxicMushroom = new Item("독버섯 추출액", 10, Item.ItemType.Potion, "독성에 의해 신체가 강화된다", null, 1500, false);

    public static Item[] StoreItems =
    {
        invincibleShield, dragonGuard, celestialArmor,
        windbladeBow, dragonSword, heavenlySpear,
        potato, chicken, boyang,
        sweetJuice, midPotion, toxicMushroom
    };
}




class Dungeon
{
    public int RecDef { get; set; }
    public int Reward { get; set; }
    public int AdditionalReward { get; set; }
    public string DungeonInfo { get; set; }
    public int RewardExp { get; set; }

    Random random = new Random();

    public Dungeon(int stage)
    {
        switch (stage)
        {
            case 1:
                RecDef = 15;
                Reward = 1000;
                DungeonInfo = "난이도   ★☆☆   | 권장 방어력: 15";
                RewardExp = 30;
                break;
            case 2:
                RecDef = 40;
                Reward = 1700;
                DungeonInfo = "난이도   ★★☆   | 권장 방어력: 40";
                RewardExp = 50;
                break;
            case 3:
                RecDef = 55;
                Reward = 2500;
                DungeonInfo = "난이도   ★★★   | 권장 방어력: 55";
                RewardExp = 70;
                break;
        }
    }


    public bool DungeonFight(Character player)      // 던전 승패 결정
    {
        if (player.Def + player.AddDef < RecDef)
        {
            int winProb = random.Next(1, 6);
            if (winProb == 1 || winProb == 2)
            {
                player.CurrentHp = player.CurrentHp / 2;
                return false;
            }       
        }
        bool success = true;
        DungeonResult(player, ref success);
        return success;
    }


    public void DungeonResult(Character player, ref bool success)   // 리워드 결정
    {
        int defGap = (player.Def + player.AddDef) - RecDef;
        player.CurrentHp -= random.Next(20 - defGap, 36 - defGap);
        if (player.CurrentHp <= 0)
        {
            player.CurrentHp = 0;       // 전투에 의해  체력 고갈 시 패배
            success = false;
            return;
        }

        AdditionalReward = Reward * random.Next(player.Atk + player.AddAtk, (player.Atk + player.AddAtk) * 2 + 1) / 100;
        player.Gold += Reward + AdditionalReward;
    }
}