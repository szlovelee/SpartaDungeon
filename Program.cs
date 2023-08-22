using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Numerics;

internal class Program
{
    private static Character player;

    static void Main(string[] args)
    {
        GameDataSetting();
        DisplayGameIntro();
    }

    static void GameDataSetting()
    {
        // 캐릭터 정보 세팅
        player = new Character("Chad", "전사", 1, 10, 5, 100, 1500);

        // 아이템 정보 세팅
        Item ironArmor = new Item("무쇠 갑옷", "방어력      +5", Item.ItemType.Shield, "무쇠로 만들어져 튼튼한 갑옷입니다.", 1, 0, false);
        Item oldSword = new Item("낡은 검", "공격력      +2", Item.ItemType.Weapon, "쉽게 볼 수 있는 낡은 검입니다.", 1, 0, false);

    player.PossessedItems = new List<Item> { ironArmor, oldSword };
    }

    static void DisplayGameIntro()  //시작화면
    {
        Console.Clear();

        Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
        Console.WriteLine("이곳에서 전전으로 들어가기 전 활동을 할 수 있습니다.");
        Console.WriteLine();
        Console.WriteLine("1. 상태 보기");
        Console.WriteLine("2. 인벤토리");
        Console.WriteLine("3. 상점");
        Console.WriteLine("4. 던전 입장");

        Console.WriteLine();
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(1, 4);
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
                DisplayDungeon();
                break;
        }
    }

    static void DisplayMyInfo()     // 상태보기
    {
        Console.Clear();

        Console.WriteLine("상태보기");
        Console.WriteLine("캐릭터의 정보르 표시합니다.");
        Console.WriteLine();
        Console.WriteLine($"Lv.{player.Level}");
        Console.WriteLine($"{player.Name}({player.Job})");
        Console.WriteLine($"공격력 : {player.Atk + player.AddAtk}");
        Console.WriteLine($"방어력 : {player.Def + player.AddDef}");
        Console.WriteLine($"체력   : {player.CurrentHp} / {player.Hp}");
        Console.WriteLine($"Gold   : {player.Gold} G");
        Console.WriteLine();
        Console.WriteLine("0. 나가기");
        Console.WriteLine();

        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 0);
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

        Console.WriteLine("인벤토리");
        Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        Console.WriteLine();

        Console.WriteLine($"체력 {player.CurrentHp} / {player.Hp}");
        if (player.AddAtk != 0) Console.WriteLine($"공격력 {player.Atk} + {player.AddAtk}");
        else Console.WriteLine($"공격력 {player.Atk}");
        if (player.AddDef != 0) Console.WriteLine($"방어력 {player.Def} + {player.AddDef}");
        else Console.WriteLine($"방어력 {player.Def}");
        Console.WriteLine();



        Console.WriteLine("[아이템 목록]");


        foreach (Item item in player.PossessedItems)
        {
            Console.Write("- ");
            if (item.Equipped) Console.Write("[E]");
            else Console.Write("   ");
            WritingItem(item);
            Console.SetCursorPosition(86, Console.CursorTop -1);
            if (item.Level != null) Console.WriteLine($" Lv. {item.Level} ");
            else Console.WriteLine(" (소모품) ");
        }

        Console.WriteLine();
        Console.WriteLine("1. 장착 관리");
        Console.WriteLine("0. 나가기");
        Console.WriteLine();

        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 1);
        switch (input)
        {
            case 0:
                DisplayGameIntro();
                break;
            case 1:
                EquipmentManager();
                break;
        }
    }

    static void EquipmentManager()
    {
        Console.Clear();

        Console.WriteLine("인벤토리 - 장착 관리");
        Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
        Console.WriteLine();

        Console.WriteLine($"체력 {player.CurrentHp} / {player.Hp}");
        if (player.AddAtk != 0) Console.WriteLine($"공격력 {player.Atk} + {player.AddAtk}");
        else Console.WriteLine($"공격력 {player.Atk}");
        if (player.AddDef != 0) Console.WriteLine($"방어력 {player.Def} + {player.AddDef}");
        else Console.WriteLine($"방어력 {player.Def}");
        Console.WriteLine();

        Console.WriteLine("[아이템 목록]");

        int count = 1;
        foreach (Item item in player.PossessedItems)
        {
            Console.Write($"{count++}.");
            if (item.Equipped) Console.Write("[E]");
            else Console.Write("   ");
            WritingItem(item);
            Console.SetCursorPosition(86, Console.CursorTop - 1);
            if (item.Level != null) Console.WriteLine($" Lv. {item.Level} ");
            else Console.WriteLine(" (소모품) ");
        }

        Console.WriteLine();
        Console.WriteLine($"1 ~ {player.PossessedItems.Count}. 해당 아이템 장착 및 소모");

        Console.WriteLine("0. 나가기");
        Console.WriteLine();

        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, player.PossessedItems.Count);
        if (input == 0)
        {
            DisplayInventory();
        }
        else
        {

            switch (player.PossessedItems[input - 1].Type)
            {
                case Item.ItemType.Weapon:
                    if (player.PossessedItems[input - 1].Equipped == false)
                    {
                        player.AddAtk = int.Parse(player.PossessedItems[input - 1].Effect.ToString().Split("+")[1]);
                        int index1 = player.PossessedItems.FindIndex(i => i.Equipped && i.Type == Item.ItemType.Weapon);
                        if (index1 != -1)
                        {
                            player.PossessedItems[index1].Equipped = false;
                        }
                        player.PossessedItems[input - 1].Equipped = true;
                    }
                    else
                    {
                        player.AddAtk = 0;
                        player.PossessedItems[input - 1].Equipped = false;
                    }
                    
                    break;

                case Item.ItemType.Shield:
                    if (player.PossessedItems[input - 1].Equipped == false)
                    {
                        player.AddDef = int.Parse(player.PossessedItems[input - 1].Effect.ToString().Split("+")[1]);
                        int index2 = player.PossessedItems.FindIndex(i => i.Equipped && i.Type == Item.ItemType.Shield);
                        if (index2 != -1)
                        {
                            player.PossessedItems[index2].Equipped = false;
                        }
                        player.PossessedItems[input - 1].Equipped = true;
                    }
                    else
                    {
                        player.AddDef = 0;
                        player.PossessedItems[input - 1].Equipped = false;
                    }

                    break;

                case Item.ItemType.Food:
                    int addedHp = int.Parse(player.PossessedItems[input - 1].Effect.ToString().Split("+")[1]);
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
                        player.PossessedItems.RemoveAt(input - 1);
                    }
                    break;

                case Item.ItemType.Potion:
                    int AddHp = int.Parse(player.PossessedItems[input - 1].Effect.ToString().Split("+")[1]);
                    if (player.CurrentHp == player.Hp)
                    {
                        player.CurrentHp += AddHp;
                    }
                    player.Hp += AddHp;
                    player.PossessedItems.RemoveAt(input - 1);
                    break;
            }
            EquipmentManager();
        }
    }

  
    static void DisplayStore()
    {
        Console.Clear();

        Console.WriteLine("상점");
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.WriteLine();

        Console.WriteLine("[보유 골드]");
        Console.WriteLine($"{player.Gold} G");
        Console.WriteLine();

        Console.WriteLine("[상품]");

        foreach (Item item in Store.StoreItems)
        {
            Console.Write("-    ");
            WritingItem(item);
            Console.SetCursorPosition(86, Console.CursorTop - 1);
            if (player.PossessedItems.IndexOf(item) != -1) Console.WriteLine(" 구매 완료 ");
            else Console.WriteLine($" {item.Price} G ");
            


        }

        Console.WriteLine();
        Console.WriteLine("1. 아이템 구매");
        Console.WriteLine("2. 아이템 판매");
        Console.WriteLine("0. 나가기");
        Console.WriteLine();

        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 2);
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

    static void ItemBuy()
    {
        Console.Clear();

        Console.WriteLine("상점 - 아이템 구매");
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.WriteLine();

        Console.WriteLine("[보유 골드]");
        Console.WriteLine($"{player.Gold} G");
        Console.WriteLine();

        Console.WriteLine("[상품]");

        int count = 1;
        foreach (Item item in Store.StoreItems)
        {
            Console.Write("-  ");
            Console.Write($"{count++}.");
            WritingItem(item);
            Console.SetCursorPosition(86, Console.CursorTop - 1);
            if (player.PossessedItems.IndexOf(item) != -1) Console.WriteLine(" 구매 완료 ");
            else Console.WriteLine($" {item.Price} G ");
        }

        Console.WriteLine();
        Console.WriteLine("1 ~ 12. 해당 아이템 구매");
        Console.WriteLine("0. 나가기");
        Console.WriteLine();

        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 12);
        if (input == 0) DisplayStore();
        else if (player.PossessedItems.IndexOf(Store.StoreItems[input - 1]) != -1)
        {
            AnswerClear();
            Console.WriteLine("이미 구매한 아이템입니다.");
            Thread.Sleep(2000);

        }
        else if (Store.StoreItems[input - 1].Price <= player.Gold)
        {
            AnswerClear();
            Console.WriteLine("구매를 완료했습니다.");
            player.PossessedItems.Add(Store.StoreItems[input - 1]);
            player.Gold -= Store.StoreItems[input - 1].Price;
            Thread.Sleep(2000);

        }
        else if (Store.StoreItems[input - 1].Price > player.Gold)
        {
            AnswerClear();
            Console.WriteLine("Gold가 부족합니다.");
            Thread.Sleep(2000);

        }
        ItemBuy();
    }

    static void ItemSell()
    {
        Console.Clear();

        Console.WriteLine("상점 - 아이템 판매");
        Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
        Console.WriteLine();

        Console.WriteLine("[보유 골드]");
        Console.WriteLine($"{player.Gold} G");
        Console.WriteLine();

        Console.WriteLine("[보유 아이템]");

        if (player.PossessedItems.Count <= 2)
        {
            Console.WriteLine("판매 가능한 아이템이 없습니다.");
            Console.WriteLine();
        }
        else
        {
            int count = 1;
            foreach (Item item in player.PossessedItems)
            {
                if (item.Price == 0) continue;
                Console.Write("-  ");
                Console.Write($"{count++}.");
                WritingItem(item);
                Console.SetCursorPosition(86, Console.CursorTop - 1);
                Console.WriteLine($" {(int)(item.Price * 0.85)} G ");
            }

            Console.WriteLine();
            if (player.PossessedItems.Count == 3) Console.WriteLine("1. 해당 아이템 판매");
            else Console.WriteLine($"1 ~ {player.PossessedItems.Count - 2}. 해당 아이템 판매");
        }

        Console.WriteLine("0. 나가기");
        Console.WriteLine();

        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, player.PossessedItems.Count - 2);
        if (input == 0) DisplayStore();
        else
        {
            AnswerClear();
            Console.WriteLine($"{player.PossessedItems[input + 1].Name}을(를) 판매하시겠습니까? (판매: 0, 취소: 1)");
            Console.WriteLine();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">> ");

            int answer = CheckValidInput(0, 1);
            if (answer == 1)
            {
                ItemSell();
            }
            else
            {
                if (player.PossessedItems[input +  1].Equipped == true)
                {
                    if (player.PossessedItems[input + 1].Type == Item.ItemType.Weapon) player.AddAtk = 0;
                    else if (player.PossessedItems[input + 1].Type == Item.ItemType.Shield) player.AddDef = 0;
                }

                player.Gold += (int)(player.PossessedItems[input + 1].Price * 0.85);
                player.PossessedItems.RemoveAt(input + 1);

                AnswerClear();
                Console.WriteLine("판매가 완료되었습니다.");
                Thread.Sleep(2000);
            }

            ItemSell();
        }

    }

  
    static void WritingItem(Item item)
    {
        Console.Write("               |");
        Console.SetCursorPosition(5, Console.CursorTop);
        Console.Write($" {item.Name} ");
        Console.SetCursorPosition(21, Console.CursorTop);
        Console.Write("        |");
        Console.SetCursorPosition(21, Console.CursorTop);
        Console.Write($" {item.Type.ToString()} ");
        Console.SetCursorPosition(30, Console.CursorTop);
        Console.Write("                 |");
        Console.SetCursorPosition(30, Console.CursorTop);
        Console.Write($" {item.Effect}");
        Console.SetCursorPosition(48, Console.CursorTop);
        Console.Write("                                    |");
        Console.SetCursorPosition(48, Console.CursorTop);
        Console.WriteLine($" {item.Desc} ");
    }

    static void DisplayDungeon()
    {
        Console.Clear();

        Console.WriteLine("던전 입장");
        Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
        Console.WriteLine();

        Console.WriteLine($"나의 방어력: {player.Def + player.AddDef}");
        Console.WriteLine();

        Console.WriteLine("1. 난이도   ★☆☆   | 권장 방어력: 15");
        Console.WriteLine("2. 난이도   ★★☆   | 권장 방어력: 40");
        Console.WriteLine("3. 난이도   ★★★   | 권장 방어력: 55");
        Console.WriteLine("0. 나가기");
        Console.WriteLine();

        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, 3);

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

    static void EnteringDungeon(int input)
    {
        Console.Clear();

        Console.WriteLine("던전 입장");
        Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
        Console.WriteLine();

        Console.WriteLine($"나의 방어력: {player.Def + player.AddDef}");
        Console.WriteLine();

        Dungeon dungeon = new Dungeon(input);

        AnswerClear();
        Console.WriteLine(dungeon.DungeonInfo);
        Console.WriteLine();
        Console.Write("던전에 입장하시겠습니까?");

        if (dungeon.RecDef > player.Def + player.AddDef) Console.WriteLine("    (현재 방어력이 낮습니다.)");
        else Console.WriteLine();

        Console.WriteLine("1. 입장");
        Console.WriteLine("0. 나가기");
        Console.WriteLine();
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int answer = CheckValidInput(0, 1);
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

    static void FightEnd(Dungeon dungeon)
    {
        Console.Clear();



        Console.WriteLine("던전 탐험 결과");

        if (dungeon.DungeonFight(player))
        {
            Console.WriteLine("던전 클리어");
            Console.WriteLine();
            Console.WriteLine("[탐험 보상]");
            Console.WriteLine($"기본 보상 {dungeon.Reward} G");
            Console.WriteLine($"추가 보상 {dungeon.AdditionalReward} G");
        }
        else
        {
            Console.WriteLine("던전 실패");

        }

        Console.WriteLine();
        Console.WriteLine($"현재 체력 {player.CurrentHp} / {player.Hp}");

        Console.WriteLine();
        Console.WriteLine("0. 나가기");

        Console.WriteLine();
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0,0);

        if (input == 0) DisplayGameIntro();
    }


    static int CheckValidInput(int min, int max)
    {
        while (true)
        {
            string input = Console.ReadLine();

            bool parseSuccess = int.TryParse(input, out var ret);
            if (parseSuccess)
            {
                if (ret >= min && ret <= max)
                    return ret;
            }

            AnswerClear();
            Console.Write("잘못된 입력입니다.");
            Thread.Sleep(2000);
            AnswerClear();
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">> ");
        }
    }

    static void AnswerClear()
    {
        Console.SetCursorPosition(0, Console.CursorTop - 2);

        for (int i = 0; i < 3; i++)  // 3줄 지우기
        {
            Console.Write(new string(' ', Console.WindowWidth)); // 현재 줄 지우기
        }

        Console.SetCursorPosition(0, Console.CursorTop - 2);
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
    }
}

public class Item
{
    public enum ItemType
    {
        Weapon,
        Shield,
        Food,
        Potion
    }

    public string Name { get; }
    public string Effect { get; set; }
    public ItemType Type { get; }     // 아이템 분류
    public string Desc { get; }     // 아이템 설명
    public int? Level { get; set; }
    public int Price { get; }
    public bool Equipped { get; set; }

    public Item(string name, string effect, ItemType type, string desc, int? level, int price, bool equipped)
    {
        Name = name;
        Effect = effect;
        Type = type;
        Desc = desc;
        Level = level;
        Price = price;
        Equipped = equipped;
    }
}

class Store
{
    static Item invincibleShield = new Item("무적의 방패", "방어력      +10", Item.ItemType.Shield, "고대 전사가 들었다 전해지는 방패", 1, 1000, false);
    static Item dragonGuard = new Item("용의 수호", "방어력      +30", Item.ItemType.Shield, "용의 비늘로 만든 강력한 방패", 1, 5000, false);
    static Item celestialArmor = new Item("천상의 갑주", "방어력      +50", Item.ItemType.Shield, "천계의 갑옷", 1, 10000, false);

    static Item windbladeBow = new Item("검풍의 활", "공격력      +5", Item.ItemType.Weapon, "고대 전사가 들었다 전해지는 방패", 1, 1000, false);
    static Item dragonSword = new Item("비룡의 검", "공격력      +10", Item.ItemType.Weapon, "용의 비늘로 만든 강력한 방패", 1, 5000, false);
    static Item heavenlySpear = new Item("천무의 창", "공격력      +20", Item.ItemType.Weapon, "천계의 갑옷", 1, 10000, false);

    static Item potato = new Item("감자", "체력 회복   +10", Item.ItemType.Food, "맛은 없지만 필요한 비상식량", null, 200, false);
    static Item chicken = new Item("백숙", "체력 회복   +30", Item.ItemType.Food, "가성비 좋은 닭요리", null, 500, false);
    static Item boyang = new Item("삼선보양탕", "체력 회복   +70", Item.ItemType.Food, "체력 회복에 좋은 보양탕", null, 1000, false);   

    static Item sweetJuice = new Item("달달주스", "체력 최대치 +2", Item.ItemType.Potion, "힘을 나게 해주는 신비한 주스", null, 500, false);
    static Item midPotion = new Item("중급 보약", "체력 최대치 +5", Item.ItemType.Potion, "가성비 좋은 닭요리", null, 800, false);
    static Item toxicMushroom = new Item("독버섯 추출액", "체력 최대치 +10", Item.ItemType.Potion, "독성에 의해 신체가 강화된다", null, 1500, false);

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

    Random random = new Random();

    public Dungeon(int stage)
    {
        switch (stage)
        {
            case 1:
                RecDef = 15;
                Reward = 1000;
                DungeonInfo = "난이도   ★☆☆   | 권장 방어력: 15";
                break;
            case 2:
                RecDef = 40;
                Reward = 1700;
                DungeonInfo = "난이도   ★★☆   | 권장 방어력: 40";
                break;
            case 3:
                RecDef = 55;
                Reward = 2500;
                DungeonInfo = "난이도   ★★★   | 권장 방어력: 55";
                break;
        }
    }


    public bool DungeonFight(Character player)
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

        DungeonResult(player);
        return true;
    }


    public void DungeonResult(Character player)
    {
        int defGap = (player.Def + player.AddDef) - RecDef;
        player.CurrentHp -= random.Next(20 - defGap, 36 - defGap);

        AdditionalReward = Reward * random.Next(player.Atk + player.AddAtk, (player.Atk + player.AddAtk) * 2 + 1) / 100;
        player.Gold += Reward + AdditionalReward;
    }
}