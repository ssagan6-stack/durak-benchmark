namespace DurakBenchmark;

internal class IKB : IPlayer
{
    private List<SCard> hand = new();

    public string GetName() => "IKB";
    public int GetCount() => hand.Count;
    public void AddToHand(SCard card)
{
    Suits trump = MTable.GetTrump().Suit;
    if (needSorted)
        hand.Add(card);
    else
    {
        int index = 0;
        while (index < hand.Count && IsStronger(card, hand[index]))
            index++;
        hand.Insert(index, card);
    }
}
    bool needSorted = true;
    private int turnCount = 0;
    public void ShowHand()
    {
        Console.WriteLine("Hand " + GetName());
        foreach (var c in hand) { MTable.ShowCard(c); Console.Write(MTable.Separator); }
        Console.WriteLine();
    }

    // Атака: вернуть одну или несколько карт для хода
    public List<SCard> LayCards()
    {
        if (needSorted) Sort();
        turnCount++;
        List<SCard> lay = new List<SCard>();
        lay.Add(hand[0]);
        hand.RemoveAt(0);
        return lay;
    }

    private bool IsStronger(SCard a, SCard b)
    {
    Suits trump = MTable.GetTrump().Suit;
    if (a.Suit == trump && b.Suit != trump)
        return true;
    else if (a.Suit != trump && b.Suit == trump)
        return false;
    else return a.Rank > b.Rank;
    }
    private void Sort()
    {
    for (int i = 0; i < hand.Count - 1; i++)
        for (int j = 0; j < hand.Count - 1 - i; j++)
            if (IsStronger(hand[j], hand[j + 1]))
                (hand[j], hand[j + 1]) = (hand[j + 1], hand[j]);
    needSorted = false;
    }
    // Защита: true = отбился, false = берёт карты
    public bool Defend(List<SCardPair> table)
{
    if (needSorted) Sort();
    Suits trump = MTable.GetTrump().Suit;

    for (int i = 0; i < table.Count; i++)
    {
        SCardPair pair = table[i];
        if (pair.Beaten) continue;

        SCard attack = pair.Down;
        int chosenIndex = -1;

        // Ищем первую карту той же масти и старше атаки
        for (int j = 0; j < hand.Count; j++)
        {
            if (hand[j].Suit == attack.Suit && IsStronger(hand[j], attack))
            {
                chosenIndex = j;
                break;
            }
        }

        // Если не нашли и атака не козырь - берем первый козырь
        if (chosenIndex == -1 && attack.Suit != trump)
        {
            for (int j = 0; j < hand.Count; j++)
            {
                if (hand[j].Suit == trump)
                {
                    chosenIndex = j;
                    break;
                }
            }
        }

        if (chosenIndex == -1)
            return false;

        pair.SetUp(hand[chosenIndex], trump);
        table[i] = pair;
        hand.RemoveAt(chosenIndex);
    }

    return true;
}

    // Подкидывание: true = подкинул карту, false = пас
    public bool AddCards(List<SCardPair> table)
{
    if (needSorted) Sort();

    List<int> ranksOnTable = new List<int>();

    // собираем все достоинства на столе
    foreach (SCardPair pair in table)
    {
        ranksOnTable.Add(pair.Down.Rank);

        if (pair.Beaten)
            ranksOnTable.Add(pair.Up.Rank);
    }

    // ищем карту для подкидывания
    for (int i = 0; i < hand.Count; i++)
    {
        if (ranksOnTable.Contains(hand[i].Rank))
        {
            table.Add(new SCardPair(hand[i]));
            hand.RemoveAt(i);
            return true;
        }
    }

    return false;
}
}
