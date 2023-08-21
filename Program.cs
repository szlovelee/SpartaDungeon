using System;
using System.Reflection.Emit;
using System.Collections.Generic;

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
        player.PossessedItems.Add(Store.StoreItems[0]);
        player.PossessedItems.Add(Store.StoreItems[3]);
        player.PossessedItems.Add(Store.StoreItems[6]);
        player.PossessedItems.Add(Store.StoreItems[9]);

    }

    static void DisplayGameIntro()  //시작화면
    {
        Console.Clear();

        Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
        Console.WriteLine("이곳에서 전전으로 들어가기 전 활동을 할 수 있습니다.");
        Console.WriteLine();
        Console.WriteLine("1. 상태보기");
        Console.WriteLine("2. 인벤토리");
        Console.WriteLine();
        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(1, 2);
        switch (input)
        {
            case 1:
                DisplayMyInfo();
                break;

            case 2:
                DisplayInventory();
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
        Console.WriteLine($"체력   : {player.Hp}");
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
        }

        Console.WriteLine();
        Console.WriteLine($"1~{player.PossessedItems.Count}. 해당 아이템 장착 및 소모");
        Console.WriteLine("0. 나가기");
        Console.WriteLine();

        Console.WriteLine("원하시는 행동을 입력해주세요.");
        Console.Write(">> ");

        int input = CheckValidInput(0, player.PossessedItems.Count);
        if (input == 0)
        {
            DisplayGameIntro();
        }
        else
        {
            switch (player.PossessedItems[input - 1].Type)
            {
                case Item.ItemType.Weapon:
                    player.AddAtk = int.Parse(player.PossessedItems[input - 1].Effect.ToString().Split("+")[1]);
                    int index1 = player.PossessedItems.FindIndex(i => i.Equipped && i.Type == Item.ItemType.Weapon);
                    if (index1 != -1)
                    {
                        player.PossessedItems[index1].Equipped = false;
                    }
                    player.PossessedItems[input - 1].Equipped = true;
                    break;
                case Item.ItemType.Shield:
                    player.AddDef = int.Parse(player.PossessedItems[input - 1].Effect.ToString().Split("+")[1]);
                    int index2 = player.PossessedItems.FindIndex(i => i.Equipped && i.Type == Item.ItemType.Shield);
                    if (index2 != -1)
                    {
                        player.PossessedItems[index2].Equipped = false;
                    }
                    player.PossessedItems[input - 1].Equipped = true;
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
        Console.Write($" {item.Desc} ");
        Console.SetCursorPosition(86, Console.CursorTop);
        if (item.Level != null) Console.WriteLine($"Lv. {item.Level}");
        else Console.WriteLine();
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
    private static Item ironArmor = new Item("무쇠 갑옷", "방어력   +5", Item.ItemType.Shield, "무쇠로 만들어져 튼튼한 갑옷입니다.", 1, 0, false);
    private static Item oldSword = new Item("낡은 검", "공격력   +2", Item.ItemType.Weapon, "쉽게 볼 수 있는 낡은 검입니다.", 1, 0, false);

    public List<Item> PossessedItems = new List<Item> { ironArmor, oldSword };

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
    static Item invincibleShield = new Item("무적의 방패", "방어력   +10", Item.ItemType.Shield, "고대 전사가 들었다 전해지는 방패", 1, 1000, false);
    static Item dragonGuard = new Item("용의 수호", "방어력   +30", Item.ItemType.Shield, "용의 비늘로 만든 강력한 방패", 1, 5000, false);
    static Item celestialArmor = new Item("천상의 갑주", "방어력   +50", Item.ItemType.Shield, "천계의 갑옷", 1, 10000, false);

    static Item windbladeBow = new Item("검풍의 활", "공격력   +5", Item.ItemType.Weapon, "고대 전사가 들었다 전해지는 방패", 1, 1000, false);
    static Item dragonSword = new Item("비룡의 검", "공격력   +10", Item.ItemType.Weapon, "용의 비늘로 만든 강력한 방패", 1, 5000, false);
    static Item heavenlySpear = new Item("천무의 창", "공격력   +20", Item.ItemType.Weapon, "천계의 갑옷", 1, 10000, false);

    static Item potato = new Item("감자", "체력 회복  +10", Item.ItemType.Food, "맛은 없지만 필요한 비상식량", null, 200, false);
    static Item chicken = new Item("백숙", "체력 회복  +30", Item.ItemType.Food, "가성비 좋은 닭요리", null, 500, false);
    static Item boyang = new Item("삼선보양탕", "체력 회복  +70", Item.ItemType.Food, "체력 회복에 좋은 보양탕", null, 1000, false);

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

