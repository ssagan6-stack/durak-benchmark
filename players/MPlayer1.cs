namespace DurakBenchmark;

internal class MPlayer1 : IPlayer
{
    {
        private string name = "Sasha";
        private List<SCard> hand = new List<SCard>();       // карты на руке

        public string GetName()
        {
            return name;
        }

        public int GetCount()
        {
            return hand.Count;
        }
        public void AddToHand(SCard card)
        {
            hand.Add(card);
        }

        // Сделать ход (первый)
        public List<SCard> LayCards()
        {
            List<SCard> result = new List<SCard>();
            if (hand.Count == 0) return result;

            SCard trumpCard = MTable.GetTrump();
            Suits trump = trumpCard.Suit;

            // Ищем минимальную не-козырную карту
            int bestIndex = -1;
            int bestRank = int.MaxValue;

            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].Suit != trump && hand[i].Rank < bestRank)
                {
                    bestRank = hand[i].Rank;
                    bestIndex = i;
                }
            }
            // Если нет некозырных берём минимальный козырь
            if (bestIndex == -1)
            {
                bestRank = int.MaxValue;
                for (int i = 0; i < hand.Count; i++)
                {
                    if (hand[i].Rank < bestRank)
                    {
                        bestRank = hand[i].Rank;
                        bestIndex = i;
                    }
                }
            }


            if (bestIndex == -1 || bestIndex >= hand.Count) return result;
            result.Add(hand[bestIndex]);
            hand.RemoveAt(bestIndex);
            return result;
        }

        // Отбиться.
        // На вход подается набор карт на столе, часть из них могут быть уже покрыты
        public bool Defend(List<SCardPair> table)
        {
            SCard trumpCard = MTable.GetTrump();
            Suits trump = trumpCard.Suit;
            for (int i = 0; i < table.Count; i++)
            {
                SCardPair pair = table[i];
                if (pair.Beaten) continue; // уже покрыта

                // Ищем минимальную карту которой можно побить
                int bestIndex = -1;
                int bestRank = int.MaxValue;

                for (int j = 0; j < hand.Count; j++)
                {
                    SCard candidate = hand[j];

                    // Проверяем бьёт ли кандидат через SetUp
                    SCardPair test = new SCardPair(pair.Down);
                    bool canBeat = test.SetUp(candidate, trump);

                    if (canBeat && candidate.Rank < bestRank)
                    {
                        bestRank = candidate.Rank;
                        bestIndex = j;
                    }
                }

                if (bestIndex == -1)
                    return false; // нечем бить

                // Бьём карту на столе
                pair.SetUp(hand[bestIndex], trump);
                table[i] = pair;
                hand.RemoveAt(bestIndex);
            }

            return true;
        }


        // Подбросить карты
        // На вход подаются карты на столе
        public bool AddCards(List<SCardPair> table)
        {
            if (hand.Count == 0) return false;

            // Собираем ранги карт которые уже на столе
            List<int> tableRanks = new List<int>();
            foreach (SCardPair pair in table)
            {
                if (!tableRanks.Contains(pair.Down.Rank))
                    tableRanks.Add(pair.Down.Rank);
                if (pair.Beaten && !tableRanks.Contains(pair.Up.Rank))
                    tableRanks.Add(pair.Up.Rank);
            }

            bool added = false;

            // Ищем карту в руке с подходящим рангом
            for (int i = hand.Count - 1; i >= 0; i--)
            {
                if (tableRanks.Contains(hand[i].Rank))
                {
                    table.Add(new SCardPair(hand[i]));
                    hand.RemoveAt(i);
                    added = true;
                    break; // подкидываем по одной
                }
            }

            return added;
        }


        // Вывести в консоль карты на руке
        public void ShowHand()
        {
            Console.WriteLine("Hand " + name);
            foreach (SCard card in hand)
            {
                MTable.ShowCard(card);
                Console.Write(MTable.Separator);
            }
            Console.WriteLine();
        }
    }
}
